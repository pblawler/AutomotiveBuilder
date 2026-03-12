using AutomotiveBuilder.Static;
using AutomotiveBuilder.Statics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static AutomotiveBuilder.Static.MessengerService;

namespace AutomotiveBuilder.DAL
{
    public class AutoBuilder : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region MessengerService Subscription
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

                case MessageType.DataChanged:
                    /// Trigger the vehicle properties to update their values based on the new data.  
                    /// This will cascade down through the components and parts to update all relevant cost and inventory data.
                    /// 
                    // Handle data changed messages
                    CalculateCosts();
                    break;
            }
        }
        private void SendMessage(string text, MessageType type = MessageType.Info)
        {
            MessengerService.LogMessage(text, type);
        }
        #endregion


        private string _projectID = "";
        private Component? _vehicle;
        /// will be a hiearchy of components from vehicle down to nuts and bolts
        /// roll up cost for total, buy breakdown of components, etc...
        /// track compoonents and quanties required, used, outstanding, etc....
        /// 

        [XmlIgnore]
        public Component? Vehicle
        {
            get { return _vehicle; }
            set
            {
                _vehicle = value;
                OnPropertyChanged(nameof(Vehicle));
            }
        }

        public AutoBuilder()
        {
            InitializeBOM();
            StaticMethods.PartsChanged = true;
            CalculateCosts();
        }

        public AutoBuilder(string ProjectID)
        {
            _projectID = ProjectID;
            string filename = StaticMethods.BOMfolder + ProjectID + ".xml";
            if (File.Exists(filename))
            {
                var serializer = new XmlSerializer(typeof(Component));
                using (var reader = new StreamReader(filename))
                {
                    Component? bom = (Component?)serializer.Deserialize(reader);
                    _vehicle = bom;
                    _vehicle.LinkProperties();
                    _vehicle.LoadPartOptions();
                }
            }
            if ((Vehicle == null) || (Vehicle.Components == null) || (Vehicle.Components.Count == 0)) InitializeBOM();
            _vehicle.PropertyChanged += (s, e) => { OnPropertyChanged(e?.PropertyName); };
            StaticMethods.PartsChanged = true;
            CalculateCosts();
            OnPropertyChanged(nameof(Vehicle));
        }

        private void InitializeBOM()
        {
            if ((Vehicle == null) || (Vehicle.Components == null) || (Vehicle.Components.Count == 0))
            {
                if (Vehicle == null) _vehicle = new Component();
                _vehicle!.Components = new ObservableCollection<Component>();
                Component nc = new Component();
                nc.Name = "Vehicle";
                nc.Components = new ObservableCollection<Component>();
                nc.CategoryID = "e73887b2-e0d9-42b7-9e06-9862a1efc020";
                Component cc = new Component();
                cc.Name = "Engine";
                cc.CategoryID = "b63dca37-b1e1-4380-98c0-94945ab95574";
                cc.Components = new ObservableCollection<Component>();
                nc.AddComponent(cc);
                cc = new Component();
                cc.Name = "Transmission";
                cc.CategoryID = "8fb83946-fa5e-4223-add5-921197aa05e7";
                cc.Components = new ObservableCollection<Component>();
                nc.AddComponent(cc);
                _vehicle.AddComponent(nc);

            }
            CalculateCosts();
        }

        public void SaveBOM()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                string filename = StaticMethods.BOMfolder + _projectID + ".xml";
                if (File.Exists(filename)) File.Delete(filename);
                var serializer = new XmlSerializer(typeof(Component));
                using (var writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, _vehicle);
                }
            }
            catch
            {

            }
            Cursor.Current = Cursors.Default;
        }

        public void CalculateCosts()
        {
            if (Vehicle != null)
            {
                var remCost = Vehicle.Components[0].AssemblyRemainingCost;
                var Cost = Vehicle.Components[0].Cost;
                var TotCost = Vehicle.Components[0].TotalCost;
                var subAssmRemCost = Vehicle.Components[0].SubAssemblyRemainingCost;
                var assmCost = Vehicle.Components[0].AssemblyCost;
                var tmp1 = Vehicle.Components[0].RemainingCost;
                var tmp2 = Vehicle.Components[0].TotalCost;
                var tmp3 = Vehicle.Components[0].RemainingPartsRequired;
                var tmp4 = Vehicle.Components[0].PartsOnHand;
                StaticMethods.PartsChanged = false;
                OnPropertyChanged(nameof(Vehicle));
            }

        }
    }

    public class Component : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region MessengerService Subscription
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

                case MessageType.DataChanged:
                    // Handle data changed messages
                   
                    break;
            }
        }
        private void SendMessage(string text, MessageType type = MessageType.Info)
        {
            MessengerService.LogMessage(text, type);
        }
        #endregion


        /// recursive hiearchy of component parts
        /// 

        #region Private Members
        private string _ID="";
        private string _parentID = "";
        private string _catagoryID="";
        private string _subCatagoryID="";
        private int _numRequired;
        private int _installedQuantity;
        private decimal _cost;
        private decimal _totalCost;
        private string _name = "new component";
        private string _description = "";
        private Part _selectedPart;
        private string _EnableCompPrtSelTxt;
        private bool _enableSelPrt = false;
        ///these may need to inhiert from parts but need to make sure not to redundantly save data
        ///  also need to have a removed property for these to adjust cost.  This will allow tracking of total
        ///  amount spent even on broken or replaced/removed parts.  This could grow unruley so need abitlity to purge
        ///  removed/replaced items.
        private List<string> _PartsUsedIDs = new List<string>();
        private List<string> _OptionalPartIDs = new List<string>();
        private BindingList<Part> _OptionalParts = new BindingList<Part>();

        private ObservableCollection<Component> _components = new ObservableCollection<Component>();
        private Component _selectedComponent;
        private ComponentPart _selectedComponentPart;
        private decimal _assmCost;
        private decimal _assmRemCost;
        private decimal _subassmRemCost;
        private BindingList<UsedPart> _UsedParts = new BindingList<UsedPart>();
        #endregion


        public string ID
        {
            get
            {
                if (_ID == "") _ID = Guid.NewGuid().ToString();
                return _ID;
            }
            set
            {
                if (_ID == value) return;
                _ID = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public string ParentID
        {
            get { return _parentID; }
            set
            {
                if (value != _parentID)
                {
                    _parentID = value;
                    OnPropertyChanged(nameof(ParentID));
                }
            }
        }
        public string CategoryID
        {
            get { return _catagoryID; }
            set
            {
                if (value != _catagoryID)
                {
                    _catagoryID = value;
                    OnPropertyChanged(nameof(CategoryID));
                }
            }
        }
        public string SubCategoryID
        {
            get { return _subCatagoryID; }
            set
            {
                if (value != _subCatagoryID)
                {
                    _subCatagoryID = value;
                    OnPropertyChanged(nameof(SubCategoryID));
                }
            }
        }
        public ObservableCollection<Component> Components
        {
            get { return _components; }
            set
            {
                _components = value;
                OnPropertyChanged(nameof(Components));
            }




        }
        public Component? SelectedComponent
        {
            get { return _selectedComponent; }
            set
            {
                _selectedComponent = value;
                OnPropertyChanged(nameof(SelectedComponent));
                OnPropertyChanged(nameof(ImgHEX));
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                if(_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public int NumRequired
        {
            get { return _numRequired; }
            set
            {
                if(_numRequired != value)
                {
                    _numRequired = value;
                    OnPropertyChanged(nameof(NumRequired));
                }
            }
        }
        public int InstalledQuantity
        {
            get { return _installedQuantity; }
            set
            {
                if(_installedQuantity != value)
                {
                    _installedQuantity = value;
                    OnPropertyChanged(nameof(InstalledQuantity));
                }
            }
        }
        public decimal Cost
        {
            get { return _cost; }
            set
            {
                if(_cost != value)
                {
                    _cost = value;
                    OnPropertyChanged(nameof(Cost));
                    OnPropertyChanged(nameof(TotalCost));
                }
            }
        }
        public int PartsUsedCount
        {
            get 
            {
                if (_PartsUsedIDs == null) return 0;
                return _PartsUsedIDs.Count; 
            }
        }        
        public List<string> OptionalPartIDs
        {
            get { return _OptionalPartIDs; }
            set
            {
                _OptionalPartIDs = value;
                OnPropertyChanged(nameof(OptionalPartIDs));
            }
        }
        public ComponentPart? SelectedComponentPart
        {
            get { return _selectedComponentPart; }
            set
            {
                _selectedComponentPart = value;
                OnPropertyChanged(nameof(SelectedComponentPart));
                OnPropertyChanged(nameof(ImgHEX));
            }
        }
        public BindingList<UsedPart> UsedParts
        {
            get { return _UsedParts; }
            set
            {
                if (_UsedParts != value)
                {
                    _UsedParts = value;
                    OnPropertyChanged(nameof(UsedParts));
                }
            }
        }

        [XmlIgnore]
        public decimal RemainingCost
        {
            get { return RemainingPartsRequired * Cost; }
        }
        [XmlIgnore]
        public decimal AssemblyRemainingCost
        {
            get
            {
                if (StaticMethods.PartsChanged)
                {
                    _assmRemCost = RemainingCost;
                    foreach (var comp in Components)
                    {
                        _assmRemCost += comp.AssemblyRemainingCost;
                    }
                }
                return _assmRemCost;
            }
        }
        [XmlIgnore]
        public int PartsOnHand
        {
            get
            {
                int onHand = 0;
                if (SelectedComponentPart != null)
                {
                    if (StaticMethods.PartsChanged)
                    {
                        onHand = StaticMethods.GetPartInventory(SelectedComponentPart.PartID);
                    }
                }
                return onHand;
            }
        }
        [XmlIgnore]
        public int RemainingPartsRequired
        {
            get
            {
                int rpr = (NumRequired - InstalledQuantity - PartsOnHand);
                if (rpr < 0) rpr = 0;
                return rpr; }
        }
        [XmlIgnore]
        public string EnableCompPrtSelTxt
        {
            get 
            {
                if(_enableSelPrt)
                {
                    _EnableCompPrtSelTxt = "Part Selection Enabled";
                }
                else
                {
                    _EnableCompPrtSelTxt = "Enable Part Selection";
                }
                return _EnableCompPrtSelTxt;
            }           
        }
        [XmlIgnore]
        public bool EnableSelPrt
        {
            get { return _enableSelPrt; }
            set
            {
                if(_enableSelPrt != value)
                {
                    _enableSelPrt = value;
                    OnPropertyChanged(nameof(EnableSelPrt));
                    OnPropertyChanged(nameof(EnableCompPrtSelTxt));
                }
            }
        }
        [XmlIgnore]
        public Part SelectedPart
        {
            get { return _selectedPart; }
            set
            {                               
                _selectedPart = value;
                OnPropertyChanged(nameof(SelectedPart));
                if (!EnableSelPrt) return;
                if (_selectedComponentPart == null) _selectedComponentPart = new ComponentPart();
                _selectedComponentPart.CopyPart(_selectedPart);                               
                _enableSelPrt = false;
                OnPropertyChanged(nameof(EnableSelPrt));
                OnPropertyChanged(nameof(EnableCompPrtSelTxt));
                _cost = SelectedComponentPart.Cost;
                OnPropertyChanged(nameof(SelectedComponentPart));
                OnPropertyChanged(nameof(Cost));
            }
        }
        [XmlIgnore]
        public string ImgHEX
        {
            get
            {
                string rtn = "";
                if (SelectedComponentPart != null)  rtn = StaticMethods.GetPartImgHex(SelectedComponentPart.PartID);
                return rtn;
            }
        }
        [XmlIgnore]
        public BindingList<Part> OptionalParts
        {
            get { return _OptionalParts; }
            set
            {
                _OptionalParts = value;
                OnPropertyChanged(nameof(OptionalParts));
            }
        }
        [XmlIgnore]
        public string Vender
        {
            get
            {
                string rtn = "";
                if (SelectedComponentPart != null) rtn = StaticMethods.GetVenderName(SelectedComponentPart.VenderID);
                return rtn;
            }
        }
        [XmlIgnore]
        public decimal AssemblyCost
        {
            get
            {
                if (StaticMethods.PartsChanged)
                {
                    _assmCost = TotalCost;
                    foreach (var comp in Components)
                    {
                        _assmCost += comp.AssemblyCost;
                    }
                }
                return _assmCost;
            }
        }
        [XmlIgnore]
        public decimal TotalCost
        {
            get
            {
                _totalCost = NumRequired * Cost;
                return _totalCost;
            }
        }
        [XmlIgnore]
        public decimal SubAssemblyRemainingCost
        {
            get
            {
                if (StaticMethods.PartsChanged)
                {
                    _subassmRemCost = 0;
                    foreach (var comp in Components)
                    {
                        _subassmRemCost += comp.AssemblyRemainingCost;
                    }
                }
                return _subassmRemCost;
            }
        }


        public void LinkProperties()
        {
            foreach(var comp in Components)
            {
                comp.PropertyChanged += (s, e) => { OnPropertyChanged(e?.PropertyName); };
                comp.LinkProperties();
            }
        }
        public void LoadPartOptions()
        {
            Part part;

            part = null;

            if (_OptionalPartIDs != null)
            {
                foreach (var partID in _OptionalPartIDs)
                {
                    part = StaticMethods.ProjPartList.Where(p => p.ID == partID).FirstOrDefault();
                    if ((part != null) && (!_OptionalParts.Contains(part))) _OptionalParts.Add(part);
                }
                part = null;
            }
            foreach (var comp in Components)
            {
                comp.LoadPartOptions();               
            }

        }

        public void AddComponent(Component component)
        {
            component.PropertyChanged += (s, e) => { OnPropertyChanged(e?.PropertyName); };
            Components.Add(component);
        }
        public void AddPart(Part part)
        {
            if (part == null) return;
            if(_OptionalPartIDs == null) _OptionalPartIDs = new List<string>(); 
            if(_OptionalPartIDs.Contains(part.ID)) return;
            _OptionalPartIDs.Add(part.ID);
            OptionalParts.Add(part);
            OnPropertyChanged(nameof(OptionalParts));
            OnPropertyChanged(nameof(CategoryID));
        }
    }

    public class ComponentPart : Part
    {
        private int _quantityUsed = 0;
        private int _quantityReserved=0;
        private int _quantityRemoved=0;
        private string _partID = "";

        public string PartID
        {
            get { return _partID; }
            set
            {
                if (_partID != value)
                {
                    _partID = value;
                    OnPropertyChanged(nameof(PartID));
                }
            }
        }
        public int QuantityUsed
        {
            get { return _quantityUsed; }
            set
            {
                if (_quantityUsed != value)
                {
                    _quantityUsed = value;
                    OnPropertyChanged(nameof(QuantityUsed));
                }
            }
        }
        public int QuantityReserved
        {
            get { return _quantityReserved; }
            set
            {
                if (_quantityReserved != value)
                {
                    _quantityReserved = value;
                    OnPropertyChanged(nameof(QuantityReserved));
                }
            }
        }
        public int QuantityRemoved
        {
            get { return _quantityRemoved; }
            set
            {
                if (_quantityRemoved != value)
                {
                    _quantityRemoved = value;
                    OnPropertyChanged(nameof(QuantityRemoved));
                }
            }
        }
        public void CopyPart(Part part)
        {
            if (part == null) return;
            this.PartID = part.ID;
            this.Name = part.Name;
            this.Description = part.Description;
            this.Cost = part.Cost;
            this.CategoryID = part.CategoryID;
            this.SubCategoryID = part.SubCategoryID;
            this.VenderID = part.VenderID;
            this.URL = part.URL;
            this.PartNumber = part.PartNumber;
            this.Tax = part.Tax;
            this.Shipping = part.Shipping;
        }
    }


    ///TODO: on the venders view update the vender parts data grid when the selected parts are updated, the catagory info is not updating
    ///add a menu to the venders view to open the catagory view and refresh the catagorys when the catagory view closes.
    ///When adding a new part to a vender make the new part the selected part.
    ///
    /// TODO: on the components heiarchy add the ability to rearrange catagory relationships, delete catagorys/branches, and make catagory/branches
    /// inactive.  like when switching engines, trans, etc....
    /// 
    /// TODO: Add generic parts inventory view where all parts are listed and sort/filter ability.
    /// Catagory view - Make catagory/subcatagory data grids selection single record
    /// 
    /// TODO: 


    /// TODO:  Add lates order from LMR and walmart
}
