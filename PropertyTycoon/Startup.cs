using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PropertyTycoon.Startup))]
namespace PropertyTycoon
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
