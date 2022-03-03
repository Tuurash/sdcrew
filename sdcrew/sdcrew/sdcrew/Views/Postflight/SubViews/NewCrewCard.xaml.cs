using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.Views.Settings;
using Xamarin.Forms;

namespace sdcrew.Views.Postflight.SubViews
{
    public partial class NewCrewCard : ContentView
    {
        FlightCrewMember flightCrewMember;
        CrewDetailsVM CrewDetails;
        PostflightServices postflightServices;

        string BlockTime = String.Empty;
        string FlightTime = String.Empty;

        int ParentGridRow = 0;

        public NewCrewCard(int crewGridRow)
        {
            InitializeComponent();
            postflightServices = new PostflightServices();

            ParentGridRow = crewGridRow;
        }

        public NewCrewCard(FlightCrewMember crew, int crewGridRow, string blockTime, string flightTime)
        {
            flightCrewMember = crew;
            InitializeComponent();
            postflightServices = new PostflightServices();
            ParentGridRow = crewGridRow;

            BlockTime = blockTime;
            FlightTime = flightTime;

            if (flightCrewMember != null)
            {
                Task.Run(async () =>
                {
                    await FillCrewInfo();
                });
            }

            MessagingCenter.Subscribe<string>(this, "CrewTypeSelected", (ob) =>
            {
                lblCrewMemberType.Text = dropDownCrewRole.Text = ob.Split('-')[0];
            });

            MessagingCenter.Subscribe<string>(this, "ApproachRemoved", async (ob) =>
            {
                int CGR = int.Parse(ob);
                await RemoveApproachGridRow(CGR);
            });
        }

        public NewCrewCard(CrewDetailsVM crewDetails, int crewGridRow, string blockTime, string flightTime)
        {
            CrewDetails = crewDetails;
            InitializeComponent();
            postflightServices = new PostflightServices();
            ParentGridRow = crewGridRow;

            BlockTime = blockTime;
            FlightTime = flightTime;

            if (CrewDetails != null)
            {
                Task.Run(async () =>
                {
                    await FillCrewInfo();
                });
            }

            MessagingCenter.Subscribe<string>(this, "CrewTypeSelected", (ob) =>
            {
                lblCrewMemberType.Text = dropDownCrewRole.Text = ob.Split('-')[0];
            });

            MessagingCenter.Subscribe<string>(this, "ApproachRemoved", async (ob) =>
            {
                int CGR = int.Parse(ob);
                await RemoveApproachGridRow(CGR);
            });
        }


        private async Task FillCrewInfo()
        {
            await Task.Delay(1);
            if (CrewDetails != null)
            {
                flightCrewMember = new FlightCrewMember
                {
                    crewMemberType = CrewDetails.CrewMemberTypeName,
                };

                Device.BeginInvokeOnMainThread(() =>
                {
                    lblCrewName.Text = CrewDetails.FullName;
                });
            }

        }

        void deleteCrew_Tapped(System.Object sender, System.EventArgs e)
        {
            MessagingCenter.Send(ParentGridRow.ToString(), "CrewRemoved");
        }

        void editCrew_Tapped(System.Object sender, System.EventArgs e)
        {
            if (EditDetails.IsVisible == true)
            {
                EditDetails.IsVisible = false;
            }
            else { EditDetails.IsVisible = true; }
            MessagingCenter.Send<App>((App)Application.Current, "CrewUpdated");
        }

        async void dropDownNRole_Tapped(System.Object sender, System.EventArgs e)
        {
            var crewTypes = await postflightServices.CrewMember_TypesAsync();
            List<string> Names = new List<string>();
            foreach (var item in crewTypes)
            {
                Names.Add(item.Name + "-" + item.Description);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new popupDropdown(Names, "CrewTypeSelected")));
        }

        void txtInstTime_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {

            //TimeSpan FlightTym = TimeSpan.Parse(FlightTime);

            //if (TimeSpan.TryParse(txtInstTime.Text, out TimeSpan instTym) == true)
            //{
            //    if (!String.IsNullOrEmpty(FlightTime))
            //    {
            //        if (FlightTym < instTym)
            //        {
            //            //Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert("Alert", "Inst Time Can't be greater than flight time", "OK"));
            //            txtInstTime.Text = String.Empty;
            //        }
            //    }
            //}
        }

        void txtNightTime_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //if (!String.IsNullOrEmpty(BlockTime))
            //{
            //    if (DateTime.Parse(txtInstTime.Text) < DateTime.Parse(BlockTime))
            //    {
            //        Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert("Alert", "Night Time Can't be greater than block time", "OK"));
            //        txtInstTime.Text = String.Empty;
            //    }
            //}
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

        private async Task RemoveApproachGridRow(int cGR)
        {

            var children = ApproachGrid.Children.ToList();
            foreach (var child in children.Where(child => Grid.GetRow(child) == cGR))
            {
                ApproachGrid.Children.Remove(child);
            }
            await Task.Delay(1);
            children = ApproachGrid.Children.ToList();
            if (children.Count < 1)
            {
                ApprchGridRow = 0;
            }

            MessagingCenter.Send<App>((App)Application.Current, "CrewUpdated");
        }

    }
}
