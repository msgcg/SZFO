using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SZFO.Startup))]
namespace SZFO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
