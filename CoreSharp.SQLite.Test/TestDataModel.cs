using System;
using System.Collections.Generic;
using System.Text;

namespace NC.SQLite.Test
{
    [Table("Stock", flags: CreateFlags.ImplicitIndex | CreateFlags.FullTextSearch3)]
    public class Stock
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Symbol { get; set; }
    }

    [Table("Valuation")]
    public class Valuation
    {
        [PrimaryKey, AutoIncrement]
        public Guid Id { get; set; }
        [Indexed(null, 0, false)]
        public int StockId { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }

        public Boolean Boolean { get; set; }
        public Byte Byte { get; set; }
        public SByte SByte { get; set; }
        public Int16 Int16 { get; set; }
        public Int32 Int32 { get; set; }
        public UInt16 UInt16 { get; set; }
        public System.Boolean Bool_System { get; set; }
        public System.SByte SByte_System { get; set; }
        public System.Int16 Int16_System { get; set; }
        public System.Int32 Int32_System { get; set; }
        public System.UInt16 UInt16_System { get; set; }
        public byte byte_keyword { get; set; }
        public sbyte Byte_keyword { get; set; }
        public short short_keyword { get; set; }
        public int int_keyword { get; set; }
        public float float_keyword { get; set; }
        public double double_keyword { get; set; }
        public decimal decimal_keyword { get; set; }
        public Decimal Decimal { get; set; }

        public string string_keyword { get; set; }
        public String String { get; set; }
        public StringBuilder StringBuilder { get; set; }
        public System.Text.StringBuilder StringBuilder_System { get; set; }
        public Uri Uri { get; set; }
        public System.Uri Uri_System { get; set; }
        public UriBuilder UriBuilder { get; set; }
        public System.UriBuilder UriBuilder_System { get; set; }


        public UInt32 UInt32 { get; set; }
        [Ignore]
        public UInt64 UInt64 { get; set; }
        public Int64 Int64 { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public DateTime DateTime { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public System.TimeSpan TimeSpan_system        { get; set; }
        public System.DateTime DateTime_system { get; set; }
        public System.DateTimeOffset DateTimeOffset_system { get; set; }
        public System.Int64 Int64_system { get; set; }
        public System.UInt32 UInt32_system { get; set; }
        [Ignore]
        public System.UInt64 UInt64_system { get; set; }
        public long long_keyword { get; set; }
        public uint uint_keyword { get; set; }
        [Ignore]
        public ulong ulong_keyword { get; set; }

        public byte[] bytearray_keyword { get; set; }
        public Byte[] bytearray { get; set; }
        public System.Byte[] bytearray_system { get; set; }

        public Guid Guid { get; set; }
        public System.Guid Guid_system { get; set; }
    }

}
