using System;
using System.Collections.Generic;

using Rg.Plugins.Popup.Services;

using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.Views.Postflight.Modals.DropdownModals;

using Xamarin.Forms;

namespace sdcrew.Views.Postflight.SubViews
{
    public partial class NewDutyTime : ContentView
    {
        PostflightServices postflightServices;
        CrewDetailsVM Crew = new CrewDetailsVM();
        int ParentGridRow = 0;

        public NewDutyTime()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<CrewDetailsVM>(this, "DutyDetailsCrewChanged", async (ob) =>
            {
                Crew = ob;
                FillDutyDetails();
            });
        }

        public NewDutyTime(int dutyGridGridRow, CrewDetailsVM crewDetails)
        {
            InitializeComponent();
            Crew = crewDetails;
            ParentGridRow = dutyGridGridRow;
            FillDutyDetails();

            MessagingCenter.Subscribe<CrewDetailsVM>(this, "DutyDetailsCrewChanged" + ParentGridRow, (ob) =>
              {
                  Crew = ob;
                  FillDutyDetails();
              });
        }

        private void FillDutyDetails()
        {
            lblCrewName.Text = dropDownCrew.Text = Crew.LastName + ", " + Crew.FirstName;
        }




        void edit_Tapped(System.Object sender, System.EventArgs e)
        {
            if (gridDutyDetails.IsVisible == true)
            { gridDutyDetails.IsVisible = false; }
            else { gridDutyDetails.IsVisible = true; }
            MessagingCenter.Send<App>((App)Application.Current, "DutyUpdated");
        }

        void dropdownCrew_Tapped(System.Object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new CrewListDropdown("DutyDetailsCrewChanged" + ParentGridRow));
        }
    }
}
