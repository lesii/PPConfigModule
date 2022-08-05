# Setting Core v1.0

Setting模块可以在项目中使用配套api将配置表文件直接转换成对应的PPSettingBase类的对象形式，使用反射生成。

使用静态类PPExtensionModule.Setting，调用静态方法。

在[这里](../README.md)返回模组功能说明。

## 数据结构介绍

## 接口介绍

+ 获取项目设置
  + 说明
    >通过设置名称加载项目设置对象，获取项目设置基类对象PPSettingBase
  + 接口调用
  
    ```C#
    public static PPSettingBase GetProjectSetting(string _settingName, string inConfigFileName = "", string inConfigFilePath = "");
    ```

  + 参数
    |      参数名      |         参数意义         |
    |:----------------:|:------------------------:|
    |   _settingName   |  设置名称，等同于段落名  |
    | inConfigFileName | 文件名，默认PPConfig.ini |
    | inConfigFilePath |  文件路径，默认/PPConfig |
  + 返回
    |       返回数据       | 参数意义 |
    |:--------------------:|:--------:|
    | return PPSettingBase | 项目设置 |

+ 获取项目设置(泛型)
  + 说明
    >通过设置名称，加载项目设置对象PPSettingBase子类对象,根据T类型进行转换
  + 接口调用
  
    ```C#
    public static T GetProjectSetting<T>(string _settingName, string inConfigFileName = "", string inConfigFilePath = "") where T : PPSettingBase;
    ```

  + 参数
    |      参数名      |         参数意义         |
    |:----------------:|:------------------------:|
    |   _settingName   |  设置名称，等同于段落名  |
    | inConfigFileName | 文件名，默认PPConfig.ini |
    | inConfigFilePath |  文件路径，默认/PPConfig |
  + 返回
    |       返回数据       | 参数意义 |
    |:--------------------:|:--------:|
    | return PPSettingBase | 项目设置 |

+ 保存项目设置
  + 说明
    >将项目设置PPSettingBase对象保存到配置表文件中
  + 接口调用
  
    ```C#
     public static bool SaveProjectSetting(PPSettingBase _setting, string inConfigFileName = "", string inConfigFilePath = "", bool bOverrideExist = true);
    ```

  + 参数
  
    |      参数名      |         参数意义         |
    |:----------------:|:-----------------------:|
    |    _setting      |         设置实例对象     |
    | inConfigFileName | 文件名，默认PPConfig.ini |
    | inConfigFilePath |  文件路径，默认/PPConfig |
    |  bOverrideExist  |      覆盖已存在段落      |

  + 返回
    |   返回数据  |    参数意义    |
    |:-----------:|:--------------:|
    | return bool |    查询成功    |

+ 构建项目设置段落(泛型)
  + 说明
    >将项目设置构建成欸指标段落数据PPCfgSection
  + 接口调用
  
    ```C#
     public static PPCfgSection BuildConfigSection<T>(T _inSetting) where T : PPSettingBase;
    ```

  + 参数
  
    | 参数名 |   参数意义   |
    |:------:|:------------:|
    |    T   | 项目设置对象 |

  + 返回
    |       返回数据      |     参数意义     |
    |:-------------------:|:----------------:|
    | return PPCfgSection | 返回段落数据结构 |

+ 生成设置实例(泛型)
  + 说明
    >将段落数据结构生成对应的项目设置对象实例
  + 接口调用
  
    ```C#
     public static T CreateSettingInstance<T>(PPCfgSection _inSection) where T : PPSettingBase;
    ```

  + 参数
  
    |    参数名    |   参数意义   |
    |:------------:|:------------:|
    | PPCfgSection | 项目设置对象 |

  + 返回
    | 返回数据 |   参数意义   |
    |:--------:|:------------:|
    | return T | 获取对象实例 |

## 结束

[返回](../README.md)返回模组功能说明。
