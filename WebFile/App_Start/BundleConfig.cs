using System.Web;
using System.Web.Optimization;

namespace WebFile
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            // 使用 Modernizr 的开发版本进行开发和了解信息。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/js").Include(
                        "~/Content/LigerUI/lib/jquery/jquery-1.9.0.min.js",
                        "~/Content/LigerUI/lib/ligerUI/js/core/base.js",
                        "~/Content/LigerUI/lib/ligerUI/js/ligerui.min.js"
                        ));

            

            bundles.Add(new StyleBundle("~/css").Include(
                        "~/Content/LigerUI/lib/ligerUI/skins/Aqua/css/ligerui-all.css"
                       ));
        }
    }
}