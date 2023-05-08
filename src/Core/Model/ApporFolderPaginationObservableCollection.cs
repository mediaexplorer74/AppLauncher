using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace appLauncher.Core.Model
{
    public class ApporFolderPaginationObservableCollection : ObservableCollection<ApporFolderBase>
    {
        private ObservableCollection<ApporFolderBase> originalCollection;
        [NonSerialized]
        private int Page;
        [NonSerialized]
        private int CountPerPage;
        private int startIndex;
        private int endIndex;

        public ApporFolderPaginationObservableCollection(IEnumerable<ApporFolderBase> collection) : base(collection)
        {

            Page = SettingsHelper.totalAppSettings.LastPageNumber;
            CountPerPage = SettingsHelper.totalAppSettings.AppsPerPage;
            startIndex = Page * CountPerPage;
            endIndex = startIndex + CountPerPage;
            originalCollection = new ObservableCollection<ApporFolderBase>(collection);
            RecalculateThePageItems();
            GlobalVariables.PageNumChanged += PageChanged;
            GlobalVariables.NumofApps += SizedChanged;
        }

        private void RecalculateThePageItems()
        {
            ClearItems();


            for (int i = startIndex; i < endIndex; i++)
            {
                if (originalCollection.Count > i)
                    base.InsertItem(i - startIndex, originalCollection[i]);
            }
        }

        public int GetIndexApp(ApporFolderBase app)
        {
            return originalCollection.IndexOf(app);
        }
        public void MoveApp(int initailindex, int newindex)
        {
            originalCollection.Move(initailindex, newindex);
            RecalculateThePageItems();
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));

        }
        public void GetFilteredApps(string selected)
        {
            int loc = 0;
            List<ApporFolderBase> orderlist = new List<ApporFolderBase>();
            List<AppTile> orderedapps = new List<AppTile>();
            List<AppFolder> folders = new List<AppFolder>();
            switch (selected)
            {
                case "AppAZ":
                    orderedapps = originalCollection.Cast<AppTile>().OrderBy(x => x.AppName).ToList();
                    folders = originalCollection.Cast<AppFolder>().ToList();
                    for (int i = 0; i < originalCollection.Count; i++)
                    {
                        if (folders.Any(x => x.ListPosition == i))
                        {
                            orderlist.Add(folders.Find(y => y.ListPosition == i));
                            continue;
                        }
                        else
                        {

                            var c = orderedapps.First();
                            c.ListPosition = i;
                            orderlist.Add(c);
                            orderedapps.Remove(c);
                        }
                    }
                    originalCollection = new ObservableCollection<ApporFolderBase>(orderlist);


                    break;
                case "AppZA":
                    orderedapps = originalCollection.Cast<AppTile>().OrderByDescending(x => x.AppName).ToList();
                    folders = originalCollection.Cast<AppFolder>().ToList();
                    for (int i = 0; i < originalCollection.Count; i++)
                    {
                        if (folders.Any(x => x.ListPosition == i))
                        {
                            orderlist.Add(folders.Find(y => y.ListPosition == i));
                            continue;
                        }
                        else
                        {

                            var c = orderedapps.First();
                            c.ListPosition = i;
                            orderlist.Add(c);
                            orderedapps.Remove(c);
                        }
                    }
                    originalCollection = new ObservableCollection<ApporFolderBase>(orderlist);
                    break;
                case "DevAZ":
                    orderedapps = originalCollection.Cast<AppTile>().OrderBy(x => x.AppDeveloper).ToList();
                    folders = originalCollection.Cast<AppFolder>().ToList();
                    for (int i = 0; i < originalCollection.Count; i++)
                    {
                        if (folders.Any(x => x.ListPosition == i))
                        {
                            orderlist.Add(folders.Find(y => y.ListPosition == i));
                            continue;
                        }
                        else
                        {

                            var c = orderedapps.First();
                            c.ListPosition = i;
                            orderlist.Add(c);
                            orderedapps.Remove(c);
                        }
                    }
                    originalCollection = new ObservableCollection<ApporFolderBase>(orderlist);
                    break;
                case "DevZA":
                    orderedapps = originalCollection.Cast<AppTile>().OrderByDescending(x => x.AppDeveloper).ToList();
                    folders = originalCollection.Cast<AppFolder>().ToList();
                    for (int i = 0; i < originalCollection.Count; i++)
                    {
                        if (folders.Any(x => x.ListPosition == i))
                        {
                            orderlist.Add(folders.Find(y => y.ListPosition == i));
                            continue;
                        }
                        else
                        {

                            var c = orderedapps.First();
                            c.ListPosition = i;
                            orderlist.Add(c);
                            orderedapps.Remove(c);
                        }
                    }
                    originalCollection = new ObservableCollection<ApporFolderBase>(orderlist);
                    break;
                case "InstalledNewest":
                    orderedapps = originalCollection.Cast<AppTile>().OrderByDescending(x => x.InstalledDate).ToList();
                    folders = originalCollection.Cast<AppFolder>().ToList();
                    for (int i = 0; i < originalCollection.Count; i++)
                    {
                        if (folders.Any(x => x.ListPosition == i))
                        {
                            orderlist.Add(folders.Find(y => y.ListPosition == i));
                            continue;
                        }
                        else
                        {

                            var c = orderedapps.First();
                            c.ListPosition = i;
                            orderlist.Add(c);
                            orderedapps.Remove(c);
                        }
                    }
                    originalCollection = new ObservableCollection<ApporFolderBase>(orderlist);
                    break;
                case "InstalledOldest":
                    orderedapps = originalCollection.Cast<AppTile>().OrderBy(x => x.InstalledDate).ToList();
                    folders = originalCollection.Cast<AppFolder>().ToList();
                    for (int i = 0; i < originalCollection.Count; i++)
                    {
                        if (folders.Any(x => x.ListPosition == i))
                        {
                            orderlist.Add(folders.Find(y => y.ListPosition == i));
                            continue;
                        }
                        else
                        {

                            var c = orderedapps.First();
                            c.ListPosition = i;
                            orderlist.Add(c);
                            orderedapps.Remove(c);
                        }
                    }
                    originalCollection = new ObservableCollection<ApporFolderBase>(orderlist);
                    // new ObservableCollection<Apps>(orderlist);
                    break;

                default:
                    return;


            }

            RecalculateThePageItems();
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }






        protected override void InsertItem(int index, ApporFolderBase item)
        {


            //Check if the Index is with in the current Page then add to the collection as bellow. And add to the originalCollection also
            if ((index >= startIndex) && (index < endIndex))
            {
                base.InsertItem(index - startIndex, item);

                if (Count > CountPerPage)
                    base.RemoveItem(endIndex);
            }

            if (index >= Count)
                originalCollection.Add(item);
            else
                originalCollection.Insert(index, item);
        }

        protected override void RemoveItem(int index)
        {
            int startIndex = Page * CountPerPage;
            int endIndex = startIndex + CountPerPage;
            //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
            if ((index >= startIndex) && (index < endIndex))
            {
                this.RemoveAt(index - startIndex);

                if (Count <= CountPerPage)
                    base.InsertItem(endIndex - 1, originalCollection[index + 1]);
            }

            originalCollection.RemoveAt(index);
        }
        public ObservableCollection<ApporFolderBase> GetOriginalCollection()
        {
            return originalCollection;
        }
        public void PageChanged(PageChangedEventArgs e)
        {
            Page = e.PageIndex;
            startIndex = Page * CountPerPage;
            endIndex = startIndex + CountPerPage;
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        public void SizedChanged(AppPageSizeChangedEventArgs e)
        {
            CountPerPage = e.AppPageSize;
            startIndex = Page * CountPerPage;
            endIndex = startIndex + CountPerPage;
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
    }
}
