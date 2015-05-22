namespace EdugameCloud.Lti.Core.Domain.Entities
{
    public sealed class AcRole
    {
        public static readonly AcRole Host = new AcRole { Id = 1, Name = "Host" };
        public static readonly AcRole Presenter = new AcRole { Id = 2, Name = "Presenter" };
        public static readonly AcRole Participant = new AcRole { Id = 3, Name = "Participant" };


        public int Id { get; set; }

        public string Name { get; set; }

    }

}
// 1 = Host; 2 = Presenter; 3 = Participant