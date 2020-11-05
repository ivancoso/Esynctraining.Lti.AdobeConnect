using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class CancelRecordingJobResult : ResultBase
    {
        public CancelRecordingJobResult(StatusInfo status)
            : base(status)
        {

        }
    }
}
