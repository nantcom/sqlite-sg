﻿// This code file contains code from 
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

namespace CoreSharp.SQLite
{
	/// <summary>
	/// The class which handle actual SQLite3 interop
	/// </summary>
	public static class SQLite3
	{
		public enum Result : int
		{
			OK = 0,
			Error = 1,
			Internal = 2,
			Perm = 3,
			Abort = 4,
			Busy = 5,
			Locked = 6,
			NoMem = 7,
			ReadOnly = 8,
			Interrupt = 9,
			IOError = 10,
			Corrupt = 11,
			NotFound = 12,
			Full = 13,
			CannotOpen = 14,
			LockErr = 15,
			Empty = 16,
			SchemaChngd = 17,
			TooBig = 18,
			Constraint = 19,
			Mismatch = 20,
			Misuse = 21,
			NotImplementedLFS = 22,
			AccessDenied = 23,
			Format = 24,
			Range = 25,
			NonDBFile = 26,
			Notice = 27,
			Warning = 28,
			Row = 100,
			Done = 101
		}

		public enum ExtendedResult : int
		{
			IOErrorRead = (Result.IOError | (1 << 8)),
			IOErrorShortRead = (Result.IOError | (2 << 8)),
			IOErrorWrite = (Result.IOError | (3 << 8)),
			IOErrorFsync = (Result.IOError | (4 << 8)),
			IOErrorDirFSync = (Result.IOError | (5 << 8)),
			IOErrorTruncate = (Result.IOError | (6 << 8)),
			IOErrorFStat = (Result.IOError | (7 << 8)),
			IOErrorUnlock = (Result.IOError | (8 << 8)),
			IOErrorRdlock = (Result.IOError | (9 << 8)),
			IOErrorDelete = (Result.IOError | (10 << 8)),
			IOErrorBlocked = (Result.IOError | (11 << 8)),
			IOErrorNoMem = (Result.IOError | (12 << 8)),
			IOErrorAccess = (Result.IOError | (13 << 8)),
			IOErrorCheckReservedLock = (Result.IOError | (14 << 8)),
			IOErrorLock = (Result.IOError | (15 << 8)),
			IOErrorClose = (Result.IOError | (16 << 8)),
			IOErrorDirClose = (Result.IOError | (17 << 8)),
			IOErrorSHMOpen = (Result.IOError | (18 << 8)),
			IOErrorSHMSize = (Result.IOError | (19 << 8)),
			IOErrorSHMLock = (Result.IOError | (20 << 8)),
			IOErrorSHMMap = (Result.IOError | (21 << 8)),
			IOErrorSeek = (Result.IOError | (22 << 8)),
			IOErrorDeleteNoEnt = (Result.IOError | (23 << 8)),
			IOErrorMMap = (Result.IOError | (24 << 8)),
			LockedSharedcache = (Result.Locked | (1 << 8)),
			BusyRecovery = (Result.Busy | (1 << 8)),
			CannottOpenNoTempDir = (Result.CannotOpen | (1 << 8)),
			CannotOpenIsDir = (Result.CannotOpen | (2 << 8)),
			CannotOpenFullPath = (Result.CannotOpen | (3 << 8)),
			CorruptVTab = (Result.Corrupt | (1 << 8)),
			ReadonlyRecovery = (Result.ReadOnly | (1 << 8)),
			ReadonlyCannotLock = (Result.ReadOnly | (2 << 8)),
			ReadonlyRollback = (Result.ReadOnly | (3 << 8)),
			AbortRollback = (Result.Abort | (2 << 8)),
			ConstraintCheck = (Result.Constraint | (1 << 8)),
			ConstraintCommitHook = (Result.Constraint | (2 << 8)),
			ConstraintForeignKey = (Result.Constraint | (3 << 8)),
			ConstraintFunction = (Result.Constraint | (4 << 8)),
			ConstraintNotNull = (Result.Constraint | (5 << 8)),
			ConstraintPrimaryKey = (Result.Constraint | (6 << 8)),
			ConstraintTrigger = (Result.Constraint | (7 << 8)),
			ConstraintUnique = (Result.Constraint | (8 << 8)),
			ConstraintVTab = (Result.Constraint | (9 << 8)),
			NoticeRecoverWAL = (Result.Notice | (1 << 8)),
			NoticeRecoverRollback = (Result.Notice | (2 << 8))
		}


		public enum ConfigOption : int
		{
			SingleThread = 1,
			MultiThread = 2,
			Serialized = 3
		}

		const string LibraryPath = "sqlite3";

#if !USE_CSHARP_SQLITE && !USE_WP8_NATIVE_SQLITE && !USE_SQLITEPCL_RAW
		[DllImport(LibraryPath, EntryPoint = "sqlite3_threadsafe", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Threadsafe();

		[DllImport(LibraryPath, EntryPoint = "sqlite3_open", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db, int flags, [MarshalAs(UnmanagedType.LPStr)] string zvfs);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Open(byte[] filename, out IntPtr db, int flags, [MarshalAs(UnmanagedType.LPStr)] string zvfs);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_open16", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Open16([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr db);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_enable_load_extension", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result EnableLoadExtension(IntPtr db, int onoff);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_close", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Close(IntPtr db);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_close_v2", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Close2(IntPtr db);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_initialize", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Initialize();

		[DllImport(LibraryPath, EntryPoint = "sqlite3_shutdown", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Shutdown();

		[DllImport(LibraryPath, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Config(ConfigOption option);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_win32_set_directory", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int SetDirectory(uint directoryType, string directoryPath);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_busy_timeout", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result BusyTimeout(IntPtr db, int milliseconds);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_changes", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Changes(IntPtr db);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_prepare_v2", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Prepare2(IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string sql, int numBytes, out IntPtr stmt, IntPtr pzTail);

#if NETFX_CORE
		[DllImport (LibraryPath, EntryPoint = "sqlite3_prepare_v2", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Prepare2 (IntPtr db, byte[] queryBytes, int numBytes, out IntPtr stmt, IntPtr pzTail);
#endif

		public static IntPtr Prepare2(IntPtr db, string query)
		{
			IntPtr stmt;
#if NETFX_CORE
            byte[] queryBytes = System.Text.UTF8Encoding.UTF8.GetBytes (query);
            var r = Prepare2 (db, queryBytes, queryBytes.Length, out stmt, IntPtr.Zero);
#else
			var r = Prepare2(db, query, System.Text.UTF8Encoding.UTF8.GetByteCount(query), out stmt, IntPtr.Zero);
#endif
			if (r != Result.OK)
			{
				throw new SQLiteException(r, GetErrmsg(db));
			}
			return stmt;
		}

		[DllImport(LibraryPath, EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Step(IntPtr stmt);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_reset", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Reset(IntPtr stmt);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_finalize", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Finalize(IntPtr stmt);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_last_insert_rowid", CallingConvention = CallingConvention.Cdecl)]
		public static extern long LastInsertRowid(IntPtr db);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_errmsg16", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Errmsg(IntPtr db);

		public static string GetErrmsg(IntPtr db)
		{
			return Marshal.PtrToStringUni(Errmsg(db));
		}

		[DllImport(LibraryPath, EntryPoint = "sqlite3_bind_parameter_index", CallingConvention = CallingConvention.Cdecl)]
		public static extern int BindParameterIndex(IntPtr stmt, [MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_bind_null", CallingConvention = CallingConvention.Cdecl)]
		public static extern int BindNull(IntPtr stmt, int index);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
		public static extern int BindInt(IntPtr stmt, int index, int val);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
		public static extern int BindInt64(IntPtr stmt, int index, long val);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_bind_double", CallingConvention = CallingConvention.Cdecl)]
		public static extern int BindDouble(IntPtr stmt, int index, double val);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_bind_text16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int BindText(IntPtr stmt, int index, [MarshalAs(UnmanagedType.LPWStr)] string val, int n, IntPtr free);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_bind_blob", CallingConvention = CallingConvention.Cdecl)]
		public static extern int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_count", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ColumnCount(IntPtr stmt);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_name", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ColumnName(IntPtr stmt, int index);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_name16", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr ColumnName16Internal(IntPtr stmt, int index);
		public static string ColumnName16(IntPtr stmt, int index)
		{
			return Marshal.PtrToStringUni(ColumnName16Internal(stmt, index));
		}

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_type", CallingConvention = CallingConvention.Cdecl)]
		public static extern ColType ColumnType(IntPtr stmt, int index);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_int", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ColumnInt(IntPtr stmt, int index);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_int64", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ColumnInt64(IntPtr stmt, int index);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_double", CallingConvention = CallingConvention.Cdecl)]
		public static extern double ColumnDouble(IntPtr stmt, int index);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_text", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ColumnText(IntPtr stmt, int index);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_text16", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ColumnText16(IntPtr stmt, int index);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_blob", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ColumnBlob(IntPtr stmt, int index);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_column_bytes", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ColumnBytes(IntPtr stmt, int index);

		public static string ColumnString(IntPtr stmt, int index)
		{
			return Marshal.PtrToStringUni(SQLite3.ColumnText16(stmt, index));
		}

		public static byte[] ColumnByteArray(IntPtr stmt, int index)
		{
			int length = ColumnBytes(stmt, index);
			var result = new byte[length];
			if (length > 0)
				Marshal.Copy(ColumnBlob(stmt, index), result, 0, length);
			return result;
		}

		[DllImport(LibraryPath, EntryPoint = "sqlite3_errcode", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result GetResult(Sqlite3DatabaseHandle db);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_extended_errcode", CallingConvention = CallingConvention.Cdecl)]
		public static extern ExtendedResult ExtendedErrCode(IntPtr db);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_libversion_number", CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibVersionNumber();

		[DllImport(LibraryPath, EntryPoint = "sqlite3_backup_init", CallingConvention = CallingConvention.Cdecl)]
		public static extern Sqlite3BackupHandle BackupInit(Sqlite3DatabaseHandle destDb, [MarshalAs(UnmanagedType.LPStr)] string destName, Sqlite3DatabaseHandle sourceDb, [MarshalAs(UnmanagedType.LPStr)] string sourceName);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_backup_step", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result BackupStep(Sqlite3BackupHandle backup, int numPages);

		[DllImport(LibraryPath, EntryPoint = "sqlite3_backup_finish", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result BackupFinish(Sqlite3BackupHandle backup);
#else
		public static Result Open (string filename, out Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_open (filename, out db);
		}

		public static Result Open (string filename, out Sqlite3DatabaseHandle db, int flags, string vfsName)
		{
#if USE_WP8_NATIVE_SQLITE
			return (Result)Sqlite3.sqlite3_open_v2(filename, out db, flags, vfsName ?? "");
#else
			return (Result)Sqlite3.sqlite3_open_v2 (filename, out db, flags, vfsName);
#endif
		}

		public static Result Close (Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_close (db);
		}

		public static Result Close2 (Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_close_v2 (db);
		}

		public static Result BusyTimeout (Sqlite3DatabaseHandle db, int milliseconds)
		{
			return (Result)Sqlite3.sqlite3_busy_timeout (db, milliseconds);
		}

		public static int Changes (Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_changes (db);
		}

		public static Sqlite3Statement Prepare2 (Sqlite3DatabaseHandle db, string query)
		{
			Sqlite3Statement stmt = default (Sqlite3Statement);
#if USE_WP8_NATIVE_SQLITE || USE_SQLITEPCL_RAW
			var r = Sqlite3.sqlite3_prepare_v2 (db, query, out stmt);
#else
			stmt = new Sqlite3Statement();
			var r = Sqlite3.sqlite3_prepare_v2(db, query, -1, ref stmt, 0);
#endif
			if (r != 0) {
				throw SQLiteException.New ((Result)r, GetErrmsg (db));
			}
			return stmt;
		}

		public static Result Step (Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_step (stmt);
		}

		public static Result Reset (Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_reset (stmt);
		}

		public static Result Finalize (Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_finalize (stmt);
		}

		public static long LastInsertRowid (Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_last_insert_rowid (db);
		}

		public static string GetErrmsg (Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_errmsg (db).utf8_to_string ();
		}

		public static int BindParameterIndex (Sqlite3Statement stmt, string name)
		{
			return Sqlite3.sqlite3_bind_parameter_index (stmt, name);
		}

		public static int BindNull (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_bind_null (stmt, index);
		}

		public static int BindInt (Sqlite3Statement stmt, int index, int val)
		{
			return Sqlite3.sqlite3_bind_int (stmt, index, val);
		}

		public static int BindInt64 (Sqlite3Statement stmt, int index, long val)
		{
			return Sqlite3.sqlite3_bind_int64 (stmt, index, val);
		}

		public static int BindDouble (Sqlite3Statement stmt, int index, double val)
		{
			return Sqlite3.sqlite3_bind_double (stmt, index, val);
		}

		public static int BindText (Sqlite3Statement stmt, int index, string val, int n, IntPtr free)
		{
#if USE_WP8_NATIVE_SQLITE
			return Sqlite3.sqlite3_bind_text(stmt, index, val, n);
#elif USE_SQLITEPCL_RAW
			return Sqlite3.sqlite3_bind_text (stmt, index, val);
#else
			return Sqlite3.sqlite3_bind_text(stmt, index, val, n, null);
#endif
		}

		public static int BindBlob (Sqlite3Statement stmt, int index, byte[] val, int n, IntPtr free)
		{
#if USE_WP8_NATIVE_SQLITE
			return Sqlite3.sqlite3_bind_blob(stmt, index, val, n);
#elif USE_SQLITEPCL_RAW
			return Sqlite3.sqlite3_bind_blob (stmt, index, val);
#else
			return Sqlite3.sqlite3_bind_blob(stmt, index, val, n, null);
#endif
		}

		public static int ColumnCount (Sqlite3Statement stmt)
		{
			return Sqlite3.sqlite3_column_count (stmt);
		}

		public static string ColumnName (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_name (stmt, index).utf8_to_string ();
		}

		public static string ColumnName16 (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_name (stmt, index).utf8_to_string ();
		}

		public static ColType ColumnType (Sqlite3Statement stmt, int index)
		{
			return (ColType)Sqlite3.sqlite3_column_type (stmt, index);
		}

		public static int ColumnInt (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_int (stmt, index);
		}

		public static long ColumnInt64 (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_int64 (stmt, index);
		}

		public static double ColumnDouble (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_double (stmt, index);
		}

		public static string ColumnText (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text (stmt, index).utf8_to_string ();
		}

		public static string ColumnText16 (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text (stmt, index).utf8_to_string ();
		}

		public static byte[] ColumnBlob (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_blob (stmt, index).ToArray ();
		}

		public static int ColumnBytes (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_bytes (stmt, index);
		}

		public static string ColumnString (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text (stmt, index).utf8_to_string ();
		}

		public static byte[] ColumnByteArray (Sqlite3Statement stmt, int index)
		{
			int length = ColumnBytes (stmt, index);
			if (length > 0) {
				return ColumnBlob (stmt, index);
			}
			return new byte[0];
		}

		public static Result EnableLoadExtension (Sqlite3DatabaseHandle db, int onoff)
		{
			return (Result)Sqlite3.sqlite3_enable_load_extension (db, onoff);
		}

		public static int LibVersionNumber ()
		{
			return Sqlite3.sqlite3_libversion_number ();
		}

		public static Result GetResult (Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_errcode (db);
		}

		public static ExtendedResult ExtendedErrCode (Sqlite3DatabaseHandle db)
		{
			return (ExtendedResult)Sqlite3.sqlite3_extended_errcode (db);
		}

		public static Sqlite3BackupHandle BackupInit (Sqlite3DatabaseHandle destDb, string destName, Sqlite3DatabaseHandle sourceDb, string sourceName)
		{
			return Sqlite3.sqlite3_backup_init (destDb, destName, sourceDb, sourceName);
		}

		public static Result BackupStep (Sqlite3BackupHandle backup, int numPages)
		{
			return (Result)Sqlite3.sqlite3_backup_step (backup, numPages);
		}

		public static Result BackupFinish (Sqlite3BackupHandle backup)
		{
			return (Result)Sqlite3.sqlite3_backup_finish (backup);
		}
#endif

		public enum ColType : int
		{
			Integer = 1,
			Float = 2,
			Text = 3,
			Blob = 4,
			Null = 5
		}
	}
}
