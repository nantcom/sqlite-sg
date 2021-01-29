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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;
using System;
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
