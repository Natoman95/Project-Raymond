using System;
using System.Collections.Generic;
using System.Web.Http;
using Gordon360.Services;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;

namespace Gordon360.Controllers.Api
{
    /// <summary>
    /// Controller that handles web requests related to ride objects belonging to the Gordon transit app
    /// </summary>
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/transit/ride")]
    public class RideController : ApiController

    {
        private IRideService _rideService;

        public RideController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _rideService = new RideService(_unitOfWork);
        }

        // Get a ride by its unique id
        [HttpGet]
        [Route("id/{id}")]
        public IHttpActionResult GetById(int id)
        {
            TransitRideViewModel ride = _rideService.GetById(id);
            if (ride != null)
            {
                return Ok(ride);
            }
            else
            {
                return NotFound();
            }
        }

        // Get the offered rides that belong to a user
        [HttpGet]
        [Route("user/{username}/offered")]
        public IHttpActionResult GetOfferedByUserName(string username)
        {
            IEnumerable<TransitRideViewModel> rides = _rideService.GetOfferedByUsername(username);
            return Ok(rides);
        }

        // Get the confirmed rides that belong to a user
        [HttpGet]
        [Route("user/{username}/confirmed")]
        public IHttpActionResult GetConfirmedByUsername(string username)
        {
            IEnumerable<TransitRideViewModel> rides = _rideService.GetConfirmedByUsername(username);
            return Ok(rides);
        }

        // Find suitable rides based on desired origin and destination
        [HttpGet]
        [Route("location/{origin}/{destination}")]
        public IHttpActionResult GetByLocation(string origin, string destination)
        {
            IEnumerable<TransitRideViewModel> rides = _rideService.GetByLocation(origin, destination);
            if (rides != null)
            {
                return Ok(rides);
            }
            else
            {
                return NotFound();
            }
        }

        // Add a new ride to the database
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostRide([FromBody] TransitRideViewModel ride)
        {
            _rideService.PostRide(ride);
            return Ok(ride);
        }

        [HttpPut]
        [Route("passengers/{rideid}/{passengerid}")]
        public IHttpActionResult UpdatePassengers(int rideid, int passengerid)
        {
            _rideService.UpdatePassengers(rideid, passengerid);
            return Ok();
        }

        [HttpPut]
        [Route("origin/{id}/{origin}")]
        public IHttpActionResult UpdateOrigin(int id, string origin)
        {
            _rideService.UpdateOrigin(id, origin);
            return Ok();
        }

        [HttpPut]
        [Route("destination/{id}/{destination}")]
        public IHttpActionResult UpdateDestination(int id, string destination)
        {
            _rideService.UpdateDestination(id, destination);
            return Ok();
        }

        [HttpPut]
        [Route("date/{id}/{dateTime}")]
        public IHttpActionResult UpdateDateTime(int id, DateTime dateTime)
        {
            _rideService.UpdateDateTime(id, dateTime);
            return Ok();
        }

        [HttpPut]
        [Route("note/{id}/{note}")]
        public IHttpActionResult UpdateNote(int id, string note)
        {
            _rideService.UpdateNote(id, note);
            return Ok();
        }

        [HttpPut]
        [Route("capacity/{id}/{capacity}")]
        public IHttpActionResult UpdateCapacity(int id, int capacity)
        {
            _rideService.UpdateCapacity(id, capacity);
            return Ok();
        }

        [HttpPut]
        [Route("requests/{rideid}/{requestid}")]
        public IHttpActionResult UpdateRequests(int rideid, int requestid)
        {
            _rideService.UpdateRequests(rideid, requestid);
            return Ok();
        }

        // Finds a ride by id and deletes it
        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult DeleteRide(int id)
        {
            _rideService.DeleteRide(id);
            return Ok();
        }
    }
}