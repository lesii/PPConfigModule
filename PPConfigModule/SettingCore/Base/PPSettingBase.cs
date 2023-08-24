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

        public bool Save(string _inConfigFileName = "", string _inConfigFilePath = "", bool bOverrideExist = true)
        {
            return Setting.SaveProjectSetting(this, _inConfigFileName, _inConfigFilePath, bOverrideExist);
        }

        public override string ToString()
        {
            PPCfgSection section = Setting.BuildConfigSection(this);

            return section.ToString();
        }


    }

    public class TPPSettingBase<TC> : PPSettingBase where TC : TPPSettingBase<TC>
    {
        public static TC Load(string settingName = "", string inConfigFileName = "", string inConfigFilePath = "")
        {
            return Setting.Load<TC>(inConfigFileName, inConfigFilePath);

        }
    }
    
}



