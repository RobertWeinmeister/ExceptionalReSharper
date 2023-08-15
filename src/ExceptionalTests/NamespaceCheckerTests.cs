using NUnit.Framework;
using ReSharper.ExceptionalContinued.Utilities;

namespace ExceptionalTests
{
    [TestFixture]
    public class NamespaceCheckerTests
    {
        [Test]
        public void NamespaceDoesNotMatchDirectly() =>
            Assert.Multiple(static () =>
                            {
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "not"), Is.False);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "start.middle"),
                                            Is.False);
                            });

        [Test]
        public void NamespaceMatchesDirectly() =>
            Assert.Multiple(static () =>
                            {
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "start"),  Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "middle"), Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "end"),    Is.True);
                            });

        [Test]
        public void NamespaceWithWildcardMatches() =>
            Assert.Multiple(static () =>
                            {
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "start*"), Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "s*t"),    Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "*art"),   Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "*start"), Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "*dd*"),   Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "m*d*e"),  Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "*end"),   Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "end*"),   Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "e*d"),    Is.True);
                            });

        [Test]
        public void NamespaceWithWildcardMatchesNotAcross() =>
            Assert.Multiple(static () =>
                            {
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "start*end"),
                                            Is.False);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "s*m"), Is.False);
                            });

        [Test]
        public void NamespaceWithWildcardSingleMatches() =>
            Assert.Multiple(static () =>
                            {
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "st?rt"),  Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "m?ddl?"), Is.True);
                                Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "e?d"),    Is.True);
                            });

        [Test]
        public void NamespaceWithWildcardSingleMatchesNotAcross() =>
            Assert.That(NamespaceChecker.HasNamespaceMatch("start.middle.end", "start*middle"), Is.False);
    }
}