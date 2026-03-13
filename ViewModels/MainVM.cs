using AutomotiveBuilder.DAL;
using AutomotiveBuilder.Properties;
using AutomotiveBuilder.Static;
using AutomotiveBuilder.Statics;
using AutomotiveBuilder.ViewModels.DriveTrain;
using AutomotiveBuilder.Views;
using AutomotiveBuilder.Views.DriveTrain;
using AutomotiveBuilder.Views.PartUtils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using static AutomotiveBuilder.Static.MessengerService;

namespace AutomotiveBuilder
{
    public partial class MainViewModel : ObservableObject
    {
        #region Message Handlers


        /// TODO: Implement accross the app to handle data updates for refreshing lists and views.
        ///  


        private void OnNewMessage(string text, MessageType type)
        {
            // Handle incoming messages from the MessengerService
                switch (type)
                {
                    case MessageType.Info:
                        // Handle info messages
                        break;
                    case MessageType.Warning:
                        // Handle warning messages
                        break;
                    case MessageType.Error:
                        // Handle error messages
                        break;
                    case MessageType.DataChanged:
                        // Handle data changed messages, e.g., refresh views or update properties
                        switch(text)
                    {
                        case "Part Catagories":
                            OnPropertyChanged(nameof(PartCatagories));
                            OnPropertyChanged(nameof(PartSubCatagories));
                            break;

                        default:

                            break;
                    }
                        break;
            }
        }
        private void SendMessage(string text, MessageType type = MessageType.Info)
        {
            MessengerService.LogMessage(text, type);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

            MessengerService.MessageReceived += OnNewMessage;


            string projectFile = Settings.Default.CurrentProjectFile;
            if (File.Exists(projectFile))
            {
                try
                {
                    Stream reader = new FileStream(projectFile, FileMode.Open);
                    var serializer = new XmlSerializer(typeof(Project));
                    _CurrentProject = (Project) serializer.Deserialize(reader);
                    _CurrentProject.BOM = new AutoBuilder(_CurrentProject.ID);

                }
                catch
                {
                    //trips on startup, ignore
                    _CurrentProject = new Project();
                }
            }
            else
            {
                _CurrentProject = new Project();
            }
            _CurrentProject.PropertyChanged += (s, e) => { UpdateAllProperties(); };
            _MakeList = StaticMethods.LoadMakes();
            CalculateInjectorAF();
            CalculateInjectorRequiredFlow();
            CalculateET();
            _CurrentProject.BOM.PropertyChanged += (s, e) => { UpdateAllProperties(); };
            if((_CurrentProject.BOM !=null) && (_CurrentProject.BOM.Vehicle != null) && (_CurrentProject.BOM.Vehicle.Components != null))
            {
                if(_CurrentProject.BOM.Vehicle.Components.Count > 0)
                {
                    _CurrentProject.SelectedComponent = _CurrentProject.BOM.Vehicle.Components[0];
                }
            }
            _Loaded = true;
        }

        #endregion

        #region Private Members
        private Project _CurrentProject;
        private string _FileName = "";
        private MakesList _MakeList;
        private BindingList<Model> _ModelList;
        private float _calcToolInjRF = 80.0f;
        private float _calcToolInjRP = 60.0f;
        private float _calcToolInjAP = 43.0f;
        private float _calcToolInjAF;
        private float _calcCHP = 600.0f;
        private int _calcNoInj = 8;
        private float _calcBSFC = 0.55f;
        private float _calcDutyCycle = 0.75f;
        private int _calcInjectorSize = 0;
        private int _calcWHP = 600;
        private int _calcVehicalWght = 3000;
        private float _calc14et = 9.0f;
        private float _calc18et = 6.5f;
        private float _calc14mph = 145;
        private float _calc18mph = 111;
        private bool _calcPerf = false;
        private bool _Loaded = false;
        private DriveTrainVM _driveTrainVM;
        #endregion

        #region Properties
        public Project CurrentProject
        {
            get { return _CurrentProject; }
        }
        public MakesList ProjMakeList
        {
            get { return _MakeList; }
        }

        #region References
        public BindingList<Model> ProjModelList
        {
            get 
            {
                if (CurrentProject != null)
                {
                    return GetMakeModelList(CurrentProject.MakeID);
                }
                else
                {
                    return null;
                }
            }
        }
        public BindingList<Engine> ProjEngineModelList
        {
            get
            {
                return GetMakeEngineModelList(CurrentProject.EngineMakeID);
            }
        }
        public BindingList<PartCatagory> PartCatagories
        {
            get
            {                
                return StaticMethods.PartCatagories;
            }
        }
        public BindingList<PartSubCatagory> PartSubCatagories
        {
            get
            {
                BindingList<PartSubCatagory> _PartSubCatagories = new BindingList<PartSubCatagory>();
                if ((_CurrentProject.SelectedComponent != null)  && (_CurrentProject.SelectedComponent.CategoryID != ""))
                {
                    foreach (var cat in StaticMethods.PartCatagories)
                    {
                        if (cat.ID == _CurrentProject.SelectedComponent.CategoryID)
                        {
                            _PartSubCatagories = cat.SubCategories;
                            return _PartSubCatagories;
                        }
                    }
                }
                return _PartSubCatagories;
            }
        }
        public bool Loaded { get{ return _Loaded; } }

        public DriveTrainVM DriveTrainVM
        {
            get
            {
                if (_driveTrainVM == null)
                {
                    _driveTrainVM = new DriveTrainVM();
                    _driveTrainVM.DriveTrainData = _CurrentProject.DriveTrain;
                }
                return _driveTrainVM;
            }
        }

        #endregion

        #region Calculator Properties
        public float calcToolInjRF
        {
            get { return _calcToolInjRF; }
            set
            {
                _calcToolInjRF = value;
                OnPropertyChanged(nameof(calcToolInjRF));
                CalculateInjectorAF();
            }
        }
        public float calcToolInjRP
        {
            get { return _calcToolInjRP; }
            set
            {
                _calcToolInjRP = value;
                OnPropertyChanged(nameof(calcToolInjRP));
                CalculateInjectorAF();
            }
        }
        public float calcToolInjAP
        {
            get { return _calcToolInjAP; }
            set
            {
                _calcToolInjAP = value;
                OnPropertyChanged(nameof(calcToolInjAP));
                CalculateInjectorAF();
            }
        }
        public float calcToolInjAF
        {
            get { return _calcToolInjAF; }
        }
        public float calcCHP
        {
            get { return _calcCHP; }
            set
            {
                _calcCHP = value;
                OnPropertyChanged(nameof(calcCHP));
                CalculateInjectorRequiredFlow();
            }
        }
        public int calcNoInj
        {
            get { return _calcNoInj; }
            set
            {
                _calcNoInj = value;
                OnPropertyChanged(nameof(calcNoInj));
                CalculateInjectorRequiredFlow();
            }
        }
        public float calcBSFC
        {
            get { return _calcBSFC; }
            set
            {
                _calcBSFC = value;
                OnPropertyChanged(nameof(calcBSFC));
                CalculateInjectorRequiredFlow();
            }
        }
        public float calcDutyCycle
        {
            get { return _calcDutyCycle; }
            set
            {
                _calcDutyCycle = value;
                OnPropertyChanged(nameof(calcDutyCycle));
                CalculateInjectorRequiredFlow();
            }
        }
        public int calcInjectorSize
        {
            get { return _calcInjectorSize; }
        }
        public int calcWHP
        {
            get { return _calcWHP; }
            set
            {
                _calcWHP = value;
                OnPropertyChanged(nameof(calcWHP));
                if(!_calcPerf) CalculateET();
            }
        }
        public int calcVehicalWght
        {
            get {return _calcVehicalWght; }
            set
            {
                _calcVehicalWght = value;
                OnPropertyChanged(nameof(calcVehicalWght));
                if (!_calcPerf) CalculateET();
            }
        }
        public float calc14et
        {
            get { return _calc14et; }
            set
            {
                _calc14et = value;
                OnPropertyChanged(nameof(calc14et));
                if (!_calcPerf) CalculateRWHP();
            }
        }
        public float calc18et
        {
            get { return _calc18et; }
            set
            {
                _calc18et = value;
                OnPropertyChanged(nameof(calc18et));
                if (!_calcPerf) CalculateFeRWHP();
            }
        }
        public float calc14mph
        {
            get { return _calc14mph; }
            set
            {
                _calc14mph = value;
                if (!_calcPerf) CalculateTSRWHP();
            }
        }
        public float calc14mphW
        {
            get { return _calc14mph; }
            set
            {
                _calc14mph = value;
                if (!_calcPerf) CalculateVW();
            }
        }
        public float calc18mph
        {
            get { return _calc18mph; }
            set
            {
                _calc18mph = value;
                OnPropertyChanged(nameof(calc18mph));
                if (!_calcPerf) CalculateTSERWHP();
            }
        }
        public float calc18mphW
        {
            get { return _calc18mph; }
            set
            {
                _calc18mph = value;
                OnPropertyChanged(nameof(calc18mph));
                if (!_calcPerf) CalculateVWe();
            }
        }

        #endregion

        #endregion

        #region Relay Commands

        [RelayCommand]
        private void AddProject(object args)
        {
            OnPropertyChanged(nameof(args.ToString));
            UpdateAllProperties();
        }

        [RelayCommand]
        private void SaveProject(object args)
        {           
            if (_CurrentProject.FileName == "")
            {
                _CurrentProject.FileName = StaticMethods.ProjectFolder + "ExampleProject.xml";
            }
            _CurrentProject.SaveProject();
            _CurrentProject.BOM.SaveBOM();
            OnPropertyChanged(nameof(args.ToString));
            UpdateAllProperties();
        }

        [RelayCommand]
        private void SaveProjectAs(object args)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "html File|*.html|xml File|*.xml";
            saveFileDialog1.Title = "Save an Project File";
            saveFileDialog1.ShowDialog();

            string strFileName = "";

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                strFileName = saveFileDialog1.FileName;

                var serializer = new XmlSerializer(typeof(Project));

                if (File.Exists(strFileName))
                {
                    string strCaption = "File Exists!";
                    string strMessage = "A file with this name exists Override the existing file?";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;

                    result = MessageBox.Show(strMessage, strCaption, buttons);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                using (var writer = new StreamWriter(strFileName))
                {
                    serializer.Serialize(writer, _CurrentProject);
                }
                OnPropertyChanged(nameof(args.ToString));
                UpdateAllProperties();
            }
        }

        [RelayCommand]
        private void OpenProject(object args)
        {
            var fileContent = string.Empty;

            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {

                openFileDialog.InitialDirectory = StaticMethods.ProjectFolder;
                openFileDialog.Filter = "html files (*.html)|*.html|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    _FileName = openFileDialog.FileName;                    

                    using (Stream reader = new FileStream(_FileName, FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(Project));
                        _CurrentProject = new Project();
                        // Call the Deserialize method to restore the object's state.
                        _CurrentProject = (Project) serializer.Deserialize(reader);
                    }
                    _MakeList = StaticMethods.LoadMakes();
                }
                UpdateAllProperties();
            }
        }

        [RelayCommand]
        private void SetMake(object args)
        {
           // Make tmpMake = ProjMakeList.ReturnMakeByID(MakeID);
            //if (tmpMake != null)
            //{
            //    _CurrentProject.Make = tmpMake.Name;
            //    UpdateAllProperties();
            //}
        }

        [RelayCommand]
        private void OpenMakeModelRepo(object args)
        {
            MakeModelRepo makeModelRepo = new MakeModelRepo();
            makeModelRepo.Show();
        }

        [RelayCommand]
        private void OpenMakeEngineRepo(object args)
        {
            MakeEngineRepo makeEngineRepo = new MakeEngineRepo();
            makeEngineRepo.Show();
        }

        [RelayCommand]
        private void OpenVenders(object args)
        {
            VendersView vendersView = new VendersView();
            vendersView.Show();
        }

        [RelayCommand]
        private void OpenPartCatagories(object args)
        {
            PartCatagoriesView partCatagories = new PartCatagoriesView();
            partCatagories.Show();
        }

        [RelayCommand]
        private void OpenInjectorCalc(object args)
        {
            InjectorCalcView injCalc = new InjectorCalcView();
            injCalc.Show();
        }

        [RelayCommand]
        private void OpenHPETCalc(object args)
        {
            HPandETcalculatorView hpetCalc = new HPandETcalculatorView();
            hpetCalc.Show();
        }

        [RelayCommand]
        private void AddBOMitem(object args)
        {
            if (_CurrentProject.SelectedComponent == null) return;
            DAL.Component nc = new DAL.Component();
            nc.Name = _CurrentProject.SelectedComponent.Name + " new item";
            nc.ParentID = _CurrentProject.SelectedComponent.ID;
            nc.CategoryID = _CurrentProject.SelectedComponent.CategoryID;
            nc.SubCategoryID = _CurrentProject.SelectedComponent.SubCategoryID;
            _CurrentProject.SelectedComponent.AddComponent(nc);
            OnPropertyChanged(nameof(CurrentProject.BOM.Vehicle.Components));
        }

        [RelayCommand]
        private void DeleteItem(object args)
        {
            if (_CurrentProject.SelectedComponent == null) return;
            if (MessageBox.Show("Delete " + _CurrentProject.SelectedComponent.Name + "?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (DAL.Component bc in _CurrentProject.BOM.Vehicle.Components)
                {
                    if (bc.ID == _CurrentProject.SelectedComponent.ID)
                    {
                        _CurrentProject.BOM.Vehicle.Components.Remove(bc);
                        break;
                    }
                    else
                    {
                        if(RemoveSubComponent(bc, _CurrentProject.SelectedComponent.ID)) break;
                    }
                }
                OnPropertyChanged(nameof(CurrentProject.BOM.Vehicle.Components));
            }
            Cursor.Current = Cursors.Default;
        }

        private bool RemoveSubComponent(DAL.Component TargetComponent, string ID)
        {
            bool rtn = false;

            if (_CurrentProject.SelectedComponent == null) return rtn;
            foreach (DAL.Component bc in TargetComponent.Components)
            {
                if (bc.ID == ID)
                {
                    TargetComponent.Components.Remove(bc);
                    rtn = true;
                    break;
                }
                else
                {
                    rtn =RemoveSubComponent(bc, ID);
                    if (rtn) break;
                }
            }
            return rtn;
        }

        [RelayCommand]
        private void AddPart(object args)
        { 
            SelectPartView spv = new SelectPartView();
            spv.ShowDialog();
            _CurrentProject.SelectedComponent.AddPart(spv.SelectedPart);
            spv.Close();
            OnPropertyChanged(nameof(CurrentProject.BOM.Vehicle.Components));
        }

        [RelayCommand]
        private void EnablePartSel(object args)
        {
            if (_CurrentProject.SelectedComponent == null) return;
            _CurrentProject.SelectedComponent.EnableSelPrt = !_CurrentProject.SelectedComponent.EnableSelPrt;
        }

        [RelayCommand]
        private void OpenPartURL(object args)
        {
            if (_CurrentProject.SelectedComponent == null) return;
            if (_CurrentProject.SelectedComponent.SelectedPart == null) return;
            if (_CurrentProject.SelectedComponent.SelectedPart.URL == "") return;
            Process.Start(new ProcessStartInfo { FileName = _CurrentProject.SelectedComponent.SelectedPart.URL, UseShellExecute = true });
        }

        [RelayCommand]
        private void OpenDriveTrainCalc(object args)
        {
            DriveTrainView driveTrainView = new DriveTrainView();
            driveTrainView.Show();
            //DriveTrainVM DTVM = driveTrainView.DataContext as DriveTrainVM;
            driveTrainView.DataContext = new DriveTrainVM();
            ((DriveTrainVM)driveTrainView.DataContext).DriveTrainData = CurrentProject.DriveTrain;
        }
        #endregion

        #region Private Routines

        private BindingList<Model> GetMakeModelList(string MakeID)
        {
            BindingList<Model> modelList = new BindingList<Model>();
            Make tmpMake = ProjMakeList.ReturnMakeByID(MakeID);
            if ((tmpMake != null) && (tmpMake.Models != null))
            {
                foreach (Model mod in tmpMake.Models)
                {
                    modelList.Add(mod);
                }
            }
            return modelList;
        }
        private BindingList<Engine> GetMakeEngineModelList(string MakeID)
        {
            BindingList<Engine> modelList = new BindingList<Engine>();
            Make tmpMake = ProjMakeList.ReturnMakeByID(MakeID);
            if ((tmpMake != null) && (tmpMake.Engines != null))
            {
                foreach (Engine mod in tmpMake.Engines)
                {
                    modelList.Add(mod);
                }
            }
            return modelList;
        }
        private void CalculateInjectorAF()
        {
            
            if ((calcToolInjRP != 0) && (calcToolInjAP !=0))
            {
                _calcToolInjAF = StaticMethods.CalculateInjectorFlowPressureCorrection(calcToolInjRF, calcToolInjRP, calcToolInjAP);
            }
            else
            {
                _calcToolInjAF = 0;
            }
            OnPropertyChanged(nameof(calcToolInjAF));
        }
        private void CalculateInjectorRequiredFlow()
        {

            if ((calcBSFC != 0) && (calcNoInj != 0))
            {
                _calcInjectorSize = StaticMethods.CalculateInjectorRequiredFlow(calcCHP,calcNoInj,calcBSFC,calcDutyCycle);
            }
            else
            {
                _calcInjectorSize = 0;
            }
            OnPropertyChanged(nameof(calcInjectorSize));
        }
        private void CalculateET()
        {
            _calcPerf = true;

            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc14mph =(float) (234.0 * Math.Pow(((float)_calcWHP /_calcVehicalWght),((float)1 / 3)));
            _calc18mph = (float)(147.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            OnPropertyChanged(nameof(calc14et));
            OnPropertyChanged(nameof(calc18et));
            OnPropertyChanged(nameof(calc14mph));
            OnPropertyChanged(nameof(calc14mphW));
            OnPropertyChanged(nameof(calc18mph));
            OnPropertyChanged(nameof(calc18mphW));
            _calcPerf = false;
        }
        private void CalculateRWHP()
        {
            _calcPerf = true;

            _calcWHP = (int) Math.Round(((float)_calcVehicalWght / Math.Pow((_calc14et / 5.825), 3)),0);
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc14mph = (float)(234.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));
            _calc18mph = (float)(147.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            OnPropertyChanged(nameof(calcWHP));
            OnPropertyChanged(nameof(calc18et));
            OnPropertyChanged(nameof(calc14mph));
            OnPropertyChanged(nameof(calc14mphW));
            OnPropertyChanged(nameof(calc18mph));
            OnPropertyChanged(nameof(calc18mphW));
            _calcPerf = false;
        }
        private void CalculateFeRWHP()
        {
            _calcPerf = true;

            _calcWHP = (int)Math.Round(((float)_calcVehicalWght / Math.Pow((_calc18et / 3.58), 3)), 0);
            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc14mph = (float)(234.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));
            _calc18mph = (float)(147.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            OnPropertyChanged(nameof(calcWHP));
            OnPropertyChanged(nameof(calc14et));
            OnPropertyChanged(nameof(calc14mph));
            OnPropertyChanged(nameof(calc14mphW));
            OnPropertyChanged(nameof(calc18mph));
            OnPropertyChanged(nameof(calc18mphW));
            _calcPerf = false;
        }
        private void CalculateTSRWHP()
        {
            _calcPerf = true;

            _calcWHP = (int) Math.Round((float) _calcVehicalWght * Math.Pow((_calc14mph/234), 3),0);
            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18mph = (float)(147.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            OnPropertyChanged(nameof(calcWHP));
            OnPropertyChanged(nameof(calc14et));
            OnPropertyChanged(nameof(calc18et));
            OnPropertyChanged(nameof(calc14mph));
            OnPropertyChanged(nameof(calc14mphW));
            OnPropertyChanged(nameof(calc18mph));
            OnPropertyChanged(nameof(calc18mphW));
            _calcPerf = false;
        }
        private void CalculateTSERWHP()
        {
            _calcPerf = true;

            _calcWHP = (int)Math.Round((float)_calcVehicalWght * Math.Pow((_calc18mph / 147), 3), 0);
            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc14mph = (float)(234 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            OnPropertyChanged(nameof(calcWHP));
            OnPropertyChanged(nameof(calc14et));
            OnPropertyChanged(nameof(calc18et));
            OnPropertyChanged(nameof(calc14mph));
            OnPropertyChanged(nameof(calc14mphW));
            OnPropertyChanged(nameof(calc18mph));
            OnPropertyChanged(nameof(calc18mphW));

            _calcPerf = false;
        }
        private void CalculateVWe()
        {
            _calcPerf = true;

            _calcVehicalWght = (int) Math.Round((float) _calcWHP / Math.Pow((_calc18mph / 147), 3),0);
            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc14mph = (float)(234 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            OnPropertyChanged(nameof(calcVehicalWght));
            OnPropertyChanged(nameof(calc14et));
            OnPropertyChanged(nameof(calc18et));
            OnPropertyChanged(nameof(calc14mph));
            OnPropertyChanged(nameof(calc14mphW));
            OnPropertyChanged(nameof(calc18mph));

            _calcPerf = false;
        }
        private void CalculateVW()
        {
            _calcPerf = true;

            _calcVehicalWght = (int)Math.Round((float)_calcWHP / Math.Pow((_calc14mph / 234), 3), 0);
            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18mph = (float)(147 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            OnPropertyChanged(nameof(calcVehicalWght));
            OnPropertyChanged(nameof(calc14et));
            OnPropertyChanged(nameof(calc18et));
            OnPropertyChanged(nameof(calc18mph));
            OnPropertyChanged(nameof(calc18mphW));
            OnPropertyChanged(nameof(calc14mph));
            OnPropertyChanged(nameof(calc14mphW));
            OnPropertyChanged(nameof(calc18mph));

            _calcPerf = false;
        }

        #endregion

        #region PropertyEvents
        private void UpdateAllProperties()
        {
            OnPropertyChanged(nameof(CurrentProject));
            OnPropertyChanged(nameof(ProjModelList));
            OnPropertyChanged(nameof(ProjEngineModelList));
            OnPropertyChanged(nameof(PartCatagories));
            OnPropertyChanged(nameof(PartSubCatagories));
        }
        #endregion
    }
}
