using Hangfire.Dashboard;

namespace GreenRoam.HangFireSetup
{
    public class DashboardNoAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true;
    }
}
