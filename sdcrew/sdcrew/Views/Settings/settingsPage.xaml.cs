using sdcrew.Models;
using sdcrew.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using sdcrew.Services;
using Rg.Plugins.Popup.Services;
using sdcrew.Services.Data;
using System;
using sdcrew.ViewModels;

namespace sdcrew.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class settingsPage : ContentPage
    {

        string pickedRefreshtime = "";
        string pickedTheme = "";
        string pickedPastEvent = "";
        string pickedFutureEvent = "";
        string pickedPostflightPastEvent = "";

        UserServices _userServices = new UserServices();
        SettingsViewModel settingsViewModel;

        public settingsPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await FillUserinfo();

                //preflight Subnav
                if (!String.IsNullOrEmpty(Services.Settings.PreflightSubviewVisibility))
                {
                    switchSubNavVisibility.IsToggled = bool.Parse(Services.Settings.PreflightSubviewVisibility);
                }
                else { switchSubNavVisibility.IsToggled = true; }

                //postflight subnav
                if (!String.IsNullOrEmpty(Services.Settings.PostflightSubviewVisibility))
                {
                    switchPostflightSubNav.IsToggled = bool.Parse(Services.Settings.PostflightSubviewVisibility);
                }
                else { switchPostflightSubNav.IsToggled = true; }

                //postflightSubNavItems
                if (!String.IsNullOrEmpty(Services.Settings.PostflightSubNavItems))
                {
                    string getCache = Services.Settings.PostflightSubNavItems;

                    if (getCache.Contains("NOTES"))
                    { NoteVSwitch.IsToggled = true; }

                    if (getCache.Contains("PASSENGERS"))
                    { PassengerVSwitch.IsToggled = true; }

                    if (getCache.Contains("ADDITIONAL DETAILS"))
                    { AdditionDetailsVSwitch.IsToggled = true; }

                    if (getCache.Contains("EXPENSES"))
                    { ExpenseVSwitch.IsToggled = true; }

                    if (getCache.Contains("DE/ANTI-ICE"))
                    { DeAntiIceVSwitch.IsToggled = true; }

                    if (getCache.Contains("APU & CUSTOM COMPONENT(S)"))
                    { APUVSwitch.IsToggled = true; }

                    if (getCache.Contains("OILS & FLUIDS"))
                    { OilVSwitch.IsToggled = true; }

                    if (getCache.Contains("SQUAWKS & DISCREPANCIES"))
                    { SquawkVswitch.IsToggled = true; }

                    if (getCache.Contains("DUTY TIME"))
                    { DutyTimeVSwitch.IsToggled = true; }

                    if (getCache.Contains("CHECKLISTS"))
                    { ChecklistsVSwitch.IsToggled = true; }
                }

            });

            #region MessegingCenters

            //RefreshtimePicked
            MessagingCenter.Subscribe<string>(this, "RefreshtimePicked", (ob) =>
            {
                settingsViewModel = new SettingsViewModel();
                pickedRefreshtime = ob;
                PopulateRefreshTimePicker();

                Task.Run(async () => await settingsViewModel.PerformScheduledRefresh()).ConfigureAwait(false);
            });

            //ThemePicked
            MessagingCenter.Subscribe<string>(this, "ThemePicked", (ob) =>
            {
                pickedTheme = ob;
                PopulateThemePicker();
            });
            //preflightPasteventpicked
            MessagingCenter.Subscribe<string>(this, "preflightPasteventpicked", (ob) =>
            {
                pickedPastEvent = ob;
                PopulatePastEventPicker();
            });
            //preflightFutureventpicked
            MessagingCenter.Subscribe<string>(this, "preflightFutureventpicked", (ob) =>
            {
                pickedFutureEvent = ob;
                PopulateFutureEventPicker();
            });
            //PostflightPasteventpicked
            MessagingCenter.Subscribe<string>(this, "PostflightPasteventpicked", (ob) =>
            {
                pickedPostflightPastEvent = ob;
                PopulatePostFlightPicker();
            });

            #endregion
        }

        public async Task FillUserinfo()
        {
            var user = await _userServices.GetUser();
            lblUserName.Text = user.Name;
            lblUserEmail.Text = user.Email;

            //var client = new System.Net.Http.HttpClient();

            //System.IO.Stream imagestream = await client.GetStreamAsync(user.ImageUri);
            //imgUser.Source = ImageSource.FromStream(() => imagestream);

            //System.Uri uri;
            //System.Uri.TryCreate(user.ImageUri, UriKind.Absolute, out uri);
            //Task<ImageSource> result = Task<ImageSource>.Factory.StartNew(() => ImageSource.FromUri(uri));
            //imgUser.Source = await result;

            ImgUserSrc.Uri = user.ImageUri;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!String.IsNullOrEmpty(Services.Settings.UpdateNotification))
            {
                if (Services.Settings.UpdateNotification != "True")
                {
                    AppUpdate.IsToggled = false;
                }
                else { AppUpdate.IsToggled = true; }
            }
            else
            {
                AppUpdate.IsToggled = false;
            }


            PopulateRefreshTimePicker();
            PopulateThemePicker();
            PopulatePastEventPicker();
            PopulateFutureEventPicker();
            PopulatePostFlightPicker();
        }

        void AppUpdate_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            if (!String.IsNullOrEmpty(Services.Settings.UpdateNotification))
            {
                if (AppUpdate.IsToggled.ToString() != Services.Settings.UpdateNotification)
                {
                    Services.Settings.UpdateNotification = AppUpdate.IsToggled.ToString();
                }
            }
            else
            {
                Services.Settings.UpdateNotification = AppUpdate.IsToggled.ToString();
            }
        }

        void AppUpdate_Toggled_1(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            if (!String.IsNullOrEmpty(Services.Settings.UpdateNotification))
            {
                if (AppUpdate.IsToggled.ToString() != Services.Settings.UpdateNotification)
                {
                    Services.Settings.UpdateNotification = AppUpdate.IsToggled.ToString();
                }
                else { }
            }
        }

        #region RefreshTime

        public List<string> PopulateReshtime()
        {
            var pickerList = new List<string>();

            pickerList.Add("1 Minuite");
            pickerList.Add("5 Minuite");
            pickerList.Add("10 Minuite");
            pickerList.Add("15 Minuite");

            return pickerList;
        }

        public void PopulateRefreshTimePicker()
        {
            string getRefreshtimeFromStorage = Services.Settings.Refreshtimer;

            if (pickedRefreshtime != "")
            {
                if (pickedRefreshtime != getRefreshtimeFromStorage)
                {
                    lblpckdPrefreshtime.Text = pickedRefreshtime;
                    Services.Settings.Refreshtimer = pickedRefreshtime;
                }
            }
            else
            {
                lblpckdPrefreshtime.Text = PopulateReshtime()[0];

            }
        }

        //Refresh timer
        void dropdownRefreshtime_Tapped(System.Object sender, System.EventArgs e)
        {
            Task.Run(() => PopupNavigation.Instance.PushAsync(new popupDropdown(PopulateReshtime(), "RefreshtimePicked")));
        }

        #endregion

        #region Theme

        public List<string> PopulateTheme()
        {
            var pickerList = new List<string>();
            pickerList.Add("Dark");
            pickerList.Add("Light");
            pickerList.Add("SD Blue");
            pickerList.Add("SD Light");

            return pickerList;
        }

        public void PopulateThemePicker()
        {
            if (pickedTheme != "")
            {
                lblpckdTheme.Text = pickedTheme;
            }
            else
            {
                lblpckdTheme.Text = PopulateTheme()[0];
            }
        }

        void dropdownTheme_Tapped(System.Object sender, System.EventArgs e)
        {
            Task.Run(() => PopupNavigation.Instance.PushAsync(new popupDropdown(PopulateTheme(), "ThemePicked")));
        }

        #endregion

        #region Preflight done

        #region Past Events preflight

        public List<string> PopulatePastEvents()
        {
            var pickerList = new List<string>();
            pickerList.Add("Present");
            pickerList.Add("-1 days");
            pickerList.Add("-2 days");
            pickerList.Add("-3 days");
            pickerList.Add("-4 days");
            pickerList.Add("-5 days");
            pickerList.Add("-6 days");
            pickerList.Add("-7 days");

            return pickerList;
        }

        public void PopulatePastEventPicker()
        {
            string getDatefromStorage = Services.Settings.PreflightPastDays;

            if (pickedPastEvent != "")
            {
                if (pickedPastEvent != getDatefromStorage)
                {
                    lblpckdPastEvnt.Text = pickedPastEvent;
                    Services.Settings.PreflightPastDays = pickedPastEvent;

                    MessagingCenter.Send("DummyVal", "PreflightDateUpdated");
                }
            }
            else if (!String.IsNullOrEmpty(getDatefromStorage))
            {
                lblpckdPastEvnt.Text = getDatefromStorage;
            }
            else
            {
                lblpckdPastEvnt.Text = PopulatePastEvents()[0];
            }
        }

        void dropdownPastEvnt_Tapped(System.Object sender, System.EventArgs e)
        {
            Task.Run(() => PopupNavigation.Instance.PushAsync(new popupDropdown(PopulatePastEvents(), "preflightPasteventpicked")));
        }

        #endregion

        #region FutureEvents preflight

        public List<string> PopulateFutureEvents()
        {
            var pickerList = new List<string>();


            pickerList.Add("Present");

            for (int i = 1; i < 31; i++)
            {
                pickerList.Add("+" + i + " days");
            }

            return pickerList;
        }

        public void PopulateFutureEventPicker()
        {
            string getDatefromStorage = Services.Settings.PreflightFutureDays;

            if (pickedFutureEvent != "")
            {
                if (pickedFutureEvent != getDatefromStorage)
                {
                    lblFutureEvent.Text = pickedFutureEvent;
                    Services.Settings.PreflightFutureDays = pickedFutureEvent;

                    MessagingCenter.Send("DummyVal", "PreflightDateUpdated");
                }
            }
            else if (!String.IsNullOrEmpty(getDatefromStorage) & getDatefromStorage != "")
            {
                lblFutureEvent.Text = getDatefromStorage;
            }
            else
            {
                lblpckdPastEvnt.Text = "Present";
            }
        }

        void dropdownFutureEvnt_Tapped(System.Object sender, System.EventArgs e)
        {
            Task.Run(() => PopupNavigation.Instance.PushAsync(new popupDropdown(PopulateFutureEvents(), "preflightFutureventpicked")));
        }

        #endregion

        void switchSubNavVisibility_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            string SwitchStatus = switchSubNavVisibility.IsToggled.ToString();

            if (SwitchStatus != Services.Settings.PreflightSubviewVisibility)
            {
                if (!(switchSubNavVisibility.IsToggled))
                {
                    Services.Settings.PreflightSubviewVisibility = "false";
                }
                else
                {
                    Services.Settings.PreflightSubviewVisibility = "true";
                }

                MessagingCenter.Send("DummyVal", "PreflightSubviewUpdated");
            }

        }

        #endregion

        #region past Events postflight

        public void PopulatePostFlightPicker()
        {
            string getDatefromStorage = Services.Settings.PostflightPastDays;

            if (pickedPostflightPastEvent != "")
            {
                if (pickedPostflightPastEvent != getDatefromStorage)
                {
                    lblPostflightPastEvent.Text = pickedPostflightPastEvent;
                    Services.Settings.PostflightPastDays = pickedPostflightPastEvent;

                    MessagingCenter.Send("DummyVal", "PostflightUpdated");
                }
            }
            else if (!String.IsNullOrEmpty(getDatefromStorage))
            {
                lblPostflightPastEvent.Text = getDatefromStorage;
            }
            else
            {
                lblPostflightPastEvent.Text = PopulatePastEvents()[0];
            }
        }

        void dropdownPostflightPastEvnt_Tapped(System.Object sender, System.EventArgs e)
        {
            Task.Run(() => PopupNavigation.Instance.PushAsync(new popupDropdown(PopulatePastEvents(), "PostflightPasteventpicked")));
        }

        void switchPostflightSubNav_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            string SwitchStatus = switchPostflightSubNav.IsToggled.ToString();

            if (SwitchStatus != Services.Settings.PostflightSubviewVisibility)
            {
                if (!(switchPostflightSubNav.IsToggled))
                {
                    Services.Settings.PostflightSubviewVisibility = "false";
                }
                else
                {
                    Services.Settings.PostflightSubviewVisibility = "true";
                }

                MessagingCenter.Send("DummyVal", "PostflightSubviewUpdated");
            }
        }

        #region Postflight Tabs

        string tabVal = "";

        //NOTES
        void NoteVSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "NOTES";

            if (!NoteVSwitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }


            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");
        }

        //Passengers
        void PassengerVSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "PASSENGERS";

            if (!PassengerVSwitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }


            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");
        }

        //ADDITIONAL DETAILS
        void AdditionDetails_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "ADDITIONAL DETAILS";

            if (!AdditionDetailsVSwitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }


            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");
        }

        //Expenses
        void ExpenseVSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "EXPENSES";

            if (!ExpenseVSwitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }
            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");

        }

        //DOCUMENTS
        void DocVSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "DOCUMENTS";

            if (!DocVSwitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }
            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");

        }

        //DE/ANTI-ICE
        void DeAntiIceVSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "DE/ANTI-ICE";

            if (!DeAntiIceVSwitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }
            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");
        }

        //APU & CUSTOM COMPONENT(S)
        void APUVSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "APU & CUSTOM COMPONENT(S)";

            if (!APUVSwitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }
            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");
        }

        //OilVSwitch
        void OilVSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "OILS & FLUIDS";

            if (!OilVSwitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }
            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");
        }

        //SQUAWKS & DISCREPANCIES
        void SquawkVswitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "SQUAWKS & DISCREPANCIES";

            if (!SquawkVswitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }
            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");

        }

        //DUTY TIME
        void DutyTimeVSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "DUTY TIME";

            if (!DutyTimeVSwitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }
            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");
        }

        //CHECKLISTS
        void ChecklistsVSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            tabVal = "CHECKLISTS";

            if (!ChecklistsVSwitch.IsToggled)
            {
                Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems.Replace("," + tabVal, "");
            }
            else
            {
                if (!Services.Settings.PostflightSubNavItems.ToLower().Contains(tabVal.ToLower()))
                {
                    Services.Settings.PostflightSubNavItems = Services.Settings.PostflightSubNavItems + "," + tabVal;
                }
            }
            MessagingCenter.Send("DummyVal", "PostflightSubNavItemsUpdated");
        }



        #endregion

        #endregion

        void AutomaticAirports_Update_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
        }

        void btnBack_Tapped(System.Object sender, System.EventArgs e)
        {
        }

    }
}