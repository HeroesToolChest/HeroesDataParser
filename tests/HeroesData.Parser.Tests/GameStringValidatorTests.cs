using HeroesData.Parser.GameStrings;
using Xunit;

namespace HeroesData.Parser.Tests
{
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
        private readonly string ExtraTagDescription6 = "<li/>previous location.";
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

        [Fact]
        public void ValidateTests()
        {
            Assert.Equal(NoTagsDescription, GameStringValidator.Validate(NoTagsDescription)); // no changes
            Assert.Equal(NormalTagsDescription1, GameStringValidator.Validate(NormalTagsDescription1)); // no changes
            Assert.Equal(NormalTagsDescription2, GameStringValidator.Validate(NormalTagsDescription2)); // no changes
            Assert.Equal(NoTagsDescription, GameStringValidator.Validate(ExtraTagDescription1));
            Assert.Equal(NoTagsDescription, GameStringValidator.Validate(ExtraTagDescription2));
            Assert.Equal(NoTagsDescription, GameStringValidator.Validate(ExtraTagDescription3));
            Assert.Equal(NoTagsDescription, GameStringValidator.Validate(ExtraTagDescription4));
            Assert.Equal(NoTagsDescription, GameStringValidator.Validate(ExtraTagDescription5));
            Assert.Equal(NoTagsDescription, GameStringValidator.Validate(ExtraTagDescription6));
            Assert.Equal(NewLineTagDescription1, GameStringValidator.Validate(NewLineTagDescription1)); // no changes
            Assert.Equal(NewLineTagDescription2, GameStringValidator.Validate(NewLineTagDescription2)); // no changes
            Assert.Equal(SelfCloseTagDescription1, GameStringValidator.Validate(SelfCloseTagDescription1)); // no changes
            Assert.Equal(SelfCloseTagDescription2, GameStringValidator.Validate(SelfCloseTagDescription2)); // no changes
            Assert.Equal(SelfCloseTagDescription3, GameStringValidator.Validate(SelfCloseTagDescription3)); // no changes
            Assert.Equal(SelfCloseTagDescription4, GameStringValidator.Validate(SelfCloseTagDescription4)); // no changes
            Assert.Equal(NormalTagsDescription2, GameStringValidator.Validate(DuplicateTagsDescription1));
            Assert.Equal(NormalTagsDescription2, GameStringValidator.Validate(DuplicateTagsDescription2));
        }

        [Fact]
        public void ValidateConvertedNewlineTagsTests()
        {
            Assert.Equal(ConvertNewLineTagDescription1Corrected, GameStringValidator.Validate(ConvertNewLineTagDescription1));
            Assert.Equal(ConvertNewLineTagDescription2Corrected, GameStringValidator.Validate(ConvertNewLineTagDescription2));
            Assert.Equal(ConvertNewLineTagDescription3Corrected, GameStringValidator.Validate(ConvertNewLineTagDescription3));
        }

        [Fact]
        public void ValidateCaseTagsTests()
        {
            Assert.Equal(UpperCaseTagDescription1Corrected, GameStringValidator.Validate(UpperCaseTagDescription1));
        }

        [Fact]
        public void ValidateSpaceTagsTests()
        {
            Assert.Equal(NormalTagsDescription2, GameStringValidator.Validate(ExtraSpacesTagDescription1));
            Assert.Equal(NormalTagsDescription2, GameStringValidator.Validate(ExtraSpacesTagDescription2));
        }

        [Fact]
        public void ValidateEmptyTagsTests()
        {
            Assert.Equal(EmptyTagsDescription1Corrected, GameStringValidator.Validate(EmptyTagsDescription1));
            Assert.Equal(EmptyTagsDescription2Corrected, GameStringValidator.Validate(EmptyTagsDescription2));
            Assert.Equal(EmptyTagsDescription3Corrected, GameStringValidator.Validate(EmptyTagsDescription3));
        }

        [Fact]
        public void ValidateNestedTagsTests()
        {
            Assert.Equal(NestedTagDescription1Corrected, GameStringValidator.Validate(NestedTagDescription1));
            Assert.Equal(NestedTagDescription2Corrected, GameStringValidator.Validate(NestedTagDescription2));
            Assert.Equal(NestedTagDescription3Corrected, GameStringValidator.Validate(NestedTagDescription3));
            Assert.Equal(NestedTagDescription4Corrected, GameStringValidator.Validate(NestedTagDescription4));
        }

        [Fact]
        public void ValidateNestedNewLineTagsTests()
        {
            Assert.Equal(NestedNewLineTagDescription1Corrected, GameStringValidator.Validate(NestedNewLineTagDescription1));
            Assert.Equal(NestedNewLineTagDescription2Corrected, GameStringValidator.Validate(NestedNewLineTagDescription2));
        }

        [Fact]
        public void ValidateRealDescriptionTests()
        {
            Assert.Equal(DiabloBlackSoulstoneCorrected, GameStringValidator.Validate(DiabloBlackSoulstone));
            Assert.Equal(DVaMechSelfDestructCorrected, GameStringValidator.Validate(DVaMechSelfDestruct));
            Assert.Equal(ValeeraCheapShotCorrected, GameStringValidator.Validate(ValeeraCheapShot));
            Assert.Equal(CrusaderPunishSame, GameStringValidator.Validate(CrusaderPunish));
        }

        [Fact]
        public void ValidatePlainTextTests()
        {
            Assert.Equal(PlainText1, GameStringValidator.GetPlainText(NestedTagDescription1, false, false));
            Assert.Equal(PlainText2, GameStringValidator.GetPlainText(NestedNewLineTagDescription2Corrected, false, false));
            Assert.Equal(PlainText3, GameStringValidator.GetPlainText(ValeeraCheapShotCorrected, false, false));
            Assert.Equal(PlainText4Corrected, GameStringValidator.GetPlainText(PlainText4, false, false));
            Assert.Equal(PlainText5Corrected, GameStringValidator.GetPlainText(PlainText5, false, false));
        }

        [Fact]
        public void ValidatePlainTextNewlineTests()
        {
            Assert.Equal(PlainTextNewline1, GameStringValidator.GetPlainText(NestedNewLineTagDescription1Corrected, true, false));
            Assert.Equal(PlainTextNewline2, GameStringValidator.GetPlainText(ValeeraCheapShotCorrected, true, false));
            Assert.Equal(PlainTextNewline3Corrected, GameStringValidator.GetPlainText(PlainTextNewline3, true, false));
        }

        [Fact]
        public void ValidatePlainTextScalingTests()
        {
            Assert.Equal(PlainTextScaling1Corrected, GameStringValidator.GetPlainText(PlainTextScaling1, false, true));
            Assert.Equal(PlainTextScaling2Corrected, GameStringValidator.GetPlainText(PlainTextScaling2, false, true));
        }

        [Fact]
        public void ValidatePlainTextScalingNewlineTests()
        {
            Assert.Equal(PlainTextScalingNewline1Corrected, GameStringValidator.GetPlainText(PlainTextScalingNewline1, true, true));
            Assert.Equal(PlainTextScalingNewline2Corrected, GameStringValidator.GetPlainText(PlainTextScalingNewline2, true, true));
        }

        [Fact]
        public void ValidateColoredTextTests()
        {
            Assert.Equal(ColoredText1Corrected, GameStringValidator.GetColoredText(ColoredText1, false));
            Assert.Equal(ColoredText2Corrected, GameStringValidator.GetColoredText(ColoredText2, false));
        }

        [Fact]
        public void ValidateColoredTextScalingTests()
        {
            Assert.Equal(ColoredTextScaling1Corrected, GameStringValidator.GetColoredText(ColoredTextScaling1, true));
            Assert.Equal(ColoredTextScaling2Corrected, GameStringValidator.GetColoredText(ColoredTextScaling2, true));
        }
    }
}
