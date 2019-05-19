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
        protected Unit AlteracpassAllianceCavalry { get; set; }
        protected Unit BraxisHoldoutTerranArchangelLaner { get; set; }
        protected Unit AzmodanDemonLieutenant { get; set; }
        protected Unit HanamuraMercDefenderSentinel { get; set; }
        protected Unit MercDefenderSentinel { get; set; }
        protected Unit OverwatchDataMercDefenderMeleeBruiser { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            UnitParser unitParser = new UnitParser(XmlDataType, UnitOverrideLoader);
            Assert.IsTrue(unitParser.Items.Count > 0);
        }

        private void LoadTestData()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(GameData, OverrideFileNameSuffix);
            UnitOverrideLoader = (UnitOverrideLoader)xmlDataOverriders.GetOverrider(typeof(UnitParser));
        }

        private void Parse()
        {
            UnitParser unitParser = new UnitParser(XmlDataType, UnitOverrideLoader);
            AzmodanDemonLieutenant = unitParser.Parse("AzmodanDemonLieutenant");
            ZagaraHydralisk = unitParser.Parse("ZagaraHydralisk");
            TownCannonTowerL2 = unitParser.Parse("TownCannonTowerL2");
            VolskayaVehicle = unitParser.Parse("VolskayaVehicle");
            MercDefenderSentinel = unitParser.Parse("MercDefenderSentinel");

            ParseBraxisHoldoutData(unitParser);
            ParseAlteracPassData(unitParser);
            ParseHanamura(unitParser);
            ParseOverwatchData(unitParser);
        }

        private void ParseAlteracPassData(UnitParser unitParser)
        {
            GameData.AppendGameData(GameData.GetMapGameData("alteracpass.stormmod"));
            AlteracpassAllianceCavalry = unitParser.Parse("AllianceCavalry", "alteracpass.stormmod");
            GameData.RestoreGameData();
        }

        private void ParseBraxisHoldoutData(UnitParser unitParser)
        {
            GameData.AppendGameData(GameData.GetMapGameData("braxisholdoutdata.stormmod"));
            BraxisHoldoutTerranArchangelLaner = unitParser.Parse("TerranArchangelLaner", "braxisholdoutdata.stormmod");
            GameData.RestoreGameData();
        }

        private void ParseHanamura(UnitParser unitParser)
        {
            GameData.AppendGameData(GameData.GetMapGameData("hanamura.stormmod"));
            HanamuraMercDefenderSentinel = unitParser.Parse("MercDefenderSentinel", "hanamura.stormmod");
            GameData.RestoreGameData();
        }

        private void ParseOverwatchData(UnitParser unitParser)
        {
            GameData.AppendGameData(GameData.GetMapGameData("overwatchdata.stormmod"));
            OverwatchDataMercDefenderMeleeBruiser = unitParser.Parse("MercDefenderMeleeBruiser", "overwatchdata.stormmod");
            GameData.RestoreGameData();
        }
    }
}
