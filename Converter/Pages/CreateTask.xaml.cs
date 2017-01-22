﻿using Converter.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Converter.Controls;
using Windows.Storage;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Converter.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CreateTask : Page
    {
        public CreateTask()
        {
            this.InitializeComponent();

            inputFilePicker.OpenFileFilters = SupportedInputFiles.GetList();

            outputFilePicker.SaveFileChoices = new List<KeyValuePair<string, IList<string>>>()
            {
                (formatPicker.SelectedConfiguration ?? new TranscodeConfiguration()).SaveChoice()
            };
        }

        private void FormatPicker_SelectedConfigChanged(TranscodeConfigPicker sender, TranscodeConfiguration newConfig)
        {
            outputFilePicker.SaveFileChoices = new List<KeyValuePair<string, IList<string>>>()
            {
                (newConfig ?? new TranscodeConfiguration()).SaveChoice()
            };
        }

        private async void submitButtion_Click(object sender, RoutedEventArgs e)
        {
            var config = formatPicker.SelectedConfiguration;
            if (config == null) throw new Exception();

            var source = inputFilePicker.SelectedItem as StorageFile;
            if (source == null) throw new Exception();
            var destination = outputFilePicker.SelectedItem as StorageFile;
            if (destination == null) throw new Exception();

            TranscodeTask task = new TranscodeTask(source, destination, config);
            await task.PrepareAsync();
            if (task.Status == TranscodeTask.TranscodeStatus.ReadyToStart)
            {
                task.StartTranscode();
                TranscodingManager.Tasks.Add(task);
                Frame.GoBack();
            }
        }
    }
}