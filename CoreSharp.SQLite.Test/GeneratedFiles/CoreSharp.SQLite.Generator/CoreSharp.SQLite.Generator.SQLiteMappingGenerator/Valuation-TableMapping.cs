using System;
using System.Text.Json;
using System.Collections.Generic;
using CoreSharp.SQLite;

using Sqlite3Statement = System.IntPtr;

namespace CoreSharp.SQLite.Test
{
    /// <summary>
    /// Auto-Generated Table Mapping Class for Valuation
    /// </summary>
    public class ValuationTableMapping : StaticTableMapping<Valuation>
    {
        /// <summary>
        /// Name of the Table created in SQLite Database (Set to Valuation)
        /// </summary>
        public override string TableName => "Valuation";
        
        /// <summary>
        /// CreateFlags as set in source None
        /// </summary>
        public virtual CreateFlags Flags => (CreateFlags)0;
        
        protected override string InsertCommand => "create  table if not exists \"Valuation\"  (\"Id\" varchar(36) ,\"StockId\" integer ,\"Time\" bigint ,\"Price\" varchar )";
        protected override string ReplaceCommand => "insert or replace into \"Valuation\"(\"Id\",\"StockId\",\"Time\",\"Price\") values (?,?,?,?)";
        protected override string UpdateCommand => "CANNOT UPDATE DUE TO NO PK";
        protected override string DeleteCommand => "CANNOT DELETE DUE TO NO PK";

        /// <summary>
        /// Create New Instance of ValuationTableMapping
        /// </summary>
        public ValuationTableMapping()
        {
            this.Columns = new();

            
            this.Columns["Id"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Id",
                Setter = (target, value) => target.Id = (System.Guid)value,
                Getter = (source) => source.Id
            };
            this.Columns["StockId"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "StockId",
                Setter = (target, value) => target.StockId = (int)value,
                Getter = (source) => source.StockId
            };
            this.Columns["Time"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Time",
                Setter = (target, value) => target.Time = (System.DateTime)value,
                Getter = (source) => source.Time
            };
            this.Columns["Price"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Price",
                Setter = (target, value) => target.Price = (decimal)value,
                Getter = (source) => source.Price
            };

            
        }

        /// <summary>
        /// Drops the table
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public override int DropTable( SQLiteConnection connection )
        {
            return connection.ExecuteNonQuery("drop table if exists \"Valuation\"");
        }

        public override void CreateTable(SQLiteConnection connection)
        {
            connection.ExecuteNonQuery("create  table if not exists \"Valuation\"  (\"Id\" varchar(36) ,\"StockId\" integer ,\"Time\" bigint ,\"Price\" varchar )");
        }
        
        public override void MigrateTable(SQLiteConnection connection, List<string> existingColumns)
        {
			// this dictionary contains list of columns and
			// commands to alter table and create column
			// existing column will remove item in this dictionary            
            var allColumns = new Dictionary<string, string>();

            /* var addCol = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(p, StoreDateTimeAsTicks, StoreTimeSpanAsTicks);*/
            allColumns["Id"] = "alter table \"Valuation\" add column \"Id\" varchar(36) ";
            allColumns["StockId"] = "alter table \"Valuation\" add column \"StockId\" integer ";
            allColumns["Time"] = "alter table \"Valuation\" add column \"Time\" bigint ";
            allColumns["Price"] = "alter table \"Valuation\" add column \"Price\" varchar ";

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
            
        }
        
        public override int Insert(SQLiteConnection connection, Valuation input, bool replace)
        {
			if (replace)
            {
                return this.Replace(connection, input);
            }            
            
            

            var cmd = this.GetPreparedInsertCommand(connection);
            cmd.ParameterBinder = (stmt) =>
            {
                SQLite3.BindText( (stmt), 0, input.Id.ToString() , -1, new IntPtr(-1) );
                SQLite3.BindInt( (stmt), 1, input.StockId  );
                SQLite3.BindInt64( (stmt), 2, input.Time.Ticks  );
                SQLite3.BindText( (stmt), 3, JsonSerializer.Serialize(input.Price) , -1, new IntPtr(-1) );
                
            };

			cmd.ExecuteNonQuery();

            return 0;
        }

		protected override int Replace(SQLiteConnection connection, Valuation input)
		{
			var cmd = this.GetPreparedReplaceCommand(connection);
            cmd.ParameterBinder = (stmt) =>
            {
                SQLite3.BindText( (stmt), 0, input.Id.ToString() , -1, new IntPtr(-1) );
                SQLite3.BindInt( (stmt), 1, input.StockId  );
                SQLite3.BindInt64( (stmt), 2, input.Time.Ticks  );
                SQLite3.BindText( (stmt), 3, JsonSerializer.Serialize(input.Price) , -1, new IntPtr(-1) );
                
            };

			cmd.ExecuteNonQuery();

            return 0;
        }
        
        

        public override Valuation ReadStatementResult(Sqlite3Statement stmt, string[] columnNames = null)
		{
            if (columnNames == null)
            {
				// static fast reading code
				return this.ReadSequentialColumnFromStatement(stmt);
            }

            Valuation result = new();

            for (int i = 0; i < columnNames.Length; i++)
            {
                switch (columnNames[i])
                {
                    case "Id":
                        result.Id = new System.Guid(SQLite3.ColumnString( (stmt), i));
                        break;        
                    case "StockId":
                        result.StockId = SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "Time":
                        result.Time = new System.DateTime(SQLite3.ColumnInt64( (stmt), i));
                        break;        
                    case "Price":
                        result.Price = JsonSerializer.Deserialize<decimal>(SQLite3.ColumnString( (stmt), i));
                        break;        
                    
                    default:
                        break;
                }
            }

            return result;
		}
        
        protected override Valuation ReadSequentialColumnFromStatement(Sqlite3Statement stmt)
        {
            Valuation result = new();
            
            result.Id = new System.Guid(SQLite3.ColumnString( (stmt), 0));
            result.StockId = SQLite3.ColumnInt( (stmt), 1);
            result.Time = new System.DateTime(SQLite3.ColumnInt64( (stmt), 2));
            result.Price = JsonSerializer.Deserialize<decimal>(SQLite3.ColumnString( (stmt), 3));
            
            return result;
        }


    }
}
}