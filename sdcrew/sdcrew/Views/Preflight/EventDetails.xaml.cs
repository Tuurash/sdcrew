using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using sdcrew.Models;
using sdcrew.Services.Data;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class EventDetails : ContentPage
    {
        dynamic flight = new PreflightVM();

        PreflightServices svm;

        string getStaffnote="";

        public EventDetails(dynamic flightObj,string StaffNote)
        {
            svm = new PreflightServices();
            flight = flightObj;
            getStaffnote = StaffNote;

            InitializeComponent(); 
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Run(() => FillEventDetails());
        }

        private void FillEventDetails()
        {
            lblEventHeader.Text = flight.eventName;

            if (flight.eventName == "Staff")
            {
                lblAircraft.Text = "";
                lblEventType.Text = flight.tailNumber;
            }
            else
            {
                lblAircraft.Text = flight.tailNumber;
                lblEventType.Text = flight.eventName;
            }

            
            
            lblLocation.Text = flight.departureAirportIcao;
            lblApprover.Text = "";
            lblPlanner.Text = "";
            lblRequester.Text = "";
            lblNotes.Text = getStaffnote;
            lblStart.Text = flight.StartTime.ToString("dddd dd MMM yyyy");
            lblEnd.Text = flight.EndTime.ToString("dddd dd MMM yyyy");

        }

        async void btnBack_Tapped(System.Object sender, System.EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);
            //Application.Current.MainPage.Navigation.PopAsync();
            await Navigation.PopAsync();
        }
    }

}
