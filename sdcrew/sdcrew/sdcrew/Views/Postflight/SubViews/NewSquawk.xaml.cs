using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Services;

using sdcrew.Services.Data;
using sdcrew.Views.Settings;

using Xamarin.Forms;

namespace sdcrew.Views.Postflight.SubViews
{
    public partial class NewSquawk : ContentView
    {
        PostflightServices svm;

        int ParentGridRow = 0;

        public NewSquawk()
        {
            InitializeComponent();
            svm = new PostflightServices();
        }

        public NewSquawk(int squawkGridRow)
        {
            InitializeComponent();
            svm = new PostflightServices();
            ParentGridRow = squawkGridRow;
            MessagingCenter.Subscribe<string>(this, "ATACodePicked", (ob) =>
            {
                lblAtaCode.Text = ob;
                dropdownSquawkAtaCode.Text = ob;
            });

            MessagingCenter.Subscribe<string>(this, "SquawkTypePicked", (ob) =>
            {
                dropdownSquawkDiscrepencyType.Text = ob;
            });

            MessagingCenter.Subscribe<string>(this, "SquawkCategoryPicked", (ob) =>
            {
                dropdownSquawkCatagory.Text = ob;
            });

            MessagingCenter.Subscribe<string>(this, "ReportedByPicked", (ob) =>
            {
                dropdownSquawkReportedBy.Text = ob;
            });
        }

        void EditSquawk_Tapped(System.Object sender, System.EventArgs e)
        {
            if (editSquawkForm.IsVisible == false)
            {
                editSquawkForm.IsVisible = true;
            }
            else
            {
                editSquawkForm.IsVisible = false;
            }

            MessagingCenter.Send<App>((App)Application.Current, "SquawkUpdated");
        }

        private void delete_Tapped(object sender, EventArgs e)
        {
            MessagingCenter.Send(ParentGridRow.ToString(), "SquawkRemoved");
        }


        async void dropdownSquawkAtaCode_Tapped(System.Object sender, System.EventArgs e)
        {
            var aTACodes = await svm.ATACodesAsync();

            List<string> ataCodeNames = new List<string>();
            foreach (var item in aTACodes)
            {
                ataCodeNames.Add(item.Name);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new popupDropdown(ataCodeNames, "ATACodePicked")));
        }

        async void dropdownSquawkType_Tapped(System.Object sender, System.EventArgs e)
        {
            var squawkTypes = await svm.SquawkTypesAsync();

            List<string> squawkTypeNames = new List<string>();
            foreach (var item in squawkTypes)
            {
                squawkTypeNames.Add(item.Name);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new popupDropdown(squawkTypeNames, "SquawkTypePicked")));
        }

        async void dropdownSquawkReportedBy_Tapped(System.Object sender, System.EventArgs e)
        {
            var crews = await svm.GetCrewsAsync();

            List<string> crewNames = new List<string>();
            foreach (var item in crews)
            {
                crewNames.Add(item.FullName);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new popupDropdown(crewNames, "ReportedByPicked")));
        }

        async void dropdownSquawkCatagory_Tapped(System.Object sender, System.EventArgs e)
        {
            var squawkCatagories = await svm.SquawkCategoriesAsync();

            List<string> squawkCatNames = new List<string>();
            foreach (var item in squawkCatagories)
            {
                squawkCatNames.Add(item.Name);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new popupDropdown(squawkCatNames, "SquawkCategoryPicked")));
        }


    }
}
