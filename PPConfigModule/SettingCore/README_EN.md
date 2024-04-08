# Setting Core v1.2

The Setting module can directly convert the configuration table file into the corresponding object form of the PPSettingBase class using the matching API in the project, and generate it using reflection.

Using the static class PPExtensionModule Setting, calling static methods.

[Here](../../README_EN.md) to return to the module function description.

## Data Structure

## Interface API

+ Get project settings
  + Description
    >Load Setting Object Refernce by setting name to obtain the project setting base object PPSettingBase
  + Interface Call
  
    ```C#
    public static PPSettingBase GetProjectSetting(string _settingName, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + Parameters
    |      Param       |           Significance         |
    |:----------------:|:------------------------------:|
    |   _settingName   |  SettingName,Equal SectionName |
    | inConfigFileName | FileName ,Defualt PPConfig.cfg |
    | inConfigFilePath |  File path，Defualt ./PPConfig |
  + Return
    |         return       |       Significance    |
    |:--------------------:|:---------------------:|
    | return PPSettingBase |  return setting class |

+ Get project settings (generic)
  + Description
    >By the setting name, load the  Setting Object Instance  subclass object was subclass of PPSettingBase and convert it according to the T type.
  + Interface Call
  
    ```C#
    public static T GetProjectSetting<T>(string _settingName, string inConfigFileName = "", string inConfigFilePath = "") where T : PPSettingBase;
    ```

  + Parameters
    |      Param      |         Significance         |
    |:----------------:|:------------------------:|
    |   _settingName   |  SettingName,Equal SectionName  |
    | inConfigFileName | FileName ,Defualt PPConfig.cfg |
    | inConfigFilePath |  File path，Defualt ./PPConfig |
  + Return
    |         return       |  Significance  |
    |:--------------------:|:--------------:|
    | return PPSettingBase | Setting Object |

+ Save Setting
  + Description
    >Save the project settings PPSettingBase object to the configuration table file
  + Interface Call
  
    ```C#
     public static bool SaveProjectSetting(PPSettingBase _setting, string inConfigFileName = "", string inConfigFilePath = "", bool bOverrideExist = true);
    ```

  + Parameters
  
    |      Param       |           Significance         |
    |:----------------:|:------------------------------:|
    |    _setting      |  Set setting object content    |
    | inConfigFileName | FileName ,Defualt PPConfig.cfg |
    | inConfigFilePath |  File path，Defualt ./PPConfig |
    |  bOverrideExist  |      override section          |

  + Return
    |     return  |  Significance  |
    |:-----------:|:--------------:|
    | return bool |    result      |

+ Build project settings paragraph (generic)
  + Description
    >Build project settings into section data PPCfgSection.
  + Interface Call
  
    ```C#
     public static PPCfgSection BuildConfigSection<T>(T _inSetting) where T : PPSettingBase;
    ```

  + Parameters
  
    | Param |   Significance   |
    |:------:|:------------:|
    |    T   | Setting Object Ref |

  + Return
    |         return      |   Significance   |
    |:-------------------:|:----------------:|
    | return PPCfgSection | Return Section   |

+ Generate setting instances (generic)
  + Description
    >Generate corresponding setting object instances for section data structures
  + Interface Call
  
    ```C#
     public static T CreateSettingInstance<T>(PPCfgSection _inSection) where T : PPSettingBase;
    ```

  + Parameters
  
    |    Param     |   Significance     |
    |:------------:|:------------------:|
    | PPCfgSection | Setting Object Ref |

  + Return
    |   return    |     Significance    |
    |:-----------:|:-------------------:|
    | return T    | Get Object Instance |

## End

[Here](../../README_EN.md) to return to the module function description.
