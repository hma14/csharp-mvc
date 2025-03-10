using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Omnae.Startup))]
namespace Omnae
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
