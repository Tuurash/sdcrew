using System;
using System.Collections.Generic;
using sdcrew.Models;
using sdcrew.Services.Data;
using Xamarin.Forms;

namespace sdcrew.Views.Preflight
{

    public partial class EventDetails : ContentPage
    {

        dynamic flight = new PreflightVM();

        PreflightServices svm;

        public EventDetails(dynamic flightObj)
        {
            svm = new PreflightServices();
            flight = flightObj;

            InitializeComponent();

            FillEventDetails();
        }

        private void FillEventDetails()
        {
            lblAircraft.Text = flight.tailNumber;
            lblEventType.Text = flight.eventName;
            lblLocation.Text = flight.departureAirportIcao;
            lblApprover.Text = "";
            lblPlanner.Text = "";
            lblRequester.Text = "";
            lblNotes.Text = flight.notes;
            lblStart.Text = flight.StartTime.ToString("dddd dd MMM yyyy");
            lblEnd.Text = flight.EndTime.ToString("dddd dd MMM yyyy");

        }

        void btnBack_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }

}
