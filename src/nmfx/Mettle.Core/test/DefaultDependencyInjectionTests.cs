using System;
using System.Collections.Generic;
using System.Text;
using Mettle;
using Xunit.Sdk;

namespace Tests
{
    public class DefaultDependencyInjectionTests
    {
        [UnitTest]
        public void VerifyAssertInjection(IAssert assert2)
        {
            var assert = new AssertImpl();
            assert.NotNull(assert2);
            assert.Throws<Mettle.Sdk.EqualException>(()
                => assert2.Equal("left", "right"));
        }

        [UnitTest]
        public void VerifyTestCaseInjection(IXunitTestCase testCase)
        {
            var assert = new AssertImpl();
            assert.NotNull(testCase);
        }

        [UnitTest(TicketKind = "Bug")]
        public void VerifyTicketKind(IXunitTestCase testCase)
        {
            var assert = new AssertImpl();
            assert.NotNull(testCase);
            testCase.Traits.TryGetValue("ticketKind", out var values);
            assert.NotNull(values);
            assert.Equal("Bug", values[0]);
        }

        [UnitTest]
        public void VerifyServiceProvider(IServiceProvider serviceProvider)
        {
            var assert = new AssertImpl();
            var autoResolve = serviceProvider.GetService(typeof(DefaultDependencyInjectionTests));
            assert.NotNull(autoResolve);
            assert.IsType<DefaultDependencyInjectionTests>(autoResolve);
        }
    }
}
