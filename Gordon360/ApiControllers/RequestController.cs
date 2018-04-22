using System;
using System.Web.Http;
using Gordon360.Services;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using System.Collections.Generic;

namespace Gordon360.Controllers.Api
{
    /// <summary>
    /// Controller that handles web requests related to request objects belonging to the Gordon transit app
    /// </summary>
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/transit/request")]
    public class RequestController: ApiController

    {
        private static IRequestService _requestService;

        public RequestController()
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            _requestService = new RequestService(unitOfWork);
        }

        // Get a request by its unique id
        [HttpGet]
        [Route("id/{id}")]
        public IHttpActionResult GetById(int id)
        {
            TransitRequestViewModel request = _requestService.GetById(id);
            return Ok(request);
        }

        // Get the requests belonging to a user
        [HttpGet]
        [Route("user/{username}")]
        public IHttpActionResult GetByUsername(string username)
        {
            IEnumerable<TransitRequestViewModel> requests = _requestService.GetByUsername(username);
            return Ok(requests);
        }

        // Add a new request to the database
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostRequest([FromBody] TransitRequestViewModel request)
        {
            _requestService.PostRequest(request);
            return Ok(request);
        }

        [HttpPut]
        [Route("ride/{requestid}/{rideid}")]
        public IHttpActionResult UpdateRide(int requestid, int rideid)
        {
            _requestService.UpdateRide(requestid, rideid);
            return Ok();
        }

        [HttpPut]
        [Route("origin/{id}/{origin}")]
        public IHttpActionResult UpdateOrigin(int id, string origin)
        {
            _requestService.UpdateOrigin(id, origin);
            return Ok();
        }

        [HttpPut]
        [Route("destination/{id}/{destination}")]
        public IHttpActionResult UpdateDestination(int id, string destination)
        {
            _requestService.UpdateDestination(id, destination);
            return Ok();
        }

        [HttpPut]
        [Route("earliest/{id}/{dateTime}")]
        public IHttpActionResult UpdateEarliestDateTime(int id, DateTime dateTime)
        {
            _requestService.UpdateEarliestDateTime(id, dateTime);
            return Ok();
        }

        [HttpPut]
        [Route("latest/{id}/{dateTime}")]
        public IHttpActionResult UpdateLatestDateTime(int id, DateTime dateTime)
        {
            _requestService.UpdateLatestDateTime(id, dateTime);
            return Ok();
        }

        // Find a request by its unique id and delete it
        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult DeleteRequest(int id)
        {
            _requestService.DeleteRequest(id);
            return Ok();
        }
    }
}