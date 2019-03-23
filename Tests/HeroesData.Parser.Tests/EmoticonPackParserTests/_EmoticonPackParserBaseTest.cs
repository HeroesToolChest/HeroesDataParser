﻿using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroesData.Parser.Tests.EmoticonPackParserTests
{
    [TestClass]
    public class EmoticonPackParserBaseTest : ParserBase
    {
        public EmoticonPackParserBaseTest()
        {
            Parse();
        }

        protected EmoticonPack JohannaEmoticonPack2 { get; set; }
        protected EmoticonPack DeputyVallaPack1 { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            EmoticonPackParser emoticonPackParser = new EmoticonPackParser(GameData, DefaultData);
            Assert.IsTrue(emoticonPackParser.Items.Count > 0);
        }

        private void Parse()
        {
            EmoticonPackParser emoticonPackParser = new EmoticonPackParser(GameData, DefaultData);
            JohannaEmoticonPack2 = emoticonPackParser.Parse("JohannaEmoticonPack2");
            DeputyVallaPack1 = emoticonPackParser.Parse("DeputyVallaPack1");
        }
    }
}
