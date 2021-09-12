using System;
using System.Collections.Generic;
using System.Text;

namespace C4.ImaginingNetCore.Notifier.Logging.Response
{
    public class ErrorResponse : Status
    {
        public string ErrorMessage { get; private set; }

        public ErrorResponse()
        {
            IsSuccess = false;
            ErrorMessage = "There has been an error";
        }
    }
}
