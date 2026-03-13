using AutomotiveBuilder.Static;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutomotiveBuilder.Static.MessengerService;

namespace AutomotiveBuilder.DAL
{
    public class Safeties : INotifyPropertyChanged
    {
        #region Property Changed Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Message Handlers
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
                    break;
                case MessageType.DataChanged:
                    // Handle data changed messages, e.g., refresh views or update properties
                    switch (text)
                    {
                        case "Part Catagories":

                            break;

                        default:

                            break;
                    }
                    break;
            }
        }
        private void SendMessage(string text, MessageType type = MessageType.Info)
        {
            MessengerService.LogMessage(text, type);
        }

        #endregion

        public Safeties()
        {
                     
        }


    }

    public class FuelPressureSafety : INotifyPropertyChanged
    {
        #region Property Changed Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Message Handlers
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
                    break;
                case MessageType.DataChanged:
                    // Handle data changed messages, e.g., refresh views or update properties
                    switch (text)
                    {
                        case "Part Catagories":

                            break;

                        default:

                            break;
                    }
                    break;
            }
        }
        private void SendMessage(string text, MessageType type = MessageType.Info)
        {
            MessengerService.LogMessage(text, type);
        }

        #endregion

        #region Private Members
        private int _tableSize;
        private float[] _mapPSI;
        private int[] _mapkPa;
        private float[] fuelPressurePSI;
        private float _basePressurePSI;
        private float _minFuelPressurePSI;
        private float _maxFuelPressurePSI;
        private float _maxMAPPSI;

        #endregion

        #region Properties
        public int TableSize
        {
            get => _tableSize;
            set
            {
                if (_tableSize != value)
                {
                    _tableSize = value;
                    OnPropertyChanged(nameof(TableSize));
                    MapPSI = new float[_tableSize];
                    MapkPa = new int[_tableSize];
                    FuelPressurePSI = new float[_tableSize];
                    LoadTableData();
                }
            }
        }
        public float[] MapPSI
        {
            get => _mapPSI;
            set
            {
                if (_mapPSI != value)
                {
                    _mapPSI = value;
                    OnPropertyChanged(nameof(MapPSI));
                }
            }
        }
        public int[] MapkPa
        {
            get => _mapkPa;
            set
            {
                if (_mapkPa != value)
                {
                    _mapkPa = value;
                    OnPropertyChanged(nameof(MapkPa));
                }
            }
        }
        public float[] FuelPressurePSI
        {
            get => fuelPressurePSI;
            set
            {
                if (fuelPressurePSI != value)
                {
                    fuelPressurePSI = value;
                    OnPropertyChanged(nameof(FuelPressurePSI));
                }
            }
        }
        public float BasePressure
        {
            get => _basePressurePSI;
            set
            {
                if (_basePressurePSI != value)
                {
                    _basePressurePSI = value;
                    OnPropertyChanged(nameof(BasePressure));
                    SendMessage($"Base Pressure updated to {value}", MessageType.DataChanged);
                }
            }
        }
        public float MaxMAPPSI
        {
            get => _maxMAPPSI;
            set
            {
                if (_maxMAPPSI != value)
                {
                    _maxMAPPSI = value;
                    OnPropertyChanged(nameof(MaxMAPPSI));
                }
            }
        }
        public float MinFuelPressurePSI
        {
            get => _minFuelPressurePSI;
            set
            {
                if (_minFuelPressurePSI != value)
                {
                    _minFuelPressurePSI = value;
                    OnPropertyChanged(nameof(MinFuelPressurePSI));
                }
            }
        }
        public float MaxFuelPressurePSI
        {
            get => _maxFuelPressurePSI;
            set
            {
                if (_maxFuelPressurePSI != value)
                {
                    _maxFuelPressurePSI = value;
                    OnPropertyChanged(nameof(MaxFuelPressurePSI));
                }
            }
        }

        #endregion

        public FuelPressureSafety()
        {
        }

        private void LoadTableData()
        {
            if(MaxMAPPSI <= 0 || TableSize <= 1)
            {
                return;
            }
            for (int i = 0; i < TableSize; i++)
            {
                MapkPa[i] = (int)((MaxMAPPSI / 0.145038f) * ((float)i / (TableSize - 1)));
                MapPSI[i] = (float)((MapkPa[i] * 0.145038f) -14.696);
                FuelPressurePSI[i] = BasePressure + (MapPSI[i]);
            }
        }


        public class FuelPressureDelta : INotifyPropertyChanged
        {
            #region Property Changed Events
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region Private Members
            private int _mapkPa;
            private float _fuelPressurePSI;
            private float _deltaPSI;

            #endregion

            #region Properties
            public int MapkPa
            {
                get => _mapkPa;
                set
                {
                    if (_mapkPa != value)
                    {
                        _mapkPa = value;
                        OnPropertyChanged(nameof(MapkPa));
                    }
                }
            }
            public float FuelPressurePSI
            {
                get => _fuelPressurePSI;
                set
                {
                    if (_fuelPressurePSI != value)
                    {
                        _fuelPressurePSI = value;
                        OnPropertyChanged(nameof(FuelPressurePSI));
                    }
                }
            }
            public float DeltaPSI
            {
                get => _deltaPSI;
                set
                {
                    if (_deltaPSI != value)
                    {
                        _deltaPSI = value;
                        OnPropertyChanged(nameof(DeltaPSI));
                    }
                }
            }

            #endregion

            public FuelPressureDelta()
            {
            }
        }
    }

}
