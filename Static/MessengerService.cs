using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomotiveBuilder.Static
{
    public static class MessengerService
    {
            // Event that subscribers (like UI components) listen to
            public static event Action<string, MessageType>? MessageReceived;

            // Method anyone can call to 'sink' a message into the application bus
            public static void LogMessage(string message, MessageType type = MessageType.Info)
            {
                // Invoke the event on the UI thread to ensure WPF stability
                App.Current.Dispatcher.Invoke(() => MessageReceived?.Invoke(message, type));
            }

        public enum MessageType { Info, Warning, Error, DataChanged }
        
    }
}
