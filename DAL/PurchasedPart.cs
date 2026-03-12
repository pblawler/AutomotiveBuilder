using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomotiveBuilder.DAL
{
    public class PurchasedPart : Part
    {
        #region Constructor
        public PurchasedPart() : base()
        {
        }
        #endregion

        #region Private Members
        private DateTime _PurchaseDate;
        private decimal _PurchasePrice;
        private string _PurchasedFromVenderID;
        private int _QuantityPurchased;
        private decimal _subTotal;
        private decimal _taxAmount;
        private decimal _shippingAmount;
        private decimal _surCharges;
        private decimal _total;
        #endregion

        #region Properties
        public DateTime PurchaseDate
        {
            get { return _PurchaseDate; }
            set
            {
                if (_PurchaseDate != value)
                {
                    _PurchaseDate = value;
                    OnPropertyChanged(nameof(PurchaseDate));
                }
            }
        }
        public decimal PurchasePrice
        {
            get { return _PurchasePrice; }
            set
            {
                if (_PurchasePrice != value)
                {
                    _PurchasePrice = value;
                    OnPropertyChanged(nameof(PurchasePrice));
                }
            }
        }
        public string PurchasedFromVenderID
        {
            get { return _PurchasedFromVenderID; }
            set
            {
                if (_PurchasedFromVenderID != value)
                {
                    _PurchasedFromVenderID = value;
                    OnPropertyChanged(nameof(PurchasedFromVenderID));
                }
            }
        }
        public int QuantityPurchased
        {
            get { return _QuantityPurchased; }
            set
            {
                if (_QuantityPurchased != value)
                {
                    _QuantityPurchased = value;
                    OnPropertyChanged(nameof(QuantityPurchased));
                }
            }
        }
        public decimal SubTotal
        {
            get
            {
                    _subTotal = _PurchasePrice * _QuantityPurchased;
                return _subTotal; 
            }
        }
        public decimal TaxAmount
        {
            get { return _taxAmount; }
            set
            {
                if (_taxAmount != value)
                {
                    _taxAmount = value;
                    OnPropertyChanged(nameof(TaxAmount));
                }
            }
        }
        public decimal ShippingAmount
        {
            get { return _shippingAmount; }
            set
            {
                if (_shippingAmount != value)
                {
                    _shippingAmount = value;
                    OnPropertyChanged(nameof(ShippingAmount));
                }
            }
        }
        public decimal SurCharges
        {
            get { return _surCharges; }
            set
            {
                if (_surCharges != value)
                {
                    _surCharges = value;
                    OnPropertyChanged(nameof(SurCharges));
                }
            }
        }
        public decimal Total
        {
            get
            {
                _total = SubTotal + _taxAmount + _shippingAmount + _surCharges;
                return _total;
            }
        }
        #endregion
    }
}
