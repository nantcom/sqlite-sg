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
    public class PropertyMappingModel
    {
        public const string ImplicitPkName = "Id";
        public const string ImplicitIndexSuffix = "Id";

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

        public bool IsStoreAsText { get; private set; }

        public bool IsNotNull { get; private set; }

        public int MaxStringLength { get; private set; }

        private CreateFlags CreateFlags { get; set; }

        public bool IsAutoIncrement { get; set; }

        public bool IsAutoGuid { get; set; }

        public bool IsEnum { get; set; }

        public List<IndexModel> IndexModels { get; set; } = new();

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="property"></param>
        /// <param name="createFlags"></param>
        public PropertyMappingModel(IPropertySymbol property, CreateFlags createFlags )
        {
            this.CreateFlags = createFlags;

            // this property is not mapped due to access restrictions
            if (property.IsAbstract ||
                property.IsReadOnly ||
                property.SetMethod?.DeclaredAccessibility == Accessibility.Private)

            {
                return;
            }

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
                this.IsPrimaryKey = string.Compare(this.PropertyName, PropertyMappingModel.ImplicitPkName, StringComparison.OrdinalIgnoreCase) == 0;
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
                && this.PropertyName.EndsWith(PropertyMappingModel.ImplicitIndexSuffix, StringComparison.OrdinalIgnoreCase)
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

        private void ParseCollationAttribute(AttributeData attr)
        {
            this.ColumnCollation = (string)attr.GetAttributeConstructorValueByParameterName("collation");
        }

        private void ParsePrimaryKeyAttribute(AttributeData attr)
        {
            this.IsPrimaryKey = true;
        }

        private void ParseAutoIncrementAttribute(AttributeData attr)
        {
            this.IsAutoIncrement = true;
        }

        private void ParseStoreAsTextAttribute(AttributeData attr)
        {
            this.IsStoreAsText = true;
        }
        
        private void ParseNotNullAttribute(AttributeData attr)
        {
            this.IsNotNull = true;
        }
                
        private void ParseMaxLengthAttribute(AttributeData attr)
        {
            this.MaxStringLength = (int)attr.GetAttributeConstructorValueByParameterName("length");
        }

        private void ParseIndexedAttribute(AttributeData attr)
        {
            var index = new IndexModel();
            index.Name = (string)attr.GetAttributeConstructorValueByParameterName("name");
            index.Order = (int)attr.GetAttributeConstructorValueByParameterName("order");
            index.Unique = (bool)attr.GetAttributeConstructorValueByParameterName("unique");

            this.IndexModels.Add(index);
        }

        #endregion

        private static Dictionary<string, string> _ClrTypeMapping;

        /// <summary>
        /// Get column type from current property type
        /// </summary>
        private void IdentifyColumnType(IPropertySymbol property)
        {
            // Initialize Clr Type Mapping on first use
            if (_ClrTypeMapping == null)
            {
                _ClrTypeMapping = new ();

                var type = "integer";
                _ClrTypeMapping["Boolean"] = type;
                _ClrTypeMapping["Byte"] = type;
                _ClrTypeMapping["SByte"] = type;
                _ClrTypeMapping["Int16"] = type;
                _ClrTypeMapping["Int32"] = type;
                _ClrTypeMapping["Int64"] = type;
                _ClrTypeMapping["UInt16"] = type;
                _ClrTypeMapping["UInt32"] = type;
                _ClrTypeMapping["UInt64"] = type;
                
                _ClrTypeMapping["byte"] = type;
                _ClrTypeMapping["sbyte"] = type;
                _ClrTypeMapping["short"] = type;
                _ClrTypeMapping["int"] = type;
                _ClrTypeMapping["long"] = type;
                _ClrTypeMapping["ushort"] = type;
                _ClrTypeMapping["uint"] = type;
                _ClrTypeMapping["ulong"] = type;

                type = "float";
                _ClrTypeMapping["float"] = type;
                _ClrTypeMapping["double"] = type;
                _ClrTypeMapping["Decimal"] = type;
                
                type = "varchar";
                _ClrTypeMapping["string"] = type;
                _ClrTypeMapping["String"] = type;
                _ClrTypeMapping["StringBuilder"] = type;
                _ClrTypeMapping["Uri"] = type;
                _ClrTypeMapping["UriBuilder"] = type;
                
                type = "bigint";
                _ClrTypeMapping["TimeSpan"] = type;
                _ClrTypeMapping["DateTime"] = type;
                _ClrTypeMapping["DateTimeOffset"] = type;

                type = "blob";
                _ClrTypeMapping["byte[]"] = type;

                type = "varchar(36)";
                _ClrTypeMapping["Guid"] = type;

            }

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
        }
        
        
    }

}