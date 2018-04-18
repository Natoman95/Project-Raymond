﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class TransitRequestViewModel
    {
        public int requestId { get; set; }
        public string requesterUsername { get; set; }
        public Nullable<int> rideId { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public DateTime earliestDepartureDateTime { get; set; }
        public DateTime latestDepartureDateTime { get; set; }
        public string requesterNote { get; set; }

        public static implicit operator TransitRequestViewModel(Transit_Requests model)
        {
            TransitRequestViewModel viewModel = new TransitRequestViewModel
            {
                requestId = model.request_id,
                requesterUsername = model.requester_username,
                rideId = model.ride_id,
                origin = model.origin,
                destination = model.destination,
                earliestDepartureDateTime = model.earliest_departure_datetime,
                latestDepartureDateTime = model.latest_departure_datetime,
                requesterNote = model.requester_note
            };

            return viewModel;
        }
    }
}