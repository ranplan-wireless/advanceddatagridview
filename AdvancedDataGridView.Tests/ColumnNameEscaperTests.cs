using NUnit.Framework;
using Zuby.ADGV;

namespace Zuby.ADGV.Tests
{
    [TestFixture]
    public class ColumnNameEscaperTests
    {
        [Test]
        public void GivenNameWithoutBracket_WhenEscape_ThenUnchanged()
        {
            Assert.That(ColumnNameEscaper.Escape("RX_LEVEL"), Is.EqualTo("RX_LEVEL"));
        }

        [Test]
        public void GivenNameWithOpenBracketOnly_WhenEscape_ThenUnchanged()
        {
            Assert.That(ColumnNameEscaper.Escape("Signal [dB"), Is.EqualTo("Signal [dB"));
        }

        [Test]
        public void GivenNameWithClosingBracket_WhenEscape_ThenBackslashEscaped()
        {
            Assert.That(ColumnNameEscaper.Escape("WiFi RSSI [dBm]"), Is.EqualTo(@"WiFi RSSI [dBm\]"));
        }

        [Test]
        public void GivenEscapedName_WhenUnescape_ThenOriginalName()
        {
            Assert.That(ColumnNameEscaper.Unescape(@"WiFi RSSI [dBm\]"), Is.EqualTo("WiFi RSSI [dBm]"));
        }

        [Test]
        public void GivenPlainToken_WhenUnescape_ThenUnchanged()
        {
            Assert.That(ColumnNameEscaper.Unescape("RX_LEVEL"), Is.EqualTo("RX_LEVEL"));
        }

        [Test]
        public void GivenExpressionWithEscapedTokens_WhenParseColumnTokens_ThenRealNames()
        {
            var expression = @"([WiFi RSSI [dBm\]] IN ('-44')) AND ([X] = '1')";

            Assert.That(ColumnNameEscaper.ParseColumnTokens(expression),
                Is.EqualTo(new[] { "WiFi RSSI [dBm]", "X" }));
        }

        [Test]
        public void GivenLegacyUnescapedExpression_WhenParseColumnTokens_ThenNames()
        {
            Assert.That(ColumnNameEscaper.ParseColumnTokens("([Y] IN ('1.64'))"),
                Is.EqualTo(new[] { "Y" }));
        }

        [Test]
        public void GivenSortExpressionWithEscapedToken_WhenParseColumnTokens_ThenRealName()
        {
            Assert.That(ColumnNameEscaper.ParseColumnTokens(@"[WiFi RSSI [dBm\]] ASC, [X] DESC"),
                Is.EqualTo(new[] { "WiFi RSSI [dBm]", "X" }));
        }

        [Test]
        public void GivenTemplateAndBracketName_WhenFormatColumnExpression_ThenEscapedReference()
        {
            Assert.That(ColumnNameEscaper.FormatColumnExpression("([{0}] IN ('-44'))", "WiFi RSSI [dBm]"),
                Is.EqualTo(@"([WiFi RSSI [dBm\]] IN ('-44'))"));
        }

        [Test]
        public void GivenTemplateAndPlainName_WhenFormatColumnExpression_ThenPlainReference()
        {
            Assert.That(ColumnNameEscaper.FormatColumnExpression("[{0}] ASC", "X"),
                Is.EqualTo("[X] ASC"));
        }
    }
}
