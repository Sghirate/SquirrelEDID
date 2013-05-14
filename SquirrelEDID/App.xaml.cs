using SquirrelEDID.Model;
using SquirrelEDID.Model.Win32;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using SquirrelEDID.View;
using SquirrelEDID.View.Controls;
using SquirrelEDID.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SquirrelEDID.Utilities.Extensions;
using System.Reflection;
using System.Windows.Media;
using Elysium;
using SquirrelEDID.Utilities.Converters;
using System.IO;

namespace SquirrelEDID
{
    public enum ApplicationStates
    {
        Welcome,
        About,
        EDID,
        FolderBrowser
    }

    public enum Prompts
    {
        None,
        Screen,
        Programmer,
        Library
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [Setting(Key = "Theme", DefaultValue = "Dark", Converter = typeof(StringToThemeConverter))]
        public Theme Theme { get; set; }
        [Setting(Key = "AccentBrush", DefaultValue = "#FF017BCD", Converter = typeof(StringToSolidColorBrushConverter))]
        public SolidColorBrush AccentBrush { get; set; }
        [Setting(Key = "ContrastBrush", DefaultValue = "#FFFFFFFF", Converter = typeof(StringToSolidColorBrushConverter))]
        public SolidColorBrush ContrastBrush { get; set; }

        public static ApplicationStates CurrentState { get; set; }
        public static List<Tuple<ApplicationStates, ApplicationStates, SlideDirection>> SlideDirections { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            IoC.Set<App>(this);
            Resources["app"] = this;

            CacheViews();
            InitSettings();
            BuildSlideDirections();
            IoC.Set<Programmer>(new Programmer());

            ApplyTheme();

            CurrentState = ApplicationStates.Welcome;

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            string pathStrings = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "missing_strings");
            if (File.Exists(pathStrings))
                File.Delete(pathStrings);
            var missing = I18NExtension.Missing.Select(str => String.Format("{0}\t...", str));
            File.AppendAllLines(pathStrings, missing);

            SaveSettings();
            base.OnExit(e);
        }

        private void AddSlideDirection(ApplicationStates from, ApplicationStates to, SlideDirection dir)
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

        private void BuildSlideDirections()
        {
            SlideDirections = new List<Tuple<ApplicationStates, ApplicationStates, SlideDirection>>();
            AddSlideDirection(ApplicationStates.Welcome, ApplicationStates.About, SlideDirection.Right);
            AddSlideDirection(ApplicationStates.Welcome, ApplicationStates.EDID, SlideDirection.Left);
        }

        private void InitSettings()
        {
            Settings settings = Settings.Load("settings.json");

            settings.LoadObject(this);
            settings.LoadObject(IoC.Get<MainViewModel>());
            settings.LoadObject(IoC.Get<EDIDViewModel>());
            
            IoC.Set<Settings>(settings);
        }

        private void SaveSettings()
        {
            Settings settings = IoC.Get<Settings>();

            settings.SaveObject(this);
            settings.SaveObject(IoC.Get<MainViewModel>());
            settings.SaveObject(IoC.Get<EDIDViewModel>());

            settings.Save();
        }

        private void CacheViews()
        {
            IoC.Set<WelcomeViewModel>(new WelcomeViewModel());
            IoC.Set<WelcomeView>(new WelcomeView());
            IoC.Set<AboutViewModel>(new AboutViewModel());
            IoC.Set<AboutView>(new AboutView());
            IoC.Set<FolderBrowserViewModel>(new FolderBrowserViewModel());
            IoC.Set<FolderBrowserView>(new FolderBrowserView());
            IoC.Set<EDIDViewModel>(new EDIDViewModel());
            IoC.Set<EDIDView>(new EDIDView());

            IoC.Set<PromptProgrammerView>(new PromptProgrammerView());
            IoC.Set<PromptScreenView>(new PromptScreenView());
            IoC.Set<PromptLibraryView>(new PromptLibraryView());
        }

        public void ApplyTheme()
        {
            Elysium.Manager.Apply(this, Theme, AccentBrush, ContrastBrush);
        }
    }
}
