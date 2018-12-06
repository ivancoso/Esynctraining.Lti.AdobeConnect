using System.Collections.Generic;

namespace Esynctraining.Lti.Lms.Canvas
{
    public class CanvasApiErrorWrapper
    {
        public List<CanvasError> errors { get; set; }
    }

    public class CanvasError
    {
        public string message { get; set; }
    }
}