using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using Sqlite3Statement = System.IntPtr;

namespace CoreSharp.SQLite
{
    public interface ITableMapping<T>
    {
        Type SourceType { get; }

        /// <summary>
        /// Gets the name of the mapped table
        /// </summary>
        string TableName { get; }

		Dictionary<string, IColumnMapping<T>> Columns { get; }

        int DropTable(SQLiteConnection connection);

        /// <summary>
        /// Create 
        /// </summary>
        /// <param name="connection"></param>
        void CreateTable(SQLiteConnection connection);

        /// <summary>
        /// Migrate existing database table to contain new fields added by current mapping
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="existingColumns"></param>
        void MigrateTable(SQLiteConnection connection, List<string> existingColumns);

        /// <summary>
        /// Create indexes, as specified by Attributes in SourceType, on the database
        /// </summary>
        /// <param name="connection"></param>
        void CreateIndex(SQLiteConnection connection);

		/// <summary>
		/// Insert object or replace
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="input"></param>
		/// <param name="replace"></param>
		int Insert(SQLiteConnection connection, T input, bool replace);

		/// <summary>
		/// Updates the object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		int Update(SQLiteConnection connection, T input);

		/// <summary>
		/// Delete the specified object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		int Delete(SQLiteConnection connection, T input);

		/// <summary>
		/// Delete an object from database using specified primary key
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		int DeleteByPrimaryKey(SQLiteConnection connection, object pk);

		/// <summary>
		/// Delete all items of this type from the database
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		int DeleteAll(SQLiteConnection connection);

		/// <summary>
		/// Read Statement Result
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		T ReadStatementResult(Sqlite3Statement stmt, string[] columnNames = null);

		/// <summary>
		/// Gets ampped column name from given property name
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
        string GetMappedColumnName(string propertyName);
    }

	public interface IColumnMapping<T>
    {
        Type SourceType { get; }

        /// <summary>
        /// Gets the name of the Column
        /// </summary>
        string ColumnName { get; }

		Action<T, object> Setter { get; }

		Func<T, object> Getter { get; }
	}

	public sealed class StaticColumnMapping<T> : IColumnMapping<T>
    {
		public Type SourceType => typeof(T);

		/// <summary>
		/// Gets the name of the Column
		/// </summary>
		public string ColumnName { get; set; }

		public Action<T, object> Setter { get; set; }
		public Func<T, object> Getter { get; set; }
	}

	public abstract class StaticTableMapping<T> : ITableMapping<T> where T : new()
	{
		public abstract string TableName { get; }

		public virtual Type SourceType => typeof(T);

		public Dictionary<string, IColumnMapping<T>> Columns { get; protected set; }

		/// <summary>
		/// Provide an Insert Command
		/// </summary>
		protected abstract string InsertCommand { get; }

		/// <summary>
		/// Provide a replace command
		/// </summary>
		protected abstract string ReplaceCommand { get; }

		/// <summary>
		/// Provide an update command
		/// </summary>
		protected abstract string UpdateCommand { get; }

		/// <summary>
		/// Provide a Delete command
		/// </summary>
		protected abstract string DeleteCommand { get; }

		/// <summary>
		/// Drops the table
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public virtual int DropTable( SQLiteConnection connection )
		{
			throw new NotImplementedException();
			/*var query = $"drop table if exists \"{this.TableName}\"";
            return connection.ExecuteNonQuery(query);*/
        }

		/// <summary>
		/// Create the Table for storing this mapped class
		/// </summary>
		/// <param name="connection">Connection to use</param>
        public virtual void CreateTable(SQLiteConnection connection)
        {
            /*
             * 
				// Facilitate virtual tables a.k.a. full-text search.
				bool fts3 = (createFlags & CreateFlags.FullTextSearch3) != 0;
				bool fts4 = (createFlags & CreateFlags.FullTextSearch4) != 0;
				bool fts = fts3 || fts4;
				var @virtual = fts ? "virtual " : string.Empty;
				var @using = fts3 ? "using fts3 " : fts4 ? "using fts4 " : string.Empty;

				// Build query.
				var query = "create " + @virtual + "table if not exists \"" + map.TableName + "\" " + @using + "(\n";
				var decls = map.Columns.Select(p => Orm.SqlDecl(p, StoreDateTimeAsTicks, StoreTimeSpanAsTicks));
				var decl = string.Join(",\n", decls.ToArray());
				query += decl;
				query += ")";
				if (map.WithoutRowId)
				{
					query += " without rowid";
				}

				Execute(query);
             * 
             */
            throw new NotImplementedException();
        }

		/// <summary>
		/// Add Missing columns in the database to store all properties in current
		/// class definition
		/// </summary>
		/// <param name="connection">Connection to use</param>
		/// <param name="existingColumns">columns currently in the database</param>
        public virtual void MigrateTable(SQLiteConnection connection, List<string> existingColumns)
        {
			// this dictionary contains list of columns and
			// commands to alter table and create column
			// existing column will remove item in this dictionary
			var allColumns = new Dictionary<string, string>();

            foreach (var column in existingColumns)
            {
                if (allColumns.ContainsKey(column))
                {
					allColumns.Remove(column);
                }
            }

			foreach (var column in allColumns)
			{
				connection.ExecuteNonQuery(column.Value);
			}

			/*
             var toBeAdded = new List<TableMappingColumn>();

			foreach (var p in map.Columns)
			{
				var found = false;
				foreach (var c in existingCols)
				{
					found = (string.Compare(p.Name, c.Name, StringComparison.OrdinalIgnoreCase) == 0);
					if (found)
						break;
				}
				if (!found)
				{
					toBeAdded.Add(p);
				}
			}

			foreach (var p in toBeAdded)
			{
				var addCol = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(p, StoreDateTimeAsTicks, StoreTimeSpanAsTicks);
				Execute(addCol);
			}
             */


			throw new NotImplementedException();
        }

		/// <summary>
		/// Create Indexes for current table as specified using attribute in class
		/// </summary>
		/// <param name="connection"></param>
        public virtual void CreateIndex(SQLiteConnection connection)
        {
			/*
             
			var indexes = new Dictionary<string, IndexInfo>();
			foreach (var c in map.Columns)
			{
				foreach (var i in c.Indices)
				{
					var iname = i.Name ?? map.TableName + "_" + c.Name;
					IndexInfo iinfo;
					if (!indexes.TryGetValue(iname, out iinfo))
					{
						iinfo = new IndexInfo
						{
							IndexName = iname,
							TableName = map.TableName,
							Unique = i.Unique,
							Columns = new List<IndexedColumn>()
						};
						indexes.Add(iname, iinfo);
					}

					if (i.Unique != iinfo.Unique)
						throw new Exception("All the columns in an index must have the same value for their Unique property");

					iinfo.Columns.Add(new IndexedColumn
					{
						Order = i.Order,
						ColumnName = c.Name
					});
				}
			}

			foreach (var indexName in indexes.Keys)
			{
				var index = indexes[indexName];
				var columns = index.Columns.OrderBy(i => i.Order).Select(i => i.ColumnName).ToArray();
				CreateIndex(indexName, index.TableName, columns, index.Unique);
			}

            // Craete Index Code
			const string sqlFormat = "create {2} index if not exists \"{3}\" on \"{0}\"(\"{1}\")";
			var sql = String.Format(sqlFormat, tableName, string.Join("\", \"", columnNames), unique ? "unique" : "", indexName);
			return Execute(sql);

             */
		}

		/// <summary>
		/// Gets an object by its primary key
		/// </summary>
		/// <param name="pk"></param>
		/// <returns></returns>
		public virtual T GetByPrimaryKey<TKey>(TKey pk)
        {
			// use a precompiled query
			throw new NotImplementedException();
        }

		/// <summary>
		/// TODO: Implement actual caching logic
		/// </summary>
		/// <returns></returns>
		protected virtual SQLiteCommand GetPreparedInsertCommand(SQLiteConnection connection)
        {
			var cmd = new SQLiteCommand(connection, this.InsertCommand, true);
			return cmd;
		}

		/// <summary>
		/// TODO: Implement actual caching logic
		/// </summary>
		/// <returns></returns>
		protected virtual SQLiteCommand GetPreparedReplaceCommand(SQLiteConnection connection)
		{
			var cmd = new SQLiteCommand(connection, this.ReplaceCommand, true);
			return cmd;
		}

		/// <summary>
		/// TODO: Implement actual caching logic
		/// </summary>
		/// <returns></returns>
		protected virtual SQLiteCommand GetPreparedUpdateCommand(SQLiteConnection connection)
		{
			var cmd = new SQLiteCommand(connection, this.UpdateCommand, true);
			return cmd;
		}

		/// <summary>
		/// TODO: Implement actual caching logic
		/// </summary>
		/// <returns></returns>
		protected virtual SQLiteCommand GetPreparedDeleteCommand(SQLiteConnection connection)
		{
			var cmd = new SQLiteCommand(connection, this.DeleteCommand, true);
			return cmd;
		}

		/// <summary>
		/// Perform insert or replace operation
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="input"></param>
		/// <param name="replace"></param>
		/// <returns></returns>
		public virtual int Insert(SQLiteConnection connection, T input, bool replace)
		{
			throw new NotImplementedException();

			// skeleton code

			if (replace)
			{
				return this.Replace(connection, input);
			}

			var cmdText = "insert into table_name(field1,field2) values (?,?) ";
			var cmd = this.GetPreparedInsertCommand(connection);
			cmd.ParameterBinder = (stmt) =>
			{
				// implement the binding call
				JsonSerializer.Serialize(input);
			};
			cmd.ExecuteNonQuery();

			// loop through all property in the same order as insert command generation
			//SQLite3.BindInt(stmt, 0, 0);

		}

		protected virtual int Replace(SQLiteConnection connection, T obj)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Perform update operation
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		public virtual int Update(SQLiteConnection connection, T input)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Perform delete operation, using Object's Primary Key
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		public virtual int Delete(SQLiteConnection conn, T input)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Delete all items from the table
		/// </summary>
		/// <param name="conn"></param>
		/// <returns></returns>
		public virtual int DeleteAll(SQLiteConnection conn)
		{
			var query = $"delete from \"{this.TableName}\"";
			return conn.ExecuteNonQuery(query);
		}

		/// <summary>
		/// Delete an object by its primary key
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual int DeleteByPrimaryKey( SQLiteConnection conn, object key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Read result from SQL Statement and create new instance of T
		/// </summary>
		/// <param name="stmt"></param>
		/// <param name="columnNames"></param>
		/// <returns></returns>
		public virtual T ReadStatementResult(Sqlite3Statement stmt, string[] columnNames = null)
		{
            if (columnNames == null)
            {
				// static fast reading code
				return this.ReadSequentialColumnFromStatement(stmt);
            }

            for (int i = 0; i < columnNames.Length; i++)
            {
                switch (columnNames[i])
                {
                    default:
                        break;
                }
            }

			throw new NotImplementedException();
		}

		protected abstract T ReadSequentialColumnFromStatement(Sqlite3Statement stmt);

        public string GetMappedColumnName(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
