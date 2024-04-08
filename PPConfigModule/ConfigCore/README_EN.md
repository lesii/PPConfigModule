# Config Core v1.2

Showcased the method of directly using Configuration to import project configuration tables. Using the static class PPExtensionModule Configure, call static methods.

[Here](../../README_EN.md) to return to the module function description.

## Data Structure

+ PPCfgData

  ```C#
  public struct PPCfgData
  {
    public string configFullName;
    private Dictionary<string, PPCfgSection> m_kv;

    public void CopyTo(ref PPCfgData _other);
    public Dictionary<string, PPCfgSection> GetCopyPairs();
    public void AddSection( PPCfgSection _inSection);
    public bool AddContent(string _sectionName, PPCfgContent _content);
    public bool GetSection(string _sectionName,ref PPCfgSection _outSection);
    public void CreateNewSection(string _sectionName);
    public bool ContainsSection(string _sectionName);
    public void GetAllSectionNames(ref List<string> outSectionNames);
    public override string ToString();
  }
  ```

+ PPCfgSection

  ```C#
  public struct PPCfgSection
  {
    public string sectionName;
    private Dictionary<string, PPCfgContent> m_kv ;

    public void CopyTo(ref PPCfgSection _other);
    public void AddContent(PPCfgContent _inContent);
    public void RemoveContent(PPCfgContent _inContent);
    public void AddContent(string _inKey, string _inValue);
    public void RemoveContent(string _inKey);
    public bool TryGetContent(string _inKey, ref PPCfgContent _outContent);
    public bool TryGetPairValue(string _inKey, ref string _outValue);
    public Dictionary<string, PPCfgContent> GetCopyPairs();
    public override string ToString();
  }
  ```

+ PPCfgContent

  ```C#
  public struct PPCfgContent
  {
    public string Key;
    public string Value;

    public PPCfgContent(string _inKey,string _inValue);
    public void CopyTo(ref PPCfgContent _other);
    public override string ToString();
  }
  ```

## Interface Introduction

+ Get configuration table data

  + Description

    >Get configuration table data structure

  + Interface call

    ```C#
    public static bool GetConfig(ref PPCfgData outItem, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + Parameter
  
    |      Param       |       Significance      |
    |:----------------:|:------------------------:|
    | inConfigFileName | Filename， PPConfig.ini |
    | inConfigFilePath |  File path，  /PPConfig |

  + Return Value
    |   return    |  Significance  |
    |:-----------:|:--------------:|
    | ref outItem |  return value  |
    | return bool |    search ret  |

+ Get Key Value pair
  + Description
    >Get a Section key value pair in the configuration table
  + Interface call

    ```C#
    public static bool GetConfigPairs(ref Dictionary<string, string> pairs, string inSectionName,     string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + Parameter
  
    |      Params      |     Significance     |
    |:----------------:|:--------------------:|
    |   inSectionName  |      SectionName     |
    | inConfigFileName |filename，PPConfig.ini|
    | inConfigFilePath |filepath,  /PPConfig  |

  + Return
    |  ReturnVal |    Significance    |
    |:---------:|:--------------:|
    | ref pairs | Return配置表列表 |
    |return bool |Return查询结果          |

+ Get Section
  + Description
    >According to the transmitted section, fill in the corresponding section data, and fill in the corresponding section name in the sectionName of the PPCfgSection that needs to be queried.
  + Interface call

    ```C#
    public static bool GetConfigSection(ref PPCfgSection section, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + Parameter
  
    |      Params      |         Significance         |
    |:----------------:|:------------------------:|
    | inConfigFileName | filename，默认PPConfig.ini |
    | inConfigFilePath |  filepath,  /PPConfig |

  + Return
    |   ReturnVal  |    Significance   |
    |:------------:|:-----------------:|
    | ref section  |  Back SectionInfo |
    | return bool  |   Search Rslt     |

+ Get all section names
  + Description
    >Get all section names in a configuration table
  + Interface call

    ```C#
    public static bool GetAllSectionNames(ref List<string> outSectionNames, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + Parameter
  
    |      Params      |         Significance         |
    |:----------------:|:------------------------:|
    | inConfigFileName | filename,   PPConfig.ini |
    | inConfigFilePath |  filepath,  /PPConfig    |

  + Return
    |       ReturnVal      |      Significance    |
    |:-------------------:|:------------------:   |
    | ref outSectionNames | Get Section info      |
    |     return bool     |      Search Rslt      |

+ Get the number of section
  + Description
    >Query the number of section in the current configuration table
  + Interface call

    ```C#
    public static int GetConfigSectionNum(string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + Parameter
  
    |      Params      |         Significance     |
    |:----------------:|:------------------------:|
    | inConfigFileName | filename，PPConfig.ini   |
    | inConfigFilePath |  filepath,  /PPConfig    |

  + Return
    |  ReturnVal  |   Significance |
    |:-----------:|:---------------:|
    | return int  | Search Rslt     |

+ Get an item in a section
  + Description
    >Based on the passed section name, KeyName retrieves the Value
  + Interface call

    ```C#
    public static bool GetConfigItem(ref string outItem, string inSectionName, string inKeyName, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + Parameter
  
    |      Params      |         Significance     |
    |:----------------:|:------------------------:|
    |   inSectionName  |       Section Name       |
    |     inKeyName    |         Key Name         |
    | inConfigFileName | filename,   PPConfig.ini |
    | inConfigFilePath |  filepath,  /PPConfig    |

  + Return
    |  ReturnVal  |    Significance   |
    |:-----------:|:-----------------:|
    | ref outItem |  Get Out Reslut   |
    | return bool |    Search Rslt    |

+ Save section
  + Description
    >Save the current section data structure PPCfgSection in the configuration table file
  + Interface call

    ```C#
    public static bool SaveSection(PPCfgSection sectionData, string inConfigFileName = "", string inConfigFilePath = "", bool bOverrideExist = true);
    ```

  + Parameter
  
    |      Params      |       Significance       |
    |:----------------:|:------------------------:|
    |    sectionData   |        saved data        |
    | inConfigFileName | filename，PPConfig.ini   |
    | inConfigFilePath |  filepath,  /PPConfig    |
    |  bOverrideExist  |      override            |

  + Return
    |   ReturnVal  |    Significance    |
    |:-----------:|:--------------:|
    | return bool |    Search Rslt    |

+ Save configuration table data
  + Description
    >Save the current configuration table data structure PPCfgData into the configuration table file
  + Interface call

    ```C#
    public static bool SaveConfig(PPCfgData cfgData, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + Parameter
  
    |      Params      |         Significance    |
    |:----------------:|:-----------------------:|
    |      cfgData     |        config data      |
    | inConfigFileName | filename， PPConfig.ini |
    | inConfigFilePath |  filepath,  /PPConfig   |

  + Return
    |   ReturnVal  |    Significance    |
    |:-----------:|:--------------:|
    | return bool |    Search Rslt    |

## End

[Here](../../README_EN.md) to return to the module function description.
