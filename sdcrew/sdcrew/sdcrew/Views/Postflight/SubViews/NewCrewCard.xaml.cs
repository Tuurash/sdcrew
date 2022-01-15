using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using sdcrew.Models;
using sdcrew.Services.Data;

using Xamarin.Forms;

namespace sdcrew.Views.Postflight.SubViews
{
    public partial class NewCrewCard : ContentView
    {
        FlightCrewMember flightCrewMember;
        PostflightServices postflightServices;

        int ParentGridRow = 0;

        public NewCrewCard(int crewGridRow)
        {
            InitializeComponent();
            postflightServices = new PostflightServices();

            ParentGridRow = crewGridRow;
        }

        public NewCrewCard(FlightCrewMember crew, int crewGridRow)
        {
            flightCrewMember = crew;
            InitializeComponent();

            ParentGridRow = crewGridRow;

            if (flightCrewMember != null)
            {
                FillCrewInfo();
            }
        }

        private async void FillCrewInfo()
        {
            lblCrewMemberType.Text = flightCrewMember.crewMemberType;
            lblCrewName.Text = flightCrewMember.lastName + ", " + flightCrewMember.firstName;
            //lblInstrument,lblNight

            var logbook = await postflightServices.GetLogbookAsync(flightCrewMember.FlightCrewMemberID);

            lblDayTakeoffCount.Text = logbook.dayTakeoffs.ToString();
            lblNightTakeoffCount.Text = logbook.nightTakeoffs.ToString();

            lblLandingDayCount.Text = logbook.dayLandings.ToString();
            lblLandingNightCount.Text = logbook.nightTakeoffs.ToString();

            lblApproches.Text = "Approches " + logbook.logbookApproaches.Count;
        }

        void deleteCrew_Tapped(System.Object sender, System.EventArgs e)
        {
            //newCrewGrid.Children.Clear();
            //MessagingCenter.Send<App>((App)Application.Current, "CrewUpdated");
            MessagingCenter.Send(ParentGridRow.ToString(), "CrewRemoved");
        }



        void editCrew_Tapped(System.Object sender, System.EventArgs e)
        {
            EditDetails.IsVisible = true;
            MessagingCenter.Send<App>((App)Application.Current, "CrewUpdated");
        }




        void dropDownNRole_Tapped(System.Object sender, System.EventArgs e)
        {
        }

        void txtInstTime_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
        }

        void txtNightTime_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
        }

        void isTracked_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
        }

        void isHold_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
        }


        int ApprchGridRow = 1;
        public async Task AddNewApproach()
        {
            await Task.Delay(1);
            ApproachGrid.Children.Add(new NewApproach(ApprchGridRow), 0, 2, ApprchGridRow, ApprchGridRow + 1);
            ApprchGridRow++;

        }

        async void btnAddApproches_Tapped(System.Object sender, System.EventArgs e)
        {
            await AddNewApproach();
            MessagingCenter.Send<App>((App)Application.Current, "CrewUpdated");
        }
    }
}
