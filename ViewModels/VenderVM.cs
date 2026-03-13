using AutomotiveBuilder.DAL;
using AutomotiveBuilder.Properties;
using AutomotiveBuilder.Statics;
using AutomotiveBuilder.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace AutomotiveBuilder
{
    public partial class VenderVM : ObservableObject
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the VenderViewModel class.
        /// </summary>
        public VenderVM()
        {
            _venderList = new VenderList();
            if (_venderList.Count > 0) SelectedVender = _venderList[0];
            OnPropertyChanged(nameof(MyVenderList));
            OnPropertyChanged(nameof(SelectedVender));
        }

        #endregion

        #region Private Members
        private VenderList _venderList;
        private Vender _selectedVender;
        private string _FileName = Statics.StaticMethods.PartsFolder + "Venders.xml";
        private PartCatagory _selectedPartCatagory;
        #endregion

        #region Properties
        public Vender SelectedVender
        {
            get { return _selectedVender; }
            set 
            {
                if (_selectedVender != value)
                {
                    if(_selectedVender != null)
                    {
                        _selectedVender.PropertyChanged -= _selectedVender_PropertyChanged;
                    }                    
                    _selectedVender = value;
                    if ((_selectedVender == null) & (MyVenderList.Count > 0))
                    {
                        _selectedVender = MyVenderList[0];
                    }
                    else if ((_selectedVender == null) & (MyVenderList.Count == 0))
                    {
                        return;
                    }
                    _selectedVender.PropertyChanged += _selectedVender_PropertyChanged;
                    OnPropertyChanged(nameof(SelectedVender));
                    OnPropertyChanged(nameof(PartCatagories));
                    OnPropertyChanged(nameof(PartSubCatagories));
                    OnPropertyChanged(nameof(SelectedVender.SelectedPart));
                    if ((_selectedVender.Parts != null) && (_selectedVender.Parts.Count > 0)) _selectedVender.SelectedPart = _selectedVender.Parts[0];
                }
            }
        }

        private void _selectedVender_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            switch(e.PropertyName)
            {
                case "CategoryID":
                    OnPropertyChanged(nameof(PartSubCatagories));
                    break;

            }

        }

        public VenderList MyVenderList
        {
            get
            {
                return _venderList;
            }
            set
            {
                _venderList = value;
                OnPropertyChanged(nameof(MyVenderList));
            }
        }
        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }
        public PartCatagory SelectedPartCatagory
        {
            get { return _selectedPartCatagory; }
            set
            {
                _selectedPartCatagory = value;
                OnPropertyChanged(nameof(SelectedPartCatagory));
                OnPropertyChanged(nameof(PartSubCatagories));
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
                if ((SelectedVender != null) && (SelectedVender.SelectedPart !=null) && (SelectedVender.SelectedPart.CategoryID !=""))
                {
                    foreach(var cat in StaticMethods.PartCatagories)
                    {
                        if(cat.ID == SelectedVender.SelectedPart.CategoryID)
                        {
                            _PartSubCatagories = cat.SubCategories;
                            return _PartSubCatagories;
                        }
                    }
                }
                return _PartSubCatagories;
            }
        }


        #endregion

        #region Relay Commands
        private RelayCommand<object> _AddVender;
        private RelayCommand<object> _SaveVenders;
        private RelayCommand<object> _AddCatagory;
        private RelayCommand<object> _AddSubCatagory;
        private RelayCommand<object> _SaveCatagories;
        private RelayCommand<object> _AddVenderPart;

        public RelayCommand<object> AddVenderCommand
        {
            get
            {
                return _AddVender ?? (_AddVender = new RelayCommand<object>((X) => AddVender(X)));
            }
        }
        public RelayCommand<object> SaveVendersCommand
        {
            get
            {
                return _SaveVenders ?? (_SaveVenders = new RelayCommand<object>((X) => SaveVenders(X)));
            }
        }
        public RelayCommand<object> AddCatagoryCommand
        {
            get
            {
                return _AddCatagory ?? (_AddCatagory = new RelayCommand<object>((X) => AddCatagory(X)));
            }
        }
        public RelayCommand<object> AddSubCatagoryCommand
        {
            get
            {
                return _AddSubCatagory ?? (_AddCatagory = new RelayCommand<object>((X) => AddSubCatagory(X)));
            }
        }        
        public RelayCommand<object> SaveCatagoriesCommand
        {
            get
            {
                return _SaveCatagories ?? (_SaveCatagories = new RelayCommand<object>((X) => SaveCatagories(X)));
            }
        }
        public RelayCommand<object> AddVenderPartCommand
        {
            get
            {
                return _AddVenderPart ?? (_AddVenderPart = new RelayCommand<object>((X) => AddVenderPart(X)));
            }
        }


        private void AddVender(object Args)
        {
            Vender tmpVender = new Vender();
            tmpVender.ID = System.Guid.NewGuid().ToString();
            tmpVender.Name = "New Vender" + " " + MyVenderList.Count.ToString();
            MyVenderList.Add(tmpVender);    
            SelectedVender = tmpVender;
            UpdateAllProperties();
        }
        private void SaveVenders(object Args)
        {
            MyVenderList.SaveVenderList(FileName);
            StaticMethods.VendersChanged = true;
        }
        private void AddCatagory(object Args)
        {
            PartCatagory np = new PartCatagory();
            np.Name = "New Catagory " +  StaticMethods.PartCatagories.Count.ToString();
            np.Description = "New Part Catagory";
            _selectedPartCatagory = np;
            StaticMethods.PartCatagories.Add(np);
            OnPropertyChanged(nameof(PartCatagories));
            OnPropertyChanged(nameof(SelectedPartCatagory));
        }
        private void AddSubCatagory(object Args)
        {
            if(SelectedPartCatagory == null) return;
            PartSubCatagory np = new PartSubCatagory();
            np.ID = System.Guid.NewGuid().ToString();
            np.CategoryID = SelectedPartCatagory.ID;
            np.Name = "New SubCatagory " + SelectedPartCatagory.SubCategories.Count.ToString();
            np.Description = "New Part SubCatagory";            
            SelectedPartCatagory.SubCategories.Add(np);
            SelectedPartCatagory.SelectedSubcatagory = np;
            OnPropertyChanged(nameof(SelectedPartCatagory.SubCategories));
            OnPropertyChanged(nameof(SelectedPartCatagory.SelectedSubcatagory));
        }
        private void SaveCatagories(object Args)
        {
            StaticMethods.SavePartCatagories();
            OnPropertyChanged(nameof(PartCatagories));
        }
        private void AddVenderPart(object Args)
        {
            if(SelectedVender == null) return;
            Part tmpVPart = new Part();
            tmpVPart.Name = "New Part" + " " + SelectedVender.Parts.Count.ToString();
            tmpVPart.VenderID = SelectedVender.ID;
            SelectedVender.Parts.Add(tmpVPart);
            SelectedVender.SelectedPart = tmpVPart;
            UpdateAllProperties();
        }
        #endregion

        #region PropertyEvents
        private void UpdateAllProperties()
        {
            OnPropertyChanged(nameof(MyVenderList));
            OnPropertyChanged(nameof(SelectedVender));
            OnPropertyChanged(nameof(SelectedVender.Parts));
        }
        #endregion
    }
}
