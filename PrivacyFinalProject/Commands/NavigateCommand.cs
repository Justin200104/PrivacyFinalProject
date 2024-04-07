using PrivacyFinalProject.Services;
using PrivacyFinalProject.Stores;
using PrivacyFinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyFinalProject.Commands
{
    internal class NavigateCommand : CommandBase
    {
        private readonly INavigationService _navigationService;

        public NavigateCommand(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Execute(object parameter)
        {
            _navigationService.Navigate();
        }
    }
}