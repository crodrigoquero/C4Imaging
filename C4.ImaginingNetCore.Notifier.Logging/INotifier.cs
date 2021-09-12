using C4.ImaginingNetCore.Notifier.Logging.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace C4.ImaginingNetCore.Notifier.Logging
{
    public interface INotifier
    {
        Status SendNotification(string message, int eventId);
        Status SendError(string message, int eventId);
        Status SendWarning(string message, int eventId);
        Status SendTrace(string message, int eventId);
        Status SendCritical(string message, int eventId);
    }
}
