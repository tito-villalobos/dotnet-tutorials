namespace UnderstandingLinq;

using System.Collections;

public record Person(Guid Id, string Name, int Number);

public class People : IEnumerable<Person>
{
    public IEnumerator<Person> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly List<Person> _personList = new();
    public People()
    {
        var i = 1;
        _personList.Add(new Person(Guid.NewGuid(), "Foo", i++));
        _personList.Add(new Person(Guid.NewGuid(), "Bar", i++));
        _personList.Add(new Person(Guid.NewGuid(), "Baz", i++));
    }
    
}
