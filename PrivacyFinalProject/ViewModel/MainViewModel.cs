using PrivacyFinalProject.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyFinalProject.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly ModelNavigationStore _modelNavigationStore;

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
        public ViewModelBase CurrentModalViewModel => _modelNavigationStore.CurrentViewModel;
        public bool IsOpen => _modelNavigationStore.IsOpen;

        public MainViewModel(NavigationStore navigationStore, ModelNavigationStore modelNavigationStore)
        {
            _navigationStore = navigationStore;
            _modelNavigationStore = modelNavigationStore;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _modelNavigationStore.CurrentViewModelChanged += OnCurrentModelViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void OnCurrentModelViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentModalViewModel));
            OnPropertyChanged(nameof(IsOpen));
        }
    }
}
