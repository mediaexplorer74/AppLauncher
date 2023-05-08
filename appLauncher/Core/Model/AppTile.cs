using appLauncher.Core.Brushes;

using Microsoft.Toolkit.Uwp.Helpers;

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    [Serializable]
    public class AppTile : ApporFolderBase
    {
        private string _appLogo;
        private string _appFullName;
        private string _appDescription;
        private string _appDeveloper;
        private long _appInstalledDate;
        private string _appTip;
        internal object LogoColor;
        internal object BackColor;
        internal object TextColor;

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
        public AppTile() { }


        public string AppName
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
        public string AppFullName
        {
            get
            {
                return _appFullName;
            }
            set
            {
                _appFullName = value;
            }
        }
        public Color AppBackgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(base._backgroundColor))
                {
                    return "Blue".ToColor();
                }
                return base._backgroundColor.ToColor();
            }
            set
            {
                SetProperty(ref base._backgroundColor, value.ToString());
            }
        }
        public Color AppForgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(base._forgroundColor))
                {
                    return "Black".ToColor();
                }
                return base._forgroundColor.ToColor();
            }
            set
            {
                SetProperty(ref base._forgroundColor, value.ToString());
            }
        }
        public Color AppTextColor
        {
            get
            {
                if (string.IsNullOrEmpty(base._textColor))
                {
                    return "Red".ToColor();
                }
                return base._textColor.ToColor();
            }
            set
            {
                SetProperty(ref base._textColor, value.ToString());
            }
        }
        public string Serializelogo
        {
            get
            {
                return Convert.ToBase64String(AppLogo);
            }
            set
            {
                AppLogo = Convert.FromBase64String(value);
            }
        }
        [XmlIgnore]
        public byte[] AppLogo
        {
            get
            {
                return Convert.FromBase64String(_appLogo);
            }
            set
            {
                SetProperty(ref _appLogo, Convert.ToBase64String(value));
            }
        }
        public string AppDescription
        {
            get
            {
                return _appDescription;
            }
            set
            {
                _appDescription = value;
            }
        }
        public string AppDeveloper
        {
            get
            {
                return _appDeveloper;
            }
            set
            {
                _appDeveloper = value;
            }
        }
        public DateTimeOffset InstalledDate
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(_appInstalledDate);
            }
            set
            {
                _appInstalledDate = value.ToUnixTimeSeconds();
            }
        }
        public string AppTip
        {
            get
            {
                string message = (string.IsNullOrEmpty(_appTip)) ? $"Name: {AppName}{Environment.NewLine}Developer: {AppDeveloper}{Environment.NewLine}Installed: {InstalledDate}" : _appTip;
                return message;
            }
            set
            {
                _appTip = value;
            }

        }
        [XmlIgnore]
        public Brush AppBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(AppBackgroundColor);
            }
        }
        [XmlIgnore]
        public Brush AppForgroundBrush
        {
            get
            {
                return new MaskedBrush(AppLogo.AsBuffer().AsStream().AsRandomAccessStream(), AppForgroundColor);
            }
        }
        [XmlIgnore]
        public Brush TextBrush
        {
            get
            {
                return new SolidColorBrush(AppTextColor);
            }
        }

        public static implicit operator AppTile(Apps v)
        {
            throw new NotImplementedException();
        }
    }
}
