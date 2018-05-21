using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests
{
    [TestClass]
    public class MathEvalTests
    {
        [TestMethod]
        public void CalculatePathEquationTest()
        {
            Assert.IsTrue(MathEval.CalculatePathEquation("(12 + 6.000000) * (0.1875 + 0.062500) - (12 * 0.1875) / (12 * 0.1875) * 100") == 100);
            Assert.IsTrue(MathEval.CalculatePathEquation("17 / 34 * 100") == 50);
            Assert.IsTrue(MathEval.CalculatePathEquation("(57.8 / 34) * 100 - 100") == 70);
            Assert.IsTrue(MathEval.CalculatePathEquation("-100*(1-1.400000)") == 40);
            Assert.IsTrue(MathEval.CalculatePathEquation("--100") == 100);
            Assert.IsTrue(MathEval.CalculatePathEquation("-100*-0.15") == 15);
            Assert.IsTrue(MathEval.CalculatePathEquation("-100 * (0.225/-0.15)") == 150);
            Assert.IsTrue(MathEval.CalculatePathEquation("(1+(-0.6)*100)") == 40);
            Assert.IsTrue(MathEval.CalculatePathEquation("-(-0.6--0.3)*100") == 30);
            Assert.IsTrue(MathEval.CalculatePathEquation("- (-0.7*100)") == 70);
            Assert.IsTrue(MathEval.CalculatePathEquation("-0.5") == -0.5);
            Assert.IsTrue(MathEval.CalculatePathEquation("0") == 0);
        }
    }
}
