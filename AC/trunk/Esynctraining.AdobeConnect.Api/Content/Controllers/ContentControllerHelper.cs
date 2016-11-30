using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect.Api.Content.Controllers
{
    public sealed class ContentControllerHelper<TDto>
    {
        private readonly ILogger _logger;
        private readonly IContentService _contentService;
        private readonly IScoContentDtoMapper<TDto> _mapper;


        public ContentControllerHelper(ILogger logger, IContentService contentService, IScoContentDtoMapper<TDto> mapper)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (contentService == null)
                throw new ArgumentNullException(nameof(contentService));
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            _logger = logger;
            _contentService = contentService;
            _mapper = mapper;
        }


        //public async Task<OperationResultWithData<IEnumerable<TDto>>> GetShortcuts(IEnumerable<ScoShortcutType> typesToReturn)
        //{
        //    try
        //    {
        //        return OperationResultWithData<IEnumerable<TDto>>.Success(_contentService.GetShortcuts(typesToReturn).Select(x => _mapper.Map(x)));
        //    }
        //    catch (AdobeConnectException ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("GetShortcuts", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("GetShortcuts", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //}

        //[HttpGet]
        //[Route("my-content")]
        //public async Task<OperationResultWithData<IEnumerable<TDto>>> GetMyContent()
        //{
        //    try
        //    {
        //        return OperationResultWithData<IEnumerable<TDto>>.Success(_contentService.GetMyContent().Select(x => _mapper.Map(x)));
        //    }
        //    catch (AdobeConnectException ex)
        //    {
        //        if (ex.Status.Code == StatusCodes.no_access && ex.Status.SubCode == StatusSubCodes.denied)
        //        {
        //            return OperationResultWithData<IEnumerable<TDto>>.Error("You do not have permission to access this item.");
        //        }

        //        string errorMessage = GetOutputErrorMessage("GetMyContent", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("GetMyContent", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //}

        //public async Task<OperationResultWithData<IEnumerable<TDto>>> GetUserContent(string userLogin)
        //{
        //    if (string.IsNullOrWhiteSpace(userLogin))
        //        throw new ArgumentException("Non-empty value expected", nameof(userLogin));

        //    try
        //    {
        //        return OperationResultWithData<IEnumerable<TDto>>.Success(_contentService.GetUserContent(userLogin).Select(x => _mapper.Map(x)));
        //    }
        //    catch (AdobeConnectException ex)
        //    {
        //        if (ex.Status.Code == StatusCodes.no_access && ex.Status.SubCode == StatusSubCodes.denied)
        //        {
        //            return OperationResultWithData<IEnumerable<TDto>>.Error("You do not have permission to access this item.");
        //        }

        //        string errorMessage = GetOutputErrorMessage("GetMyContent", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("GetMyContent", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //}

        //public async Task<OperationResultWithData<IEnumerable<TDto>>> GetSharedContent()
        //{
        //    try
        //    {
        //        return OperationResultWithData<IEnumerable<TDto>>.Success(_contentService.GetSharedContent().Select(x => _mapper.Map(x)));
        //    }
        //    catch (AdobeConnectException ex)
        //    {
        //        if (ex.Status.Code == StatusCodes.no_access && ex.Status.SubCode == StatusSubCodes.denied)
        //        {
        //            return OperationResultWithData<IEnumerable<TDto>>.Error("You do not have permission to access this item.");
        //        }

        //        string errorMessage = GetOutputErrorMessage("GetSharedContent", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("GetSharedContent", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //}

        public async Task<OperationResultWithData<IEnumerable<TDto>>> GetFolderContent(string folderScoId, IDtoProcessor<TDto> processor)
        {
            if (string.IsNullOrWhiteSpace(folderScoId))
                throw new ArgumentException("Non-empty value expected", nameof(folderScoId));
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            try
            {
                var result = _contentService.GetFolderContent(folderScoId)
                    .Select(x => _mapper.Map(x))
                    .Select(y => processor.Process(y));

                return result.ToSuccessResult();
            }
            catch (AdobeConnectException ex)
            {
                if (ex.Status.Code == StatusCodes.no_access && ex.Status.SubCode == StatusSubCodes.denied)
                {
                    return OperationResultWithData<IEnumerable<TDto>>.Error("You do not have permission to access this item.");
                }

                string errorMessage = GetOutputErrorMessage("FolderContent", ex);
                return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("FolderContent", ex);
                return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
            }
        }

        public OperationResultWithData<string> GetDownloadAsZipLink(string scoId, string breezeToken)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));
            if (string.IsNullOrWhiteSpace(breezeToken))
                throw new ArgumentException("Non-empty value expected", nameof(breezeToken));

            try
            {
                string url = _contentService.GetDownloadAsZipLink(scoId, breezeToken);

                return url.ToSuccessResult();
            }
            catch (WarningMessageException ex)
            {
                return OperationResultWithData<string>.Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "AccessFile exception. sco-id:{0}.", scoId);
                return OperationResultWithData<string>.Error(ex.Message + ex.StackTrace);
            }
        }


        private string GetOutputErrorMessage(string methodName, Exception ex)
        {
            // TODO: logger.Error(methodName + lmsInfo, ex);

            if (ex is IUserMessageException)
                return ex.Message;

            return ex.ToString();

            //TODO:
            //return IsDebug
            //    ? Resources.Messages.ExceptionOccured + ex.ToString()
            //    : Resources.Messages.ExceptionMessage;
        }

    }

}
