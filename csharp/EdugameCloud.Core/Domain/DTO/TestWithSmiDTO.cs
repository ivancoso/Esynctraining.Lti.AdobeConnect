namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    [DataContract]
    [KnownType(typeof(SubModuleItemDTO))]
    public class TestWithSmiDTO : TestDTO
    {
        #region Constructors and Destructors

        public TestWithSmiDTO()
        {
        }

        public TestWithSmiDTO(Test result, SubModuleItem smi = null)
            : base(result)
        {
            this.SmiVO = new SubModuleItemDTO(smi.Return(x => x, result.SubModuleItem));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the SMI.
        /// </summary>
        [DataMember]
        public SubModuleItemDTO SmiVO { get; set; }

        #endregion
    }
}