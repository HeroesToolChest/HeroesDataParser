using Heroes.Icons.Parser.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Heroes.Icons.Parser.Tests
{
    [TestClass]
    public class TooltipDescriptionTests
    {
        private readonly string TestDescription = "<img path=\"QuestIcon\"/>Deal <c val=\"#TooltipNumbers\">500~~0.035~~</c> damage<n/>Deal an additional <c val=\"#TooltipNumbers\">200~~0.04~~ </c>damage per second";

        private readonly string PlainText = "Deal 500 damage Deal an additional 200 damage per second";
        private readonly string PlainTextWithNewlines = "Deal 500 damage<n/>Deal an additional 200 damage per second";
        private readonly string PlainTextWithScaling = "Deal 500 (+3.5% per level) damage Deal an additional 200 (+4% per level) damage per second";
        private readonly string PlainTextWithScalingWithNewlines = "Deal 500 (+3.5% per level) damage<n/>Deal an additional 200 (+4% per level) damage per second";
        private readonly string ColoredText = "<img path=\"QuestIcon\"/>Deal <c val=\"#TooltipNumbers\">500</c> damage<n/>Deal an additional <c val=\"#TooltipNumbers\">200 </c>damage per second";
        private readonly string ColoredTextWithScaling = "<img path=\"QuestIcon\"/>Deal <c val=\"#TooltipNumbers\">500 (+3.5% per level)</c> damage<n/>Deal an additional <c val=\"#TooltipNumbers\">200 (+4% per level) </c>damage per second";

        [TestMethod]
        public void DescriptionTest()
        {
            TooltipDescription tooltipDescription = new TooltipDescription(TestDescription);

            Assert.IsTrue(tooltipDescription.GetPlainText == PlainText);
            Assert.IsTrue(tooltipDescription.GetPlainTextWithNewlines == PlainTextWithNewlines);
            Assert.IsTrue(tooltipDescription.GetPlainTextWithScaling == PlainTextWithScaling);
            Assert.IsTrue(tooltipDescription.GetPlainTextWithScalingWithNewlines == PlainTextWithScalingWithNewlines);
            Assert.IsTrue(tooltipDescription.GetColoredText == ColoredText);
            Assert.IsTrue(tooltipDescription.GetColoredTextWithScaling == ColoredTextWithScaling);
        }
    }
}
