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

namespace AutomotiveBuilder.DAL
{
    public class Vender : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructor
        public Vender()
        {
        }
        #endregion

        #region Private Members
        private string _ID = Guid.NewGuid().ToString();
        private string _Name;
        private string _ContactName;
        private string _Phone;
        private string _Email;
        private string _Address;
        private string _City;
        private string _State;
        private string _Zip;
        private string _URL;
        private BindingList<Part> _Parts = new BindingList<Part>();
        private Part _SelectedPart;
        #endregion

        #region Properties
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
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
        public string ContactName
        {
            get { return _ContactName; }
            set
            {
                if (_ContactName != value)
                {
                    _ContactName = value;
                    OnPropertyChanged(nameof(ContactName));
                }
            }
        }
        public string Phone
        {
            get { return _Phone; }
            set
            {
                if (_Phone != value)
                {
                    _Phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }
        public string Email
        {
            get { return _Email; }
            set
            {
                if (_Email != value)
                {
                    _Email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }
        public string Address
        {
            get { return _Address; }
            set
            {
                if (_Address != value)
                {
                    _Address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }
        public string City
        {
            get { return _City; }
            set
            {
                if (_City != value)
                {
                    _City = value;
                    OnPropertyChanged(nameof(City));
                }
            }
        }
        public string State
        {
            get { return _State; }
            set
            {
                if (_State != value)
                {
                    _State = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }
        public string Zip
        {
            get { return _Zip; }
            set
            {
                if (_Zip != value)
                {
                    _Zip = value;
                    OnPropertyChanged(nameof(Zip));
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
        public Part SelectedPart
        {
            get
            {
                if(_SelectedPart == null && _Parts.Count > 0)
                {
                    _SelectedPart = _Parts[0];
                    _SelectedPart.PropertyChanged += _SelectedPart_PropertyChanged;
                }
                return _SelectedPart; 
            }
            set 
            {
                if (_SelectedPart != null)
                {
                    _SelectedPart.PropertyChanged -= _SelectedPart_PropertyChanged;
                }
                _SelectedPart = value;
                if (_SelectedPart == null) return;
                _SelectedPart.PropertyChanged += _SelectedPart_PropertyChanged;
                OnPropertyChanged(nameof(SelectedPart)); 
            }
        }

        private void _SelectedPart_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            OnPropertyChanged("CatagoryID");
        }

        [XmlIgnore]
        public BindingList<Part> Parts
        {
            get { return _Parts; }
            set
            {
                if (_Parts != value)
                {
                    _Parts = value;
                    OnPropertyChanged(nameof(Parts));
                }
            }
        }

        #endregion
    }

    public class VenderList : ObservableCollection<Vender>
    {

        public VenderList()
        {            
            string FileName = StaticMethods.PartsFolder + "Venders.xml";
            if (File.Exists(FileName))
            {
                try
                {
                    using (Stream reader = new FileStream(FileName, FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(VenderList));
                        // Call the Deserialize method to restore the object's state.
                        var deserialized = (VenderList)serializer.Deserialize(reader);
                        this.Clear();
                        foreach (var vender in deserialized)
                        {
                            this.Add(vender);
                        }
                        this.LastUpdate = deserialized.LastUpdate;
                    }
                }
                catch
                {
                //This can occur if the file is in use.  ignore for now.
                }
            }
            else
            {
                Vender NewVender = new Vender();
                NewVender.ID = Guid.NewGuid().ToString();
                NewVender.Name = "Summit";
                this.Add(NewVender);
                var serializer = new XmlSerializer(typeof(VenderList));
                using (var writer = new StreamWriter(FileName))
                {
                    serializer.Serialize(writer, this);
                }
            }
                this.LoadParts();
            //}
            //catch (Exception ex)
            //{
            //    string message = "Loading Vender data (file Venders.xml): | " + ex.Message + " | The Vender data file will be backed up and a new one created.";
            //    StaticMethods.RemakeMakesFile();
            //    //("A new Vender File has been created, please restart application.", "Vender Data File Backup/Repair");
            //}
        }
        public VenderList(bool loadParts)
        {
            if(!loadParts) return;
            string FileName = StaticMethods.PartsFolder + "Venders.xml";
            if (File.Exists(FileName))
            {
                try
                {
                    using (Stream reader = new FileStream(FileName, FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(VenderList));
                        // Call the Deserialize method to restore the object's state.
                        var deserialized = (VenderList)serializer.Deserialize(reader);
                        this.Clear();
                        foreach (var vender in deserialized)
                        {
                            this.Add(vender);
                        }
                        this.LastUpdate = deserialized.LastUpdate;
                    }
                }
                catch
                {
                    //This can occur if the file is in use.  ignore for now.
                }
            }
            else
            {
                Vender NewVender = new Vender();
                NewVender.ID = Guid.NewGuid().ToString();
                NewVender.Name = "Summit";
                this.Add(NewVender);
                var serializer = new XmlSerializer(typeof(VenderList));
                using (var writer = new StreamWriter(FileName))
                {
                    serializer.Serialize(writer, this);
                }
            }
            this.LoadParts();
            //}
            //catch (Exception ex)
            //{
            //    string message = "Loading Vender data (file Venders.xml): | " + ex.Message + " | The Vender data file will be backed up and a new one created.";
            //    StaticMethods.RemakeMakesFile();
            //    //("A new Vender File has been created, please restart application.", "Vender Data File Backup/Repair");
            //}
        }

        private DateTime _LastUpdate;

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
        public Vender ReturnVenderByID(string ID)
        {
            Vender rtnVender = null;

            foreach (Vender vdr in this)
            {
                if (vdr.ID == ID)
                {
                    return vdr;
                }
            }

            return rtnVender;
        }
        public Vender ReturnVenderFromName(string name)
        {
            Vender rtnVender = null;

            foreach (Vender tmpVender in this)
            {
                if (tmpVender.Name == name)
                {
                    return tmpVender;
                }
            }
            return rtnVender;
        }
        public void SaveVenderList(string FileName)
        {
            string PartsFile = StaticMethods.GetPartsFileName();

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            var serializer = new XmlSerializer(typeof(VenderList));
            using (var writer = new StreamWriter(FileName))
            {
                serializer.Serialize(writer, this);
            }
            PartList partList = new PartList();
            foreach (Vender vdr in this)
            {
                foreach (Part prt in vdr.Parts)
                {
                    foreach (Part existingPart in partList)
                    {
                        if (existingPart.ID == prt.ID)
                        {
                            partList.Remove(existingPart);
                        }
                    }
                    partList.Add(prt);
                }                
            }
            /// Remove parts that are not associated with any vender
            foreach (Part prt in partList)
            {
                bool partExists = false;
                foreach (Vender vdr in this)
                {
                    foreach (var vprt in vdr.Parts)
                    {
                        if (vprt.ID == prt.ID)
                        {
                            partExists = true;
                            break;
                        }
                    }  
                    if (partExists)
                    {
                        break;
                    }
                }
                if (!partExists)
                {
                    partList.Remove(prt);
                }
            }
            partList.SavePartList(PartsFile);
            StaticMethods.PartsChanged = true;
            MessengerService.LogMessage("Vender List", MessengerService.MessageType.DataChanged);
        }
        public void LoadParts()
        {
            foreach (var vndr in this)
            {
                foreach (var vpart in StaticMethods.ProjPartList.Where(prt => prt.VenderID == vndr.ID))
                {
                    vndr.Parts.Add(vpart);
                }
                if (vndr.Parts.Count > 0)
                {
                    vndr.SelectedPart = vndr.Parts[0];
                }
            }
        }
    }
}
