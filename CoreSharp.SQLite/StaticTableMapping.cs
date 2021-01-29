using System;
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

		IColumnMapping[] Columns { get; }

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

	public interface IColumnMapping
    {
        Type SourceType { get; }

        /// <summary>
        /// Gets the name of the Column
        /// </summary>
        string ColumnName { get; }
	}

	public sealed class StaticColumnMapping<T,T2> : IColumnMapping
    {
		public Type SourceType => typeof(T);

		/// <summary>
		/// Gets the name of the Column
		/// </summary>
		public string ColumnName { get; set; }

		public Action<T, T2> Setter { get; set; }
		public Func<T, T2> Getter { get; set; }
	}

    public abstract class StaticTableMapping<T> : ITableMapping<T>
    {
        public abstract string TableName { get; }

		public virtual Type SourceType => typeof(T);

		public IColumnMapping[] Columns { get; protected set; }

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

		public virtual int Insert(SQLiteConnection connection, T input, bool replace)
		{
			throw new NotImplementedException();

			string cmdText = "insert into table_name(field1,field2) values (?,?) ";

			// loop through all property in the same order as insert command generation
			//SQLite3.BindInt(stmt, 0, 0);

			if (replace)
            {
				return this.Replace(connection, input);
			}

			/* Generate this code as static version
		
			// Create GUID automatically
			if (map.PK != null && map.PK.IsAutoGuid)
			{
				// set PK only when input has no PK value
				if (map.PK.GetValue(obj).Equals(Guid.Empty))
				{
					map.PK.SetValue(obj, Guid.NewGuid());
				}
			}

			// GetInsertCommand function
			{
			var cols = map.InsertColumns;
			string insertSql;
			if (cols.Length == 0 && map.Columns.Length == 1 && map.Columns[0].IsAutoInc)
			{
				insertSql = string.Format("insert {1} into \"{0}\" default values", map.TableName, extra);
			}
			else
			{
				var replacing = string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0;

				if (replacing)
				{
					cols = map.InsertOrReplaceColumns;
				}

				insertSql = string.Format("insert {3} into \"{0}\"({1}) values ({2})", map.TableName,
								   string.Join(",", (from c in cols
													 select "\"" + c.Name + "\"").ToArray()),
								   string.Join(",", (from c in cols
													 select "?").ToArray()), extra);

			}

			var insertCommand = new PreparedSqlLiteInsertCommand(this, insertSql);
			return insertCommand;
			}

			
			var replacing = string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0;

			var cols = replacing ? map.InsertOrReplaceColumns : map.InsertColumns;
			var vals = new object[cols.Length];
			for (var i = 0; i < vals.Length; i++)
			{
				vals[i] = cols[i].GetValue(obj);
			}

			var insertCmd = GetInsertCommand(map, extra);

			// A SQLite prepared statement can be bound for only one operation at a time.
				try
				{
					count = insertCmd.ExecuteNonQuery(vals);
				}
				catch (SQLiteException ex)
				{
					if (SQLite3.ExtendedErrCode(this.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
					{
						throw NotNullConstraintViolationException.New(ex.Result, ex.Message, map, obj);
					}
					throw;
				}

				if (map.HasAutoIncPK)
				{
					var id = SQLite3.LastInsertRowid(Handle);
					map.SetAutoIncPK(obj, id);
				}

			 */
		}

		protected virtual int Replace(SQLiteConnection connection, T obj)
		{
			throw new NotImplementedException();

			/*Generate this code as static version
			
			cols = map.InsertOrReplaceColumns;
				insertSql = string.Format("insert {3} into \"{0}\"({1}) values ({2})", map.TableName,
								   string.Join(",", (from c in cols
													 select "\"" + c.Name + "\"").ToArray()),
								   string.Join(",", (from c in cols
													 select "?").ToArray()), extra);

			 */
		}

		public virtual int Update(SQLiteConnection connection, T input)
		{
			throw new NotImplementedException();
			/*
			 
			var pk = map.PK;

			if (pk == null)
			{
				throw new NotSupportedException("Cannot update " + map.TableName + ": it has no PK");
			}

			var cols = from p in map.Columns
					   where p != pk
					   select p;
			var vals = from c in cols
					   select c.GetValue(obj);
			var ps = new List<object>(vals);
			if (ps.Count == 0)
			{
				// There is a PK but no accompanying data,
				// so reset the PK to make the UPDATE work.
				cols = map.Columns;
				vals = from c in cols
					   select c.GetValue(obj);
				ps = new List<object>(vals);
			}
			ps.Add(pk.GetValue(obj));
			var q = string.Format("update \"{0}\" set {1} where {2} = ? ", map.TableName, string.Join(",", (from c in cols
																											select "\"" + c.Name + "\" = ? ").ToArray()), pk.Name);

			try
			{
				rowsAffected = Execute(q, ps.ToArray());
			}
			catch (SQLiteException ex)
			{

				if (ex.Result == SQLite3.Result.Constraint && SQLite3.ExtendedErrCode(this.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
				{
					throw NotNullConstraintViolationException.New(ex, map, obj);
				}

				throw ex;
			}


			 */
		}

		public virtual int Delete(SQLiteConnection conn, T input)
		{
			throw new NotImplementedException();
			/*
			var pk = map.PK;
			if (pk == null)
			{
				throw new NotSupportedException("Cannot delete " + map.TableName + ": it has no PK");
			}
			var q = string.Format("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
			var count = Execute(q, pk.GetValue(objectToDelete));
			if (count > 0)
				OnTableChanged(map, NotifyTableChangedAction.Delete);
			return count;*/
		}

		public virtual int DeleteByPrimaryKey(SQLiteConnection conn, object input)
		{
			throw new NotImplementedException();
			/*
			var pk = map.PK;
			if (pk == null)
			{
				throw new NotSupportedException("Cannot delete " + map.TableName + ": it has no PK");
			}
			var q = string.Format("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
			var count = Execute(q, pk.GetValue(objectToDelete));
			if (count > 0)
				OnTableChanged(map, NotifyTableChangedAction.Delete);
			return count;*/
		}

		public virtual int DeleteAll(SQLiteConnection conn)
		{
			var query = $"delete from \"{this.TableName}\"";
			return conn.ExecuteNonQuery(query);
		}

		public virtual T ReadStatementResult(Sqlite3Statement stmt, string[] columnNames = null)
		{
			/*
				// Prepare Mapping for the given statement first
			  
			 	var cols = new TableMappingColumn[SQLite3.ColumnCount(stmt)];

				for (int i = 0; i < cols.Length; i++)
				{
					var name = SQLite3.ColumnName16(stmt, i);
					cols[i] = map.FindColumn(name);
				}

				// actually read the object
				
				while (SQLite3.Step(stmt) == SQLite3.Result.Row)
				{
					var obj = Activator.CreateInstance(map.MappedType);
					for (int i = 0; i < cols.Length; i++)
					{
						if (cols[i] == null)
							continue;

						var colType = SQLite3.ColumnType(stmt, i);
						var val = ReadCol(stmt, i, colType, cols[i].ColumnType);
						cols[i].SetValue(obj, val);
					}
					OnInstanceCreated(obj);
					yield return (T)obj;
				}

			 */

			throw new NotImplementedException();
		}

        public string GetMappedColumnName(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
