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
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NC.SQLite.SourceGen
{
    public class TableMappingModel
    {
        // ref: https://github.com/praeclarum/sqlite-net/blob/ff6507e2accd79ab60aa84a1039215884e4118fa/src/SQLite.cs#L488
        private struct IndexedColumn
        {
            public int Order;
            public string ColumnName;
        }

        private struct IndexInfo
        {
            public string IndexName;
            public string TableName;
            public bool Unique;
            public List<IndexedColumn> Columns;
        }

        public string Namespace { get; set; }

        public string MappedClassName { get; set; }

        public string TableName { get; set; }

        public string CreateFlagsAsSpecified
        {
            get
            {
                return ((CreateFlags)this.CreateFlagsInt).ToString();
            }
        }

        public int CreateFlagsInt { get; set; }

        public bool HasAutoIncrementPrimaryKey { get; set; }

        public ColumnMappingModel PrimaryKey { get; set; }

        public List<ColumnMappingModel> ColumnMappingModels { get; set; } = new();

        /// <summary>
        /// Columns to include in insertion
        /// </summary>
        public IEnumerable<ColumnMappingModel> InsertColumns
        {
            get
            {
                return this.ColumnMappingModels.Where(c => c.IsAutoIncrement == false);
            }
        }

        /// <summary>
        /// Columns to include in Replace
        /// </summary>
        public IEnumerable<ColumnMappingModel> ReplaceColumns
        {
            get
            {
                return this.ColumnMappingModels;
            }
        }

        /// <summary>
        /// Columns to include in Update
        /// </summary>
        public IEnumerable<ColumnMappingModel> UpdateColumns
        {
            get
            {
                return this.ColumnMappingModels.Where(c => c.IsPrimaryKey == false);
            }
        }

        public Dictionary<string, string> SQLCommands { get; set; } = new();

        public IEnumerable<string> IndexCommands => this.GetCreateIndexCommands().Select(s => s.EscapeQuote());

        public TableMappingModel(AttributeData attr)
        {
            this.TableName = attr.GetAttributeConstructorValueByParameterName("name") as string;
            this.CreateFlagsInt = (int)attr.GetAttributeConstructorValueByParameterName("flags");
        }

        /// <summary>
        /// Update the model based on new items/changed properties
        /// of ColumnMappingModels
        /// </summary>
        public void Update()
        {
            this.HasAutoIncrementPrimaryKey = this.ColumnMappingModels.Any(c => c.IsAutoIncrement && c.IsPrimaryKey);
            this.PrimaryKey = this.ColumnMappingModels.Where(c => c.IsPrimaryKey).FirstOrDefault();

            this.SQLCommands["Create"] = this.GetCreateTableCommand().EscapeQuote();
            this.SQLCommands["Insert"] = this.GetInsertCommand().EscapeQuote();
            this.SQLCommands["Replace"] = this.GetReplaceCommand().EscapeQuote();
            this.SQLCommands["Update"] = this.GetUpdateCommand().EscapeQuote();
            this.SQLCommands["Delete"] = this.GetDeleteCommand().EscapeQuote();
        }

        /// <summary>
        /// Get SQL Statement for Create Table with all columns
        /// </summary>
        /// <returns></returns>
        public string GetCreateTableCommand()
        {
            // Ref: https://github.com/praeclarum/sqlite-net/blob/master/src/SQLite.cs#L547

            var createFlags = (CreateFlags)this.CreateFlagsInt;

            bool fts3 = (createFlags & CreateFlags.FullTextSearch3) != 0;
            bool fts4 = (createFlags & CreateFlags.FullTextSearch4) != 0;
            bool fts = fts3 || fts4;

            var @virtual = fts ? "virtual" : string.Empty;
            var @using = fts3 ? "using fts3" : fts4 ? "using fts4" : string.Empty;

            // Build query.
            var query = $"create {@virtual} table if not exists \"{this.TableName}\" {@using} (";
            var decls = this.ColumnMappingModels.Select( c => c.GetSQLColumnDeclaration());
            var decl = string.Join(",", decls.ToArray());
            query += decl;
            query += ")";

            /* We do not support this
            if (map.WithoutRowId)
            {
                query += " without rowid";
            }*/

            return query;
        }

        /// <summary>
        /// Generate Create Index Commands
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCreateIndexCommands()
        {
            // ref: https://github.com/praeclarum/sqlite-net/blob/master/src/SQLite.cs#L587

            var indexes = new Dictionary<string, IndexInfo>();
            foreach (var c in this.ColumnMappingModels)
            {
                foreach (var i in c.IndexModels)
                {
                    var iname = i.Name ?? this.TableName + "_" + c.ColumnName;
                    IndexInfo iinfo;
                    if (!indexes.TryGetValue(iname, out iinfo))
                    {
                        iinfo = new IndexInfo
                        {
                            IndexName = iname,
                            TableName = this.TableName,
                            Unique = i.Unique,
                            Columns = new List<IndexedColumn>()
                        };
                        indexes.Add(iname, iinfo);
                    }

                    if (i.Unique != iinfo.Unique)
                        throw new Exception("All the columns in an index must have the same value for their Unique property");

                    iinfo.Columns.Add(new IndexedColumn
                    {
                        Order = i.Order,
                        ColumnName = c.ColumnName
                    });
                }
            }

            foreach (var indexName in indexes.Keys)
            {
                var index = indexes[indexName];
                var columns = index.Columns.OrderBy(i => i.Order).Select(i => i.ColumnName).ToArray();

                //CreateIndex(indexName, index.TableName, columns, index.Unique);
                // ref: https://github.com/praeclarum/sqlite-net/blob/master/src/SQLite.cs#L718

                const string sqlFormat = "create {2} index if not exists \"{3}\" on \"{0}\"(\"{1}\")";
                var sql = String.Format(sqlFormat, index.TableName, string.Join("\", \"", columns), index.Unique ? "unique" : "", indexName);

                yield return sql;
            }
        }

        /// <summary>
        /// Get SQL Statement for Inserting data of this type into database
        /// </summary>
        /// <returns></returns>
        public string GetInsertCommand()
        {
            // ref: https://github.com/praeclarum/sqlite-net/blob/ff6507e2accd79ab60aa84a1039215884e4118fa/src/SQLite.cs#L1730
            // ref: https://github.com/praeclarum/sqlite-net/blob/ff6507e2accd79ab60aa84a1039215884e4118fa/src/SQLite.cs#L1807

            var cols = this.InsertColumns.ToList();
            var extra = "";

            string insertSql;
            if (cols.Count == 0 && this.ColumnMappingModels.Count == 1 && this.ColumnMappingModels[0].IsAutoIncrement)
            {
                insertSql = $"insert into \"{this.TableName}\" default values";
            }
            else
            {
                insertSql = string.Format("insert {3} into \"{0}\"({1}) values ({2})", this.TableName,
                                   string.Join(",", (from c in cols
                                                     select "\"" + c.ColumnName + "\"").ToArray()),
                                   string.Join(",", (from c in cols
                                                     select "?").ToArray()), extra);
            }

            return insertSql;
        }

        /// <summary>
        /// Get SQL Statement for Insert or replace data of this type into database
        /// </summary>
        /// <returns></returns>
        public string GetReplaceCommand()
        {
            // ref: https://github.com/praeclarum/sqlite-net/blob/ff6507e2accd79ab60aa84a1039215884e4118fa/src/SQLite.cs#L1730
            // ref: https://github.com/praeclarum/sqlite-net/blob/ff6507e2accd79ab60aa84a1039215884e4118fa/src/SQLite.cs#L1807

            var cols = this.ReplaceColumns.ToList();
            var extra = "or replace";

            string insertSql;
            insertSql = string.Format("insert {3} into \"{0}\"({1}) values ({2})", this.TableName,
                                    string.Join(",", (from c in cols
                                                      select "\"" + c.ColumnName + "\"").ToArray()),
                                    string.Join(",", (from c in cols
                                                      select "?").ToArray()), extra);

            return insertSql;
        }

        /// <summary>
        /// Gets an update command
        /// </summary>
        /// <returns></returns>
        public string GetUpdateCommand()
        {
            // ref: https://github.com/praeclarum/sqlite-net/blob/ff6507e2accd79ab60aa84a1039215884e4118fa/src/SQLite.cs#L1866
            if (this.PrimaryKey == null)
            {
                // error;
                return "CANNOT UPDATE DUE TO NO PK";
            }

            var cols = from p in this.ColumnMappingModels
                       where p.IsPrimaryKey == false
                       select "\"" + p.ColumnName + "\" = ? ";

            var colList = string.Join(",", cols.ToArray());

            return $"update \"{this.TableName}\" set {colList} where {this.PrimaryKey.ColumnName} = ? ";
        }

        /// <summary>
        /// Gets delete command
        /// </summary>
        /// <returns></returns>
        public string GetDeleteCommand()
        {
            // ref: https://github.com/praeclarum/sqlite-net/blob/ff6507e2accd79ab60aa84a1039215884e4118fa/src/SQLite.cs#L1956
            if (this.PrimaryKey == null)
            {
                // error;
                return "CANNOT DELETE DUE TO NO PK";
            }

            return $"delete from \"{this.TableName}\" where \"{this.PrimaryKey.ColumnName}\" = ?";
        }
    }
}