using Heroes.Models;
using HeroesData.Parser.Overrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class UnitParserBaseTest : ParserBase
    {
        private readonly string OverrideFileNameSuffix = "overrides-dataparsertest.xml";

        private UnitOverrideLoader UnitOverrideLoader;

        public UnitParserBaseTest()
        {
            LoadTestData();
            Parse();
        }

        protected Unit ZagaraHydralisk { get; set; }
        protected Unit TownCannonTowerL2 { get; set; }
        protected Unit VolskayaVehicle { get; set; }
        protected Unit AllianceCavalry { get; set; }
        protected Unit TerranArchangelLaner { get; set; }
        protected Unit AzmodanDemonLieutenant { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            UnitParser unitParser = new UnitParser(Configuration, GameData, DefaultData, UnitOverrideLoader);
            Assert.IsTrue(unitParser.Items.Count > 0);
        }

        private void LoadTestData()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(GameData, OverrideFileNameSuffix);
            UnitOverrideLoader = (UnitOverrideLoader)xmlDataOverriders.GetOverrider(typeof(UnitParser));
        }

        private void Parse()
        {
            UnitParser unitParser = new UnitParser(Configuration, GameData, DefaultData, UnitOverrideLoader);
            AzmodanDemonLieutenant = unitParser.Parse("AzmodanDemonLieutenant");
            ZagaraHydralisk = unitParser.Parse("ZagaraHydralisk");
            TownCannonTowerL2 = unitParser.Parse("TownCannonTowerL2");
            VolskayaVehicle = unitParser.Parse("VolskayaVehicle");

            ParseBraxisHoldoutData(unitParser);
            ParseAlteracPassData(unitParser);
        }

        private void ParseAlteracPassData(UnitParser unitParser)
        {
            GameData.AppendGameData(GameData.GetMapGameData("alteracpass.stormmod"));
            AllianceCavalry = unitParser.Parse("AllianceCavalry", "alteracpass.stormmod");
            GameData.RestoreGameData();
        }

        private void ParseBraxisHoldoutData(UnitParser unitParser)
        {
            GameData.AppendGameData(GameData.GetMapGameData("braxisholdoutdata.stormmod"));
            TerranArchangelLaner = unitParser.Parse("TerranArchangelLaner", "braxisholdoutdata.stormmod");
            GameData.RestoreGameData();
        }
    }
}
