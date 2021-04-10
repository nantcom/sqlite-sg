// This code file contains code from 
// https://github.com/praeclarum/sqlite-net
// with minor or no modifications
//
// Copyright (c) 2009-2019 Krueger Systems, Inc.
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
using System.Collections.Generic;
using System.Text;

namespace NC.SQLite
{


	[Flags]
	public enum CreateFlags
	{
		/// <summary>
		/// Use the default creation options
		/// </summary>
		None = 0x000,
		/// <summary>
		/// Create a primary key index for a property called 'Id' (case-insensitive).
		/// This avoids the need for the [PrimaryKey] attribute.
		/// </summary>
		ImplicitPK = 0x001,
		/// <summary>
		/// Create indices for properties ending in 'Id' (case-insensitive).
		/// </summary>
		ImplicitIndex = 0x002,
		/// <summary>
		/// Create a primary key for a property called 'Id' and
		/// create an indices for properties ending in 'Id' (case-insensitive).
		/// </summary>
		AllImplicit = 0x003,
		/// <summary>
		/// Force the primary key property to be auto incrementing.
		/// This avoids the need for the [AutoIncrement] attribute.
		/// The primary key property on the class should have type int or long.
		/// </summary>
		AutoIncPK = 0x004,
		/// <summary>
		/// Create virtual table using FTS3
		/// </summary>
		FullTextSearch3 = 0x100,
		/// <summary>
		/// Create virtual table using FTS4
		/// </summary>
		FullTextSearch4 = 0x200
	}


	[AttributeUsage(AttributeTargets.Class)]
	public class TableAttribute : Attribute
	{
		public string Name { get; }

		/// <summary>
		/// Specify additional create flags for the table
		/// </summary>
		public CreateFlags CreateFlags { get; }

		public TableAttribute(string name, CreateFlags flags = CreateFlags.None)
		{
			this.Name = name;
			this.CreateFlags = flags;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class ColumnAttribute : Attribute
	{
		public string Name { get; }

		public ColumnAttribute(string name)
		{
			this.Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class PrimaryKeyAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class AutoIncrementAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class IndexedAttribute : Attribute
	{
		public string Name { get; }
		public int Order { get; }
		public bool Unique { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="order"></param>
		/// <param name="unique"></param>
		public IndexedAttribute(string name, int order, bool unique)
		{
			this.Name = name;
            this.Order = order;
            this.Unique = unique;
        }
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class IgnoreAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class UniqueAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class MaxLengthAttribute : Attribute
	{
		public int Value { get; }

		public MaxLengthAttribute(int length)
		{
			this.Value = length;
		}
	}

	public sealed class PreserveAttribute : System.Attribute
	{
		public bool AllMembers;
        public bool Conditional;

        public PreserveAttribute(bool allMembers, bool conditional)
        {
            this.AllMembers = allMembers;
            this.Conditional = conditional;
        }
	}

	/// <summary>
	/// Select the collating sequence to use on a column.
	/// "BINARY", "NOCASE", and "RTRIM" are supported.
	/// "BINARY" is the default.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class CollationAttribute : Attribute
	{
		public CollationType Value { get; }

		public CollationAttribute(CollationType collation)
		{
			this.Value = collation;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class NotNullAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Enum)]
	public class StoreAsTextAttribute : Attribute
	{
    }

    public enum CollationType
    {
        Binary,
        Nocase,
		Rtrim,
	}

	/// <summary>
	/// Result of table creation
	/// </summary>
	public enum CreateTableResult
	{
		Invalid,
		Created,
		Migrated,
	}

}
