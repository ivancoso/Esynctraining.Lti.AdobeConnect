using Esynctraining.Lti.Lms.AgilixBuzz;
using Esynctraining.Lti.Lms.Canvas;
using System;
using Esynctraining.Lti.Lms.Common.API;

namespace Esynctraining.Lti.Zoom.Api
{
    public class LmsUserServiceFactory
    {
        private readonly CanvasLmsUserService _canvasLmsUserService;
        private readonly AgilixBuzzLmsUserService _agilixBuzzLmsUserService;

        public LmsUserServiceFactory(CanvasLmsUserService canvasLmsUserService, AgilixBuzzLmsUserService agilixBuzzLmsUserService)
        {
            _canvasLmsUserService = canvasLmsUserService ?? throw new ArgumentNullException(nameof(canvasLmsUserService));
            _agilixBuzzLmsUserService = agilixBuzzLmsUserService ?? throw new ArgumentNullException(nameof(agilixBuzzLmsUserService));
        }

        public LmsUserServiceBase GetUserService(int productId)
        {
            switch (productId)
            {
                case 2:
                    return _canvasLmsUserService;
                case 3:
                    return _agilixBuzzLmsUserService;
            }

            return null;
        }
    }
}
