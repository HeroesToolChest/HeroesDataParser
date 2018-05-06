using Heroes.Icons.Parser.Descriptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Heroes.Icons.Parser.Tests
{
    [TestClass]
    public class DescriptionValidationTests
    {
        private readonly string NoTagsDescription = "previous location.";
        private readonly string NormalTagsDescription1 = "Every <c val=\"#TooltipNumbers\">18</c> seconds, deals <c val=\"#TooltipNumbers\">125</c> bonus by <c val=\"#TooltipNumbers\">2</c> seconds.";
        private readonly string NormalTagsDescription2 = "<c val=\"#TooltipQuest\"> Repeatable Quest:</c> Gain<c val=\"#TooltipNumbers\">10</c>";
        private readonly string ExtraTagDescription1 = "</w>previous location.";
        private readonly string ExtraTagDescription2 = "previous location.</w>";
        private readonly string ExtraTagDescription3 = "previous </w>location.";
        private readonly string ExtraTagDescription4 = "previous <w>location.";
        private readonly string ExtraTagDescription5 = "previous <w><w>location.";
        private readonly string NewLineTagDescription1 = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%</c><n/>Health Per Second Bonus: <c val=\"#TooltipNumbers\">0</c>";
        private readonly string NewLineTagDescription2 = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%</c><n/><n/>Health Per Second Bonus: <c val=\"#TooltipNumbers\">0</c>";
        private readonly string SelfCloseTagDescription1 = "<img path=\"sdf\"/>previous location.";
        private readonly string SelfCloseTagDescription2 = "previous<img path=\"sdf\"/>";
        private readonly string SelfCloseTagDescription3 = "previous<img path=\"sdf\"/><c val=\"#TooltipQuest\"> Repeatable Quest:</c>";
        private readonly string SelfCloseTagDescription4 = "previous<c val=\"#TooltipQuest\"> Repeatable Quest:</c><img path=\"sdf\"/>";
        private readonly string DuplicateTagsDescription1 = "<c val=\"#TooltipQuest\"> Repeatable Quest:</c> Gain<c val=\"#TooltipNumbers\">10</c></c>";
        private readonly string DuplicateTagsDescription2 = "<c val=\"#TooltipQuest\"> Repeatable Quest:</c></c> Gain<c val=\"#TooltipNumbers\">10</c></c>";

        // Convert newline tags </n> to <n/>
        private readonly string ConvertNewLineTagDescription1 = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%</c></n>Health Per Second Bonus: <c val=\"#TooltipNumbers\">0</c>";
        private readonly string ConvertNewLineTagDescription1Corrected = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%</c><n/>Health Per Second Bonus: <c val=\"#TooltipNumbers\">0</c>";
        private readonly string ConvertNewLineTagDescription2 = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%</c></n></n>Health Per Second Bonus: <c val=\"#TooltipNumbers\">0</c>";
        private readonly string ConvertNewLineTagDescription2Corrected = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%</c><n/><n/>Health Per Second Bonus: <c val=\"#TooltipNumbers\">0</c>";
        private readonly string ConvertNewLineTagDescription3 = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%</c></n></n>Health Per Second Bonus: <c val=\"#TooltipNumbers\">0</c></n>";
        private readonly string ConvertNewLineTagDescription3Corrected = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%</c><n/><n/>Health Per Second Bonus: <c val=\"#TooltipNumbers\">0</c><n/>";

        // Case tags
        private readonly string UpperCaseTagDescription1 = "<C val=\"#TooltipQuest\"> Repeatable Quest:</C> Gain<C val=\"#TooltipNumbers\">10</c>";
        private readonly string UpperCaseTagDescription1Corrected = "<c val=\"#TooltipQuest\"> Repeatable Quest:</c> Gain<c val=\"#TooltipNumbers\">10</c>";

        // space in tags
        private readonly string ExtraSpacesTagDescription1 = "<c  val=\"#TooltipQuest\"> Repeatable Quest:</c> Gain<c val=\"#TooltipNumbers\">10</c>";
        private readonly string ExtraSpacesTagDescription2 = "<c     val=\"#TooltipQuest\"> Repeatable Quest:</c> Gain<c val=\"#TooltipNumbers\">10</c>";

        // Empty text tags
        private readonly string EmptyTagsDescription1 = "<c val=\"#TooltipQuest\"></c><c val=\"#TooltipNumbers\"></c>";
        private readonly string EmptyTagsDescription1Corrected = string.Empty;
        private readonly string EmptyTagsDescription2 = "test1<c val=\"#TooltipQuest\">test2</c>test3 <c val=\"#TooltipNumbers\"></c>";
        private readonly string EmptyTagsDescription2Corrected = "test1<c val=\"#TooltipQuest\">test2</c>test3 ";
        private readonly string EmptyTagsDescription3 = "<c val=\"#TooltipQuest\"></C>test1<c val=\"#TooltipQuest\">test2</c>test3 <c val=\"#TooltipNumbers\"></c>";
        private readonly string EmptyTagsDescription3Corrected = "test1<c val=\"#TooltipQuest\">test2</c>test3 ";

        // nested tags
        private readonly string NestedTagDescription1 = "<c val=\"FF8000\">Gain <c val=\"#TooltipNumbers\">30%</c> points</c>";
        private readonly string NestedTagDescription1Corrected = "<c val=\"FF8000\">Gain </c><c val=\"#TooltipNumbers\">30%</c><c val=\"FF8000\"> points</c>";
        private readonly string NestedTagDescription2 = "<c val=\"FF8000\">Gain <c val=\"#TooltipNumbers\">30%</c> points <c val=\"#TooltipNumbers\">30%</c> charges</c>";
        private readonly string NestedTagDescription2Corrected = "<c val=\"FF8000\">Gain </c><c val=\"#TooltipNumbers\">30%</c><c val=\"FF8000\"> points </c><c val=\"#TooltipNumbers\">30%</c><c val=\"FF8000\"> charges</c>";
        private readonly string NestedTagDescription3 = "<c val=\"FF8000\"><c val=\"#TooltipNumbers\"></c></c>";
        private readonly string NestedTagDescription3Corrected = string.Empty;
        private readonly string NestedTagDescription4 = "<c val=\"FF8000\">45%<c val=\"#TooltipNumbers\"></c></c>";
        private readonly string NestedTagDescription4Corrected = "<c val=\"FF8000\">45%</c>";

        // nested new line
        private readonly string NestedNewLineTagDescription1 = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%<n/>5%</c>Health <c val=\"#TooltipNumbers\">0</c>"; // new line between c tags
        private readonly string NestedNewLineTagDescription1Corrected = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%</c><n/><c val=\"#TooltipNumbers\">5%</c>Health <c val=\"#TooltipNumbers\">0</c>";
        private readonly string NestedNewLineTagDescription2 = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%<n/></c>Health <c val=\"#TooltipNumbers\">0</c>";
        private readonly string NestedNewLineTagDescription2Corrected = "Max Health Bonus: <c val=\"#TooltipNumbers\">0%</c><n/>Health <c val=\"#TooltipNumbers\">0</c>";

        // real descriptions
        private readonly string DiabloBlackSoulstone = "<img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Repeatable Quest:</c> Gain <c val=\"#TooltipNumbers\">10</c> Souls per Hero killed and <c val=\"#TooltipNumbers\">1</c> Soul per Minion, up to <c val=\"#TooltipNumbers\">100</c>. For each Soul, gain <c val=\"#TooltipNumbers\">0.4%</w></c> maximum Health. If Diablo has <c val=\"#TooltipNumbers\">100</c> Souls upon dying, he will resurrect in <c val=\"#TooltipNumbers\">5</c> seconds but lose <c val=\"#TooltipNumbers\">100</c> Souls.";
        private readonly string DiabloBlackSoulstoneCorrected = "<img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Repeatable Quest:</c> Gain <c val=\"#TooltipNumbers\">10</c> Souls per Hero killed and <c val=\"#TooltipNumbers\">1</c> Soul per Minion, up to <c val=\"#TooltipNumbers\">100</c>. For each Soul, gain <c val=\"#TooltipNumbers\">0.4%</c> maximum Health. If Diablo has <c val=\"#TooltipNumbers\">100</c> Souls upon dying, he will resurrect in <c val=\"#TooltipNumbers\">5</c> seconds but lose <c val=\"#TooltipNumbers\">100</c> Souls.";
        private readonly string DVaMechSelfDestruct = "Eject from the Mech, setting it to self-destruct after <c val=\"#TooltipNumbers\">4</c> seconds. Deals <c val=\"#TooltipNumbers\">400</c> to <c val=\"#TooltipNumbers\">1200</c> damage in a large area, depending on distance from center. Only deals <c val=\"#TooltipNumbers\">50%</c> damage against Structures.</n></n><c val=\"FF8000\">Gain <c val=\"#TooltipNumbers\">1%</c> Charge for every <c val=\"#TooltipNumbers\">2</c> seconds spent Basic Attacking, and <c val=\"#TooltipNumbers\">30%</c> Charge per <c val=\"#TooltipNumbers\">100%</c> of Mech Health lost.</c>";
        private readonly string DVaMechSelfDestructCorrected = "Eject from the Mech, setting it to self-destruct after <c val=\"#TooltipNumbers\">4</c> seconds. Deals <c val=\"#TooltipNumbers\">400</c> to <c val=\"#TooltipNumbers\">1200</c> damage in a large area, depending on distance from center. Only deals <c val=\"#TooltipNumbers\">50%</c> damage against Structures.<n/><n/><c val=\"FF8000\">Gain </c><c val=\"#TooltipNumbers\">1%</c><c val=\"FF8000\"> Charge for every </c><c val=\"#TooltipNumbers\">2</c><c val=\"FF8000\"> seconds spent Basic Attacking, and </c><c val=\"#TooltipNumbers\">30%</c><c val=\"FF8000\"> Charge per </c><c val=\"#TooltipNumbers\">100%</c><c val=\"FF8000\"> of Mech Health lost.</c>";
        private readonly string ValeeraCheapShot = "Deal <c val=\"#TooltipNumbers\">30</c> damage to an enemy, Stun them for <c val=\"#TooltipNumbers\">0.75</c> seconds, and Blind them for <c val=\"#TooltipNumbers\">2</c> seconds once Cheap Shot's Stun expires.<n/><n/><c val=\"#GlowColorRed\">Awards 1 Combo Point.</c><n/><n/><c val=\"#ColorViolet\">Unstealth: Blade Flurry<n/></c>Deal damage in an area around Valeera.";
        private readonly string ValeeraCheapShotCorrected = "Deal <c val=\"#TooltipNumbers\">30</c> damage to an enemy, Stun them for <c val=\"#TooltipNumbers\">0.75</c> seconds, and Blind them for <c val=\"#TooltipNumbers\">2</c> seconds once Cheap Shot's Stun expires.<n/><n/><c val=\"#GlowColorRed\">Awards 1 Combo Point.</c><n/><n/><c val=\"#ColorViolet\">Unstealth: Blade Flurry</c><n/>Deal damage in an area around Valeera.";
        private readonly string CrusaderPunish = "Step forward dealing <c val=\"#TooltipNumbers\">113</c> damage and Slowing enemies by <c val=\"#TooltipNumbers\">60%</c> decaying over <c val=\"#TooltipNumbers\">2</c> seconds.";
        private readonly string CrusaderPunishSame = "Step forward dealing <c val=\"#TooltipNumbers\">113</c> damage and Slowing enemies by <c val=\"#TooltipNumbers\">60%</c> decaying over <c val=\"#TooltipNumbers\">2</c> seconds.";

        // plain text
        private readonly string PlainText1 = "Gain 30% points"; // NestedTagDescription1
        private readonly string PlainText2 = "Max Health Bonus: 0% Health 0"; // NestedNewLineTagDescription2Corrected
        private readonly string PlainText3 = "Deal 30 damage to an enemy, Stun them for 0.75 seconds, and Blind them for 2 seconds once Cheap Shot's Stun expires.  Awards 1 Combo Point.  Unstealth: Blade Flurry Deal damage in an area around Valeera."; // ValeeraCheapShotCorrected
        private readonly string PlainText4 = "<c val=\"#TooltipNumbers\">100~~0.04~~</c> damage per second<n/>";
        private readonly string PlainText4Corrected = "100 damage per second";
        private readonly string PlainText5 = "<c val=\"#TooltipNumbers\">100~~0.04~~</c> damage per second ~~0.05~~";
        private readonly string PlainText5Corrected = "100 damage per second";

        // plain text with newlines
        private readonly string PlainTextNewline1 = "Max Health Bonus: 0%<n/>5%Health 0"; // NestedNewLineTagDescription1Corrected
        private readonly string PlainTextNewline2 = "Deal 30 damage to an enemy, Stun them for 0.75 seconds, and Blind them for 2 seconds once Cheap Shot's Stun expires.<n/><n/>Awards 1 Combo Point.<n/><n/>Unstealth: Blade Flurry<n/>Deal damage in an area around Valeera."; // ValeeraCheapShotCorrected
        private readonly string PlainTextNewline3 = "<c val=\"#TooltipNumbers\">100~~0.04~~</c><n/> damage per second ~~0.05~~";
        private readonly string PlainTextNewline3Corrected = "100<n/> damage per second";

        // plain text with scaling
        private readonly string PlainTextScaling1 = "<c val=\"#TooltipNumbers\">120~~0.04~~</c><n/> damage per second";
        private readonly string PlainTextScaling1Corrected = "120 (+4% per level)  damage per second";
        private readonly string PlainTextScaling2 = "<c val=\"#TooltipNumbers\">120~~0.05~~</c> damage per second ~~0.035~~<n/>";
        private readonly string PlainTextScaling2Corrected = "120 (+5% per level) damage per second  (+3.5% per level)";

        // plain text with scaling newlines
        private readonly string PlainTextScalingNewline1 = "<c val=\"#TooltipNumbers\">120~~0.04~~</c><n/> damage per second";
        private readonly string PlainTextScalingNewline1Corrected = "120 (+4% per level)<n/> damage per second";
        private readonly string PlainTextScalingNewline2 = "<c val=\"#TooltipNumbers\">120~~0.05~~</c> damage per <n/> second ~~0.035~~";
        private readonly string PlainTextScalingNewline2Corrected = "120 (+5% per level) damage per <n/> second  (+3.5% per level)";

        // colored text
        private readonly string ColoredText1 = "<c val=\"#TooltipNumbers\">100~~0.04~~</c><n/> damage per second<n/>";
        private readonly string ColoredText1Corrected = "<c val=\"#TooltipNumbers\">100</c><n/> damage per second<n/>";
        private readonly string ColoredText2 = "<c val=\"#TooltipNumbers\">100~~0.04~~</c> damage per second ~~0.05~~";
        private readonly string ColoredText2Corrected = "<c val=\"#TooltipNumbers\">100</c> damage per second";

        // colored text with scaling
        private readonly string ColoredTextScaling1 = "<c val=\"#TooltipNumbers\">100~~0.04~~</c><n/> damage per second<n/>";
        private readonly string ColoredTextScaling1Corrected = "<c val=\"#TooltipNumbers\">100 (+4% per level)</c><n/> damage per second<n/>";
        private readonly string ColoredTextScaling2 = "<c val=\"#TooltipNumbers\">100~~0.04~~</c> damage per second ~~0.05~~";
        private readonly string ColoredTextScaling2Corrected = "<c val=\"#TooltipNumbers\">100 (+4% per level)</c> damage per second  (+5% per level)";

        [TestMethod]
        public void ValidateTests()
        {
            Assert.IsTrue(DescriptionValidation.Validate(NoTagsDescription) == NoTagsDescription); // no changes
            Assert.IsTrue(DescriptionValidation.Validate(NormalTagsDescription1) == NormalTagsDescription1); // no changes
            Assert.IsTrue(DescriptionValidation.Validate(NormalTagsDescription2) == NormalTagsDescription2); // no changes
            Assert.IsTrue(DescriptionValidation.Validate(ExtraTagDescription1) == NoTagsDescription);
            Assert.IsTrue(DescriptionValidation.Validate(ExtraTagDescription2) == NoTagsDescription);
            Assert.IsTrue(DescriptionValidation.Validate(ExtraTagDescription3) == NoTagsDescription);
            Assert.IsTrue(DescriptionValidation.Validate(ExtraTagDescription4) == NoTagsDescription);
            Assert.IsTrue(DescriptionValidation.Validate(ExtraTagDescription5) == NoTagsDescription);
            Assert.IsTrue(DescriptionValidation.Validate(NewLineTagDescription1) == NewLineTagDescription1); // no changes
            Assert.IsTrue(DescriptionValidation.Validate(NewLineTagDescription2) == NewLineTagDescription2); // no changes
            Assert.IsTrue(DescriptionValidation.Validate(SelfCloseTagDescription1) == SelfCloseTagDescription1); // no changes
            Assert.IsTrue(DescriptionValidation.Validate(SelfCloseTagDescription2) == SelfCloseTagDescription2); // no changes
            Assert.IsTrue(DescriptionValidation.Validate(SelfCloseTagDescription3) == SelfCloseTagDescription3); // no changes
            Assert.IsTrue(DescriptionValidation.Validate(SelfCloseTagDescription4) == SelfCloseTagDescription4); // no changes
            Assert.IsTrue(DescriptionValidation.Validate(DuplicateTagsDescription1) == NormalTagsDescription2);
            Assert.IsTrue(DescriptionValidation.Validate(DuplicateTagsDescription2) == NormalTagsDescription2);
        }

        [TestMethod]
        public void ValidateConvertedNewlineTagsTests()
        {
            Assert.IsTrue(DescriptionValidation.Validate(ConvertNewLineTagDescription1) == ConvertNewLineTagDescription1Corrected);
            Assert.IsTrue(DescriptionValidation.Validate(ConvertNewLineTagDescription2) == ConvertNewLineTagDescription2Corrected);
            Assert.IsTrue(DescriptionValidation.Validate(ConvertNewLineTagDescription3) == ConvertNewLineTagDescription3Corrected);
        }

        [TestMethod]
        public void ValidateCaseTagsTests()
        {
            Assert.IsTrue(DescriptionValidation.Validate(UpperCaseTagDescription1) == UpperCaseTagDescription1Corrected);
        }

        [TestMethod]
        public void ValidateSpaceTagsTests()
        {
            Assert.IsTrue(DescriptionValidation.Validate(ExtraSpacesTagDescription1) == NormalTagsDescription2);
            Assert.IsTrue(DescriptionValidation.Validate(ExtraSpacesTagDescription2) == NormalTagsDescription2);
        }

        [TestMethod]
        public void ValidateEmptyTagsTests()
        {
            Assert.IsTrue(DescriptionValidation.Validate(EmptyTagsDescription1) == EmptyTagsDescription1Corrected);
            Assert.IsTrue(DescriptionValidation.Validate(EmptyTagsDescription2) == EmptyTagsDescription2Corrected);
            Assert.IsTrue(DescriptionValidation.Validate(EmptyTagsDescription3) == EmptyTagsDescription3Corrected);
        }

        [TestMethod]
        public void ValidateNestedTagsTests()
        {
            Assert.IsTrue(DescriptionValidation.Validate(NestedTagDescription1) == NestedTagDescription1Corrected);
            Assert.IsTrue(DescriptionValidation.Validate(NestedTagDescription2) == NestedTagDescription2Corrected);
            Assert.IsTrue(DescriptionValidation.Validate(NestedTagDescription3) == NestedTagDescription3Corrected);
            Assert.IsTrue(DescriptionValidation.Validate(NestedTagDescription4) == NestedTagDescription4Corrected);
        }

        [TestMethod]
        public void ValidateNestedNewLineTagsTests()
        {
            Assert.IsTrue(DescriptionValidation.Validate(NestedNewLineTagDescription1) == NestedNewLineTagDescription1Corrected);
            Assert.IsTrue(DescriptionValidation.Validate(NestedNewLineTagDescription2) == NestedNewLineTagDescription2Corrected);
        }

        [TestMethod]
        public void ValidateRealDescriptionTests()
        {
            Assert.IsTrue(DescriptionValidation.Validate(DiabloBlackSoulstone) == DiabloBlackSoulstoneCorrected);
            Assert.IsTrue(DescriptionValidation.Validate(DVaMechSelfDestruct) == DVaMechSelfDestructCorrected);
            Assert.IsTrue(DescriptionValidation.Validate(ValeeraCheapShot) == ValeeraCheapShotCorrected);
            Assert.IsTrue(DescriptionValidation.Validate(CrusaderPunish) == CrusaderPunishSame);
        }

        [TestMethod]
        public void ValidatePlainTextTests()
        {
            Assert.IsTrue(DescriptionValidation.GetPlainText(NestedTagDescription1, false, false) == PlainText1);
            Assert.IsTrue(DescriptionValidation.GetPlainText(NestedNewLineTagDescription2Corrected, false, false) == PlainText2);
            Assert.IsTrue(DescriptionValidation.GetPlainText(ValeeraCheapShotCorrected, false, false) == PlainText3);
            Assert.IsTrue(DescriptionValidation.GetPlainText(PlainText4, false, false) == PlainText4Corrected);
            Assert.IsTrue(DescriptionValidation.GetPlainText(PlainText5, false, false) == PlainText5Corrected);
        }

        [TestMethod]
        public void ValidatePlainTextNewlineTests()
        {
            Assert.IsTrue(DescriptionValidation.GetPlainText(NestedNewLineTagDescription1Corrected, true, false) == PlainTextNewline1);
            Assert.IsTrue(DescriptionValidation.GetPlainText(ValeeraCheapShotCorrected, true, false) == PlainTextNewline2);
            Assert.IsTrue(DescriptionValidation.GetPlainText(PlainTextNewline3, true, false) == PlainTextNewline3Corrected);
        }

        [TestMethod]
        public void ValidatePlainTextScalingTests()
        {
            Assert.IsTrue(DescriptionValidation.GetPlainText(PlainTextScaling1, false, true) == PlainTextScaling1Corrected);
            Assert.IsTrue(DescriptionValidation.GetPlainText(PlainTextScaling2, false, true) == PlainTextScaling2Corrected);
        }

        [TestMethod]
        public void ValidatePlainTextScalingNewlineTests()
        {
            Assert.IsTrue(DescriptionValidation.GetPlainText(PlainTextScalingNewline1, true, true) == PlainTextScalingNewline1Corrected);
            Assert.IsTrue(DescriptionValidation.GetPlainText(PlainTextScalingNewline2, true, true) == PlainTextScalingNewline2Corrected);
        }

        [TestMethod]
        public void ValidateColoredTextTests()
        {
            Assert.IsTrue(DescriptionValidation.GetColoredText(ColoredText1, false) == ColoredText1Corrected);
            Assert.IsTrue(DescriptionValidation.GetColoredText(ColoredText2, false) == ColoredText2Corrected);
        }

        [TestMethod]
        public void ValidateColoredTextScalingTests()
        {
            Assert.IsTrue(DescriptionValidation.GetColoredText(ColoredTextScaling1, true) == ColoredTextScaling1Corrected);
            Assert.IsTrue(DescriptionValidation.GetColoredText(ColoredTextScaling2, true) == ColoredTextScaling2Corrected);
        }
    }
}
