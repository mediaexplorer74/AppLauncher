using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace appLauncher.Core.Model
{
    [XmlRoot("lists")]
    public class AppFolderList
    {

        public ObservableCollection<ApporFolderBase> AllApps = new ObservableCollection<ApporFolderBase>();
    }
}
