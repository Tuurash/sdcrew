using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using sdcrew.Models;

using sdcrew.Services.Data;
using sdcrew.Views.Postflight.SubViews;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Postflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class PostflightDetails : ContentPage
    {
        public List<Tab> AllTabs { get; set; }
        dynamic flight = new PostFlightVM();

        PostflightServices svm;



        bool IsRunning = false;
        string SubNavItemString = "";


        public PostflightDetails(dynamic flightObj)
        {
            InitializeComponent();
            BindingContext = this;

            FillSubNav();

            if (!String.IsNullOrEmpty(Services.Settings.PostflightSubviewVisibility))
            {
                postflighSubNav.IsVisible = bool.Parse(Services.Settings.PostflightSubviewVisibility);
            }
            else
            {
                postflighSubNav.IsVisible = true;
            }

            //PostflightSubNavItemsUpdate
            MessagingCenter.Subscribe<string>(this, "PostflightSubNavItemsUpdated", (ob) =>
            {
                FillSubNav();
            });

            MessagingCenter.Subscribe<string>(this, "PostflightSubviewUpdated", (ob) =>
            {
                postflighSubNav.IsVisible = bool.Parse(Services.Settings.PostflightSubviewVisibility);
            });

            flight = flightObj;

            svm = new PostflightServices();


            Task.Run(() =>
            {
                FillNavbarInfo();
            });

            MessagingCenter.Subscribe<AllAirports>(this, "airportSelectedData", (ob) =>
            {
                AllAirports receivedData = ob;
            });

        }

        private void FillSubNav()
        {
            List<Tab> AllTabs = new List<Tab>();

            SubNavItemString = Services.Settings.PostflightSubNavItems + ",OOOI,FUEL,CREW,";
            string[] navItems = SubNavItemString.Split(',');
            var fliteredTabs = Tabs.Get().Where(x => navItems.Contains(x.TabHeader)).OrderBy(x => x.TabIndex).ToList();

            AllTabs = fliteredTabs;

            collectionViewListHorizontal.ItemsSource = AllTabs;
        }

        private void FillNavbarInfo()
        {
            throw new NotImplementedException();
        }

        void HeaderSubNav_Tapped(System.Object sender, System.EventArgs e)
        {
        }

        async void btnBack_Tapped(System.Object sender, System.EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);
            await Navigation.PopAsync(true);
        }

        int CrewGridRow = 1;

        void btnAddCrew_Tapped(System.Object sender, System.EventArgs e)
        {
            List<CrewTempCard> crews = new List<CrewTempCard>();

            CrewGrid.Children.Add(new NewCrewCard(), 0, 2, CrewGridRow, CrewGridRow + 1);

            CrewGridRow++;
        }
    }

    public class Tab
    {
        public string TabHeader { get; set; }
        public int TabIndex { get; set; }

        public string TabColor { get; set; }
    }

    public class Tabs
    {
        public static List<Tab> Get()
        {
            string color = "#192E48";

            List<Tab> Tablist = new List<Tab>();

            Tablist.Add(new Tab() { TabHeader = "OOOI", TabIndex = 1, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "FUEL", TabIndex = 2, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "CREW", TabIndex = 3, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "PASSENGERS", TabIndex = 4, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "ADDITIONAL DETAILS", TabIndex = 5, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "NOTES", TabIndex = 6, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "EXPENSES", TabIndex = 7, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "DOCUMENTS", TabIndex = 8, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "DE/ANTI-ICE", TabIndex = 9, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "APU & CUSTOM COMPONENT(S)", TabIndex = 10, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "OILS & FLUIDS", TabIndex = 11, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "SQUAWKS & DISCREPANCIES", TabIndex = 12, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "DUTY TIME", TabIndex = 13, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "CHECKLISTS", TabIndex = 14, TabColor = color });

            return Tablist;
        }
    }

    //Issue: https://stackoverflow.com/questions/62416052/how-to-display-contentview-in-a-collectionview-in-xamarin-forms
    public class CrewTempCard
    {
        public string CrewTitle { get; set; }
        public string CrewName { get; set; }

    }
}
