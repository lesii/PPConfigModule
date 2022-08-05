# Config Core v1.0

展示了直接使用Config进行项目配置表导入的方法。使用静态类PPExtensionModule.Config，调用静态方法。

在[这里](../README.md)返回模组功能说明。

## 数据结构

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

## 接口介绍

+ 获取配置表数据

  + 说明

    >获取配置表数据结构

  + 接口调用

    ```C#
    public static bool GetConfig(ref PPCfgData outItem, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + 参数
  
    |      参数名      |         参数意义         |
    |:----------------:|:------------------------:|
    | inConfigFileName | 文件名，默认PPConfig.ini |
    | inConfigFilePath |  文件路径，默认/PPConfig |

  + 返回
    |   返回数据  |    参数意义    |
    |:-----------:|:--------------:|
    | ref outItem | 获取返回的结果 |
    | return bool |    查询成功    |

+ 获取键值对
  + 说明
    >获取配置表中某个段落键值对
  + 接口调用

    ```C#
    public static bool GetConfigPairs(ref Dictionary<string, string> pairs, string inSectionName,     string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + 参数
  
    |      参数名      |     参数意义     |
    |:----------------:|:----------------:|
    |   inSectionName  |段落名称                |
    | inConfigFileName |文件名，默认PPConfig.ini|
    | inConfigFilePath |文件路径，默认/PPConfig |

  + 返回
    |  返回数据 |    参数意义    |
    |:---------:|:--------------:|
    | ref pairs | 返回配置表列表 |
    |return bool |返回查询结果          |

+ 获取段落
  + 说明
    >根据传递的段落，填入对应段落的数据，在PPCfgSection中sectionName中填入对应需要查询的段落名称。
  + 接口调用

    ```C#
    public static bool GetConfigSection(ref PPCfgSection section, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + 参数
  
    |      参数名      |         参数意义         |
    |:----------------:|:------------------------:|
    | inConfigFileName | 文件名，默认PPConfig.ini |
    | inConfigFilePath |  文件路径，默认/PPConfig |

  + 返回
    |   返回数据  |   参数意义   |
    |:-----------:|:------------:|
    | ref section | 填入段落信息 |
    | return bool |   查询成功   |

+ 获取所有段落名
  + 说明
    >获取某个配置表中所有段落名称
  + 接口调用

    ```C#
    public static bool GetAllSectionNames(ref List<string> outSectionNames, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + 参数
  
    |      参数名      |         参数意义         |
    |:----------------:|:------------------------:|
    | inConfigFileName | 文件名，默认PPConfig.ini |
    | inConfigFilePath |  文件路径，默认/PPConfig |

  + 返回
    |       返回数据      |      参数意义      |
    |:-------------------:|:------------------:|
    | ref outSectionNames | 获取返回段落名容器 |
    |     return bool     |      查询成功      |

+ 获取段落数量
  + 说明
    >查询当前配置表中段落数量
  + 接口调用

    ```C#
    public static int GetConfigSectionNum(string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + 参数
  
    |      参数名      |         参数意义         |
    |:----------------:|:------------------------:|
    | inConfigFileName | 文件名，默认PPConfig.ini |
    | inConfigFilePath |  文件路径，默认/PPConfig |

  + 返回
    |  返回数据  |   参数意义    |
    |:----------:|:-----------:|
    | return int | 查询成功     |

+ 获取段落中某项
  + 说明
    >根据传递的段落名，KeyName获取Value
  + 接口调用

    ```C#
    public static bool GetConfigItem(ref string outItem, string inSectionName, string inKeyName, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + 参数
  
    |      参数名      |         参数意义         |
    |:----------------:|:------------------------:|
    |   inSectionName  |          段落名          |
    |     inKeyName    |           键名           |
    | inConfigFileName | 文件名，默认PPConfig.ini |
    | inConfigFilePath |  文件路径，默认/PPConfig |

  + 返回
    |   返回数据  |    参数意义    |
    |:-----------:|:--------------:|
    | ref outItem | 获取返回的结果 |
    | return bool |    查询成功    |

+ 保存段落
  + 说明
    >将当前的段落数据结构PPCfgSection保存进配置表文件中
  + 接口调用

    ```C#
    public static bool SaveSection(PPCfgSection sectionData, string inConfigFileName = "", string inConfigFilePath = "", bool bOverrideExist = true);
    ```

  + 参数
  
    |      参数名      |         参数意义         |
    |:----------------:|:------------------------:|
    |    sectionData   |         段落数据         |
    | inConfigFileName | 文件名，默认PPConfig.ini |
    | inConfigFilePath |  文件路径，默认/PPConfig |
    |  bOverrideExist  |      覆盖已存在段落      |

  + 返回
    |   返回数据  |    参数意义    |
    |:-----------:|:--------------:|
    | return bool |    查询成功    |

+ 保存配置表数据
  + 说明
    >将当前的配置表数据结构PPCfgData保存进配置表文件中
  + 接口调用

    ```C#
    public static bool SaveConfig(PPCfgData cfgData, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + 参数
  
    |      参数名      |         参数意义         |
    |:----------------:|:------------------------:|
    |      cfgData     |        配置表数据        |
    | inConfigFileName | 文件名，默认PPConfig.ini |
    | inConfigFilePath |  文件路径，默认/PPConfig |

  + 返回
    |   返回数据  |    参数意义    |
    |:-----------:|:--------------:|
    | return bool |    查询成功    |

## 结束

[返回](../README.md)返回模组功能说明。
