namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Business.Queries;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    ///     The distractor model.
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
    }
}