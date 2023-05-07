using appLauncher.Core.Model;

using Microsoft.Toolkit.Uwp.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using Windows.Storage;

namespace appLauncher.Core.Helpers
{
    public static class xmlApporFolderHelper
    {
        static string filename = Path.Combine(ApplicationData.Current.LocalFolder.Path, "coll.xml");
        public static void SerializeApporFolders()
        {
            XElement rootel = new XElement("AllApps");
            List<ApporFolderBase> allapps = new List<ApporFolderBase>();
            //foreach (Apps item in packageHelper.Apps)
            //{

            //    AppTile app = new AppTile()
            //    {
            //        AppName = item.Name,
            //        AppDeveloper = item.Developer,
            //        AppDescription = item.Description,
            //        AppLogo = item.Logo,
            //        AppTip = item.Tip,
            //        AppBackgroundColor = item.BackColor,
            //        AppForgroundColor = item.LogoColor,
            //        AppTextColor = item.TextColor,
            //        InstalledDate = item.InstalledDate,
            //        AppFullName = item.FullName,
            //        ListPosition = item.ListPosition
            //    };
            //    allapps.Add(app);
            //}
            allapps = packageHelper.apporFolders;
            foreach (var s in allapps)
            {
                if (s.isfolder)
                {
                    rootel.Add(GenerateFolderXel((AppFolder)s));

                }
                else
                {
                    rootel.Add(GenerateAppXel((AppTile)s));
                }
            }

            rootel.Save(filename);

        }
        public static void DeserializeApporFolders()
        {
            List<ApporFolderBase> allapps = new List<ApporFolderBase>();
            XElement doc = XElement.Load(filename);
            var ac = doc.Elements();
            foreach (var item in ac)
            {
                if ((bool)item.Element("isfolder"))
                {
                    allapps.Add(DeserializeFolder(item));
                }
                else
                {
                    allapps.Add(DeserializeApp(item));
                }
            }
            packageHelper.apporFolders = allapps;
        }
        public static XElement GenerateAppXel(AppTile app)
        {
            XElement appel = new XElement("app");
            XElement listposel = new XElement("position", app.ListPosition);
            XElement isfolder = new XElement("isfolder", app.isfolder);
            XElement nameel = new XElement("name", app.AppName);
            XElement fullnameel = new XElement("fullname", app.AppFullName);
            XElement descriptionel = new XElement("description", app.AppDescription);
            XElement developerel = new XElement("developer", app.AppDeveloper);
            XElement installedel = new XElement("installed", app.InstalledDate);
            XElement tipel = new XElement("tip", app.AppTip);
            XElement logoel = new XElement("logo", Convert.ToBase64String(app.AppLogo));
            XElement logocolorel = new XElement("logocolor", app.AppForgroundColor.ToString());
            XElement backcolorel = new XElement("backcolor", app.AppBackgroundColor.ToString());
            XElement textcolorel = new XElement("textcolor", app.AppTextColor.ToString());
            appel.Add(listposel);
            appel.Add(isfolder);
            appel.Add(nameel);
            appel.Add(fullnameel);
            appel.Add(descriptionel);
            appel.Add(developerel);
            appel.Add(installedel);
            appel.Add(tipel);
            appel.Add(logoel);
            appel.Add(logocolorel);
            appel.Add(backcolorel);
            appel.Add(textcolorel);
            return appel;

        }
        public static XElement GenerateFolderXel(AppFolder folder)
        {
            XElement appFolder = new XElement("folder");
            XElement isfolder = new XElement("isfolder", folder.isfolder);
            XElement name = new XElement("name", folder.FolderName);
            XElement pos = new XElement("position", folder.ListPosition);
            XElement backcolor = new XElement("backcolor", folder.FolderBackgroundColor.ToString());
            XElement frontcolor = new XElement("frontcolor", folder.FolderForgroundColor.ToString());
            XElement textcolor = new XElement("textcolor", folder.FolderTextColor.ToString());
            XElement folderapps = new XElement("folderapps");
            foreach (AppTile item in folder.Tiles)
            {
                folderapps.Add(GenerateAppXel(item));
            }
            appFolder.Add(name);
            appFolder.Add(isfolder);
            appFolder.Add(pos);
            appFolder.Add(backcolor);
            appFolder.Add(frontcolor);
            appFolder.Add(textcolor);
            appFolder.Add(folderapps);


            return appFolder;
        }
        public static AppTile DeserializeApp(XElement serapp)
        {
            AppTile app = new AppTile();
            app.ListPosition = (int)serapp.Element("position");
            app.AppName = (string)serapp.Element("name");
            app.AppFullName = (string)serapp.Element("fullname");
            app.AppDescription = (string)serapp.Element("description");
            app.AppDeveloper = (string)serapp.Element("developer");
            app.InstalledDate = (DateTimeOffset)serapp.Element("installed");
            app.AppTip = (string)serapp.Element("tip");
            app.AppLogo = Convert.FromBase64String((string)serapp.Element("logo"));
            app.AppForgroundColor = ((string)serapp.Element("logocolor")).ToColor();
            app.AppBackgroundColor = ((string)serapp.Element("backcolor")).ToColor();
            app.AppTextColor = ((string)serapp.Element("textcolor")).ToColor();
            return app;


        }
        public static AppFolder DeserializeFolder(XElement serfolder)
        {
            AppFolder appFolder = new AppFolder();
            appFolder.FolderName = (string)serfolder.Element("name");
            appFolder.isfolder = (bool)serfolder.Element("isfolder");
            appFolder.ListPosition = (int)serfolder.Element("position");
            appFolder.FolderBackgroundColor = ((string)serfolder.Element("frontcolor")).ToColor();
            appFolder.FolderBackgroundColor = ((string)serfolder.Element("backcolor")).ToColor();
            appFolder.FolderTextColor = ((string)serfolder.Element("textcolor")).ToColor();
            XElement folderapps = serfolder.Element("folderapps");
            foreach (XElement item in folderapps.Descendants("app"))
            {
                appFolder.Tiles.Add(DeserializeApp(item));
            }
            return appFolder;
        }

    }
}
