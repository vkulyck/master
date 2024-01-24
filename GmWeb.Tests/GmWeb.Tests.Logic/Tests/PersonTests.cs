using GmWeb.Tests.Logic.Models;

namespace GmWeb.Tests.Logic.Tests;
[Collection(nameof(LogicTestCollection))]
public class PersonTests : LogicTestBase<PersonTests>
{

    [Fact]
    public void TestFullName()
    {
        Assert.Equal("LN, T. FN MN, Sfx", new Person
        {
            LastName = "LN",
            Title = "T.",
            FirstName = "FN",
            MiddleName = "MN",
            Suffix = "Sfx"
        }.FullName);

        Assert.Equal("LN, T. FN, Sfx", new Person
        {
            LastName = "LN",
            Title = "T.",
            FirstName = "FN",
            Suffix = "Sfx"
        }.FullName);

        Assert.Equal("LN, FN, Sfx", new Person
        {
            LastName = "LN",
            FirstName = "FN",
            Suffix = "Sfx"
        }.FullName);

        Assert.Equal("LN, T. FN MN", new Person
        {
            LastName = "LN",
            Title = "T.",
            FirstName = "FN",
            MiddleName = "MN"
        }.FullName);

        Assert.Equal("LN, Sfx", new Person
        {
            LastName = "LN",
            Suffix = "Sfx"
        }.FullName);

        Assert.Equal("T. MN, Sfx", new Person
        {
            Title = "T.",
            MiddleName = "MN",
            Suffix = "Sfx"
        }.FullName);
    }
}