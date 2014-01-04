using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JustPhotoGallery.Web.Startup))]
namespace JustPhotoGallery.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
