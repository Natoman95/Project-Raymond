using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Repositories;
using Gordon360.Models.ViewModels;
using Gordon360.Models;

namespace Gordon360.Services
{
    /// <summary>
    /// Service that handles interactions with the database related to the Gordon Transit app's ride objects
    /// </summary>
    public class RideService : IRideService
    {
        private IUnitOfWork _unitOfWork;

        public RideService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public TransitRideViewModel GetById(int id)
        {
            TransitRideViewModel ride;
            Transit_Rides query = _unitOfWork.TransitRideRepository.GetById(id);
            if (query == null)
            {
                ride = null;
            }
            else
            {
                // Implicitly convert most values
                TransitRideViewModel convertedRide = query;
                // The requests have to be retrieved via another query
                convertedRide.requests = GetRequests(query.ride_id);

                ride = convertedRide;
            }

            return ride;
        }

        public IEnumerable<TransitRideViewModel> GetOfferedByUsername(string username)
        {
            List<TransitRideViewModel> offeredRides = new List<TransitRideViewModel>();
            IEnumerable<Transit_Rides> rides = _unitOfWork.TransitRideRepository.
                Where(m => m.driver_username == username);
            if (rides == null)
            {
                offeredRides = null;
            }
            else
            {
                // Convert models to view models
                foreach (Transit_Rides ride in rides)
                {
                    // Implicitly convert most values
                    TransitRideViewModel convertedRide = ride;
                    // The requests have to be retrieved via another query
                    convertedRide.requests = GetRequests(ride.ride_id);

                    offeredRides.Add(convertedRide);
                }
            }
            return offeredRides;
        }

        public IEnumerable<TransitRideViewModel> GetConfirmedByUsername(string username)
        {
            List<TransitRideViewModel> rides = new List<TransitRideViewModel>();
            // Get the requests belonging to the user that have been confirmed
            IEnumerable<Transit_Requests> requests = _unitOfWork.TransitRequestRepository.
                Where(m => m.requester_username == username && m.is_confirmed == true);
            if (requests == null)
            {
                rides = null;
            }
            else
            {
                {
                    // Get the rides this user is a confirmed passenger on
                    foreach (Transit_Requests request in requests)
                    {
                        TransitRideViewModel ride = GetById(request.ride_id);
                        rides.Add(ride);
                    }
                }
            }
            return rides;
        }

        public IEnumerable<TransitRideViewModel> GetPendingByUsername(string username)
        {
            List<TransitRideViewModel> rides = new List<TransitRideViewModel>();
            // Get the requests belonging to the user that have not been confirmed
            IEnumerable<Transit_Requests> requests = _unitOfWork.TransitRequestRepository.
                Where(m => m.requester_username == username && m.is_confirmed == false);
            if (requests == null)
            {
                rides = null;
            }
            else
            {
                {
                    // Get the rides this user is a pending passenger on
                    foreach (Transit_Requests request in requests)
                    {
                        TransitRideViewModel ride = GetById(request.ride_id);
                        rides.Add(ride);
                    }
                }
            }
            return rides;
        }

        public IEnumerable<TransitRideViewModel> GetByLocation(
            string origin, string destination, DateTime startDate, DateTime endDate)
        {
            // This will have to be modified later to include a much more complex algorithm
            List<TransitRideViewModel> result = new List<TransitRideViewModel>();
            // Search conditions:
            // The ride's departure time is within the desired startDate and endDate
            // The desired origin is contained in the ride's origin string or vice versa
            // The desired destination is contained in the ride's destination or vice versa
            IEnumerable<Transit_Rides> rides = _unitOfWork.TransitRideRepository.
                Where(m => (m.destination.ToLower().Contains(destination.ToLower()) || destination.ToLower().Contains(m.destination.ToLower()))
                && (m.origin.ToLower().Contains(origin) || origin.Contains(m.origin.ToLower()))
                && m.departure_datetime >= startDate
                && m.departure_datetime <= endDate);
            if (rides == null)
            {
                result = null;
            }
            else
            {
                // Convert models to view models
                foreach(Transit_Rides ride in rides)
                {
                    // Implicitly convert most values
                    TransitRideViewModel convertedRide = ride;
                    // The requests have to be retrieved via another query
                    convertedRide.requests = GetRequests(ride.ride_id);

                    result.Add(convertedRide);
                }
            }
            return result;
        }

        public void PostRide(TransitRideViewModel ride)
        {
            // Convert ride view model to ride model for storage in the database
            Transit_Rides rideModel = new Transit_Rides();
            rideModel.transaction_datetime = DateTime.Now;
            rideModel.driver_username = ride.driverUsername;
            rideModel.max_capacity = ride.maxCapacity;
            rideModel.origin = ride.origin;
            rideModel.destination = ride.destination;
            rideModel.departure_datetime = ride.departureDateTime;
            rideModel.driver_note = ride.driverNote;

            // Add the model as a new row in the database
            _unitOfWork.TransitRideRepository.Add(rideModel);
            _unitOfWork.Save();
        }

        public void UpdateOrigin(int id, string origin)
        {
            Transit_Rides ride = _unitOfWork.TransitRideRepository.GetById(id);
            if (ride != null)
            {
                _unitOfWork.TransitRideRepository.Attach(ride);
                ride.origin = origin;
                _unitOfWork.Save();
            }
        }

        public void UpdateDestination(int id, string destination)
        {
            Transit_Rides ride = _unitOfWork.TransitRideRepository.GetById(id);
            if (ride != null)
            {
                _unitOfWork.TransitRideRepository.Attach(ride);
                ride.destination = destination;
                _unitOfWork.Save();
            }
        }

        public void UpdateDateTime(int id, DateTime dateTime)
        {
            Transit_Rides ride = _unitOfWork.TransitRideRepository.GetById(id);
            if (ride != null)
            {
                _unitOfWork.TransitRideRepository.Attach(ride);
                ride.departure_datetime = dateTime;
                _unitOfWork.Save();
            }
        }

        public void UpdateNote(int id, string note)
        {
            Transit_Rides ride = _unitOfWork.TransitRideRepository.GetById(id);
            if (ride != null)
            {
                _unitOfWork.TransitRideRepository.Attach(ride);
                ride.driver_note = note;
                _unitOfWork.Save();
            }
        }

        public void UpdateCapacity(int id, int capacity)
        {
            Transit_Rides ride = _unitOfWork.TransitRideRepository.GetById(id);
            if (ride != null)
            {
                _unitOfWork.TransitRideRepository.Attach(ride);
                ride.max_capacity = capacity;
                _unitOfWork.Save();
            }
        }

        public TransitRideViewModel DeleteRide(int id)
        {
            TransitRideViewModel result = new TransitRideViewModel();
            Transit_Rides ride = _unitOfWork.TransitRideRepository.GetById(id);
            if (ride != null)
            {
                result = ride;
                _unitOfWork.TransitRideRepository.Delete(ride);
                _unitOfWork.Save();
            }

            return result;
        }

        private List<TransitRequestViewModel> GetRequests(int rideId)
        {
            List<TransitRequestViewModel> requestViews = new List<TransitRequestViewModel>();
            IEnumerable<Transit_Requests> requests = _unitOfWork.TransitRequestRepository.
                Where(m => m.ride_id == rideId);
            if (requests != null)
            {
                foreach (Transit_Requests request in requests)
                {
                    requestViews.Add(request);
                }
            }

            return requestViews;
        }
    }
}