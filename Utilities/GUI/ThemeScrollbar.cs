﻿using System;
using System.Windows;

namespace Utilities.GUI
{
    public class ThemeScrollbar
    {
        public static void AddToApplicationResources(Application application)
        {
            string MODULE_NAME = typeof(ThemeScrollbar).Assembly.GetName().Name;
            string resource_location = string.Format("/{0};component/GUI/ThemeScrollbar.xaml", MODULE_NAME);
            ResourceDictionary rd = Application.LoadComponent(new Uri(resource_location, UriKind.Relative)) as ResourceDictionary;
            application.Resources.MergedDictionaries.Add(rd);
        }
    }
}
