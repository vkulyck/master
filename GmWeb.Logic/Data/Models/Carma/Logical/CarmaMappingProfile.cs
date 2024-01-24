using System;
using MappingProfile = AutoMapper.Profile;
using iCalRecurrence = Ical.Net.DataTypes.RecurrencePattern;

namespace GmWeb.Logic.Data.Models.Carma;
public class CarmaMappingProfile : MappingProfile
{
    public CarmaMappingProfile()
    {
        this.CreateMap<User, GmIdentity>().ReverseMap();
        this.CreateMap<CalendarRecurrence, iCalRecurrence>().ReverseMap();
        this.CreateMap<Activity, CalendarSchedule>().ReverseMap();
    }
}