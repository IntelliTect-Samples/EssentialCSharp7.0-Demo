using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Referenced: http://www.tugberkugurlu.com/archive/compiling-c-sharp-code-into-memory-and-executing-it-with-roslyn

namespace CSharp7
{
    public class CompilerAssert
    {
        public static void StatementsFailCompilation(
            String statements, params string[] expectedErrors)
        {
            string expectedErrorsText =
                expectedErrors.OrderBy(item => item).Aggregate((accumulator, item) =>
                    string.Join(Environment.NewLine, accumulator, item));
            StatementsFailCompilation(statements, expectedErrorsText);
        }
        public static void StatementsFailCompilation(
            String statements, IEnumerable<string> expectedErrors)
        {
            expectedErrors = expectedErrors.OrderBy(item => item);
            string expectedErrorsText =
                expectedErrors.Aggregate((accumulator, item) =>
                    string.Join(Environment.NewLine, accumulator, item));
            StatementsFailCompilation(statements, expectedErrorsText);
        }

        public static void StatementsFailCompilation(string methodBodycode, string expectedErrorsText)
        {
            string methodName = "M" + Guid.NewGuid().ToString().Replace("-", "");
            ClassMembersFailCompilation($@"
            void { methodName }()
            {{
                { methodBodycode }
            }}
            ", expectedErrorsText);
        }

        public static void ClassMembersFailCompilation(string classMemberCode, string expectedErrorsText)
        {
            string className = "C" + Guid.NewGuid().ToString().Replace("-", "");
            CodeFailCompilation($@"
            class { className }
            {{
                { classMemberCode }
            }}
            ", expectedErrorsText);
        }

        public static void CodeFailCompilation(
            string code, string expectedErrorsText)
        {
            IEnumerable<string> actualErrors = new List<string>();
            code = $@"
                using System;
                using System.IO;
                using System.Collections.Generic;
                using CSharp7;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                { code }
                ";
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
                code , new CSharpParseOptions(kind: SourceCodeKind.Script)
            );
            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references;
            // Use the following in .NET Core
            // See https://github.com/aspnet/Announcements/issues/149
            /*
            references =
                DependencyContext.Default.CompileLibraries.SelectMany(
                    item => item.ResolveReferencePaths()).Select(
                    item => MetadataReference.CreateFromFile(item)).ToArray();
            */
            references = AppDomain.CurrentDomain.GetAssemblies().Select(
                item => MetadataReference.CreateFromFile(item.Location, 
                new MetadataReferenceProperties())).ToArray();

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree }, 
                references: references,
                options: 
                    new CSharpCompilationOptions(
                        outputKind: OutputKind.DynamicallyLinkedLibrary
                        ));
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                IEnumerable<Diagnostic> relevantDiagnostics = result.Diagnostics.Where(
                    item => item.Severity >= DiagnosticSeverity.Warning);
                if (!result.Success || relevantDiagnostics.Count()>0)
                {
                    actualErrors = relevantDiagnostics.Select(item =>
                        $"{item.Severity} {item.Id}: {item.GetMessage()}");
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());
                }
            }
            actualErrors = actualErrors.OrderBy(item => item);
            string actualErrorsText =
                actualErrors.Aggregate((accumulator, item) =>
                    string.Join(Environment.NewLine, accumulator, item));
            //string message = $"Expected '{expectedErrorsText}' but was '{actualErrorsText}'";
            Assert.AreEqual<string>(expectedErrorsText, actualErrorsText);
        }
    }

}