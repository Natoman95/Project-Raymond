using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    /// <summary>
    /// Models request objects belonging to the Gordon transit app
    /// Makes the generated DB model more user friendly and easier for the client to interact with
    /// </summary>
    public class TransitRequestViewModel
    {
        public int requestID { get; set; }
        public string requesterUsername { get; set; }
        public int rideID { get; set; }
        public string requesterNote { get; set; }
        public bool isConfirmed { get; set; }

        public static implicit operator TransitRequestViewModel(Transit_Requests model)
        {
            TransitRequestViewModel viewModel = new TransitRequestViewModel
            {
                requestID = model.request_id,
                requesterUsername = model.requester_username,
                rideID = model.ride_id,
                requesterNote = model.requester_note,
                isConfirmed = model.is_confirmed
            };

            return viewModel;
        }
    }
}