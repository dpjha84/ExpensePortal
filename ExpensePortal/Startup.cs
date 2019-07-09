using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ExpensePortal.Startup))]
namespace ExpensePortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
