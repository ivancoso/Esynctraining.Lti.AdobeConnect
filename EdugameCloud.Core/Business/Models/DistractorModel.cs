using System.Linq;
using EdugameCloud.Core.Domain.DTO;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using NHibernate;

    /// <summary>
    /// The distractor model.
    /// </summary>
    public class DistractorModel : BaseModel<Distractor, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DistractorModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public DistractorModel(IRepository<Distractor, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page items.
        /// </param>
        /// <param name="totalCount">
        /// The total Count.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Distractor}"/>.
        /// </returns>
        public IEnumerable<Distractor> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            var queryOver = new DefaultQueryOver<Distractor, int>().GetQueryOver();
            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            var pagedQuery = queryOver.Fetch(x => x.Image).Eager.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Distractor}"/>.
        /// </returns>
        public override IEnumerable<Distractor> GetAll()
        {
            var defaultQuery = new DefaultQueryOver<Distractor, int>().GetQueryOver().Fetch(x => x.Image).Eager;
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get one by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        public override IFutureValue<Distractor> GetOneById(int id)
        {
            var queryOver = new DefaultQueryOver<Distractor, int>().GetQueryOver().Where(x => x.Id == id).Fetch(x => x.Image).Eager;
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get all by user id and sub module item id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="subModuleItemId">
        /// The sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Distractor}"/>.
        /// </returns>
        public IEnumerable<Distractor> GetAllByUserIdAndSubModuleItemId(int userId, int subModuleItemId)
        {
            var queryOver = new DefaultQueryOver<Distractor, int>()
                .GetQueryOver()
                .Where(x => x.CreatedBy != null && x.CreatedBy.Id == userId)
                .JoinQueryOver(x => x.Question)
                .Where(q => q.SubModuleItem.Id == subModuleItemId)
                .Fetch(x => x.Image).Eager;
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all by user id and question id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="questionId">
        /// The question id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Distractor}"/>.
        /// </returns>
        public IEnumerable<Distractor> GetAllByUserIdAndQuestionId(int userId, int questionId)
        {
            var queryOver = new DefaultQueryOver<Distractor, int>()
                .GetQueryOver()
                .Where(x => x.CreatedBy != null && x.CreatedBy.Id == userId)
                .JoinQueryOver(x => x.Question)
                .Where(q => q.Id == questionId)
                .Fetch(x => x.Image).Eager;
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get one by question id and lms id.
        /// </summary>
        /// <param name="questionId">
        /// The question id.
        /// </param>
        /// <param name="lmsId">
        /// The lms id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Question}"/>.
        /// </returns>
        public IFutureValue<Distractor> GetOneByQuestionIdAndLmsId(int questionId, int lmsId)
        {
            var queryOver = new DefaultQueryOver<Distractor, int>().GetQueryOver().Where(x => x.Question.Id == questionId && x.LmsAnswerId == lmsId).Fetch(x => x.Image).Eager;
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get all by question id.
        /// </summary>
        /// <param name="questionId">
        /// The question id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Distractor}"/>.
        /// </returns>
        public IEnumerable<Distractor> GetAllByQuestionId(int questionId)
        {
            var queryOver = new DefaultQueryOver<Distractor, int>()
                .GetQueryOver()
                .JoinQueryOver(x => x.Question)
                .Where(q => q.Id == questionId)
                .Fetch(x => x.Image).Eager;
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all by questions ids.
        /// </summary>
        /// <param name="questionIds">
        /// The question ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Distractor}"/>.
        /// </returns>
        public IEnumerable<Distractor> GetAllByQuestionsIds(List<int> questionIds)
        {
            var queryOver = new DefaultQueryOver<Distractor, int>()
                .GetQueryOver().Where(x => x.IsActive == true)
                .AndRestrictionOn(x => x.Question.Id).IsIn(questionIds).Fetch(x => x.Image).Eager;
            return this.Repository.FindAll(queryOver);
        }

		  public IEnumerable<DistractorFromStoredProcedureDTO> GetDistractorsBySMIId(int smiId)
	    {
		    Distractor d = null;
		    Question q = null;
		    SubModuleItem smi = null;
		    File f = null;
		    DistractorFromStoredProcedureDTO dto = null;
			var qieryOver = new DefaultQueryOver<Distractor, int>().GetQueryOver(()=>d)
				.JoinQueryOver(x=>x.Question, ()=>q, JoinType.InnerJoin)
				.JoinQueryOver(()=>q.SubModuleItem, ()=>smi, JoinType.InnerJoin)
                .JoinQueryOver(() => d.Image, () => f, JoinType.LeftOuterJoin)
				.Where(()=>q.SubModuleItem.Id == smiId && q.IsActive == true && d.IsActive == true)
				.SelectList(res=>
					res.Select(Projections.Distinct(Projections.ProjectionList()
						.Add(Projections.Property(() => d.Id))
						.Add(Projections.Property(() => d.Question.Id))
						.Add(Projections.Property(() => d.DistractorType))
						.Add(Projections.Property(() => d.DistractorName))
						.Add(Projections.Property(() => d.DistractorOrder))
						.Add(Projections.Property(() => d.IsCorrect))
						.Add(Projections.Property(() => d.Image.Id))
						.Add(Projections.Property(() => f.X))
						.Add(Projections.Property(() => f.Y))
						.Add(Projections.Property(() => f.Height))
						.Add(Projections.Property(() => f.Width))
						.Add(Projections.Property(() => q.Rows))
				        ))
				.Select(()=>d.Id)
				.WithAlias(()=>dto.distractorId)
				.Select(()=>d.Question.Id)
				.WithAlias(()=>dto.questionId)
				.Select(()=>d.DistractorType)
				.WithAlias(()=>dto.distractorType)
				.Select(()=>d.DistractorName)
				.WithAlias(()=>dto.distractor)
				.Select(()=>d.DistractorOrder)
				.WithAlias(()=>dto.distractorOrder)
				.Select(()=>d.IsCorrect)
				.WithAlias(()=>dto.isCorrect)
				.Select(()=>d.Image.Id)
				.WithAlias(()=>dto.imageId)
				.Select(()=>f.X)
				.WithAlias(()=>dto.x)
				.Select(()=>f.Y)
				.WithAlias(()=>dto.y)
				.Select(()=>f.Height)
				.WithAlias(()=>dto.height)
				.Select(()=>f.Width)
				.WithAlias(()=>dto.width)
                .Select(()=>q.Rows)
                .WithAlias(()=>dto.rows)
				).TransformUsing(Transformers.AliasToBean<DistractorFromStoredProcedureDTO>());
			var result = Repository.FindAll<DistractorFromStoredProcedureDTO>(qieryOver).ToList();
			 return result;
	    }
    }
}