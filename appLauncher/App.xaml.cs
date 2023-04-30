﻿using appLauncher.Core.Helpers;
using appLauncher.Core.Pages;

using Microsoft.AppCenter.Analytics;

using Microsoft.AppCenter.Crashes;

using System;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using Application = Windows.UI.Xaml.Application;

namespace appLauncher
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            App.Current.UnhandledException += App_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //    Dictionary<string, string> result = new Dictionary<string, string>();
            //    result.Add("UnhandedExceptionmessage", e.Exception.Message);
            //    result.Add("StackTrace", e.Exception.StackTrace);
            //    result.Add("TargetSite", e.Exception.TargetSite.Name);
            //    result.Add("ExceptionSource", e.Exception.Source);
            //    result.AddRange((IEnumerable<KeyValuePair<string, string>>)e.Exception.Data);
            Analytics.TrackEvent("Background Task unhandled exception");
            Crashes.TrackError(e.Exception);
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            //Dictionary<string, string> result = new Dictionary<string, string>();
            //result.Add("Unhandedmessage", e.Message);
            //result.Add("UnhandedExceptionmessage", e.Exception.Message);
            //result.Add("StackTrace", e.Exception.StackTrace);
            //result.Add("TargetSite", e.Exception.TargetSite.Name);
            //result.Add("ExceptionSource", e.Exception.Source);
            //result.AddRange((IEnumerable<KeyValuePair<string, string>>)e.Exception.Data);
            Analytics.TrackEvent("Unhandled Error occurred");
            Crashes.TrackError(e.Exception);
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            try
            {
                SettingsHelper.ConfigureAppCenter();
                await SettingsHelper.LoadAppSettingsAsync();
                await SettingsHelper.CheckAppSettings();
                Analytics.TrackEvent("Application has been launched");
                //Extends view into status bar/title bar, depending on the device used.
                var appView = ApplicationView.GetForCurrentView();
                appView.SetPreferredMinSize(new Size(360, 360));
                appView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                SettingsHelper.SetApplicationResources();
                if (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Desktop")
                {
                    appView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                    appView.TitleBar.BackgroundColor = Colors.Transparent;
                    CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                }

                if (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile")
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

                }


                Frame rootFrame = Window.Current.Content as Frame;



                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new Frame();
                    rootFrame.NavigationFailed += OnNavigationFailed;
                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;
                    if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                    {
                        bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
                        splashScreen extendedSplash = new splashScreen(e.SplashScreen, loadState, ref rootFrame);
                        rootFrame.Content = extendedSplash;
                        Window.Current.Content = rootFrame;
                    }
                }
                if (e.PrelaunchActivated == false)
                {
                    if (rootFrame.Content == null)
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        rootFrame.Navigate(typeof(MainPage), e.Arguments);
                    }
                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Application crashed on launch");
                Crashes.TrackError(es);
            }
        }
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            try
            {
                await packageHelper.SaveCollectionAsync();
                await ImageHelper.SaveImageOrder();
                SettingsHelper.totalAppSettings.ImagesLoaded = false;
                await SettingsHelper.SaveAppSettingsAsync();
            }
            catch (Exception es)
            {
                Crashes.TrackError(es);
                Analytics.TrackEvent("Crashed during shutdown");
            }
            Analytics.TrackEvent("Application shutting down");
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
