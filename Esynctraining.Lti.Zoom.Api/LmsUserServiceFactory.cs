using Esynctraining.Lti.Lms.AgilixBuzz;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Schoology;
using System;

namespace Esynctraining.Lti.Zoom.Api
{
    public class LmsUserServiceFactory
    {
        private readonly CanvasLmsUserService _canvasLmsUserService;
        private readonly AgilixBuzzLmsUserService _agilixBuzzLmsUserService;
        private readonly SchoologyLmsUserService _schoologyLmsUserService;

        public LmsUserServiceFactory(CanvasLmsUserService canvasLmsUserService, AgilixBuzzLmsUserService agilixBuzzLmsUserService, SchoologyLmsUserService schoologyLmsUserService)
        {
            _canvasLmsUserService = canvasLmsUserService ?? throw new ArgumentNullException(nameof(canvasLmsUserService));
            _agilixBuzzLmsUserService = agilixBuzzLmsUserService ?? throw new ArgumentNullException(nameof(agilixBuzzLmsUserService));
            _schoologyLmsUserService = schoologyLmsUserService ?? throw new ArgumentNullException(nameof(schoologyLmsUserService));
        }

        public LmsUserServiceBase GetUserService(int productId)
        {
            switch (productId)
            {
                case 1010:
                    return _canvasLmsUserService;
                case 1020:
                    return _agilixBuzzLmsUserService;
                case 1030:
                    return _schoologyLmsUserService;
            }

            return null;
        }
    }
}
