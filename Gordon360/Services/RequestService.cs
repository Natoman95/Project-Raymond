using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Repositories;
using Gordon360.Models;
using Gordon360.Models.ViewModels;

namespace Gordon360.Services
{
    public class RequestService : IRequestService
    {
        private IUnitOfWork _unitOfWork;

        public RequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public TransitRequestViewModel GetById(int id)
        {
            TransitRequestViewModel result;
            Transit_Requests query = _unitOfWork.TransitRequestRepository.GetById(id);
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

        public IEnumerable<TransitRequestViewModel> GetByUsername(string username)
        {
            List<TransitRequestViewModel> result = new List<TransitRequestViewModel>();
            IEnumerable<Transit_Requests> query = _unitOfWork.TransitRequestRepository.Where(m => m.requester_username == username);
            if (query == null)
            {
                result = null;
            }
            else
            {
                // Convert models to view models
                foreach (Transit_Requests item in query)
                {
                    TransitRequestViewModel convertedItem = item;
                    result.Add(convertedItem);
                }
            }
            return result;
        }

        public void PostRequest(TransitRequestViewModel request)
        {
            // Convert ride view model to ride model for storage in the database
            Transit_Requests requestModel = new Transit_Requests();
            requestModel.transaction_datetime = DateTime.Now;
            requestModel.requester_username = request.requesterUsername;
            requestModel.ride_id = request.rideId;
            requestModel.origin = request.origin;
            requestModel.destination = request.destination;
            requestModel.earliest_departure_datetime = request.earliestDepartureDateTime;
            requestModel.latest_departure_datetime = request.latestDepartureDateTime;
            requestModel.requester_note = request.requesterNote;

            // Add the model as a new row in the database
            _unitOfWork.TransitRequestRepository.Add(requestModel);
            _unitOfWork.Save();
        }

        public void UpdateRide(int requestId, int rideId)
        {
            
        }

        public void UpdateOrigin(int id, string origin)
        {
            
        }

        public void UpdateDestination(int id, string destination)
        {
            
        }

        public void UpdateEarliestDateTime(int id, DateTime dateTime)
        {
            
        }

        public void UpdateLatestDateTime(int id, DateTime dateTime)
        {
            
        }

        public void DeleteRequest(int id)
        {
            Transit_Requests query = _unitOfWork.TransitRequestRepository.GetById(id);
            if (query != null)
            {
                _unitOfWork.TransitRequestRepository.Delete(query);
                _unitOfWork.Save();
            }
        }
    }
}