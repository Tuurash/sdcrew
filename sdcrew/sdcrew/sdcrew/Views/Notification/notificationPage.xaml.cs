using sdcrew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Notification
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class notificationPage : ContentPage
    {
        public notificationPage()
        {
            InitializeComponent();
        }

        PostFlightVM flight = new PostFlightVM();

        private void Button_Clicked(object sender, EventArgs e)
        {
            
        }

        //public async Task PostOooiAsync()
        //{
        //    //On=> Flight Stop ; Off=>Flight Start  ; In=> blockStart ; Out=> blockstop
        //    if (flight.postedFlightId != 0)
        //    {
        //        //Put method to Update
        //        OooiPostModel oooiPostModel = new OooiPostModel
        //        {
        //            PostedFlightId = flight.postedFlightId,
        //            AircraftProfileId = flight.aircraftProfileId,
        //            ArrivalAirportId = OooiArrivalAirportId,
        //            DepartureAirportId = OooiDepartAirportId,
        //            BlockStartDateTime = DateTime.Parse(lblOooiIn.Text),
        //            BlockStopDateTime = DateTime.Parse(lblOooiOut.Text),
        //            FlightStartDateTime = DateTime.Parse(lblOooiOff.Text),
        //            FlightStopDateTime = DateTime.Parse(lblOooiOn.Text),
        //            ScheduledFlightId = flight.scheduledFlightId,
        //        };

        //        var UpdateResponse = await svm.PutOooiAsync(oooiPostModel, oooiPostModel.PostedFlightId);
        //    }
        //    else
        //    {
        //        //Post and will return PostedFlightId
        //        OooiPostModel oooiPostModel = new OooiPostModel
        //        {
        //            AircraftProfileId = flight.aircraftProfileId,
        //            ArrivalAirportId = OooiArrivalAirportId,
        //            DepartureAirportId = OooiDepartAirportId,
        //            BlockStartDateTime = DateTime.Parse(lblOooiIn.Text),
        //            BlockStopDateTime = DateTime.Parse(lblOooiOut.Text),
        //            FlightStartDateTime = DateTime.Parse(lblOooiOff.Text),
        //            FlightStopDateTime = DateTime.Parse(lblOooiOn.Text),
        //            ScheduledFlightId = flight.scheduledFlightId,
        //        };

        //        GeneratedPostedFlightId = await svm.PostOooiAsync(oooiPostModel);
        //    }
        //}
    }
}