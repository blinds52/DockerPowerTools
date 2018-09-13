﻿using GalaSoft.MvvmLight;

namespace DockerPowerTools.RegistryExplorer.ViewModel
{
    public class TagViewModel : ViewModelBase
    {
        private bool _isSelected;

        public TagViewModel(string repository, string tag, string registry)
        {
            Repository = repository;
            Tag = tag;
            Registry = registry;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value; 
                RaisePropertyChanged();
            }
        }

        public string Registry { get; }

        public string Repository { get; }

        public string Tag { get; }
    }
}