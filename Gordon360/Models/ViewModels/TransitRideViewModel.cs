using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class TransitRideViewModel
    {
        public int rideId { get; set; }
        public string driverUsername { get; set; }
        public List<string> passengerUsernames { get; set; }
        public List<int> requestIds { get; set; }
        public int maxCapacity { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public DateTime departureDateTime { get; set; }
        public string driverNote { get; set; }

        // Convert comma delimmeted list into a list of ints
        private static List<int> CommaStringToIntList(string commaList)
        {
            List<int> intList = new List<int>();
            string[] commaSplits = commaList.Split(',');
            foreach (string strItem in commaSplits) {
                int intItem;
                bool didParse = Int32.TryParse(strItem, out intItem);
                if (didParse)
                {
                    intList.Add(intItem);
                }
            }

            return intList;
        }

        // Convert comma delimmeted list into a list of strings
        private static List<string> CommaStringToStrList(string commaList)
        {
            List<string> stringList = new List<string>();
            if (commaList.Length > 0)
            {
                string[] commaSplits = commaList.Split(',');
                foreach (string commaItem in commaSplits)
                {
                    stringList.Add(commaItem);
                }
            }

            return stringList;
        }

        public static implicit operator TransitRideViewModel(Transit_Rides model)
        {
            TransitRideViewModel viewModel = new TransitRideViewModel
            {
                rideId = model.ride_id,
                driverUsername = model.driver_username,
                passengerUsernames = CommaStringToStrList(model.passenger_usernames),
                requestIds = CommaStringToIntList(model.request_ids),
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