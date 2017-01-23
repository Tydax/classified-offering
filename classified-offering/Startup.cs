using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(classified_offering.Startup))]
namespace classified_offering
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
