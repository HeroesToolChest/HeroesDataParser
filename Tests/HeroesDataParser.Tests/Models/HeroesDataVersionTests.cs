namespace Heroes.Element.Models.Tests;

[TestClass]
public class HeroesDataVersionTests
{
    [TestMethod]
    [DataRow(0, 0, 0, 00000)]
    [DataRow(-1, -1, -1, -1, false)]
    [DataRow(-567, -324, -23, -234234, false)]
    public void Ctor_NegativeValues_ReturnsZeroVersion(int major, int minor, int revision, int build, bool isPtr = false)
    {
        // arrange
        HeroesDataVersion expected = new(0, 0, 0, 0, isPtr);

        // act
        HeroesDataVersion heroesVersion = new(major, minor, revision, build, isPtr);

        // assert
        heroesVersion.Should().Be(expected);
    }

    [TestMethod]
    [DataRow(1, 1, 1, 11111)]
    [DataRow(1, 1, 1, 11111, false)]
    public void Equals_SameValues_ReturnsTrue(int major, int minor, int revision, int build, bool isPtr = false)
    {
        // arrange
        HeroesDataVersion expected = new(major, minor, revision, build, isPtr);

        // act
        HeroesDataVersion item1 = new(major, minor, revision, build, isPtr);

        // assert
        item1.Should().Be(expected);
    }

    [TestMethod]
    public void EqualsMethod_InvalidComparision_ReturnsFalse()
    {
        // arrange
        HeroesDataVersion version = new(2, 45, 124, 154787, false);

        // act
        bool result1 = version.Equals((int?)null);
        bool result2 = version.Equals(5);

        // assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [TestMethod]
    public void EqualsMethod_ValidComparison_ReturnsTrue()
    {
        // arrange
        HeroesDataVersion version = new(2, 45, 124, 154787, false);

        // act
        bool result = version.Equals(obj: version);

        // assert
        result.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, true, 1)]
    [DataRow(-1, 1, 1, 11111, false, -1)]
    [DataRow(1, 1, 1, 1, false, 0)]
    [DataRow(1, 1, 1, 1, true, -1)]
    public void CompareTo_VariousComparisons_EvaluatedCorrectly(int major, int minor, int revision, int build, bool isPtr, int result)
    {
        // arrange
        HeroesDataVersion compareVersion = new(major, minor, revision, build, isPtr);
        HeroesDataVersion baseVersion = new(1, 1, 1, 1, false);

        // act
        int compare = compareVersion.CompareTo(baseVersion);

        // assert
        compare.Should().Be(result);
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111)]
    [DataRow(-1, 1, 1, 11111, false)]
    [DataRow(-1, 1, 1, 20, true)]
    [DataRow(-1, 2, 1, 11111, false)]
    [DataRow(1, 1, -1, 11111, false)]
    [DataRow(1, 1, 1, 11111, false)]
    public void NotEquals_VariousComparisons_ReturnsNotEquals(int major, int minor, int revision, int build, bool isPtr = false)
    {
        // Arrange
        HeroesDataVersion baseline = new(1, 1, 1, 11111, true);
        HeroesDataVersion other = new(major, minor, revision, build, isPtr);

        // Act
        bool areEqual = baseline.Equals(other);

        // Assert
        areEqual.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 2, 11111, false)]
    public void OperatorEqual_VariousComparisons_EvaluatedCorrectly(int major, int minor, int revision, int build, bool isPtr, int major2, int minor2, int revision2, int build2, bool isPtr2)
    {
        // Arrange
        HeroesDataVersion left = new(major, minor, revision, build, isPtr);
        HeroesDataVersion right = new(major2, minor2, revision2, build2, isPtr2);
        HeroesDataVersion? nullVersion = null;

        // Act
        bool leftEqRight = left == right;
        bool nullEqRight = nullVersion == right;
        bool rightEqNull = right == nullVersion;
        bool nullEqNull = nullVersion == null;

        // Assert
        nullEqRight.Should().BeFalse();
        rightEqNull.Should().BeFalse();
        nullEqNull.Should().BeTrue();
        leftEqRight.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, false, 3, 2, 2, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 3, 2, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 8, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 2, 12112, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 2, 11111, true)]
    [DataRow(2, 2, 2, 11111, false, 2, 4, 78, 45781, false)]
    public void OperatorNotEqual_VariousComparisons_EvaluatedCorrectly(int major, int minor, int revision, int build, bool isPtr, int major2, int minor2, int revision2, int build2, bool isPtr2)
    {
        // Arrange
        HeroesDataVersion left = new(major, minor, revision, build, isPtr);
        HeroesDataVersion right = new(major2, minor2, revision2, build2, isPtr2);
        HeroesDataVersion? nullVersion = null;

        // Act
        bool nullNeRight = nullVersion != right;
        bool rightNeNull = right != nullVersion;
        bool nullNeNull = nullVersion != null;
        bool leftNeRight = left != right;

        // Assert
        nullNeRight.Should().BeTrue();
        rightNeNull.Should().BeTrue();
        nullNeNull.Should().BeFalse();
        leftNeRight.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 2, 2, 11111, false, 2, 2, 2, 11111, false)]
    [DataRow(2, 1, 2, 11111, false, 2, 2, 2, 11111, false)]
    [DataRow(2, 2, 0, 11111, false, 2, 2, 2, 11111, false)]
    [DataRow(2, 2, 2, 10000, false, 2, 2, 2, 11111, false)]
    [DataRow(2, 2, 2, 10000, true, 2, 2, 2, 11111, false)]
    [DataRow(2, 2, 2, 10000, true, 2, 2, 2, 10000, false)]
    public void OperatorLessThan_VariousComparisons_EvaluatedCorrectly(int major, int minor, int revision, int build, bool isPtr, int major2, int minor2, int revision2, int build2, bool isPtr2)
    {
        // Arrange
        HeroesDataVersion left = new(major, minor, revision, build, isPtr);
        HeroesDataVersion right = new(major2, minor2, revision2, build2, isPtr2);
        HeroesDataVersion? nullVersion = null;

        // Act
        bool nullLtRight = nullVersion < right;
        bool rightLtNull = right < nullVersion;
#pragma warning disable CS1718
        bool leftLtLeft = left < left;
#pragma warning restore CS1718
        bool rightLtLeft = right < left;
        bool leftLtRight = left < right;
        bool nullLtNull = nullVersion < null;

        // Assert
        nullLtRight.Should().BeTrue();
        rightLtNull.Should().BeFalse();
        nullLtNull.Should().BeFalse();
        leftLtLeft.Should().BeFalse();
        rightLtLeft.Should().BeFalse();
        leftLtRight.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 2, 2, 11111, false, 2, 2, 2, 11111, false)]
    [DataRow(2, 1, 2, 11111, false, 2, 2, 2, 11111, false)]
    [DataRow(2, 2, 0, 11111, false, 2, 2, 2, 11111, false)]
    [DataRow(2, 2, 2, 10000, false, 2, 2, 2, 11111, false)]
    [DataRow(2, 2, 2, 10000, true, 2, 2, 2, 11111, false)]
    [DataRow(2, 2, 2, 10000, true, 2, 2, 2, 10000, true)]
    public void OperatorLessThanOrEqual_VariousComparisons_EvaluatedCorrectly(int major, int minor, int revision, int build, bool isPtr, int major2, int minor2, int revision2, int build2, bool isPtr2)
    {
        // Arrange
        HeroesDataVersion left = new(major, minor, revision, build, isPtr);
        HeroesDataVersion right = new(major2, minor2, revision2, build2, isPtr2);
        HeroesDataVersion? nullVersion = null;

        // Act
        bool nullLeRight = nullVersion <= right;
        bool rightLeNull = right <= nullVersion;
        bool nullLeNull = nullVersion <= null;
#pragma warning disable CS1718
        bool leftLeLeft = left <= left;
#pragma warning restore CS1718
        bool leftLeRight = left <= right;

        // Assert
        nullLeRight.Should().BeTrue();
        rightLeNull.Should().BeFalse();
        nullLeNull.Should().BeTrue();
        leftLeLeft.Should().BeTrue();
        leftLeRight.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, false, 1, 2, 2, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 1, 2, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 0, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 2, 10000, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 2, 11111, true)]
    public void OperatorGreaterThan_VariousComparisons_EvaluatedCorrectly(int major, int minor, int revision, int build, bool isPtr, int major2, int minor2, int revision2, int build2, bool isPtr2)
    {
        // arrange
        HeroesDataVersion left = new(major, minor, revision, build, isPtr);
        HeroesDataVersion right = new(major2, minor2, revision2, build2, isPtr2);

        // act
#pragma warning disable SA1131 // Use readable conditions
        bool nullGtRight = null! > right;
#pragma warning restore SA1131 // Use readable conditions
        bool rightGtNull = right > null!;
        bool nullGtNull = null! > (HeroesDataVersion)null!;
#pragma warning disable CS1718 // Comparison made to same variable
        bool leftGtLeft = left > left;
#pragma warning restore CS1718 // Comparison made to same variable
        bool rightGtLeft = right > left;
        bool leftGtRight = left > right;

        // assert
        nullGtRight.Should().BeFalse();
        rightGtNull.Should().BeTrue();
        nullGtNull.Should().BeFalse();
        leftGtLeft.Should().BeFalse();
        rightGtLeft.Should().BeFalse();
        leftGtRight.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, false, 1, 2, 2, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 1, 2, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 0, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 2, 10000, false)]
    [DataRow(2, 2, 2, 11111, true, 2, 2, 2, 10000, false)]
    [DataRow(2, 2, 2, 10000, true, 2, 2, 2, 10000, true)]
    public void OperatorGreaterThanOrEqual_NullAndVersionComparisons_EvaluatedCorrectly(int major, int minor, int revision, int build, bool isPtr, int major2, int minor2, int revision2, int build2, bool isPtr2)
    {
        // arrange
        HeroesDataVersion a = new(major, minor, revision, build, isPtr);
        HeroesDataVersion b = new(major2, minor2, revision2, build2, isPtr2);
        HeroesDataVersion? nullValue = null;

        // act
        bool nullGeB = nullValue! >= b;
        bool bGeNull = b >= nullValue!;
#pragma warning disable CS1718 // Comparison made to same variable
        bool nullGeNull = nullValue! >= nullValue!;
        bool aGeA = a >= a;
#pragma warning restore CS1718 // Comparison made to same variable
        bool aGeB = a >= b;

        // assert
        nullGeB.Should().BeFalse("null should not be >= non-null");
        bGeNull.Should().BeTrue("non-null should be >= null");
        nullGeNull.Should().BeTrue("null should be >= null");
        aGeA.Should().BeTrue("instance should be >= itself");
        aGeB.Should().BeTrue("left version should be >= right version for provided data set");
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 2, 11111, false)]
    public void GetHashCode_SameValues_ReturnsEqual(int major, int minor, int revision, int build, bool isPtr, int major2, int minor2, int revision2, int build2, bool isPtr2)
    {
        // arrange
        int expectedHashCode = new HeroesDataVersion(major, minor, revision, build, isPtr).GetHashCode();

        // act
        int actualHashCode = new HeroesDataVersion(major2, minor2, revision2, build2, isPtr2).GetHashCode();

        // assert
        actualHashCode.Should().Be(expectedHashCode);
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, false, 0, 2, 2, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 74, 2, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 247, 11111, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 2, 12457, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 2, 11112, false)]
    [DataRow(2, 2, 2, 11111, false, 2, 2, 2, 11111, true)]
    [DataRow(2, 2, 2, 11111, true, 2, 2, 2, 11111, false)]
    public void GetHashCode_DifferentValues_ReturnsNotEqual(int major, int minor, int revision, int build, bool isPtr, int major2, int minor2, int revision2, int build2, bool isPtr2)
    {
        // arrange
        int expectedHashCode = new HeroesDataVersion(major, minor, revision, build, isPtr).GetHashCode();

        // act
        int actualHashCode = new HeroesDataVersion(major2, minor2, revision2, build2, isPtr2).GetHashCode();

        // assert
        actualHashCode.Should().NotBe(expectedHashCode);
    }

    [TestMethod]
    [DataRow("2.34.3.34567", 2, 34, 3, 34567, false)]
    [DataRow("0.0.0.0", 0, 0, 0, 0, false)]
    [DataRow("-12.-45.-458.-45787", 0, 0, 0, 0, false)]
    [DataRow("-12.-45.-458.97", 0, 0, 0, 97, false)]
    [DataRow("2.34.3.34567_ptr", 2, 34, 3, 34567, true)]
    [DataRow("2.34.3.34567_PTr", 2, 34, 3, 34567, true)]
    public void TryParse_WithValidFormat_ReturnsParsedValues(string value, int major, int minor, int revision, int build, bool isPtr)
    {
        // arrange
        string input = value;
        HeroesDataVersion expected = new(major, minor, revision, build, isPtr);

        // act
        bool returnResult = HeroesDataVersion.TryParse(input, out HeroesDataVersion? result);
        bool returnResultSpan = HeroesDataVersion.TryParse(input.AsSpan(), out HeroesDataVersion? resultSpan);

        // assert
        returnResult.Should().BeTrue();
        returnResultSpan.Should().BeTrue();

        result.Should().Be(expected);
        resultSpan.Should().Be(expected);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow(null)]
    [DataRow("2.34.3.34567a")]
    [DataRow("k2.34.3.34567")]
    [DataRow("2.34k.3.34567")]
    [DataRow("2.34.3&.34567")]
    [DataRow("2.34.3.34567.4")]
    [DataRow("2.34.3")]
    [DataRow("2.34.3.34567_uu")]
    [DataRow("2.34.3.34567_ptr_ptr")]
    [DataRow("2.34.3.34567_dfg")]
    [DataRow("2.34.3.345679a_ptr")]
    public void TryParse_WithInvalidFormat_ReturnsFalse(string? value)
    {
        // arrange
        string? input = value;

        // act
        bool result = HeroesDataVersion.TryParse(input, out HeroesDataVersion? _);
        bool resultSpan = HeroesDataVersion.TryParse(input.AsSpan(), out HeroesDataVersion? _);

        // assert
        result.Should().BeFalse();
        resultSpan.Should().BeFalse();
    }

    [TestMethod]
    [DataRow("2.34.3.34567", 2, 34, 3, 34567, false)]
    [DataRow("0.0.0.0", 0, 0, 0, 0, false)]
    [DataRow("-12.-45.-458.-45787", 0, 0, 0, 0, false)]
    [DataRow("-12.-45.-458.97", 0, 0, 0, 97, false)]
    [DataRow("2.34.3.34567_ptr", 2, 34, 3, 34567, true)]
    [DataRow("2.34.3.34567_PTr", 2, 34, 3, 34567, true)]
    public void Parse_WithValidFormat_IsParsed(string value, int major, int minor, int revision, int build, bool isPtr)
    {
        // Arrange
        string input = value;
        HeroesDataVersion expected = new(major, minor, revision, build, isPtr);

        // Act
        HeroesDataVersion actualFromString = HeroesDataVersion.Parse(input);
        HeroesDataVersion actualFromSpan = HeroesDataVersion.Parse(input.AsSpan());

        // Assert
        actualFromString.Should().Be(expected);
        actualFromSpan.Should().Be(expected);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("2.34.3.34567a")]
    [DataRow("k2.34.3.34567")]
    [DataRow("2.34k.3.34567")]
    [DataRow("2.34.3&.34567")]
    [DataRow("2.34.3.34567.4")]
    [DataRow("2.34.3")]
    [DataRow("2.34.3.34567_uu")]
    [DataRow("2.34.3.34567_ptr_ptr")]
    [DataRow("2.34.3.34567_dfg")]
    [DataRow("2.34.3.345679a_ptr")]
    public void Parse_WithInvalidFormat_ThrowsException(string value)
    {
        // arrange
        string input = value;

        // act
        Action act = () => HeroesDataVersion.Parse(input);
        Action actSpan = () => HeroesDataVersion.Parse(value.AsSpan());

        // assert
        act.Should().ThrowExactly<FormatException>();
        actSpan.Should().ThrowExactly<FormatException>();
    }

    [TestMethod]
    public void Parse_WithInvalidFormat_ThrowsException()
    {
        // arrange
        string? input = null;

        // act
        Action act = () => HeroesDataVersion.Parse(input!);

        // assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }
}