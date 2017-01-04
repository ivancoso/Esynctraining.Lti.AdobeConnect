using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Logging;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;

namespace Esynctraining.AdobeConnect
{
    public partial class AdobeConnectProxy : IAdobeConnectProxy
    {
        public CollectionResult<AssetResponseInfo> ReportAssetResponseInfo(string meetingScoId, string interactionId)
        {
            if (string.IsNullOrWhiteSpace(meetingScoId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingScoId));
            if (string.IsNullOrWhiteSpace(interactionId))
                throw new ArgumentException("Non-empty value expected", nameof(interactionId));

            return Execute(() => { return _provider.ReportAssetResponseInfo(meetingScoId, interactionId); },
                meetingScoId, interactionId);
        }

        public CollectionResult<QuizQuestionResponseItem> ReportQuizQuestionResponse(string meetingScoId)
        {
            if (string.IsNullOrWhiteSpace(meetingScoId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingScoId));

            return Execute(() => { return _provider.ReportQuizQuestionResponse(meetingScoId); },
                meetingScoId);
        }

        public CollectionResult<QuizInteractionItem> ReportQuizInteractions(string meetingScoId)
        {
            if (string.IsNullOrWhiteSpace(meetingScoId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingScoId));

            return Execute(() => { return _provider.ReportQuizInteractions(meetingScoId); },
                meetingScoId);
        }

        public CollectionResult<QuizQuestionDistributionItem> ReportQuizQuestionDistribution(string meetingScoId)
        {
            if (string.IsNullOrWhiteSpace(meetingScoId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingScoId));

            return Execute(() => { return _provider.ReportQuizQuestionDistribution(meetingScoId); },
                meetingScoId);
        }

    }

}
