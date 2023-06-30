namespace UnderstandingLinqTests.Extensions;

using FluentAssertions;
using UnderstandingLinq.Extensions;

public class StringExtensionTests
{
    [Theory]
    [InlineData("foo", "Foo")]
    [InlineData("Already Upper", "Already Upper")]
    [InlineData("1 is not a number", "1 is not a number")]
    [InlineData("", "")]
    public void FirstLetterToUpper_Should_ConvertToUpper(string input, string expected)
    {
        var result = input.FirstLetterToUpper();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("foo", "Foo")]
    public void FirstLetterToUpper_WhenCalledManually_Should_ConvertToUpper(string input, string expected)
    {
        var result = UnderstandingLinq.Extensions.StringExtensions.FirstLetterToUpper(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("foo", 1, "fOo")]
    [InlineData("Already Upper", 7, "Already Upper")]
    [InlineData("short", 12, "short")]
    [InlineData("", 1, "")]
    public void LetterAtIdxToUpper_Should_ConvertToUpper(string input, int inputIdx, string expected)
    {
        var result = input.LetterAtIndexToUpper(inputIdx);
        result.Should().Be(expected);
    }
    
}