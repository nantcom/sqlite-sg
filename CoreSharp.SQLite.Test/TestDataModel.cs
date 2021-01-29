using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSharp.SQLite.Test
{
	[Table("Stock", flags: CreateFlags.ImplicitIndex | CreateFlags.FullTextSearch3)]
	public class Stock
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
        public string Symbol { get; set; }
		
		public Valuation[] Valuations { get; set; }
    }

	[Table("Valuation")]
	public class Valuation
	{
		[PrimaryKey]
		public Guid Id { get; set; }
		[Indexed]
		public int StockId { get; set; }
		public DateTime Time { get; set; }
		public decimal Price { get; set; }
	}
}
