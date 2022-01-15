using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

using sdcrew.Models;
using sdcrew.Repositories.PreflightRepos;
using sdcrew.Services.Data;
using sdcrew.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using input = Plugin.InputKit.Shared.Controls;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class FlightCheckList : PopupPage, INotifyPropertyChanged
    {
        //Maintenance, Pilot, Scheduling from json

        #region refs

        PreflightServices preflightServices;
        UserServices userServices;
        PreflightRepository preflightRepository;
        int ScheduledAircraftTripID = 0;
        int LegNumber = 0;
        int FlightId = 0;

        int pflag = 0;
        int mflag = 0;
        int sflag = 0;

        bool IsRefreshing = false;

        User user = new User();

        public List<string> selectedMaintenanceIds = new List<string>();
        public List<string> selectedScheduleIds = new List<string>();
        public List<string> selectedPilotIds = new List<string>();

        public List<string> NotselectedMaintenanceIds = new List<string>();
        public List<string> NotselectedScheduleIds = new List<string>();
        public List<string> NotselectedPilotIds = new List<string>();

        public Task FetchChecklists { get; set; }

        public string getUserEsignStatement = Services.Settings.GetUserMail + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

        #region listHeaders

        private bool schHeaderVisible = false;
        public bool SchHeaderVisible
        {
            get
            {
                return schHeaderVisible;
            }

            set
            {
                schHeaderVisible = value;
                OnPropertyChanged();
            }
        }

        private bool pilotHeaderVisible = false;
        public bool PilotHeaderVisible
        {
            get
            {
                return pilotHeaderVisible;
            }

            set
            {
                pilotHeaderVisible = value;
                OnPropertyChanged();
            }
        }


        private bool mntctHeaderVisible = false;
        public bool MntcHeaderVisible
        {
            get
            {
                return mntctHeaderVisible;
            }

            set
            {
                mntctHeaderVisible = value;
                OnPropertyChanged();
            }
        }

        private string headerText = Services.Settings.GetUserMail + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
        public string HeaderText
        {
            get
            {
                return headerText;
            }
            set
            {
                headerText = value;
            }
        }


        #endregion

        #endregion

        public FlightCheckList(dynamic scheduledAircraftTripId, dynamic legNumber, dynamic flightId)
        {
            #region refs

            preflightServices = new PreflightServices();
            userServices = new UserServices();
            preflightRepository = new PreflightRepository();

            ScheduledAircraftTripID = scheduledAircraftTripId;
            LegNumber = legNumber;
            FlightId = flightId;

            selectedMaintenanceIds.Clear();
            selectedScheduleIds.Clear();
            selectedPilotIds.Clear();

            NotselectedMaintenanceIds.Clear();
            NotselectedScheduleIds.Clear();
            NotselectedPilotIds.Clear();

            #endregion

            user = Task.Run(async () => await userServices.GetUser()).Result;

            InitializeComponent();
            BindingContext = this;

            MainThread.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
            Task.Delay(1);

            var t = preflightServices.AddPreflight_Checklists(ScheduledAircraftTripID, LegNumber, 1, FlightId).Result;
            if (t == true)
            {
                FIllChecklists();
            }

            Loader.IsVisible = false;
        }

        #region Fill_List

        private void FIllChecklists()
        {
            Pilotchecklist.ItemsSource = GetPilotchecklist();
            ScheduleChecklist.ItemsSource = GetScheduleList();
            MaintenanceList.ItemsSource = GetMaintenanceCheckList();
        }



        public ObservableCollection<FlightChecklist> GetScheduleList()
        {
            string getUser = user.Email + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

            var filteredTails = preflightServices.GetAll_ChecklistsByRoleName(ScheduledAircraftTripID, LegNumber, 1, FlightId, "Scheduling");
            bool getHeaderVisibility = false;
            var getSignedID = filteredTails.Select(x => x.checklistSignId).FirstOrDefault();

            var ar = filteredTails.Where(x => x.selectedByUser != "");

            if (getSignedID != 0 && ar.Count() != 0)
            {
                SchHeaderVisible = true;
                ScheduleChecklist.IsEnabled = false;

                sflag = 1;
            }

            List<FlightChecklist> SchChklst = new List<FlightChecklist>();

            bool getSelected = false;


            foreach (var item in filteredTails)
            {
                if (item.selectedByUser != null && item.selectedByUser != "")
                {
                    getSelected = true;
                    getUser = user.Email + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                }
                else
                {
                    getSelected = false;
                    getUser = "";
                }
                SchChklst.Add(new FlightChecklist
                {
                    isVisible = "True",
                    isChecked = getSelected,
                    ChecklistName = item.configTaskName,
                    configTaskId = item.configTaskId,
                    checkedByUsermailDate = getUser,
                    HeaderIsVisible = getHeaderVisibility
                });
            }

            var MChklistCollection = new ObservableCollection<FlightChecklist>(SchChklst);
            return MChklistCollection;
        }

        public ObservableCollection<FlightChecklist> GetMaintenanceCheckList()
        {
            var filteredTails = preflightServices.GetAll_ChecklistsByRoleName(ScheduledAircraftTripID, LegNumber, 1, FlightId, "Maintenance");
            bool getHeaderVisibility = false;
            var getSignedID = filteredTails.Select(x => x.checklistSignId).FirstOrDefault();
            if (getSignedID != 0)
            {
                MntcHeaderVisible = true;
                MaintenanceList.IsEnabled = false;

                mflag = 1;
            }


            List<FlightChecklist> MaintenanceChklst = new List<FlightChecklist>();

            bool getSelected = false;
            string getUser = "";

            foreach (var item in filteredTails)
            {
                if (item.selectedByUser != null && item.selectedByUser != "")
                {
                    getSelected = true;
                    getUser = getUserEsignStatement;
                }
                else
                {
                    getSelected = false;
                    getUser = "";
                }
                MaintenanceChklst.Add(new FlightChecklist
                {
                    isVisible = "True",
                    isChecked = getSelected,
                    ChecklistName = item.configTaskName,
                    configTaskId = item.configTaskId,
                    checkedByUsermailDate = getUser,
                    HeaderIsVisible = getHeaderVisibility
                });
            }

            var MChklistCollection = new ObservableCollection<FlightChecklist>(MaintenanceChklst);
            return MChklistCollection;
        }

        public ObservableCollection<FlightChecklist> GetPilotchecklist()
        {

            var filteredTails = preflightServices.GetAll_ChecklistsByRoleName(ScheduledAircraftTripID, LegNumber, 1, FlightId, "Pilot");
            bool getHeaderVisibility = false;
            var getSignedID = filteredTails.Select(x => x.checklistSignId).FirstOrDefault();
            if (getSignedID != 0)
            {
                PilotHeaderVisible = true;
                Pilotchecklist.IsEnabled = false;

                pflag = 1;
            }


            List<FlightChecklist> PilotChklst = new List<FlightChecklist>();

            bool getSelected = false;
            string getUser = "";


            foreach (var item in filteredTails)
            {
                if (item.selectedByUser != null && item.selectedByUser != "")
                {
                    getSelected = true;
                    getUser = user.Email + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                }
                else
                {
                    getSelected = false;
                    getUser = "";
                }
                PilotChklst.Add(new FlightChecklist
                {
                    isVisible = "True",
                    isChecked = getSelected,
                    ChecklistName = item.configTaskName,
                    configTaskId = item.configTaskId,
                    checkedByUsermailDate = getUser,
                    HeaderIsVisible = getHeaderVisibility
                });
            }

            var MChklistCollection = new ObservableCollection<FlightChecklist>(PilotChklst);
            return MChklistCollection;
        }

        #endregion

        #region Checked_Changed

        private void chkBoxMaintenance_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            var checkbox = (input.CheckBox)sender;
            var ob = checkbox.BindingContext as FlightChecklist;

            if (ob.isChecked)
            {
                try
                {
                    selectedMaintenanceIds.Add(ob.configTaskId.ToString());
                    NotselectedMaintenanceIds.Remove(ob.configTaskId.ToString());

                }
                catch { }
            }
            else
            {
                try
                {
                    selectedMaintenanceIds.Remove(ob.configTaskId.ToString());
                    NotselectedMaintenanceIds.Add(ob.configTaskId.ToString());
                }
                catch (Exception) { }
            }
        }

        private void chkSchedule_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            var checkbox = (input.CheckBox)sender;
            var ob = checkbox.BindingContext as FlightChecklist;

            if (ob.isChecked)
            {
                try
                {
                    selectedScheduleIds.Add(ob.configTaskId.ToString());
                    NotselectedScheduleIds.Remove(ob.configTaskId.ToString());

                }
                catch { }
            }
            else
            {
                try
                {
                    selectedScheduleIds.Remove(ob.configTaskId.ToString());
                    NotselectedScheduleIds.Add(ob.configTaskId.ToString());
                }
                catch { }
            }
        }

        private void chkPilot_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            //var checkbox = (CheckBox)sender;
            var checkbox = (input.CheckBox)sender;
            var ob = checkbox.BindingContext as FlightChecklist;

            if (ob.isChecked)
            {
                try
                {
                    selectedPilotIds.Add(ob.configTaskId.ToString());
                    NotselectedPilotIds.Remove(ob.configTaskId.ToString());
                }
                catch { }
            }
            else
            {
                try
                {
                    selectedPilotIds.Remove(ob.configTaskId.ToString());
                    NotselectedPilotIds.Add(ob.configTaskId.ToString());
                }
                catch { }
            }
        }


        #endregion

        #region SavingRegion


        private async void FABSave_Clicked(object sender, EventArgs e)
        {
            //roll type pilot=2, scheduling=1,maintenance=3

            if (IsRefreshing == false)
            {
                IsRefreshing = true;

                int ActiveTab = ChklistTabview.SelectedIndex; //pilot -1, schedule 1, maintenance 2

                int checkListTypeId = 1;
                int checklistRoleTypeId = 0;

                string action = "";

                if (ActiveTab >= 2)
                {
                    mntctHeaderVisible = true;
                    MaintenanceList.IsEnabled = false;

                    if (mflag > 0)
                    {
                        action = "Cancel";
                    }
                    else
                    {
                        action = await DisplayActionSheet(null, "Cancel", null, "Save Checklist", "e-Sign");
                    }

                }

                else if (ActiveTab == 1)
                {
                    if (sflag > 0)
                    {
                        action = "Cancel";
                    }
                    else { action = await DisplayActionSheet(null, "Cancel", null, "Save Checklist", "e-Sign"); }
                }

                else if (ActiveTab == -1 || ActiveTab == 0)
                {
                    if (pflag > 0)
                    {
                        action = "Cancel";
                    }
                    else { action = await DisplayActionSheet(null, "Cancel", null, "Save Checklist", "e-Sign"); }
                }



                if (action == "Save Checklist")
                {

                    MainThread.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
                    await Task.Delay(1);

                    await SaveChecklist();
                    await SaveUnsaveChecklist();
                }
                else if (action == "e-Sign")
                {
                    string getEsignPin = await DisplayPromptAsync("Enter E-Signature PIN", "", keyboard: Keyboard.Numeric);

                    if (getEsignPin != null && getEsignPin != "")
                    {
                        await SaveChecklist();
                        await SaveUnsaveChecklist();

                        MainThread.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
                        await Task.Delay(1);


                        List<ChecklistVM> getSavedList = new List<ChecklistVM>();

                        if (ActiveTab >= 2)
                        {
                            checklistRoleTypeId = 3;
                            getSavedList = preflightServices.Get_SelectedChecklists(selectedMaintenanceIds, "Maintenance", ScheduledAircraftTripID, LegNumber);
                        }

                        else if (ActiveTab == 1)
                        {
                            checklistRoleTypeId = 1;
                            getSavedList = preflightServices.Get_SelectedChecklists(selectedScheduleIds, "Scheduling", ScheduledAircraftTripID, LegNumber);

                        }

                        else if (ActiveTab == -1 || ActiveTab == 0)
                        {
                            checklistRoleTypeId = 2;
                            getSavedList = preflightServices.Get_SelectedChecklists(selectedPilotIds, "Pilot", ScheduledAircraftTripID, LegNumber);
                        }

                        var getESignStatus = await preflightRepository.postESign(int.Parse(getEsignPin), FlightId, 1, checkListTypeId, checklistRoleTypeId, getSavedList, user.Email, LegNumber);

                        if (getESignStatus == true)
                        {
                            if (ActiveTab >= 2)
                            {
                                MntcHeaderVisible = true;
                                MaintenanceList.IsEnabled = false;

                            }

                            else if (ActiveTab == 1)
                            {
                                SchHeaderVisible = true;
                                ScheduleChecklist.IsEnabled = false;
                            }

                            else if (ActiveTab == -1 || ActiveTab == 0)
                            {
                                PilotHeaderVisible = true;
                                Pilotchecklist.IsEnabled = false;
                            }
                        }
                    }

                }

                else
                {
                    Loader.IsVisible = false;
                    IsRefreshing = false;

                    return;
                }

                Loader.IsVisible = false;
                IsRefreshing = false;
            }
        }

        public async Task SaveChecklist()
        {
            MainThread.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

            int ActiveTab = ChklistTabview.SelectedIndex; //pilot -1, schedule 1, maintenance 2

            ChecklistVM VChecklist = new ChecklistVM();

            if (ActiveTab == 2)
            {
                #region Maintenance

                var listMaintenance = preflightServices.Get_SelectedChecklists(selectedMaintenanceIds, "Maintenance", ScheduledAircraftTripID, LegNumber);

                foreach (var selected in listMaintenance)
                {
                    VChecklist = new ChecklistVM
                    {
                        customerId = selected.customerId,
                        tripId = selected.tripId,
                        flightId = selected.flightId,
                        legNumber = LegNumber,                       //check Errors Here
                        checkListTypeId = selected.checkListTypeId,
                        checklistRoleTypeId = selected.checklistRoleTypeId,
                        checklistRoleTypeName = selected.checklistRoleTypeName,
                        configTaskId = selected.configTaskId,
                        configTaskName = selected.configTaskName,
                        configTaskOrder = selected.configTaskOrder,
                        selectedByUser = user.Email,  //Change
                        selectedDateTime = DateTime.Now,
                        checklistSignId = selected.checklistSignId
                    };

                    var PostMaintenance = preflightRepository.Save_Checklist(VChecklist);

                    preflightServices.UpdateChecklistVm(VChecklist);


                }

                #endregion
            }

            if (ActiveTab == 1)
            {
                #region Scheduling

                var listScheduling = preflightServices.Get_SelectedChecklists(selectedScheduleIds, "Scheduling", ScheduledAircraftTripID, LegNumber);

                foreach (var selected in listScheduling)
                {
                    VChecklist = new ChecklistVM
                    {
                        customerId = selected.customerId,
                        tripId = selected.tripId,
                        flightId = selected.flightId,
                        legNumber = LegNumber,                       //check Errors Here
                        checkListTypeId = selected.checkListTypeId,
                        checklistRoleTypeId = selected.checklistRoleTypeId,
                        checklistRoleTypeName = selected.checklistRoleTypeName,
                        configTaskId = selected.configTaskId,
                        configTaskName = selected.configTaskName,
                        configTaskOrder = selected.configTaskOrder,
                        selectedByUser = user.Email,  //Change
                        selectedDateTime = DateTime.Now,
                        checklistSignId = selected.checklistSignId
                    };

                    var PostMaintenance = preflightRepository.SaveChecklist(VChecklist);

                    preflightServices.UpdateChecklistVm(VChecklist);


                }

                #endregion
            }

            if (ActiveTab == -1 || ActiveTab == 0)
            {
                #region Pilot

                var listPilot = preflightServices.Get_SelectedChecklists(selectedPilotIds, "Pilot", ScheduledAircraftTripID, LegNumber);

                foreach (var selected in listPilot)
                {
                    VChecklist = new ChecklistVM
                    {
                        customerId = selected.customerId,
                        tripId = selected.tripId,
                        flightId = selected.flightId,
                        legNumber = LegNumber,                       //check Errors Here
                        checkListTypeId = selected.checkListTypeId,
                        checklistRoleTypeId = selected.checklistRoleTypeId,
                        checklistRoleTypeName = selected.checklistRoleTypeName,
                        configTaskId = selected.configTaskId,
                        configTaskName = selected.configTaskName,
                        configTaskOrder = selected.configTaskOrder,
                        selectedByUser = user.Email,  //Change
                        selectedDateTime = DateTime.Now,
                        checklistSignId = selected.checklistSignId
                    };

                    var PostList = preflightRepository.SaveChecklist(VChecklist);
                    preflightServices.UpdateChecklistVm(VChecklist);
                }

                #endregion
            }

            await Task.Delay(10);

        }

        public async Task SaveUnsaveChecklist()
        {

            MainThread.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

            int ActiveTab = ChklistTabview.SelectedIndex; //pilot -1, schedule 1, maintenance 2

            ChecklistVM VChecklist = new ChecklistVM();

            if (ActiveTab == 2)
            {
                #region Maintenance

                var UnselectedlistMaintenance = preflightServices.Get_UnSelectedChecklists(NotselectedMaintenanceIds, "Maintenance", ScheduledAircraftTripID, LegNumber);

                foreach (var unselected in UnselectedlistMaintenance)
                {
                    VChecklist = new ChecklistVM
                    {
                        customerId = unselected.customerId,
                        tripId = unselected.tripId,
                        flightId = unselected.flightId,
                        legNumber = LegNumber,                       //check Errors Here
                        checkListTypeId = unselected.checkListTypeId,
                        checklistRoleTypeId = unselected.checklistRoleTypeId,
                        checklistRoleTypeName = unselected.checklistRoleTypeName,
                        configTaskId = unselected.configTaskId,
                        configTaskName = unselected.configTaskName,
                        configTaskOrder = unselected.configTaskOrder,
                        selectedByUser = "",  //Change
                        selectedDateTime = DateTime.Now,
                        checklistSignId = unselected.checklistSignId
                    };

                    var PostMaintenance = preflightRepository.DeleteChecklist(VChecklist);
                    preflightServices.UpdateChecklistVm(VChecklist);
                }

                #endregion
            }

            if (ActiveTab == 1)
            {
                #region Scheduling


                var UnselectedlistScheduling = preflightServices.Get_UnSelectedChecklists(NotselectedScheduleIds, "Scheduling", ScheduledAircraftTripID, LegNumber);

                foreach (var unselected in UnselectedlistScheduling)
                {
                    VChecklist = new ChecklistVM
                    {
                        customerId = unselected.customerId,
                        tripId = unselected.tripId,
                        flightId = unselected.flightId,
                        legNumber = LegNumber,                       //check Errors Here
                        checkListTypeId = unselected.checkListTypeId,
                        checklistRoleTypeId = unselected.checklistRoleTypeId,
                        checklistRoleTypeName = unselected.checklistRoleTypeName,
                        configTaskId = unselected.configTaskId,
                        configTaskName = unselected.configTaskName,
                        configTaskOrder = unselected.configTaskOrder,
                        selectedByUser = "",  //Change
                        selectedDateTime = DateTime.Now,
                        checklistSignId = unselected.checklistSignId
                    };

                    var PostMaintenance = preflightRepository.DeleteChecklist(VChecklist);
                    preflightServices.UpdateChecklistVm(VChecklist);

                }
                #endregion
            }

            if (ActiveTab == -1 || ActiveTab == 0)
            {
                #region Pilot

                var UnselectedlistPilot = preflightServices.Get_UnSelectedChecklists(NotselectedPilotIds, "Pilot", ScheduledAircraftTripID, LegNumber);

                foreach (var unselected in UnselectedlistPilot)
                {
                    VChecklist = new ChecklistVM
                    {
                        customerId = unselected.customerId,
                        tripId = unselected.tripId,
                        flightId = unselected.flightId,
                        legNumber = LegNumber,                       //check Errors Here
                        checkListTypeId = unselected.checkListTypeId,
                        checklistRoleTypeId = unselected.checklistRoleTypeId,
                        checklistRoleTypeName = unselected.checklistRoleTypeName,
                        configTaskId = unselected.configTaskId,
                        configTaskName = unselected.configTaskName,
                        configTaskOrder = unselected.configTaskOrder,
                        selectedByUser = "",  //Change
                        selectedDateTime = DateTime.Now,
                        checklistSignId = unselected.checklistSignId
                    };

                    var PostMaintenance = preflightRepository.DeleteChecklist(VChecklist);
                    preflightServices.UpdateChecklistVm(VChecklist);
                }

                #endregion
            }

            await Task.Delay(100);
        }

        #endregion

        private async void btnColosePopup_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                MessagingCenter.Send<App>((App)Application.Current, "OnChecklistChanged");
            }
            catch (Exception) { }
        }

    }

    public class FlightChecklist : INotifyPropertyChanged
    {
        public string isVisible { get; set; }

        public string ChecklistName { get; set; }
        public int configTaskId { get; set; }

        public bool HeaderIsVisible { get; set; }

        public bool IsChecked;
        public bool isChecked
        {
            get
            {

                return IsChecked;
            }

            set
            {
                if (IsChecked != value)
                {
                    IsChecked = value;

                    if (IsChecked)
                    {
                        CheckedByUsermailDate = Services.Settings.GetUserMail + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                    }
                    else
                    {
                        CheckedByUsermailDate = "";
                    }

                    onPropertyChanged();
                    onPropertyChanged("checkedByUsermailDate");
                }
            }

        }

        private string CheckedByUsermailDate { get; set; }
        public string checkedByUsermailDate
        {
            set
            {
                onPropertyChanged("checkedByUsermailDate");
            }
            get => CheckedByUsermailDate;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void onPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}