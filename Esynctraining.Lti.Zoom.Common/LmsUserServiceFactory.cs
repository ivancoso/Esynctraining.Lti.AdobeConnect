using System;
using Esynctraining.Lti.Lms.AgilixBuzz;
using Esynctraining.Lti.Lms.BlackBoard;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Schoology;

namespace Esynctraining.Lti.Zoom.Common
{
    public class LmsUserServiceFactory
    {
        private readonly CanvasLmsUserService _canvasLmsUserService;
        private readonly AgilixBuzzLmsUserService _agilixBuzzLmsUserService;
        private readonly SchoologyLmsUserService _schoologyLmsUserService;
        private readonly BlackboardLmsUserService _blackBoardLmsUserService;

        public LmsUserServiceFactory(CanvasLmsUserService canvasLmsUserService, 
            AgilixBuzzLmsUserService agilixBuzzLmsUserService, 
            SchoologyLmsUserService schoologyLmsUserService,
            BlackboardLmsUserService blackBoardLmsUserService
            )
        {
            _canvasLmsUserService = canvasLmsUserService ?? throw new ArgumentNullException(nameof(canvasLmsUserService));
            _agilixBuzzLmsUserService = agilixBuzzLmsUserService ?? throw new ArgumentNullException(nameof(agilixBuzzLmsUserService));
            _schoologyLmsUserService = schoologyLmsUserService ?? throw new ArgumentNullException(nameof(schoologyLmsUserService));
            _blackBoardLmsUserService = blackBoardLmsUserService ?? throw new ArgumentNullException(nameof(blackBoardLmsUserService));
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
                case 1040:
                    return _blackBoardLmsUserService;
            }

            return null;
        }
    }
}
