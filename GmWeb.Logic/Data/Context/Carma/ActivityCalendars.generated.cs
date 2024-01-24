using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class ActivityCalendars : BaseDataCollection<ActivityCalendar, ActivityCalendars, CarmaCache, CarmaContext>
    {
        public ActivityCalendars(CarmaCache cache) : base(cache) { }

        public override DbSet<ActivityCalendar> EntitySet => this.DataContext.ActivityCalendars;
	}
}
