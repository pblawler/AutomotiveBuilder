using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutomotiveBuilder.DAL
{
    public class PartType : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructor
        public PartType()
        {
        }
        #endregion

        #region Private Members
        private string _ID = Guid.NewGuid().ToString();
        private string _Name;
        private string _Description;
        private int _quantityInStock;
        private int _quantityOutStock;
        private int _quantityRequired;
        private int _quantityUsed;
        private decimal _Cost;
        private decimal _TotalPurchaseCost;
        private decimal _RemainingCost;
        private decimal _CostInStock;
        private decimal _CostRequired;
        private BindingList<Part> _PartOptions = new BindingList<Part>();

        #endregion

        #region Properties
        public string ID
        {
            get { return _ID; }
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
        public int QuantityInStock
        {
            get { return _quantityInStock; }
            set
            {
                if (_quantityInStock != value)
                {
                    _quantityInStock = value;
                    OnPropertyChanged(nameof(QuantityInStock));
                    OnPropertyChanged(nameof(QuantityOutStock));
                }
            }
        }
        public int QuantityOutStock
        {
            get
            {
                _quantityOutStock = _quantityRequired - _quantityInStock;
                if (_quantityOutStock < 0)
                {
                    _quantityOutStock = 0;
                }
                return _quantityOutStock; 
            }
        }
        public int QuantityRequired
        {
            get { return _quantityRequired; }
            set
            {
                if (_quantityRequired != value)
                {
                    _quantityRequired = value;
                    OnPropertyChanged(nameof(QuantityRequired));
                    OnPropertyChanged(nameof(QuantityOutStock));
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
        public decimal Cost
        {
            get { return _Cost; }
            set
            {
                if (_Cost != value)
                {
                    _Cost = value;
                    OnPropertyChanged(nameof(Cost));
                }
            }
        }
        public decimal TotalPurchaseCost
        {
            get { return _TotalPurchaseCost; }
            set
            {
                if (_TotalPurchaseCost != value)
                {
                    _TotalPurchaseCost = value;
                    OnPropertyChanged(nameof(TotalPurchaseCost));
                }
            }
        }
        public decimal RemainingCost
        {
            get
            {
                _RemainingCost = _Cost * _quantityOutStock;
                return _RemainingCost; 
            }            
        }
        public decimal CostInStock
        {
            get
            {
                _CostInStock = _Cost * _quantityInStock;
                return _CostInStock; 
            }
        }
        public decimal CostRequired
        {
            get
            {
                _CostRequired = _Cost * _quantityRequired;
                return _CostRequired; 
            }
        }
        public BindingList<Part> PartOptions
        {
            get { return _PartOptions; }
            set
            {
                if (_PartOptions != value)
                {
                    _PartOptions = value;
                    OnPropertyChanged(nameof(PartOptions));
                }
            }
        }
        #endregion
    }
}
