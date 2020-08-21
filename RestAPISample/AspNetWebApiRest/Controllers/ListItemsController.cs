using AspNetWebApiRest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AspNetWebApiRest.Controllers
{
    [Authorize]
    public class ListItemsController : ApiController
    {
        private static readonly List<CustomListItem> _listItems = new List<CustomListItem>();
        // GET api/<controller>
        public IEnumerable<CustomListItem> Get()
        {
            return _listItems;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            var item = _listItems.FirstOrDefault(x => x.Id == id);
            if(item != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]NewCustomListItem model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var maxId = 0;
            if(_listItems.Count > 0)
            {
                maxId = _listItems.Max(x => x.Id);
            }
            var item = new CustomListItem
            {
                Id = maxId + 1,
                Text = model.Text
            };
            _listItems.Add(item);
            return Request.CreateResponse(HttpStatusCode.Created, item);
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody]CustomListItem model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var item = _listItems.FirstOrDefault(x => x.Id == model.Id);
            if (item != null)
            {
                item.Text = model.Text;
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(int id)
        {
            var item = _listItems.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                _listItems.Remove(item);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}