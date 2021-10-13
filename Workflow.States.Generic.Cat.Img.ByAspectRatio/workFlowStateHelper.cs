using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Workflow.States.Generic.Cat.Img.ByAspectRatio
{
    public class WorkFlowStateHelper
    {
        private string _workflowStateInboxPath;
        public WorkFlowStateHelper(string workflowStateInboxPath)
        {
            _workflowStateInboxPath = workflowStateInboxPath;
        }

        public bool DiretoryPathIsTheOutputOfToWorkflowState(string diretoryPath, int workFlowStateNumber)
        {
            string[] directoryList = GetworkFlowStateOutputDirectories(workFlowStateNumber, true);
            foreach (string fileEntry in directoryList)
            {
                if (fileEntry.ToString() == diretoryPath)
                {
                    return true;
                }
            }

            return false;
        }

        public int GetWorkFlowStateOrderNumberFromDirectoryPath(string diretoryPath)
        {
            diretoryPath = diretoryPath.Replace(@_workflowStateInboxPath, "");
            return diretoryPath.Split(@"\").Length - 1;
        }

        public bool WorkFlowStateHasSubdirectories(int workFlowStateNumber)
        {
            if (GetworkFlowStateOutputDirectories(workFlowStateNumber).Length == 0)
            {
                return false;
            }

            return true;
        }

        public string[] GetworkFlowStateInputDirectories(int workFlowStateNumber, bool getFullPath = false)
        {
            string[] directories = GetworkFlowStateOutputDirectories(workFlowStateNumber - 1, getFullPath);
            if (directories.Length == 0)
            {
                Array.Resize(ref directories, 1);
                directories[0] = _workflowStateInboxPath;
            }

            return directories;
        }

        public string[] GetworkFlowStateOutputDirectories(int workFlowStateNumber, bool GetFullPath = false)
        {
            List<WorkFlowStateDirectory> workFlowStateDirectories = new List<WorkFlowStateDirectory>();

            string[] fileEntries = Directory.GetDirectories(@_workflowStateInboxPath, "*.*", SearchOption.AllDirectories);
            foreach (string fileEntry in fileEntries) // get all the files from the inbox ...
            {
                WorkFlowStateDirectory workFlowStateDirectory = new WorkFlowStateDirectory();
                workFlowStateDirectory.Path = fileEntry.ToString().Replace(@_workflowStateInboxPath, "");
                workFlowStateDirectory.Level = workFlowStateDirectory.Path.Split(@"\").Length - 1;

                workFlowStateDirectories.Add(workFlowStateDirectory);

            }

            if (GetFullPath)
            {
                //concatenate the full path
                foreach (WorkFlowStateDirectory dir in workFlowStateDirectories)
                {
                    dir.Path = _workflowStateInboxPath + dir.Path;
                }
            }

            return workFlowStateDirectories.Where(d => d.Level == workFlowStateNumber).Select(x => x.Path).ToArray();
        }
    }
}
