//using System.Runtime.Serialization;
//using EdugameCloud.Core.Domain.Entities;

//namespace EdugameCloud.Core.Domain.DTO
//{
//    [DataContract]
//    public class SchoolDTO
//    {
//        public SchoolDTO(School school)
//        {
//            SchoolId = school.Id;
//            StateId = school.StateId;
//            State = new StateDTO(school.State);
//            AccountName = school.AccountName;
//            SchoolNumber = school.SchoolNumber;
//            SchoolEmail = school.SchoolEmail;
//            CorporateName = school.CorporateName;
//            AdvRepresentative = school.AdvRepresentative;
//            ESSRepresentative = school.ESSRepresentative;
//            FBCRepresentative = school.FBCRepresentative;
//            MktgRepresentative = school.MktgRepresentative;
//            StandardsRepresentative = school.StandardsRepresentative;
//            Fax = school.Fax;
//            MainPhone = school.MainPhone;
//            OnsiteOperator = school.OnsiteOperator;
//            FirstDirector = school.FirstDirector;
//            SpeedDialNumber = school.SpeedDialNumber;
//        }

//        [DataMember]
//        public int SchoolId { get; set; }
//        [DataMember]
//        public string AccountName { get; set; }
//        //public string State { get; set; }
//        [DataMember]
//        public int StateId { get; set; }
//        [DataMember]
//        public StateDTO State { get; set; }
//        [DataMember]
//        public string SchoolNumber { get; set; }
//        [DataMember]
//        public string OnsiteOperator { get; set; }
//        [DataMember]
//        public string FirstDirector { get; set; }
//        [DataMember]
//        public string MainPhone { get; set; }
//        [DataMember]
//        public string Fax { get; set; }
//        [DataMember]
//        public string SpeedDialNumber { get; set; }
//        [DataMember]
//        public string SchoolEmail { get; set; }
//        [DataMember]
//        public string CorporateName { get; set; }
//        [DataMember]
//        public string FBCRepresentative { get; set; }
//        [DataMember]
//        public string ESSRepresentative { get; set; }
//        [DataMember]
//        public string StandardsRepresentative { get; set; }
//        [DataMember]
//        public string AdvRepresentative { get; set; }
//        [DataMember]
//        public string MktgRepresentative { get; set; }
//    }
//}