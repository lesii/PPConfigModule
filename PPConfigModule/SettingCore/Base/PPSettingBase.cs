using PPExtensionModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPExtensionModule 
{
    public class PPSettingBase
    {

        public virtual string GetSettingName()
        {
            Type myType = GetType();
            return myType.Name;
        }

        public bool SaveTo(string _inConfigFileName = "", string _inConfigFilePath = "", bool bOverrideExist = true)
        {
            PPCfgSection section = Setting.BuildConfigSection(this);

            return Config.SaveSection(section, _inConfigFileName, _inConfigFilePath, bOverrideExist);
        }

        public override string ToString()
        {
            PPCfgSection section = Setting.BuildConfigSection(this);

            return section.ToString();
        }
    }
}



