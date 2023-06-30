namespace UnderstandingLinqTests;

using System.Collections.Immutable;
using FluentAssertions;
using UnderstandingLinq;

/// <summary>
/// Section 
/// Linq is "just"* Extension Methods on IEnumerable that take Func<> as parameters.
/// </summary>
/// <remarks>
/// ** Actually... linq gets more involved with IQueryable<> and Linq-to-sql but that is for another time.
/// </remarks>
public class LinqTests
{
    private static bool IsEven(int x) => x % 2 == 0;
    private static bool IsOdd(int x) => !IsEven(x);
    
    [Fact]
    public void LinqExamples()
    {
        // Use Range() to generate an enumeration of integers.
        var enumerable = Enumerable.Range(0, 10);
        
        // Use "ToList()" and ToImmutableList() to turn values into a list
        // We do this to "instantiate" the result into an actual list and avoid "multiple-enumeration warnings"
        // However, remember that a List is an enumerable.
        var values = enumerable.ToImmutableList();

        // Simple methods to condense it into a single value.
        values.First().Should().Be(0);
        values.Last().Should().Be(10);
        values.Sum().Should().Be(55);
        
        // Note the difference between Count() on IEnumerable<> and Count the property on List.
        // This is a performance warning, because Count() is O(n) and Count is O(1)
        values.Count().Should().Be(11);
        
        // Now we start manipulating the list and returning
        values.Where(x => x % 2 == 0).Should().Equal(0, 2, 4, 6, 8, 10);
        
        // Can assign the lambda to a name
        var isEven = (int x) => x % 2 == 0;
        values.Where(isEven).Should().Equal(0, 2, 4, 6, 8, 10);
        
        // Can use local methods
        values.Where(IsEven).Should().Equal(0, 2, 4, 6, 8, 10);
        
        // Most "condense methods" have an overload that filters
        // So this:
        values.Where(IsOdd).First().Should().Be(1);
        // has a shorthand of this:
        values.First(IsOdd).Should().Be(1);
    }

    [Fact]
    public void LinqAsObjectSQLish()
    {
        var people = new People();

        var names = people.Select(person => person.Name).ToList();
        names.Should().Equal("Foo", "Bar", "Baz");
            
        var bIds = people.Where(x => x.Name.Contains("B")).ToList();
        bIds.Count.Should().Be(2);

        var bNames = people
            .Where(person => person.Name.Contains("B"))
            .Select(person => person.Name)
            .Where(name => name.EndsWith("z"))
            .Select(name => name.ToUpper());
        bNames.ToList().Should().Equal("BAZ");
    }
}