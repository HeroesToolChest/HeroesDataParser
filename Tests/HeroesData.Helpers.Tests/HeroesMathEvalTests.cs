using HeroesData.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests
{
    [TestClass]
    public class HeroesMathEvalTests
    {
        [DataTestMethod]
        [DataRow(100, "(12 + 6.000000) * (0.1875 + 0.062500) - (12 * 0.1875) / (12 * 0.1875) * 100")]
        [DataRow(50, "17 / 34 * 100")]
        [DataRow(70, "(57.8 / 34) * 100 - 100")]
        [DataRow(40, "-100*(1-1.400000)")]
        [DataRow(-100, "--100")]
        [DataRow(15, "-100*(-0.15)")]
        [DataRow(150, "-100 * (0.225/(-0.15))")]
        [DataRow(40, "(1+(-0.6)*100)")]
        [DataRow(30, "-(-0.6-(-0.3))*100")]
        [DataRow(70, "- (-0.7*100)")]
        [DataRow(-0.5, "-0.5")]
        [DataRow(0, "0")]
        [DataRow(100, "1+0*100")]
        [DataRow(100, "(1+0*100)")]
        [DataRow(60, "((5) + (3) / 5 - 1) * 100")]
        [DataRow(5, "(30/20)-1*10)")]
        [DataRow(60, "(1-(-60))*-1")]
        [DataRow(9, "--100*(-0.09)")]
        [DataRow(10, "5*/-+5")]
        [DataRow(0, "*+/-5+5")]
        public void Test(double expected, string equation)
        {
            Assert.AreEqual(expected, HeroesMathEval.CalculatePathEquation(equation));
        }
    }
}
