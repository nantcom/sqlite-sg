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
        
        protected override string InsertCommand => "create  table if not exists \"Valuation\"  (\"Id\" varchar(36) primary key ,\"StockId\" integer ,\"Time\" bigint ,\"Price\" float ,\"Boolean\" varchar ,\"Byte\" integer ,\"SByte\" integer ,\"Int16\" integer ,\"Int32\" integer ,\"UInt16\" integer ,\"Bool_System\" varchar ,\"SByte_System\" integer ,\"Int16_System\" integer ,\"Int32_System\" integer ,\"UInt16_System\" integer ,\"byte_keyword\" integer ,\"Byte_keyword\" integer ,\"short_keyword\" integer ,\"int_keyword\" integer ,\"float_keyword\" float ,\"double_keyword\" float ,\"decimal_keyword\" float ,\"Decimal\" float ,\"string_keyword\" varchar ,\"String\" varchar ,\"StringBuilder\" varchar ,\"StringBuilder_System\" varchar ,\"Uri\" varchar ,\"Uri_System\" varchar ,\"UriBuilder\" varchar ,\"UriBuilder_System\" varchar ,\"UInt32\" bigint ,\"Int64\" bigint ,\"TimeSpan\" bigint ,\"DateTime\" bigint ,\"DateTimeOffset\" bigint ,\"TimeSpan_system\" bigint ,\"DateTime_system\" bigint ,\"DateTimeOffset_system\" bigint ,\"Int64_system\" bigint ,\"UInt32_system\" bigint ,\"long_keyword\" bigint ,\"uint_keyword\" bigint ,\"bytearray_keyword\" blob ,\"bytearray\" blob ,\"bytearray_system\" blob ,\"Guid\" varchar(36) ,\"Guid_system\" varchar(36) )";
        protected override string ReplaceCommand => "insert or replace into \"Valuation\"(\"Id\",\"StockId\",\"Time\",\"Price\",\"Boolean\",\"Byte\",\"SByte\",\"Int16\",\"Int32\",\"UInt16\",\"Bool_System\",\"SByte_System\",\"Int16_System\",\"Int32_System\",\"UInt16_System\",\"byte_keyword\",\"Byte_keyword\",\"short_keyword\",\"int_keyword\",\"float_keyword\",\"double_keyword\",\"decimal_keyword\",\"Decimal\",\"string_keyword\",\"String\",\"StringBuilder\",\"StringBuilder_System\",\"Uri\",\"Uri_System\",\"UriBuilder\",\"UriBuilder_System\",\"UInt32\",\"Int64\",\"TimeSpan\",\"DateTime\",\"DateTimeOffset\",\"TimeSpan_system\",\"DateTime_system\",\"DateTimeOffset_system\",\"Int64_system\",\"UInt32_system\",\"long_keyword\",\"uint_keyword\",\"bytearray_keyword\",\"bytearray\",\"bytearray_system\",\"Guid\",\"Guid_system\") values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
        protected override string UpdateCommand => "update \"Valuation\" set \"StockId\" = ? ,\"Time\" = ? ,\"Price\" = ? ,\"Boolean\" = ? ,\"Byte\" = ? ,\"SByte\" = ? ,\"Int16\" = ? ,\"Int32\" = ? ,\"UInt16\" = ? ,\"Bool_System\" = ? ,\"SByte_System\" = ? ,\"Int16_System\" = ? ,\"Int32_System\" = ? ,\"UInt16_System\" = ? ,\"byte_keyword\" = ? ,\"Byte_keyword\" = ? ,\"short_keyword\" = ? ,\"int_keyword\" = ? ,\"float_keyword\" = ? ,\"double_keyword\" = ? ,\"decimal_keyword\" = ? ,\"Decimal\" = ? ,\"string_keyword\" = ? ,\"String\" = ? ,\"StringBuilder\" = ? ,\"StringBuilder_System\" = ? ,\"Uri\" = ? ,\"Uri_System\" = ? ,\"UriBuilder\" = ? ,\"UriBuilder_System\" = ? ,\"UInt32\" = ? ,\"Int64\" = ? ,\"TimeSpan\" = ? ,\"DateTime\" = ? ,\"DateTimeOffset\" = ? ,\"TimeSpan_system\" = ? ,\"DateTime_system\" = ? ,\"DateTimeOffset_system\" = ? ,\"Int64_system\" = ? ,\"UInt32_system\" = ? ,\"long_keyword\" = ? ,\"uint_keyword\" = ? ,\"bytearray_keyword\" = ? ,\"bytearray\" = ? ,\"bytearray_system\" = ? ,\"Guid\" = ? ,\"Guid_system\" = ?  where Id = ? ";
        protected override string DeleteCommand => "delete from \"Valuation\" where \"Id\" = ?";

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
            this.Columns["Boolean"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Boolean",
                Setter = (target, value) => target.Boolean = (bool)value,
                Getter = (source) => source.Boolean
            };
            this.Columns["Byte"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Byte",
                Setter = (target, value) => target.Byte = (byte)value,
                Getter = (source) => source.Byte
            };
            this.Columns["SByte"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "SByte",
                Setter = (target, value) => target.SByte = (sbyte)value,
                Getter = (source) => source.SByte
            };
            this.Columns["Int16"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Int16",
                Setter = (target, value) => target.Int16 = (short)value,
                Getter = (source) => source.Int16
            };
            this.Columns["Int32"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Int32",
                Setter = (target, value) => target.Int32 = (int)value,
                Getter = (source) => source.Int32
            };
            this.Columns["UInt16"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "UInt16",
                Setter = (target, value) => target.UInt16 = (ushort)value,
                Getter = (source) => source.UInt16
            };
            this.Columns["Bool_System"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Bool_System",
                Setter = (target, value) => target.Bool_System = (bool)value,
                Getter = (source) => source.Bool_System
            };
            this.Columns["SByte_System"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "SByte_System",
                Setter = (target, value) => target.SByte_System = (sbyte)value,
                Getter = (source) => source.SByte_System
            };
            this.Columns["Int16_System"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Int16_System",
                Setter = (target, value) => target.Int16_System = (short)value,
                Getter = (source) => source.Int16_System
            };
            this.Columns["Int32_System"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Int32_System",
                Setter = (target, value) => target.Int32_System = (int)value,
                Getter = (source) => source.Int32_System
            };
            this.Columns["UInt16_System"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "UInt16_System",
                Setter = (target, value) => target.UInt16_System = (ushort)value,
                Getter = (source) => source.UInt16_System
            };
            this.Columns["byte_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "byte_keyword",
                Setter = (target, value) => target.byte_keyword = (byte)value,
                Getter = (source) => source.byte_keyword
            };
            this.Columns["Byte_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Byte_keyword",
                Setter = (target, value) => target.Byte_keyword = (sbyte)value,
                Getter = (source) => source.Byte_keyword
            };
            this.Columns["short_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "short_keyword",
                Setter = (target, value) => target.short_keyword = (short)value,
                Getter = (source) => source.short_keyword
            };
            this.Columns["int_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "int_keyword",
                Setter = (target, value) => target.int_keyword = (int)value,
                Getter = (source) => source.int_keyword
            };
            this.Columns["float_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "float_keyword",
                Setter = (target, value) => target.float_keyword = (float)value,
                Getter = (source) => source.float_keyword
            };
            this.Columns["double_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "double_keyword",
                Setter = (target, value) => target.double_keyword = (double)value,
                Getter = (source) => source.double_keyword
            };
            this.Columns["decimal_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "decimal_keyword",
                Setter = (target, value) => target.decimal_keyword = (decimal)value,
                Getter = (source) => source.decimal_keyword
            };
            this.Columns["Decimal"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Decimal",
                Setter = (target, value) => target.Decimal = (decimal)value,
                Getter = (source) => source.Decimal
            };
            this.Columns["string_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "string_keyword",
                Setter = (target, value) => target.string_keyword = (string)value,
                Getter = (source) => source.string_keyword
            };
            this.Columns["String"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "String",
                Setter = (target, value) => target.String = (string)value,
                Getter = (source) => source.String
            };
            this.Columns["StringBuilder"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "StringBuilder",
                Setter = (target, value) => target.StringBuilder = (System.Text.StringBuilder)value,
                Getter = (source) => source.StringBuilder
            };
            this.Columns["StringBuilder_System"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "StringBuilder_System",
                Setter = (target, value) => target.StringBuilder_System = (System.Text.StringBuilder)value,
                Getter = (source) => source.StringBuilder_System
            };
            this.Columns["Uri"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Uri",
                Setter = (target, value) => target.Uri = (System.Uri)value,
                Getter = (source) => source.Uri
            };
            this.Columns["Uri_System"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Uri_System",
                Setter = (target, value) => target.Uri_System = (System.Uri)value,
                Getter = (source) => source.Uri_System
            };
            this.Columns["UriBuilder"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "UriBuilder",
                Setter = (target, value) => target.UriBuilder = (System.UriBuilder)value,
                Getter = (source) => source.UriBuilder
            };
            this.Columns["UriBuilder_System"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "UriBuilder_System",
                Setter = (target, value) => target.UriBuilder_System = (System.UriBuilder)value,
                Getter = (source) => source.UriBuilder_System
            };
            this.Columns["UInt32"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "UInt32",
                Setter = (target, value) => target.UInt32 = (uint)value,
                Getter = (source) => source.UInt32
            };
            this.Columns["Int64"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Int64",
                Setter = (target, value) => target.Int64 = (long)value,
                Getter = (source) => source.Int64
            };
            this.Columns["TimeSpan"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "TimeSpan",
                Setter = (target, value) => target.TimeSpan = (System.TimeSpan)value,
                Getter = (source) => source.TimeSpan
            };
            this.Columns["DateTime"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "DateTime",
                Setter = (target, value) => target.DateTime = (System.DateTime)value,
                Getter = (source) => source.DateTime
            };
            this.Columns["DateTimeOffset"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "DateTimeOffset",
                Setter = (target, value) => target.DateTimeOffset = (System.DateTimeOffset)value,
                Getter = (source) => source.DateTimeOffset
            };
            this.Columns["TimeSpan_system"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "TimeSpan_system",
                Setter = (target, value) => target.TimeSpan_system = (System.TimeSpan)value,
                Getter = (source) => source.TimeSpan_system
            };
            this.Columns["DateTime_system"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "DateTime_system",
                Setter = (target, value) => target.DateTime_system = (System.DateTime)value,
                Getter = (source) => source.DateTime_system
            };
            this.Columns["DateTimeOffset_system"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "DateTimeOffset_system",
                Setter = (target, value) => target.DateTimeOffset_system = (System.DateTimeOffset)value,
                Getter = (source) => source.DateTimeOffset_system
            };
            this.Columns["Int64_system"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Int64_system",
                Setter = (target, value) => target.Int64_system = (long)value,
                Getter = (source) => source.Int64_system
            };
            this.Columns["UInt32_system"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "UInt32_system",
                Setter = (target, value) => target.UInt32_system = (uint)value,
                Getter = (source) => source.UInt32_system
            };
            this.Columns["long_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "long_keyword",
                Setter = (target, value) => target.long_keyword = (long)value,
                Getter = (source) => source.long_keyword
            };
            this.Columns["uint_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "uint_keyword",
                Setter = (target, value) => target.uint_keyword = (uint)value,
                Getter = (source) => source.uint_keyword
            };
            this.Columns["bytearray_keyword"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "bytearray_keyword",
                Setter = (target, value) => target.bytearray_keyword = (byte[])value,
                Getter = (source) => source.bytearray_keyword
            };
            this.Columns["bytearray"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "bytearray",
                Setter = (target, value) => target.bytearray = (byte[])value,
                Getter = (source) => source.bytearray
            };
            this.Columns["bytearray_system"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "bytearray_system",
                Setter = (target, value) => target.bytearray_system = (byte[])value,
                Getter = (source) => source.bytearray_system
            };
            this.Columns["Guid"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Guid",
                Setter = (target, value) => target.Guid = (System.Guid)value,
                Getter = (source) => source.Guid
            };
            this.Columns["Guid_system"] = new StaticColumnMapping<Valuation>()
            {
                ColumnName = "Guid_system",
                Setter = (target, value) => target.Guid_system = (System.Guid)value,
                Getter = (source) => source.Guid_system
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
            connection.ExecuteNonQuery("create  table if not exists \"Valuation\"  (\"Id\" varchar(36) primary key ,\"StockId\" integer ,\"Time\" bigint ,\"Price\" float ,\"Boolean\" varchar ,\"Byte\" integer ,\"SByte\" integer ,\"Int16\" integer ,\"Int32\" integer ,\"UInt16\" integer ,\"Bool_System\" varchar ,\"SByte_System\" integer ,\"Int16_System\" integer ,\"Int32_System\" integer ,\"UInt16_System\" integer ,\"byte_keyword\" integer ,\"Byte_keyword\" integer ,\"short_keyword\" integer ,\"int_keyword\" integer ,\"float_keyword\" float ,\"double_keyword\" float ,\"decimal_keyword\" float ,\"Decimal\" float ,\"string_keyword\" varchar ,\"String\" varchar ,\"StringBuilder\" varchar ,\"StringBuilder_System\" varchar ,\"Uri\" varchar ,\"Uri_System\" varchar ,\"UriBuilder\" varchar ,\"UriBuilder_System\" varchar ,\"UInt32\" bigint ,\"Int64\" bigint ,\"TimeSpan\" bigint ,\"DateTime\" bigint ,\"DateTimeOffset\" bigint ,\"TimeSpan_system\" bigint ,\"DateTime_system\" bigint ,\"DateTimeOffset_system\" bigint ,\"Int64_system\" bigint ,\"UInt32_system\" bigint ,\"long_keyword\" bigint ,\"uint_keyword\" bigint ,\"bytearray_keyword\" blob ,\"bytearray\" blob ,\"bytearray_system\" blob ,\"Guid\" varchar(36) ,\"Guid_system\" varchar(36) )");
        }
        
        public override void MigrateTable(SQLiteConnection connection, List<string> existingColumns)
        {
			// this dictionary contains list of columns and
			// commands to alter table and create column
			// existing column will remove item in this dictionary            
            var allColumns = new Dictionary<string, string>();

            /* var addCol = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(p, StoreDateTimeAsTicks, StoreTimeSpanAsTicks);*/
            allColumns["Id"] = "alter table \"Valuation\" add column \"Id\" varchar(36) primary key ";
            allColumns["StockId"] = "alter table \"Valuation\" add column \"StockId\" integer ";
            allColumns["Time"] = "alter table \"Valuation\" add column \"Time\" bigint ";
            allColumns["Price"] = "alter table \"Valuation\" add column \"Price\" float ";
            allColumns["Boolean"] = "alter table \"Valuation\" add column \"Boolean\" varchar ";
            allColumns["Byte"] = "alter table \"Valuation\" add column \"Byte\" integer ";
            allColumns["SByte"] = "alter table \"Valuation\" add column \"SByte\" integer ";
            allColumns["Int16"] = "alter table \"Valuation\" add column \"Int16\" integer ";
            allColumns["Int32"] = "alter table \"Valuation\" add column \"Int32\" integer ";
            allColumns["UInt16"] = "alter table \"Valuation\" add column \"UInt16\" integer ";
            allColumns["Bool_System"] = "alter table \"Valuation\" add column \"Bool_System\" varchar ";
            allColumns["SByte_System"] = "alter table \"Valuation\" add column \"SByte_System\" integer ";
            allColumns["Int16_System"] = "alter table \"Valuation\" add column \"Int16_System\" integer ";
            allColumns["Int32_System"] = "alter table \"Valuation\" add column \"Int32_System\" integer ";
            allColumns["UInt16_System"] = "alter table \"Valuation\" add column \"UInt16_System\" integer ";
            allColumns["byte_keyword"] = "alter table \"Valuation\" add column \"byte_keyword\" integer ";
            allColumns["Byte_keyword"] = "alter table \"Valuation\" add column \"Byte_keyword\" integer ";
            allColumns["short_keyword"] = "alter table \"Valuation\" add column \"short_keyword\" integer ";
            allColumns["int_keyword"] = "alter table \"Valuation\" add column \"int_keyword\" integer ";
            allColumns["float_keyword"] = "alter table \"Valuation\" add column \"float_keyword\" float ";
            allColumns["double_keyword"] = "alter table \"Valuation\" add column \"double_keyword\" float ";
            allColumns["decimal_keyword"] = "alter table \"Valuation\" add column \"decimal_keyword\" float ";
            allColumns["Decimal"] = "alter table \"Valuation\" add column \"Decimal\" float ";
            allColumns["string_keyword"] = "alter table \"Valuation\" add column \"string_keyword\" varchar ";
            allColumns["String"] = "alter table \"Valuation\" add column \"String\" varchar ";
            allColumns["StringBuilder"] = "alter table \"Valuation\" add column \"StringBuilder\" varchar ";
            allColumns["StringBuilder_System"] = "alter table \"Valuation\" add column \"StringBuilder_System\" varchar ";
            allColumns["Uri"] = "alter table \"Valuation\" add column \"Uri\" varchar ";
            allColumns["Uri_System"] = "alter table \"Valuation\" add column \"Uri_System\" varchar ";
            allColumns["UriBuilder"] = "alter table \"Valuation\" add column \"UriBuilder\" varchar ";
            allColumns["UriBuilder_System"] = "alter table \"Valuation\" add column \"UriBuilder_System\" varchar ";
            allColumns["UInt32"] = "alter table \"Valuation\" add column \"UInt32\" bigint ";
            allColumns["Int64"] = "alter table \"Valuation\" add column \"Int64\" bigint ";
            allColumns["TimeSpan"] = "alter table \"Valuation\" add column \"TimeSpan\" bigint ";
            allColumns["DateTime"] = "alter table \"Valuation\" add column \"DateTime\" bigint ";
            allColumns["DateTimeOffset"] = "alter table \"Valuation\" add column \"DateTimeOffset\" bigint ";
            allColumns["TimeSpan_system"] = "alter table \"Valuation\" add column \"TimeSpan_system\" bigint ";
            allColumns["DateTime_system"] = "alter table \"Valuation\" add column \"DateTime_system\" bigint ";
            allColumns["DateTimeOffset_system"] = "alter table \"Valuation\" add column \"DateTimeOffset_system\" bigint ";
            allColumns["Int64_system"] = "alter table \"Valuation\" add column \"Int64_system\" bigint ";
            allColumns["UInt32_system"] = "alter table \"Valuation\" add column \"UInt32_system\" bigint ";
            allColumns["long_keyword"] = "alter table \"Valuation\" add column \"long_keyword\" bigint ";
            allColumns["uint_keyword"] = "alter table \"Valuation\" add column \"uint_keyword\" bigint ";
            allColumns["bytearray_keyword"] = "alter table \"Valuation\" add column \"bytearray_keyword\" blob ";
            allColumns["bytearray"] = "alter table \"Valuation\" add column \"bytearray\" blob ";
            allColumns["bytearray_system"] = "alter table \"Valuation\" add column \"bytearray_system\" blob ";
            allColumns["Guid"] = "alter table \"Valuation\" add column \"Guid\" varchar(36) ";
            allColumns["Guid_system"] = "alter table \"Valuation\" add column \"Guid_system\" varchar(36) ";

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
            
            connection.ExecuteNonQuery( "create  index if not exists \"\" on \"Valuation\"(\"StockId\")" );
            
        }
        
        public override int Insert(SQLiteConnection connection, Valuation input, bool replace)
        {
			if (replace)
            {
                return this.Replace(connection, input);
            }            
            
                        
            if ( input.Id == Guid.Empty )
            {
                input.Id = Guid.NewGuid();
            }

            var cmd = this.GetPreparedInsertCommand(connection);
            cmd.ParameterBinder = (stmt) =>
            {
                SQLite3.BindText( (stmt), 0, input.Id.ToString() , 72, _NegativePtr );
                SQLite3.BindInt( (stmt), 1, input.StockId  );
                SQLite3.BindInt64( (stmt), 2, input.Time.Ticks  );
                SQLite3.BindDouble( (stmt), 3, (double)input.Price  );
                SQLite3.BindText( (stmt), 4, JsonSerializer.Serialize(input.Boolean) , -1, _NegativePtr );
                SQLite3.BindInt( (stmt), 5, (int)input.Byte  );
                SQLite3.BindInt( (stmt), 6, (int)input.SByte  );
                SQLite3.BindInt( (stmt), 7, (int)input.Int16  );
                SQLite3.BindInt( (stmt), 8, input.Int32  );
                SQLite3.BindInt( (stmt), 9, (int)input.UInt16  );
                SQLite3.BindText( (stmt), 10, JsonSerializer.Serialize(input.Bool_System) , -1, _NegativePtr );
                SQLite3.BindInt( (stmt), 11, (int)input.SByte_System  );
                SQLite3.BindInt( (stmt), 12, (int)input.Int16_System  );
                SQLite3.BindInt( (stmt), 13, input.Int32_System  );
                SQLite3.BindInt( (stmt), 14, (int)input.UInt16_System  );
                SQLite3.BindInt( (stmt), 15, (int)input.byte_keyword  );
                SQLite3.BindInt( (stmt), 16, (int)input.Byte_keyword  );
                SQLite3.BindInt( (stmt), 17, (int)input.short_keyword  );
                SQLite3.BindInt( (stmt), 18, input.int_keyword  );
                SQLite3.BindDouble( (stmt), 19, input.float_keyword  );
                SQLite3.BindDouble( (stmt), 20, input.double_keyword  );
                SQLite3.BindDouble( (stmt), 21, (double)input.decimal_keyword  );
                SQLite3.BindDouble( (stmt), 22, (double)input.Decimal  );
                SQLite3.BindText( (stmt), 23, input.string_keyword , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 24, input.String , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 25, input.StringBuilder.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 26, input.StringBuilder_System.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 27, input.Uri.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 28, input.Uri_System.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 29, input.UriBuilder.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 30, input.UriBuilder_System.ToString() , -1, _NegativePtr );
                SQLite3.BindInt64( (stmt), 31, (long)input.UInt32  );
                SQLite3.BindInt64( (stmt), 32, input.Int64  );
                SQLite3.BindInt64( (stmt), 33, input.TimeSpan.Ticks  );
                SQLite3.BindInt64( (stmt), 34, input.DateTime.Ticks  );
                SQLite3.BindInt64( (stmt), 35, input.DateTimeOffset.UtcTicks  );
                SQLite3.BindInt64( (stmt), 36, input.TimeSpan_system.Ticks  );
                SQLite3.BindInt64( (stmt), 37, input.DateTime_system.Ticks  );
                SQLite3.BindInt64( (stmt), 38, input.DateTimeOffset_system.UtcTicks  );
                SQLite3.BindInt64( (stmt), 39, input.Int64_system  );
                SQLite3.BindInt64( (stmt), 40, (long)input.UInt32_system  );
                SQLite3.BindInt64( (stmt), 41, input.long_keyword  );
                SQLite3.BindInt64( (stmt), 42, (long)input.uint_keyword  );
                SQLite3.BindBlob( (stmt), 43, input.bytearray_keyword , input.bytearray_keyword.Length, _NegativePtr );
                SQLite3.BindBlob( (stmt), 44, input.bytearray , input.bytearray.Length, _NegativePtr );
                SQLite3.BindBlob( (stmt), 45, input.bytearray_system , input.bytearray_system.Length, _NegativePtr );
                SQLite3.BindText( (stmt), 46, input.Guid.ToString() , 72, _NegativePtr );
                SQLite3.BindText( (stmt), 47, input.Guid_system.ToString() , 72, _NegativePtr );
                
            };

			cmd.ExecuteNonQuery();

            return 0;
        }

		protected override int Replace(SQLiteConnection connection, Valuation input)
		{
			var cmd = this.GetPreparedReplaceCommand(connection);
            cmd.ParameterBinder = (stmt) =>
            {
                SQLite3.BindText( (stmt), 0, input.Id.ToString() , 72, _NegativePtr );
                SQLite3.BindInt( (stmt), 1, input.StockId  );
                SQLite3.BindInt64( (stmt), 2, input.Time.Ticks  );
                SQLite3.BindDouble( (stmt), 3, (double)input.Price  );
                SQLite3.BindText( (stmt), 4, JsonSerializer.Serialize(input.Boolean) , -1, _NegativePtr );
                SQLite3.BindInt( (stmt), 5, (int)input.Byte  );
                SQLite3.BindInt( (stmt), 6, (int)input.SByte  );
                SQLite3.BindInt( (stmt), 7, (int)input.Int16  );
                SQLite3.BindInt( (stmt), 8, input.Int32  );
                SQLite3.BindInt( (stmt), 9, (int)input.UInt16  );
                SQLite3.BindText( (stmt), 10, JsonSerializer.Serialize(input.Bool_System) , -1, _NegativePtr );
                SQLite3.BindInt( (stmt), 11, (int)input.SByte_System  );
                SQLite3.BindInt( (stmt), 12, (int)input.Int16_System  );
                SQLite3.BindInt( (stmt), 13, input.Int32_System  );
                SQLite3.BindInt( (stmt), 14, (int)input.UInt16_System  );
                SQLite3.BindInt( (stmt), 15, (int)input.byte_keyword  );
                SQLite3.BindInt( (stmt), 16, (int)input.Byte_keyword  );
                SQLite3.BindInt( (stmt), 17, (int)input.short_keyword  );
                SQLite3.BindInt( (stmt), 18, input.int_keyword  );
                SQLite3.BindDouble( (stmt), 19, input.float_keyword  );
                SQLite3.BindDouble( (stmt), 20, input.double_keyword  );
                SQLite3.BindDouble( (stmt), 21, (double)input.decimal_keyword  );
                SQLite3.BindDouble( (stmt), 22, (double)input.Decimal  );
                SQLite3.BindText( (stmt), 23, input.string_keyword , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 24, input.String , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 25, input.StringBuilder.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 26, input.StringBuilder_System.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 27, input.Uri.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 28, input.Uri_System.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 29, input.UriBuilder.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 30, input.UriBuilder_System.ToString() , -1, _NegativePtr );
                SQLite3.BindInt64( (stmt), 31, (long)input.UInt32  );
                SQLite3.BindInt64( (stmt), 32, input.Int64  );
                SQLite3.BindInt64( (stmt), 33, input.TimeSpan.Ticks  );
                SQLite3.BindInt64( (stmt), 34, input.DateTime.Ticks  );
                SQLite3.BindInt64( (stmt), 35, input.DateTimeOffset.UtcTicks  );
                SQLite3.BindInt64( (stmt), 36, input.TimeSpan_system.Ticks  );
                SQLite3.BindInt64( (stmt), 37, input.DateTime_system.Ticks  );
                SQLite3.BindInt64( (stmt), 38, input.DateTimeOffset_system.UtcTicks  );
                SQLite3.BindInt64( (stmt), 39, input.Int64_system  );
                SQLite3.BindInt64( (stmt), 40, (long)input.UInt32_system  );
                SQLite3.BindInt64( (stmt), 41, input.long_keyword  );
                SQLite3.BindInt64( (stmt), 42, (long)input.uint_keyword  );
                SQLite3.BindBlob( (stmt), 43, input.bytearray_keyword , input.bytearray_keyword.Length, _NegativePtr );
                SQLite3.BindBlob( (stmt), 44, input.bytearray , input.bytearray.Length, _NegativePtr );
                SQLite3.BindBlob( (stmt), 45, input.bytearray_system , input.bytearray_system.Length, _NegativePtr );
                SQLite3.BindText( (stmt), 46, input.Guid.ToString() , 72, _NegativePtr );
                SQLite3.BindText( (stmt), 47, input.Guid_system.ToString() , 72, _NegativePtr );
                
            };

			cmd.ExecuteNonQuery();

            return 0;
        }
        
        public override int Update(SQLiteConnection connection, Valuation input)
		{
            var cmd = this.GetPreparedUpdateCommand(connection);
			cmd.ParameterBinder = (stmt) =>
			{
                SQLite3.BindInt( (stmt), 0, input.StockId  );
                SQLite3.BindInt64( (stmt), 1, input.Time.Ticks  );
                SQLite3.BindDouble( (stmt), 2, (double)input.Price  );
                SQLite3.BindText( (stmt), 3, JsonSerializer.Serialize(input.Boolean) , -1, _NegativePtr );
                SQLite3.BindInt( (stmt), 4, (int)input.Byte  );
                SQLite3.BindInt( (stmt), 5, (int)input.SByte  );
                SQLite3.BindInt( (stmt), 6, (int)input.Int16  );
                SQLite3.BindInt( (stmt), 7, input.Int32  );
                SQLite3.BindInt( (stmt), 8, (int)input.UInt16  );
                SQLite3.BindText( (stmt), 9, JsonSerializer.Serialize(input.Bool_System) , -1, _NegativePtr );
                SQLite3.BindInt( (stmt), 10, (int)input.SByte_System  );
                SQLite3.BindInt( (stmt), 11, (int)input.Int16_System  );
                SQLite3.BindInt( (stmt), 12, input.Int32_System  );
                SQLite3.BindInt( (stmt), 13, (int)input.UInt16_System  );
                SQLite3.BindInt( (stmt), 14, (int)input.byte_keyword  );
                SQLite3.BindInt( (stmt), 15, (int)input.Byte_keyword  );
                SQLite3.BindInt( (stmt), 16, (int)input.short_keyword  );
                SQLite3.BindInt( (stmt), 17, input.int_keyword  );
                SQLite3.BindDouble( (stmt), 18, input.float_keyword  );
                SQLite3.BindDouble( (stmt), 19, input.double_keyword  );
                SQLite3.BindDouble( (stmt), 20, (double)input.decimal_keyword  );
                SQLite3.BindDouble( (stmt), 21, (double)input.Decimal  );
                SQLite3.BindText( (stmt), 22, input.string_keyword , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 23, input.String , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 24, input.StringBuilder.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 25, input.StringBuilder_System.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 26, input.Uri.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 27, input.Uri_System.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 28, input.UriBuilder.ToString() , -1, _NegativePtr );
                SQLite3.BindText( (stmt), 29, input.UriBuilder_System.ToString() , -1, _NegativePtr );
                SQLite3.BindInt64( (stmt), 30, (long)input.UInt32  );
                SQLite3.BindInt64( (stmt), 31, input.Int64  );
                SQLite3.BindInt64( (stmt), 32, input.TimeSpan.Ticks  );
                SQLite3.BindInt64( (stmt), 33, input.DateTime.Ticks  );
                SQLite3.BindInt64( (stmt), 34, input.DateTimeOffset.UtcTicks  );
                SQLite3.BindInt64( (stmt), 35, input.TimeSpan_system.Ticks  );
                SQLite3.BindInt64( (stmt), 36, input.DateTime_system.Ticks  );
                SQLite3.BindInt64( (stmt), 37, input.DateTimeOffset_system.UtcTicks  );
                SQLite3.BindInt64( (stmt), 38, input.Int64_system  );
                SQLite3.BindInt64( (stmt), 39, (long)input.UInt32_system  );
                SQLite3.BindInt64( (stmt), 40, input.long_keyword  );
                SQLite3.BindInt64( (stmt), 41, (long)input.uint_keyword  );
                SQLite3.BindBlob( (stmt), 42, input.bytearray_keyword , input.bytearray_keyword.Length, _NegativePtr );
                SQLite3.BindBlob( (stmt), 43, input.bytearray , input.bytearray.Length, _NegativePtr );
                SQLite3.BindBlob( (stmt), 44, input.bytearray_system , input.bytearray_system.Length, _NegativePtr );
                SQLite3.BindText( (stmt), 45, input.Guid.ToString() , 72, _NegativePtr );
                SQLite3.BindText( (stmt), 46, input.Guid_system.ToString() , 72, _NegativePtr );
                SQLite3.BindText( (stmt), 47, input.Id.ToString() , 72, _NegativePtr );
            };
            
			return cmd.ExecuteNonQuery();
        }
        
		public override int Delete(SQLiteConnection connection, Valuation input)
		{
            var cmd = this.GetPreparedDeleteCommand(connection);
			cmd.ParameterBinder = (stmt) =>
			{
                SQLite3.BindText( (stmt), 0, input.Id.ToString() , 72, _NegativePtr );
            };
            
			return cmd.ExecuteNonQuery();
        }
        
		public int DeleteByPrimaryKey(SQLiteConnection connection, System.Guid pk)
		{
            var cmd = this.GetPreparedDeleteCommand(connection);
			cmd.ParameterBinder = (stmt) =>
			{
                SQLite3.BindText( (stmt), 0, pk.ToString() , 72, _NegativePtr );
			};
			return cmd.ExecuteNonQuery();
        }

		public override int DeleteByPrimaryKey(SQLiteConnection connection, object pk)
		{
            var cmd = this.GetPreparedDeleteCommand(connection);
			cmd.ParameterBinder = (stmt) =>
			{
                SQLite3.BindText( (stmt), 0, pk.ToString() , 72, _NegativePtr );
			};
			return cmd.ExecuteNonQuery();
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
                    case "StockId":
                        result.StockId = (int)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "Time":
                        result.Time = new System.DateTime(SQLite3.ColumnInt64( (stmt), i));
                        break;        
                    case "Price":
                        result.Price = (decimal)SQLite3.ColumnDouble( (stmt), i);
                        break;        
                    case "Boolean":
                        result.Boolean = JsonSerializer.Deserialize<bool>(SQLite3.ColumnString( (stmt), i));
                        break;        
                    case "Byte":
                        result.Byte = (byte)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "SByte":
                        result.SByte = (sbyte)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "Int16":
                        result.Int16 = (short)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "Int32":
                        result.Int32 = (int)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "UInt16":
                        result.UInt16 = (ushort)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "Bool_System":
                        result.Bool_System = JsonSerializer.Deserialize<bool>(SQLite3.ColumnString( (stmt), i));
                        break;        
                    case "SByte_System":
                        result.SByte_System = (sbyte)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "Int16_System":
                        result.Int16_System = (short)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "Int32_System":
                        result.Int32_System = (int)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "UInt16_System":
                        result.UInt16_System = (ushort)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "byte_keyword":
                        result.byte_keyword = (byte)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "Byte_keyword":
                        result.Byte_keyword = (sbyte)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "short_keyword":
                        result.short_keyword = (short)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "int_keyword":
                        result.int_keyword = (int)SQLite3.ColumnInt( (stmt), i);
                        break;        
                    case "float_keyword":
                        result.float_keyword = (float)SQLite3.ColumnDouble( (stmt), i);
                        break;        
                    case "double_keyword":
                        result.double_keyword = SQLite3.ColumnDouble( (stmt), i);
                        break;        
                    case "decimal_keyword":
                        result.decimal_keyword = (decimal)SQLite3.ColumnDouble( (stmt), i);
                        break;        
                    case "Decimal":
                        result.Decimal = (decimal)SQLite3.ColumnDouble( (stmt), i);
                        break;        
                    case "string_keyword":
                        result.string_keyword = SQLite3.ColumnString( (stmt), i);
                        break;        
                    case "String":
                        result.String = SQLite3.ColumnString( (stmt), i);
                        break;        
                    case "StringBuilder":
                        result.StringBuilder = new System.Text.StringBuilder(SQLite3.ColumnString( (stmt), i));
                        break;        
                    case "StringBuilder_System":
                        result.StringBuilder_System = new System.Text.StringBuilder(SQLite3.ColumnString( (stmt), i));
                        break;        
                    case "Uri":
                        result.Uri = new System.Uri(SQLite3.ColumnString( (stmt), i));
                        break;        
                    case "Uri_System":
                        result.Uri_System = new System.Uri(SQLite3.ColumnString( (stmt), i));
                        break;        
                    case "UriBuilder":
                        result.UriBuilder = new System.UriBuilder(SQLite3.ColumnString( (stmt), i));
                        break;        
                    case "UriBuilder_System":
                        result.UriBuilder_System = new System.UriBuilder(SQLite3.ColumnString( (stmt), i));
                        break;        
                    case "UInt32":
                        result.UInt32 = (uint)SQLite3.ColumnInt64( (stmt), i);
                        break;        
                    case "Int64":
                        result.Int64 = SQLite3.ColumnInt64( (stmt), i);
                        break;        
                    case "TimeSpan":
                        result.TimeSpan = new System.TimeSpan(SQLite3.ColumnInt64( (stmt), i));
                        break;        
                    case "DateTime":
                        result.DateTime = new System.DateTime(SQLite3.ColumnInt64( (stmt), i));
                        break;        
                    case "DateTimeOffset":
                        result.DateTimeOffset = new System.DateTimeOffset(SQLite3.ColumnInt64( (stmt), i), TimeSpan.Zero);
                        break;        
                    case "TimeSpan_system":
                        result.TimeSpan_system = new System.TimeSpan(SQLite3.ColumnInt64( (stmt), i));
                        break;        
                    case "DateTime_system":
                        result.DateTime_system = new System.DateTime(SQLite3.ColumnInt64( (stmt), i));
                        break;        
                    case "DateTimeOffset_system":
                        result.DateTimeOffset_system = new System.DateTimeOffset(SQLite3.ColumnInt64( (stmt), i), TimeSpan.Zero);
                        break;        
                    case "Int64_system":
                        result.Int64_system = SQLite3.ColumnInt64( (stmt), i);
                        break;        
                    case "UInt32_system":
                        result.UInt32_system = (uint)SQLite3.ColumnInt64( (stmt), i);
                        break;        
                    case "long_keyword":
                        result.long_keyword = SQLite3.ColumnInt64( (stmt), i);
                        break;        
                    case "uint_keyword":
                        result.uint_keyword = (uint)SQLite3.ColumnInt64( (stmt), i);
                        break;        
                    case "bytearray_keyword":
                        result.bytearray_keyword = SQLite3.ColumnByteArray( (stmt), i);
                        break;        
                    case "bytearray":
                        result.bytearray = SQLite3.ColumnByteArray( (stmt), i);
                        break;        
                    case "bytearray_system":
                        result.bytearray_system = SQLite3.ColumnByteArray( (stmt), i);
                        break;        
                    case "Guid":
                        result.Guid = new System.Guid(SQLite3.ColumnString( (stmt), i));
                        break;        
                    case "Guid_system":
                        result.Guid_system = new System.Guid(SQLite3.ColumnString( (stmt), i));
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
            result.StockId = (int)SQLite3.ColumnInt( (stmt), 1);
            result.Time = new System.DateTime(SQLite3.ColumnInt64( (stmt), 2));
            result.Price = (decimal)SQLite3.ColumnDouble( (stmt), 3);
            result.Boolean = JsonSerializer.Deserialize<bool>(SQLite3.ColumnString( (stmt), 4));
            result.Byte = (byte)SQLite3.ColumnInt( (stmt), 5);
            result.SByte = (sbyte)SQLite3.ColumnInt( (stmt), 6);
            result.Int16 = (short)SQLite3.ColumnInt( (stmt), 7);
            result.Int32 = (int)SQLite3.ColumnInt( (stmt), 8);
            result.UInt16 = (ushort)SQLite3.ColumnInt( (stmt), 9);
            result.Bool_System = JsonSerializer.Deserialize<bool>(SQLite3.ColumnString( (stmt), 10));
            result.SByte_System = (sbyte)SQLite3.ColumnInt( (stmt), 11);
            result.Int16_System = (short)SQLite3.ColumnInt( (stmt), 12);
            result.Int32_System = (int)SQLite3.ColumnInt( (stmt), 13);
            result.UInt16_System = (ushort)SQLite3.ColumnInt( (stmt), 14);
            result.byte_keyword = (byte)SQLite3.ColumnInt( (stmt), 15);
            result.Byte_keyword = (sbyte)SQLite3.ColumnInt( (stmt), 16);
            result.short_keyword = (short)SQLite3.ColumnInt( (stmt), 17);
            result.int_keyword = (int)SQLite3.ColumnInt( (stmt), 18);
            result.float_keyword = (float)SQLite3.ColumnDouble( (stmt), 19);
            result.double_keyword = SQLite3.ColumnDouble( (stmt), 20);
            result.decimal_keyword = (decimal)SQLite3.ColumnDouble( (stmt), 21);
            result.Decimal = (decimal)SQLite3.ColumnDouble( (stmt), 22);
            result.string_keyword = SQLite3.ColumnString( (stmt), 23);
            result.String = SQLite3.ColumnString( (stmt), 24);
            result.StringBuilder = new System.Text.StringBuilder(SQLite3.ColumnString( (stmt), 25));
            result.StringBuilder_System = new System.Text.StringBuilder(SQLite3.ColumnString( (stmt), 26));
            result.Uri = new System.Uri(SQLite3.ColumnString( (stmt), 27));
            result.Uri_System = new System.Uri(SQLite3.ColumnString( (stmt), 28));
            result.UriBuilder = new System.UriBuilder(SQLite3.ColumnString( (stmt), 29));
            result.UriBuilder_System = new System.UriBuilder(SQLite3.ColumnString( (stmt), 30));
            result.UInt32 = (uint)SQLite3.ColumnInt64( (stmt), 31);
            result.Int64 = SQLite3.ColumnInt64( (stmt), 32);
            result.TimeSpan = new System.TimeSpan(SQLite3.ColumnInt64( (stmt), 33));
            result.DateTime = new System.DateTime(SQLite3.ColumnInt64( (stmt), 34));
            result.DateTimeOffset = new System.DateTimeOffset(SQLite3.ColumnInt64( (stmt), 35), TimeSpan.Zero);
            result.TimeSpan_system = new System.TimeSpan(SQLite3.ColumnInt64( (stmt), 36));
            result.DateTime_system = new System.DateTime(SQLite3.ColumnInt64( (stmt), 37));
            result.DateTimeOffset_system = new System.DateTimeOffset(SQLite3.ColumnInt64( (stmt), 38), TimeSpan.Zero);
            result.Int64_system = SQLite3.ColumnInt64( (stmt), 39);
            result.UInt32_system = (uint)SQLite3.ColumnInt64( (stmt), 40);
            result.long_keyword = SQLite3.ColumnInt64( (stmt), 41);
            result.uint_keyword = (uint)SQLite3.ColumnInt64( (stmt), 42);
            result.bytearray_keyword = SQLite3.ColumnByteArray( (stmt), 43);
            result.bytearray = SQLite3.ColumnByteArray( (stmt), 44);
            result.bytearray_system = SQLite3.ColumnByteArray( (stmt), 45);
            result.Guid = new System.Guid(SQLite3.ColumnString( (stmt), 46));
            result.Guid_system = new System.Guid(SQLite3.ColumnString( (stmt), 47));
            
            return result;
        }


    }
}