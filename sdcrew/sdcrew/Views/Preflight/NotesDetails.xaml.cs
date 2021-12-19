using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Services.Data;
using System;
using System.ComponentModel;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class NotesDetails : PopupPage
    {
        public PreflightServices preflightServices;

        PreflightNote preflightNote = new PreflightNote();

        public int getNoteID { get; set; }

        public NotesDetails(int _noteId)
        {
            InitializeComponent();
            preflightServices = new PreflightServices();


            preflightNote = preflightServices.getNoteByID(_noteId);

            lblDate.Text = preflightNote.calendarDate.ToString("dddd dd MMM yyyy");
            lblNoteDetails.Text = preflightNote.note;
        }

        private async void btnColosePopup_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}
