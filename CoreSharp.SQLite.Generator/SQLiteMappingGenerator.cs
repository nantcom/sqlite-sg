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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace CoreSharp.SQLite.Generator
{
    [Generator]
    public class SQLiteMappingGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is SQLiteMappingReceiver receiver)
            {
                foreach (var cls in receiver.MappedClasses)
                {
                    cls.Parse(context.Compilation);
                    context.AddSource(cls.Model.MappedClassName + "-TableMapping.cs", SourceText.From(cls.GenerateMappingClass(), Encoding.UTF8));
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SQLiteMappingReceiver());
        }


    }

    public sealed class SQLiteMappingReceiver : ISyntaxReceiver
    {
        /// <summary>
        /// All Classes in Target Project which has Table Attribute
        /// </summary>
        public List<SQLiteTableMappingParser> MappedClasses { get; } = new List<SQLiteTableMappingParser>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax typeDeclarationSyntax)
            {
                foreach (var attributeList in
                 typeDeclarationSyntax.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        if (attribute.Name.ToString() == "Table" ||
                          attribute.Name.ToString() == "TableAttribute")
                        {
                            this.MappedClasses.Add(new SQLiteTableMappingParser( typeDeclarationSyntax ));
                        }
                    }
                }
            }
        }
    }

}
