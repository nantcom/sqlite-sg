using System;
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
        /// CreateFlags as set in source 0
        /// </summary>
        public virtual CreateFlags Flags => (CreateFlags)0;

        /// <summary>
        /// Create New Instance of ValuationTableMapping
        /// </summary>
        public ValuationTableMapping()
        {
            this.Columns = new IColumnMapping[4];

            
            

            this.Columns[0] = new StaticColumnMapping<Valuation,int>()
            {
                ColumnName = "Id",
                Setter = (target, value) => target.Id = value,
                Getter = (source) => source.Id
            };

            

            

            this.Columns[1] = new StaticColumnMapping<Valuation,int>()
            {
                ColumnName = "StockId",
                Setter = (target, value) => target.StockId = value,
                Getter = (source) => source.StockId
            };

            

            

            this.Columns[2] = new StaticColumnMapping<Valuation,System.DateTime>()
            {
                ColumnName = "Time",
                Setter = (target, value) => target.Time = value,
                Getter = (source) => source.Time
            };

            

            

            this.Columns[3] = new StaticColumnMapping<Valuation,decimal>()
            {
                ColumnName = "Price",
                Setter = (target, value) => target.Price = value,
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


    }
}