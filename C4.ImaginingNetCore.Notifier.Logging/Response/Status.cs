using System;
using System.Collections.Generic;
using System.Text;

namespace C4.ImaginingNetCore.Notifier.Logging.Response
{
    public abstract class Status
    {
        public bool IsSuccess { get; internal set; }
    }
}
