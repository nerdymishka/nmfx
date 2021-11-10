using System;
using System.Collections.Generic;
using System.Text;
using Mettle;

[assembly: MettleTestFramework]

namespace Tests
{
    public class TestAttributeTests
    {
        [Test]
        public void VerifyTestAttributeWorks()
        {
            var assert = new AssertImpl();
            assert.True("special".Contains("cia"));
        }

        [UnitTest]
        public void VerifyUnitTestWorks()
        {
            var assert = new AssertImpl();
            assert.True("special".Contains("al"));
        }

        [IntegrationTest]
        public void VerifyIntegrationTestWorks()
        {
            var assert = new AssertImpl();
            assert.True("special".Contains("sp"));
        }

        [FunctionalTest]
        public void VerifyFunctionalTestWorks()
        {
            var assert = new AssertImpl();
            assert.True("special".Contains("ec"));
        }

        [Test(TicketKind = "Bug")]
        public void VerifySkip()
        {
            var assert = new AssertImpl();
            assert.Skip("Because I wanna");
        }
    }
}
