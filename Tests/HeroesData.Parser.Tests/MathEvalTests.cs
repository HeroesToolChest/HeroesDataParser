using Xunit;

namespace HeroesData.Parser.Tests
{
    public class MathEvalTests
    {
        [Fact]
        public void CalculatePathEquationTest()
        {
            Assert.Equal(100, MathEval.CalculatePathEquation("(12 + 6.000000) * (0.1875 + 0.062500) - (12 * 0.1875) / (12 * 0.1875) * 100"));
            Assert.Equal(50, MathEval.CalculatePathEquation("17 / 34 * 100"));
            Assert.Equal(70, MathEval.CalculatePathEquation("(57.8 / 34) * 100 - 100"));
            Assert.Equal(40, MathEval.CalculatePathEquation("-100*(1-1.400000)"));
            Assert.Equal(100, MathEval.CalculatePathEquation("--100"));
            Assert.Equal(15, MathEval.CalculatePathEquation("-100*-0.15"));
            Assert.Equal(150, MathEval.CalculatePathEquation("-100 * (0.225/-0.15)"));
            Assert.Equal(40, MathEval.CalculatePathEquation("(1+(-0.6)*100)"));
            Assert.Equal(30, MathEval.CalculatePathEquation("-(-0.6--0.3)*100"));
            Assert.Equal(70, MathEval.CalculatePathEquation("- (-0.7*100)"));
            Assert.Equal(-0.5, MathEval.CalculatePathEquation("-0.5"));
            Assert.Equal(0, MathEval.CalculatePathEquation("0"));
            Assert.Equal(100, MathEval.CalculatePathEquation("1+0*100"));
            Assert.Equal(100, MathEval.CalculatePathEquation("(1+0*100)"));
            Assert.Equal(60, MathEval.CalculatePathEquation("((5) + (3) / 5 - 1) * 100"));
            Assert.Equal(5, MathEval.CalculatePathEquation("(30/20)-1*10)")); // missing a (left) parenthesis
        }
    }
}
