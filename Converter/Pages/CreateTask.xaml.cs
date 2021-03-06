﻿using Converter.Classes;
using Converter.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Converter.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CreateTask : Page
    {
        private const string CreationFailed = "CreateTask_CreationFailed";
        private const string SpecifyConfig = "CreateTask_SpecifyConfig";
        private const string SpecifyInput = "CreateTask_SpecifyInput";
        private const string SpecifyOutput = "CreateTask_SpecifyOutput";
        private const string CannotStart = "CreateTask_CannotStart";

        public CreateTask()
        {
            this.InitializeComponent();

            MediaPlayer mediaPlayer = new MediaPlayer();
            inputFilePreview.SetMediaPlayer(mediaPlayer);
            mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

            inputFilePicker.OpenFileFilters = SupportedFormats.InputFileTypes();

            outputFilePicker.SaveFileChoices = new List<KeyValuePair<string, IList<string>>>()
            {
                (formatPicker.SelectedConfiguration ?? new TranscodeConfiguration()).SaveChoice()
            };
        }

        private async void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                inputFilePreview.Visibility = Visibility.Visible;
            });
        }

        private async void MediaPlayer_MediaFailed(Windows.Media.Playback.MediaPlayer sender, Windows.Media.Playback.MediaPlayerFailedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                inputFilePreview.Visibility = Visibility.Collapsed;
                var loader = new ResourceLoader();
                ContentDialog dialog = new ContentDialog
                {
                    Content = loader.GetString("CreateTask_FileNotSupported"),
                    PrimaryButtonText = loader.GetString("Dialog_Ok")
                };
                await dialog.ShowAsync();
            });
        }

        private void FormatPicker_SelectedConfigChanged(object sender, EventArgs newConfig)
        {
            var formatPicker = sender as TranscodeConfigPicker;
            outputFilePicker.SaveFileChoices = new List<KeyValuePair<string, IList<string>>>()
            {
                (formatPicker?.SelectedConfiguration
                ?? new TranscodeConfiguration()).SaveChoice()
            };
        }

        private async void SubmitButtion_Click(object sender, RoutedEventArgs e)
        {
            loadingControl.IsLoading = true;
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            try
            {
                var source = inputFilePicker.SelectedItem as StorageFile;
                if (source == null)
                {
                    throw new Exception(loader.GetString(SpecifyInput));
                }

                var config = formatPicker.SelectedConfiguration;
                if (config == null)
                {
                    throw new Exception(loader.GetString(SpecifyConfig));
                }

                var destination = outputFilePicker.SelectedItem as StorageFile;
                if (destination == null)
                {
                    throw new Exception(loader.GetString(SpecifyOutput));
                }

                TranscodeTask task = new TranscodeTask(source, destination, config);
                await task.PrepareAsync();
                if (task.Status != TranscodeTask.TranscodeStatus.ReadyToStart)
                {
                    throw new Exception(loader.GetString(CannotStart));
                }
                else
                {
                    task.StartTranscode();
                    TranscodeTaskManager.Current.Tasks.Add(task);
                    Frame.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.Message, loader.GetString(CreationFailed));
                var result = dialog.ShowAsync();
            }
            finally
            {
                loadingControl.IsLoading = false;
            }
        }

        private void InputFilePicker_SelectionChanged(object sender, EventArgs e)
        {
            var selectedFile = inputFilePicker.SelectedItem as StorageFile;
            if (selectedFile == null)
            {
                inputFilePreview.Source = null;
                outputFilePicker.SuggestedFileName = string.Empty;
            }
            else
            {
                inputFilePreview.Source = MediaSource.CreateFromStorageFile(selectedFile);
                outputFilePicker.SuggestedFileName = selectedFile.DisplayName ?? string.Empty;
            }
        }
    }
}