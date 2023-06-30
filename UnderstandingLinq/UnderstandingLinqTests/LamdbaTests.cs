namespace UnderstandingLinqTests;

using FluentAssertions;

// 1. Lambdas are just a shorthand for defining a Func<> inline with an implict return type
// 2. Lambdas can take arguments in addition to return type
// 3. Lambdas implicitly "capture" variables (like the "this" for Func)
// 4. Lambdas major use is to quickly create Func<> arguments
public class LamdbaTests
{
    // 1. Lambdas are just a shorthand for defining a Func<> inline with an implict return type
    [Fact]
    public void GetIntLambda_Should_ReturnValue()
    {
        // Zero input parameters require ()
        Func<int> getIntFn = () => 3;
        var result = getIntFn();
        result.Should().Be(3);

        // Can use "var"
        var getIntFn2 = () => 4;
        getIntFn2().Should().Be(4);

        // Explicit Lambda Notation
        var explicitNotationFn = () => {
            var x = 2;
            var y = 3;
            return x + y;
        };
        explicitNotationFn().Should().Be(5);
    }

    // 2. Lambdas can take arguments in addition to return type
    [Theory]
    [InlineData(3, 13)]
    public void AddTen_Lambda_Should_ReturnAdded10(int input, int expected)
    {
        // if input type is deduce-able, do not need to specify input type.
        Func<int, int> addTenFn = x => x + 10;
        addTenFn(input).Should().Be(expected);
        
        // if input type is not deduce-able must specify it explicity with params around the argument.
        var addTenLocalFn = (int x) => x + 10;
        addTenLocalFn(input).Should().Be(expected);
    }

    [Theory]
    [InlineData(5, 7, 12)]
    public void AddTogetherLambda_Should_ReturnSum(int input1, int input2, int expected)
    {
        // Multiple input parameters always requires () around the input names.
        Func<int, int, int> addFn = (x, y) => x + y;
        addFn(input1, input2).Should().Be(expected);

        // Cannot deduce input param types, so they must be specificed
        var addFn2 = (int x, int y) => x + y;
        addFn2(input1, input2).Should().Be(expected);
    }
    
    
    // 3. Lambdas implicitly "capture" variables (like the "this" for Func)
    [Fact]
    public void CounterFunctions_Should_HaveImplicitThis()
    {
        var counter = new FuncTests.Counter();
        counter.IncrementBy(10);

        // Take the counter as an explicit parameter
        // Use explicit lambda notation.
        Action<FuncTests.Counter> incCounterFn = ctr => ctr.Increment();
        // Need to pass in the counter
        incCounterFn(counter);
        counter.Value.Should().Be(11);
        
        
        // Implicitly capture the local variable 'counter' in the lambda
        Action incCapturedCounterFn = () => counter.Increment();
        incCapturedCounterFn();
        counter.Value.Should().Be(12);

        
        // Must use explicit notation for multiple statements
        Func<int> incrementAndReturnCaptureCounterFn = () => {
            counter.Increment();
            return counter.Value;
        };

        incrementAndReturnCaptureCounterFn().Should().Be(13);
        counter.Value.Should().Be(13);
    }


    public class SuperDuperCounter
    {
        private readonly List<int> _previousValues = new();
        public IEnumerable<int> PreviousValues => _previousValues;
        public int Value { get; private set; }

        public void ChangeBy(Func<int, int> changeValueFn)
        {
            var oldValue = Value;
            _previousValues.Add(oldValue);
            Value = changeValueFn(oldValue);
        }
    }
    
    // 4. Lambdas major use is to quickly create Func<> arguments
    [Fact]
    public void SuperCounterLambdas_Work()
    {
        var counter = new SuperDuperCounter();
        
        counter.ChangeBy(_ => 3);
        counter.Value.Should().Be(3);
        counter.PreviousValues.Should().Equal(0);
        
        counter.ChangeBy(x => x-10);
        counter.Value.Should().Be(-7);
        counter.PreviousValues.Should().Equal(0, 3);
        
        counter.ChangeBy(x => x*x);
        counter.Value.Should().Be(49);
        counter.PreviousValues.Should().Equal(0, 3, -7);
    }
}