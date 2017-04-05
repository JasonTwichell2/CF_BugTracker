using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BugSleuth2.Startup))]
namespace BugSleuth2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
