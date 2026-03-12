using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomotiveBuilder.DAL
{
    public class PartSubCatagory : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructor
        public PartSubCatagory()
        {
        }
        #endregion

        #region Private Members
        private string _ID = Guid.NewGuid().ToString();
        private string _CategoryID;
        private string _Name;
        private string _Description;
        #endregion

        #region Properties
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
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
        #endregion

    }
}
