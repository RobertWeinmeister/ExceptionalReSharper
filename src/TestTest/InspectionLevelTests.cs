using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;

namespace TestTest
{
    [TestFixture]
    public class CodeInspectionTests : CSharpHighlightingTestBase
    {
        [Test]
        public void TestAgain()
        {
            // Runs a specific set of files
            //DoTestSolution("../../../testdata/test.cs");
            Assert.That(true);
        }

       /// <inheritdoc />
       protected override PsiLanguageType CompilerIdsLanguage { get; }

    }
}
