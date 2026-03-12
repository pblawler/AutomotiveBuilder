using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using AutomotiveBuilder.DAL;
using AutomotiveBuilder.Statics;
using MVVMtools;
using MVVMtools.Command;

namespace AutomotiveBuilder.ViewModels
{
    internal class MakeModelViewModel : ViewModelBase
    {
        #region Constructor
        public MakeModelViewModel()
        {
            makes = StaticMethods.LoadMakes();
            RaisePropertyChanged(nameof(Makes));
            SelectedMake = Makes[0];
        }
        #endregion

        #region Private Members
        private Make _selectedmake;
        private Model _selectedmodel;
        private Engine _selectedEngine;
        private MakesList makes;
        private BindingList<Model> models;
        private BindingList<Engine> engines;
        private string _FileName = Statics.StaticMethods.MakeModelFolder + "Makes.xml";
        #endregion

        #region Properties
        public Make SelectedMake {  get { return _selectedmake; } set { _selectedmake = value; } } 
        public Model SelectedModel { get { return _selectedmodel; } set { _selectedmodel = value; } }
        public Engine SelectedEngine { get { return _selectedEngine; } set { _selectedEngine = value; RaisePropertyChanged(nameof(SelectedEngine)); } }
        public MakesList Makes { get { return makes; } set { makes = value; } }
        public BindingList<Model> Models { get { return models; } set { models = value; } }
        public BindingList<Engine> Engines { get { return engines; } set { engines = value; } }
        public string FileName { get { return _FileName; } set { _FileName = value; } }
        #endregion

        #region Relay Commands
        private RelayCommand<object> _SaveMakes;
        private RelayCommand<object> _SelectMake;
        private RelayCommand<object> _SelectModel;
        private RelayCommand<object> _SortMakesName;
        private RelayCommand<object> _SortMakesCountry;
        private RelayCommand<object> _AddModel;
        private RelayCommand<object> _AddEngine;
        private RelayCommand<object> _BrowseModelURL;
        private RelayCommand<object> _BrowseEngineURL;
        private RelayCommand<object> _BrowseMakeURL;
        private RelayCommand<object> _OpenMakesFile;
        private RelayCommand<object> _SaveMakesAs;

        public RelayCommand<object> SaveMakesCommand
        {
            get
            {
                return _SaveMakes ?? (_SaveMakes = new RelayCommand<object>((X) => SaveMakes(X)));
            }
        }
        public RelayCommand<object> SaveMakesAsCommand
        {
            get
            {
                return _SaveMakesAs ?? (_SaveMakesAs = new RelayCommand<object>((X) => SaveMakesAs(X)));
            }
        }
        public RelayCommand<object> SelectMakeCommand
        {
            get
            {
                return _SelectMake ?? (_SelectMake = new RelayCommand<object>((X) => SelectMake(X)));
            }
        }
        public RelayCommand<object> SelectModelCommand
        {
            get
            {
                return _SelectModel ?? (_SelectModel = new RelayCommand<object>((X) => SelectModel(X)));
            }
        }
        public RelayCommand<object> SortMakesNameCommand
        {
            get
            {
                return _SortMakesName ?? (_SortMakesName = new RelayCommand<object>((X) => SortMakesName(X)));
            }
        }
        public RelayCommand<object> SortMakesCountryCommand
        {
            get
            {
                return _SortMakesCountry ?? (_SortMakesCountry = new RelayCommand<object>((X) => SortMakesCountry(X)));
            }
        }
        public RelayCommand<object> AddModelCommand
        {
            get
            {
                return _AddModel ?? (_AddModel = new RelayCommand<object>((X) => AddModel(X)));
            }
        }
        public RelayCommand<object> AddEngineCommand
        {
            get
            {
                return _AddEngine ?? (_AddEngine = new RelayCommand<object>((X) => AddEngine(X)));
            }
        }
        public RelayCommand<object> BrowseModelURLCommand
        {
            get
            {
                return _BrowseModelURL ?? (_BrowseModelURL = new RelayCommand<object>((X) => BrowseModelURL(X)));
            }
        }
        public RelayCommand<object> BrowseEngineURLCommand
        {
            get
            {
                return _BrowseEngineURL ?? (_BrowseEngineURL = new RelayCommand<object>((X) => BrowseEngineURL(X)));
            }
        }
        public RelayCommand<object> BrowseMakeURLCommand
        {
            get
            {
                return _BrowseMakeURL ?? (_BrowseMakeURL = new RelayCommand<object>((X) => BrowseMakeURL(X)));
            }
        }
        public RelayCommand<object> OpenMakesFileCommand
        {
            get
            {
                return _OpenMakesFile ?? (_OpenMakesFile = new RelayCommand<object>((X) => OpenMakesFile(X)));
            }
        }
        

        private void SaveMakes(object Args)
        {
            Makes.SaveMakeList(FileName);
        }
        private void SaveMakesAs(object Args)
        {
            FileDialog SaveFileDialog = new SaveFileDialog();
            DialogResult dr;

            SaveFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            SaveFileDialog.FilterIndex = 1;
            SaveFileDialog.RestoreDirectory = true;
            dr = SaveFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                FileName = SaveFileDialog.FileName;
            }
            else
            {
                return;
            }
            if (FileName != "")
            {
                Makes.SaveMakeList(FileName);
                RaisePropertyChanged(nameof(FileName));
                RaisePropertyChanged(nameof(Makes));
            }            
        }
        private void SelectMake(object Args)
        {
            try
            {
                SelectedModel = SelectedMake.Models[0];
            }
            catch
            {
                SelectedModel = null;
            }
            RaisePropertyChanged(nameof(SelectedMake));
            RaisePropertyChanged(nameof(SelectedModel));
        }
        private void SelectModel(object Args)
        {            
            RaisePropertyChanged(nameof(SelectedModel));
        }
        private void SortMakesName(object Args)
        {

            Makes.Sort();
            RaisePropertyChanged(nameof(Makes));
        }
        private void SortMakesCountry(object Args)
        {
            MakeCountryComparer makeCountryComparer = new MakeCountryComparer();    

            Makes.SortMake(makeCountryComparer);
            RaisePropertyChanged(nameof(Makes));
        }
        private void AddModel(object Args)
        {
            SelectedModel = new Model();
            SelectedModel.Name = "New Model";
            SelectedMake.Models.Add(SelectedModel);
            RaisePropertyChanged(nameof(Makes));
        }
        private void AddEngine(object Args)
        {
            SelectedEngine = new Engine();
            SelectedEngine.Name = "New Engine" + " " + SelectedMake.Engines.Count.ToString();
            SelectedMake.Engines.Add(SelectedEngine);
            RaisePropertyChanged(nameof(Engines));
            RaisePropertyChanged(nameof(SelectedEngine));
        }
        private void BrowseModelURL(object Args)
        {
            try
            {
                StaticMethods.OpenExternalFile(SelectedModel.URL);
            }
            catch
            {
                //Nothing to do
            }
        }
        private void BrowseEngineURL(object Args)
        {
            try
            {
                StaticMethods.OpenExternalFile(SelectedEngine.URL);
            }
            catch
            {
                //Nothing to do
            }
        }
        private void BrowseMakeURL(object Args)
        {
            try
            {
                StaticMethods.OpenExternalFile(SelectedMake.Url);
            }
            catch
            {
                //Nothing to do
            }
        }
        private void OpenMakesFile(object Args)
        {
            try
            {

                if (System.Windows.MessageBox.Show("This will remove the currently loaded data.  Do you wish to proceed?", "Open File", MessageBoxButton.YesNo, MessageBoxImage.Information) != MessageBoxResult.Yes)
                {
                    return;
                }
                makes = new MakesList();
                RaisePropertyChanged(nameof(Makes));
                FileDialog OpenFileDialog = new OpenFileDialog();
                OpenFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                OpenFileDialog.FilterIndex = 1;
                DialogResult dr;
                dr = OpenFileDialog.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    FileName = OpenFileDialog.FileName;
                }
                else
                {
                    makes = StaticMethods.LoadMakes();
                    RaisePropertyChanged(nameof(Makes));
                    SelectedMake = Makes[0];
                    return;
                }
                makes = StaticMethods.LoadMakesFile(FileName);
                RaisePropertyChanged(nameof(Makes));
            }
            catch
            {
                //Nothing to do
            }
        }
        #endregion
       
    }
}
