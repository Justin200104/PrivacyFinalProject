using PrivacyFinalProject.Commands;
using PrivacyFinalProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrivacyFinalProject.ViewModel
{
    internal class CreateAccountViewModel :ViewModelBase
    {
        public ICommand NavigateLoginCommand { get; }

        public CreateAccountViewModel(INavigationService loginNavigationService)
        {
            NavigateLoginCommand = new NavigateCommand(loginNavigationService);
        }
    }
}
