using System;
using EdugameCloud.Core.Domain.Entities;

namespace EdugameCloud.MVC.Services
{
    public class ReportServiceFactory
    {
        public static IExtendedReportService GetReportService(SubModuleItemType subModuleType, int reportType)
        {
            switch (reportType)
            {
                case 1:
                    return new HFExtendedReportService();
                case 2:
                    if(subModuleType == SubModuleItemType.Survey)
                        return new MngSurveyReportService();
                    return new MngQuizReportService();
                default:
                    throw new InvalidOperationException($"Invalid detailed report type: {reportType}");
            }
        }
    }
}