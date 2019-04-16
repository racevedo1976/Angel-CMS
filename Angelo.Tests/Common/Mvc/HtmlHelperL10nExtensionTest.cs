using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Angelo.Tests.Common.Mvc
{
    /// <summary>
    /// Contains unit tests for HtmlHelperL10n extension methods.
    /// </summary>
    [TestClass]
    public class HtmlHelperL10nExtensionTest
    {
        #region Localize
        #endregion // Localize
        #region ShortNameFor
        [TestMethod]
        [TestCategory("HtmlHelperL10nExtensions.Bounds")]
        [Description("Ensures that HtmlHelperL10nExtensions.ShortNameFor throws an ArgumentNullException when the 'helper' argument is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShortNameForShouldThrowWhenHelperIsNull()
        {
            // Arrange
            var helper = default(IHtmlHelper<string>);
            var expression = ;

            // Execution should not reach this point
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("HtmlHelperL10nExtensions.Bounds")]
        [Description("Ensures that HtmlHelperL10nExtensions.ShortNameFor throws an ArgumentNullException when the 'helper' argument is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShortNameForShouldThrowWhenExpressionIsNull()
        {

        }
        #endregion // ShortNameFor
    }
}
