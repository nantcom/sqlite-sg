using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
