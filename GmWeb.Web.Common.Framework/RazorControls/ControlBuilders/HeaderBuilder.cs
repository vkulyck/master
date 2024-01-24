using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public enum HeadingLevel { H1 = 1, H2 = 2, H3 = 3, H4 = 4, H5 = 5, H6 = 6 };
    public class H1Builder : HeaderBuilder
    {
        public H1Builder() : base(1) { }
    }
    public class H2Builder : HeaderBuilder
    {
        public H2Builder() : base(2) { }
    }
    public class H3Builder : HeaderBuilder
    {
        public H3Builder() : base(3) { }
    }
    public class H4Builder : HeaderBuilder
    {
        public H4Builder() : base(4) { }
    }
    public class HeaderBuilder : ControlBuilder<HeaderBuilder>
    {
        public HeaderBuilder(int level) : base($"h{level}") { }
        public HeaderBuilder(HeadingLevel level) : base($"h{(int)level}") { }
    }
}