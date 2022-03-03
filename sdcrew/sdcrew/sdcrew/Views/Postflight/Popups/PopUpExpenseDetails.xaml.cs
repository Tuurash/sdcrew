using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using sdcrew.Services.Data;
using sdcrew.Views.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Postflight.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopUpExpenseDetails : PopupPage
    {
        PostflightServices svm = new PostflightServices();

        public PopUpExpenseDetails()
        {
            InitializeComponent();
            Loader.IsVisible = false;

            if (dropdownExpenseType.Text == "01-Fuel")
            {
                FuelExpenseDetails.IsVisible = true;
            }
            else
            {
                FuelExpenseDetails.IsVisible = false;
            }

            MessagingCenter.Subscribe<string>(this, "ExpenseCategorySelected", (ob) =>
            {
                dropdownExpenseType.Text = ob;

                if (dropdownExpenseType.Text == "01-Fuel")
                {
                    FuelExpenseDetails.IsVisible = true;
                }
                else
                {
                    FuelExpenseDetails.IsVisible = false;
                }
            });

            MessagingCenter.Subscribe<string>(this, "PaymentTypeSelected", (ob) =>
            {
                dropdownPaymentType.Text = ob;
            });

            MessagingCenter.Subscribe<string>(this, "QuantityTypeSelected", (ob) =>
            {
                dropdownUnit.Text = ob;
            });
        }

        private async void dropdownExpenseType_Tapped(object sender, EventArgs e)
        {
            var expenseCatagories = await svm.ExpenseCategoriesAsync();

            List<string> expenseCatagoryNames = new List<string>();
            foreach (var item in expenseCatagories)
            {
                expenseCatagoryNames.Add(item.Name);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new popupDropdown(expenseCatagoryNames, "ExpenseCategorySelected")));
        }

        private async void dropdownPaymentType_Tapped(object sender, EventArgs e)
        {
            var paymentTypes = await svm.PaymentTypesAsync();

            List<string> paymentTypeNames = new List<string>();
            foreach (var item in paymentTypes)
            {
                paymentTypeNames.Add(item.Name);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new popupDropdown(paymentTypeNames, "PaymentTypeSelected")));
        }

        private async void dropdownUnit_Tapped(object sender, EventArgs e)
        {
            var quantityTypes = await svm.QuantityTypesAsync();

            List<string> quantityTypeNames = new List<string>();
            foreach (var item in quantityTypes)
            {
                quantityTypeNames.Add(item.Name);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new popupDropdown(quantityTypeNames, "QuantityTypeSelected")));
        }

        private async void fileReciept_Tapped(object sender, EventArgs e)
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
                        imgfileReciept.Source = ImageSource.FromStream(() => stream);

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
                    imgfileReciept.Source = ImageSource.FromStream(() => stream);
                }
            }
        }

        private async void btnClosePopup_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }

        private void FabSaveExpense_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<App>((App)Application.Current, "ExpenseAdded");
            Task.Delay(10);
            PopupNavigation.Instance.PopAsync();
        }
    }
}