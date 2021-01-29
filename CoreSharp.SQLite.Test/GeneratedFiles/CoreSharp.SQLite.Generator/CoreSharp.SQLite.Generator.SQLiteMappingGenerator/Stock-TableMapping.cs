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
        
        protected override string InsertCommand => "create virtual table if not exists \"Stock\" using fts3 (\"Id\" integer primary key autoincrement ,\"Symbol\" varchar )";
        protected override string ReplaceCommand => "insert or replace into \"Stock\"(\"Id\",\"Symbol\") values (?,?)";
        protected override string UpdateCommand => "update \"Stock\" set \"Symbol\" = ?  where Id = ? ";
        protected override string DeleteCommand => "delete from \"Stock\" where \"Id\" = ?";

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

            
                // has auto inc pk
            
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
            connection.ExecuteNonQuery("create virtual table if not exists \"Stock\" using fts3 (\"Id\" integer primary key autoincrement ,\"Symbol\" varchar )");
        }
        
        public override void MigrateTable(SQLiteConnection connection, List<string> existingColumns)
        {
			// this dictionary contains list of columns and
			// commands to alter table and create column
			// existing column will remove item in this dictionary            
            var allColumns = new Dictionary<string, string>();

            /* var addCol = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(p, StoreDateTimeAsTicks, StoreTimeSpanAsTicks);*/
            allColumns["Id"] = "alter table \"Stock\" add column \"Id\" integer primary key autoincrement ";
            allColumns["Symbol"] = "alter table \"Stock\" add column \"Symbol\" varchar ";

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
        
        public override int Insert(SQLiteConnection connection, Stock input, bool replace)
        {
			if (replace)
            {
                return this.Replace(connection, input);
            }            
            
            

            var cmd = this.GetPreparedInsertCommand(connection);
            cmd.ParameterBinder = (stmt) =>
            {
                SQLite3.BindText( (stmt), 0, input.Symbol , -1, _NegativePtr );
                
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
                SQLite3.BindText( (stmt), 1, input.Symbol , -1, _NegativePtr );
                
            };

			cmd.ExecuteNonQuery();

            return 0;
        }
        
        public override int Update(SQLiteConnection connection, Stock input)
		{
            var cmd = this.GetPreparedUpdateCommand(connection);
			cmd.ParameterBinder = (stmt) =>
			{
                SQLite3.BindText( (stmt), 0, input.Symbol , -1, _NegativePtr );
                SQLite3.BindInt( (stmt), 1, input.Id  );
            };
            
			return cmd.ExecuteNonQuery();
        }
        
		public override int Delete(SQLiteConnection connection, Stock input)
		{
            var cmd = this.GetPreparedDeleteCommand(connection);
			cmd.ParameterBinder = (stmt) =>
			{
                SQLite3.BindInt( (stmt), 0, input.Id  );
            };
            
			return cmd.ExecuteNonQuery();
        }
        
		public int DeleteByPrimaryKey(SQLiteConnection connection, int pk)
		{
            var cmd = this.GetPreparedDeleteCommand(connection);
			cmd.ParameterBinder = (stmt) =>
			{
                SQLite3.BindInt( (stmt), 0, pk  );
			};
			return cmd.ExecuteNonQuery();
        }

		public override int DeleteByPrimaryKey(SQLiteConnection connection, object pk)
		{
            var cmd = this.GetPreparedDeleteCommand(connection);
			cmd.ParameterBinder = (stmt) =>
			{
                SQLite3.BindInt( (stmt), 0, (int)pk  );
			};
			return cmd.ExecuteNonQuery();
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
                    case "Symbol":
                        result.Symbol = SQLite3.ColumnString( (stmt), i);
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
            
            result.Id = (int)SQLite3.ColumnInt( (stmt), 0);
            result.Symbol = SQLite3.ColumnString( (stmt), 1);
            
            return result;
        }


    }
}