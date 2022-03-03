using DocumentFormat.OpenXml.Wordprocessing;

using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

using sdcrew.Services.Data;
using sdcrew.Views.Settings;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Postflight.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopUpDocumentDetails : PopupPage
    {
        PostflightServices postflightServices;
        TempDoc Dcc = new TempDoc();
        Stream UploadedFileStream;

        public PopUpDocumentDetails()
        {
            postflightServices = new PostflightServices();
            InitializeComponent();
            Loader.IsVisible = false;
            FillDocumentTypes();
            Dcc = null;

            MessagingCenter.Subscribe<string>(this, "DocumentTypeClicked", (ob) =>
            {
                dropdownnDocType.Text = ob;
            });

        }

        public PopUpDocumentDetails(TempDoc doc)
        {
            postflightServices = new PostflightServices();
            InitializeComponent();
            Loader.IsVisible = false;
            FillDocumentTypes();

            Dcc = doc;
            MessagingCenter.Subscribe<string>(this, "DocumentTypeClicked", (ob) =>
            {
                dropdownnDocType.Text = ob;
            });

            FillDoc(doc);
        }

        private void FillDoc(TempDoc doc)
        {
            dropdownnDocType.Text = doc.DocType;
            txtDocTitle.Text = doc.DocTitle;
            txtDocNotes.Text = doc.DocNote;

            imgAttatchment.Source = ImageSource.FromStream(() => doc.DocAttatchment);
        }

        private async void FillDocumentTypes()
        {
            var docTypes = await postflightServices.DocumentTypesAsync();
            Device.BeginInvokeOnMainThread(() => dropdownnDocType.Text = docTypes.Select(x => x.name).FirstOrDefault());
        }

        async void dropdownDocType_Tapped(System.Object sender, System.EventArgs e)
        {
            var docTypes = await postflightServices.DocumentTypesAsync();
            List<string> docTypeNames = new List<string>();
            foreach (var item in docTypes)
            {
                docTypeNames.Add(item.name);
            }

            await PopupNavigation.Instance.PushAsync(new popupDropdown(docTypeNames, "DocumentTypeClicked"));
        }

        private async void AddDoc_Tapped(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet(null, "Cancel", null, "Take Photo", "Choose");

            if (action == "Take Photo")
            {

                try
                {
                    var photo = await MediaPicker.CapturePhotoAsync();

                    if (photo != null)
                    {
                        var stream = await photo.OpenReadAsync();
                        imgAttatchment.Source = ImageSource.FromStream(() => stream);
                        UploadedFileStream = stream;
                    }
                }
                catch (FeatureNotSupportedException)
                {
                    await DisplayAlert("Alert", "Feature Not Supported", "OK");
                }
                catch (PermissionException)
                {
                    await DisplayAlert("Alert", "Camera Permission not Granted", "OK");
                }
            }
            else if (action == "Choose")
            {
                var pickResult = await FilePicker.PickAsync(new PickOptions { PickerTitle = "Pick An Attatchment" });

                if (pickResult != null)
                {
                    var stream = await pickResult.OpenReadAsync();
                    imgAttatchment.Source = ImageSource.FromStream(() => stream);

                    UploadedFileStream = stream;
                }
            }


        }

        private void FabSaveDoc_Clicked(object sender, EventArgs e)
        {
            if (Dcc != null)
            {
                if (UploadedFileStream != null)
                {
                    var doc = new TempDoc
                    {
                        DocTitle = txtDocTitle.Text,
                        DocType = dropdownnDocType.Text,
                        DocNote = txtDocNotes.Text,
                        DocAttatchment = UploadedFileStream
                    };

                    MessagingCenter.Send(doc, "DocUpdated");
                    PopupNavigation.Instance.PopAsync();
                }
                else
                {
                    DisplayAlert("Alert", "Please fill the necessary fields", "OK");
                }
            }
            else
            {
                if (UploadedFileStream != null)
                {
                    var doc = new TempDoc
                    {
                        DocTitle = txtDocTitle.Text,
                        DocType = dropdownnDocType.Text,
                        DocNote = txtDocNotes.Text,
                        DocAttatchment = UploadedFileStream
                    };
                    var ob = doc;

                    MessagingCenter.Send(ob, "DocAdded");

                    PopupNavigation.Instance.PopAsync();
                }
                else
                {
                    DisplayAlert("Alert", "Please fill the necessary fields", "OK");
                }
            }

        }
        private async void btnClose_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }

    public class TempDoc
    {
        public string DocTitle { get; set; }
        public string DocType { get; set; }
        public string DocNote { get; set; }
        public Stream DocAttatchment { get; set; }
    }
}