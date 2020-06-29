using System.ComponentModel.DataAnnotations;

namespace Esynctraining.Lti.Zoom.Domain
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

    }
}