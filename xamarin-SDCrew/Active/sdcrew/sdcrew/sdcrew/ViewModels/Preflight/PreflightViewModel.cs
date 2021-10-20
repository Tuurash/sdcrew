using sdcrew.Models;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace sdcrew.ViewModels.Preflight
{
    public class PreflightViewModel:BaseViewModel
    {
        PreflightServices _preflightServices;
        public AsyncCommand UpdatePreflightCommand { get; }

        public PreflightViewModel()
        {
            _preflightServices = new PreflightServices();
        }

    }
}
