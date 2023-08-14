using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;

namespace ExceptionalTests.SystemTesting
{
    [TestFixture]
    public class CodeInspectionTests : CSharpHighlightingTestBase
    {
        [Test]
        public void Test1()
        {
            // Runs a specific set of files
            DoTestSolution("../../../testdata/test.cs2");
            Assert.That(true);
        }

        /// <inheritdoc />
        //protected override PsiLanguageType CompilerIdsLanguage { get; }
    }
}
