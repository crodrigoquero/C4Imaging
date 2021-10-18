using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.States.Kernel.Models
{
    public class Skill
    {
        // the following fields group & properties only make sense if a skill-plugin has been loaded
        private string _description = "N/A";
        private string _skillGuid = "N/A";
        private string _contributor = "N/A";   // name & surname of the skill developer
        private string _category = "N/A";      // individual category or categories hierarchy separated by the "\" char 
        private string _status = "Not Loaded";

        // all fileds must be informed. Throw an exception in any og them is null / empty
        public Skill(string description, string skillGuid, string contributor, string category, string status)
        {
            // check for null / empty values and throw an exception if needed
            // (if any param null, it means something went wrong when the skill plugin was loaded):

            if (description == string.Empty) throw new Exception("description can't be null");
            if (skillGuid == string.Empty) throw new Exception("skillGuid can't be null");
            if (contributor == string.Empty) throw new Exception("contributor can't be null");
            if (category == string.Empty) throw new Exception("category can't be null");
            if (status == string.Empty) throw new Exception("status can't be null");

            _description = description;
            _skillGuid = skillGuid;
            _contributor = contributor;   
            _category = category;      
            _status = status;

    }

        public string SkillDescription => _description;
        public string SkillGuid => _skillGuid;  
        public string SkillContributor => _contributor; 
        public string SkillCategory => _category; 
        public string SkillStatus => _status; 
    
    }
}
