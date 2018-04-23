using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    /// <summary>
    /// Models ride objects belonging to the Gordon transit app
    /// Makes the generated DB model more user friendly and easier for the client to interact with
    /// </summary>
    public class TransitRideViewModel
    {
        public int rideID { get; set; }
        public string driverUsername { get; set; }
        public List<string> passengerUsernames { get; set; }
        public List<int> requestIDs { get; set; }
        public int maxCapacity { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public DateTime departureDateTime { get; set; }
        public string driverNote { get; set; }

        public static implicit operator TransitRideViewModel(Transit_Rides model)
        {
            TransitRideViewModel viewModel = new TransitRideViewModel
            {
                rideID = model.ride_id,
                driverUsername = model.driver_username,
                maxCapacity = model.max_capacity,
                origin = model.origin,
                destination = model.destination,
                departureDateTime = model.departure_datetime,
                driverNote = model.driver_note
            };

            return viewModel;
        }
    }
}