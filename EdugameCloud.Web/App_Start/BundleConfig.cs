namespace EdugameCloud.Web.App_Start
{
    using System.Web.Optimization;

    using EdugameCloud.MVC.Constants;

    /// <summary>
    /// The bundle config.
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// The register bundles. For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        /// </summary>
        /// <param name="bundles">
        /// The bundles.
        /// </param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle(Bundles.JQuery).Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle(Bundles.Core).Include("~/Scripts/core.js"));

            bundles.Add(new ScriptBundle(Bundles.JQueryValidation).Include("~/Scripts/jquery.unobtrusive*", "~/Scripts/jquery.validate*"));

            bundles.Add(new StyleBundle(Bundles.Css).Include("~/Content/styles/*.css"));
        }
    }
}