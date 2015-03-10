namespace EdugameCloud.MVC.ViewModels
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    using BaseController = EdugameCloud.MVC.Controllers.BaseController;

    /// <summary>
    /// The entity view model.
    /// </summary>
    /// <typeparam name="T">
    /// The entity
    /// </typeparam>
    /// <typeparam name="TId">
    /// The entity id
    /// </typeparam>
    public abstract class EntityViewModel<T, TId> : BaseViewModel
        where T : IEntity<TId> 
        where TId : struct 
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityViewModel{T, TId}"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected EntityViewModel(BaseController controller, T entity)
            : base(controller)
        {
            this.SetEntity(entity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityViewModel{T,TId}"/> class. 
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected EntityViewModel(BaseController controller, int? page, T entity)
            : base(controller, page)
        {
            this.SetEntity(entity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityViewModel{T, TId}"/> class.
        /// </summary>
        protected EntityViewModel()
        {
            this.SetEntity(default(T));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public TId Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether is transient.
        /// </summary>
        public virtual bool IsTransient
        {
            get
            {
                return this.Id.Equals(default(TId));
            }
        }

        /// <summary>
        /// Gets or sets the view model id.
        /// </summary>
        public Guid ViewModelId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The set entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        private void SetEntity(T entity)
        {
            this.Id = entity != null ? entity.Id : default(TId);
            this.ViewModelId = Guid.NewGuid();
        }

        #endregion
    }
}