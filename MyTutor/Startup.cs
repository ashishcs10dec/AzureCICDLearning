using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyTutor.Startup))]
namespace MyTutor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
