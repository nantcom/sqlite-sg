using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreSharp.SQLite.Generator
{
    public class IndexModel
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public bool Unique { get; set; }
    }

    /// <summary>
    /// Class which stores information about Property and its mapping to database column
    /// </summary>
    public class ColumnMappingModel
    {
        public const string ImplicitPkName = "Id";
        public const string ImplicitIndexSuffix = "Id";


        private static Dictionary<string, string> _ClrTypeMapping;
        private static Dictionary<string, string> _Sqlite3TypeToFunction;

        static ColumnMappingModel()
        {
            ColumnMappingModel.BuildClrTypeMapping();
            ColumnMappingModel.BuildSQLite3TypeToFunctionMapping();
        }

        private static void BuildClrTypeMapping()
        {
            _ClrTypeMapping = new();

            var type = "integer";
            _ClrTypeMapping["Boolean"] = type;
            _ClrTypeMapping["Byte"] = type;
            _ClrTypeMapping["SByte"] = type;
            _ClrTypeMapping["Int16"] = type;
            _ClrTypeMapping["Int32"] = type;
            _ClrTypeMapping["UInt16"] = type;
            _ClrTypeMapping["System.Boolean"] = type;
            _ClrTypeMapping["System.Byte"] = type;
            _ClrTypeMapping["System.SByte"] = type;
            _ClrTypeMapping["System.Int16"] = type;
            _ClrTypeMapping["System.Int32"] = type;
            _ClrTypeMapping["System.UInt16"] = type;

            _ClrTypeMapping["byte"] = type;
            _ClrTypeMapping["sbyte"] = type;
            _ClrTypeMapping["short"] = type;
            _ClrTypeMapping["int"] = type;
            _ClrTypeMapping["ushort"] = type;

            type = "float";
            _ClrTypeMapping["float"] = type;
            _ClrTypeMapping["double"] = type;
            _ClrTypeMapping["Decimal"] = type;
            _ClrTypeMapping["System.Decimal"] = type;

            type = "varchar";
            _ClrTypeMapping["string"] = type;
            _ClrTypeMapping["String"] = type;
            _ClrTypeMapping["StringBuilder"] = type;
            _ClrTypeMapping["System.Text.StringBuilder"] = type;
            _ClrTypeMapping["Uri"] = type;
            _ClrTypeMapping["System.Uri"] = type;
            _ClrTypeMapping["UriBuilder"] = type;
            _ClrTypeMapping["System.UriBuilder"] = type;

            type = "bigint";
            _ClrTypeMapping["UInt32"] = type;
            _ClrTypeMapping["UInt64"] = type;
            _ClrTypeMapping["Int64"] = type;

            _ClrTypeMapping["TimeSpan"] = type;
            _ClrTypeMapping["DateTime"] = type;
            _ClrTypeMapping["DateTimeOffset"] = type;

            _ClrTypeMapping["System.TimeSpan"] = type;
            _ClrTypeMapping["System.DateTime"] = type;
            _ClrTypeMapping["System.DateTimeOffset"] = type;
            _ClrTypeMapping["System.Int64"] = type;
            _ClrTypeMapping["System.UInt32"] = type;
            _ClrTypeMapping["System.UInt64"] = type;

            _ClrTypeMapping["long"] = type;
            _ClrTypeMapping["uint"] = type;
            _ClrTypeMapping["ulong"] = type;

            type = "blob";
            _ClrTypeMapping["byte[]"] = type;

            type = "varchar(36)";
            _ClrTypeMapping["Guid"] = type;
            _ClrTypeMapping["System.Guid"] = type;

        }

        private static void BuildSQLite3TypeToFunctionMapping()
        {
            _Sqlite3TypeToFunction = new();
            _Sqlite3TypeToFunction["integer"] = "Int";
            _Sqlite3TypeToFunction["float"] = "Double";
            _Sqlite3TypeToFunction["bigint"] = "Int64";
            _Sqlite3TypeToFunction["blob"] = "Blob";
            _Sqlite3TypeToFunction["varchar"] = "Text";
            _Sqlite3TypeToFunction["varchar(36)"] = "Text";
        }

        public string ColumnName { get; private set; }

        public string ColumnType { get; private set; }

        public string ColumnCollation { get; private set; }

        public string PropertyName { get; private set; }

        public string PropertyType { get; private set; }

        /// <summary>
        /// Whether this Property will be mapped
        /// Property with public get/set accessor and publicly accessible
        /// is mapped. Ignore Attribute will also cause the property
        /// to be excluded from mapping
        /// </summary>
        public bool IsMapped { get; private set; }

        public bool IsIndexed { get; private set; }

        public bool IsPrimaryKey { get; private set; }

        public bool IsUnique { get; set; }

        public bool IsStoreAsText { get; private set; }

        public bool IsNotNull { get; private set; }

        /// <summary>
        /// Whether this property is a reference type, which may be null
        /// </summary>
        public bool IsReferenceType { get; private set; }

        public int MaxStringLength { get; private set; }

        private CreateFlags CreateFlags { get; set; }

        public bool IsAutoIncrement { get; set; }

        public bool IsAutoGuid { get; set; }

        public bool IsEnum { get; set; }

        /// <summary>
        /// This property type is not recognized as primitive types and will be kept as JSON string in database
        /// </summary>
        public bool IsStoringAsJSON { get; set; }

        public List<IndexModel> IndexModels { get; set; } = new();

        public string SQLColumnDeclaration => this.GetSQLColumnDeclaration().EscapeQuote();

        public string SQLite3BindFunctionCall => this.GetSQLite3BindFunctionCall().EscapeQuote();

        public string SQLite3ReadFunctionCall => this.GetSQLite3ReadFunctionCall().EscapeQuote();

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="property"></param>
        /// <param name="createFlags"></param>
        public ColumnMappingModel(IPropertySymbol property, CreateFlags createFlags )
        {
            this.CreateFlags = createFlags;

            // this property is not mapped due to access restrictions
            if (property.IsAbstract ||
                property.IsReadOnly ||
                property.SetMethod?.DeclaredAccessibility == Accessibility.Private)

            {
                return;
            }

            this.IsMapped = true;
            this.PropertyType = property.Type.ToString();
            this.PropertyName = property.Name;
            this.ColumnName = property.Name;

            // Create dictioanry of parsers
            var parsers = this.GetType().GetMethods()
                            .Where(m => m.Name.StartsWith("Parse"))
                            .ToDictionary(
                                m => m.Name.Substring(5),
                                m => (Parser)m.CreateDelegate(typeof(Parser)));

            foreach (var attr in property.GetAttributes())
            {
                Parser p;
                if (parsers.TryGetValue( attr.AttributeClass.Name, out p ))
                {
                    p(attr);
                }
            }

            this.IdentifyColumnType(property);

            // Implicit Primary Key logic
            if ((createFlags & CreateFlags.ImplicitPK) == CreateFlags.ImplicitPK)
            {
                this.IsPrimaryKey = string.Compare(this.PropertyName, ColumnMappingModel.ImplicitPkName, StringComparison.OrdinalIgnoreCase) == 0;
            }

            // Auto Increment Primary Key Logic
            this.IsAutoIncrement = this.IsAutoIncrement || (this.IsPrimaryKey && ((createFlags & CreateFlags.AutoIncPK) == CreateFlags.AutoIncPK));
            this.IsAutoGuid = this.IsAutoIncrement && this.PropertyType == "Guid";
            this.IsAutoIncrement = this.IsAutoIncrement && !this.IsAutoGuid;

            // Index Creation Logic
            // index are parsed already in "GetAttributes" loop
            if (
                this.IndexModels.Count == 0 //!Indices.Any()
                && !this.IsPrimaryKey // && !IsPK
                && ((createFlags & CreateFlags.ImplicitIndex) == CreateFlags.ImplicitIndex)
                && this.PropertyName.EndsWith(ColumnMappingModel.ImplicitIndexSuffix, StringComparison.OrdinalIgnoreCase)
            )
            {
                // add an implicit index
                this.IndexModels.Add(new IndexModel());
            }
        }

        #region Attribute Parsers

        private delegate void Parser(AttributeData attr);

        private void ParseColumnAttribute(AttributeData attr)
        {
            this.ColumnName = (string)attr.GetAttributeConstructorValueByParameterName("name");
        }

        private void ParsePrimaryKeyAttribute(AttributeData attr)
        {
            this.IsPrimaryKey = true;
        }

        private void ParseAutoIncrementAttribute(AttributeData attr)
        {
            this.IsAutoIncrement = true;
        }

        private void ParseIndexedAttribute(AttributeData attr)
        {
            var index = new IndexModel();
            index.Name = (string)attr.GetAttributeConstructorValueByParameterName("name");
            index.Order = (int)attr.GetAttributeConstructorValueByParameterName("order");
            index.Unique = (bool)attr.GetAttributeConstructorValueByParameterName("unique");

            this.IndexModels.Add(index);
        }

        private void ParseIgnoreAttribute(AttributeData attr)
        {
            this.IsMapped = false;
        }

        private void ParseUniqueAttribute(AttributeData attr)
        {
            this.IsUnique = true;
        }

        private void ParseMaxLengthAttribute(AttributeData attr)
        {
            this.MaxStringLength = (int)attr.GetAttributeConstructorValueByParameterName("length");
        }

        private void ParseCollationAttribute(AttributeData attr)
        {
            this.ColumnCollation = (string)attr.GetAttributeConstructorValueByParameterName("collation");
        }

        private void ParseNotNullAttribute(AttributeData attr)
        {
            this.IsNotNull = true;
        }

        private void ParseStoreAsTextAttribute(AttributeData attr)
        {
            this.IsStoreAsText = true;
        }


        #endregion

        /// <summary>
        /// Get column type from current property type
        /// </summary>
        private void IdentifyColumnType(IPropertySymbol property)
        {
            // need special care for enum
            if (property.Type.BaseType?.EnumUnderlyingType != null)
            {
                this.IsEnum = true;
                if (this.IsStoreAsText)
                {
                    this.ColumnType = "varchar";
                }
                else
                {
                    this.ColumnType = "integer";
                }

                return;
            }

            string databaseType;
            if (_ClrTypeMapping.TryGetValue( this.PropertyType, out databaseType ))
            {
                this.ColumnType = databaseType;
                return;
            }

            // Cannot recognize the type, we will store it as JSON
            this.ColumnType = "varchar";
            this.IsStoringAsJSON = true;
        }

        /// <summary>
        /// Gets SQL Column Definition for this column
        /// </summary>
        /// <returns></returns>
        public string GetSQLColumnDeclaration()
        {
            // Original Code From SQLite-net Orm.SqlDecl Function

            string decl = $"\"{this.ColumnName}\" {this.ColumnType} ";

            if (this.IsPrimaryKey)
            {
                decl += "primary key ";
            }
            if (this.IsAutoIncrement)
            {
                decl += "autoincrement ";
            }
            if (this.IsNotNull)
            {
                decl += "not null ";
            }
            if (!string.IsNullOrEmpty(this.ColumnCollation))
            {
                decl += $"collate {this.ColumnCollation}";
            }

            return decl;
        }

        /// <summary>
        /// Gets the SQLite3 Function Name to bind value of this property
        /// </summary>
        /// <returns></returns>
        public string GetSQLite3BindFunctionCall( 
            string statementParameterText = "(stmt)",
            string indexParameterText = "(index)",
            string valueParameterText = "(value)")
        {
            // ref: https://github.com/praeclarum/sqlite-net/blob/ff6507e2accd79ab60aa84a1039215884e4118fa/src/SQLite.cs#L3084

            var functionName = _Sqlite3TypeToFunction[this.ColumnType];
            var valueExpression = valueParameterText;
            var extraParameters = "";;

            Func<string, bool> match = t => this.PropertyType.ToLowerInvariant().EndsWith(t);

            var toInt32 = new string[] { "byte", "sbyte", "uint16", "int16", "ushort", "short" };
            var toInt64 = new string[] { "uint32", "uint" };
            var getTicks = new string[] { "timespan", "datetime" };
            var getUtcTicks = new string[] { "datetimeoffset" };
            var toStrings = new string[] { "guid", "uri", "uribuilder", "stringbuilder" };

            if (toInt32.Any(match))
            {
                valueExpression = $"(int){valueParameterText}";
            }

            if (toInt64.Any(match))
            {
                valueExpression = $"(long){valueParameterText}";
            }

            if (getTicks.Any(match))
            {
                valueExpression = $"{valueParameterText}.Ticks";
            }

            if (getUtcTicks.Any(match))
            {
                valueExpression = $"{valueParameterText}.UtcTicks";
            }

            if (toStrings.Any(match))
            {
                valueExpression = $"{valueParameterText}.ToString()";
            }

            if (this.IsStoringAsJSON)
            {
                valueExpression = $"JsonSerializer.Serialize({valueExpression})";
                extraParameters = ", -1, new IntPtr(-1)";
            }

            if (this.ColumnType.StartsWith("varchar"))
            {
                extraParameters = ", -1, new IntPtr(-1)";
            }

            return $"SQLite3.Bind{functionName}( {statementParameterText}, {indexParameterText}, {valueExpression} {extraParameters} )";
        }

        public string GetSQLite3ReadFunctionCall( string statementParameterText = "(stmt)", string indexParameterText = "(index)")
        {
            var functionName = _Sqlite3TypeToFunction[this.ColumnType];
            var initializer = "{0}";

            if (functionName == "Text")
            {
                functionName = "String";
            }

            Func<string, bool> match = t => this.PropertyType.ToLowerInvariant().EndsWith(t);

            var fromTicks = new string[] { "timespan", "datetime" };
            var dateTimeOffset = new string[] { "datetimeoffset" };
            var fromStrings = new string[] { "guid", "uri", "uribuilder", "stringbuilder" };

            if (fromTicks.Any(match))
            {
                initializer = "new " + this.PropertyType + "({0})";
            }

            if (dateTimeOffset.Any(match))
            {
                initializer = "new " + this.PropertyType + "({0})";
            }

            if (fromStrings.Any(match))
            {
                initializer = "new " + this.PropertyType + "({0})";
            }

            if (this.IsStoringAsJSON)
            {
                initializer = "JsonSerializer.Deserialize<" + this.PropertyType + ">({0})";
            }


            return string.Format( initializer, 
                    $"SQLite3.Column{functionName}( {statementParameterText}, {indexParameterText})" );
        }
    }

}