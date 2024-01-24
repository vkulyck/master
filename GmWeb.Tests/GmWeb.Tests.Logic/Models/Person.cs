using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Interfaces;

namespace GmWeb.Tests.Logic.Models;

internal class Person : IPerson
{

    public string Title { get; set; }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public string Suffix { get; set; }

    public string FullName => ((IPerson)this).GetFullName();

    public override string ToString() => this.FullName;
}
