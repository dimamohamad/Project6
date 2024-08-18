using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Optimization;

namespace template
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css").Include(
               "~/css/ajax-loader.gif",
            "~/css/bootstrap.min.css",
              "~/css/vendor.css",
              "~/css/style.css"


           ));

        }
    }
}
