using System;
using System.Collections.Generic;
using System.Text;
using Mettle;

namespace NerdyMishka.Mettle.Core.Tests
{
    public class TestAttributeTests
    {
        [Test]
        public void VerifyTestAttributeWorks()
        {
            var assert = new AssertImpl();
            assert.True("special".Contains("cia"));
        }

        [Test(TicketKind = "Bug")]
        public void VerifyAttributes()
        {
            var assert = new AssertImpl();
            assert.True("bug".Contains("ug"));
        }

        [Test(TicketKind = "Bug")]
        public void VerifySkip()
        {
            var assert = new AssertImpl();
            assert.Skip("Because I wanna");
        }
    }
}
