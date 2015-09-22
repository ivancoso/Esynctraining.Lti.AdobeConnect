namespace EdugameCloud.Core.Domain.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The question for.
    /// </summary>
    public class QuestionFor : Entity 
    {
        #region Fields

        /// <summary>
        ///     The questionType.
        /// </summary>
        private IEnumerable<QuestionTypeEnum> questionTypes;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionFor"/> class. 
        /// </summary>
        /// <param name="types">
        /// The types.
        /// </param>
        public QuestionFor(params QuestionTypeEnum[] types)
        {
            this.questionTypes = types.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionFor"/> class. 
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public QuestionFor(IEnumerable<QuestionTypeEnum> instance)
        {
            this.questionTypes = instance;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the correct message.
        /// </summary>
        public virtual Question Question { get; set; }

        /// <summary>
        /// Gets or sets the questionType.
        /// </summary>
        public virtual IEnumerable<QuestionTypeEnum> QuestionTypes
        {
            get
            {
                return this.questionTypes;
            }

            set
            {
                this.questionTypes = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is mandatory.
        /// </summary>
        public virtual bool IsMandatory { get; set; }

        /// <summary>
        /// Gets or sets the room number.
        /// </summary>
        public virtual int? PageNumber { get; set; }
        
        #endregion
    }
}