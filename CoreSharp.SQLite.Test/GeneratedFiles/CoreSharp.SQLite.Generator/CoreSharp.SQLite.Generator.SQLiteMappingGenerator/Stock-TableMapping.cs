using System;
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
        /// CreateFlags as set in source 258
        /// </summary>
        public virtual CreateFlags Flags => (CreateFlags)258;

        /// <summary>
        /// Create New Instance of StockTableMapping
        /// </summary>
        public StockTableMapping()
        {
            this.Columns = new IColumnMapping[2];

            
            

            this.Columns[0] = new StaticColumnMapping<Stock,int>()
            {
                ColumnName = "Id",
                Setter = (target, value) => target.Id = value,
                Getter = (source) => source.Id
            };

            

            

            this.Columns[1] = new StaticColumnMapping<Stock,string>()
            {
                ColumnName = "Symbol",
                Setter = (target, value) => target.Symbol = value,
                Getter = (source) => source.Symbol
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


    }
}