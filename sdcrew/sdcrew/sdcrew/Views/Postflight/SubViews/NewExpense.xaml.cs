using System;
using System.Collections.Generic;

using sdcrew.Services.Data;

using Xamarin.Forms;

namespace sdcrew.Views.Postflight.SubViews
{
    public partial class NewExpense : ContentView
    {


        public NewExpense()
        {
            InitializeComponent();
        }

        PostflightServices postflightServices;
        int ParentGridRow = 0;

        public NewExpense(int expenseGridRow)
        {
            InitializeComponent();

            postflightServices = new PostflightServices();
            ParentGridRow = expenseGridRow;
        }

        void delete_Tapped(System.Object sender, System.EventArgs e)
        {
            MessagingCenter.Send(ParentGridRow.ToString(), "ExpenseRemoved");
        }

        void edit_Tapped(System.Object sender, System.EventArgs e)
        {
            MessagingCenter.Send<App>((App)Application.Current, "ExpenseAdded");

        }
    }
}
