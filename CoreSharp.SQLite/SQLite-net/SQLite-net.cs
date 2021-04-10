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
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

using Sqlite3DatabaseHandle = System.IntPtr;
using Sqlite3BackupHandle = System.IntPtr;
using Sqlite3Statement = System.IntPtr;

#pragma warning disable 1591 // XML Doc Comments

namespace NC.SQLite
{
	public class SQLiteException : Exception
	{
		public SQLite3.Result Result { get; }

		public SQLiteException(SQLite3.Result r, string message) : base(message)
		{
			Result = r;
		}
	}

	public class NotNullConstraintViolationException : SQLiteException
	{
		public NotNullConstraintViolationException(SQLite3.Result r, string message)
			: base(r, message)
		{

		}

	}

	[Flags]
	public enum SQLiteOpenFlags
	{
		ReadOnly = 1, ReadWrite = 2, Create = 4,
		NoMutex = 0x8000, FullMutex = 0x10000,
		SharedCache = 0x20000, PrivateCache = 0x40000,
		ProtectionComplete = 0x00100000,
		ProtectionCompleteUnlessOpen = 0x00200000,
		ProtectionCompleteUntilFirstUserAuthentication = 0x00300000,
		ProtectionNone = 0x00400000
	}

	public class NotifyTableChangedEventArgs : EventArgs
	{
		public Type TableType { get; }
		public NotifyTableChangedAction Action { get; }

		public NotifyTableChangedEventArgs(Type table, NotifyTableChangedAction action)
		{
			TableType = table;
			Action = action;
		}
	}

	public enum NotifyTableChangedAction
	{
		Insert,
		Update,
		Delete,
	}

}