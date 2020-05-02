using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using Application = System.Windows.Application;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Windows.Threading;

namespace BatchWriter
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Commands

        // general
        public ICommand Command_OpenRoot { get; set; }
        public ICommand Command_OpenAddonFolder { get; set; }
        public ICommand Command_OpenServerAddonsFolder { get; set; }
        public ICommand Command_ShowFolder { get; set; }
        
        // arma 3 server update
        public ICommand Command_UpdateArma3Server { get; set; }

        // addons update
        public ICommand Command_OpenSteamCMDFolder { get; set; }
        public ICommand Command_OpenSteamRootFolder { get; set; }
        public ICommand Command_AddSteamWorkshopID { get; set; }
        public ICommand Command_RemoveSteamWorkshopID { get; set; }
        public ICommand Command_FetchAddonList { get; set; }
        public ICommand Command_UpdateAddons { get; set; }
        public ICommand Command_AbortUpdate { get; set; }
        
        // addons purge        
        public ICommand Command_ScanLocalAddons { get; set; }
        public ICommand Command_AddPreservedAddon { get; set; }
        public ICommand Command_RemovePreservedAddon { get; set; }
        public ICommand Command_RemoveSelectedAddons { get; set; }

        // batch file creation
        public ICommand Command_OpenConfig { get; set; }
        public ICommand Command_OpenProfiles { get; set; }
        public ICommand Command_WriteSyntax { get; set; }
        public ICommand Command_CreateBat { get; set; }
        public ICommand Command_CreateHCBat { get; set; }

        #endregion

        #region Properties general

        private string _addonFolder;
        public string AddonFolder
        {
            get { return _addonFolder; }
            set
            {
                if (value != _addonFolder)
                {
                    _addonFolder = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private string _serverAddonsFolder;
        public string ServerAddonsFolder
        {
            get { return _serverAddonsFolder; }
            set
            {
                if (value != _serverAddonsFolder)
                {
                    _serverAddonsFolder = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private string _rootFolder;
        public string RootFolder
        {
            get { return _rootFolder; }
            set
            {
                if (value != _rootFolder)
                {
                    _rootFolder = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }
        
        private bool _autoGenerate = false;
        private bool _debug = false;
        private string _reportFile = "updateReport.txt";

        private bool _batchonly = false;

        #endregion

        #region Properties autogeneration
        
        string _reportPath = "";
        bool _reportPlz = false;

        private int _serverUpdateStatus;
        /// <summary>
        /// 0: invalid, 1: success, 2: failed
        /// </summary>
        public int ServerUpdateStatus
        {
            get { return _serverUpdateStatus; }
            set
            {
                if (value != _serverUpdateStatus)
                {
                    _serverUpdateStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _fetchAddonsStatus;
        /// <summary>
        /// 0: invalid, 1: success, 2: failed
        /// </summary>
        public int FetchAddonsStatus
        {
            get { return _fetchAddonsStatus; }
            set
            {
                if (value != _fetchAddonsStatus)
                {
                    _fetchAddonsStatus = value;
                    OnPropertyChanged();                    
                }
            }
        }

        private List<string> _report = new List<string>();

        #endregion

        #region Properties Addons update

        public ObservableCollection<string> SteamWorkshopIDs { get; set; }
        public ObservableCollection<AddonItem> Addons { get; set; }

        private string _newWorkshopID;
        public string NewWorkshopID
        {
            get { return _newWorkshopID; }
            set
            {
                if (value != _newWorkshopID)
                {
                    _newWorkshopID = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _steamCMDFolder;
        public string SteamCMDFolder
        {
            get { return _steamCMDFolder; }
            set
            {
                if (value != _steamCMDFolder)
                {
                    _steamCMDFolder = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private string _steamRootFolder;
        public string SteamRootFolder
        {
            get { return _steamRootFolder; }
            set
            {
                if (value != _steamRootFolder)
                {
                    _steamRootFolder = value;
                    OnPropertyChanged();
                    WriteSettings();
                    OnSteamRootfolderChanged();
                }
            }
        }


        private string _steamAccount;
        public string SteamAccount
        {
            get { return _steamAccount; }
            set
            {
                if (value != _steamAccount)
                {
                    _steamAccount = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private string _steamCMDOutput;
        public string SteamCMDOutput
        {
            get { return _steamCMDOutput; }
            set
            {
                if (value != _steamCMDOutput)
                {
                    _steamCMDOutput = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _anonymousSteamLogon;
        public bool AnonymousSteamLogon
        {
            get { return _anonymousSteamLogon; }
            set
            {
                if (value != _anonymousSteamLogon)
                {
                    _anonymousSteamLogon = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private int _maxRetryCount;
        public int MaxRetryCount
        {
            get { return _maxRetryCount; }
            set
            {
                if (value != _maxRetryCount)
                {
                    _maxRetryCount = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private bool _limitBandwidth;
        public bool LimitBandwidth
        {
            get { return _limitBandwidth; }
            set
            {
                if (value != _limitBandwidth)
                {
                    _limitBandwidth = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private int _maxBandwidth;
        public int MaxBandwidth
        {
            get { return _maxBandwidth; }
            set
            {
                if (value != _maxBandwidth)
                {
                    _maxBandwidth = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private bool _updateActive;
        public bool UpdateActive
        {
            get { return _updateActive; }
            set
            {
                if (value != _updateActive)
                {
                    _updateActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _abortUpdate;
        public bool AbortUpdate
        {
            get { return _abortUpdate; }
            set
            {
                if (value != _abortUpdate)
                {
                    _abortUpdate = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _selectedNumber;
        public int SelectedNumber
        {
            get { return _selectedNumber; }
            set
            {
                if (value != _selectedNumber)
                {
                    _selectedNumber = value;
                    OnPropertyChanged();
                    if (_reportPlz) UpdateReportStatus();
                }
            }
        }

        private int _succeededNumber;
        public int SucceededNumber
        {
            get { return _succeededNumber; }
            set
            {
                if (value != _succeededNumber)
                {
                    _succeededNumber = value;
                    OnPropertyChanged();
                    if (_reportPlz) UpdateReportStatus();
                }
            }
        }

        #endregion

        #region Properties Addon purge
        
        public ObservableCollection<string> PreservedAddons { get; set; }

        public ObservableCollection<string> UnknownAddons { get; set; }

        private int _deletedAddons;

        #endregion

        #region Properties batch file creation

        private string _profilesFolder;

        public string ProfilesFolder
        {
            get { return _profilesFolder; }
            set
            {
                if (value != _profilesFolder)
                {
                    _profilesFolder = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private string _config;

        public string Config
        {
            get { return _config; }
            set
            {
                if (value != _config)
                {
                    _config = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private bool _writePort;

        public bool WritePort
        {
            get { return _writePort; }
            set
            {
                if (value != _writePort)
                {
                    _writePort = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private bool _writeFilePatching;

        public bool WriteFilePatching
        {
            get { return _writeFilePatching; }
            set
            {
                if (value != _writeFilePatching)
                {
                    _writeFilePatching = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private bool _writeAutoInit;

        public bool WriteAutoInit
        {
            get { return _writeAutoInit; }
            set
            {
                if (value != _writeAutoInit)
                {
                    _writeAutoInit = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private bool _writeHyperThreading;

        public bool WriteHyperThreading
        {
            get { return _writeHyperThreading; }
            set
            {
                if (value != _writeHyperThreading)
                {
                    _writeHyperThreading = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private bool _is64bit;

        public bool Is64bit
        {
            get { return _is64bit; }
            set
            {
                if (value != _is64bit)
                {
                    _is64bit = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }
        
        private string _port = "2302";

        public string Port
        {
            get { return _port; }
            set
            {
                if (value != _port)
                {
                    _port = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private string _folderContent;

        public string FolderContent
        {
            get { return _folderContent; }
            set
            {
                if (value != _folderContent)
                {
                    _folderContent = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _resultContent;

        public string ResultContent
        {
            get { return _resultContent; }
            set
            {
                if (value != _resultContent)
                {
                    _resultContent = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (value != _fileName)
                {
                    _fileName = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private string _headlessClientFileName;

        public string HeadlessClientFileName
        {
            get { return _headlessClientFileName; }
            set
            {
                if (value != _headlessClientFileName)
                {
                    _headlessClientFileName = value;
                    OnPropertyChanged();
                    WriteSettings();
                }
            }
        }

        private string _settingsFilePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.txt"); }
        }
        
        private bool _initializing;

        #endregion

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        #region Constructors

        public ViewModel()
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Contains("-nowindow") ||
                args.Contains("-autogenerate")) _autoGenerate = true;

            if (args.Contains("-debug")) _debug = true;

            if (args.Contains("-batchonly")) _batchonly = true;

            _initializing = true;

            Command_OpenRoot = new DelegateCommand(OpenRoot);
            Command_OpenAddonFolder = new DelegateCommand(OpenAddonFolder);
            Command_OpenServerAddonsFolder = new DelegateCommand(OpenServerAddonsFolder);
            Command_ShowFolder = new DelegateCommand(ShowFolder);
            
            Command_UpdateArma3Server = new DelegateCommand(UpdateArma3Server);
            
            Command_OpenSteamCMDFolder = new DelegateCommand(OpenSteamCMDFolder);
            Command_OpenSteamRootFolder = new DelegateCommand(OpenSteamRootFolder);
            Command_AddSteamWorkshopID = new DelegateCommand(AddSteamWorkshopID);
            Command_RemoveSteamWorkshopID = new DelegateCommand(RemoveSteamWorkshopID);
            Command_FetchAddonList = new DelegateCommand(FetchAddonList);
            Command_UpdateAddons = new DelegateCommand(UpdateAddons);
            Command_AbortUpdate = new DelegateCommand(OnAbortUpdate);

            Command_ScanLocalAddons = new DelegateCommand(ScanLocalAddons);
            Command_AddPreservedAddon = new DelegateCommand(OnAddPreservedAddon);
            Command_RemovePreservedAddon = new DelegateCommand(OnRemovePreservedAddon);
            Command_RemoveSelectedAddons = new DelegateCommand(OnRemoveSelectedAddons);

            Command_OpenConfig = new DelegateCommand(OpenConfig);
            Command_OpenProfiles = new DelegateCommand(OpenProfiles);
            Command_WriteSyntax = new DelegateCommand(WriteSyntax);
            Command_CreateBat = new DelegateCommand(CreateBat);
            Command_CreateHCBat = new DelegateCommand(CreateHCBat);

            SteamWorkshopIDs = new ObservableCollection<string>();
            Addons = new ObservableCollection<AddonItem>();

            PreservedAddons = new ObservableCollection<string>();
            UnknownAddons = new ObservableCollection<string>();

            SteamWorkshopIDs.CollectionChanged += OnSteamWorkshopIDsChanged;
            PreservedAddons.CollectionChanged += OnPreservedAddonsChanged;

            Initialize();

            _initializing = false;

            if (_autoGenerate)
            {
                Application.Current.Dispatcher.Invoke(() => AutoGenerate());
            }
            else if (_batchonly)
            {
                Application.Current.Dispatcher.Invoke(() => AutoGenerateBatchOnly());
            }
        }
                
        #endregion

        #region Methods general

        public void Kill()
        {
            foreach (Process p in Process.GetProcessesByName("SteamCMD.exe")) p.Kill();
            Environment.Exit(0);
        }

        private void Initialize()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    _initializing = false;
                    return;
                }

                List<string> settings = new List<string>();

                using (StreamReader file = new StreamReader(_settingsFilePath))
                {
                    while (!file.EndOfStream)
                    {
                        settings.Add(file.ReadLine());
                    }
                }

                foreach (string setting in settings)
                {
                    if (setting.StartsWith("SteamCMDFolder=")) SteamCMDFolder = setting.Remove(0, "SteamCMDFolder=".Length);
                    if (setting.StartsWith("SteamRootFolder=")) SteamRootFolder = setting.Remove(0, "SteamRootFolder=".Length);
                    if (setting.StartsWith("SteamAccount=")) SteamAccount = setting.Remove(0, "SteamAccount=".Length);
                    if (setting.StartsWith("AnonymousSteamLogon=")) AnonymousSteamLogon = int.Parse(setting.Remove(0, "AnonymousSteamLogon=".Length)) == 1;
                    if (setting.StartsWith("MaxRetryCount="))
                    {
                        int count;
                        int.TryParse(setting.Remove(0, "MaxRetryCount=".Length), out count);
                        MaxRetryCount = count > 0 ? count : 3;
                    }
                    if (setting.StartsWith("LimitBandwidth=")) LimitBandwidth = int.Parse(setting.Remove(0, "LimitBandwidth=".Length)) == 1;
                    if (setting.StartsWith("MaxBandwidth="))
                    {
                        int count;
                        int.TryParse(setting.Remove(0, "MaxBandwidth=".Length), out count);
                        MaxBandwidth = count > 0 ? count : 50000;
                    }
                    if (setting.StartsWith("FileName=")) FileName = setting.Remove(0, "FileName=".Length);
                    if (setting.StartsWith("RootFolder=")) RootFolder = setting.Remove(0, "RootFolder=".Length);
                    if (setting.StartsWith("AddonFolder=")) AddonFolder = setting.Remove(0, "AddonFolder=".Length);
                    if (setting.StartsWith("ServerAddonsFolder=")) ServerAddonsFolder = setting.Remove(0, "ServerAddonsFolder=".Length);
                    if (setting.StartsWith("ProfilesFolder=")) ProfilesFolder = setting.Remove(0, "ProfilesFolder=".Length);
                    if (setting.StartsWith("Config=")) Config = setting.Remove(0, "Config=".Length);
                    if (setting.StartsWith("WritePort=")) WritePort = int.Parse(setting.Remove(0, "WritePort=".Length)) == 1;
                    if (setting.StartsWith("Port=")) Port = setting.Remove(0, "Port=".Length);
                    if (setting.StartsWith("WriteFilePatching=")) WriteFilePatching = int.Parse(setting.Remove(0, "WriteFilePatching=".Length)) == 1;
                    if (setting.StartsWith("WriteAutoInit=")) WriteAutoInit = int.Parse(setting.Remove(0, "WriteAutoInit=".Length)) == 1;
                    if (setting.StartsWith("WriteHyperThreading=")) WriteHyperThreading = int.Parse(setting.Remove(0, "WriteHyperThreading=".Length)) == 1;
                    if (setting.StartsWith("Is64bit=")) Is64bit = int.Parse(setting.Remove(0, "Is64bit=".Length)) == 1;
                    if (setting.StartsWith("HeadlessClientFileName=")) HeadlessClientFileName = setting.Remove(0, "HeadlessClientFileName=".Length);
                    //if (setting.StartsWith("AutoGenerate=")) autoGenerate = int.Parse(setting.Remove(0, "AutoGenerate=".Length)) == 1;

                    if (setting.StartsWith("SteamWorkshopID=")) SteamWorkshopIDs.Add(setting.Remove(0, "SteamWorkshopID=".Length));

                    if (setting.StartsWith("PreservedAddon=")) PreservedAddons.Add(setting.Remove(0, "PreservedAddon=".Length));
                }

                if (String.IsNullOrEmpty(SteamAccount)) SteamAccount = "anonymous";

                if (String.IsNullOrEmpty(Port)) Port = "2302";
                if (String.IsNullOrEmpty(FileName)) FileName = "Arma3Server_Starter";
                if (String.IsNullOrEmpty(HeadlessClientFileName)) HeadlessClientFileName = "Arma3HeadlessClient_Starter";

                if (!String.IsNullOrEmpty(AddonFolder)) ScanAddons();
            }
            catch
            {
                System.Windows.MessageBox.Show("Could not reload settings. Sorry.");
            }
        }
        
        private void OpenRoot(object obj)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

            fbd.Description = "Select root directory";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                RootFolder = fbd.SelectedPath;
            }
        }

        private void OpenAddonFolder(object obj)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

            fbd.Description = "Select addon directory";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                AddonFolder = fbd.SelectedPath;

                ScanAddons();
            }
        }

        private void OpenServerAddonsFolder(object obj)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

            fbd.Description = "Select server addons directory";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                ServerAddonsFolder = fbd.SelectedPath;
            }
        }
        
        private void WriteSettings()
        {
            if (_initializing) return;

            List<string> settings = new List<string>();

            //settings.Add("AutoGenerate=0");
            settings.Add("SteamCMDFolder=" + SteamCMDFolder);
            settings.Add("SteamRootFolder=" + SteamRootFolder);
            settings.Add("SteamAccount=" + SteamAccount);
            settings.Add("AnonymousSteamLogon=" + (AnonymousSteamLogon ? "1" : "0"));
            settings.Add("MaxRetryCount=" + MaxRetryCount);
            settings.Add("LimitBandwidth=" + (LimitBandwidth ? "1" : "0"));
            settings.Add("MaxBandwidth=" + MaxBandwidth);
            settings.Add("FileName=" + FileName);            
            settings.Add("RootFolder=" + RootFolder);
            settings.Add("AddonFolder=" + AddonFolder);
            settings.Add("ServerAddonsFolder=" + ServerAddonsFolder);
            settings.Add("ProfilesFolder=" + ProfilesFolder);
            settings.Add("Config=" + Config);
            settings.Add("WritePort=" + (WritePort ? "1" : "0"));
            settings.Add("Port=" + Port);
            settings.Add("WriteFilePatching=" + (WriteFilePatching ? "1" : "0"));
            settings.Add("WriteAutoInit=" + (WriteAutoInit ? "1" : "0"));
            settings.Add("WriteHyperThreading=" + (WriteHyperThreading ? "1" : "0"));
            settings.Add("Is64bit=" + (Is64bit ? "1" : "0"));
            settings.Add("HeadlessClientFileName=" + HeadlessClientFileName);
            settings.Add("");

            foreach (string id in SteamWorkshopIDs)
            {
                settings.Add("SteamWorkshopID=" + id);
            }

            settings.Add("");

            foreach (string dir in PreservedAddons)
            {
                settings.Add("PreservedAddon=" + dir);
            }

            using (StreamWriter file = new StreamWriter(_settingsFilePath, false))
            {
                file.Write(String.Join("\n", settings.ToArray()));
            }
        }
        
        private void ShowFolder(object obj)
        {
            if (Directory.Exists(obj as string)) Process.Start(obj as string);
        }

        #endregion

        #region Autogeneration

        private void AutoGenerateBatchOnly()
        {
            if (_debug) AllocConsole();

            if (!String.IsNullOrEmpty(RootFolder))
            {
                _reportPath = Path.Combine(RootFolder, _reportFile);
                if (File.Exists(_reportPath)) File.Delete(_reportPath);

                var file = File.CreateText(_reportPath);
                file.Close();

                AddReportLine(_reportPath, DateTime.Now.ToString("F"));
            }
            else Application.Current.Shutdown(1);

            AddReportLine(_reportPath, "Fetching addons from Steam Workshop...");
            FetchAddonList(null);
            while (FetchAddonsStatus == 0) Thread.Sleep(200);
            if (FetchAddonsStatus == 1)
            {
                AddReportLineAppend(_reportPath, "Success!");                
            }
            else
            {
                AddReportLineAppend(_reportPath, "Failed!");
                AddReportLineAppend(_reportPath, "Shutting down - " + DateTime.Now.ToString("T"));
                Application.Current.Shutdown(1);
            }

            AddReportLine(_reportPath, "Creating batch files...");
            try
            {
                WriteSyntax(null);
                CreateBat(null);
                CreateHCBat(null);
                AddReportLineAppend(_reportPath, "Success!");
            }
            catch
            {
                AddReportLineAppend(_reportPath, "Failed! :(");
            }

            AddReportLineAppend(_reportPath, "Finished - " + DateTime.Now.ToString("T"));

            Application.Current.Shutdown(1);
        }

        private void AutoGenerate()
        {
            if (_debug) AllocConsole();

            if (!String.IsNullOrEmpty(RootFolder))
            {
                _reportPath = Path.Combine(RootFolder, _reportFile);
                if (File.Exists(_reportPath)) File.Delete(_reportPath);

                var file = File.CreateText(_reportPath);
                file.Close();
                                
                AddReportLine(_reportPath, DateTime.Now.ToString("F"));
            }
            else Application.Current.Shutdown(1);

            AddReportLine(_reportPath, "Updating Arma 3 server...");
            UpdateArma3Server(null);
            while (UpdateActive)
            {
                Thread.Sleep(500);
            }
            if (ServerUpdateStatus != 1)
            {
                AddReportLineAppend(_reportPath, "Failed!");
                AddReportLineAppend(_reportPath, "Shutting down - " + DateTime.Now.ToString("T"));
                Application.Current.Shutdown(1);
            }
            else
            {
                AddReportLineAppend(_reportPath, "Success!");
            }

            AddReportLine(_reportPath, "Fetching addons from Steam Workshop...");
            FetchAddonList(null);
            while (FetchAddonsStatus == 0) Thread.Sleep(200);
            if (FetchAddonsStatus == 1)
            {
                AddReportLineAppend(_reportPath, "Success!");

                AddReportLine(_reportPath, "Updating addons...");
                _reportPlz = true;
                UpdateAddons(null);
                while (UpdateActive)
                {
                    Thread.Sleep(200);
                }
                _reportPlz = false;
                if (SucceededNumber == SelectedNumber)
                {
                    ReplaceLastReportLine(_reportPath, "Updating addons...Success!");
                }
                else
                {
                    ReplaceLastReportLine(_reportPath, "Updating addons...Failed for " + (SelectedNumber - SucceededNumber) + " addons.");
                }

                AddReportLine(_reportPath, "Removing obsolete addons...");
                ScanLocalAddons(null);
                OnRemoveSelectedAddons(null);
                if (UnknownAddons.Count == 0)
                {
                    if (_deletedAddons > 0)
                    {
                        AddReportLineAppend(_reportPath, "Success: " + _deletedAddons + " Addon(s) deleted.");
                    }
                    else
                    {
                        AddReportLineAppend(_reportPath, "None found!");
                    }                   
                }
            }
            else
            {
                AddReportLineAppend(_reportPath, "Failed!");
                AddReportLineAppend(_reportPath, "Shutting down - " + DateTime.Now.ToString("T"));
                Application.Current.Shutdown(1);
            }

            AddReportLine(_reportPath, "Creating batch files...");
            try
            {
                WriteSyntax(null);
                CreateBat(null);                
                CreateHCBat(null);
                AddReportLineAppend(_reportPath, "Success!");
            }
            catch
            {
                AddReportLineAppend(_reportPath, "Failed! :(");
            }

            AddReportLineAppend(_reportPath, "Finished - " + DateTime.Now.ToString("T"));

            Application.Current.Shutdown(1);
        }

        private void AddReportLine(string path, string line)
        {
            _report.Add(line);

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    if (_debug) Console.WriteLine("Debug -> " + line + "(" + i + ")");

                    if (File.Exists(path)) File.Delete(path);

                    File.WriteAllLines(path, _report);

                    //using (StreamWriter file = File.AppendText(path))
                    //{
                    //    for (int j = 0; j < _report.Count - 1; j++)
                    //    {
                    //        file.WriteLine(_report[j]);
                    //    }
                    //}

                    //if (_debug) Console.WriteLine(line);
                    //using (StreamWriter file = File.AppendText(path))
                    //{
                    //    file.WriteLine(line);
                    //}
                    break;
                }
                catch (Exception e)
                {
                    if (_debug) Console.WriteLine(e.Message);
                    Thread.Sleep(100);
                }
            }            
        }

        private void AddReportLineAppend(string path, string append)
        {
            _report[_report.Count - 1] += append;

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    if (_debug) Console.WriteLine("Debug -> " + append + "(" + i + ")");

                    if (File.Exists(path)) File.Delete(path);

                    File.WriteAllLines(path, _report);

                    //using (StreamWriter file = File.AppendText(path))
                    //{
                    //    for (int j = 0; j < _report.Count - 1; j++)
                    //    {
                    //        file.WriteLine(_report[j]);
                    //    }
                    //}

                    //if (_debug) Console.Write(append);
                    //using (StreamWriter file = File.AppendText(path))
                    //{
                    //    file.Write(append);
                    //}
                    break;
                }
                catch (Exception e)
                {
                    if (_debug) Console.WriteLine(e.Message);
                    Thread.Sleep(1000);
                }
            }            
        }

        private void ReplaceLastReportLine(string path, string replacement)
        {
            if (_report[_report.Count - 1].StartsWith(replacement.Substring(0, 4))) _report[_report.Count - 1] = replacement;
            else _report.Add(replacement);

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    if (_debug) Console.WriteLine("Debug -> " + replacement + "(" + i + ")");

                    if (File.Exists(path)) File.Delete(path);

                    File.WriteAllLines(path, _report);

                    //using (StreamWriter file = File.AppendText(path))
                    //{
                    //    for (int j = 0; j < _report.Count - 1; j++)
                    //    {
                    //        file.WriteLine(_report[j]);
                    //        if (_debug) Console.WriteLine("Debug -> Writing line " + _report[j]);
                    //    }
                    //}
                    break;
                }
                catch (Exception e)
                {
                    if (_debug) Console.WriteLine(e.Message);
                    Thread.Sleep(1000);
                }
            }            
        }

        private void UpdateReportStatus()
        {
            ReplaceLastReportLine(_reportPath, "Updating addons..." + SucceededNumber + "/" + SelectedNumber);
        }

        #endregion

        #region Methods Arma 3 server update

        private void UpdateArma3Server(object obj)
        {
            ServerUpdateStatus = 0;

            UpdateActive = true;

            if (_debug) Console.WriteLine("Debug -> UpdateArma3Server");

            string pw = "";
            string gc = "";

            if (AnonymousSteamLogon)
            {
                SteamAccount = "anonymous";
            }
            else
            {
                SteamCredentialsDialog scd = new SteamCredentialsDialog();

                scd.SteamAccount = SteamAccount;

                if (scd.ShowDialog() == true)
                {
                    SteamAccount = scd.SteamAccount;
                    pw = scd.SteamPassword;
                    gc = scd.SteamGuardCode;
                }
                else return;
            }

            List<string> l = new List<string>();
            l.Add("0");

            Thread thread = new Thread(() => ExecuteSteamCMD(l, pw, gc));

            thread.Start();

            if (_debug) Console.WriteLine("Debug -> UpdateArma3Server: Thread started");
        }


        #endregion

        #region Methods addons update

        private void OpenSteamCMDFolder(object obj)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

            fbd.Description = "Select steamcmd.exe directory";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                SteamCMDFolder = fbd.SelectedPath;
            }
        }

        private void OpenSteamRootFolder(object obj)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

            fbd.Description = "Select Steam (library) root directory";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                SteamRootFolder = fbd.SelectedPath;
            }
        }
        
        private void AddSteamWorkshopID(object obj)
        {
            int parsedResult = 0;
            int.TryParse(NewWorkshopID, out parsedResult); // make sure ID is integer

            if (!String.IsNullOrEmpty(NewWorkshopID) &&
                parsedResult != 0)
            {
                if (!SteamWorkshopIDs.Contains(NewWorkshopID)) SteamWorkshopIDs.Add(NewWorkshopID);
                NewWorkshopID = "";
            }
        }

        private void RemoveSteamWorkshopID(object obj)
        {
            if (obj is string)
            {
                SteamWorkshopIDs.Remove((string)obj);
            }
        }

        private void CheckSelections()
        {
            SelectedNumber = Addons.Where(x => x.IsSelected).Count();
        }

        private void FetchAddonList(object obj)
        {
            FetchAddonsStatus = 0;

            string urlRoot = "https://steamcommunity.com/sharedfiles/filedetails/?id=";

            foreach (string id in SteamWorkshopIDs)
            {
                string sourceString = new System.Net.WebClient().DownloadString(urlRoot + id);

                string regexaddons = ".*?<a href=\"https:\\/\\/steamcommunity\\.com\\/sharedfiles\\/filedetails\\/\\?id=(.*?)\"><div class=\"workshopItemPreviewHolder.*?\\n.*?\\n.*?\\n.*?workshopItemTitle\">(.*?)<\\/div>";

                MatchCollection itemreg = Regex.Matches(sourceString, regexaddons, RegexOptions.IgnoreCase);

                if (itemreg.Count > 0)
                {
                    foreach (Match match in itemreg)
                    {
                        AddonItem item = new AddonItem(CheckSelections);

                        item.Name = match.Groups[2].Value as string;
                        item.ID = match.Groups[1].Value as string;
                        item.IsSelected = true;

                        if (Addons.Where(x => x.ID == item.ID).Count() == 0) Addons.Add(item);
                    }
                }
                else
                {
                    string regextitle = ".*?<title>Steam Workshop :: (.*?)</title>.*?";
                    itemreg = Regex.Matches(sourceString, regextitle, RegexOptions.IgnoreCase);

                    if (itemreg.Count > 0)
                    {
                        AddonItem item = new AddonItem(null);

                        item.Name = itemreg[0].Groups[1].Value as string;
                        item.ID = id;
                        item.IsSelected = true;

                        if (Addons.Where(x => x.ID == item.ID).Count() == 0) Addons.Add(item);
                    }
                }

                
            }

            Console.Out.WriteLine("Fetched " + Addons.Count + " addons from workshop.");

            if (Addons.Count > 0) FetchAddonsStatus = 1;
            else FetchAddonsStatus = 2;
        }

        private void CheckUpdateStatus()
        {
            SucceededNumber = Addons.Where(x => x.UpdateStatus == 1).Count();

            Console.Out.WriteLine("Completed " + SucceededNumber + "/" + SelectedNumber + " checks.");
        }

        private void SteamCMDOutputAddLine(string line, bool extraLine = false)
        {
            if (extraLine)
            {
                SteamCMDOutput += "\n";
            }
            string l = DateTime.Now.ToLocalTime().ToString("HH:mm:ss") + ": " + line;
            SteamCMDOutput += "\n" + l;

        }

        private void SteamCMDOutputAppend(string line)
        {
            SteamCMDOutput += line;
        }

        private void UpdateAddons(object obj)
        {
            UpdateActive = true;

            List<string>  iDs = new List<string>();

            foreach (AddonItem item in Addons)
            {
                if (item.IsSelected && item.UpdateStatus != 1)
                {
                    iDs.Add(item.ID);                    
                }
            }

            string pw = "";
            string gc = "";

            if (AnonymousSteamLogon)
            {
                SteamAccount = "anonymous";
            }
            else
            {
                SteamCredentialsDialog scd = new SteamCredentialsDialog();

                scd.SteamAccount = SteamAccount;

                if (scd.ShowDialog() == true)
                {
                    SteamAccount = scd.SteamAccount;
                    pw = scd.SteamPassword;
                    gc = scd.SteamGuardCode;
                }
                else return;
            }

            if (iDs.Count > 0)
            {
                SelectedNumber = iDs.Count;
                SucceededNumber = 0;

                Thread thread = new Thread(() => ExecuteSteamCMD(iDs, pw, gc));

                thread.Start();
            }
        }

        //private Task ReadoutTimer(Process process, string commandString)
        //{
        //    while (!process.HasExited)
        //    {
        //        Thread.Sleep(5000);

        //        if (_readoutToken)
        //        {
        //            process.StandardInput.WriteLine(commandString);
        //            _readoutToken = false;
        //        }
        //    }

        //    return null;
        //}
        //private volatile bool _readoutToken;

        private Task ExecuteSteamCMD(List<string> iDs, string pw, string gc)
        {
            for (int i = 0; i < MaxRetryCount + 1; i++)
            {
                bool finished = true;

                if (!iDs.Contains("0"))
                {
                    foreach (AddonItem item in Addons)
                    {
                        if (item.IsSelected && item.UpdateStatus != 1)
                        {
                            finished = false;
                            break;
                        }
                    }

                    if (finished) break;
                }
                else if (ServerUpdateStatus > 0)
                {
                    break;
                }

                if (_debug) Console.WriteLine("Debug -> ExecuteSteamCMD: retry " + i);

                if (i > 0 && !_autoGenerate)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SteamCMDOutputAddLine("################", true);
                        SteamCMDOutputAddLine("### Retry count " + i + " ##");
                        SteamCMDOutputAddLine("################\n");
                    });
                }

                foreach (string id in iDs)
                {
                    if (AbortUpdate) break;

                    if (_debug) Console.WriteLine("Debug -> ExecuteSteamCMD: id " + id);

                    try
                    {
                        if (id != "0")
                        {
                            AddonItem it = null;

                            foreach (AddonItem item in Addons.Where(x => x.ID == id))
                            {
                                it = item;
                                break;
                            }

                            if (it == null || it.UpdateStatus == 1) continue;
                        }

                        string loginString = AnonymousSteamLogon ? " +login anonymous " : " +login " + SteamAccount + " " + pw + " " + gc + " ";

                        string bandwidthLimitString = LimitBandwidth ? ("+set_download_throttle " + MaxBandwidth) : "";

                        //string timeoutString = "\"+@csecCSRequestProcessorTimeOut 900\" ";

                        string installDirString = "+force_install_dir " + SteamRootFolder + " ";

                        string dlString = id != "0" ? "+workshop_download_item 107410 " + id + " -validate " : "+app_update 233780 -validate ";

                        string quitString = "+quit";

                        if (_debug) Console.WriteLine("Debug -> ExecuteSteamCMD: SteamCMD arguments:" + loginString + bandwidthLimitString + installDirString + dlString + quitString);

                        var startInfo = new ProcessStartInfo
                        {
                            FileName = SteamCMDFolder + "\\steamcmd.exe",
                            Arguments = loginString + bandwidthLimitString + installDirString + dlString + quitString,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            RedirectStandardInput = false
                        };

                        var process = new Process
                        {
                            StartInfo = startInfo
                        };

                        try
                        {
                            process.Start();

                            Thread.Sleep(5);
                        }
                        catch (Exception e)
                        {
                            System.Windows.MessageBox.Show(e.Message + "\n" + e.StackTrace);
                        }

                        process.BeginOutputReadLine();

                        process.OutputDataReceived += Process_OutputDataReceived;

                        while (process != null &&
                               !process.HasExited)
                        {
                            Thread.Sleep(200);
                        }

                        foreach (AddonItem item in Addons.Where(x => x.ID == id))
                        {
                            if (item.UpdateStatus != 1) item.UpdateStatus = 2;
                        }

                        if (!_autoGenerate) Application.Current.Dispatcher.Invoke(() => SteamCMDOutputAddLine("Disconnected.\n", true));
                    }
                    catch (Exception e)
                    {
                        if (_debug) Console.WriteLine("Debug -> ExecuteSteamCMD: Error " + e.Message);
                        UpdateActive = false;
                        AbortUpdate = false;
                        System.Windows.MessageBox.Show("\nError while working on id " + id + ":\n" + e.Message);
                    }

                }

                if (AbortUpdate) i = 99999;
            }

            if (!AbortUpdate) foreach (AddonItem item in Addons) if (item.UpdateStatus == 2) item.UpdateStatus = 3;

            if (iDs.Contains("0") && !_autoGenerate)
            {
                if (ServerUpdateStatus == 1)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SteamCMDOutputAddLine("################", true);
                        SteamCMDOutputAddLine("##### Success #####");
                        SteamCMDOutputAddLine("################\n");
                    });
                }
                else
                {
                    ServerUpdateStatus = 2;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SteamCMDOutputAddLine("################", true);
                        SteamCMDOutputAddLine("##### Failed #####");
                        SteamCMDOutputAddLine("################\n");
                    });
                }
            }

            UpdateActive = false;
            AbortUpdate = false;
            if (_debug) Console.WriteLine("Debug -> ExecuteSteamCMD: UpdateActive = false");

            if (!_autoGenerate)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SteamCMDOutputAddLine("################", true);
                    SteamCMDOutputAddLine("##### Finished ####");
                    SteamCMDOutputAddLine("################\n");
                });
            }

            return null;
        }

        private string _lastLine;

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string newLine = e.Data;

            if (newLine == null) return;

            if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(newLine, "Success. Downloaded item", CompareOptions.IgnoreCase) >= 0) //_updateResult = 2;
            {
                string snip = newLine.Substring("Success. Downloaded item ".Length, 20);
                snip = snip.Substring(0, snip.IndexOf(" to"));

                foreach (AddonItem item in Addons.Where(x => x.ID == snip)) item.UpdateStatus = 1;
                CheckUpdateStatus();
            }

            if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(newLine, "Success! App", CompareOptions.IgnoreCase) >= 0) //_updateResult = 2;
            {
                ServerUpdateStatus = 1;

                if (_debug && !newLine.StartsWith("Debug")) Console.WriteLine("Debug -> Process_OutputDataReceived: Success for server update");
            }

            if (!_autoGenerate)
            {
                if (newLine == _lastLine)
                {
                    Application.Current.Dispatcher.Invoke(() => SteamCMDOutputAppend("+"));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() => SteamCMDOutputAddLine(newLine));
                    _lastLine = newLine;
                }
            }
            else
            {
                if (_debug) Console.WriteLine("Debug -> Process_OutputDataReceived: " + newLine);
            }
        }

        private void OnSteamWorkshopIDsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            WriteSettings();
        }

        private void OnSteamRootfolderChanged()
        {
            if (!String.IsNullOrEmpty(SteamRootFolder))
            {
                AddonFolder = SteamRootFolder + "\\steamapps\\workshop\\content\\107410";
            }
        }
        private void OnAbortUpdate(object obj)
        {
            AbortUpdate = true;
        }

        #endregion

        #region Methods addon purge

        private void OnAddPreservedAddon(object obj)
        {
            if ((obj as IEnumerable<object>).Count() > 0)
            {
                List<string> dif = new List<string>();

                foreach (object o in (obj as IEnumerable<object>))
                {
                    dif.Add(o as string);
                }

                foreach (string s in dif)
                {
                    PreservedAddons.Add(s);
                    UnknownAddons.Remove(s);
                }
            }
        }

        private void OnRemovePreservedAddon(object obj)
        {
            if ((obj as IEnumerable<object>).Count() > 0)
            {
                List<string> dif = new List<string>();

                foreach (object o in (obj as IEnumerable<object>))
                {
                    dif.Add(o as string);               
                }

                foreach (string s in dif)
                {
                    UnknownAddons.Add(s);
                    PreservedAddons.Remove(s);
                }                
            }
        }

        private void ScanLocalAddons(object obj)
        {
            List<string> directories = Directory.GetDirectories(AddonFolder).ToList();
            UnknownAddons.Clear();

            foreach (string dir in directories)
            {
                if (Addons.Where(x => x.ID == dir.Substring(dir.LastIndexOf("\\")+1) &&
                                      x.IsSelected).Count() < 1)
                {
                    if (!PreservedAddons.Contains(dir)) UnknownAddons.Add(dir);
                }
            }
        }

        private void OnRemoveSelectedAddons(object obj)
        {
            _deletedAddons = 0;
            foreach (string rm in UnknownAddons)
            {
                if (Directory.Exists(rm)) Directory.Delete(rm, true);
                _deletedAddons++;
            }
            ScanLocalAddons(null);
        }
        
        private void OnPreservedAddonsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            WriteSettings();
        }
        
        #endregion

        #region Methods batch file creation

        private void OpenProfiles(object obj)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

            fbd.Description = "Select profiles directory";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                ProfilesFolder = fbd.SelectedPath;
            }
        }

        private void OpenConfig(object obj)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            ofd.Title = "Select config file";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Config = ofd.FileName;
            }            
        }

        private void ScanAddons()
        {
            if (Directory.Exists(AddonFolder))
            {
                List<string> directories = Directory.GetDirectories(AddonFolder).ToList();

                FolderContent = String.Join(";\n", directories);
            }
        }

        private void WriteSyntax(object obj)
        {
            if (String.IsNullOrEmpty(RootFolder) ||
                String.IsNullOrEmpty(Config) ||
                String.IsNullOrEmpty(ProfilesFolder))
            {
                System.Windows.MessageBox.Show("Check folders and config!");
            }

            ScanAddons();

            ResultContent = "";
            ResultContent += Is64bit ? "arma3server_x64.exe " : "arma3server.exe ";
            if (WritePort) ResultContent += "-port=" + Port + " ";
            if (WriteFilePatching) ResultContent += "-filePatching ";
            if (WriteAutoInit) ResultContent += "-autoInit ";
            if (WriteHyperThreading) ResultContent += "-enableHT ";
            ResultContent += "\"-config=" + Config + "\" ";
            ResultContent += "\"-profiles=" + ProfilesFolder + "\" ";
            if (!String.IsNullOrEmpty(FolderContent)) ResultContent += "-mod=\"" + FolderContent.Replace("\n", "") + "\" ";
            if (!String.IsNullOrEmpty(ServerAddonsFolder)) ResultContent += "-serverMod=\"" + ServerAddonsFolder + "\"";

        }

        private void CreateBat(object obj)
        {
            if (String.IsNullOrEmpty(ResultContent))
            {
                System.Windows.MessageBox.Show("Write syntax first!");
                return;
            }

            string path = Path.Combine(RootFolder, FileName + ".bat");

            using (StreamWriter file = new StreamWriter(path, false))
            {
                file.Write(ResultContent);
            }
        }

        private void CreateHCBat(object obj)
        {
            ScanAddons();

            string hcContent = "";
            hcContent += Is64bit ? "arma3server_x64.exe " : "arma3server.exe ";
            hcContent += "-client -password=foxtrott5 -name=server ";
            if (WritePort) hcContent += "-port=" + Port + " ";
            hcContent += "-filePatching ";
            hcContent += "-enableHT ";
            hcContent += "\"-config=" + Config + "\" ";
            hcContent += "\"-profiles=" + ProfilesFolder + "\" ";
            if (!String.IsNullOrEmpty(FolderContent)) hcContent += "-mod=\"" +
                                                                    FolderContent.Replace("\n", "") +
                                                                    (String.IsNullOrEmpty(ServerAddonsFolder) ? "" : ";" + ServerAddonsFolder) +
                                                                    "\" ";
            
            string path = Path.Combine(RootFolder, HeadlessClientFileName + ".bat");

            using (StreamWriter file = new StreamWriter(path, false))
            {
                file.Write(hcContent);
            }
        }

        #endregion

        #region Notifypropertychanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
