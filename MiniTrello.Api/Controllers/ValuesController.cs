using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;
using AttributeRouting.Web.Http;

namespace MiniTrello.Api.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [GET("values")]
        public IEnumerable<string> Get()
        {
            return new[] {"value1", "value2"};
        }

        // GET api/values/5
        [GET("values/{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [POST("values")]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [PUT("values/{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [DELETE("values")]
        public void Delete(int id)
        {
        }
    }
}