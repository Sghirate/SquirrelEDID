using SquirrelEDID.Model;
using SquirrelEDID.Model.Win32;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using SquirrelEDID.View;
using SquirrelEDID.View.Controls;
using SquirrelEDID.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SquirrelEDID.Utilities.Extensions;
using System.Reflection;

namespace SquirrelEDID
{
    public enum ApplicationStates
    {
        Welcome,
        About,
        Settings,
        EDID,
        Programmer,
        Writer,
        Library,
        System,
        FolderBrowser
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ApplicationStates CurrentState { get; set; }
        public static List<Tuple<ApplicationStates, ApplicationStates, SlideDirection>> SlideDirections { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            CacheViews();
            InitSettings();
            BuildSlideDirections();
            IoC.Set<Programmer>(new Programmer());

            CurrentState = ApplicationStates.Welcome;

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveSettings();
            base.OnExit(e);
        }

        private static void AddSlideDirection(ApplicationStates from, ApplicationStates to, SlideDirection dir)
        {
            // There . . . 
            SlideDirections.Add(new Tuple<ApplicationStates, ApplicationStates, SlideDirection>(from, to, dir));
            // . . . And Back Again
            SlideDirection back = dir;
            if (dir == SlideDirection.Down)
                back = SlideDirection.Up;
            else if (dir == SlideDirection.Up)
                back = SlideDirection.Down;
            else if (dir == SlideDirection.Left)
                back = SlideDirection.Right;
            else if (dir == SlideDirection.Right)
                back = SlideDirection.Left;
            SlideDirections.Add(new Tuple<ApplicationStates, ApplicationStates, SlideDirection>(to, from, back));
            // By Bilbo Baggins
        }

        private static void BuildSlideDirections()
        {
            SlideDirections = new List<Tuple<ApplicationStates, ApplicationStates, SlideDirection>>();
            AddSlideDirection(ApplicationStates.Welcome, ApplicationStates.About, SlideDirection.Down);
            AddSlideDirection(ApplicationStates.Welcome, ApplicationStates.Settings, SlideDirection.Left);
            AddSlideDirection(ApplicationStates.Settings, ApplicationStates.EDID, SlideDirection.Down);
            AddSlideDirection(ApplicationStates.Settings, ApplicationStates.Writer, SlideDirection.Left);
            AddSlideDirection(ApplicationStates.Settings, ApplicationStates.Library, SlideDirection.Up);
            AddSlideDirection(ApplicationStates.Settings, ApplicationStates.System, SlideDirection.Up);
            AddSlideDirection(ApplicationStates.Settings, ApplicationStates.FolderBrowser, SlideDirection.Up);
        }

        private static void InitSettings()
        {
            Settings settings = Settings.Load("settings.json");
            
            settings.LoadObject(IoC.Get<MainViewModel>());
            settings.LoadObject(IoC.Get<SettingsViewModel>());
            
            IoC.Set<Settings>(settings);
        }

        private static void SaveSettings()
        {
            Settings settings = IoC.Get<Settings>();

            settings.SaveObject(IoC.Get<MainViewModel>());
            settings.SaveObject(IoC.Get<SettingsViewModel>());

            settings.Save();
        }

        private static void CacheViews()
        {
            IoC.Set<WelcomeViewModel>(new WelcomeViewModel());
            IoC.Set<WelcomeView>(new WelcomeView());
            IoC.Set<AboutViewModel>(new AboutViewModel());
            IoC.Set<AboutView>(new AboutView());
            IoC.Set<SettingsViewModel>(new SettingsViewModel());
            IoC.Set<SettingsView>(new SettingsView());
            IoC.Set<FolderBrowserViewModel>(new FolderBrowserViewModel());
            IoC.Set<FolderBrowserView>(new FolderBrowserView());
            IoC.Set<SystemViewModel>(new SystemViewModel());
            IoC.Set<SystemView>(new SystemView());
            IoC.Set<EDIDViewModel>(new EDIDViewModel());
            IoC.Set<EDIDView>(new EDIDView());
            IoC.Set<ProgrammerViewModel>(new ProgrammerViewModel());
            IoC.Set<ProgrammerView>(new ProgrammerView());
            IoC.Set<WriterViewModel>(new WriterViewModel());
            IoC.Set<WriterView>(new WriterView());
        }
    }
}
