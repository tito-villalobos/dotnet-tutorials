namespace UnderstandingLinqTests;

using System.Globalization;
using FluentAssertions;

// 1. Func<> is a type and have variables
// 2. Func<> can take arguments in addition to return type
// 3. Func<> can have implicit this pointer ("Mini interface")
// 4. Action<> is just Func for void/no return
// 5. Pred<T> is shorthand for Func<T,bool>
// 6. Func can be a parameter to a function/method
public class FuncTests
{
    // 1. Func<> is a type and have variables
    private static int GetThree()
    {
        return 3;
    }
    
    [Fact]
    public void GetThree_Should_ReturnThree()
    {
        var result = GetThree();
        result.Should().Be(3);
    }

    [Fact]
    public void GetThreeFn_Should_ReturnThree()
    {
        Func<int> getIntFn = GetThree;
        var result = getIntFn();
        result.Should().Be(3);
    }

    // 2. Func<> can take arguments in addition to return type
    private static int ParseInt(string str) => int.Parse(str, CultureInfo.InvariantCulture);
    private static int AddTogether(int x, int y) => x + y;

    [Theory]
    [InlineData("3", 3)]
    public void ParseInt_Should_ReturnStringValue(string input, int expected)
    {
        var result = ParseInt(input);
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("3", 3)]
    public void ParseInt_Should_ReturnValue(string input, int expected)
    {
        Func<string,int> addFn = ParseInt;
        var result = addFn(input);
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData(3, 3, 6)]
    public void AddTogether_Should_ReturnSum(int x, int y, int expected)
    {
        AddTogether(x, y).Should().Be(expected);
        
        Func<int, int, int> addFn = AddTogether;
        addFn(x, y).Should().Be(expected);
    }


    // 3. Func can have implicit this pointer ("Mini interface")
    public class Counter
    {
        public int Value { get; protected set; }
        public int GetNext() => ++Value;
        public int GetNextPlusValue(int toAdd) => GetNext() + toAdd;
        public void Increment() => Value++;
        public void IncrementBy(int toAdd) => Value += toAdd;
    }

    [Fact]
    public void CounterFunctions_Should_HaveImplicitThis()
    {
        var counter = new Counter();

        var result1 = counter.GetNext();
        result1.Should().Be(1);

        var nextFn = counter.GetNext;
        var result2 = nextFn();
        result2.Should().Be(2);

        var result3 = counter.GetNextPlusValue(10);
        result3.Should().Be(13);

        var nextPlusValueFn = counter.GetNextPlusValue;
        var result4 = nextPlusValueFn(10);
        result4.Should().Be(14);

        // Sanity Check
        counter.Value.Should().Be(4);
    }

    // 4. Action is just Func for void/no return;
    [Fact]
    public void Increment_Should_IncreaseValue()
    {
        var counter = new Counter();
        Action incrementCounterFn = counter.Increment;
        incrementCounterFn();
        counter.Value.Should().Be(1);

        // Type parameters for action input types.
        Action<int> incrementByFn = counter.IncrementBy;
        incrementByFn(10);
        counter.Value.Should().Be(11);
    }
    
    
    // 5. Func can be a parameter to a function/method
    public class SuperCounter : Counter
    {
        public void ChangeBy(Func<int, int> changeValueFn)
        {
            try
            {
                Value = changeValueFn(Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public static int AddTen(int x) => checked(x + 10);
    public static int AddIntMax(int x) => checked(x + int.MaxValue);

    [Fact]
    public void ChangeBy_Should_ModifyValue()
    {
        var counter = new SuperCounter();
        counter.ChangeBy(AddTen);
        counter.Value.Should().Be(10);

        Func<int, int> addFn = AddTen;
        counter.ChangeBy(addFn);
        counter.Value.Should().Be(20);
        
        // ChangeBy will handle the integer overflow error from checked.
        counter.ChangeBy(AddIntMax);
        counter.Value.Should().Be(20);
    }
    
}