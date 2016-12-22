using com.wiris.editor.toolbar;
using Esynctraining.NHibernate;
using FluentNHibernate.Utils;
using NHibernate.Criterion;
using NHibernate.Linq;
using PDFAnnotation.Core.Domain.DTO;

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
        /// The get marks by file.
        /// </summary>
        /// <param name="fileId">
        /// The file id.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ATMark}"/>.
        /// </returns>
        public IEnumerable<ATMark> GetMarksByFileOlderThen(Guid fileId, DateTime date)
        {
            ATMark mark = null;
            return this.Repository.Session.QueryOver(() => mark).Where(x => x.File.Id == fileId && x.DateChanged > date).Fetch(x => x.File).Eager.Future<ATMark>().ToList();
        }

        /// <summary>
        /// The get marks by file.
        /// </summary>
        /// <param name="fileId">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ATMark}"/>.
        /// </returns>
        public IEnumerable<ATMark> GetMarksByFile(Guid fileId)
        {
            ATMark mark = null;
            return this.Repository.Session.QueryOver(() => mark).Where(x => x.File.Id == fileId).Fetch(x => x.File).Eager.Future<ATMark>().ToList();
        }


        /// <summary>
        /// The delete all marks from page
        /// </summary>
        /// <param name="fileId">The file id</param>
        /// <param name="pageIndexes"></param>
        public void DeleteAllMarksFromPage(Guid fileId, int[] pageIndexes, File file)
        {
            var result = this.GetMarksByFile(fileId).ToList();
            var marksForDeleting = result.Where(x => pageIndexes.Contains(x.PageIndex));
            var marksForChanging = result.Where(x => !pageIndexes.Contains(x.PageIndex));
            for (int i = 1; i < file.NumberOfPages+1; i++)
            {
                if (pageIndexes.Contains(i))
                {
                        foreach (var mark in marksForDeleting)
                        {
                            this.RegisterDelete(mark);
                        }
                        foreach (var mark in marksForChanging)
                        {
                            if (mark.PageIndex > i)
                            {
                            mark.PageIndex = mark.PageIndex - 1;
                            this.RegisterSave(mark);
                        }

                        }
                }
            }
            this.Flush();
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
        public ObjectsForMarkDTO GetMarksForFile(Guid fileId)
        {
            // trick to make NHibernate to load all child instances in one roundtrip to db (if db supports it)
            var result = new ObjectsForMarkDTO();
      
            // when this is executed, the three queries are executed in one roundtrip
            var results = GetMarks(fileId);
            foreach (var joinedMark in results)
            {
                foreach (var joinedNote in joinedMark.Shapes)
                {
                    result.shapes.Add(joinedNote);
                }
                foreach (var joinedDrawing in joinedMark.Drawings)
                {
                    result.drawings.Add(joinedDrawing);
                }
                foreach (var joinedHighlight in joinedMark.HighlightStrikeOuts)
                {
                    result.atHighlightStrikeOuts.Add(joinedHighlight);
                }
                foreach (var joinedText in joinedMark.TextItems)
                {
                    result.atTextItems.Add(joinedText);
                }
                foreach (var rotation in joinedMark.Rotations)
                {
                    result.atRotations.Add(rotation);
                }
                foreach (var picture in joinedMark.Pictures)
                {
                    result.atPictures.Add(picture);
                }
                foreach (var formula in joinedMark.Formulas)
                {
                    result.atFormulas.Add(formula);
                }
                foreach (var annotation in joinedMark.Annotations)
                {
                    result.atAnnotations.Add(annotation);
                }
            }
            return result;
        }


        public IEnumerable<ATMark> GetMarks(Guid fileId)
        {
            ATMark mark = null;
            ATShape note = null;
            ATDrawing atDrawing = null;
            ATHighlightStrikeOut atHighlightStrikeOut = null;
            ATTextItem textItem = null;
            ATRotation rotationItem = null;
            ATPicture pictureItem = null;
            ATFormula formulaItem = null;
            ATAnnotation annotationItem = null;

            // Query to load the companies   
       //     var marks = this.Repository.Session.QueryOver(() => mark).Where(x => x.File.Id == fileId).Fetch(x => x.File).Eager.Future<ATMark>();



            var mrs = this.Repository.Session.QueryOver<ATMark>()
                .Left.JoinAlias(p => p.Annotations, () => annotationItem)
                .Left.JoinAlias(p => p.Shapes, () => note)
                .Left.JoinAlias(p => p.Drawings, () => atDrawing)
                .Left.JoinAlias(p => p.HighlightStrikeOuts, () => atHighlightStrikeOut)
                .Left.JoinAlias(p => p.TextItems, () => textItem)
                .Left.JoinAlias(p => p.Rotations, () => rotationItem)
                .Left.JoinAlias(p => p.Pictures, () => pictureItem)
                .Left.JoinAlias(p => p.Formulas, () => formulaItem)
                .Where(p => p.File.Id == fileId)
                .Fetch(p => p.File).Eager.Future<ATMark>();
              //  .List();

            //// Query to load the Notes
            //var notes = this.Repository.Session.QueryOver(() => note)
            //    .JoinAlias(p => p.Mark, () => mark)
            //    .Future<ATShape>();

            //// Query to load the Drawings
            //var drawings = this.Repository.Session.QueryOver(() => atDrawing)
            //    .JoinAlias(p => p.Mark, () => mark)
            //    .Future<ATDrawing>();

            //// Query to load the Highlights
            //var highlights = this.Repository.Session.QueryOver(() => atHighlightStrikeOut)
            //    .JoinAlias(p => p.Mark, () => mark)
            //    .Future<ATHighlightStrikeOut>();

            //// Query to load the TextItems
            //var textItems = this.Repository.Session.QueryOver(() => textItem)
            //    .JoinAlias(p => p.Mark, () => mark)
            //    .Future<ATTextItem>();

            //// Query to load the Rotations
            //var rotationItems = this.Repository.Session.QueryOver(() => rotationItem)
            //    .JoinAlias(p => p.Mark, () => mark)
            //    .Future<ATRotation>();

            //// Query to load the Pictures
            //var pictureItems = this.Repository.Session.QueryOver(() => pictureItem)
            //    .JoinAlias(p => p.Mark, () => mark)
            //    .Future<ATPicture>();

            //// Query to load the Formulas
            //var formulaItems = this.Repository.Session.QueryOver(() => formulaItem)
            //    .JoinAlias(p => p.Mark, () => mark)
            //    .Future<ATFormula>();

            //// Query to load the Annotations
            //var annotationItems = this.Repository.Session.QueryOver(() => annotationItem)
            //    .JoinAlias(p => p.Mark, () => mark)
            //    .Future<ATAnnotation>();

            return mrs.OrderBy(x => x.DateChanged).ToList();
        }
#pragma warning restore 168
    }
}
