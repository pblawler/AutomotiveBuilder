using AutomotiveBuilder.Statics;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Xml.Serialization;

namespace AutomotiveBuilder.DAL
{
    public class Project : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructor
        public Project()
        {   
            if(_BOM == null) _BOM = new AutoBuilder(this.ID);
            _BOM.PropertyChanged += PropertyChanged;
        }
        #endregion

        #region Private Members
        private string _Name;
        private DateTime _startDate;
        private string _MakeID;
        private string _EngineMakeID;
        private string _ModelID;
        private string _EngineModelID;
        private string _TrimCode;
        private string _PaintCode;
        private string _InteriorCode;
        private string _VIN;
        private string _Notes;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private string _ID="";
        private string _FileName = "";
        private AutoBuilder _BOM;
        private string _selectedComponentPath;
        private Component _selectedComponent;
        private string _imgHex;
        private DTM _driveTrain;
        #endregion

        #region Properties
        public string ID
        {
            get
            {
                if(_ID == "")
                {
                    _ID = Guid.NewGuid().ToString();
                }   
                return _ID; 
            }
            set
            {
                if(_ID == value) return;
                _ID = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        public string Name
        {
            get { return _Name; }
            set
            {
                try
                {
                    _Name = value;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                try
                {
                    _startDate = value;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        public DateTime EndDate
        {
            get { return _EndDate; }
            set
            {
                try
                {
                    _EndDate = value;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        public string MakeID
        {
            get { return _MakeID; }
            set
            {
                try
                {
                    _MakeID = value;
                    OnPropertyChanged(nameof(MakeID));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        public string EngineMakeID
        {
            get { return _EngineMakeID; }
            set
            {
                try
                {
                    _EngineMakeID = value;
                    OnPropertyChanged(nameof(EngineMakeID));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        public string ModelID
        {
            get { return _ModelID; }
            set
            {
                try
                {
                    _ModelID = value;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        public string EngineModelID
        {
            get { return _EngineModelID; }
            set
            {
                try
                {
                    _EngineModelID = value;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        public string FileName
        {
            get { return _FileName; }
            set
            {
                try
                {
                    _FileName = value;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        public string MakeName
        {
            get
            {
                string strMakeName = "";
                MakesList makes = StaticMethods.LoadMakes();
                Make make = makes.FirstOrDefault(m => m.ID == _MakeID);
                if (make != null)
                {
                    strMakeName = make.Name;
                }
                return strMakeName;
            }
        }
        public string ModelName
        {
            get
            {
                string strModelName = "";
                MakesList makes = StaticMethods.LoadMakes();
                Make make = makes.FirstOrDefault(m => m.ID == _MakeID);
                if (make != null)
                {
                    Model model = make.Models.FirstOrDefault(md => md.ID == _ModelID);
                    if (model != null)
                    {
                        strModelName = model.DisplayName;
                    }
                }
                return strModelName;
            }
        }
        public AutoBuilder BOM
        {
            get { return _BOM; }
            set
            {
                _BOM = value;
                OnPropertyChanged(nameof(BOM));
            }
        }
        public string SelectedComponentPath
        {
            get { return _selectedComponentPath; }
            set
            {
                _selectedComponentPath = value;
                OnPropertyChanged(nameof(SelectedComponentPath));
            }
        }
        [XmlIgnore]
        public Component SelectedComponent
        {
            get { return _selectedComponent; }
            set
            {
                if(_selectedComponent != value)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    _selectedComponent = value;
                    OnPropertyChanged(nameof(SelectedComponent));
                    OnPropertyChanged(nameof(SelectedComponent.ImgHEX));
                    Cursor.Current = Cursors.Default;
                }
            }
        }
        public string ImgHEX
        {
            get { return _imgHex; }
            set
            {
                if (_imgHex != value)
                {
                    _imgHex = value;
                    OnPropertyChanged(nameof(ImgHEX));
                }
            }
        }
        public string Notes
        {
            get { return _Notes; }
            set
            {
                try
                {
                    _Notes = value;
                    //OnPropertyChanged(nameof(Notes));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        public DTM DriveTrain
        {
            get 
            {
                if(_driveTrain == null)
                {
                    _driveTrain = new DTM();
                }   
                return _driveTrain; 
            }
            set
            {
                try
                {
                    _driveTrain = value;
                    OnPropertyChanged(nameof(DriveTrain));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion       

        public void SaveProject()
        {
            string strFileName;

            var serializer = new XmlSerializer(typeof(Project));

            if (_FileName == "")
            {
                strFileName = StaticMethods.ProjectFolder + "ExampleProject.xml";
            }
            else
            {
                strFileName = _FileName;
            }
            try
            {
                if (File.Exists(strFileName))
                {
                    File.Delete(strFileName);
                }
            }
            catch
            {
                ///The file is being accessed try again later.

                System.Windows.MessageBox.Show("The file is being accessed try again later.");
                return;
            }
            using (var writer = new StreamWriter(strFileName))
            {
                serializer.Serialize(writer, this);
            }
        }
    }
}
