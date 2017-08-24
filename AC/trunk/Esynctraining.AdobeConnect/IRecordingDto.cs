using System.ComponentModel.DataAnnotations;

namespace Esynctraining.AdobeConnect
{
    public interface IRecordingDto
    {
        [Required]
        string Id { get; set; }

        [Required]
        bool IsPublic { get; set; }

        [Required]
        bool Published { get; set; }

        [Required]
        long BeginAt { get; set; }

        string Duration { get; set; }

        string Name { get; set; }
    }

}
