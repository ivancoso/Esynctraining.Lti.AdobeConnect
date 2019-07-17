using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API.Canvas
{
    public interface ICanvasAPI
    {
        string BuildAuthUrl(string returnUrl, string lmsDomain, string oAuthId, string session);

        Task<OAuthTokenResponse> RequestToken(string basePath, string oAuthId, string oAuthKey, string code,
            string lmsDomain);

        Task<string> RequestTokenByRefreshToken(RefreshTokenParamsDto refreshTokenParams, string lmsDomain);

        Task<bool> IsTokenExpired(string api, string userToken);

        Task<LmsUserDTO> GetUser(string api, string userToken, string userId);
        Task<AnnouncementDTO> CreateAnnouncement(string api, string userToken, string courseId, string title, string message);
    }
}
