using System;
using System.Reflection;

namespace PPExtensionModule
{
    public static class Setting
    {
        private const BindingFlags InstanceBindFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public static PPSettingBase GetProjectSetting(string _settingName, string inConfigFileName = "", string inConfigFilePath = "")
        {
            return GetProjectSetting<PPSettingBase>(_settingName, inConfigFileName, inConfigFilePath);
        }


        public static bool SaveProjectSetting(PPSettingBase _setting, string inConfigFileName = "", string inConfigFilePath = "", bool bOverrideExist = true)
        {
            PPCfgSection section = BuildConfigSection(_setting);
            
            return Config.SaveSection(section, inConfigFileName, inConfigFilePath, bOverrideExist);
        }
        

        public static T GetProjectSetting<T>(string _settingName, string inConfigFileName = "", string inConfigFilePath = "") where T : PPSettingBase
        {
            PPCfgSection secData = new PPCfgSection();
            secData.sectionName = _settingName;

            if (!Config.GetConfigSection(ref secData, inConfigFileName, inConfigFilePath)) return null;

            return CreateSettingInstance<T>(secData);
        }

        public static PPCfgSection BuildConfigSection<T>(T _inSetting) where T : PPSettingBase
        {
            PPCfgSection createdSection = new PPCfgSection();
            createdSection.sectionName = _inSetting.GetSettingName();
            var fields = _inSetting.GetType().GetFields(InstanceBindFlags);

            foreach (var item in fields)
            {
                string key = item.Name;
                string value = item.GetValue(_inSetting).ToString();
                createdSection.AddContent(key, value);
            }

            return createdSection;
        }

        public static T CreateSettingInstance<T>(PPCfgSection _inSection) where T : PPSettingBase
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            object obj = assembly.CreateInstance(_inSection.sectionName);

            if (obj == null) return null;

            var fields = obj.GetType().GetFields(InstanceBindFlags);

            foreach (var item in fields)
            {
                string keyName = item.Name;

                string _res = "";
                if (_inSection.TryGetPairValue(keyName, ref _res))
                {
                    item.SetValue(obj, Convert.ChangeType(_res, item.FieldType));
                }
            }
            return (T)obj;
        }
    }

}