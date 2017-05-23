using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyModel;
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

        private static MetadataReference CreateMetadataReference(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var moduleMetadata = ModuleMetadata.CreateFromStream(stream, System.Reflection.PortableExecutable.PEStreamOptions.PrefetchMetadata);
                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);

                return assemblyMetadata.GetReference(filePath: path);
            }
        }

        private static List<MetadataReference> GetReferences()
        {
            var applicationAssembly = Assembly.GetExecutingAssembly();
            var dependencyContext = DependencyContext.Load(applicationAssembly);
            var references = dependencyContext
                ?.CompileLibraries
                .SelectMany(library => library.ResolveReferencePaths())
                .ToList();

            var libraryPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var metadataReferences = new List<MetadataReference>();
            foreach (var path in references ?? Enumerable.Empty<string>())
            {
                if (libraryPaths.Add(path))
                {
                    var metadataReference = CreateMetadataReference(path);
                    metadataReferences.Add(metadataReference);
                }
            }

            return metadataReferences;
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

            List<MetadataReference> references;

            // FAILS: DependencyContext.Load returns null with .NET 4.6
            //DependencyContext dependencyContext = DependencyContext.Load(
            //        Assembly.GetExecutingAssembly());
            //references = dependencyContext.CompileLibraries.SelectMany(
            //        item => item.ResolveReferencePaths()).Select(
            //        item => MetadataReference.CreateFromFile(item)).Cast< MetadataReference>().ToList();

            references = (AppDomain.CurrentDomain.GetAssemblies().Select(
                item => MetadataReference.CreateFromFile(item.Location,
                new MetadataReferenceProperties()))).Cast<MetadataReference>().ToList();
            //MetadataReference valueTupleReference = MetadataReference.CreateFromFile(typeof(ValueTuple).Assembly.Location);
            // references.Add(valueTupleReference);

            // IDIOSYNCRASY: https://github.com/dotnet/roslyn/issues/18775
            references = references.Where(item => !item.Display.EndsWith($"{nameof(System.ValueTuple)}.dll")).ToList();

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree }, 
                references: references,
                options: 
                    new CSharpCompilationOptions(
                        outputKind: OutputKind.DynamicallyLinkedLibrary
                        ));
            //compilation.AddReferences(new MetadataReference(typeof(object).Assembly.Location));
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