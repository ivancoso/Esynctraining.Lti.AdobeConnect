namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using System.Web.UI;
    using Core.Business.Models;
    using EdugameCloud.Core;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.MVC.Attributes;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using File = EdugameCloud.Core.Domain.Entities.File;

    [HandleError]
    public partial class BuildDeliverController : BaseController
    {
        private const string PublicFolderPath = "~/Content/swf/pub";
        private static readonly object _publicBuildZipLocker = new object();


        private readonly FileModel fileModel;
        private readonly AuthenticationModel authenticationModel;
        private readonly UserModel userModel;
        private readonly ILogger logger;
        private readonly IBuildVersionProcessor versionProcessor;


        private User CurrentUser
        {
            get
            {
                return (User)this.authenticationModel.GetCurrentUser(x => this.userModel.GetOneByEmail(x).Value);
            }
        }


        public BuildDeliverController(
            FileModel fileModel,
            UserModel userModel,
            AuthenticationModel authenticationModel,
            ApplicationSettingsProvider settings,
            ILogger logger, IBuildVersionProcessor versionProcessor)
            : base(settings)
        {
            this.fileModel = fileModel;
            this.userModel = userModel;
            this.authenticationModel = authenticationModel;
            this.logger = logger;
            this.versionProcessor = versionProcessor;
        }


        [HttpGet]
        [OutputCache(Duration = 2592000, VaryByParam = "id", NoStore = false, Location = OutputCacheLocation.Any)]
        public virtual ActionResult GetFile(string id)
        {
            File file = null;
            Guid webOrbId;
            if (Guid.TryParse(id, out webOrbId))
            {
                file = this.fileModel.GetOneByWebOrbId(webOrbId).Value;
            }

            if (file != null)
            {
                byte[] buffer = this.fileModel.GetData(file);
                if (buffer != null)
                {
                    return this.File(buffer, file.FileName.GetContentTypeByExtension(), file.FileName);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [CustomAuthorize]
        public virtual ActionResult Public(string fileName)
        {
            var file = new FileInfo(Path.Combine(Server.MapPath(PublicFolderPath), fileName));
            if (file.Exists)
            {
                return File(file.FullName,
                    file.Extension.EndsWith("ZIP", StringComparison.OrdinalIgnoreCase)
                        ? "application/zip"
                        : "application/x-shockwave-flash",
                    file.Name);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [CustomAuthorize]
        public virtual ActionResult GetPublicBuild()
        {
            try
            {
                var user = this.CurrentUser;
                if ((user != null) && (user.Company != null))
                {
                    var filePattern = (string)Settings.PublicBuildSelector;
                    var path = Server.MapPath(PublicFolderPath);
                    Version version = versionProcessor.ProcessVersion(path,
                        filePattern);
                    if (version == null)
                    {
                        logger.Warn("Could'n find any POD build");
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }

                    var publicBuild = filePattern.Replace("*", version.ToString());
                    string physicalPath = Path.Combine(Server.MapPath(PublicFolderPath), publicBuild);
                    Company company = user.Company;
                    if (company.CurrentLicense.With(x => x.LicenseStatus == CompanyLicenseStatus.Enterprise) 
                        && (company.Theme != null) 
                        && System.IO.File.Exists(physicalPath))
                    {
                        // NOTE: current POD size is about 960kb
                        var ms = new MemoryStream(960 * 1024);

                        using (var archive = ZipFile.OpenRead(physicalPath))
                        {
                            using (var arc = new ZipArchive(ms))
                            {
                                foreach (var file in archive.Entries.Where(x => x.Name != "config.xml"))
                                {
                                    using (Stream fs = arc.CreateEntry(file.Name).Open())
                                    {
                                        file.Open().CopyTo(fs);
                                    }
                                }

                                using (var fs = arc.CreateEntry("config.xml").Open())
                                {
                                    var xml = string.Format("<config><themeId>{0}</themeId><gateway>{1}</gateway></config>",
                                        company.Theme.With(x => x.Id),
                                        Settings.BaseServiceUrl);

                                    var xmlBuffer = System.Text.Encoding.ASCII.GetBytes(xml);
                                    fs.Write(xmlBuffer, 0, xmlBuffer.Length);
                                }
                            }
                        }

                        ms.Position = 0;
                        return this.File(ms, "application/zip", Path.GetFileName(physicalPath));
                    }

                    EnsureServicePathConfigExists(physicalPath);
                    return this.RedirectToAction(EdugameCloudT4.BuildDeliver.Public(publicBuild));
                }
            }
            catch (Exception ex)
            {
                logger.Error("BuildDeliverController.GetPublicBuild.", ex);
                throw;
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [CustomAuthorize]
        public virtual ActionResult GetMobileBuild()
        {
            try
            {
                var user = this.CurrentUser;
                if ((user != null) && (user.Company != null))
                {
                    var filePattern = (string)Settings.MobileBuildSelector;
                    var path = Server.MapPath(PublicFolderPath);
                    var version = versionProcessor.ProcessVersion(path, filePattern);
                    if (version == null)
                    {
                        logger.Warn("Could'n find any mobile POD build");
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }

                    var publicBuild = filePattern.Replace("*", version.ToString());
                    string physicalPath = Path.Combine(Server.MapPath(PublicFolderPath), publicBuild);
                    Company company = user.Company;
                    if (company.CurrentLicense.With(x => x.LicenseStatus == CompanyLicenseStatus.Enterprise)
                        && (company.Theme != null)
                        && System.IO.File.Exists(physicalPath))
                    {
                        // NOTE: current POD size is about 960kb
                        var ms = new MemoryStream(960 * 1024);

                        using (var archive = ZipFile.OpenRead(physicalPath))
                        {
                            using (var arc = new ZipArchive(ms))
                            {
                                foreach (var file in archive.Entries.Where(x => x.Name != "config.xml"))
                                {
                                    using (Stream fs = arc.CreateEntry(file.Name).Open())
                                    {
                                        file.Open().CopyTo(fs);
                                    }
                                }

                                using (var fs = arc.CreateEntry("config.xml").Open())
                                {
                                    var xml = string.Format("<config><themeId>{0}</themeId><gateway>{1}</gateway></config>",
                                        company.Theme.With(x => x.Id),
                                        Settings.BaseServiceUrl);

                                    var xmlBuffer = System.Text.Encoding.ASCII.GetBytes(xml);
                                    fs.Write(xmlBuffer, 0, xmlBuffer.Length);
                                }
                            }
                        }

                        ms.Position = 0;
                        return this.File(ms, "application/zip", Path.GetFileName(physicalPath));
                    }

                    EnsureServicePathConfigExists(physicalPath);
                    return this.RedirectToAction(EdugameCloudT4.BuildDeliver.Public(publicBuild));
                }
            }
            catch (Exception ex)
            {
                logger.Error("BuildDeliverController.GetPublicBuild.", ex);
                throw;
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }


        private void EnsureServicePathConfigExists(string physicalPath)
        {
            if (!System.IO.File.Exists(physicalPath))
                return;

            if (string.IsNullOrWhiteSpace(Settings.BaseServiceUrl as string))
            {
                logger.Error("EnsureServicePathConfigExists. Settings.BaseServiceUrl is empty.");
                return;
            }

            int fileCount;
            using (var archive = ZipFile.OpenRead(physicalPath))
            {
                fileCount = archive.Entries.Count();
            }
            // NOTE: swf + config
            if (fileCount != 2)
            {
                lock (_publicBuildZipLocker)
                {
                    using (var archive = ZipFile.OpenRead(physicalPath))
                    {
                        fileCount = archive.Entries.Count();
                    }

                    if (fileCount != 2)
                    {
                        try
                        {
                            var ms = new MemoryStream();
                            using (var archive = ZipFile.OpenRead(physicalPath))
                            {
                                using (var arc = new ZipArchive(ms))
                                {
                                    foreach (var file in archive.Entries.Where(x => x.Name != "config.xml"))
                                    {
                                        using (Stream fs = arc.CreateEntry(file.Name).Open())
                                        {
                                            file.Open().CopyTo(fs);
                                        }
                                    }

                                    using (var fs = arc.CreateEntry("config.xml").Open())
                                    {
                                        var xml = string.Format("<config><gateway>{0}</gateway></config>", Settings.BaseServiceUrl);
                                        var xmlBuffer = System.Text.Encoding.ASCII.GetBytes(xml);
                                        fs.Write(xmlBuffer, 0, xmlBuffer.Length);
                                    }
                                }
                            }

                            using (var file = new FileStream(physicalPath, FileMode.Create, FileAccess.Write))
                            {
                                ms.WriteTo(file);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("EnsureServicePathConfigExists. Error during config writing.", ex);
                            throw new InvalidOperationException("An error occurred, please try again later.");
                        }
                    }
                }
            }

        }

    }

}