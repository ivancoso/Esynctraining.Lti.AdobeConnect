using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ClosedXML.Excel;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using NHibernate;

    /// <summary>
    /// The SN GroupDiscussion model.
    /// </summary>
    public class SNGroupDiscussionModel : BaseModel<SNGroupDiscussion, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNGroupDiscussionModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SNGroupDiscussionModel(IRepository<SNGroupDiscussion, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all by session id.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SNSessionMember}"/>.
        /// </returns>
        public IFutureValue<SNGroupDiscussion> GetOneByACSessionId(int sessionId)
        {
            var queryOver =
                new DefaultQueryOver<SNGroupDiscussion, int>().GetQueryOver().Where(x => x.ACSessionId == sessionId).Take(1);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get all by session ids.
        /// </summary>
        /// <param name="sessionIds">
        /// The session ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SNGroupDiscussion}"/>.
        /// </returns>
        public IEnumerable<SNGroupDiscussion> GetAllByACSessionIds(List<int> sessionIds)
        {
            var queryOver =
                new DefaultQueryOver<SNGroupDiscussion, int>().GetQueryOver().WhereRestrictionOn(x => x.ACSessionId).IsIn(sessionIds);
            return this.Repository.FindAll(queryOver);
        }

        #endregion

        public byte[] ConvertGroupDiscussionToExcel(SNGroupDiscussion groupDiscussion)
        {
            try
            {
                dynamic x = groupDiscussion.GroupDiscussionData.ToDynamic();
                dynamic xmlRoot = x.messages;
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add(groupDiscussion.GroupDiscussionTitle.ReplaceAll(@"\/?*[]"));
                worksheet.Column(2).Width = 40;
                AddHeaders(worksheet);
                if (!(xmlRoot is string))
                {
                    int counter = 1;
                    foreach (var message in xmlRoot.message)
                    {
                        /*Participant Name*/
                        worksheet.Cell(counter + 1, 1).Value = message.userName;
                        /*Participant Message*/
                        worksheet.Cell(counter + 1, 2).Value = message.text;
                        worksheet.Cell(counter + 1, 2).Style.Alignment.WrapText = true;
                        /*Likes*/
                        worksheet.Cell(counter + 1, 3).Value = message.likes;
                        /*Dislikes*/
                        worksheet.Cell(counter + 1, 4).Value = message.dislikes;
                        counter++;
                    }
                }
                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                IoC.Resolve<ILogger>().Error("SNGroupDiscussionModel.ConvertGroupDiscussionToExcel", ex);
                return null;
            }
        }

        /// <summary>
        /// The add headers.
        /// </summary>
        /// <param name="worksheet">
        /// The worksheet.
        /// </param>
        private void AddHeaders(IXLWorksheet worksheet)
        {
            var headers = new List<string> { "Participant Name", "Participant Message", "Likes", "Dislikes" };
            for (int i = 1; i <= headers.Count; i++)
            {
                worksheet.Cell(1, i).Value = headers[i - 1];
            }
        }
    }
}