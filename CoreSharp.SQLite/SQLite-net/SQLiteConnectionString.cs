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

	/// <summary>
	/// Represents a parsed connection string.
	/// </summary>
	public class SQLiteConnectionString
	{
		const string DateTimeSqliteDefaultFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff";

		public string UniqueKey { get; }
		public string DatabasePath { get; }
		public byte[] Key { get; }
		public SQLiteOpenFlags OpenFlags { get; }
		public string VfsName { get; }

#if NETFX_CORE
		static readonly string MetroStyleDataPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;

        public static readonly string[] InMemoryDbPaths = new[]
        {
            ":memory:",
            "file::memory:"
        };

        public static bool IsInMemoryPath(string databasePath)
        {
            return InMemoryDbPaths.Any(i => i.Equals(databasePath, StringComparison.OrdinalIgnoreCase));
        }

#endif

		/// <summary>
		/// Constructs a new SQLiteConnectionString with all the data needed to open an SQLiteConnection.
		/// </summary>
		/// <param name="databasePath">
		/// Specifies the path to the database file.
		/// </param>
		/// <param name="openFlags">
		/// Flags controlling how the connection should be opened.
		/// </param>
		/// <param name="storeDateTimeAsTicks">
		/// Specifies whether to store DateTime properties as ticks (true) or strings (false). You
		/// absolutely do want to store them as Ticks in all new projects. The value of false is
		/// only here for backwards compatibility. There is a *significant* speed advantage, with no
		/// down sides, when setting storeDateTimeAsTicks = true.
		/// If you use DateTimeOffset properties, it will be always stored as ticks regardingless
		/// the storeDateTimeAsTicks parameter.
		/// </param>
		/// <param name="key">
		/// Specifies the encryption key to use on the database. Should be a string or a byte[].
		/// </param>
		/// <param name="preKeyAction">
		/// Executes prior to setting key for SQLCipher databases
		/// </param>
		/// <param name="postKeyAction">
		/// Executes after setting key for SQLCipher databases
		/// </param>
		/// <param name="vfsName">
		/// Specifies the Virtual File System to use on the database.
		/// </param>
		/// <param name="dateTimeStringFormat">
		/// Specifies the format to use when storing DateTime properties as strings.
		/// </param>
		/// <param name="storeTimeSpanAsTicks">
		/// Specifies whether to store TimeSpan properties as ticks (true) or strings (false). You
		/// absolutely do want to store them as Ticks in all new projects. The value of false is
		/// only here for backwards compatibility. There is a *significant* speed advantage, with no
		/// down sides, when setting storeTimeSpanAsTicks = true.
		/// </param>
		public SQLiteConnectionString(string databasePath,
			SQLiteOpenFlags openFlags = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite,
			byte[] key = null,
			string vfsName = null )
		{
			if (key != null && !((key is byte[]) || (key is string)))
				throw new ArgumentException("Encryption keys must be strings or byte arrays", nameof(key));

			this.UniqueKey = string.Format("{0}_{1:X8}", databasePath, (uint)openFlags);
			this.Key = key;
			this.OpenFlags = openFlags;
			this.VfsName = vfsName;

#if NETFX_CORE
			DatabasePath = IsInMemoryPath(databasePath)
                ? databasePath
                : System.IO.Path.Combine(MetroStyleDataPath, databasePath);

#else
			this.DatabasePath = databasePath;
#endif
		}
	}

}
