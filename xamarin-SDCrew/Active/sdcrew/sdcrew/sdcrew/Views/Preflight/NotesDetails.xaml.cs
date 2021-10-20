﻿using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using sdcrew.Models;
using sdcrew.Services.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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

            lblDate.Text = preflightNote.calendarDate.ToString();
            lblNoteDetails.Text = preflightNote.note;




        }

        
    }
}
