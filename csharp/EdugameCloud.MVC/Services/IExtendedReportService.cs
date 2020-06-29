using System.Collections.Generic;
using EdugameCloud.Core.Domain.DTO;

namespace EdugameCloud.MVC.Services
{
    public interface IExtendedReportService
    {
        byte[] GetExcelExtendedReportBytes(IEnumerable<ExtendedReportDto> dtos);

    }

}