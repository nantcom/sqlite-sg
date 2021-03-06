﻿// this extension method is collected from many sample code from the internet

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NC.SQLite.SourceGen
{
    internal static class Extensions
    {
        public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
        {
            var current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
        {
            return type.GetBaseTypesAndThis().SelectMany(n => n.GetMembers());
        }

        public static CompilationUnitSyntax GetCompilationUnit(this SyntaxNode syntaxNode)
        {
            return syntaxNode.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();
        }

        public static string GetClassName(this ClassDeclarationSyntax proxy)
        {
            return proxy.Identifier.Text;
        }

        public static string GetClassModifier(this ClassDeclarationSyntax proxy)
        {
            return proxy.Modifiers.ToFullString().Trim();
        }

        public static bool HaveAttribute(this ClassDeclarationSyntax classSyntax, string attributeName)
        {
            return classSyntax.AttributeLists.Count > 0 &&
                   classSyntax.AttributeLists.SelectMany(al => al.Attributes
                           .Where(a => (a.Name as IdentifierNameSyntax).Identifier.Text == attributeName))
                       .Any();
        }


        public static string GetNamespace(this CompilationUnitSyntax root)
        {
            return root.ChildNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault()
                .Name
                .ToString();
        }

        public static List<string> GetUsings(this CompilationUnitSyntax root)
        {
            return root.ChildNodes()
                .OfType<UsingDirectiveSyntax>()
                .Select(n => n.Name.ToString())
                .ToList();
        }

        // FROM: https://www.codeproject.com/articles/861548/roslyn-code-analysis-in-easy-samples-part
        public static object GetAttributeConstructorValueByParameterName(this AttributeData attributeData, string argName)
        {

            // Get the parameter
            IParameterSymbol parameterSymbol = attributeData.AttributeConstructor
                .Parameters
                .Where((constructorParam) => constructorParam.Name == argName).FirstOrDefault();

            // get the index of the parameter
            int parameterIdx = attributeData.AttributeConstructor.Parameters.IndexOf(parameterSymbol);

            if (parameterIdx == -1)
            {
                return null;
            }

            // get the construct argument corresponding to this parameter
            TypedConstant constructorArg = attributeData.ConstructorArguments[parameterIdx];

            // return the value passed to the attribute
            return constructorArg.Value;
        }

        /// <summary>
        /// Escape Quote in the string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EscapeQuote(this string input)
        {
            return input.Replace("\"", "\\\"");
        }
    }
}
