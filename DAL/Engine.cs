using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomotiveBuilder.DAL
{
    public class Engine
    {
        #region Private Members
        private string _name;
        private string _description;
        private string _ModelNumber;
        private string _URL;
        private string _Year;
        private string _ID = Guid.NewGuid().ToString();
        #endregion

        #region Properties
        public string ID { get { return _ID; } set { _ID = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public string ModelNumber { get { return _ModelNumber; } set { _ModelNumber = value; } }
        public string URL { get { return _URL; } set { _URL = value; } }
        public string Year { get { return _Year; } set { _Year = value; } }
        public string DisplayName
        {
            get
            {
                return $"{_Year} {_name} {_ModelNumber}";
            }
        }

        #endregion
    }
}
