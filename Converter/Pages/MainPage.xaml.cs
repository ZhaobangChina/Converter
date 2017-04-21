﻿using Converter.Classes;
using Converter.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Converter
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            taskListView.DataContext = TranscodeTaskListViewModel.Current.TaskViewModels;
        }

        private void CreateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.CreateTask));
        }

        private void CreateMultipleButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.CreateMultipleTasks));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.AboutPage));
        }
    }
}
