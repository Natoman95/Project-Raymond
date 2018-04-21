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
            TransitRideViewModel result;
            Transit_Rides query = _unitOfWork.TransitRideRepository.GetById(id);
            if (query == null)
            {
                result = null;
            }
            else
            {
                // Convert model to view model
                result = query;
            }
            return result;
        }

        public IEnumerable<TransitRideViewModel> GetOfferedByUsername(string username)
        {
            List<TransitRideViewModel> result = new List<TransitRideViewModel>();
            IEnumerable<Transit_Rides> query = _unitOfWork.TransitRideRepository.Where(m => m.driver_username == username);
            if (query == null)
            {
                result = null;
            }
            else
            {
                // Convert models to view models
                foreach (Transit_Rides item in query)
                {
                    TransitRideViewModel convertedItem = item;
                    result.Add(convertedItem);
                }
            }
            return result;
        }

        public IEnumerable<TransitRideViewModel> GetConfirmedByUsername(string username)
        {
            List<TransitRideViewModel> result = new List<TransitRideViewModel>();
            // The passengers on a ride in the database have their ids in a comma-separated string
            // We have to check to see if the id is in that string
            IEnumerable<Transit_Rides> query = _unitOfWork.TransitRideRepository.
                Where(m => m.passenger_usernames.Trim().Contains(username));
            if (query == null)
            {
                result = null;
            }
            else
            {
                // Convert models to view models
                foreach (Transit_Rides item in query)
                {
                    TransitRideViewModel convertedItem = item;
                    result.Add(convertedItem);
                }
            }
            return result;
        }

        public IEnumerable<TransitRideViewModel> GetByLocation(string origin, string destination)
        {
            // This will have to be modified later to include a much more complex algorithm
            List<TransitRideViewModel> result = new List<TransitRideViewModel>();
            IEnumerable<Transit_Rides> query = _unitOfWork.TransitRideRepository.Where(m => m.destination == destination);
            if (query == null)
            {
                result = null;
            }
            else
            {
                // Convert models to view models
                foreach(Transit_Rides item in query)
                {
                    TransitRideViewModel convertedItem = item;
                    result.Add(convertedItem);
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
            rideModel.passenger_usernames = StrListToCommaString(ride.passengerUsernames);
            rideModel.request_ids = IntListToCommaString(ride.requestIds);
            rideModel.max_capacity = ride.maxCapacity;
            rideModel.origin = ride.origin;
            rideModel.destination = ride.destination;
            rideModel.departure_datetime = ride.departureDateTime;
            rideModel.driver_note = ride.driverNote;

            // Add the model as a new row in the database
            _unitOfWork.TransitRideRepository.Add(rideModel);
            _unitOfWork.Save();
        }

        public void UpdatePassengers(int rideId, int passengerId)
        {

        }

        public void UpdateOrigin(int id, string origin)
        {
            Transit_Rides query = _unitOfWork.TransitRideRepository.GetById(id);
            if (query != null)
            {
                _unitOfWork.TransitRideRepository.Attach(query);
                query.origin = origin;
                _unitOfWork.Save();
            }
        }

        public void UpdateDestination(int id, string destination)
        {
            Transit_Rides query = _unitOfWork.TransitRideRepository.GetById(id);
            if (query != null)
            {
                _unitOfWork.TransitRideRepository.Attach(query);
                query.destination = destination;
                _unitOfWork.Save();
            }
        }

        public void UpdateDateTime(int id, DateTime dateTime)
        {
            Transit_Rides query = _unitOfWork.TransitRideRepository.GetById(id);
            if (query != null)
            {
                _unitOfWork.TransitRideRepository.Attach(query);
                query.departure_datetime = dateTime;
                _unitOfWork.Save();
            }
        }

        public void UpdateNote(int id, string note)
        {
            Transit_Rides query = _unitOfWork.TransitRideRepository.GetById(id);
            if (query != null)
            {
                _unitOfWork.TransitRideRepository.Attach(query);
                query.driver_note = note;
                _unitOfWork.Save();
            }
        }

        public void UpdateCapacity(int id, int capacity)
        {
            Transit_Rides query = _unitOfWork.TransitRideRepository.GetById(id);
            if (query != null)
            {
                _unitOfWork.TransitRideRepository.Attach(query);
                query.max_capacity = capacity;
                _unitOfWork.Save();
            }
        }

        public void UpdateRequests(int rideId, int requestId)
        {

        }

        public void DeleteRide(int id)
        {
            Transit_Rides query = _unitOfWork.TransitRideRepository.GetById(id);
            if (query != null)
            {
                _unitOfWork.TransitRideRepository.Delete(query);
                _unitOfWork.Save();
            }
        }

        private string StrListToCommaString(List<string> stringList)
        {
            string commaList = "";
            int index = 1;
            foreach(string stringItem in stringList)
            {
                commaList += stringItem;
                if (stringList.Count > index)
                {
                    commaList += ",";
                }
                index++;
            }
            return commaList;
        }

        private string IntListToCommaString(List<int> intList)
        {
            string commaList = "";
            int index = 1;
            foreach (int intItem in intList)
            {
                commaList += intItem.ToString();
                if (intList.Count > index)
                {
                    commaList += ",";
                }
                index++;
            }
            return commaList;
        }
    }
}