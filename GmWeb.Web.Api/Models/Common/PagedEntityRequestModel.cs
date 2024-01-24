using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Performance.Paging;
using GmWeb.Logic.Utility.Extensions.Chronometry;
using GmWeb.Logic.Utility.Primitives;
using GmWeb.Web.Api.Utility;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SystemIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace GmWeb.Web.Api.Models.Common
{
    public interface IAgencyRequest
    {
        int? AgencyID { get; set; }
    }
    public abstract class UserActivityRequest
    {
        [Required]
        public Guid ActivityID { get; set; }
        [Required]
        public Guid LookupID { get; set; }
    }
    public class ConfirmClientRequest : UserActivityRequest { }
    public class RegisterClientRequest : UserActivityRequest { }
    public class UnregisterClientRequest : UserActivityRequest { }

    public class ClientActivityRegisterRequest
    {
        public int UserID { get; set; }
        public Guid ActivityID { get; set; }

    }
    public record class UnregisteredClientListRequest : PagedListRequest, IAgencyRequest
    {
        public int? AgencyID { get; set; }
        public Guid ActivityID { get; set; }
    }
    public record class RegisteredClientListRequest : PagedListRequest
    {
        public Guid ActivityID { get; set; }
    }
    public record class ActivityListRequest : TimePeriodListRequest<Activity>, IAgencyRequest
    {
        public int? ClientID { get; set; }
        public int? AgencyID { get; set; }
        [SystemIgnore]
        public int PageIndexOffset => this.Period.PeriodCount(this.PageDate);

        public override IExtendedPageListRequest<Activity, DateTime> Extend()
        {
            Expression<Func<Activity, DateTime>> grouper = this.Period switch
            {
                TimePeriod.Daily => a => a.StartTime.Date,
                TimePeriod.Weekly => a => a.StartTime.RecentWeekStart(),
                TimePeriod.Monthly => a => a.StartTime.RecentMonthStart(),
                TimePeriod.Quarterly => a => a.StartTime.RecentQuarterStart(),
                TimePeriod.Yearly => a => a.StartTime.RecentYearStart(),
                _ => null
            };
            var extended = new ExtendedTimePeriodListRequest<Activity>
            {
                PageIndex = this.PageIndex + this.PageIndexOffset,
                PageSize = this.PageSize,
                SortKeySelector = a => a.StartTime,
                GroupKeySelector = grouper
            };
            return extended;
        }
    }

    public record class ClientListRequest : AlphabeticalListRequest<User>, IAgencyRequest
    {
        [SystemIgnore]
        internal ViewerFilter<UserConfigStatus> Filter { get; set; } = new ViewerFilter<UserConfigStatus>();
        public UserConfigStatus Include { get => Filter.Include; set => Filter.Include = value; }
        public UserConfigStatus Exclude { get => Filter.Exclude; set => Filter.Exclude = value; }
        public int? AgencyID { get; set; }
        [SystemIgnore]
        internal User Viewer { get; set; }
        public override IExtendedPageListRequest<User, string> Extend()
        {
            if (this.Viewer == null)
                throw new ArgumentNullException(nameof(this.Viewer));
            return new ExtendedAlphabeticalListRequest<User>
            {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                GroupKey = this.Letter,
                SortKeySelector = u => u.LastName + u.FirstName,
                GroupKeySelector = u => u.LastName.Substring(0, 1),
                FilterPredicates =
                    this.Include.HasFlag(UserConfigStatus.Starred) ?
                        new Expression<Func<User, bool>>[]
                        {
                            (User u) => u.ParentConfigs
                                .Where(x => x.OwnerID == this.Viewer.UserID)
                                .Where(x => (x.Status & UserConfigStatus.Starred) == UserConfigStatus.Starred)
                                .Any()
                        }
                    : this.Exclude.HasFlag(UserConfigStatus.Starred) ?
                        new Expression<Func<User, bool>>[]
                        {
                            (User u) => !u.ParentConfigs
                                .Where(x => x.OwnerID == this.Viewer.UserID)
                                .Where(x => (x.Status & UserConfigStatus.Starred) == UserConfigStatus.Starred)
                                .Any()
                        }
                    : new Expression<Func<User, bool>>[] { }
            };
        }
    }
    public class AgencyRelatedRequest : IAgencyRequest
    {
        [Range(1, int.MaxValue)]
        public int? AgencyID { get; set; }
    }
}
