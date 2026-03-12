using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomotiveBuilder.DAL
{
    public class PartCatagory : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructor
        public PartCatagory()
        {
        }
        #endregion

        #region Private Members
        private string _ID = "";
        private string _Name;
        private string _Description;
        private BindingList<PartSubCatagory> _SubCategories = new BindingList<PartSubCatagory>();
        private PartSubCatagory _selectedSubcatagory;
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
        public BindingList<PartSubCatagory> SubCategories
        {
            get 
            {
                _SubCategories = new BindingList<PartSubCatagory>(_SubCategories.OrderBy(x => x.Name).ToList());
                return _SubCategories; 
            }
            set
            {
                if (_SubCategories != value)
                {
                    _SubCategories = value;
                    OnPropertyChanged(nameof(SubCategories));
                }
            }
        }
        public PartSubCatagory SelectedSubcatagory
        {
            get { return _selectedSubcatagory; }
            set
            {
                _selectedSubcatagory = value;
                OnPropertyChanged(nameof(SelectedSubcatagory));
            }

        }
        #endregion

    }
}
