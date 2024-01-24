using AutoMapper;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Web.Api.Models.Common;
using GmWeb.Web.Api.Models.Identity;
using GmWeb.Web.Common.Auth;
using GmWeb.Web.Common.Utility;
using GmWeb.Logic.Utility.Identity.DTO;
using Microsoft.AspNetCore.Identity;

namespace GmWeb.Web.Api.Utility
{
    public class ApiMappingProfile : CommonMappingProfile
    {
        public ApiMappingProfile()
        {
            this.CreateMap<GmIdentity, EditUserViewModel>().ReverseMap();

            this.CreateMap<Agency, AgencyDTO>().ReverseMap();
            this.CreateMap<Agency, AgencyDetailsDTO>().ReverseMap();

            this.CreateMap<User, UserDetailsDTO>().ReverseMap();

            this.CreateMap<Note, NoteDTO>().ReverseMap();
            this.CreateMap<Note, NoteDetailsDTO>().ReverseMap();
            this.CreateMap<NoteInsertDTO, Note>();
            this.CreateMap<NoteUpdateDTO, Note>()
                .ForMember(dest => dest.Title, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Title)))
                .ForMember(dest => dest.Message, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Message)))
                .ForMember(dest => dest.IsFlagged, opt => opt.Condition(src => src.IsFlagged.HasValue))
            ;

            this.CreateMap<Thread, ThreadDTO>()
                .ForMember(dest => dest.Message, opt =>
                {
                    opt.MapFrom(src => src.Content);
                })
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(src => src.ContentModified))
            ;
            this.CreateMap<Thread, ThreadDetailsDTO>()
                .ForMember(dest => dest.Message, opt =>
                {
                    opt.MapFrom(src => src.Content);
                })
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(src => src.ContentModified))
            ;
            this.CreateMap<ThreadInsertDTO, Thread>()
                .ForMember(dest => dest.Content, opt =>
                {
                    opt.MapFrom(src => src.Message);
                })
            ;
            this.CreateMap<ThreadUpdateDTO, Thread>()
                .ForMember(dest => dest.Title, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Title)))
                .ForMember(dest => dest.Content, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Message));
                    opt.MapFrom(src => src.Message);
                })
                .ForMember(dest => dest.IsFlagged, opt => opt.Condition(src => src.IsFlagged.HasValue))
            ;

            this.CreateMap<Activity, ActivityDTO>().ReverseMap();
            this.CreateMap<Activity, ActivityDetailsDTO>().ReverseMap();
            this.CreateMap<CalendarSchedule, CalendarDetailsDTO>();
            this.CreateMap<CalendarSchedule, CalendarDetailsDTO>();
            this.CreateMap<ScheduleInsertDTO, CalendarSchedule>();
            this.CreateMap<ScheduleUpdateDTO, CalendarSchedule>()
                .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Name)))
                .ForMember(dest => dest.StartTime, opt => opt.Condition(src => src.StartTime.HasValue))
                .ForMember(dest => dest.EndTime, opt => opt.Condition(src => src.EndTime.HasValue))
                .ForMember(dest => dest.Location, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Location)))
                .ForMember(dest => dest.OrganizerID, opt => opt.Condition(src => src.OrganizerID.HasValue))
                .ForMember(dest => dest.Capacity, opt => opt.Condition(src => src.Capacity.HasValue))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                .ForMember(dest => dest.EventType, opt => opt.Condition(src => src.EventType != null))
                .ForMember(dest => dest.Contribution, opt => opt.Condition(src => src.Contribution != null))
                .ForMember(dest => dest.RecurrenceRules, opt => opt.Condition(src => src.RecurrenceRules != null))
            ;

            this.CreateMap<UserActivity, UserActivityDTO>().ReverseMap();

            this.CreateMap<JwtSignInResult, SignInResult>().ReverseMap();
        }
    }
}
