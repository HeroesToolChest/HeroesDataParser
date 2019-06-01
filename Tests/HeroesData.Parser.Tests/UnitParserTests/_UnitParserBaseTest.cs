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

        protected Unit AbathurLocustNestPlaceholderDummy { get; set; }
        protected Unit DVaMechPlacementDummy { get; set; }
        protected Unit AlexstraszaLifeblossomGiftOfLife { get; set; }
        protected Unit ZagaraHydralisk { get; set; }
        protected Unit TownCannonTowerL2 { get; set; }
        protected Unit VolskayaVehicle { get; set; }
        protected Unit BraxisHoldoutTerranArchangelLaner { get; set; }
        protected Unit AzmodanDemonLieutenant { get; set; }
        protected Unit HanamuraMercDefenderSentinel { get; set; }
        protected Unit MercDefenderSentinel { get; set; }
        protected Unit OverwatchDataMercDefenderMeleeBruiser { get; set; }
        protected Unit VolskayaDataVolskayaVehicleGunner { get; set; }
        protected Unit AlteracpassAllianceCavalry { get; set; }
        protected Unit AlteracpassCapturedSoldier { get; set; }
        protected Unit AlteracpassAlteracCoreBossParent { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            UnitParser unitParser = new UnitParser(XmlDataService, UnitOverrideLoader);
            Assert.IsTrue(unitParser.Items.Count > 0);
        }

        private void LoadTestData()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(GameData, OverrideFileNameSuffix);
            UnitOverrideLoader = (UnitOverrideLoader)xmlDataOverriders.GetOverrider(typeof(UnitParser));
        }

        private void Parse()
        {
            UnitParser unitParser = new UnitParser(XmlDataService, UnitOverrideLoader);
            AbathurLocustNestPlaceholderDummy = unitParser.Parse("AbathurLocustNestPlaceholderDummy");
            DVaMechPlacementDummy = unitParser.Parse("DVaMechPlacementDummy");
            AlexstraszaLifeblossomGiftOfLife = unitParser.Parse("AlexstraszaLifeblossomGiftOfLife");
            AzmodanDemonLieutenant = unitParser.Parse("AzmodanDemonLieutenant");
            ZagaraHydralisk = unitParser.Parse("ZagaraHydralisk");
            TownCannonTowerL2 = unitParser.Parse("TownCannonTowerL2");
            VolskayaVehicle = unitParser.Parse("VolskayaVehicle");
            MercDefenderSentinel = unitParser.Parse("MercDefenderSentinel");

            ParseBraxisHoldoutData(unitParser);
            ParseAlteracPassData(unitParser);
            ParseHanamura(unitParser);
            ParseOverwatchData(unitParser);
            ParseVolskayaData(unitParser);
        }

        private void ParseAlteracPassData(UnitParser unitParser)
        {
            GameData.AppendGameData(GameData.GetMapGameData("alteracpass.stormmod"));
            AlteracpassCapturedSoldier = unitParser.Parse("CapturedSoldier", "alteracpass.stormmod");
            AlteracpassAllianceCavalry = unitParser.Parse("AllianceCavalry", "alteracpass.stormmod");
            AlteracpassAlteracCoreBossParent = unitParser.Parse("AlteracCoreBossParent", "alteracpass.stormmod");
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

        private void ParseVolskayaData(UnitParser unitParser)
        {
            GameData.AppendGameData(GameData.GetMapGameData("volskayadata.stormmod"));
            VolskayaDataVolskayaVehicleGunner = unitParser.Parse("VolskayaVehicleGunner", "volskayadata.stormmod");
            GameData.RestoreGameData();
        }
    }
}
