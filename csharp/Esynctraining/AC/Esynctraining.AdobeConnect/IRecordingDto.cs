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

        /// <summary>
        /// NOTE: can have null value, if recording is still online.
        /// </summary>
        string Name { get; set; }

    }

}
