namespace UnderstandingLinqTests;

using System.Collections;
using System.Collections.Immutable;
using FluentAssertions;

public class EnumerableTests
{
    // 1. IEnumerable is any "sequence" ("virtual list") of things

    // 2. Length/Count properties vs .Count() extension method
    //      Count properties are O(1) and Count() is O(n).
    //      Generally at least want to ToList()

    // 3. Returning IEnumerable in from a method

    // 4. yield return
    private IEnumerable<int> GetIntegers()
    {
        Console.WriteLine("A");
        yield return 1;
        Console.WriteLine("B");
        yield return 2;
        Console.WriteLine("C");
        yield return 3;
        Console.WriteLine("D");

        foreach (var i in Enumerable.Range(4, 2))
            yield return i;
    }

    [Fact]
    public void QuickIntroToEnumerator_AlmostNeverUseThis()
    {
        var enumerable = GetIntegers();
        using var enumerator = enumerable.GetEnumerator();
        var idx = 0;
        while (enumerator.MoveNext())
        {
            var currentValue = enumerator.Current;
            currentValue.Should().Be(idx + 1);
            idx++;
        }
    }

    [Fact]
    public void ForEach_CallsGetEnumerator()
    {
        var idx = 0;
        foreach (var value in GetIntegers())
        {
            value.Should().Be(idx + 1);
            idx++;
        }
    }

    [Fact]
    public void GetIntegersReturnsEnumerable()
    {
        var integers = GetIntegers().ToList();
        integers.Should().Equal(1, 2, 3, 4, 5);
        integers.Should().Equal(new[] {1, 2, 3, 4, 5});
    }
    

    
    // 6. Implementing IEnumerable on a class by delegating to member
    public class NumberSet : IEnumerable<int>
    {
        private readonly Random _random;
        // private readonly List<int> _numbers;

        public NumberSet(Random random)
        {
            _random = random;
            // _numbers = EnumerateUntil6IsRolled(random).ToList();
        }

        private IEnumerable<int> EnumerateUntil6IsRolled(Random random)
        {
            while (true)
            {
                var value = random.Next(1, 6+1);
                yield return value;
                if (value == 6)
                    yield break; // exit the enumeration & method
            }
        }

        public IEnumerator<int> GetEnumerator() => EnumerateUntil6IsRolled(_random).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // public int Median
        // {
        //     get
        //     {
        //         var sorted = _numbers.Order().ToList();
        //         return sorted[_numbers.Count / 2];
        //     }
        // }
    }

    [Fact]
    public void NumberSet_Median_Should_ReturnValue()
    {
        var random = new Random(12345);
        var sut = new NumberSet(random);
        sut.Count().Should().Be(18);
        // sut.Median.Should().Be(3);
    }
    
    // 6. Not evaluated until it is iterated (ToList() or foreach)
    //  Can wrap with If's without actually
    [Fact]
    public void LazyEnumeration_MakesComposingMorePowerful()
    {
        var random = new Random(12345);
        var sut = new NumberSet(random);

        // This won't enumerate
        var sortedNumbers = (random.Next(0, 2) == 1)
            ? sut.Order()
            : sut.OrderDescending();

        // Still wont enumerate.
        var filteredNumbers = sortedNumbers.Take(5);
        
        // Now this will enumerate:
        var result = filteredNumbers.ToList();
        
        // ToList() "instantiated" the enumerable.
        result.Should().Equal(6, 5, 5, 5, 5);
    }
    

    // 7. Implementing IEnumerable on a class by using yield
    public class FixedSet : IEnumerable<FixedSet.MyRecord>
    {
        public record MyRecord(Guid Id, string Name);

        public MyRecord? A { get; } = new(Guid.NewGuid(), "A");
        public MyRecord? B { get; } = new(Guid.NewGuid(), "B");
        public MyRecord? C { get; } = new(Guid.NewGuid(), "C");

        public IEnumerator<MyRecord> GetEnumerator()
        {
            if (A is not null)
                yield return A;
            if (B is not null)
                yield return B;
            if (C is not null)
                yield return C;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Fact]
    public void FixedSetNames()
    {
        var sut = new FixedSet();
        var names = sut.Select(myRec => myRec.Name).ToImmutableList();
        names.Should().Equal("A", "B", "C");
        sut.Select(myRec => myRec.Name).Should().Equal("A", "B", "C");
    }

    // 8. IEnumerable Extension methods in the framework
    // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=net-7.0#extension-methods
    
    // General Design Approaches Around IEnumerable<>
    // I) Prefer returning IEnumerable<> from methods and letting caller decide what to do with it.
    //      Caller may ToList() or ToImmutableList() or Sort().ToList() or Where().ToList().
    // II) Prefer ctor params that take in IEnumerable<>
    //      Let Ctor create & own the collection (usually via ToImmutableList())
    // III) Call ToList() or similar when you need to ensure resources aren't held open.
}