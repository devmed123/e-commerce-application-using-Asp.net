using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(myauth.Startup))]
namespace myauth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
