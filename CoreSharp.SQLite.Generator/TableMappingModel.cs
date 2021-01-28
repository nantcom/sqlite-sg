using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CoreSharp.SQLite.Generator
{
    public class TableMappingModel
    {
        public string Namespace { get; set; }

        public string MappedClassName { get; set; }

        public string TableName { get; set; }

        public string CreateFlagsAsSpecified { get; set; }

        public int CreateFlagsInt { get; set; }

        public List<PropertyMappingModel> ColumnMappingModels { get; set; } = new();

        public TableMappingModel(AttributeData attr)
        {
            this.TableName = attr.GetAttributeConstructorValueByParameterName("name") as string;
            this.CreateFlagsInt = (int)attr.GetAttributeConstructorValueByParameterName("flags");
            this.CreateFlagsAsSpecified = attr.GetAttributeConstructorValueByParameterName("flags").ToString();
        }
    }
}