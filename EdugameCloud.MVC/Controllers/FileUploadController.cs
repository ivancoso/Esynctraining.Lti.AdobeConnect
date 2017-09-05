namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Domain;
    using EdugameCloud.Core.Business.Models;
    using Esynctraining.Core.Utils;
    using Esynctraining.Core.Logging;

    [HandleError]
    public partial class FileUploadController : BaseController
    {
        private readonly ILogger _logger;
        private FileModel FileModel => IoC.Resolve<FileModel>();


        public FileUploadController(
            ILogger logger,
            ApplicationSettingsProvider settings
            )
            : base(settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public JsonResult Upload(Guid id)
        {
            if (Request.Files.Count == 1)
            {
                try
                {
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase hpf = Request.Files[fileName];

                        var file = new UploadedFileDTO
                        {
                            fileId = id,
                            fileName = fileName,
                            contentType = hpf.ContentType,
                            content = hpf.InputStream.ReadToEnd(),
                            dateCreated = DateTime.Now,
                        };

                        FileModel.SaveWeborbFile(file);

                        // TRICK: we support single file
                        break;
                    }

                    return Json(OperationResult.Success());
                }
                catch (Exception ex)
                {
                    _logger.Error("[File.Upload] failed", ex);
                    return Json(OperationResult.Error("Error occured"));
                }
            }
            return Json(OperationResult.Error("Error occured. Not File uploaded."));
        }

    }

}