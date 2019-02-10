using HeroesData.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests
{
    [TestClass]
    public class HeroesMathEvalTests
    {
        [TestMethod]
        public void CalculatePathEquationTest()
        {
            Assert.AreEqual(100, HeroesMathEval.CalculatePathEquation("(12 + 6.000000) * (0.1875 + 0.062500) - (12 * 0.1875) / (12 * 0.1875) * 100"));
            Assert.AreEqual(50, HeroesMathEval.CalculatePathEquation("17 / 34 * 100"));
            Assert.AreEqual(70, HeroesMathEval.CalculatePathEquation("(57.8 / 34) * 100 - 100"));
            Assert.AreEqual(40, HeroesMathEval.CalculatePathEquation("-100*(1-1.400000)"));
            Assert.AreEqual(100, HeroesMathEval.CalculatePathEquation("--100"));
            Assert.AreEqual(15, HeroesMathEval.CalculatePathEquation("-100*-0.15"));
            Assert.AreEqual(150, HeroesMathEval.CalculatePathEquation("-100 * (0.225/-0.15)"));
            Assert.AreEqual(40, HeroesMathEval.CalculatePathEquation("(1+(-0.6)*100)"));
            Assert.AreEqual(30, HeroesMathEval.CalculatePathEquation("-(-0.6--0.3)*100"));
            Assert.AreEqual(70, HeroesMathEval.CalculatePathEquation("- (-0.7*100)"));
            Assert.AreEqual(-0.5, HeroesMathEval.CalculatePathEquation("-0.5"));
            Assert.AreEqual(0, HeroesMathEval.CalculatePathEquation("0"));
            Assert.AreEqual(100, HeroesMathEval.CalculatePathEquation("1+0*100"));
            Assert.AreEqual(100, HeroesMathEval.CalculatePathEquation("(1+0*100)"));
            Assert.AreEqual(60, HeroesMathEval.CalculatePathEquation("((5) + (3) / 5 - 1) * 100"));
            Assert.AreEqual(5, HeroesMathEval.CalculatePathEquation("(30/20)-1*10)")); // missing a (left) parenthesis
        }
    }
}
