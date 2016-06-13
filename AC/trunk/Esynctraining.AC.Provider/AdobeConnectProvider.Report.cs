﻿using System;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
        public UserCollectionResult ReportScoViews(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            // act: "report-sco-views"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportScoViews,
                string.Format(CommandParams.ScoId, scoId), out status);

            return ResponseIsOk(doc, status)
                       ? new UserCollectionResult(status, UserCollectionParser.Parse(doc, this.requestProcessor.ServiceUrl, null))
                       : new UserCollectionResult(status);
        }
        
    }

}
