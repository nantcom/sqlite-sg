using System;
using System.Text.Json;
using System.Collections.Generic;
using CoreSharp.SQLite;

using Sqlite3Statement = System.IntPtr;

namespace CoreSharp.SQLite.Test
{
    /// <summary>
    /// Auto-Generated Table Mapping Class for Stock
    /// </summary>
    public class StockTableMapping : StaticTableMapping<Stock>
    {
        /// <summary>
        /// Name of the Table created in SQLite Database (Set to Stock)
        /// </summary>
        public override string TableName => "Stock";
        
        /// <summary>
        /// CreateFlags as set in source ImplicitIndex, FullTextSearch3
        /// </summary>
        public virtual CreateFlags Flags => (CreateFlags)258;
        
        protected override string InsertCommand => "create virtual table if not exists \"Stock\" using fts3 (\"Id\" integer ,\"Symbol\" varchar ,\"Valuations\" varchar )";
        protected override string ReplaceCommand => "insert or replace into \"Stock\"(\"Id\",\"Symbol\",\"Valuations\") values (?,?,?)";
        protected override string UpdateCommand => "CANNOT UPDATE DUE TO NO PK";
        protected override string DeleteCommand => "CANNOT DELETE DUE TO NO PK";

        /// <summary>
        /// Create New Instance of StockTableMapping
        /// </summary>
        public StockTableMapping()
        {
            this.Columns = new();

            
            this.Columns["Id"] = new StaticColumnMapping<Stock>()
            {
                ColumnName = "Id",
                Setter = (target, value) => target.Id = (int)value,
                Getter = (source) => source.Id
            };
            this.Columns["Symbol"] = new StaticColumnMapping<Stock>()
            {
                ColumnName = "Symbol",
                Setter = (target, value) => target.Symbol = (string)value,
                Getter = (source) => source.Symbol
            };
            this.Columns["Valuations"] = new StaticColumnMapping<Stock>()
            {
                ColumnName = "Valuations",
                Setter = (target, value) => target.Valuations = (CoreSharp.SQLite.Test.Valuation[])value,
                Getter = (source) => source.Valuations
            };

            
        }

        /// <summary>
        /// Drops the table
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public override int DropTable( SQLiteConnection connection )
        {
            return connection.ExecuteNonQuery("drop table if exists \"Stock\"");
        }

        public override void CreateTable(SQLiteConnection connection)
        {
            connection.ExecuteNonQuery("create virtual table if not exists \"Stock\" using fts3 (\"Id\" integer ,\"Symbol\" varchar ,\"Valuations\" varchar )");
        }
        
        public override void MigrateTable(SQLiteConnection connection, List<string> existingColumns)
        {
			// this dictionary contains list of columns and
			// commands to alter table and create column
			// existing column will remove item in this dictionary            
            var allColumns = new Dictionary<string, string>();

            /* var addCol = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(p, StoreDateTimeAsTicks, StoreTimeSpanAsTicks);*/
            allColumns["Id"] = "alter table \"Stock\" add column \"Id\" integer ";
            allColumns["Symbol"] = "alter table \"Stock\" add column \"Symbol\" varchar ";
            allColumns["Valuations"] = "alter table \"Stock\" add column \"Valuations\" varchar ";

            // remove columns that already exists in the database
            foreach (var column in existingColumns)
            {
                if (allColumns.ContainsKey(column))
                {
					allColumns.Remove(column);
                }
            }

            // allColumns now contains only missing column
            // execute command to create them
            foreach (var column in allColumns)
            {
				connection.ExecuteNonQuery(column.Value);
			}
        }
        
        public override void CreateIndex(SQLiteConnection connection)
        {
            
            connection.ExecuteNonQuery( "create  index if not exists \"Stock_Id\" on \"Stock\"(\"Id\")" );
            
        }
        
        public override int Insert(SQLiteConnection connection, Stock input, bool replace)
        {
			if (replace)
            {
                return this.Replace(connection, input);
            }            
            
            

            var cmd = this.GetPreparedInsertCommand(connection);
            cmd.ParameterBinder = (stmt) =>
            {
                SQLite3.BindInt( (stmt), 0, input.Id  );
                SQLite3.BindText( (stmt), 1, input.Symbol , -1, new IntPtr(-1) );
                SQLite3.BindText( (stmt), 2, JsonSerializer.Serialize(input.Valuations) , -1, new IntPtr(-1) );
                
            };

			cmd.ExecuteNonQuery();

            return 0;
        }

		protected override int Replace(SQLiteConnection connection, Stock input)
		{
			var cmd = this.GetPreparedReplaceCommand(connection);
            cmd.ParameterBinder = (stmt) =>
            {
                SQLite3.BindInt( (stmt), 0, input.Id  );
                SQLite3.BindText( (stmt), 1, input.Symbol , -1, new IntPtr(-1) );
                SQLite3.BindText( (stmt), 2, JsonSerializer.Serialize(input.Valuations) , -1, new IntPtr(-1) );
                
            };

			cmd.ExecuteNonQuery();

            return 0;
        }
        
        

        public override Stock ReadStatementResult(Sqlite3Statement stmt, string[] columnNames = null)
		{
            if (columnNames == null)
            {
				// static fast reading code
				return this.ReadSequentialColumnFromStatement(stmt);
            }

            Stock result = new();

            for (int i = 0; i < columnNames.Length; i++)
            {
                switch (columnNames[i])
                {
                    case "Id":
                        result.Id = SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "Symbol":
                        result.Symbol = SQLite3.ColumnString( (stmt), i);
                        break;        
                    case "Valuations":
                        result.Valuations = JsonSerializer.Deserialize<CoreSharp.SQLite.Test.Valuation[]>(SQLite3.ColumnString( (stmt), i));
                        break;        
                    
                    default:
                        break;
                }
            }

            return result;
		}
        
        protected override Stock ReadSequentialColumnFromStatement(Sqlite3Statement stmt)
        {
            Stock result = new();
            
            result.Id = SQLite3.ColumnInt( (stmt), 0);
            result.Symbol = SQLite3.ColumnString( (stmt), 1);
            result.Valuations = JsonSerializer.Deserialize<CoreSharp.SQLite.Test.Valuation[]>(SQLite3.ColumnString( (stmt), 2));
            
            return result;
        }


    }
}