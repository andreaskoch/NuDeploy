using System.Threading;

using NUnit.Framework;

namespace CommandLine.Tests.IntegrationTests.CommandLine
{
    [TestFixture]
    public class ProgramMainTests
    {
        private readonly Mutex sequentialTestExecutionMonitor;

        public ProgramMainTests()
        {
            this.sequentialTestExecutionMonitor = new Mutex(false);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            this.sequentialTestExecutionMonitor.WaitOne();

            CommandLineIntegrationTestUtilities.Cleanup();
        }

        [TearDown]
        public void TearDown()
        {
            this.sequentialTestExecutionMonitor.ReleaseMutex();
        }         
    }
}