﻿using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace DockerPowerTools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.Initialize();
        }
    }
}
