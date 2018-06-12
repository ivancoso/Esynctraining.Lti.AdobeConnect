using System.Collections.Generic;

namespace Esynctraining.Lti.Lms.Common.Dto.Moodle
{
    /// <summary>
    /// Moodle dataset
    /// </summary>
    public class MoodleDataset
    {
        /// <summary>
        /// The id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The min 
        /// </summary>
        public string Min { get; set; }

        /// <summary>
        /// The max
        /// </summary>
        public string Max { get; set; }

        /// <summary>
        /// The moodle dataset items
        /// </summary>
        public List<MoodleDataSetItem> Items { get; set; }
    }
}
