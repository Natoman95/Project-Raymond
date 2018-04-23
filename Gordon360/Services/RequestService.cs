using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Repositories;
using Gordon360.Models;
using Gordon360.Models.ViewModels;

namespace Gordon360.Services
{
    /// <summary>
    /// Service that handles interactions with the database related to the Gordon Transit app's request objects
    /// </summary>
    public class RequestService : IRequestService
    {
        private IUnitOfWork _unitOfWork;

        public RequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public TransitRequestViewModel GetById(int id)
        {
            TransitRequestViewModel resultRequest;
            Transit_Requests request = _unitOfWork.TransitRequestRepository.GetById(id);
            if (request == null)
            {
                resultRequest = null;
            }
            else
            {
                // Convert model to view model
                resultRequest = request;
            }
            return resultRequest;
        }

        public IEnumerable<TransitRequestViewModel> GetByUsername(string username)
        {
            List<TransitRequestViewModel> userRequests = new List<TransitRequestViewModel>();
            IEnumerable<Transit_Requests> requests = _unitOfWork.TransitRequestRepository.Where(m => m.requester_username == username);
            if (requests == null)
            {
                userRequests = null;
            }
            else
            {
                // Convert models to view models
                foreach (Transit_Requests request in requests)
                {
                    TransitRequestViewModel convertedRequest = request;
                    userRequests.Add(convertedRequest);
                }
            }
            return userRequests;
        }

        public void PostRequest(TransitRequestViewModel request)
        {
            // Convert ride view model to ride model for storage in the database
            Transit_Requests requestModel = new Transit_Requests();
            requestModel.transaction_datetime = DateTime.Now;
            requestModel.requester_username = request.requesterUsername;
            requestModel.ride_id = request.rideID;
            requestModel.requester_note = request.requesterNote;
            requestModel.is_confirmed = request.isConfirmed;

            // Add the model as a new row in the database
            _unitOfWork.TransitRequestRepository.Add(requestModel);
            _unitOfWork.Save();
        }

        public void UpdateRide(int requestId, int rideId)
        {
            Transit_Requests request = _unitOfWork.TransitRequestRepository.GetById(requestId);
            if (request != null)
            {
                _unitOfWork.TransitRequestRepository.Attach(request);
                request.ride_id = rideId;
                _unitOfWork.Save();
            }
        }

        public void UpdateConfirmed(int id, bool isConfirmed)
        {
            Transit_Requests request = _unitOfWork.TransitRequestRepository.GetById(id);
            if (request != null)
            {
                _unitOfWork.TransitRequestRepository.Attach(request);
                request.is_confirmed = isConfirmed;
                _unitOfWork.Save();
            }
        }

        public void DeleteRequest(int id)
        {
            Transit_Requests request = _unitOfWork.TransitRequestRepository.GetById(id);
            if (request != null)
            {
                _unitOfWork.TransitRequestRepository.Delete(request);
                _unitOfWork.Save();
            }
        }
    }
}