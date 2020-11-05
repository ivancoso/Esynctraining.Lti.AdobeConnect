namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The state DTO.
    /// </summary>
    [DataContract]
    [Serializable]
    public class StateDTO
    {
        public StateDTO() { }

        public StateDTO(State s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            stateId = s.Id;
            stateCode = s.StateCode;
            stateName = s.StateName;
            isActive = s.IsActive;
            countryId = s.Country.With(x => x.Id);
        }


        [DataMember]
        public int stateId { get; set; }

        [DataMember]
        public string stateName { get; set; }

        [DataMember]
        public string stateCode { get; set; }

        [DataMember]
        public bool isActive { get; set; }

        [DataMember]
        public int? countryId { get; set; }

    }

}