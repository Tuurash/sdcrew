using sdcrew.Models;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace sdcrew.ViewModels.Preflight
{
    public class PreflightDetailsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        PreflightServices _preflightServices;
        public PreflightDetailsViewModel()
        {
            _preflightServices = new PreflightServices();
        }

         
    }
}
