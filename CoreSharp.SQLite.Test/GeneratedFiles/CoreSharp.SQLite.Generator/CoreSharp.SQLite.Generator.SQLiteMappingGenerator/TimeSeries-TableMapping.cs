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
    /// Auto-Generated Table Mapping Class for TimeSeries
    /// </summary>
    public class TimeSeriesTableMapping : StaticTableMapping<TimeSeries>
    {
        /// <summary>
        /// Name of the Table created in SQLite Database (Set to TimeSeries)
        /// </summary>
        public override string TableName => "TimeSeries";
        
        /// <summary>
        /// CreateFlags as set in source None
        /// </summary>
        public virtual CreateFlags Flags => (CreateFlags)0;
        
        protected override string InsertCommand => "create  table if not exists \"TimeSeries\"  (\"MyProperty\" integer )";
        protected override string ReplaceCommand => "insert or replace into \"TimeSeries\"(\"MyProperty\") values (?)";
        protected override string UpdateCommand => "CANNOT UPDATE DUE TO NO PK";
        protected override string DeleteCommand => "CANNOT DELETE DUE TO NO PK";

        /// <summary>
        /// Create New Instance of TimeSeriesTableMapping
        /// </summary>
        public TimeSeriesTableMapping()
        {
            this.Columns = new();

            
            this.Columns["MyProperty"] = new StaticColumnMapping<TimeSeries>()
            {
                ColumnName = "MyProperty",
                Setter = (target, value) => target.MyProperty = (int)value,
                Getter = (source) => source.MyProperty
            };

            
        }

        /// <summary>
        /// Drops the table
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public override int DropTable( SQLiteConnection connection )
        {
            return connection.ExecuteNonQuery("drop table if exists \"TimeSeries\"");
        }

        public override void CreateTable(SQLiteConnection connection)
        {
            connection.ExecuteNonQuery("create  table if not exists \"TimeSeries\"  (\"MyProperty\" integer )");
        }
        
        public override void MigrateTable(SQLiteConnection connection, List<string> existingColumns)
        {
			// this dictionary contains list of columns and
			// commands to alter table and create column
			// existing column will remove item in this dictionary            
            var allColumns = new Dictionary<string, string>();

            /* var addCol = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(p, StoreDateTimeAsTicks, StoreTimeSpanAsTicks);*/
            allColumns["MyProperty"] = "alter table \"TimeSeries\" add column \"MyProperty\" integer ";

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
        
        public override int Insert(SQLiteConnection connection, TimeSeries input, bool replace)
        {
			if (replace)
            {
                return this.Replace(connection, input);
            }            
            
            

            var cmd = this.GetPreparedInsertCommand(connection);
            cmd.ParameterBinder = (stmt) =>
            {
                SQLite3.BindInt( (stmt), 0, input.MyProperty  );
                
            };

			cmd.ExecuteNonQuery();

            return 0;
        }

		protected override int Replace(SQLiteConnection connection, TimeSeries input)
		{
			var cmd = this.GetPreparedReplaceCommand(connection);
            cmd.ParameterBinder = (stmt) =>
            {
                SQLite3.BindInt( (stmt), 0, input.MyProperty  );
                
            };

			cmd.ExecuteNonQuery();

            return 0;
        }
        
        

        public override TimeSeries ReadStatementResult(Sqlite3Statement stmt, string[] columnNames = null)
		{
            if (columnNames == null)
            {
				// static fast reading code
				return this.ReadSequentialColumnFromStatement(stmt);
            }

            TimeSeries result = new();

            for (int i = 0; i < columnNames.Length; i++)
            {
                switch (columnNames[i])
                {
                    case "MyProperty":
                        result.MyProperty = (int)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    
                    default:
                        break;
                }
            }

            return result;
		}
        
        protected override TimeSeries ReadSequentialColumnFromStatement(Sqlite3Statement stmt)
        {
            TimeSeries result = new();
            
            result.MyProperty = (int)SQLite3.ColumnInt( (stmt), 0);
            
            return result;
        }


    }
}