using GmWeb.Logic.Utility.Performance.Paging;
using GmWeb.Web.Common.Models.Carma;

namespace GmWeb.Web.Api.Models.Common
{
    public class HomeDTO
    {
        public PagedList<ActivityDetailsDTO> Activities { get; set; }
        public PagedList<UserDTO> AssignedClients { get; set; }
        public PagedList<ThreadDTO> Notes { get; set; }
    }

    public class HomeSummaryDTO
    {
        public int TodayActivityCount { get; set; }
        public int StarredClientCount { get; set; }
        public int ClientsWithActivitiesTodayCount { get; set; }
    }
}
