using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.HeroData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Tests
{
    [TestClass]
    public class DescriptionParserTests
    {
        private const string TestDataFolder = "TestData";
        private readonly string XmlTestDataFile = Path.Combine(TestDataFolder, "XmlTestData.xml");
        private readonly string FullDescriptionTestDataFile = Path.Combine(TestDataFolder, "FullDescriptionTestData.txt");

        private HeroDataLoader HeroDataLoader;
        private DescriptionLoader DescriptionLoader;
        private ScalingDataLoader ScalingDataLoader;

        public DescriptionParserTests()
        {
            LoadTestData();
        }

        [TestMethod]
        public void ParseTest()
        {
            DescriptionParser parser = new DescriptionParser(HeroDataLoader, DescriptionLoader, ScalingDataLoader);
            parser.Parse();

            Assert.IsTrue(parser.FullParsedDescriptions.Count == DescriptionLoader.FullDescriptions.Count);
            Assert.IsTrue(parser.InvalidFullDescriptions.Count == 0);

            Assert.IsTrue(parser.FullParsedDescriptions["AbathurToxicNestEnvenomedNestTalent"] == "Toxic Nests deal <c val=\"#TooltipNumbers\">75%</c> more damage over <c val=\"#TooltipNumbers\">3</c> seconds.");
            Assert.IsTrue(parser.FullParsedDescriptions["AnubarakBurrowChargeEpicenterTalent"] == "Increases Burrow Charge impact area by <c val=\"#TooltipNumbers\">60%</c> and lowers the cooldown by <c val=\"#TooltipNumbers\">1.25</c> seconds for each Hero hit.");
            Assert.IsTrue(parser.FullParsedDescriptions["AnubarakHardenCarapaceShedExoskeletonTalent"] == "Harden Carapace grants <c val=\"#TooltipNumbers\">30%</c> increased Movement Speed for <c val=\"#TooltipNumbers\">3</c> seconds.");
            Assert.IsTrue(parser.FullParsedDescriptions["AnubarakNerubianArmor"] == "Every <c val=\"#TooltipNumbers\">12</c> seconds, gain <c val=\"#TooltipNumbers\">30</c> Spell Armor against the next enemy Ability and subsequent Abilities for <c val=\"#TooltipNumbers\">1.5</c> seconds, reducing the damage taken by <c val=\"#TooltipNumbers\">30%</c>.");
            Assert.IsTrue(parser.FullParsedDescriptions["AzmodanAllShallBurn"] == "Channel a death beam on an enemy, dealing <c val=\"#TooltipNumbers\">100~~0.04~~</c> damage per second. Damage increases over time, to a max of <c val=\"#TooltipNumbers\">200~~0.04~~</c> per second, and is increased by <c val=\"#TooltipNumbers\">25~~0.04~~</c> against structures. Azmodan can move at <c val=\"#TooltipNumbers\">40%</c> speed while channeling.");
            Assert.IsTrue(parser.FullParsedDescriptions["TyraelElDruinsMightHolyGroundTalent"] == "Create a ring for <c val=\"#TooltipNumbers\">3</c> seconds that blocks enemies from entering the area teleported to using El'druin's Might.");
        }

        private void LoadTestData()
        {
            HeroDataLoader = new HeroDataLoader(string.Empty)
            {
                XmlData = XDocument.Load(XmlTestDataFile),
            };

            ScalingDataLoader = new ScalingDataLoader(HeroDataLoader);
            ScalingDataLoader.Load();

            SortedDictionary<string, string> fullDescriptions = new SortedDictionary<string, string>();

            using (StreamReader reader = new StreamReader(FullDescriptionTestDataFile))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] lines = line.Split(new char[] { '=' }, 2);

                    if (lines.Length == 2 && !fullDescriptions.ContainsKey(lines[0]))
                        fullDescriptions.Add(lines[0], lines[1]);
                }
            }

            DescriptionLoader = new DescriptionLoader(string.Empty)
            {
                FullDescriptions = fullDescriptions,
            };
        }
    }
}
