using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using sdcrew.Services.Data;
using sdcrew.Views.Postflight.Popups;
using Xamarin.Forms;

namespace sdcrew.Views.Postflight.SubViews
{
    public partial class NewDoc : ContentView
    {
        public NewDoc()
        {
            InitializeComponent();
        }

        PostflightServices postflightServices;
        int ParentGridRow = 0;

        TempDoc Document = new TempDoc();

        public NewDoc(int expenseGridRow)
        {
            InitializeComponent();

            postflightServices = new PostflightServices();
            ParentGridRow = expenseGridRow;

            MessagingCenter.Subscribe<TempDoc>(this, "DocAdded", async (ob) =>
            {
                Document = ob;
                FillDocInfo();
            });
        }

        public NewDoc(int expenseGridRow, TempDoc tDoc)
        {
            InitializeComponent();
            Document = tDoc;
            postflightServices = new PostflightServices();
            ParentGridRow = expenseGridRow;

            FillDocInfo();

            MessagingCenter.Subscribe<TempDoc>(this, "DocAdded", (ob) =>
            {
                Document = ob;
                FillDocInfo();
            });
        }

        private void FillDocInfo()
        {
            lblDocTitle.Text = Document.DocTitle;
            lblDocType.Text = Document.DocType;
        }

        void delete_Tapped(System.Object sender, System.EventArgs e)
        {
            MessagingCenter.Send(ParentGridRow.ToString(), "DocRemoved");
        }

        void edit_Tapped(System.Object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new PopUpDocumentDetails(Document));
            //MessagingCenter.Send(Document, "DocAdded");
        }
    }
}
