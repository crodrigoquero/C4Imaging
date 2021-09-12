using System;
using System.Collections.Generic;
using System.Text;

namespace C4.ImaginingNetCore.Notifier.Logging.Response
{
    public class SuccessResponse : Status
    {
        public SuccessResponse()
        {
            IsSuccess = true;
        }
    }
}
