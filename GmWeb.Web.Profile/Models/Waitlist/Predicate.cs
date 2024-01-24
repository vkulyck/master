using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class Predicate : EditableFlowModelBase
    {
        public string Formula { get; set; }

        public Predicate() : this(string.Empty) { }
        public Predicate(string formula)
        {
            this.Formula = formula;
        }

        public static implicit operator string(Predicate p)
        {
            return p.Formula;
        }

        public static implicit operator Predicate(string s) => new Predicate(s);

        public override string ToString()
        {
            return this.Formula;
        }
    }
}