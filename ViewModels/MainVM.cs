using AutomotiveBuilder.DAL;
using AutomotiveBuilder.Properties;
using AutomotiveBuilder.Static;
using AutomotiveBuilder.Statics;
using AutomotiveBuilder.ViewModels.DriveTrain;
using AutomotiveBuilder.Views;
using AutomotiveBuilder.Views.DriveTrain;
using AutomotiveBuilder.Views.PartUtils;
using MVVMtools;
using MVVMtools.Command;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using static AutomotiveBuilder.Static.MessengerService;

namespace AutomotiveBuilder
{
    class MainViewModel : ViewModelBase
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
                            RaisePropertyChanged(nameof(PartCatagories));
                            RaisePropertyChanged(nameof(PartSubCatagories));
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
                RaisePropertyChanged(nameof(calcToolInjRF));
                CalculateInjectorAF();
            }
        }
        public float calcToolInjRP
        {
            get { return _calcToolInjRP; }
            set
            {
                _calcToolInjRP = value;
                RaisePropertyChanged(nameof(calcToolInjRP));
                CalculateInjectorAF();
            }
        }
        public float calcToolInjAP
        {
            get { return _calcToolInjAP; }
            set
            {
                _calcToolInjAP = value;
                RaisePropertyChanged(nameof(calcToolInjAP));
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
                RaisePropertyChanged(nameof(calcCHP));
                CalculateInjectorRequiredFlow();
            }
        }
        public int calcNoInj
        {
            get { return _calcNoInj; }
            set
            {
                _calcNoInj = value;
                RaisePropertyChanged(nameof(calcNoInj));
                CalculateInjectorRequiredFlow();
            }
        }
        public float calcBSFC
        {
            get { return _calcBSFC; }
            set
            {
                _calcBSFC = value;
                RaisePropertyChanged(nameof(calcBSFC));
                CalculateInjectorRequiredFlow();
            }
        }
        public float calcDutyCycle
        {
            get { return _calcDutyCycle; }
            set
            {
                _calcDutyCycle = value;
                RaisePropertyChanged(nameof(calcDutyCycle));
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
                RaisePropertyChanged(nameof(calcWHP));
                if(!_calcPerf) CalculateET();
            }
        }
        public int calcVehicalWght
        {
            get {return _calcVehicalWght; }
            set
            {
                _calcVehicalWght = value;
                RaisePropertyChanged(nameof(calcVehicalWght));
                if (!_calcPerf) CalculateET();
            }
        }
        public float calc14et
        {
            get { return _calc14et; }
            set
            {
                _calc14et = value;
                RaisePropertyChanged(nameof(calc14et));
                if (!_calcPerf) CalculateRWHP();
            }
        }
        public float calc18et
        {
            get { return _calc18et; }
            set
            {
                _calc18et = value;
                RaisePropertyChanged(nameof(calc18et));
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
                RaisePropertyChanged(nameof(calc18mph));
                if (!_calcPerf) CalculateTSERWHP();
            }
        }
        public float calc18mphW
        {
            get { return _calc18mph; }
            set
            {
                _calc18mph = value;
                RaisePropertyChanged(nameof(calc18mph));
                if (!_calcPerf) CalculateVWe();
            }
        }

        #endregion

        #endregion

        #region Relay Commands
        private RelayCommand<object> _AddProject;
        private RelayCommand<object> _SaveProject;
        private RelayCommand<object> _SaveProjectAs;
        private RelayCommand<object> _OpenProject;
        private RelayCommand<object> _SetMake;
        private RelayCommand<object> _OpenMakeModelRepo;
        private RelayCommand<object> _OpenMakeEngineRepo;
        private RelayCommand<object> _OpenVenders;
        private RelayCommand<object> _OpenPartCatagories;
        private RelayCommand<object> _OpenInjectorCalc;
        private RelayCommand<object> _OpenHPETCalc;
        private RelayCommand<object> _AddBOMitem;
        private RelayCommand<object> _AddPart;
        private RelayCommand<object> _EnablePartSel;
        private RelayCommand<object> _OpenPartURL;
        private RelayCommand<object> _DeleteItem;
        private RelayCommand<object> _OpenDriveTrainCalc;

        public RelayCommand<object> AddProjectCommand
        {
            get
            {
                return _AddProject ?? (_AddProject = new RelayCommand<object>((X) => AddProject(X)));
            }
        }
        public RelayCommand<object> SaveProjectCommand
        {
            get
            {
                return _SaveProject ?? (_SaveProject = new RelayCommand<object>((X) => SaveProject(X)));
            }
        }
        public RelayCommand<object> SaveProjectAsCommand
        {
            get
            {
                return _SaveProjectAs ?? (_SaveProjectAs = new RelayCommand<object>((X) => SaveProjectAs(X)));
            }
        }
        public RelayCommand<object> OpenProjectCommand
        {
            get
            {
                return _OpenProject ?? (_OpenProject = new RelayCommand<object>((X) => OpenProject(X)));
            }
        }
        public RelayCommand<object> SetMakeCommand
        {
            get
            {
                return _SetMake ?? (_OpenProject = new RelayCommand<object>((X) => SetMake(X)));
            }
        }
        public RelayCommand<object> OpenMakeModelRepoCommand
        {
            get
            {
                return _OpenMakeModelRepo ?? (_OpenProject = new RelayCommand<object>((X) => OpenMakeModelRepo(X)));
            }
        }
        public RelayCommand<object> OpenMakeEngineRepoCommand
        {
            get
            {
                return _OpenMakeEngineRepo ?? (_OpenMakeEngineRepo = new RelayCommand<object>((X) => OpenMakeEngineRepo(X)));
            }
        }
        public RelayCommand<object> OpenVendersCommand
        {
            get
            {
                return _OpenVenders ?? (_OpenVenders = new RelayCommand<object>((X) => OpenVenders(X)));
            }
        }
        public RelayCommand<object> OpenPartCatagoriesCommand
        {
            get
            {
                return _OpenPartCatagories ?? (_OpenPartCatagories = new RelayCommand<object>((X) => OpenPartCatagories(X)));
            }
        }
        public RelayCommand<object> OpenInjectorCalcCommand
        {
            get
            {
                return _OpenInjectorCalc ?? (_OpenInjectorCalc = new RelayCommand<object>((X) => OpenInjectorCalc(X)));
            }
        }
        public RelayCommand<object> OpenHPETCalcCommand
        {
            get
            {
                return _OpenHPETCalc ?? (_OpenHPETCalc = new RelayCommand<object>((X) => OpenHPETCalc(X)));
            }
        }
        public RelayCommand<object> AddBOMitemCommand
        {
            get
            {
                return _AddBOMitem ?? (_AddBOMitem = new RelayCommand<object>((X) => AddBOMitem(X)));
            }
        }
        public RelayCommand<object> AddPartCommand
        {
            get
            {
                return _AddPart ?? (_AddPart = new RelayCommand<object>((X) => AddPart(X)));
            }
        }
        public RelayCommand<object> EnablePartSelCommand
        {
            get
            {
                return _EnablePartSel ?? (_EnablePartSel = new RelayCommand<object>((X) => EnablePartSel(X)));
            }
        }
        public RelayCommand<object> OpenPartURLCommand
        {
            get
            {
                return _OpenPartURL ?? (_OpenPartURL = new RelayCommand<object>((X) => OpenPartURL(X)));
            }
        }
        public RelayCommand<object> DeleteItemCommand
        {
            get
            {
                return _DeleteItem ?? (_DeleteItem = new RelayCommand<object>((X) => DeleteBOMitem(X)));
            }
        }
        public RelayCommand<object> OpenDriveTrainCalcCommand
        {
            get
            {
                return _OpenDriveTrainCalc ?? (_OpenDriveTrainCalc = new RelayCommand<object>((X) => OpenDriveTrainCalc(X)));
            }
        }
        

        private void AddProject(object Args)
        {

            RaisePropertyChanged(nameof(Args.ToString));
            UpdateAllProperties();
        }
        private void SaveProject(object Args)
        {           

            if (_CurrentProject.FileName == "")
            {
                _CurrentProject.FileName = StaticMethods.ProjectFolder + "ExampleProject.xml";
            }
            _CurrentProject.SaveProject();
            _CurrentProject.BOM.SaveBOM();
            RaisePropertyChanged(nameof(Args.ToString));
            UpdateAllProperties();
        }
        private void SaveProjectAs(object Args)
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
                RaisePropertyChanged(nameof(Args.ToString));
                UpdateAllProperties();
            }
        }
        private void OpenProject(object Args)
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
        private void SetMake(object Args)
        {
           // Make tmpMake = ProjMakeList.ReturnMakeByID(MakeID);
            //if (tmpMake != null)
            //{
            //    _CurrentProject.Make = tmpMake.Name;
            //    UpdateAllProperties();
            //}
        }
        private void OpenMakeModelRepo(object Args)
        {
            MakeModelRepo makeModelRepo = new MakeModelRepo();
            makeModelRepo.Show();
        }
        private void OpenMakeEngineRepo(object Args)
        {
            MakeEngineRepo makeEngineRepo = new MakeEngineRepo();
            makeEngineRepo.Show();
        }
        private void OpenVenders(object Args)
        {
            VendersView vendersView = new VendersView();
            vendersView.Show();
        }
        private void OpenPartCatagories(object Args)
        {
            PartCatagoriesView partCatagories = new PartCatagoriesView();
            partCatagories.Show();
        }
        private void OpenInjectorCalc(object Args)
        {
            InjectorCalcView injCalc = new InjectorCalcView();
            injCalc.Show();
        }
        private void OpenHPETCalc(object Args)
        {
            HPandETcalculatorView hpetCalc = new HPandETcalculatorView();
            hpetCalc.Show();
        }
        private void AddBOMitem(object Args)
        {
            if (_CurrentProject.SelectedComponent == null) return;
            DAL.Component nc = new DAL.Component();
            nc.Name = _CurrentProject.SelectedComponent.Name + " new item";
            nc.ParentID = _CurrentProject.SelectedComponent.ID;
            nc.CategoryID = _CurrentProject.SelectedComponent.CategoryID;
            nc.SubCategoryID = _CurrentProject.SelectedComponent.SubCategoryID;
            _CurrentProject.SelectedComponent.AddComponent(nc);
            RaisePropertyChanged(nameof(CurrentProject.BOM.Vehicle.Components));
        }
        private void DeleteBOMitem(object Args)
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
                RaisePropertyChanged(nameof(CurrentProject.BOM.Vehicle.Components));
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
        private void AddPart(object Args)
        { 
            SelectPartView spv = new SelectPartView();
            spv.ShowDialog();
            _CurrentProject.SelectedComponent.AddPart(spv.SelectedPart);
            spv.Close();
            RaisePropertyChanged(nameof(CurrentProject.BOM.Vehicle.Components));
        }
        private void EnablePartSel(object Args)
        {
            if (_CurrentProject.SelectedComponent == null) return;
            _CurrentProject.SelectedComponent.EnableSelPrt = !_CurrentProject.SelectedComponent.EnableSelPrt;
        }
        private void OpenPartURL(object Args)
        {
            if (_CurrentProject.SelectedComponent == null) return;
            if (_CurrentProject.SelectedComponent.SelectedPart == null) return;
            if (_CurrentProject.SelectedComponent.SelectedPart.URL == "") return;
            Process.Start(new ProcessStartInfo { FileName = _CurrentProject.SelectedComponent.SelectedPart.URL, UseShellExecute = true });
        }
        private void OpenDriveTrainCalc(object Args)
        {
            DriveTrainView driveTrainView = new DriveTrainView();
            driveTrainView.Show();
            DriveTrainVM DTVM = driveTrainView.DataContext as DriveTrainVM;
            DTVM.DriveTrainData = CurrentProject.DriveTrain;
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
            RaisePropertyChanged(nameof(calcToolInjAF));
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
            RaisePropertyChanged(nameof(calcInjectorSize));
        }
        private void CalculateET()
        {
            _calcPerf = true;

            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc14mph =(float) (234.0 * Math.Pow(((float)_calcWHP /_calcVehicalWght),((float)1 / 3)));
            _calc18mph = (float)(147.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            RaisePropertyChanged(nameof(calc14et));
            RaisePropertyChanged(nameof(calc18et));
            RaisePropertyChanged(nameof(calc14mph));
            RaisePropertyChanged(nameof(calc14mphW));
            RaisePropertyChanged(nameof(calc18mph));
            RaisePropertyChanged(nameof(calc18mphW));
            _calcPerf = false;
        }
        private void CalculateRWHP()
        {
            _calcPerf = true;

            _calcWHP = (int) Math.Round(((float)_calcVehicalWght / Math.Pow((_calc14et / 5.825), 3)),0);
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc14mph = (float)(234.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));
            _calc18mph = (float)(147.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            RaisePropertyChanged(nameof(calcWHP));
            RaisePropertyChanged(nameof(calc18et));
            RaisePropertyChanged(nameof(calc14mph));
            RaisePropertyChanged(nameof(calc14mphW));
            RaisePropertyChanged(nameof(calc18mph));
            RaisePropertyChanged(nameof(calc18mphW));
            _calcPerf = false;
        }
        private void CalculateFeRWHP()
        {
            _calcPerf = true;

            _calcWHP = (int)Math.Round(((float)_calcVehicalWght / Math.Pow((_calc18et / 3.58), 3)), 0);
            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc14mph = (float)(234.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));
            _calc18mph = (float)(147.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            RaisePropertyChanged(nameof(calcWHP));
            RaisePropertyChanged(nameof(calc14et));
            RaisePropertyChanged(nameof(calc14mph));
            RaisePropertyChanged(nameof(calc14mphW));
            RaisePropertyChanged(nameof(calc18mph));
            RaisePropertyChanged(nameof(calc18mphW));
            _calcPerf = false;
        }
        private void CalculateTSRWHP()
        {
            _calcPerf = true;

            _calcWHP = (int) Math.Round((float) _calcVehicalWght * Math.Pow((_calc14mph/234), 3),0);
            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18mph = (float)(147.0 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            RaisePropertyChanged(nameof(calcWHP));
            RaisePropertyChanged(nameof(calc14et));
            RaisePropertyChanged(nameof(calc18et));
            RaisePropertyChanged(nameof(calc14mph));
            RaisePropertyChanged(nameof(calc14mphW));
            RaisePropertyChanged(nameof(calc18mph));
            RaisePropertyChanged(nameof(calc18mphW));
            _calcPerf = false;
        }
        private void CalculateTSERWHP()
        {
            _calcPerf = true;

            _calcWHP = (int)Math.Round((float)_calcVehicalWght * Math.Pow((_calc18mph / 147), 3), 0);
            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc14mph = (float)(234 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            RaisePropertyChanged(nameof(calcWHP));
            RaisePropertyChanged(nameof(calc14et));
            RaisePropertyChanged(nameof(calc18et));
            RaisePropertyChanged(nameof(calc14mph));
            RaisePropertyChanged(nameof(calc14mphW));
            RaisePropertyChanged(nameof(calc18mph));
            RaisePropertyChanged(nameof(calc18mphW));

            _calcPerf = false;
        }
        private void CalculateVWe()
        {
            _calcPerf = true;

            _calcVehicalWght = (int) Math.Round((float) _calcWHP / Math.Pow((_calc18mph / 147), 3),0);
            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc14mph = (float)(234 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            RaisePropertyChanged(nameof(calcVehicalWght));
            RaisePropertyChanged(nameof(calc14et));
            RaisePropertyChanged(nameof(calc18et));
            RaisePropertyChanged(nameof(calc14mph));
            RaisePropertyChanged(nameof(calc14mphW));
            RaisePropertyChanged(nameof(calc18mph));

            _calcPerf = false;
        }
        private void CalculateVW()
        {
            _calcPerf = true;

            _calcVehicalWght = (int)Math.Round((float)_calcWHP / Math.Pow((_calc14mph / 234), 3), 0);
            _calc14et = (float)(5.825 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18et = (float)(3.58 * Math.Pow(((float)_calcVehicalWght / _calcWHP), ((float)1 / 3)));
            _calc18mph = (float)(147 * Math.Pow(((float)_calcWHP / _calcVehicalWght), ((float)1 / 3)));

            RaisePropertyChanged(nameof(calcVehicalWght));
            RaisePropertyChanged(nameof(calc14et));
            RaisePropertyChanged(nameof(calc18et));
            RaisePropertyChanged(nameof(calc18mph));
            RaisePropertyChanged(nameof(calc18mphW));
            RaisePropertyChanged(nameof(calc14mph));
            RaisePropertyChanged(nameof(calc14mphW));
            RaisePropertyChanged(nameof(calc18mph));

            _calcPerf = false;
        }

        #endregion

        #region PropertyEvents
        private void UpdateAllProperties()
        {
            RaisePropertyChanged(nameof(CurrentProject));
            RaisePropertyChanged(nameof(ProjModelList));
            RaisePropertyChanged(nameof(ProjEngineModelList));
            RaisePropertyChanged(nameof(PartCatagories));
            RaisePropertyChanged(nameof(PartSubCatagories));
        }
        #endregion
    }
}
