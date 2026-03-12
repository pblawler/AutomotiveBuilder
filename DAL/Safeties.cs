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


    }


}
