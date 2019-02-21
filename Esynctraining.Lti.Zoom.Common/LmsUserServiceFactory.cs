using System;
using Esynctraining.Lti.Lms.AgilixBuzz;
using Esynctraining.Lti.Lms.BlackBoard;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Desire2Learn;
using Esynctraining.Lti.Lms.Moodle;
using Esynctraining.Lti.Lms.Schoology;

namespace Esynctraining.Lti.Zoom.Common
{
    public class LmsUserServiceFactory
    {
        private readonly CanvasLmsUserService _canvasLmsUserService;
        private readonly AgilixBuzzLmsUserService _agilixBuzzLmsUserService;
        private readonly SchoologyLmsUserService _schoologyLmsUserService;
        private readonly BlackboardLmsUserService _blackBoardLmsUserService;
        private readonly MoodleLmsUserService _moodleLmsUserService;
        private readonly Desire2LearnLmsUserService _desire2LearnLmsUserService;

        public LmsUserServiceFactory(CanvasLmsUserService canvasLmsUserService, 
            AgilixBuzzLmsUserService agilixBuzzLmsUserService, 
            SchoologyLmsUserService schoologyLmsUserService,
            BlackboardLmsUserService blackBoardLmsUserService,
            MoodleLmsUserService moodleLmsUserService,
            Desire2LearnLmsUserService desire2LearnLmsUserService
            )
        {
            _canvasLmsUserService = canvasLmsUserService ?? throw new ArgumentNullException(nameof(canvasLmsUserService));
            _agilixBuzzLmsUserService = agilixBuzzLmsUserService ?? throw new ArgumentNullException(nameof(agilixBuzzLmsUserService));
            _schoologyLmsUserService = schoologyLmsUserService ?? throw new ArgumentNullException(nameof(schoologyLmsUserService));
            _blackBoardLmsUserService = blackBoardLmsUserService ?? throw new ArgumentNullException(nameof(blackBoardLmsUserService));
            _moodleLmsUserService = moodleLmsUserService ?? throw new ArgumentNullException(nameof(moodleLmsUserService));
            _desire2LearnLmsUserService = desire2LearnLmsUserService ?? throw new ArgumentNullException(nameof(desire2LearnLmsUserService));
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
                case 1050:
                    return _moodleLmsUserService;
                case 1070:
                    return _desire2LearnLmsUserService;
            }

            return null;
        }
    }
}
