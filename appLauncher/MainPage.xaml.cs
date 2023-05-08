using appLauncher.Helpers;
using appLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace appLauncher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //public ObservableCollection<AppListEntry> allApps = new ObservableCollection<AppListEntry>(); // old

        private int maxRows;
        public ObservableCollection<finalAppItem> finalApps;
        public ObservableCollection<finalAppItem> queriedApps = new ObservableCollection<finalAppItem>();
        public static FlipViewItem flipViewTemplate;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        bool pageIsLoaded = false;
        public MainPage()
        {
            this.InitializeComponent();
            //allApps
            finalApps= packageHelper.getAllApps();
            //Debug.Write("Success");

            //finalApps = finalAppItem.listOfApps;
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            //SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            foreach (var item in finalApps)
            {
                queriedApps.Add(item);
                //allApps.Add(item);
            }
            //screensContainerFlipView.Items.VectorChanged += Items_VectorChanged;
        }

        

        private async void appGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedApp = (AppListEntry)e.ClickedItem;
            await clickedApp.LaunchAsync();
        }
    }
}
