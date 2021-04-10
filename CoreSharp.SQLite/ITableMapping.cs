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
using System.Collections.Generic;

using Sqlite3Statement = System.IntPtr;

namespace NC.SQLite
{
	public interface ITableMapping<T>
	{
		Type SourceType { get; }

		/// <summary>
		/// Gets the name of the mapped table
		/// </summary>
		string TableName { get; }

		Dictionary<string, IColumnMapping> Columns { get; }

		int DropTable(SQLiteConnection connection);

		/// <summary>
		/// Create 
		/// </summary>
		/// <param name="connection"></param>
		void CreateTable(SQLiteConnection connection);

		/// <summary>
		/// Migrate existing database table to contain new fields added by current mapping
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="existingColumns"></param>
		void MigrateTable(SQLiteConnection connection, List<string> existingColumns);

		/// <summary>
		/// Create indexes, as specified by Attributes in SourceType, on the database
		/// </summary>
		/// <param name="connection"></param>
		void CreateIndex(SQLiteConnection connection);

		/// <summary>
		/// Insert object or replace
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="input"></param>
		/// <param name="replace"></param>
		int Insert(SQLiteConnection connection, T input, bool replace);

		/// <summary>
		/// Updates the object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		int Update(SQLiteConnection connection, T input);

		/// <summary>
		/// Delete the specified object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		int Delete(SQLiteConnection connection, T input);

		/// <summary>
		/// Delete an object from database using specified primary key
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		int DeleteByPrimaryKey(SQLiteConnection connection, object pk);

		/// <summary>
		/// Delete all items of this type from the database
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		int DeleteAll(SQLiteConnection connection);

		/// <summary>
		/// Read Statement Result
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		T ReadStatementResult(Sqlite3Statement stmt);

		/// <summary>
		/// Gets ampped column name from given property name
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		string GetMappedColumnName(string propertyName);
	}

}
