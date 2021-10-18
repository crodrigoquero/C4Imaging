using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.States.Kernel.Models
{
    public class Status
    {
        private  List<string> _managedDirectories;
        private  bool _configured = false;
        private  DateTime _startDateTime;
        private  DateTime _endDateTime;
        private  int _totalFiles = 0;
        private  string _sessionGuid;

        public Status()
        {
            // assign session guid here
            _sessionGuid = Guid.NewGuid().ToString();
        }

        public List<string> ManagedDirectories { get => _managedDirectories; set { _managedDirectories = value; } }
        public bool Configured { get => _configured; set { _configured = value; } }
        public DateTime StartDateTime { get => _startDateTime; set { _startDateTime = value; } }
        public DateTime EndDateTime { get => _endDateTime; set { _endDateTime = value; } }
        public int TotalFiles { get => _totalFiles; set { _totalFiles = value; } }
        public string SessionGuid => _sessionGuid;

    }
}
