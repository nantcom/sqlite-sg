﻿using System;
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
	/// An open connection to a SQLite database.
	/// </summary>
	public partial class SQLiteConnection : IDisposable
	{
		private bool _open;
		private TimeSpan _busyTimeout;

		//There is no way to keep ITableMapping using generic interface, so we need to keep it as object
		readonly static Dictionary<Type, object> _mappings = new Dictionary<Type, object>();
		private System.Diagnostics.Stopwatch _sw;
		private long _elapsedMilliseconds = 0;

		private int _transactionDepth = 0;
		private Random _rand = new Random();

		public Sqlite3DatabaseHandle Handle { get; private set; }
		static readonly Sqlite3DatabaseHandle NullHandle = default(Sqlite3DatabaseHandle);
		static readonly Sqlite3BackupHandle NullBackupHandle = default(Sqlite3BackupHandle);

		/// <summary>
		/// Gets the database path used by this connection.
		/// </summary>
		public string DatabasePath { get; private set; }

		/// <summary>
		/// Gets the SQLite library version number. 3007014 would be v3.7.14
		/// </summary>
		public int LibVersionNumber { get; private set; }

		/// <summary>
		/// Whether Trace lines should be written that show the execution time of queries.
		/// </summary>
		public bool TimeExecution { get; set; }

		/// <summary>
		/// Whether to write queries to <see cref="Tracer"/> during execution.
		/// </summary>
		public bool Trace { get; set; }

		/// <summary>
		/// The delegate responsible for writing trace lines.
		/// </summary>
		/// <value>The tracer.</value>
		public Action<string> Tracer { get; set; }

		/// <summary>
		/// Whether to store DateTime properties as ticks (true) or strings (false).
		/// </summary>
		public bool StoreDateTimeAsTicks { get; private set; }

		/// <summary>
		/// Whether to store TimeSpan properties as ticks (true) or strings (false).
		/// </summary>
		public bool StoreTimeSpanAsTicks { get; private set; }

		/// <summary>
		/// The format to use when storing DateTime properties as strings. Ignored if StoreDateTimeAsTicks is true.
		/// </summary>
		/// <value>The date time string format.</value>
		public string DateTimeStringFormat { get; private set; }

		/// <summary>
		/// The DateTimeStyles value to use when parsing a DateTime property string.
		/// </summary>
		/// <value>The date time style.</value>
		internal System.Globalization.DateTimeStyles DateTimeStyle { get; private set; }

		/// <summary>
		/// Constructs a new SQLiteConnection and opens a SQLite database specified by databasePath.
		/// </summary>
		/// <param name="databasePath">
		/// Specifies the path to the database file.
		/// </param>
		/// <param name="storeDateTimeAsTicks">
		/// Specifies whether to store DateTime properties as ticks (true) or strings (false). You
		/// absolutely do want to store them as Ticks in all new projects. The value of false is
		/// only here for backwards compatibility. There is a *significant* speed advantage, with no
		/// down sides, when setting storeDateTimeAsTicks = true.
		/// If you use DateTimeOffset properties, it will be always stored as ticks regardingless
		/// the storeDateTimeAsTicks parameter.
		/// </param>
		public SQLiteConnection(string databasePath, bool storeDateTimeAsTicks = true)
			: this(new SQLiteConnectionString(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks))
		{
		}

		/// <summary>
		/// Constructs a new SQLiteConnection and opens a SQLite database specified by databasePath.
		/// </summary>
		/// <param name="databasePath">
		/// Specifies the path to the database file.
		/// </param>
		/// <param name="openFlags">
		/// Flags controlling how the connection should be opened.
		/// </param>
		/// <param name="storeDateTimeAsTicks">
		/// Specifies whether to store DateTime properties as ticks (true) or strings (false). You
		/// absolutely do want to store them as Ticks in all new projects. The value of false is
		/// only here for backwards compatibility. There is a *significant* speed advantage, with no
		/// down sides, when setting storeDateTimeAsTicks = true.
		/// If you use DateTimeOffset properties, it will be always stored as ticks regardingless
		/// the storeDateTimeAsTicks parameter.
		/// </param>
		public SQLiteConnection(string databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true)
			: this(new SQLiteConnectionString(databasePath, openFlags, storeDateTimeAsTicks))
		{
		}

		/// <summary>
		/// Constructs a new SQLiteConnection and opens a SQLite database specified by databasePath.
		/// </summary>
		/// <param name="connectionString">
		/// Details on how to find and open the database.
		/// </param>
		public SQLiteConnection(SQLiteConnectionString connectionString)
		{
			if (connectionString == null)
				throw new ArgumentNullException(nameof(connectionString));
			if (connectionString.DatabasePath == null)
				throw new InvalidOperationException("DatabasePath must be specified");

			DatabasePath = connectionString.DatabasePath;

			LibVersionNumber = SQLite3.LibVersionNumber();

			Sqlite3DatabaseHandle handle;

			// open using the byte[]
			// in the case where the path may include Unicode
			// force open to using UTF-8 using sqlite3_open_v2
			var databasePathAsBytes = GetNullTerminatedUtf8(connectionString.DatabasePath);
			var r = SQLite3.Open(databasePathAsBytes, out handle, (int)connectionString.OpenFlags, connectionString.VfsName);

			Handle = handle;
			if (r != SQLite3.Result.OK)
			{
				throw SQLiteException.New(r, String.Format("Could not open database file: {0} ({1})", DatabasePath, r));
			}
			_open = true;

			StoreDateTimeAsTicks = connectionString.StoreDateTimeAsTicks;
			StoreTimeSpanAsTicks = connectionString.StoreTimeSpanAsTicks;
			DateTimeStringFormat = connectionString.DateTimeStringFormat;
			DateTimeStyle = connectionString.DateTimeStyle;

			BusyTimeout = TimeSpan.FromSeconds(1.0);
			Tracer = line => Debug.WriteLine(line);

			connectionString.PreKeyAction?.Invoke(this);
			if (connectionString.Key is string stringKey)
			{
				SetKey(stringKey);
			}
			else if (connectionString.Key is byte[] bytesKey)
			{
				SetKey(bytesKey);
			}
			else if (connectionString.Key != null)
			{
				throw new InvalidOperationException("Encryption keys must be strings or byte arrays");
			}
			connectionString.PostKeyAction?.Invoke(this);
		}

		/// <summary>
		/// Enables the write ahead logging. WAL is significantly faster in most scenarios
		/// by providing better concurrency and better disk IO performance than the normal
		/// journal mode. You only need to call this function once in the lifetime of the database.
		/// </summary>
		public void EnableWriteAheadLogging()
		{
			this.ExecuteNonQuery("PRAGMA journal_mode=WAL");
		}

		/// <summary>
		/// Convert an input string to a quoted SQL string that can be safely used in queries.
		/// </summary>
		/// <returns>The quoted string.</returns>
		/// <param name="unsafeString">The unsafe string to quote.</param>
		static string Quote(string unsafeString)
		{
			// TODO: Doesn't call sqlite3_mprintf("%Q", u) because we're waiting on https://github.com/ericsink/SQLitePCL.raw/issues/153
			if (unsafeString == null)
				return "NULL";
			var safe = unsafeString.Replace("'", "''");
			return "'" + safe + "'";
		}

		/// <summary>
		/// Sets the key used to encrypt/decrypt the database with "pragma key = ...".
		/// This must be the first thing you call before doing anything else with this connection
		/// if your database is encrypted.
		/// This only has an effect if you are using the SQLCipher nuget package.
		/// </summary>
		/// <param name="key">Ecryption key plain text that is converted to the real encryption key using PBKDF2 key derivation</param>
		void SetKey(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			var q = Quote(key);
			this.ExecuteNonQuery("pragma key = " + q);
		}

		/// <summary>
		/// Sets the key used to encrypt/decrypt the database.
		/// This must be the first thing you call before doing anything else with this connection
		/// if your database is encrypted.
		/// This only has an effect if you are using the SQLCipher nuget package.
		/// </summary>
		/// <param name="key">256-bit (32 byte) ecryption key data</param>
		void SetKey(byte[] key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (key.Length != 32)
				throw new ArgumentException("Key must be 32 bytes (256-bit)", nameof(key));
			var s = String.Join("", key.Select(x => x.ToString("X2")));

			this.ExecuteNonQuery("pragma key = \"x'" + s + "'\"");
		}

		/// <summary>
		/// Enable or disable extension loading.
		/// </summary>
		public void EnableLoadExtension(bool enabled)
		{
			SQLite3.Result r = SQLite3.EnableLoadExtension(Handle, enabled ? 1 : 0);
			if (r != SQLite3.Result.OK)
			{
				string msg = SQLite3.GetErrmsg(Handle);
				throw SQLiteException.New(r, msg);
			}
		}

		static byte[] GetNullTerminatedUtf8(string s)
		{
			var utf8Length = System.Text.Encoding.UTF8.GetByteCount(s);
			var bytes = new byte[utf8Length + 1];
			utf8Length = System.Text.Encoding.UTF8.GetBytes(s, 0, s.Length, bytes, 0);
			return bytes;
		}

		/// <summary>
		/// Sets a busy handler to sleep the specified amount of time when a table is locked.
		/// The handler will sleep multiple times until a total time of <see cref="BusyTimeout"/> has accumulated.
		/// </summary>
		public TimeSpan BusyTimeout
		{
			get { return _busyTimeout; }
			set
			{
				_busyTimeout = value;
				if (Handle != NullHandle)
				{
					SQLite3.BusyTimeout(Handle, (int)_busyTimeout.TotalMilliseconds);
				}
			}
		}

		/// <summary>
		/// Gets the table mapping for specified type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public ITableMapping<T> GetMapping<T>()
		{
			object map;
			if (_mappings.TryGetValue(typeof(T), out map))
			{
				return (ITableMapping<T>)map;
			}

			return null;
		}


		private struct IndexedColumn
		{
			public int Order;
			public string ColumnName;
		}

		private struct IndexInfo
		{
			public string IndexName;
			public string TableName;
			public bool Unique;
			public List<IndexedColumn> Columns;
		}

		/// <summary>
		/// Executes a "drop table" on the database.  This is non-recoverable.
		/// </summary>
		/// <param name="map">
		/// The TableMapping used to identify the table.
		/// </param>
		public int DropTable<T>()
		{
			return this.GetMapping<T>().DropTable(this);
		}

		/// <summary>
		/// Executes a "create table if not exists" on the database. It also
		/// creates any specified indexes on the columns of the table. It uses
		/// a schema automatically generated from the specified type. You can
		/// later access this schema by calling GetMapping.
		/// </summary>
		/// <returns>
		/// Whether the table was created or migrated.
		/// </returns>
		public CreateTableResult CreateTable<T>()
		{
			CreateTableResult result = CreateTableResult.Invalid;
			var map = this.GetMapping<T>();

			// Present a nice error if no columns specified
			if (map.Columns.Length == 0)
			{
				throw new Exception(string.Format("Cannot create a table without columns (does '{0}' have public properties?)", map.SourceType.FullName));
			}

			// Check if the table exists
			var query = $"pragma table_info(\"{map.TableName}\")";
			var existingCols = this.QueryDeferredScalars<string>( query, 1 ).ToList();

			// Create or migrate it
			if (existingCols.Count == 0)
			{
				map.CreateTable(this);
				result = CreateTableResult.Created;
			}
			else
			{
				map.MigrateTable(this, existingCols);
				result = CreateTableResult.Created;
			}

			map.CreateIndex(this);

			return result;
		}

		/// <summary>
		/// Creates a new SQLiteCommand. Can be overridden to provide a sub-class.
		/// </summary>
		/// <seealso cref="SQLiteCommand.OnInstanceCreated"/>
		protected virtual SQLiteCommand NewCommand()
		{
			return new SQLiteCommand(this);
		}

		/// <summary>
		/// Creates a new SQLiteCommand given the command text with arguments. Place a '?'
		/// in the command text for each of the arguments.
		/// </summary>
		/// <param name="cmdText">
		/// The fully escaped SQL.
		/// </param>
		/// <param name="ps">
		/// Arguments to substitute for the occurences of '?' in the command text.
		/// </param>
		/// <returns>
		/// A <see cref="SQLiteCommand"/>
		/// </returns>
		public SQLiteCommand CreateCommand(string cmdText, params object[] ps)
		{
			if (!_open)
				throw SQLiteException.New(SQLite3.Result.Error, "Cannot create commands from unopened database");

			var cmd = NewCommand();
			cmd.CommandText = cmdText;
			foreach (var o in ps)
			{
				cmd.Bind(o);
			}
			return cmd;
		}

		/// <summary>
		/// Creates a new SQLiteCommand given the command text with named arguments. Place a "[@:$]VVV"
		/// in the command text for each of the arguments. VVV represents an alphanumeric identifier.
		/// For example, @name :name and $name can all be used in the query.
		/// </summary>
		/// <param name="cmdText">
		/// The fully escaped SQL.
		/// </param>
		/// <param name="args">
		/// Arguments to substitute for the occurences of "[@:$]VVV" in the command text.
		/// </param>
		/// <returns>
		/// A <see cref="SQLiteCommand" />
		/// </returns>
		public SQLiteCommand CreateCommand(string cmdText, Dictionary<string, object> args)
		{
			if (!_open)
				throw SQLiteException.New(SQLite3.Result.Error, "Cannot create commands from unopened database");

			SQLiteCommand cmd = NewCommand();
			cmd.CommandText = cmdText;
			foreach (var kv in args)
			{
				cmd.Bind(kv.Key, kv.Value);
			}
			return cmd;
		}

		/// <summary>
		/// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
		/// in the command text for each of the arguments and then executes that command.
		/// Use this method instead of Query when you don't expect rows back. Such cases include
		/// INSERTs, UPDATEs, and DELETEs.
		/// You can set the Trace or TimeExecution properties of the connection
		/// to profile execution.
		/// </summary>
		/// <param name="query">
		/// The fully escaped SQL.
		/// </param>
		/// <param name="args">
		/// Arguments to substitute for the occurences of '?' in the query.
		/// </param>
		/// <returns>
		/// The number of rows modified in the database as a result of this execution.
		/// </returns>
		public int ExecuteNonQuery(string query, params object[] args)
		{
			var cmd = CreateCommand(query, args);

			if (TimeExecution)
			{
				if (_sw == null)
				{
					_sw = new Stopwatch();
				}
				_sw.Reset();
				_sw.Start();
			}

			var r = cmd.ExecuteNonQuery();

			if (TimeExecution)
			{
				_sw.Stop();
				_elapsedMilliseconds += _sw.ElapsedMilliseconds;
				Tracer?.Invoke(string.Format("Finished in {0} ms ({1:0.0} s total)", _sw.ElapsedMilliseconds, _elapsedMilliseconds / 1000.0));
			}

			return r;
		}

		/// <summary>
		/// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
		/// in the command text for each of the arguments and then executes that command.
		/// It returns the first column of each row of the result.
		/// </summary>
		/// <param name="query">
		/// The fully escaped SQL.
		/// </param>
		/// <param name="args">
		/// Arguments to substitute for the occurences of '?' in the query.
		/// </param>
		/// <returns>
		/// An enumerable with one result for the first column of each row returned by the query.
		/// </returns>
		public IEnumerable<T> QueryDeferredScalars<T>(string query, int columnIndex = 0, params object[] args)
		{
			var cmd = CreateCommand(query, args);
			return cmd.ExecuteDeferredScalars<T>(columnIndex);
		}

		/// <summary>
		/// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
		/// in the command text for each of the arguments and then executes that command.
		/// It returns each row of the result using the mapping automatically generated for
		/// the given type.
		/// </summary>
		/// <param name="query">
		/// The fully escaped SQL.
		/// </param>
		/// <param name="args">
		/// Arguments to substitute for the occurences of '?' in the query.
		/// </param>
		/// <returns>
		/// An enumerable with one result for each row returned by the query.
		/// The enumerator (retrieved by calling GetEnumerator() on the result of this method)
		/// will call sqlite3_step on each call to MoveNext, so the database
		/// connection must remain open for the lifetime of the enumerator.
		/// </returns>
		public IEnumerable<T> QueryDeferred<T>(string query, params object[] args) where T : new()
		{
			var cmd = CreateCommand(query, args);
			return cmd.ExecuteDeferredQuery<T>();
		}

		/// <summary>
		/// Returns a queryable interface to the table represented by the given type.
		/// </summary>
		/// <returns>
		/// A queryable object that is able to translate Where, OrderBy, and Take
		/// queries into native SQL.
		/// </returns>
		public TableQuery<T> Find<T>() where T : new()
		{
			return new TableQuery<T>(this);
		}

		/// <summary>
		/// Whether <see cref="BeginTransaction"/> has been called and the database is waiting for a <see cref="Commit"/>.
		/// </summary>
		public bool IsInTransaction
		{
			get { return _transactionDepth > 0; }
		}

		/// <summary>
		/// Begins a new transaction. Call <see cref="Commit"/> to end the transaction.
		/// </summary>
		/// <example cref="System.InvalidOperationException">Throws if a transaction has already begun.</example>
		public void BeginTransaction()
		{
			// The BEGIN command only works if the transaction stack is empty,
			//    or in other words if there are no pending transactions.
			// If the transaction stack is not empty when the BEGIN command is invoked,
			//    then the command fails with an error.
			// Rather than crash with an error, we will just ignore calls to BeginTransaction
			//    that would result in an error.
			if (Interlocked.CompareExchange(ref _transactionDepth, 1, 0) == 0)
			{
				try
				{
					this.ExecuteNonQuery("begin transaction");
				}
				catch (Exception ex)
				{
					var sqlExp = ex as SQLiteException;
					if (sqlExp != null)
					{
						// It is recommended that applications respond to the errors listed below
						//    by explicitly issuing a ROLLBACK command.
						// TODO: This rollback failsafe should be localized to all throw sites.
						switch (sqlExp.Result)
						{
							case SQLite3.Result.IOError:
							case SQLite3.Result.Full:
							case SQLite3.Result.Busy:
							case SQLite3.Result.NoMem:
							case SQLite3.Result.Interrupt:
								RollbackTo(null, true);
								break;
						}
					}
					else
					{
						// Call decrement and not VolatileWrite in case we've already
						//    created a transaction point in SaveTransactionPoint since the catch.
						Interlocked.Decrement(ref _transactionDepth);
					}

					throw;
				}
			}
			else
			{
				// Calling BeginTransaction on an already open transaction is invalid
				throw new InvalidOperationException("Cannot begin a transaction while already in a transaction.");
			}
		}

		/// <summary>
		/// Creates a savepoint in the database at the current point in the transaction timeline.
		/// Begins a new transaction if one is not in progress.
		///
		/// Call <see cref="RollbackTo(string)"/> to undo transactions since the returned savepoint.
		/// Call <see cref="Release"/> to commit transactions after the savepoint returned here.
		/// Call <see cref="Commit"/> to end the transaction, committing all changes.
		/// </summary>
		/// <returns>A string naming the savepoint.</returns>
		public string SaveTransactionPoint()
		{
			int depth = Interlocked.Increment(ref _transactionDepth) - 1;
			string retVal = "S" + _rand.Next(short.MaxValue) + "D" + depth;

			try
			{
				this.ExecuteNonQuery("savepoint " + retVal);
			}
			catch (Exception ex)
			{
				var sqlExp = ex as SQLiteException;
				if (sqlExp != null)
				{
					// It is recommended that applications respond to the errors listed below
					//    by explicitly issuing a ROLLBACK command.
					// TODO: This rollback failsafe should be localized to all throw sites.
					switch (sqlExp.Result)
					{
						case SQLite3.Result.IOError:
						case SQLite3.Result.Full:
						case SQLite3.Result.Busy:
						case SQLite3.Result.NoMem:
						case SQLite3.Result.Interrupt:
							RollbackTo(null, true);
							break;
					}
				}
				else
				{
					Interlocked.Decrement(ref _transactionDepth);
				}

				throw;
			}

			return retVal;
		}

		/// <summary>
		/// Rolls back the transaction that was begun by <see cref="BeginTransaction"/> or <see cref="SaveTransactionPoint"/>.
		/// </summary>
		public void Rollback()
		{
			this.RollbackTo(null, false);
		}

		/// <summary>
		/// Rolls back the savepoint created by <see cref="BeginTransaction"/> or SaveTransactionPoint.
		/// </summary>
		/// <param name="savepoint">The name of the savepoint to roll back to, as returned by <see cref="SaveTransactionPoint"/>.  If savepoint is null or empty, this method is equivalent to a call to <see cref="Rollback"/></param>
		public void RollbackTo(string savepoint)
		{
			this.RollbackTo(savepoint, false);
		}

		/// <summary>
		/// Rolls back the transaction that was begun by <see cref="BeginTransaction"/>.
		/// </summary>
		/// <param name="savepoint">The name of the savepoint to roll back to, as returned by <see cref="SaveTransactionPoint"/>.  If savepoint is null or empty, this method is equivalent to a call to <see cref="Rollback"/></param>
		/// <param name="noThrow">true to avoid throwing exceptions, false otherwise</param>
		private void RollbackTo(string savepoint, bool noThrow)
		{
			// Rolling back without a TO clause rolls backs all transactions
			//    and leaves the transaction stack empty.
			try
			{
				if (String.IsNullOrEmpty(savepoint))
				{
					if (Interlocked.Exchange(ref _transactionDepth, 0) > 0)
					{
						this.ExecuteNonQuery("rollback");
					}
				}
				else
				{
					this.DoSavePointExecute(savepoint, "rollback to ");
				}
			}
			catch (SQLiteException)
			{
				if (!noThrow)
					throw;

			}
			// No need to rollback if there are no transactions open.
		}

		/// <summary>
		/// Releases a savepoint returned from <see cref="SaveTransactionPoint"/>.  Releasing a savepoint
		///    makes changes since that savepoint permanent if the savepoint began the transaction,
		///    or otherwise the changes are permanent pending a call to <see cref="Commit"/>.
		///
		/// The RELEASE command is like a COMMIT for a SAVEPOINT.
		/// </summary>
		/// <param name="savepoint">The name of the savepoint to release.  The string should be the result of a call to <see cref="SaveTransactionPoint"/></param>
		public void Release(string savepoint)
		{
			try
			{
				this.DoSavePointExecute(savepoint, "release ");
			}
			catch (SQLiteException ex)
			{
				if (ex.Result == SQLite3.Result.Busy)
				{
					// Force a rollback since most people don't know this function can fail
					// Don't call Rollback() since the _transactionDepth is 0 and it won't try
					// Calling rollback makes our _transactionDepth variable correct.
					// Writes to the database only happen at depth=0, so this failure will only happen then.
					try
					{
						this.ExecuteNonQuery("rollback");
					}
					catch
					{
						// rollback can fail in all sorts of wonderful version-dependent ways. Let's just hope for the best
					}
				}
				throw;
			}
		}

		private void DoSavePointExecute(string savepoint, string cmd)
		{
			// Validate the savepoint
			int firstLen = savepoint.IndexOf('D');
			if (firstLen >= 2 && savepoint.Length > firstLen + 1)
			{
				int depth;
				if (Int32.TryParse(savepoint.Substring(firstLen + 1), out depth))
				{
					// TODO: Mild race here, but inescapable without locking almost everywhere.
					if (0 <= depth && depth < _transactionDepth)
					{
						Thread.VolatileWrite(ref _transactionDepth, depth);
						this.ExecuteNonQuery(cmd + savepoint);
						return;
					}
				}
			}

			throw new ArgumentException("savePoint is not valid, and should be the result of a call to SaveTransactionPoint.", "savePoint");
		}

		/// <summary>
		/// Commits the transaction that was begun by <see cref="BeginTransaction"/>.
		/// </summary>
		public void Commit()
		{
			if (Interlocked.Exchange(ref _transactionDepth, 0) != 0)
			{
				try
				{
					this.ExecuteNonQuery("commit");
				}
				catch
				{
					// Force a rollback since most people don't know this function can fail
					// Don't call Rollback() since the _transactionDepth is 0 and it won't try
					// Calling rollback makes our _transactionDepth variable correct.
					try
					{
						this.ExecuteNonQuery("rollback");
					}
					catch
					{
						// rollback can fail in all sorts of wonderful version-dependent ways. Let's just hope for the best
					}
					throw;
				}
			}
			// Do nothing on a commit with no open transaction
		}

		/// <summary>
		/// Executes <paramref name="action"/> within a (possibly nested) transaction by wrapping it in a SAVEPOINT. If an
		/// exception occurs the whole transaction is rolled back, not just the current savepoint. The exception
		/// is rethrown.
		/// </summary>
		/// <param name="action">
		/// The <see cref="Action"/> to perform within a transaction. <paramref name="action"/> can contain any number
		/// of operations on the connection but should never call <see cref="BeginTransaction"/> or
		/// <see cref="Commit"/>.
		/// </param>
		public void RunInTransaction(Action action)
		{
			try
			{
				var savePoint = this.SaveTransactionPoint();
				action();
				this.Release(savePoint);
			}
			catch (Exception)
			{
				this.Rollback();
				throw;
			}
		}

		/// <summary>
		/// Inserts all specified objects, nothing will be inserted if any of the insert fails
		/// </summary>
		/// <param name="objects">
		/// An <see cref="IEnumerable"/> of the objects to insert.
		/// <returns>
		/// The number of rows added to the table.
		/// </returns>
		public int InsertAll<T>(IEnumerable<T> collection)
		{
			var c = 0;

			this.RunInTransaction(() => {
				foreach (var r in collection)
				{
					c += this.Insert<T>(r);
				}
			});

			return c;
		}

		/// <summary>
		/// Inserts the given object (and updates its
		/// auto incremented primary key if it has one).
		/// The return value is the number of rows added to the table.
		/// </summary>
		/// <param name="obj">
		/// The object to insert.
		/// </param>
		/// <returns>
		/// The number of rows added to the table.
		/// </returns>
		public int Insert<T>(T obj, bool replacing = false)
		{
			var map = this.GetMapping<T>();
			var count = map.Insert(this, obj, replacing);

			if (count > 0)
			{
				this.OnTableChanged(typeof(T), NotifyTableChangedAction.Insert);
			}

			return count;
		}

		/// <summary>
		/// Updates all of the columns of a table using the specified object
		/// except for its primary key.
		/// The object is required to have a primary key.
		/// </summary>
		/// <param name="obj">
		/// The object to update. It must have a primary key designated using the PrimaryKeyAttribute.
		/// </param>
		/// <param name="objType">
		/// The type of object to insert.
		/// </param>
		/// <returns>
		/// The number of rows updated.
		/// </returns>
		public int Update<T>(T obj)
		{
			int rowsAffected = 0;
			var map = this.GetMapping<T>();

			map.Update(this, obj);

			if (rowsAffected > 0)
				OnTableChanged(typeof(T), NotifyTableChangedAction.Update);

			return rowsAffected;
		}

		/// <summary>
		/// Updates all specified objects.
		/// </summary>
		/// <param name="objects">
		/// An <see cref="IEnumerable"/> of the objects to insert.
		/// </param>
		/// <param name="runInTransaction">
		/// A boolean indicating if the inserts should be wrapped in a transaction
		/// </param>
		/// <returns>
		/// The number of rows modified.
		/// </returns>
		public int UpdateAll<T>( IEnumerable<T> objects)
		{
			var c = 0;
			this.RunInTransaction(() => {
				foreach (var r in objects)
				{
					c += this.Update(r);
				}
			});
			return c;
		}

		/// <summary>
		/// Deletes the given object from the database using its primary key.
		/// </summary>
		/// <param name="objectToDelete">
		/// The object to delete. It must have a primary key designated using the PrimaryKeyAttribute.
		/// </param>
		/// <returns>
		/// The number of rows deleted.
		/// </returns>
		public int Delete<T>(T obj)
		{
			var map = this.GetMapping<T>();

			int count = map.Delete(this, obj);
			if (count > 0)
			{
				this.OnTableChanged(typeof(T), NotifyTableChangedAction.Delete);
			}

			return count;
		}

		/// <summary>
		/// Deletes the object with the specified primary key.
		/// </summary>
		/// <param name="primaryKey">
		/// The primary key of the object to delete.
		/// </param>
		/// <param name="map">
		/// The TableMapping used to identify the table.
		/// </param>
		/// <returns>
		/// The number of objects deleted.
		/// </returns>
		public int DeleteByPrimaryKey<TObject>(object primaryKey)
		{
			var map = this.GetMapping<TObject>();

			int count = map.DeleteByPrimaryKey(this, primaryKey);
			if (count > 0)
			{
				this.OnTableChanged(typeof(TObject), NotifyTableChangedAction.Delete);
			}

			return count;
		}

		/// <summary>
		/// Deletes all the objects from the specified table.
		/// WARNING WARNING: Let me repeat. It deletes ALL the objects from the
		/// specified table. Do you really want to do that?
		/// </summary>
		/// <param name="map">
		/// The TableMapping used to identify the table.
		/// </param>
		/// <returns>
		/// The number of objects deleted.
		/// </returns>
		public int DeleteAll<T>()
		{
			var map = this.GetMapping<T>();

			int count = map.DeleteAll(this);
			if (count > 0)
			{
				this.OnTableChanged(typeof(T), NotifyTableChangedAction.Delete);
			}

			return count;
		}

		/// <summary>
		/// Backup the entire database to the specified path.
		/// </summary>
		/// <param name="destinationDatabasePath">Path to backup file.</param>
		/// <param name="databaseName">The name of the database to backup (usually "main").</param>
		public void Backup(string destinationDatabasePath, string databaseName = "main")
		{
			// Open the destination
			var r = SQLite3.Open(destinationDatabasePath, out var destHandle);
			if (r != SQLite3.Result.OK)
			{
				throw SQLiteException.New(r, "Failed to open destination database");
			}

			// Init the backup
			var backup = SQLite3.BackupInit(destHandle, databaseName, Handle, databaseName);
			if (backup == NullBackupHandle)
			{
				SQLite3.Close(destHandle);
				throw new Exception("Failed to create backup");
			}

			// Perform it
			SQLite3.BackupStep(backup, -1);
			SQLite3.BackupFinish(backup);

			// Check for errors
			r = SQLite3.GetResult(destHandle);
			string msg = "";
			if (r != SQLite3.Result.OK)
			{
				msg = SQLite3.GetErrmsg(destHandle);
			}

			// Close everything and report errors
			SQLite3.Close(destHandle);
			if (r != SQLite3.Result.OK)
			{
				throw SQLiteException.New(r, msg);
			}
		}

		~SQLiteConnection()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Close()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			var useClose2 = LibVersionNumber >= 3007014;

			if (_open && Handle != NullHandle)
			{
				try
				{
					if (disposing)
					{
						var r = useClose2 ? SQLite3.Close2(Handle) : SQLite3.Close(Handle);
						if (r != SQLite3.Result.OK)
						{
							string msg = SQLite3.GetErrmsg(Handle);
							throw SQLiteException.New(r, msg);
						}
					}
					else
					{
						var r = useClose2 ? SQLite3.Close2(Handle) : SQLite3.Close(Handle);
					}
				}
				finally
				{
					Handle = NullHandle;
					_open = false;
				}
			}
		}

		void OnTableChanged(Type t, NotifyTableChangedAction action)
		{
			var ev = TableChanged;
			if (ev != null)
				ev(this, new NotifyTableChangedEventArgs(t, action));
		}

		public event EventHandler<NotifyTableChangedEventArgs> TableChanged;
	}

}
