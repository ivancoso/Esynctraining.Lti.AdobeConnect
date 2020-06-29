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


        private readonly FileModel _fileModel;
        private readonly AuthenticationModel _authenticationModel;
        private readonly UserModel _userModel;
        private readonly ILogger _logger;
        private readonly IBuildVersionProcessor _versionProcessor;


        private User CurrentUser
        {
            get
            {
                return (User)_authenticationModel.GetCurrentUser(x => this._userModel.GetOneByEmail(x).Value);
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
            _fileModel = fileModel;
            _userModel = userModel;
            _authenticationModel = authenticationModel;
            _logger = logger;
            _versionProcessor = versionProcessor;
        }


        [HttpGet]
        [OutputCache(Duration = 2592000, VaryByParam = "id", NoStore = false, Location = OutputCacheLocation.Any)]
        public virtual ActionResult GetFile(string id)
        {
            File file = null;
            Guid webOrbId;
            if (Guid.TryParse(id, out webOrbId))
            {
                file = this._fileModel.GetOneByWebOrbId(webOrbId).Value;
            }

            if (file != null)
            {
                byte[] buffer = this._fileModel.GetData(file);
                if (buffer != null)
                {
                    return this.File(buffer, GetContentTypeByExtension(file.FileName), file.FileName);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [CustomAuthorize]
        public virtual ActionResult Public(string fileName)
        {
            _logger.Error("[BuildDeliverController.Public]");
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
        //file/get-public-build
        public virtual ActionResult GetPublicBuild()
        {
            try
            {
                var user = this.CurrentUser;
                if ((user != null) && (user.Company != null))
                {
                    var filePattern = (string)Settings.PublicBuildSelector;
                    var path = Server.MapPath(PublicFolderPath);
                    Version version = _versionProcessor.ProcessVersion(path,
                        filePattern);
                    if (version == null)
                    {
                        _logger.Warn("Could'n find any POD build");
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
                            using (var arc = new ZipArchive(ms, ZipArchiveMode.Create, true))
                            {
                                CopyAllFilesExceptConfig(archive, arc);

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

                    var file = new FileInfo(Path.Combine(Server.MapPath(PublicFolderPath), physicalPath));
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
            }
            catch (Exception ex)
            {
                _logger.Error("BuildDeliverController.GetPublicBuild.", ex);
                throw;
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [CustomAuthorize]
        //file/get-mobile-build
        public virtual ActionResult GetMobileBuild()
        {
            try
            {
                var user = this.CurrentUser;
                if ((user != null) && (user.Company != null))
                {
                    var filePattern = (string)Settings.MobileBuildSelector;
                    var path = Server.MapPath(PublicFolderPath);
                    var version = _versionProcessor.ProcessVersion(path, filePattern);
                    if (version == null)
                    {
                        _logger.Warn("Could'n find any mobile POD build");
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
                            using (var arc = new ZipArchive(ms, ZipArchiveMode.Create, true))
                            {
                                CopyAllFilesExceptConfig(archive, arc);

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

                    var file = new FileInfo(Path.Combine(Server.MapPath(PublicFolderPath), publicBuild));
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
            }
            catch (Exception ex)
            {
                _logger.Error("BuildDeliverController.GetMobileBuild.", ex);
                throw;
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }


        private static void CopyAllFilesExceptConfig(ZipArchive archive, ZipArchive arc)
        {
            foreach (var file in archive.Entries.Where(x => x.Name != "config.xml"))
            {
                using (Stream fs = arc.CreateEntry(file.FullName).Open())
                {
                    file.Open().CopyTo(fs);
                }
            }
        }

        private void EnsureServicePathConfigExists(string physicalPath)
        {
            if (!System.IO.File.Exists(physicalPath))
                return;

            if (string.IsNullOrWhiteSpace(Settings.BaseServiceUrl as string))
            {
                _logger.Error("EnsureServicePathConfigExists. Settings.BaseServiceUrl is empty.");
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
                            // NOTE: current POD size is about 960kb
                            var ms = new MemoryStream(960 * 1024);
                            using (var archive = ZipFile.OpenRead(physicalPath))
                            {
                                using (var arc = new ZipArchive(ms, ZipArchiveMode.Create, true))
                                {
                                    CopyAllFilesExceptConfig(archive, arc);

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
                            _logger.Error("EnsureServicePathConfigExists. Error during config writing.", ex);
                            throw new InvalidOperationException("An error occurred, please try again later.");
                        }
                    }
                }
            }

        }

        private static string GetContentTypeByExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName).If(x => !string.IsNullOrEmpty(x), x => x.Substring(1)) ?? string.Empty;
            ext = ext.ToLower();
            switch (ext)
            {
                case "png":
                case "gif":
                case "tiff":
                case "bmp":
                case "pict":
                    return @"image/" + ext;
                case "jpg":
                case "jpe":
                case "jpeg":
                    return @"image/jpeg";
                case "swf":
                    return @"application/x-shockwave-flash";
                case "zip":
                    return @"application/zip";
                default:
                    return null;
            }
        }

    }

}