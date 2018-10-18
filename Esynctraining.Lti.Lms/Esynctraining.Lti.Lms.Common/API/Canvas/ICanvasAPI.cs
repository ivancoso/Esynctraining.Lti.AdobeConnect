using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API.Canvas
{
    public interface ICanvasAPI
    {
        bool IsTokenExpired(string api, string userToken);
    }

}
