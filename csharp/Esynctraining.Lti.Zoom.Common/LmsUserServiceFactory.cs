using System;
using System.Threading;
using Esynctraining.Lti.Lms.AgilixBuzz;
using Esynctraining.Lti.Lms.BlackBoard;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Desire2Learn;
using Esynctraining.Lti.Lms.Moodle;
using Esynctraining.Lti.Lms.Sakai;
using Esynctraining.Lti.Lms.Schoology;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;

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
        private readonly SakaiLmsUserService _sakaiLmsUserService;

        public LmsUserServiceFactory(CanvasLmsUserService canvasLmsUserService, 
            AgilixBuzzLmsUserService agilixBuzzLmsUserService, 
            SchoologyLmsUserService schoologyLmsUserService,
            BlackboardLmsUserService blackBoardLmsUserService,
            MoodleLmsUserService moodleLmsUserService,
            Desire2LearnLmsUserService desire2LearnLmsUserService,
            SakaiLmsUserService sakaiLmsUserService
            )
        {
            _canvasLmsUserService = canvasLmsUserService ?? throw new ArgumentNullException(nameof(canvasLmsUserService));
            _agilixBuzzLmsUserService = agilixBuzzLmsUserService ?? throw new ArgumentNullException(nameof(agilixBuzzLmsUserService));
            _schoologyLmsUserService = schoologyLmsUserService ?? throw new ArgumentNullException(nameof(schoologyLmsUserService));
            _blackBoardLmsUserService = blackBoardLmsUserService ?? throw new ArgumentNullException(nameof(blackBoardLmsUserService));
            _moodleLmsUserService = moodleLmsUserService ?? throw new ArgumentNullException(nameof(moodleLmsUserService));
            _desire2LearnLmsUserService = desire2LearnLmsUserService ?? throw new ArgumentNullException(nameof(desire2LearnLmsUserService));
            _sakaiLmsUserService = sakaiLmsUserService ?? throw new ArgumentNullException(nameof(sakaiLmsUserService));
        }

        public LmsUserServiceBase GetUserService(int productId)
        {
            switch ((LMS)productId)
            {
                case LMS.Canvas:
                    return _canvasLmsUserService;
                case LMS.AgilixBuzz:
                    return _agilixBuzzLmsUserService;
                case LMS.Schoology:
                    return _schoologyLmsUserService;
                case LMS.BlackBoard:
                    return _blackBoardLmsUserService;
                case LMS.Moodle:
                    return _moodleLmsUserService;
                case LMS.Sakai:
                    return _sakaiLmsUserService;
                case LMS.Desire2Learn:
                    return _desire2LearnLmsUserService;
            }

            return null;
        }
    }
}
