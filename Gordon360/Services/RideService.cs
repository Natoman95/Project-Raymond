using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Repositories;
using Gordon360.Models.ViewModels;
using Gordon360.Models;

namespace Gordon360.Services
{
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
                // The passengers and requests have to be retrieved via another query
                List<string> passengers = new List<string>();
                List<int> pendingRequests = new List<int>();
                GetRequests(query.ride_id, passengers, pendingRequests);
                convertedRide.passengerUsernames = passengers;
                convertedRide.requestIDs = pendingRequests;

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
                    // The passengers and requests have to be retrieved via another query
                    List<string> passengers = new List<string>();
                    List<int> pendingRequests = new List<int>();
                    GetRequests(ride.ride_id, passengers, pendingRequests);
                    convertedRide.passengerUsernames = passengers;
                    convertedRide.requestIDs = pendingRequests;

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

        public IEnumerable<TransitRideViewModel> GetByLocation(string origin, string destination)
        {
            // This will have to be modified later to include a much more complex algorithm
            List<TransitRideViewModel> result = new List<TransitRideViewModel>();
            IEnumerable<Transit_Rides> rides = _unitOfWork.TransitRideRepository.Where(m => m.destination == destination);
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
                    // The passengers and requests have to be retrieved via another query
                    List<string> passengers = new List<string>();
                    List<int> pendingRequests = new List<int>();
                    GetRequests(ride.ride_id, passengers, pendingRequests);
                    convertedRide.passengerUsernames = passengers;
                    convertedRide.requestIDs = pendingRequests;

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

        public void DeleteRide(int id)
        {
            Transit_Rides ride = _unitOfWork.TransitRideRepository.GetById(id);
            if (ride != null)
            {
                _unitOfWork.TransitRideRepository.Delete(ride);
                _unitOfWork.Save();
            }
        }

        private void GetRequests(int rideId, List<string> passengers, List<int> pendingRequests)
        {
            IEnumerable<Transit_Requests> requests = _unitOfWork.TransitRequestRepository.
                Where(m => m.ride_id == rideId);
            if (requests != null)
            {
                foreach (Transit_Requests request in requests)
                {
                    if (request.is_confirmed == true)
                    {
                        passengers.Add(request.requester_username);
                    }
                    else
                    {
                        pendingRequests.Add(request.request_id);
                    }
                }
            }
        }
    }
}