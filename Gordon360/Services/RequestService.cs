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

        public TransitRequestViewModel UpdateConfirmed(int id, bool isConfirmed)
        {
            TransitRequestViewModel result = new TransitRequestViewModel();
            Transit_Requests request = _unitOfWork.TransitRequestRepository.GetById(id);
            if (request != null)
            {
                _unitOfWork.TransitRequestRepository.Attach(request);
                request.is_confirmed = isConfirmed;
                _unitOfWork.Save();
                result = request;
            }

            return result;
        }

        public TransitRequestViewModel DeleteRequest(int id)
        {
            TransitRequestViewModel result = new TransitRequestViewModel();
            Transit_Requests request = _unitOfWork.TransitRequestRepository.GetById(id);
            if (request != null)
            {
                result = request;
                _unitOfWork.TransitRequestRepository.Delete(request);
                _unitOfWork.Save();
            }

            return request;
        }
    }
}