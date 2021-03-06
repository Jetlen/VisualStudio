﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Reflection;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.PlatformUI;

namespace GitHub.VisualStudio.Helpers
{
    public class SharedDictionaryManager : ResourceDictionary
    {
        public SharedDictionaryManager()
        {
            currentTheme = Colors.DetectTheme();
        }

#region ResourceDictionaryImplementation
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        string currentTheme;

#if !XAML_DESIGNER
        static readonly Dictionary<Uri, ResourceDictionary> resourceDicts = new Dictionary<Uri, ResourceDictionary>();
        static string baseThemeUri = "pack://application:,,,/GitHub.VisualStudio;component/Styles/";

        Uri sourceUri;
        bool themed = false;
        public new Uri Source
        {
            get { return sourceUri; }
            set
            {
                if (value.ToString() == "pack://application:,,,/GitHub.VisualStudio;component/Styles/ThemeDesignTime.xaml")
                {
                    if (!themed)
                    {
                        themed = true;
                        VSColorTheme.ThemeChanged += OnThemeChange;
                    }
                    value = new Uri(baseThemeUri + "Theme" + currentTheme + ".xaml");
                }

                sourceUri = value;
                ResourceDictionary ret;
                if (resourceDicts.TryGetValue(value, out ret))
                {
                    if (ret != this)
                    {
                        MergedDictionaries.Add(ret);
                        return;
                    }
                }
                base.Source = value;
                if (ret == null)
                    resourceDicts.Add(value, this);
            }
        }

        void OnThemeChange(ThemeChangedEventArgs e)
        {
            var uri = new Uri(baseThemeUri + "Theme" + currentTheme + ".xaml");
            ResourceDictionary ret;
            if (resourceDicts.TryGetValue(uri, out ret))
                MergedDictionaries.Remove(ret);
            currentTheme = Colors.DetectTheme();
            Source = uri;
        }
#endif
#endregion
    }
}