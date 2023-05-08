using Microsoft.Toolkit.Uwp.Helpers;

using System.Collections.Generic;
using System.Xml.Serialization;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{

    public class AppFolder : ApporFolderBase
    {
        public AppFolder() { }


        private List<AppTile> _tiles = new List<AppTile>();
        public int ListPosition
        {
            get
            {
                return _listposition;
            }
            set
            {
                _listposition = value;
            }
        }
        public string FolderName
        {
            get
            {
                return base._name;
            }
            set
            {
                base._name = value;
            }
        }

        public List<AppTile> Tiles
        {
            get
            {
                return _tiles;
            }
            set
            {
                _tiles = value;
            }
        }
        public Color FolderBackgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(base._backgroundColor))
                {
                    return Colors.Violet;
                }
                return base._backgroundColor.ToColor();
            }
            set
            {
                base._backgroundColor = value.ToString();
            }
        }
        public Color FolderForgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(base._forgroundColor))
                {
                    return Colors.Transparent;
                }
                return base._forgroundColor.ToColor();
            }
            set
            {
                base._forgroundColor = value.ToString();
            }
        }
        public Color FolderTextColor
        {
            get
            {
                if (string.IsNullOrEmpty(base._textColor))
                {
                    return Colors.Red;
                }
                return base._textColor.ToColor();
            }
            set
            {
                base._textColor = value.ToString();
            }
        }

        [XmlIgnore]
        public Brush FolderBackgroundBrush
        {
            get
            {
                return null;
            }
        }
        [XmlIgnore]
        public Brush FolderForgroundBrush
        {
            get
            {
                return null;
            }
        }
        [XmlIgnore]
        public Brush FolderTextBrush
        {
            get
            {
                return null;
            }
        }
    }
}
