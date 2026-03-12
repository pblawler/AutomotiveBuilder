using AutomotiveBuilder.DAL;
using AutomotiveBuilder.Properties;
using AutomotiveBuilder.Statics;
using AutomotiveBuilder.Views;
using MVVMtools;
using MVVMtools.Command;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace AutomotiveBuilder.PartUtils
{
    class PartSelectVM : ViewModelBase
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the VenderViewModel class.
        /// </summary>
        public PartSelectVM()
        {
            RaisePropertyChanged(nameof(MyVenderList));
            RaisePropertyChanged(nameof(SelectedVender));
            LoadParts();
        }

        #endregion

        #region Private Members
        private Vender _selectedVender;
        private string _FileName = StaticMethods.PartsFolder + "Venders.xml";
        private PartCatagory _selectedPartCatagory;
        private PartSubCatagory _selectedPartSubCatagory;
        private Part _selectedPart;
        private string _catagoryID = "";
        private string _subCatagoryID = "";
        private PartList _tableParts;
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
                    _selectedVender.PropertyChanged += _selectedVender_PropertyChanged;
                    LoadParts();
                    RaisePropertyChanged(nameof(TableParts));
                    RaisePropertyChanged(nameof(SelectedVender));
                    RaisePropertyChanged(nameof(PartCatagories));
                    RaisePropertyChanged(nameof(PartSubCatagories));
                    RaisePropertyChanged(nameof(SelectedVender.SelectedPart));
                    if ((_selectedVender.Parts != null) && (_selectedVender.Parts.Count > 0)) _selectedVender.SelectedPart = _selectedVender.Parts[0];
                }
            }
        }
        private void _selectedVender_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            switch(e.PropertyName)
            {
                case "CategoryID":
                    RaisePropertyChanged(nameof(PartSubCatagories));
                    break;

            }

        }
        public VenderList MyVenderList
        {
            get
            {
                VenderList sortedList = new VenderList(false);
                foreach (var vender in StaticMethods.Venders.OrderBy(v => v.Name))
                {
                    sortedList.Add(vender);
                }

                return sortedList;
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
                    RaisePropertyChanged(nameof(FileName));
                }
            }
        }
        public PartCatagory SelectedPartCatagory
        {
            get { return _selectedPartCatagory; }
            set
            {
                _selectedPartCatagory = value;
                RaisePropertyChanged(nameof(SelectedPartCatagory));
                RaisePropertyChanged(nameof(PartSubCatagories));
            }
        }
        public BindingList<PartSubCatagory> PartSubCatagories
        {
            get
            {
                BindingList<PartSubCatagory> _PartSubCatagories = new BindingList<PartSubCatagory>();
                if ((StaticMethods.PartCatagories != null) && (CatagoryID != null))
                {
                    _selectedPartCatagory = StaticMethods.PartCatagories.Where(x => x.ID == CatagoryID).FirstOrDefault();
                    if((SelectedPartCatagory != null) && (SelectedPartCatagory.SubCategories != null)) _PartSubCatagories = SelectedPartCatagory.SubCategories;
                    return _PartSubCatagories;
                }
                return _PartSubCatagories;
            }
        }
        public PartSubCatagory SelectedPartSubCatagory
        {
            get { return _selectedPartSubCatagory; }
            set
            {
                _selectedPartSubCatagory = value;
                RaisePropertyChanged(nameof(SelectedPartSubCatagory));
            }
        }
        public PartList TableParts
        { 
            get { return _tableParts; }           
        }
        public Part SelectedPart
        {
            get { return _selectedPart; }
            set
            {
                _selectedPart = value;
                RaisePropertyChanged(nameof(SelectedPart));
            }
        }
        public string CatagoryID
        {
            get { return _catagoryID; }
            set
            {
                _catagoryID = value;
                LoadParts();
                RaisePropertyChanged(nameof(PartSubCatagories));
                RaisePropertyChanged(nameof(CatagoryID));
            }
        }
        public string SubCatagoryID
        {
            get { return _subCatagoryID; }
            set
            {
                _subCatagoryID = value;
                LoadParts();
                RaisePropertyChanged(nameof(SubCatagoryID));
            }
        }
        public BindingList<PartCatagory> PartCatagories
        {
            get
            {
                BindingList<PartCatagory> _PartCatagories = new BindingList<PartCatagory>();
                if (StaticMethods.PartCatagories != null)
                {
                    foreach (var catagory in StaticMethods.PartCatagories.OrderBy(x => x.Name))
                    {
                        _PartCatagories.Add(catagory);
                    }
                }
                return _PartCatagories;
            }
        }
        #endregion

        private void LoadParts()
        {
            _tableParts = new PartList();
            //_tableParts.Clear();
            if (_selectedVender != null)
            {
                if ((CatagoryID != null) && (CatagoryID != "") && (SubCatagoryID != null) && (SubCatagoryID != ""))
                {
                    foreach (var part in StaticMethods.ProjPartList.Where(x => x.VenderID == _selectedVender.ID && x.CategoryID == CatagoryID && x.SubCategoryID == SubCatagoryID).OrderBy(x => x.Name))
                    {
                        _tableParts.Add(part);
                    }
                }
                else if ((CatagoryID != null) && (CatagoryID != "") && ((SubCatagoryID == null) || (SubCatagoryID == "")))
                {
                    foreach (var part in StaticMethods.ProjPartList.Where(x => x.VenderID == _selectedVender.ID && x.CategoryID == CatagoryID).OrderBy(x => x.Name))
                    {
                        _tableParts.Add(part);
                    }
                }
                else
                {
                    foreach (var part in StaticMethods.ProjPartList.Where(x => x.VenderID == _selectedVender.ID).OrderBy(x => x.Name))
                    {
                        _tableParts.Add(part);
                    }
                }
            }
            else
            {
                if ((CatagoryID != null) && (CatagoryID != "") && (SubCatagoryID != null) && (SubCatagoryID != ""))
                {
                    foreach (var part in StaticMethods.ProjPartList.Where(x => x.CategoryID == CatagoryID && x.SubCategoryID == SubCatagoryID).OrderBy(x => x.Name))
                    {
                        _tableParts.Add(part);
                    }
                }
                else if ((CatagoryID != null) && (CatagoryID != "") && ((SubCatagoryID == null) || (SubCatagoryID == "")))
                {
                    foreach (var part in StaticMethods.ProjPartList.Where(x => x.CategoryID == CatagoryID).OrderBy(x => x.Name))
                    {
                        _tableParts.Add(part);
                    }
                }
                else
                {
                    foreach (var part in StaticMethods.ProjPartList.OrderBy(x => x.Name))
                    {
                        _tableParts.Add(part);
                    }
                }
            }
            RaisePropertyChanged(nameof(TableParts));
        }

    }
}
