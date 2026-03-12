using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutomotiveBuilder.DAL
{
    public class Make : IComparable
    {
        #region Private Members
        private string _ID = Guid.NewGuid().ToString();
        private string _Name;
        private string _Description;
        private string _Address;
        private string _City;
        private string _State;
        private string _PostalCode;
        private string _Country;
        private string _Phone;
        private string _Url;
        private BindingList<Model> _Models;
        private BindingList<Engine> _Engines;
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
            set { _Name = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }
        public string City
        {
            get { return _City; }
            set { _City = value; }
        }
        public string State
        {
            get { return _State; }
            set { _State = value; }
        }
        public string PostalCode
        {
            get { return _PostalCode; }
            set { _PostalCode = value; }
        }
        public string Country
        {
            get { return _Country; }
            set { _Country = value; }
        }
        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }
        public BindingList<Model> Models
        {
            get { return _Models; }
            set { _Models = value; }
        }
        public BindingList<Engine> Engines
        {
            get { return _Engines; }
            set { _Engines = value; }
        }
        #endregion

        public int CompareTo(object o)
        {            
            Make a = this;
            Make b = (Make)o;
            return  string.Compare(a.Name, b.Name);
        }


    }

    public class MakeCountryComparer : IComparer<Make>
    {
        public int Compare(Make x, Make y)
        {
            try
            {
                if (x.Country.CompareTo(y.Country) < 0)
                {
                    return -1;
                }
                else if (x.Country.CompareTo(y.Country) == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                return 0;
            }
        }
    }    

    public class MakesList : ObservableCollection<Make>
    {
        private DateTime _LastUpdate;

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }

        public Make ReturnMakeByID(string ID)
        {
            Make rtnMake = null;

            foreach (Make tmpMake in this)
            {
                if (tmpMake.ID == ID)
                {
                    return tmpMake;
                }
            }

            return rtnMake; 
        }
        public Make ReturnMakeFromName(string name)
        {
            Make rtnMake = null;

            foreach (Make tmpmake in this)
            {
                if (tmpmake.Name == name)
                {
                    return tmpmake;
                }
            }
            return rtnMake;
        }
        public void SaveMakeList(string FileName)
        {
            var serializer = new XmlSerializer(typeof(MakesList));
            using (var writer = new StreamWriter(FileName))
            {
                serializer.Serialize(writer, this);
            }
        }
    }

}
