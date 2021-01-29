using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CoreSharp.SQLite.Generator
{

    public sealed class SQLiteTableMappingParser
    {
        /// <summary>
        /// Current Understanding of the class
        /// </summary>
        public TableMappingModel Model { get; private set; }

        private static string ClasssTemplate = null;


        private ClassDeclarationSyntax _Class;

        public SQLiteTableMappingParser(ClassDeclarationSyntax classDecl)
        {
            _Class = classDecl;
        }

        public void Parse(Compilation compilation)
        {
            var root = _Class.GetCompilationUnit();
            var classSemanticModel = compilation.GetSemanticModel(_Class.SyntaxTree);
            var classSymbol = classSemanticModel.GetDeclaredSymbol(_Class);

            var tableAttribute = classSymbol.GetAttributes().Where(attr => attr.AttributeClass.Name == "TableAttribute").First();

            this.Model = new TableMappingModel(tableAttribute);
            this.Model.Namespace = root.GetNamespace();
            this.Model.MappedClassName = classSymbol.Name;

            var createFlags = (CreateFlags)this.Model.CreateFlagsInt;

            var columnMappings = from member in classSymbol.GetMembers()
                                   where member is IPropertySymbol
                                   let colModel = new ColumnMappingModel((IPropertySymbol)member, createFlags)
                                   where colModel.IsMapped
                                   select colModel;

            this.Model.ColumnMappingModels = columnMappings.ToList();
            this.Model.Update();
        }

        public string GenerateMappingClass()
        {
            if (SQLiteTableMappingParser.ClasssTemplate == null)
            {
                SQLiteTableMappingParser.ClasssTemplate = ResourceReader.GetResource("StaticTableMappingTemplate.sbncs");
            }

            var template = Template.Parse(SQLiteTableMappingParser.ClasssTemplate);
            if (template.HasErrors)
            {
                var errors = string.Join(" | ", template.Messages.Select(x => x.Message));
                throw new InvalidOperationException($"Template parse error: {template.Messages}");
            }

            var result = template.Render(this.Model, member => member.Name);

            return result;
        }

    }
}
