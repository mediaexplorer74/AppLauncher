﻿using appLauncher.Animations;
using appLauncher.Control;
using appLauncher.Core;
using appLauncher.Helpers;
using appLauncher.Model;
using appLauncher.Pages;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Media.Animation;
using Windows.System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace appLauncher
{

    /// <summary>
    /// The page where the apps are displayed. Most of the user interactions with the app launcher will be here.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int maxRows;
        private int maxColumns;
        // public ObservableCollection<finalAppItem> finalApps;
        public static FlipViewItem flipViewTemplate;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        bool pageIsLoaded = false;
        bool backgroundhasbeenset = false;
        public CoreDispatcher coredispatcher;
      
        // Delays updating the app list when the size changes.
        DispatcherTimer sizeChangeTimer = new DispatcherTimer();
        int currentTimeLeft = 0;
        const int updateTimerLength = 100; // milliseconds;
        TimeSpan timeSpan = TimeSpan.FromSeconds(15);
        DispatcherTimer dispatching = new DispatcherTimer();
        int currentindex = 0;



        /// <summary>
        /// Runs when a new instance of MainPage is created
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.SizeChanged += MainPage_SizeChanged;
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            sizeChangeTimer.Tick += SizeChangeTimer_Tick;
            screensContainerFlipView.Items.VectorChanged += Items_VectorChanged;
            backimage.RotationDelay = timeSpan;
                  
         }

       private void Dispatching_Tick(object sender, object e)
        {
            throw new NotImplementedException();
        }

        internal  async void UpdateIndicator(int pagenum)
        {
            await AdjustIndicatorStackPanel(pagenum);

        }

        // Updates grid of apps only when a bit of time has passed 
        //  after changing the size of the window.
        // Better than doing this inside the the flip view item template 
        // since you don't have a timer that's always running anymore.
        private void SizeChangeTimer_Tick(object sender, object e)
        {
            try
            {
                this.screensContainerFlipView.SelectedIndex = (GlobalVariables.pagenum > 0)
                 ? GlobalVariables.pagenum
                : 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ex] SizeChangeTimer_Tick Exception :" + ex.Message);
                Debug.WriteLine("....Details: GlobalVariables.pagenum = " + GlobalVariables.pagenum);
                Debug.WriteLine(".............screensContainerFlipView.SelectedIndex = " 
                    + this.screensContainerFlipView.SelectedIndex);

            }

            if (currentTimeLeft == 0)
            {
                currentTimeLeft = 0;
                sizeChangeTimer.Stop();
                maxRows = GlobalVariables.NumofRoworColumn(12, 84, 
                    (int)screensContainerFlipView.ActualHeight);
                maxColumns = GlobalVariables.NumofRoworColumn(12, 64, 
                    (int)screensContainerFlipView.ActualWidth);

                GlobalVariables.columns = maxColumns;
                GlobalVariables.appsperscreen=maxColumns*maxRows;
                int additionalPagesToMake = calculateExtraPages(GlobalVariables.appsperscreen) - 1;
                int fullPages = additionalPagesToMake;
                int appsLeftToAdd = AllApps.listOfApps.Count() - 
                    (fullPages * GlobalVariables.appsperscreen);

                if (appsLeftToAdd > 0)
                {
                    additionalPagesToMake += 1;
                }
               if (additionalPagesToMake > 0)
                {
                    screensContainerFlipView.Items.Clear();
                    for (int i = 0; i < additionalPagesToMake; i++)
                    {
                        screensContainerFlipView.Items.Add(i);
                    }
                }

                this.InvalidateArrange();
         
            }
            else
            {
                currentTimeLeft -= (int)sizeChangeTimer.Interval.TotalMilliseconds;
            }
            
        }

        internal object getFlipview()
        {
            return screensContainerFlipView;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!sizeChangeTimer.IsEnabled)
            {
                sizeChangeTimer.Interval = TimeSpan.FromMilliseconds(updateTimerLength / 10);
                sizeChangeTimer.Start();
            }
            currentTimeLeft = updateTimerLength;

        }

        private async void Items_VectorChanged(IObservableVector<object> sender, 
            IVectorChangedEventArgs @event)
        {
            var collection = sender;
            int count = collection.Count;

            flipViewIndicatorStackPanel.Children.Clear();

            for (int i = 0; i < count; i++)
            {
                flipViewIndicatorStackPanel.Children.Add(new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = new SolidColorBrush(Colors.Gray),
                    Margin = new Thickness(4, 0, 4, 0)
                });

            };

        await AdjustIndicatorStackPanel(GlobalVariables.pagenum);
        }

        //private async void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        //      {
        //	if (AllAppsGrid.Visibility == Visibility.Visible)
        //	{
        //		DesktopBackButton.HideBackButton();
        //		e.Handled = true;
        //		await Task.WhenAll(
        //		AllAppsGrid.Fade(0).StartAsync(),
        //		AppListViewGrid.Blur(0).StartAsync());
        //		AllAppsGrid.Visibility = Visibility.Collapsed;
        //	}
        //}

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await this.Scale(2f, 2f, (float)this.ActualWidth / 2, 
                (float)this.ActualHeight / 2, 0).StartAsync();
            await this.Scale(1, 1, (float)this.ActualWidth / 2, 
                (float)this.ActualHeight / 2, 300).StartAsync();
        }

        /// <summary>
        /// When an app is selected, the launcher will attempt to launch the selected app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void appGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedApp = (finalAppItem)e.ClickedItem;
            bool isLaunched = await clickedApp.appEntry.LaunchAsync();
            if (isLaunched == false)
            {
                Debug.WriteLine("Error: App not launched!");
            }
        }

        /// <summary>
        /// Runs when the page has loaded
        /// <para> Sorts all of the apps into pages based on how
        /// based on the size of the app window/device's screen resolution.
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await GlobalVariables.LoadBackgroundImages();

            try
            {
                this.screensContainerFlipView.SelectedIndex = (GlobalVariables.pagenum > 0)
                    ? GlobalVariables.pagenum : 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ex] Page_Loaded Exception: " + ex.Message);
            }

            maxRows = GlobalVariables.NumofRoworColumn(12, 84,
                (int)screensContainerFlipView.ActualHeight);
            maxColumns = GlobalVariables.NumofRoworColumn(12, 64,
                (int)screensContainerFlipView.ActualWidth);

            GlobalVariables.columns = maxColumns;
            GlobalVariables.appsperscreen = maxColumns * maxRows;
            int additionalPagesToMake = calculateExtraPages(GlobalVariables.appsperscreen) - 1;
            int fullPages = additionalPagesToMake;
           int appsLeftToAdd = AllApps.listOfApps.Count() - 
                (fullPages * GlobalVariables.appsperscreen);

            if (appsLeftToAdd > 0)
            {
                additionalPagesToMake += 1;
            }

            //NOTE: I wasn't able to create an ItemTemplate from C# so I made a GridView
            //in the XAML view with the desired values and used its 
            //item template to create identical GridViews.

            //If you know how to create ItemTemplates in C#, please make a pull request which
            //with a new solution for this issue or contanct me directly. 
            // It would make things way easier for everyone!
            //  DataTemplate theTemplate = appGridView.ItemTemplate;


            //Following code creates any extra app pages then adds apps to each page.
            if (additionalPagesToMake > 0)
            {
                //ControlTemplate template = new appControl().Template;

                for (int i = 0; i < additionalPagesToMake; i++)
                {
                    //screensContainerFlipView.Items.Add(new FlipViewItem()
                    //{
                    //    Content = new GridView()
                    //    {
                    //        ItemTemplate = theTemplate,
                    //        ItemsPanel = appGridView.ItemsPanel,
                    //        HorizontalAlignment = HorizontalAlignment.Center,
                    //        IsItemClickEnabled = true,
                    //        Margin = new Thickness(0, 10, 0, 0),
                    //        SelectionMode = ListViewSelectionMode.None

                    //    }

                    //});
                    screensContainerFlipView.Items.Add(i);
                }


                //        int j = i + 2;
                //        {
                //            var flipViewItem = (FlipViewItem)screensContainerFlipView.Items[j];
                //            var gridView = (GridView)flipViewItem.Content;
                //            gridView.ItemClick += appGridView_ItemClick;
                //        }

                //    }
                //    int start = 0;
                //    int end = appsPerScreen;

                //    for (int j = 1; j < fullPages + 1; j++)
                //    {

                //        FlipViewItem screen = (FlipViewItem)screensContainerFlipView.Items[j];
                //        GridView gridOfApps = (GridView)screen.Content;
                //        addItemsToGridViews(gridOfApps, start, end);
                //        if (j == 1)
                //        {
                //            start = appsPerScreen + 1;
                //            end += appsPerScreen + 1;
                //        }
                //        else
                //        {
                //            start += appsPerScreen;
                //            end += appsPerScreen;
                //        }
                //    }


                //    int startOfLastAppsToAdd = finalApps.Count() - appsLeftToAdd;


                //    FlipViewItem finalScreen = (FlipViewItem)screensContainerFlipView.Items[additionalPagesToMake + 1];
                //    GridView finalGridOfApps = (GridView)finalScreen.Content;
                //    addItemsToGridViews(finalGridOfApps, startOfLastAppsToAdd, finalApps.Count());
                //    screensContainerFlipView.SelectedItem = screensContainerFlipView.Items[1];
                //    AdjustIndicatorStackPanel(1);
                //}
                //else
                //{
                //    for (int i = 0; i < finalApps.Count() - 1; i++)
                //    {
                //        appGridView.Items.Add(finalApps[i]);
                //    }
                //}
                         
                //  pageIsLoaded = true;
                screensContainerFlipView.SelectionChanged += FlipViewMain_SelectionChanged;
                


            }
            this.screensContainerFlipView.SelectedIndex = (GlobalVariables.pagenum > 0) 
                ? GlobalVariables.pagenum 
                : 0;

            await AdjustIndicatorStackPanel(GlobalVariables.pagenum);
        }

        /// <summary>
        /// Loads local settings e.g. loads background image if it's available.
        /// </summary>
     

        /// <summary>
        /// Attempts to disable vertical scrolling.
        /// </summary>
        /// <param name="gridView"></param>
        private void disableScrollViewer(GridView gridView)
        {
            try
            {
                var border = (Border)VisualTreeHelper.GetChild(gridView, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.IsVerticalRailEnabled = false;
                scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }

            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Returns result of calculation of extra pages needed to be added.
        /// </summary>
        /// <param name="appsPerScreen"></param>
        /// <returns></returns>
        private int calculateExtraPages(int appsPerScreen)
        {
            double appsPerScreenAsDouble = appsPerScreen;
            double numberOfApps = AllApps.listOfApps.Count();
            int pagesToMake = (int)Math.Ceiling(numberOfApps / appsPerScreenAsDouble);
            return pagesToMake;
        }

        /// <summary>
        /// Adds apps to an app page
        /// </summary>
        /// <param name="gridOfApps"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        //private void addItemsToGridViews(GridView gridOfApps, int start, int end)
        //{
        //    for (int k = start; k < end; k++)
        //    {
        //        gridOfApps.Items.Add(finalApps[k]);
        //    }
        //}


        /// <summary>
        /// (Not implemented yet) Will attempt to launch an app in the launcher dock.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dockGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //TODO: try cast object as appItem then launch the app
        }

        /// <summary>
        /// Runs when launcher settings is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("You clicked on the settings icon");
            Frame.Navigate(typeof(settings));
        }

        /// <summary>
        /// Runs whenever app the the selected FlipViewItem has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private async void screensContainerFlipView_SelectionChanged(object sender, 
        // SelectionChangedEventArgs e)
        //{
        //    if (pageIsLoaded)
        //    {
        //        int SelectedIndex = screensContainerFlipView.SelectedIndex;
        //        if (SelectedIndex == 0)
        //        {
        //            //Swipe Right for Cortana!
        //            await Launcher.LaunchUriAsync(new Uri("ms-cortana://"));
        //            screensContainerFlipView.SelectedIndex = 1;
        //           await AdjustIndicatorStackPanel(screensContainerFlipView.SelectedIndex);
        //        }
        //        else
        //        {
        //            await AdjustIndicatorStackPanel(SelectedIndex);

        //        }

        //    }
        //}

        // AdjustIndicatorStackPanel
        private async Task AdjustIndicatorStackPanel(int selectedIndex)
        {
            var indicator = flipViewIndicatorStackPanel;
            Ellipse ellipseToAnimate = new Ellipse();
            for (int i = 0; i < indicator.Children.Count; i++)
            {
                if (i == selectedIndex)
                {
                    var ellipse = (Ellipse)indicator.Children[i];
                    ellipseToAnimate = ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.Orange);

                }
                else
                {
                    var ellipse = (Ellipse)indicator.Children[i];
                    ellipse.Fill = 
                        (SolidColorBrush)App.Current.Resources["DefaultTextForegroundThemeBrush"];

                }
            }
            float centerX = (float)ellipseToAnimate.ActualWidth / 2;
            float centerY = (float)ellipseToAnimate.ActualHeight / 2;
            float animationScale = 1.7f;

            double duration = 300;
            if (IndicatorAnimation.oldAnimatedEllipse != null)
            {
                await Task.WhenAll(ellipseToAnimate.Scale(animationScale, animationScale, 
                    centerX, centerY, duration, easingType: EasingType.Back).StartAsync(),
                    IndicatorAnimation.oldAnimatedEllipse.Scale(1, 1, centerX, centerY,
                    duration, easingType: EasingType.Back).StartAsync());

            }
            else
            {
                await ellipseToAnimate.Scale(animationScale, animationScale, centerX, 
                    centerY, duration, easingType: EasingType.Bounce).StartAsync();
            }

            IndicatorAnimation.oldAnimatedEllipse = ellipseToAnimate;
        }

        /// <summary>
        /// Ensures expected behaviour when using the launcher with a touch screen input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            for (int i = 1; i < screensContainerFlipView.Items.Count; i++)
            {

                //FlipViewItem screen = (FlipViewItem)screensContainerFlipView.Items[i];
                //GridView gridOfApps = (GridView)screen.Content;
                //disableScrollViewer(screensContainerFlipView);
            }
        }

        // allAppsButton_Tapped
        private void allAppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPage));

        }//allAppsButton_Tapped


        // Filterby_SelectionChanged
        private void Filterby_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selected = ((ComboBoxItem)Filterby.SelectedItem).Content.ToString();
            switch (selected)
            {
                case "AtoZ":
                    {
                        var te = AllApps.Allpackages.OrderBy(x => x.Key.DisplayInfo.DisplayName);
                        ObservableCollection<finalAppItem> items 
                            = new ObservableCollection<finalAppItem>();

                        foreach (var item in te)
                        {
                            items.Add(new finalAppItem
                            {
                                appEntry = item.Key,
                                appLogo = 
                                  AllApps.listOfApps.First(x => x.appEntry == item.Key).appLogo
                            });
                        }
                        AllApps.listOfApps = items;
                    }

                    break;
                case "Developer":
                    {

                        var te = AllApps.Allpackages.OrderBy(x => x.Value.Id.Publisher);
                        ObservableCollection<finalAppItem> items = 
                            new ObservableCollection<finalAppItem>();
                        foreach (var item in te)
                        {
                            items.Add(new finalAppItem
                            {
                                appEntry = item.Key,
                                appLogo = AllApps.listOfApps.First(x => x.appEntry == item.Key).appLogo
                            });
                        }
                        AllApps.listOfApps = items;
                    }
                    break;
                case "Installed":
                    {
                        var te = AllApps.Allpackages.OrderBy(x => x.Value.InstalledDate);
                        ObservableCollection<finalAppItem> items = 
                            new ObservableCollection<finalAppItem>();

                        foreach (var item in te)
                        {
                            items.Add(new finalAppItem
                            {
                                appEntry = item.Key,
                                appLogo = AllApps.listOfApps.First(
                                    x => x.appEntry == item.Key).appLogo
                            });
                        }
                        AllApps.listOfApps = items;
                    }
                    break;
                default:
                    break;
            }
            this.Frame.Navigate(typeof(appLauncher.MainPage));
        }

        // FlipViewMain_SelectionChanged
        private void FlipViewMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalVariables.pagenum =((FlipView)sender).SelectedIndex;
            if (e.AddedItems.Count > 0)
            {
                var flipViewItem = screensContainerFlipView.ContainerFromIndex(
                    screensContainerFlipView.SelectedIndex);

                appControl userControl = FindFirstElementInVisualTree<appControl>(flipViewItem);
                userControl.SwitchedToThisPage();
            }
            if (e.RemovedItems.Count > 0)
            {
                var flipViewItem = screensContainerFlipView.ContainerFromItem(e.RemovedItems[0]);
                appControl userControl = FindFirstElementInVisualTree<appControl>(flipViewItem);
                userControl.SwitchedFromThisPage();
            }
            AdjustIndicatorStackPanel(GlobalVariables.pagenum);
        }

        // FindFirstElementInVisualTree
        private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                        return result;

                }
            }
            return null;
        }

        // Backflip_SelectionChanged
        //private void Backflip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.AddedItems.Count <= 0) return;
        //    if (e.RemovedItems.Count <= 0) return;

        //    var newSelectedItem =(FlipViewItem) Backflip.ContainerFromItem(e.AddedItems?.FirstOrDefault());
        //    var previousSelectedItem = (FlipViewItem)Backflip.ContainerFromItem(e.RemovedItems?.FirstOrDefault());

        //    if (newSelectedItem == null) return;
        //    if (previousSelectedItem == null) return;

        //    var duration = new Duration(TimeSpan.FromMilliseconds(500));

        //    var hideAnimation = new DoubleAnimation
        //    {
        //        From = 1.0,
        //        To = 0.0,
        //        AutoReverse = false,
        //        Duration = duration
        //    };

        //    var hideSb = new Storyboard();
        //    hideSb.Children.Add(hideAnimation);
        //    Storyboard.SetTargetProperty(hideSb, "Opacity");
        //    Storyboard.SetTarget(hideSb, previousSelectedItem);

        //    hideSb.Begin();

        //    var showAnimation = new DoubleAnimation
        //    {
        //        From = 0.0,
        //        To = 1.0,
        //        AutoReverse = false,
        //        Duration = duration
        //    };

        //    var showSb = new Storyboard();
        //    showSb.Children.Add(showAnimation);
        //    Storyboard.SetTargetProperty(showSb, "Opacity");
        //    Storyboard.SetTarget(showSb, newSelectedItem);

        //    showSb.Begin();
        //}

    }//class MaiPage end

}//namespace appLauncher end
