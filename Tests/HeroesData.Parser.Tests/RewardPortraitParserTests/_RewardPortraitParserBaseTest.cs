using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.RewardPortraitParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class RewardPortraitParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public RewardPortraitParserBaseTest()
        {
            Parse();
        }

        protected RewardPortrait WhitemaneCarbotsPortrait { get; set; }
        protected RewardPortrait WhitemaneSpooky18ToonPortrait { get; set; }
        protected RewardPortrait WhitemaneBasePortrait { get; set; }
        protected RewardPortrait WhitemaneMasteryPortrait { get; set; }
        protected RewardPortrait StitchesPortraitSummer { get; set; }
        protected RewardPortrait TespaMembership2015Portrait { get; set; }
        protected RewardPortrait Season4TL2018GrandMaster { get; set; }
        protected RewardPortrait HeroesAvatar256x256Qhira { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            PortraitPackParser portraitParser = new PortraitPackParser(XmlDataService);
            Assert.IsTrue(portraitParser.Items.Count > 0);
        }

        private void Parse()
        {
            RewardPortraitParser rewardPortraitParser = new RewardPortraitParser(XmlDataService);
            WhitemaneCarbotsPortrait = rewardPortraitParser.Parse("WhitemaneCarbotsPortrait");
            WhitemaneSpooky18ToonPortrait = rewardPortraitParser.Parse("WhitemaneSpooky18ToonPortrait");
            WhitemaneBasePortrait = rewardPortraitParser.Parse("WhitemaneBasePortrait");
            WhitemaneMasteryPortrait = rewardPortraitParser.Parse("WhitemaneMasteryPortrait");
            StitchesPortraitSummer = rewardPortraitParser.Parse("StitchesPortraitSummer");
            TespaMembership2015Portrait = rewardPortraitParser.Parse("2015TespaMembershipPortrait");
            Season4TL2018GrandMaster = rewardPortraitParser.Parse("2018Season4TLGrandMaster");
            HeroesAvatar256x256Qhira = rewardPortraitParser.Parse("HeroesAvatar256x256Qhira");
        }
    }
}
