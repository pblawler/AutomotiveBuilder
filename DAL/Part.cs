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
    public class Part : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
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
                    // Handle data changed messages
                    break;
            }
        }
        private void SendMessage(string text, MessageType type = MessageType.Info)
        {
            MessengerService.LogMessage(text, type);
        }
        #endregion

        /// TODO: Add list of available venders, make vender/venderID a preferred vender
        /// 
        /// 

        #region Constructor
        public Part()
        {
        }
        #endregion

        #region Private Members
        private string _ID = "";
        private string _CategoryID="";
        private string _SubCategoryID="";
        private string _Name = "";
        private string _PartNumber = "";
        private string _Description = "";
        private decimal _Cost= (decimal) 0.0;
        private decimal _tax = (decimal) 0.0;
        private decimal _Shipping=(decimal) 0.0;
        private int _QuantityOnHand=0;
        private string _VenderID = "";
        private string _URL = "";
        private string _imgHex = "";
        private string _Location = "";
        #endregion

        #region Properties
        public string ID
        {
            get
            {
                if (_ID == "") _ID = Guid.NewGuid().ToString();
                return _ID; 
            }
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    OnPropertyChanged(nameof(ID));
                }
            }
        }
        public string CategoryID
        {
            get { return _CategoryID; }
            set
            {
                if (_CategoryID != value)
                {
                    _CategoryID = value;
                    OnPropertyChanged(nameof(CategoryID));
                }
            }
        }
        public string SubCategoryID
        {
            get { return _SubCategoryID; }
            set
            {
                if (_SubCategoryID != value)
                {
                    _SubCategoryID = value;
                    OnPropertyChanged(nameof(SubCategoryID));
                }
            }
        }
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string PartNumber
        {
            get { return _PartNumber; }
            set
            {
                if (_PartNumber != value)
                {
                    _PartNumber = value;
                    OnPropertyChanged(nameof(PartNumber));
                }
            }
        }
        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public string Catagory
        {
            get
            {
                string catName;
                catName = StaticMethods.GetPartCatagoryName(_CategoryID);
                return catName; 
            } 
        }
        public string SubCatagory
        {
            get
            {
                string catName;
                catName = StaticMethods.GetPartSubCatagoryName(_SubCategoryID);
                return catName;
            }
        }
        public decimal Cost
        {
            get { return _Cost; }
            set
            {
                if (_Cost != value)
                {
                    _Cost = value;
                    OnPropertyChanged(nameof(Cost));
                    OnPropertyChanged(nameof(Invested));
                    SendMessage($"Part '{Name}' cost updated to {Cost:C}.", MessageType.DataChanged);
                }
            }
        }
        public decimal Tax
        {
            get { return _tax; }
            set
            {
                if (_tax != value)
                {
                    _tax = value;
                    OnPropertyChanged(nameof(Tax));
                    SendMessage($"Part '{Name}' tax updated to {Tax:C}.", MessageType.DataChanged);
                }
            }
        }
        public decimal Shipping
        {
            get { return _Shipping; }
            set
            {
                if (_Shipping != value)
                {
                    _Shipping = value;
                    OnPropertyChanged(nameof(Shipping));
                    SendMessage($"Part '{Name}' shipping cost updated to {Shipping:C}.", MessageType.DataChanged);
                }
            }
        }
        public int QuantityOnHand
        {
            get { return _QuantityOnHand; }
            set
            {
                if (_QuantityOnHand != value)
                {
                    _QuantityOnHand = value;
                    OnPropertyChanged(nameof(QuantityOnHand));
                    OnPropertyChanged(nameof(Invested));
                    SendMessage($"Part '{Name}' quantity on hand updated to {QuantityOnHand}.", MessageType.DataChanged);
                }
            }
        }
        public string VenderID
        {
            get { return _VenderID; }
            set
            {
                if (_VenderID != value)
                {
                    _VenderID = value;
                    OnPropertyChanged(nameof(VenderID));
                }
            }
        }
        public string URL
        {
            get { return _URL; }
            set
            {
                if (_URL != value)
                {
                    _URL = value;
                    OnPropertyChanged(nameof(URL));
                }
            }
        }
        public string ImgHEX
        {
            get { return _imgHex; }
            set
            {
                if(_imgHex != value)
                {
                    _imgHex = value;
                    OnPropertyChanged(nameof(ImgHEX));
                }
            }
        }
        public string Location
        {
            get { return _Location; }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }
        [XmlIgnore]
        public string Invested
        {
            get
            {
                float inv = (float) _Cost * _QuantityOnHand;
                return "$" + inv.ToString();
            }
        }
        [XmlIgnore]
        public string Vender
        {
            get
            {
                string venderName;
                venderName = StaticMethods.GetVenderName(_VenderID);
                return venderName;
            }
        }
        #endregion
    }

    public class PartList : ObservableCollection<Part>
    {
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

        public PartList()
        {
        }

        public PartList(string FileName)
        {
            LoadPartList(FileName);
        }

        public void LoadPartList(string FileName = "")
        {
            try
            {
                ///TODO: stop calling this repeadily, move to static resource and update on static dirty
                Cursor.Current = Cursors.WaitCursor;
                if (FileName == "") FileName = StaticMethods.GetPartsFileName();
                if (File.Exists(FileName))
                {
                    var serializer = new XmlSerializer(typeof(PartList));
                    using (var reader = new StreamReader(FileName))
                    {
                        PartList loadedList = (PartList)serializer.Deserialize(reader);
                        this.Clear();
                        foreach (var part in loadedList)
                        {
                            this.Add(part);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, show a message to the user, etc.)
                SendMessage($"Error loading part list: {ex.Message}", MessageType.Error);
            }
            Cursor.Current = Cursors.Default;
        }
        public void SavePartList(string FileName = "")
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (FileName == "") FileName = StaticMethods.GetPartsFileName();
                if (File.Exists(FileName))
                {
                    File.Delete(FileName);
                }
                var serializer = new XmlSerializer(typeof(PartList));
                using (var writer = new StreamWriter(FileName))
                {
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, show a message to the user, etc.)
                SendMessage($"Error saving part list: {ex.Message}", MessageType.Error);
            }
            StaticMethods.PartsChanged = true;
            StaticMethods.LoadParts();            
            Cursor.Current = Cursors.Default;
            MessengerService.LogMessage("Part List", MessengerService.MessageType.DataChanged);
        }
    }

    public class UsedPart : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
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
                    // Handle data changed messages
                    break;
            }
        }
        private void SendMessage(string text, MessageType type = MessageType.Info)
        {
            MessengerService.LogMessage(text, type);
        }
        #endregion

        private string _PartID;
        private float _CostEA;
        private int _QuantityUsed;
        private int _QuantityRemoved;

        public string PartID
        {
            get { return _PartID; }
            set
            {
                if (_PartID != value)
                {
                    _PartID = value;
                    OnPropertyChanged(nameof(PartID));
                }
            }
        }
        [XmlIgnore]
        public string PartName
        {
            get
            {
                string partName;
                partName = StaticMethods.GetPartName(_PartID);
                return partName;
            }
        }
        [XmlIgnore]
        public string PartNumber
        {
            get
            {
                string partNumber;
                partNumber = StaticMethods.GetPartNumber(_PartID);
                return partNumber;
            }
        }
        public float CostEA
        {
            get { return _CostEA; }
            set
            {
                if (_CostEA != value)
                {
                    _CostEA = value;
                    OnPropertyChanged(nameof(CostEA));
                }
            }
        }
        public int QuantityUsed
        {
            get { return _QuantityUsed; }
            set
            {
                if (_QuantityUsed != value)
                {
                    _QuantityUsed = value;
                    OnPropertyChanged(nameof(QuantityUsed));
                }
            }
        }
        public int QuantityRemoved
        {
            get { return _QuantityRemoved; }
            set
            {
                if (_QuantityRemoved != value)
                {
                    _QuantityRemoved = value;
                    OnPropertyChanged(nameof(QuantityRemoved));
                }
            }
        }
    }
}
