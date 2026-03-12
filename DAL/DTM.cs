using AutomotiveBuilder.Static;
using AutomotiveBuilder.Statics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Xml.Serialization;
using static AutomotiveBuilder.Static.MessengerService;

namespace AutomotiveBuilder.DAL
{
    public class DTM : INotifyPropertyChanged
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

        public DTM()
        {
            // Initialize default values for properties if needed
            // For example, you can set default gear ratios or shift schedules here
        }

        #region Private Members
        private float _DiffRatio=3.73f;
        private int _tireWidth=275;
        private int _tireAspectRatio=60;
        private int _wheelDiameter=15;
        private int _targetRPM=2500;
        private int _maxRPM=8000;
        private int _vssPerRev=40;
        private int _WOTrpm=7500;
        private int _shiftRPM = 2500;
        private int _minRPM=1800;
        private float _gain=0.12f;
        private float _decay=0.1f;
        private int _numberOfGears=4;
        private byte _TableSize=16;
        private BindingList<Gear> _gears;
        private BindingList<ShiftSchedule> _shiftSchedules;
        private int _stallSpeed=1000;
        private int _minTCCgear=0;
        private bool _TCCtrippleDisk=false;
        private int _maxCruiseThrottle = 50;
        private string _TDCC = "NO";
        private int _wotTransThrottle = 75;
        private int _wotLockRPM = 1100;
        private int _wotMaxGear = 2;
        private List<int> _ThrottlePositions = new List<int>();
        private BindingList<TCCpoint> _TCCstrategy = new BindingList<TCCpoint>();
        #endregion

        #region Properties
        public float DiffRatio
        {
            get { return _DiffRatio; }
            set
            {
                if (_DiffRatio != value)
                {
                    _DiffRatio = value;
                    OnPropertyChanged(nameof(DiffRatio));
                    CalculateGears();
                }
            }
        }
        public int TireWidth
        {
            get { return _tireWidth; }
            set
            {
                if (_tireWidth != value)
                {
                    _tireWidth = value;
                    OnPropertyChanged(nameof(TireWidth));
                    CalculateGears();
                }
            }
        }
        public int TireAspectRatio
        {
            get { return _tireAspectRatio; }
            set
            {
                if (_tireAspectRatio != value)
                {
                    _tireAspectRatio = value;
                    OnPropertyChanged(nameof(TireAspectRatio));
                    CalculateGears();
                }
            }
        }
        public int WheelDiameter
        {
            get { return _wheelDiameter; }
            set
            {
                if (_wheelDiameter != value)
                {
                    _wheelDiameter = value;
                    OnPropertyChanged(nameof(WheelDiameter));
                    CalculateGears();
                }
            }
        }
        public int TargetRPM
        {
            get { return _targetRPM; }
            set
            {
                if (_targetRPM != value)
                {
                    _targetRPM = value;
                    OnPropertyChanged(nameof(TargetRPM));
                    CalculateGears();
                }
            }
        }
        public int MaxRPM
        {
            get { return _maxRPM; }
            set
            {
                if (_maxRPM != value)
                {
                    _maxRPM = value;
                    OnPropertyChanged(nameof(MaxRPM));
                    CalculateGears();
                }
            }
        }
        public int VSSPerRev
        {
            get { return _vssPerRev; }
            set
            {
                if (_vssPerRev != value)
                {
                    _vssPerRev = value;
                    OnPropertyChanged(nameof(VSSPerRev));
                }
            }
        }
        public int WOTrpm
        {
            get { return _WOTrpm; }
            set
            {
                if (_WOTrpm != value)
                {
                    _WOTrpm = value;
                    OnPropertyChanged(nameof(WOTrpm));
                    CalculateGears();
                }
            }
        }
        public int ShiftRPM
        {
            get { return _shiftRPM; }
            set
            {
                if (_shiftRPM != value)
                {
                    _shiftRPM = value;
                    OnPropertyChanged(nameof(ShiftRPM));
                    CalculateGears();
                }
            }
        }
        public int MinRPM
        {
            get { return _minRPM; }
            set
            {
                if (_minRPM != value)
                {
                    _minRPM = value;
                    OnPropertyChanged(nameof(MinRPM));
                    CalculateGears();
                }
            }
        }
        public float Gain
        {
            get { return _gain; }
            set
            {
                if (_gain != value)
                {
                    _gain = value;
                    OnPropertyChanged(nameof(Gain));
                    CalculateGears();
                }
            }
        }
        public float Decay
        {
            get { return _decay; }
            set
            {
                if (_decay != value)
                {
                    _decay = value;
                    OnPropertyChanged(nameof(Decay));
                    CalculateGears();
                }
            }
        }
        public int NumberOfGears
        {
            get { return _numberOfGears; }
            set
            {
                if (_numberOfGears != value)
                {
                    _numberOfGears = value;
                    if (_gears != null)
                    {
                        foreach (var gear in _gears)
                        {
                            gear.PropertyChanged -= TmpG_PropertyChanged;
                        }
                    }
                    _gears = new BindingList<Gear>();
                    Gear g;
                    for(int i = 0; i < _numberOfGears; i++)
                    {
                        g = new Gear();
                        g.GearNumber = i+1;
                        g.PropertyChanged += TmpG_PropertyChanged;
                        _gears.Add(g);
                    }
                    OnPropertyChanged(nameof(NumberOfGears));
                    OnPropertyChanged(nameof(AvailableGears));
                    FormatShiftTable();
                    CalculateGears();
                }
            }
        }
        public int TireDiametermm
        {
            get
            {
                // Calculate tire diameter based on width, aspect ratio, and wheel diameter
                return (int)(2 * (_tireWidth * (_tireAspectRatio / 100.0)) + (_wheelDiameter * 25.4));
            }
        }   
        public float TireDiameterInches
        {
            get
            {
                return TireDiametermm / 25.4f;
            }
        }
        public BindingList<Gear> Gears
        {
            get
            {               
                return _gears; 
            }
            set
            {
                if (_gears != value)
                {
                    _gears = value;
                    OnPropertyChanged(nameof(Gears));
                    OnPropertyChanged(nameof(AvailableGears));
                }
            }
        }
        public int MinTCCgear
        {
            get { return _minTCCgear; }
            set
            {
                if(_minTCCgear != value)
                {
                    _minTCCgear = value;
                    OnPropertyChanged(nameof(MinTCCgear));
                    CalculateTCCdata();
                }
            }
        }
        public int StallSpeed
        {
            get => _stallSpeed;
            set
            {
                if(_stallSpeed != value)
                {
                    _stallSpeed = value;
                    OnPropertyChanged(nameof(StallSpeed));
                    CalculateTCCdata();
                }
            }
        }
        public bool TCCtrippleDisk
        {
            get { return _TCCtrippleDisk; }
            set
            {
                if(_TCCtrippleDisk != value)
                {
                    _TCCtrippleDisk = value;
                    OnPropertyChanged(nameof(_TCCtrippleDisk));
                    OnPropertyChanged(nameof(TDCC));
                    CalculateTCCdata();
                }
            }
        }
        public int MaxCruiseThrottle
        {
            get { return _maxCruiseThrottle; }
            set
            {
                if( _maxCruiseThrottle != value)
                {
                    _maxCruiseThrottle = value;
                    OnPropertyChanged(nameof(MaxCruiseThrottle));
                    OnPropertyChanged(nameof(WTT));
                    CalculateTCCdata();
                }
            }
        }
        public int WOTtransitionThrottle
        {
            get { return _wotTransThrottle; }
            set
            {
                if( (_wotTransThrottle != value))
                {
                    _wotTransThrottle = value;
                    OnPropertyChanged(nameof(WOTtransitionThrottle));
                    CalculateTCCdata();
                }
            }
        }
        public int WOTlockRPM
        {
            get { return _wotLockRPM; }
            set
            {
                if( ( _wotLockRPM != value))
                {
                    _wotLockRPM = value;
                    OnPropertyChanged(nameof(WOTlockRPM));
                    CalculateTCCdata();
                }
            }
        }
        public int WOTmaxGear
        {
            get { return _wotMaxGear; }
            set
            {
                if(_wotMaxGear != value)
                {
                    _wotMaxGear = value;
                    OnPropertyChanged(nameof(WOTmaxGear));
                    CalculateTCCdata();
                }
            }
        }
        public BindingList<ShiftSchedule> ShiftSchedules
        {
            get { return _shiftSchedules; }
            set
            {
                if (_shiftSchedules != value)
                {
                    _shiftSchedules = value;
                    OnPropertyChanged(nameof(ShiftSchedules));
                }
            }
        }
        public byte TableSize
        {
            get { return _TableSize; }
            set
            {
                if (_TableSize != value)
                {
                    _TableSize = value;
                    FormatShiftTable();
                    OnPropertyChanged(nameof(ShiftSchedules));
                    OnPropertyChanged(nameof(TableSize));
                    CalculateTCCdata();
                }
            }
        }

        [XmlIgnore]
        public BindingList<int> AvailableGears
        {
            get
            {
                BindingList<int> ag = new BindingList<int>();
                if ((_gears == null) || (_gears.Count <= 0))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ag.Add((i + 1));
                    }
                    return ag;
                }
                for (int i = 0; i < _gears.Count; i++)
                {
                    ag.Add((i + 1));
                }                
                return ag;
            }
        }
        [XmlIgnore]
        public string TDCC
        {
            get
            {
                if(_TCCtrippleDisk)
                {
                    _TDCC = "YES";
                }
                else
                {
                    _TDCC = "NO";
                }
                return _TDCC;
            }
            set
            {
                if (_TDCC != value)
                {
                    _TDCC = value;
                    if (_TDCC == "NO")
                    {
                        _TCCtrippleDisk = false;
                    }
                    else
                    {
                        _TCCtrippleDisk = true;
                    }
                    OnPropertyChanged(nameof(TDCC));
                    CalculateTCCdata();
                }
            }
        }
        [XmlIgnore]
        public BindingList<int> MCT
        {
            get
            {
                BindingList<int> tps = new BindingList<int>();
                tps.Add(30);
                tps.Add(35);
                tps.Add(40);
                tps.Add(45);
                tps.Add(50);
                tps.Add(55);
                tps.Add(60);
                tps.Add(65);
                tps.Add(70);
                return tps;
            }

        }
        [XmlIgnore]
        public BindingList<int> WTT
        {
            get
            {
                if (MaxCruiseThrottle > 70) MaxCruiseThrottle = 70;
                int val = MaxCruiseThrottle;
                BindingList<int> wtt = new BindingList<int>();
                while(val < 100)
                {
                    val += 5;
                    if(val > 100) val = 100;
                    wtt.Add(val);
                }
                return wtt;
            }
        }
        [XmlIgnore]
        public BindingList<string> YN
        {
            get
            {
                BindingList<string> yn = new BindingList<string>();
                yn.Add("YES");
                yn.Add("NO");
                return yn;
            }
        }
        [XmlIgnore]
        public BindingList<TCCpoint> TCCstrategy
        {
            get { return _TCCstrategy; }
            set
            {
                if (_TCCstrategy != value)
                {
                    _TCCstrategy = value;
                    OnPropertyChanged(nameof(TCCstrategy));
                }
            }
        }
        [XmlIgnore]
        public int MaxMPH
        {
            get
            {
                if ((_gears == null) || (_gears.Count == 0)) return 0;
                return Gears[NumberOfGears - 1].MaxSpeed;
            }
        }
        #endregion

        #region private methods

        private void TmpG_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            CalculateGears();
        }

        private void FormatShiftTable()
        {
            int scheduleCount = 2 * (NumberOfGears - 1);
            ShiftSchedule SS;
            ShiftPoint sp;
            _ThrottlePositions = new List<int>();
            ShiftSchedules = new BindingList<ShiftSchedule>();
            for (int i = 0; i < scheduleCount; i++)
            {
                SS = new ShiftSchedule() { LowGearNumber = (i / 2) + 1, HighGearNumber = ((i / 2) + 1) + 1, ShiftDirection = (i % 2 == 0) ? 1 : -1 };
                SS.ShiftPoints = new BindingList<ShiftPoint>();
                for (int e = 0; e < _TableSize; e++)
                {
                    sp = new ShiftPoint();
                    sp.ThrottlePosition = (int)(((float)100 / (_TableSize - 1)) * e);
                    SS.ShiftPoints.Add(sp);
                    if((i==0)) _ThrottlePositions.Add(sp.ThrottlePosition);
                }
                ShiftSchedules.Add(SS);
            }
        }
        private void CalculateGears()
        {
            int smph;
            int pi = 0;
            if ((_gears == null) || (_gears.Count == 0)) return;
            foreach(var gear in Gears)
            {
                gear.MaxSpeed = CalculateMPH(MaxRPM, gear.Ratio);
                gear.CruiseSpeed = CalculateMPH(ShiftRPM, gear.Ratio);
                gear.MaxShiftSpeed = CalculateMPH(WOTrpm, gear.Ratio);
                gear.MinSpeed = CalculateMPH(MinRPM, gear.Ratio);
            }
            foreach(var schedule in ShiftSchedules)
            {
                pi = 0;
                foreach(var point in schedule.ShiftPoints)
                {
                    if((schedule.LowGearNumber == 1) && (schedule.ShiftDirection > 0))
                    {
                        smph = (int)(CalculateMPH(MinRPM, Gears[schedule.HighGearNumber-1].Ratio));
                        if(pi>0) smph = (int)(schedule.ShiftPoints[pi - 1].Mph + (schedule.ShiftPoints[pi-1].Mph*Gain));
                        if (smph< CalculateMPH(WOTrpm, Gears[schedule.LowGearNumber - 1].Ratio))
                        {
                            point.Mph = smph;
                        }
                        else
                        {
                            point.Mph = CalculateMPH(WOTrpm, Gears[schedule.LowGearNumber - 1].Ratio);
                        }
                        point.Rpm = CalculateRPM(point.Mph, Gears[schedule.LowGearNumber - 1].Ratio);
                    }
                    else if((schedule.LowGearNumber < Gears.Count) && (schedule.ShiftDirection > 0))
                    {
                        smph = (int)(CalculateMPH(MinRPM, Gears[schedule.HighGearNumber - 1].Ratio));
                        if (pi > 0) smph = (int)(schedule.ShiftPoints[pi - 1].Mph + (schedule.ShiftPoints[pi - 1].Mph * Gain));
                        if (smph < CalculateMPH(WOTrpm, Gears[schedule.LowGearNumber - 1].Ratio))
                        {
                            point.Mph = smph;
                        }
                        else
                        {
                            point.Mph = CalculateMPH(WOTrpm, Gears[schedule.LowGearNumber - 1].Ratio);
                        }
                        point.Rpm = CalculateRPM(point.Mph, Gears[schedule.LowGearNumber - 1].Ratio);
                    }
                    else if ((schedule.LowGearNumber >= 1) && (schedule.ShiftDirection < 0))
                    {
                        smph = GetMPHforUpShift(schedule.LowGearNumber, pi);
                        smph -= (int)(smph * Decay);
                        if (smph > Gears[schedule.LowGearNumber].MaxShiftSpeed) smph = Gears[schedule.LowGearNumber].MaxShiftSpeed - 5;
                        if (smph < 10) smph = 10;
                        point.Mph = smph;
                        point.Rpm = CalculateRPM(point.Mph, Gears[schedule.HighGearNumber - 1].Ratio);
                    }
                    pi++;
                }
            }
            OnPropertyChanged(nameof(Gears));
            CalculateTCCdata();
        }
        private int CalculateMPH(int RPM, float GearRatio)
        {
            int rtn = 0;

            rtn = (int)((2 * Math.PI * (TireDiameterInches / 2)) * ((RPM / GearRatio) / DiffRatio)) / (63360 / 60);
            return rtn;
        }
        private int CalculateRPM(int MPH, float GearRatio)
        {
            int rtn = 0;

            rtn = (int)(((float)MPH * ((float)63360/60)) / (2 * Math.PI * (TireDiameterInches / 2)) * DiffRatio * GearRatio);
            return rtn;
        }
        private int GetMPHforUpShift(int Gear, int Point)
        {
            int rtn = 0, pti = 0;
            foreach(var sch in ShiftSchedules)
            {
                if((sch.LowGearNumber == Gear) && (sch.ShiftDirection > 0))
                {
                    pti = 0;
                    foreach(var pt in sch.ShiftPoints)
                    {
                        if(pti == Point)
                        {
                            rtn = pt.Mph;
                            return rtn;
                        }
                        pti++;
                    }
                }
            }

            return rtn;
        }

        private void CalculateTCCdata()
        {
            // Pre check all values for validity
            if((Gears == null) || (Gears.Count == 0) || (ShiftSchedules.Count < 1))
            {
                return;
            }
            if (MinTCCgear < 1)
            {
                MinTCCgear = 1;
                return;
            }
            if (StallSpeed < 1000)
            {
                StallSpeed = 1000;
                return;
            }
            if (MaxCruiseThrottle < 25)
            {
                MaxCruiseThrottle = 25;
                return;
            }
            if (MaxCruiseThrottle > 70)
            {
                MaxCruiseThrottle = 70;
                return;
            }
            if (WOTtransitionThrottle < 50)
            {
                WOTtransitionThrottle = 50;
                return;
            }
            if ((WOTtransitionThrottle > 100) || (WOTtransitionThrottle < 50))
            {
                WOTtransitionThrottle = 75;
                return;
            }
            if ((MaxCruiseThrottle + 10) > WOTtransitionThrottle)
            {
                MaxCruiseThrottle = WOTtransitionThrottle - 10;
                return;
            }
            if (MaxCruiseThrottle < 30)
            {
                MaxCruiseThrottle = 30;
                return;
            }
            if(MinRPM < 700)
            {
                MinRPM = 850;
                return;
            }
            if((ShiftRPM -100) < MinRPM)
            {
                ShiftRPM = MinRPM + 100;
                return;
            }
            if(TableSize < 4)
            {
                TableSize = 16;
                return;
            }
            if(_ThrottlePositions.Count < 2)
            {
                _ThrottlePositions = new List<int>();
                foreach(var sp in ShiftSchedules[0].ShiftPoints)
                {
                    _ThrottlePositions.Add(sp.ThrottlePosition);
                }
            }

            int cruiseLock = CalculateMPH(ShiftRPM, Gears[(MinTCCgear - 1)].Ratio);
            int cruiseUnlock = CalculateMPH(MinRPM, Gears[(MinTCCgear - 1)].Ratio);
            int WOTlock = CalculateMPH(WOTlockRPM, Gears[WOTmaxGear-1].Ratio);
            int WOTUnlock = WOTlock - 5;
            int stallMPH = CalculateMPH(StallSpeed, Gears[WOTmaxGear - 1].Ratio);
            int maxSpeed = CalculateMPH(_maxRPM, _gears[NumberOfGears - 1].Ratio);
            _TCCstrategy = new BindingList<TCCpoint>();
            TCCpoint tCCpoint;
            foreach (var tp in _ThrottlePositions)
            {
                tCCpoint = new TCCpoint() { ThrottlePosition = tp };
                if(tp<= MaxCruiseThrottle)
                {
                    tCCpoint.LockMPH = cruiseLock;
                    tCCpoint.UnlockMPH = cruiseUnlock;
                }
                else if((tp > MaxCruiseThrottle) && (tp < WOTtransitionThrottle))
                {
                    if (TCCtrippleDisk)
                    {
                        tCCpoint.LockMPH = (int)(cruiseLock + ((WOTlock - cruiseLock) * ((float)(tp - MaxCruiseThrottle) / (WOTtransitionThrottle - MaxCruiseThrottle))));
                        tCCpoint.UnlockMPH = tCCpoint.LockMPH - 5; // (int)(cruiseUnlock + ((WOTUnlock - cruiseUnlock) * ((float)(tp - MaxCruiseThrottle) / (WOTtransitionThrottle - MaxCruiseThrottle))));
                        if (tCCpoint.LockMPH < stallMPH)
                        {
                            tCCpoint.LockMPH = stallMPH + 5;
                            tCCpoint.UnlockMPH = stallMPH;
                        }
                        ;
                    }
                    else
                    {
                        tCCpoint.LockMPH = maxSpeed + 10;
                        tCCpoint.UnlockMPH = tCCpoint.LockMPH - 5;
                    }
                }
                else
                {
                    if (TCCtrippleDisk)
                    {
                        tCCpoint.LockMPH = WOTlock;
                        tCCpoint.UnlockMPH = WOTUnlock;
                    }
                    else
                    {
                        tCCpoint.LockMPH = maxSpeed + 10;
                        tCCpoint.UnlockMPH = tCCpoint.LockMPH - 5;
                    }
                }
                if (TCCtrippleDisk)
                {
                    if (tCCpoint.LockMPH > maxSpeed)
                    {
                        tCCpoint.LockMPH = maxSpeed;
                    }
                    if (tCCpoint.UnlockMPH >= maxSpeed)
                    {
                        tCCpoint.UnlockMPH = tCCpoint.LockMPH - 10;
                    }
                }
                _TCCstrategy.Add(tCCpoint);
            }
            OnPropertyChanged(nameof(TCCstrategy));
            SendMessage("TCC Strategy Recalculated", MessageType.DataChanged);
        }

        #endregion 


    }

    public class Gear : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private int _gearNumber;
        private float _ratio;
        private int _MinSpeed;
        private int _CruiseSpeed;
        private int _MaxShiftSpeed;
        private int _MaxSpeed;

        public int GearNumber
        {
            get { return _gearNumber; }
            set
            {
                if (_gearNumber != value)
                {
                    _gearNumber = value;
                    OnPropertyChanged(nameof(GearNumber));
                }
            }
        }
        public float Ratio
        {
            get { return _ratio; }
            set
            {
                if (_ratio != value)
                {
                    _ratio = value;
                    OnPropertyChanged(nameof(Ratio));
                }
            }
        }
        public int MinSpeed
        {
            get { return _MinSpeed;}
            set
            {
                if(_MinSpeed != value)
                {
                    _MinSpeed = value;
                    OnPropertyChanged(nameof(MinSpeed));
                }
            }
        }
        public int CruiseSpeed
        {
            get { return _CruiseSpeed; }
            set
            {
                if(_CruiseSpeed != value)
                {
                    _CruiseSpeed = value;
                    OnPropertyChanged(nameof(CruiseSpeed));
                }
            }
        }
        public int MaxShiftSpeed
        {
            get { return _MaxShiftSpeed; }
            set
            {
                if(_MaxShiftSpeed != value)
                {
                    _MaxShiftSpeed = value;
                    OnPropertyChanged(nameof(MaxShiftSpeed));
                }
            }
        }
        public int MaxSpeed
        {
            get { return _MaxSpeed; }
            set
            {
                if(_MaxSpeed != value)
                {
                    _MaxSpeed = value;
                    OnPropertyChanged(nameof(MaxSpeed));
                }
            }
        }
    }
    public class ShiftSchedule : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _LowGearNumber;
        private int _HighGearNumber;
        private int _ShiftDirection; // 1 for upshift, -1 for downshift
        private string _scheduleName;   
        private BindingList<ShiftPoint> _shiftPoints = new BindingList<ShiftPoint>();

        public string ScheduleName
        {
            get
            {
                if(ShiftDirection == 1)
                {
                    return $"{_LowGearNumber} > {_HighGearNumber}";
                }
                else
                {
                    return $"{_HighGearNumber} > {_LowGearNumber}";
                }
                return _scheduleName; 
            }
        }
        public int LowGearNumber
        {
            get { return _LowGearNumber; }
            set
            {
                if (_LowGearNumber != value)
                {
                    _LowGearNumber = value;
                    OnPropertyChanged(nameof(LowGearNumber));
                }
            }
        }
        public int HighGearNumber
        {
            get { return _HighGearNumber; }
            set
            {
                if (_HighGearNumber != value)
                {
                    _HighGearNumber = value;
                    OnPropertyChanged(nameof(HighGearNumber));
                }
            }
        }
        public int ShiftDirection
        {
            get { return _ShiftDirection; }
            set
            {
                if (_ShiftDirection != value)
                {
                    _ShiftDirection = value;
                    OnPropertyChanged(nameof(ShiftDirection));
                }
            }
        }
        public BindingList<ShiftPoint> ShiftPoints
        {
            get { return _shiftPoints; }
            set
            {
                if (_shiftPoints != value)
                {
                    _shiftPoints = value;
                    OnPropertyChanged(nameof(ShiftPoints));
                }
            }
        }

    }
    public class ShiftPoint : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _ThrottlePosition;
        private int _rpm;
        private int _mph;

        public int ThrottlePosition
        {
            get { return _ThrottlePosition; }
            set
            {
                if (_ThrottlePosition != value)
                {
                    _ThrottlePosition = value;
                    OnPropertyChanged(nameof(ThrottlePosition));
                }
            }
        }
        public int Rpm
        {
            get { return _rpm; }
            set
            {
                if (_rpm != value)
                {
                    _rpm = value;
                    OnPropertyChanged(nameof(Rpm));
                }
            }
        }
        public int Mph
        {
            get { return _mph; }
            set
            {
                if (_mph != value)
                {
                    _mph = value;
                    OnPropertyChanged(nameof(Mph));
                }
            }
        }


    }
    public class TCCpoint : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _ThrottlePosition;
        private int _LockMPH;
        private int _UnlockMPH;

        public int ThrottlePosition
        {
            get { return _ThrottlePosition; }
            set
            {
                if (_ThrottlePosition != value)
                {
                    _ThrottlePosition = value;
                    OnPropertyChanged(nameof(ThrottlePosition));
                }
            }
        }
        public int LockMPH
        {
            get { return _LockMPH; }
            set
            {
                if (_LockMPH != value)
                {
                    _LockMPH = value;
                    OnPropertyChanged(nameof(LockMPH));
                }
            }
        }
        public int UnlockMPH
        {
            get { return _UnlockMPH; }
            set
            {
                if (_UnlockMPH != value)
                {
                    _UnlockMPH = value;
                    OnPropertyChanged(nameof(UnlockMPH));
                }
            }
        }
    }

}
