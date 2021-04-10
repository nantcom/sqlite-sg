// MIT License

// Copyright (c) 2021 NantCom Co., Ltd.
// by Jirawat Padungkijjanont (jirawat[at]nant.co)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sqlite3Statement = System.IntPtr;

namespace NC.SQLite
{
	public interface IReader
    {
		T ReadCurrentRow<T>(int columnIndex);

		T ReadCurrentRow<T>(string column);
	}

    /// <summary>
    /// Represents a SQLite Command to be executed
    /// </summary>
    public partial class SQLiteCommand : IDisposable, IReader
	{
		SQLiteConnection _Conn;
		private List<(string Name, object Value)> _Parameters;

		/// <summary>
		/// Whether to automatically finalize the statement
		/// </summary>
        public bool AutoDispose { get; set; }

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
		/// Whether Enums are converted to Text when binding parameter values
		/// </summary>
        public bool IsBindEnumAsText { get; private set; }

        public event Action<object> OnInstanceCreated = delegate { };

        static SQLiteCommand()
        {
			SQLiteCommand.BuildColumnReader();
			SQLiteCommand.BuildBinders();
		}

		public SQLiteCommand(SQLiteConnection conn, string commandText, bool autoDispose = true)
		{
			_Conn = conn;
			this.AutoDispose = autoDispose;
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
			if (_PreparedStatement == IntPtr.Zero)
			{
				_PreparedStatement = SQLite3.Prepare2(_Conn.Handle, this.CommandText);
				_ColNameCache = null;
			}

			if (_Parameters?.Count == 0)
			{
				return _PreparedStatement;
			}

			// use custom parameter binder instead of our own
			if (this.ParameterBinder != null)
            {
				this.ParameterBinder(_PreparedStatement);
				return _PreparedStatement;
            }

			// Named parameter or index based parameter
            if (string.IsNullOrEmpty(_Parameters[0].Name))
			{
				int nextIdx = 1;
				foreach (var p in _Parameters)
				{
					var index = nextIdx++;
					this.BindParameter(_PreparedStatement, index, p.Value);
				}
			}
            else
			{
				foreach (var p in _Parameters)
				{
					var index = SQLite3.BindParameterIndex(_PreparedStatement, p.Name);
					this.BindParameter(_PreparedStatement, index, p.Value);
				}
			}

			return _PreparedStatement;
		}

		private static readonly IntPtr _NegativePtr = new IntPtr(-1);
		private delegate void Binder(Sqlite3Statement stmt, int index, object value);
		private static readonly Dictionary<Type, Binder> _Binders = new();

		private static Binder _BindInt = (stmt, index, value) => SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
		private static Binder _BindInt64 = (stmt, index, value) => SQLite3.BindInt64(stmt, index, Convert.ToInt64(value));
		private static Binder _BindDouble = (stmt, index, value) => SQLite3.BindDouble(stmt, index, Convert.ToDouble(value));

		private static void BuildBinders()
        {
			_Binders[typeof(Int32)] = _BindInt;
			_Binders[typeof(Byte)] = _BindInt;
			_Binders[typeof(UInt16)] = _BindInt;
			_Binders[typeof(SByte)] = _BindInt;
			_Binders[typeof(Int16)] = _BindInt;
			_Binders[typeof(Boolean)] = _BindInt;

			_Binders[typeof(UInt32)] = _BindInt64;
			_Binders[typeof(Int64)] = _BindInt64;

			_Binders[typeof(float)] = _BindDouble;
			_Binders[typeof(double)] = _BindDouble;

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

			throw new NotSupportedException("Cannot bind type: " + value.GetType().Name);
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

            try
            {
                Sqlite3Statement stmt = this.Prepare();

                var r = SQLite3.Result.OK;
                r = SQLite3.Step(stmt);


                if (r == SQLite3.Result.Done)
                {
                    int rowsAffected = SQLite3.Changes(_Conn.Handle);
                    return rowsAffected;
                }
                else if (r == SQLite3.Result.Error)
                {
                    string msg = SQLite3.GetErrmsg(_Conn.Handle);
                    throw new SQLiteException(r, msg);
                }
                else if (r == SQLite3.Result.Constraint)
                {
                    if (SQLite3.ExtendedErrCode(_Conn.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
                    {
                        throw new NotNullConstraintViolationException(r, SQLite3.GetErrmsg(_Conn.Handle));
                    }
                }

                throw new SQLiteException(r, SQLite3.GetErrmsg(_Conn.Handle));
            }
            finally
            {
				this.DisposePreparedStatement();
            }
		}

		/// <summary>
		/// Get Result from Query. Optionally specify that fields are ordered per static mapping.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="staticFieldList">If true, the function will assume field list follows the
		/// static mapping and will skip column order checks. Used internally by the library.</param>
		/// <returns></returns>
		internal IEnumerable<T> ExecuteDeferredQueryMapped<T>(ITableMapping<T> map)
		{
            try
            {
                Sqlite3Statement stmt = this.Prepare();
                while (SQLite3.Step(stmt) == SQLite3.Result.Row)
                {
                    T obj = map.ReadStatementResult(stmt);
                    this.OnInstanceCreated(obj);
                    yield return obj;
                }
            }
            finally
            {
				this.DisposePreparedStatement();
            }
		}

		/// <summary>
		/// Get Result from Query. Reader supply a function to read values from statement - this is the fastest
		/// reading if you know index and type of column in advance
		/// </summary>
		/// <typeparam name="T">Return Type</typeparam>
		/// <param name="reader">Function to read result from a statement</param>
		/// <returns></returns>
		public IEnumerable<T> ExecuteDeferredQuery<T>(Func<Sqlite3Statement,T> reader)
		{
            try
            {
                Sqlite3Statement stmt = this.Prepare();

                while (SQLite3.Step(stmt) == SQLite3.Result.Row)
                {
                    T obj = reader(stmt);
                    this.OnInstanceCreated(obj);
                    yield return obj;
                }
            }
            finally
            {
				this.DisposePreparedStatement();
            }
		}

		/// <summary>
		/// Get Result from Query. Reader supply a function to read values from statement
		/// </summary>
		/// <typeparam name="T">Return Type</typeparam>
		/// <param name="reader">Function to read result from a statement</param>
		/// <returns></returns>
		public IEnumerable<T> ExecuteDeferredQuery<T>(Func<IReader, T> reader)
		{
			try
			{
				Sqlite3Statement stmt = this.Prepare();

				while (SQLite3.Step(stmt) == SQLite3.Result.Row)
				{
					T obj = reader(this);
					this.OnInstanceCreated(obj);
					yield return obj;
				}
			}
			finally
			{
				this.DisposePreparedStatement();
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
            try
            {
                var stmt = this.Prepare();
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
                    var val = this.ReadColumn<T>(stmt, columnIndex);
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
				this.DisposePreparedStatement();
            }
		}

        #region Dynamic Column Readers

        private delegate object SQLite3ColumnReader(Sqlite3Statement s, int i);
		private static Dictionary<Type, SQLite3ColumnReader> _Reader = new();

		private static SQLite3ColumnReader _ReadInt = (s, i) => SQLite3.ColumnInt(s, i);

		private static void BuildColumnReader()
        {
			_Reader[typeof(int)] = _ReadInt;
			_Reader[typeof(bool)] = _ReadInt;
			_Reader[typeof(byte)] = _ReadInt;
			_Reader[typeof(UInt16)] = _ReadInt;
			_Reader[typeof(Int16)] = _ReadInt;
			_Reader[typeof(sbyte)] = _ReadInt;

			_Reader[typeof(long)] = (s, i) => (long)SQLite3.ColumnInt64(s, i);
			_Reader[typeof(TimeSpan)] = (s, i) => TimeSpan.FromTicks((long)SQLite3.ColumnInt64(s, i));
			_Reader[typeof(DateTime)] = (s, i) => new DateTime((long)SQLite3.ColumnInt64(s, i));
			_Reader[typeof(DateTimeOffset)] = (s, i) => new DateTimeOffset((long)SQLite3.ColumnInt64(s, i), TimeSpan.Zero);

			_Reader[typeof(double)] = (s, i) => SQLite3.ColumnDouble(s,i);
			_Reader[typeof(float)] = (s, i) => (float)SQLite3.ColumnDouble(s, i);

			_Reader[typeof(string)] = (s, i) => SQLite3.ColumnString(s, i);
			_Reader[typeof(Guid)] = (s,i) => new Guid( (string)SQLite3.ColumnString(s,i) );
			_Reader[typeof(Uri)] = (s, i) => new Uri((string)SQLite3.ColumnString(s, i));
			_Reader[typeof(StringBuilder)] = (s, i) => new StringBuilder((string)SQLite3.ColumnString(s, i));
			_Reader[typeof(UriBuilder)] = (s, i) => new UriBuilder((string)SQLite3.ColumnString(s, i));

			_Reader[typeof(byte[])] = (s, i) => SQLite3.ColumnByteArray(s, i);
		}

		/// <summary>
		/// Read column value from specified statement at given index
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stmt"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private T ReadColumn<T>(Sqlite3Statement stmt, int index)
		{
			var colType = SQLite3.ColumnType(stmt, index);
            if (colType == SQLite3.ColType.Null)
            {
				return default(T);
            }

			var clrType = typeof(T);

			if (clrType.IsEnum)
			{
				if (colType == SQLite3.ColType.Text)
				{
					var value = SQLite3.ColumnString(stmt, index);
					return (T)Enum.Parse(clrType, value.ToString(), true);
				}
				else
				{
					return (T)(object)SQLite3.ColumnInt(stmt, index);
				}
			}

			SQLite3ColumnReader reader;
            if (_Reader.TryGetValue(clrType, out reader))
            {
				return (T)(object)reader(stmt, index);
            }

			throw new NotSupportedException("Don't know how to read " + clrType);
		}

		#endregion

		/// <summary>
		/// Read Value of current row
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="index"></param>
		/// <returns></returns>
		public T ReadCurrentRow<T>( int index )
        {
			return this.ReadColumn<T>(_PreparedStatement, index);
        }

		private Dictionary<string, int> _ColNameCache;

		/// <summary>
		/// Read Value of current Row
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column"></param>
		/// <returns></returns>
		public T ReadCurrentRow<T>(string column)
		{
            if (_ColNameCache == null)
			{
				_ColNameCache = new Dictionary<string, int>();
				var count = SQLite3.ColumnCount(_PreparedStatement);
				for (int i = 0; i < count; i++)
				{
					var name = SQLite3.ColumnName16(_PreparedStatement, i);
					_ColNameCache[name] = i;
				}
			}

			return this.ReadColumn<T>(_PreparedStatement, _ColNameCache[column]);
		}

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
		/// Clean up prepared statement
		/// </summary>
		private void DisposePreparedStatement()
		{
            if (this.AutoDispose == false)
            {
				return;
            }

			if (_PreparedStatement == IntPtr.Zero)
			{
				return;
			}

			SQLite3.Finalize(_PreparedStatement);
			_PreparedStatement = IntPtr.Zero;
		}

		/// <summary>
		/// Dispose the command
		/// </summary>
		public void Dispose()
		{
			this.DisposePreparedStatement();
			GC.SuppressFinalize(this);
		}

		~SQLiteCommand()
        {
			this.DisposePreparedStatement();
        }
	}

}
