using Esynctraining.Lti.Zoom.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Zoom.Controllers
{
    public partial class LtiController : BaseController
    {

        public virtual async Task<ActionResult> Home()
        {
            ViewData["Message"] = "Your application description page.";
            return this.View("About");
        }

        public virtual async Task<ActionResult> LoginWithProvider(string provider, LtiParamDTO param)
        {
            ViewData["Message"] = "Your application description page.";
            return this.View("~/Views/Home/About.chtml");
        }
    }
}
