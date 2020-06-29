using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect.WebApi.Content.Controllers
{
    // TODO: move from "Content" to "Meetings" folder !!
    public sealed class MeetingsControllerHelper<TDto>
    {
        private readonly ILogger _logger;
        private readonly IMeetingService _meetingService;
        private readonly IScoContentDtoMapper<TDto> _mapper;


        public MeetingsControllerHelper(ILogger logger, IMeetingService meetingService, IScoContentDtoMapper<TDto> mapper)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (meetingService == null)
                throw new ArgumentNullException(nameof(meetingService));
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            _logger = logger;
            _meetingService = meetingService;
            _mapper = mapper;
        }


        //public async Task<OperationResultWithData<IEnumerable<TDto>>> GetMyMeetings()
        //{
        //    try
        //    {
        //        return OperationResultWithData<IEnumerable<TDto>>.Success(_meetingService.GetUserMeetings().Select(x => _mapper.Map(x)));
        //    }
        //    catch (AdobeConnectException ex)
        //    {
        //        if (ex.Status.Code == StatusCodes.no_access && ex.Status.SubCode == StatusSubCodes.denied)
        //        {
        //            return OperationResultWithData<IEnumerable<TDto>>.Error("You do not have permission to access this item.");
        //        }

        //        string errorMessage = GetOutputErrorMessage("GetMyMeetings", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("GetMyMeetings", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //}

        //public async Task<OperationResultWithData<IEnumerable<TDto>>> GetSharedMeetings()
        //{
        //    try
        //    {
        //        return OperationResultWithData<IEnumerable<TDto>>.Success(_meetingService.GetSharedMeetings().Select(x => _mapper.Map(x)));
        //    }
        //    catch (AdobeConnectException ex)
        //    {
        //        if (ex.Status.Code == StatusCodes.no_access && ex.Status.SubCode == StatusSubCodes.denied)
        //        {
        //            return OperationResultWithData<IEnumerable<TDto>>.Error("You do not have permission to access this item.");
        //        }

        //        string errorMessage = GetOutputErrorMessage("GetSharedMeetings", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("GetSharedMeetings", ex);
        //        return OperationResultWithData<IEnumerable<TDto>>.Error(errorMessage);
        //    }
        //}

        public async Task<OperationResultWithData<IEnumerable<TDto>>> FolderContent(string folderScoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folderScoId))
                    throw new ArgumentException("folderScoId can't be empty", nameof(folderScoId));

                return OperationResultWithData<IEnumerable<TDto>>.Success(_meetingService.GetFolderMeetings(folderScoId).Select(x => _mapper.Map(x)));
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
