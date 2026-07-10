using System.Web.Optimization;

namespace TaskManager
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.bundle.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                "~/Scripts/site.js"));

            // Bootstrap is loaded from CDN in _Layout.cshtml. The local bundle only
            // contains app-specific overrides — keeping the bundle keeps Razor's
            // Styles.Render call valid even if the bootstrap NuGet package didn't
            // drop its CSS into ~/Content.
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
