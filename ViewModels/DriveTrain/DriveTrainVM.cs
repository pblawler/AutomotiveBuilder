using AutomotiveBuilder.DAL;
using AutomotiveBuilder.Properties;
using AutomotiveBuilder.Static;
using AutomotiveBuilder.Statics;
using AutomotiveBuilder.Views;
using AutomotiveBuilder.Views.PartUtils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Documents;
using System.Xml.Serialization;
using static AutomotiveBuilder.Static.MessengerService;

namespace AutomotiveBuilder.ViewModels.DriveTrain
{
    public partial class DriveTrainVM : ObservableObject
    {
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
                            OnPropertyChanged(nameof(StaticMethods.PartCatagories));
                            break;

                            case "TCC Strategy Recalculated":
                                UpdateChart();
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

        #region Constructor
        
        public DriveTrainVM()
        {

            MessengerService.MessageReceived += OnNewMessage;
            DriveTrainData = new DTM();
            DriveTrainData.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(DriveTrainData.TCCstrategy))
                {
                    UpdateChart();
                }
            };
        }

        #endregion

        #region Private Members
        private DTM _DriveTrainData;
        private BindingList<int> _WheelDiameters = new BindingList<int>() {12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
        private BindingList<int> _NoGears = new BindingList<int>() { 2, 3, 4, 5, 6, 7, 8, 9, 10};
        private int _TransGears = 4;
        private OxyPlot.PlotModel _TCCplotModel;

        #endregion

        #region Properties
        public BindingList<int> WheelDiameters
        {
            get { return _WheelDiameters; }
            set
            {
                if (_WheelDiameters != value)
                {
                    _WheelDiameters = value;
                    OnPropertyChanged(nameof(WheelDiameters));
                }
            }
        }
        public BindingList<int> NoGears
        {
            get { return _NoGears; }
            set
            {
                if (_NoGears != value)
                {
                    _NoGears = value;
                    OnPropertyChanged(nameof(NoGears));
                }
            }
        }
        public DTM DriveTrainData
        {
            get { return _DriveTrainData; }
            set
            {
                if (_DriveTrainData != value)
                {
                    _DriveTrainData = value;
                    OnPropertyChanged(nameof(DriveTrainData));
                    UpdateChart();
                }
            }
        }
        public OxyPlot.PlotModel TCCplotModel
        {
            get { return _TCCplotModel; }
            set
            {
                if (_TCCplotModel != value)
                {
                    _TCCplotModel = value;
                    OnPropertyChanged(nameof(TCCplotModel));
                }
            }
        }

        #endregion

        #region Relay Commands
        //private RelayCommand<object> _AddProject;


        //public RelayCommand<object> AddProjectCommand
        //{
        //    get
        //    {
        //        return _AddProject ?? (_AddProject = new RelayCommand<object>((X) => AddProject(X)));
        //    }
        //}

        //private void AddProject(object Args)
        //{

        //    RaisePropertyChanged(nameof(Args.ToString));
        //    UpdateAllProperties();
        //}


        #endregion

        #region PropertyEvents

        #endregion

        #region Private Methods
        private void UpdateChart()
        {
            if (_DriveTrainData == null) return;
            _TCCplotModel = new OxyPlot.PlotModel();
            _TCCplotModel.Title = "TCC Strategy";
            _TCCplotModel.TextColor = OxyColors.Black;
            _TCCplotModel.Background = OxyColors.CadetBlue;
            _TCCplotModel.Legends.Add(new Legend()
            {
                LegendBorder = OxyColors.Black,
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.TopRight,
                LegendFontWeight = FontWeights.Bold
            });

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Throttle Position (%)",
                TitleFontWeight = 700,
                MajorStep = 5,
                MinorStep = 1
            };
            _TCCplotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Speed (MPH)",
                Maximum = _DriveTrainData.MaxMPH + 20,
                Minimum = 0,
                TitleFontWeight = 700,
                MajorStep = 5,
                MinorStep = 1
            };            
            _TCCplotModel.Axes.Add(yAxis);
            
            LineSeries TCC_Lock = new LineSeries
            {
                Title = "TCC Lock",
                LabelFormatString = "{1:0}",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4
            };
            LineSeries TCC_Unlock = new LineSeries
            {
                Title = "TCC Unlock",
                LabelFormatString = "{1:0}",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4
            };
            
            int pointIndex = 0;
            foreach (var TCCpt in _DriveTrainData.TCCstrategy)
            {
                TCC_Lock.Points.Add(new DataPoint(TCCpt.ThrottlePosition, TCCpt.LockMPH));
                TCC_Unlock.Points.Add(new DataPoint(TCCpt.ThrottlePosition, TCCpt.UnlockMPH));
                pointIndex++;
            }
            _TCCplotModel.Series.Add(TCC_Lock);
            _TCCplotModel.Series.Add(TCC_Unlock);
            TCCplotModel.InvalidatePlot(true);
            OnPropertyChanged(nameof(TCCplotModel));
        }

        #endregion
    }
}
