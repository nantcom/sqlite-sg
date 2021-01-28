using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSharp.SQLite
{

	/// <summary>
	/// Flags from SQLite-net
	/// </summary>
	[Flags]
	public enum CreateFlags
	{
		None = 0x000,
		ImplicitPK = 0x001,
		ImplicitIndex = 0x002,
		AllImplicit = 0x003,
		AutoIncPK = 0x004,
		FullTextSearch3 = 0x100,
		FullTextSearch4 = 0x200
	}
}
