using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

using Sqlite3DatabaseHandle = System.IntPtr;
using Sqlite3BackupHandle = System.IntPtr;
using Sqlite3Statement = System.IntPtr;

namespace CoreSharp.SQLite
{
	/// <summary>
	/// Represents a SQLite Command to be executed
	/// </summary>
	public partial class SQLiteCommand : IDisposable
	{
		SQLiteConnection _Conn;
		private List<(string Name, object Value)> _Parameters;

		/// <summary>
		/// Command Text
		/// </summary>
		public string CommandText { get; private set; } = string.Empty;

		/// <summary>
		/// Custom Parameter binder, if set - this connection will use this function instead
		/// of internal parameter binding code in this instance
		/// </summary>
		public Action<Sqlite3Statement> ParameterBinder;

		/// <summary>
		/// Whether this instance is a prepared statement and will not be disposed
		/// after execution. Beware that prepared statement can only be used by one thread at a time.
		/// </summary>
        public bool IsPreparedStatement { get; private set; }

		/// <summary>
		/// Whether Enums are converted to Text when binding parameter values
		/// </summary>
        public bool IsBindEnumAsText { get; private set; }

        public event Action<object> OnInstanceCreated = delegate { };

        static SQLiteCommand()
        {
			SQLiteCommand.BuildColumnReader();
			SQLiteCommand.BuildBinders();
		}

		public SQLiteCommand(SQLiteConnection conn, string commandText, bool prepared = false)
		{
			_Conn = conn;
			this.CommandText = commandText;
		}

		/// <summary>
		/// Sets a list of parameters to be bound to command, overriding existing list
		/// </summary>
		/// <param name="value"></param>
		public void SetParameters( bool enumAsText, params object[] values )
        {
			this.IsBindEnumAsText = enumAsText;
			_Parameters = values.Select(v => (Name: string.Empty, Value: v)).ToList();
		}

		/// <summary>
		/// Sets a parameter list to be bound to command, overriding existing list
		/// </summary>
		/// <param name="value">Tuples of parameter names and value</param>
		public void SetNamedParameters(bool enumAsText, params (string Name, object Value)[] parameters)
		{
			this.IsBindEnumAsText = enumAsText;
			_Parameters = parameters.ToList();
		}

		/// <summary>
		/// Sets a parameter list to be bound to command, overriding existing list
		/// </summary>
		/// <param name="value"></param>
		public void SetNamedParameters(bool enumAsText, IEnumerable<KeyValuePair<string, object>> values)
		{
			this.IsBindEnumAsText = enumAsText;
			_Parameters = values.Select(p => (Name: p.Key, Value: p.Value)).ToList();
		}

		#region Prepare Statement

		private Sqlite3Statement _PreparedStatement;

		/// <summary>
		/// Prepare a command
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		private Sqlite3Statement Prepare()
		{
			Sqlite3Statement stmt;

			if (this.IsPreparedStatement)
            {
                if (_PreparedStatement == IntPtr.Zero)
                {
					_PreparedStatement = SQLite3.Prepare2(_Conn.Handle, this.CommandText);
				}
				stmt = _PreparedStatement;
			}
			else
            {
				stmt = SQLite3.Prepare2(_Conn.Handle, this.CommandText);
			}

			// use custom parameter binder instead of our own
            if (this.ParameterBinder != null)
            {
				this.ParameterBinder(stmt);
				return stmt;
            }

            if (_Parameters?[0].Name != null)
			{
				foreach (var p in _Parameters)
				{
					var index = SQLite3.BindParameterIndex(stmt, p.Name);
					this.BindParameter(stmt, index, p.Value);
				}
			}
            else
			{
				int nextIdx = 1;
				foreach (var p in _Parameters)
				{
					var index = nextIdx++;
					this.BindParameter(stmt, index, p);
				}

			}

			return stmt;
		}

		private static readonly IntPtr _NegativePtr = new IntPtr(-1);
		private delegate void Binder(Sqlite3Statement stmt, int index, object value);
		private static readonly Dictionary<Type, Binder> _Binders = new();

		private static void BuildBinders()
        {
			Binder binder;
			binder = (stmt, index, value) => SQLite3.BindInt(stmt, index, Convert.ToInt32(value));

			_Binders[typeof(Int32)] = binder;
			_Binders[typeof(Byte)] = binder;
			_Binders[typeof(UInt16)] = binder;
			_Binders[typeof(SByte)] = binder;
			_Binders[typeof(Int16)] = binder;
			_Binders[typeof(Boolean)] = binder;

			binder = (stmt, index, value) => SQLite3.BindInt64(stmt, index, Convert.ToInt64(value));
			_Binders[typeof(UInt32)] = binder;
			_Binders[typeof(Int64)] = binder;

			_Binders[typeof(string)] = (stmt, index, value) => SQLite3.BindText(stmt, index, (string)value, -1, _NegativePtr);

			_Binders[typeof(TimeSpan)] = (stmt, index, value) => SQLite3.BindInt64(stmt, index, ((TimeSpan)value).Ticks);
			_Binders[typeof(DateTime)] = (stmt, index, value) => SQLite3.BindInt64(stmt, index, ((DateTime)value).Ticks);
			_Binders[typeof(DateTimeOffset)] = (stmt, index, value) => SQLite3.BindInt64(stmt, index, ((DateTimeOffset)value).UtcTicks);

			_Binders[typeof(byte[])] = (stmt, index, value) => SQLite3.BindBlob(stmt, index, (byte[])value, ((byte[])value).Length, _NegativePtr);

			_Binders[typeof(Guid)] = (stmt, index, value) => SQLite3.BindText(stmt, index, ((Guid)value).ToString(), 72, _NegativePtr);

			_Binders[typeof(Uri)] = (stmt, index, value) => SQLite3.BindText(stmt, index, ((Uri)value).ToString(), -1, _NegativePtr);

			_Binders[typeof(StringBuilder)] = (stmt, index, value) => SQLite3.BindText(stmt, index, ((StringBuilder)value).ToString(), -1, _NegativePtr);

			_Binders[typeof(UriBuilder)] = (stmt, index, value) => SQLite3.BindText(stmt, index, ((UriBuilder)value).ToString(), -1, _NegativePtr);
		}

		/// <summary>
		/// Bind Parameter to a statement
		/// </summary>
		/// <param name="stmt"></param>
		/// <param name="index"></param>
		/// <param name="value"></param>
		/// <param name="storeDateTimeAsTicks"></param>
		/// <param name="dateTimeStringFormat"></param>
		/// <param name="storeTimeSpanAsTicks"></param>
		private void BindParameter(Sqlite3Statement stmt, int index, object value)
		{
			if (value == null)
			{
				SQLite3.BindNull(stmt, index);
				return;
			}

			Binder binder;
            if (_Binders.TryGetValue( value.GetType(), out binder))
            {
				binder(stmt, index, value);
				return;
            }

            if (value.GetType().IsEnum)
			{
				if (this.IsBindEnumAsText)
				{
					SQLite3.BindText(stmt, index, value.ToString(), -1, _NegativePtr);
				}
                else
                {
					SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
				}
			}

			throw new NotSupportedException("Cannot bind type: " + Orm.GetType(value));
		}

		#endregion

		/// <summary>
		/// Execute query without result
		/// </summary>
		/// <returns></returns>
		public int ExecuteNonQuery()
		{
			if (_Conn.Trace)
			{
				_Conn.Tracer?.Invoke("Executing: " + this);
			}

			Sqlite3Statement stmt = IntPtr.Zero;
            try
            {
				var r = SQLite3.Result.OK;

				stmt = this.Prepare();
				r = SQLite3.Step(stmt);

				if (r == SQLite3.Result.Done)
				{
					int rowsAffected = SQLite3.Changes(_Conn.Handle);
					return rowsAffected;
				}
				else if (r == SQLite3.Result.Error)
				{
					string msg = SQLite3.GetErrmsg(_Conn.Handle);
					throw SQLiteException.New(r, msg);
				}
				else if (r == SQLite3.Result.Constraint)
				{
					if (SQLite3.ExtendedErrCode(_Conn.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
					{
						throw NotNullConstraintViolationException.New(r, SQLite3.GetErrmsg(_Conn.Handle));
					}
				}

				throw SQLiteException.New(r, SQLite3.GetErrmsg(_Conn.Handle));
			}
            finally
			{
				this.Finalize(stmt);
			}
			
		}

		/// <summary>
		/// Get Result from Query. Optionally specify that fields are ordered per static mapping.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="staticFieldList">If true, the function will assume field list follows the
		/// static mapping and will skip column order checks. Used internally by the library.</param>
		/// <returns></returns>
		public IEnumerable<T> ExecuteDeferredQuery<T>(bool staticFieldList = false)
		{
			var map = _Conn.GetMapping<T>();

			if (_Conn.Trace)
			{
				_Conn.Tracer?.Invoke("Executing Query: " + this);
			}

			Sqlite3Statement stmt = IntPtr.Zero;
			try
			{
				stmt = this.Prepare();

				string[] cols = null;
                if (staticFieldList)
				{
					cols = new string[SQLite3.ColumnCount(stmt)];
					for (int i = 0; i < cols.Length; i++)
					{
						cols[i] = SQLite3.ColumnName16(stmt, i);
					}
				}

				while (SQLite3.Step(stmt) == SQLite3.Result.Row)
				{
					T obj = map.ReadStatementResult(stmt, cols);
					this.OnInstanceCreated(obj);
					yield return obj;
				}

			}
			finally
			{
				this.Finalize(stmt);
			}
		}

		/// <summary>
		/// Get First Column of First Row in the result. Or specified the column index to get
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T ExecuteScalar<T>(int colIndex = 0)
		{
			return this.ExecuteDeferredScalars<T>(colIndex).FirstOrDefault();
		}

		/// <summary>
		/// Get a single column from query result
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="columnIndex">index of column to read</param>
		/// <returns></returns>
		public IEnumerable<T> ExecuteDeferredScalars<T>(int columnIndex = 0)
		{
			var stmt = this.Prepare();
			try
			{
				var totalColumns = SQLite3.ColumnCount(stmt);
				if (totalColumns < 1)
				{
					throw new InvalidOperationException("QueryScalars should return at least one column");
				}

                if (columnIndex > totalColumns - 1)
				{
					throw new ArgumentOutOfRangeException("columnIndex is not within range of returned columns");
				}

				while (SQLite3.Step(stmt) == SQLite3.Result.Row)
				{
					var val = this.ReadCol<T>(stmt, columnIndex);
					if (val == null)
					{
						yield return default(T);
					}
					else
					{
						yield return (T)val;
					}
				}
			}
			finally
			{
				this.Finalize(stmt);
			}
		}

        #region Dynamic Column Readers

        private delegate object SQLite3ColumnReader(Sqlite3Statement s, int i);
		private static Dictionary<Type, SQLite3ColumnReader> _Reader = new();

		private static void BuildColumnReader()
        {
			SQLite3ColumnReader reader;
			
			reader = (s, i) => SQLite3.ColumnInt(s, i);
			_Reader[typeof(int)] = reader;
			_Reader[typeof(bool)] = reader;
			_Reader[typeof(byte)] = reader;
			_Reader[typeof(UInt16)] = reader;
			_Reader[typeof(Int16)] = reader;
			_Reader[typeof(sbyte)] = reader;

			reader = (s, i) => SQLite3.ColumnInt64(s, i);
			_Reader[typeof(TimeSpan)] = (s, i) => TimeSpan.FromTicks((long)reader(s, i));
			_Reader[typeof(DateTime)] = (s, i) => new DateTime((long)reader(s, i));
			_Reader[typeof(DateTimeOffset)] = (s, i) => new DateTimeOffset((long)reader(s, i), TimeSpan.Zero);

			reader = (s, i) => SQLite3.ColumnDouble(s, i);
			_Reader[typeof(double)] = reader;
			_Reader[typeof(double)] = reader;

			reader = (s, i) => SQLite3.ColumnString(s, i);
			_Reader[typeof(string)] = reader;
			_Reader[typeof(Guid)] = (s,i) => new Guid( (string)reader(s,i) );
			_Reader[typeof(Uri)] = (s, i) => new Uri((string)reader(s, i));
			_Reader[typeof(StringBuilder)] = (s, i) => new StringBuilder((string)reader(s, i));
			_Reader[typeof(UriBuilder)] = (s, i) => new UriBuilder((string)reader(s, i));
		}

		private object ReadCol<T>(Sqlite3Statement stmt, int index)
		{
			var colType = SQLite3.ColumnType(stmt, index);
            if (colType == SQLite3.ColType.Null)
            {
				return null;
            }

			var clrType = typeof(T);

			if (clrType.IsEnum)
			{
				if (colType == SQLite3.ColType.Text)
				{
					var value = SQLite3.ColumnString(stmt, index);
					return Enum.Parse(clrType, value.ToString(), true);
				}
				else
				{
					return SQLite3.ColumnInt(stmt, index);
				}
			}

			SQLite3ColumnReader reader;
            if (_Reader.TryGetValue(clrType, out reader))
            {
				return reader(stmt, index);
            }

			throw new NotSupportedException("Don't know how to read " + clrType);
		}

		#endregion

		/// <summary>
		/// Gets string representation of this commdn
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var parts = new string[1 + _Parameters.Count];
			parts[0] = CommandText;
			var i = 1;
			foreach (var b in _Parameters)
			{
				parts[i] = string.Format("  {0}: {1}", i - 1, b.Value);
				i++;
			}
			return string.Join(Environment.NewLine, parts);
		}

		/// <summary>
		/// Finalize the statement, if specified statement is the prepared statement, it will not be finalized
		/// </summary>
		/// <param name="stmt"></param>
		private void Finalize(Sqlite3Statement stmt)
		{
            if (stmt == IntPtr.Zero)
            {
				return;
            }

			// skip finalizing as requested
            if (this.IsPreparedStatement && stmt == _PreparedStatement)
            {
				return;
            }

			SQLite3.Finalize(stmt);
		}

        public void Dispose()
		{
			if (_PreparedStatement == IntPtr.Zero)
			{
				return;
			}

			SQLite3.Finalize(_PreparedStatement);
		}

		~SQLiteCommand()
        {
			this.Dispose();
        }

	}

}
