using GmWeb.Logic.Utility.Extensions.Collections;

namespace GmWeb.Logic.Interfaces;

public interface IPerson
{
    public string Title { get; }
    public string FirstName { get; }
    public string MiddleName { get; }
    public string LastName { get; }
    public string Suffix { get; }
    public string GetFullName() =>
        ", ".JoinNonNull(
            this.LastName,
            " ".JoinNonNull(
                this.Title,
                this.FirstName,
                this.MiddleName
            ),
            this.Suffix
        );
}
