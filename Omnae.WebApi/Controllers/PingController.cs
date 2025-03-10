using System.Linq;
using System.Web.Http;
using Omnae.Data;
using Omnae.WebApi.DTO;

namespace Omnae.WebApi.Controllers
{
    [Route("api")]
    public class PingController : ApiController
    {
        private OmnaeContext OmnaeContext { get; }

        public PingController(OmnaeContext omnaeContext)
        {
            OmnaeContext = omnaeContext;
        }

        // GET api
        public PingPongDTO Get()
        {
            var db = OmnaeContext.Countries.Count();

            return new PingPongDTO
            {
                Name = "Omnae Core API",
            };
        }
    }
}
