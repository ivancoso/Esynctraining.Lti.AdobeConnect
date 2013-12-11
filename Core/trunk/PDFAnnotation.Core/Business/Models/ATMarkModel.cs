namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The mark model.
    /// </summary>
    public class MarkModel : BaseModel<ATMark, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public MarkModel(IRepository<ATMark, Guid> repository)
            : base(repository)
        {
        }

        /// <summary>
        /// The get marks for file.
        /// </summary>
        /// <param name="fileId">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
#pragma warning disable 168
        public Tuple<List<ATShape>, List<ATDrawing>, List<ATHighlightStrikeOut>, List<ATTextItem>> GetMarksForFile(int fileId)
        {
            // trick to make NHibernate to load all child instances in one roundtrip to db (if db supports it)
            var result = new Tuple<List<ATShape>, List<ATDrawing>, List<ATHighlightStrikeOut>, List<ATTextItem>>(new List<ATShape>(), new List<ATDrawing>(), new List<ATHighlightStrikeOut>(), new List<ATTextItem>());
            ATMark mark = null;
            ATShape note = null;
            ATDrawing atDrawing = null;
            ATHighlightStrikeOut atHighlightStrikeOut = null;
            ATTextItem textItem = null;

            // Query to load the companies   
            var marks = this.Repository.Session.QueryOver(() => mark).Where(x => x.File.Id == fileId).Fetch(x => x.File).Eager.Future<ATMark>();

            // Query to load the Partners with the Notes
            var notes = this.Repository.Session.QueryOver(() => note)
                .JoinAlias(p => p.Mark, () => mark)
                .Future<ATShape>();

            // Query to load the Partners with the Notes
            var drawings = this.Repository.Session.QueryOver(() => atDrawing)
                .JoinAlias(p => p.Mark, () => mark)
                .Future<ATDrawing>();

            // Query to load the Partners with the Notes
            var highlights = this.Repository.Session.QueryOver(() => atHighlightStrikeOut)
                .JoinAlias(p => p.Mark, () => mark)
                .Future<ATHighlightStrikeOut>();

            // Query to load the Partners with the Notes
            var textItems = this.Repository.Session.QueryOver(() => textItem)
                .JoinAlias(p => p.Mark, () => mark)
                .Future<ATTextItem>();

            // when this is executed, the three queries are executed in one roundtrip
            var results = marks.ToList();
            foreach (var joinedMark in results)
            {
                foreach (var joinedNote in joinedMark.Shapes)
                {
                    result.Item1.Add(joinedNote);
                }
                foreach (var joinedDrawing in joinedMark.Drawings)
                {
                    result.Item2.Add(joinedDrawing);
                }
                foreach (var joinedHighlight in joinedMark.HighlightStrikeOuts)
                {
                    result.Item3.Add(joinedHighlight);
                }
                foreach (var joinedText in joinedMark.TextItems)
                {
                    result.Item4.Add(joinedText);
                }
            }
            return result;
        }
#pragma warning restore 168
    }
}
