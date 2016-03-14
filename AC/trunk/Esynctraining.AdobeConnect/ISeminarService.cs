﻿using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public interface ISeminarService
    {
        IEnumerable<ScoContent> GetAllSeminarLicenses(IAdobeConnectProxy provider);

        IEnumerable<SeminarLicenseSco> GetSharedOrUserSeminarLicenses(IAdobeConnectProxy provider,
            bool returnUserLicenses = false);

        IEnumerable<ScoContent> GetSeminars(string seminarLicenseScoId, IAdobeConnectProxy provider);

        IEnumerable<ScoContent> GetSeminarSessions(string seminarScoId, IAdobeConnectProxy provider);

        ScoInfo CreateSeminar(SeminarUpdateItem seminar, IAdobeConnectProxy provider);

        ScoInfo SaveSession(SeminarSessionDto intem, IAdobeConnectProxy provider);

        StatusInfo DeleteSesson(string seminarSessionScoId, IAdobeConnectProxy provider);

        StatusInfo DeleteSeminar(string seminarScoId, IAdobeConnectProxy provider);

    }

}
