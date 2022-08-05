using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Text;
using UnityEditor.Build;

public class PPCfgAfterBuildToDo : Editor
{
    
    [PostProcessBuild(1)]
    public static void AfterBuild(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_ANDROID
        string targetPath = Application.streamingAssetsPath+ PPExtensionModule.Config.ConfigPathDirName;

        if (Directory.Exists(targetPath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
            directoryInfo.Delete(true);
            Debug.Log("Clear Cache Cfg Dir");
            File.Delete(Application.streamingAssetsPath + @"/CfgPathCache.ini");
        }
#else
        //Debug.Log("Build Success  输出平台: " + target + "  输出路径: " + pathToBuiltProject);

        //获取拷贝路径 = 打包输出路径 + 包名(打包文件夹名) + 下级路径
        string targetPath = Path.GetDirectoryName(pathToBuiltProject) + PPExtensionModule.Config.ConfigPathDirName;
        //是否已存在文件夹
        if (!Directory.Exists(PPExtensionModule.Config.ConfigPath)) return;

        PPExtensionModule.Config.CopyTo(PPExtensionModule.Config.ConfigPath, targetPath);
#endif
    }
}


#if UNITY_ANDROID && UNITY_EDITOR
public class PPCfgAndroidPrebuild : IPreprocessBuild
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        PPExtensionModule.Config.CopyTo(PPExtensionModule.Config.ConfigPath, Application.streamingAssetsPath+ PPExtensionModule.Config.ConfigPathDirName);
        string paths = PPExtensionModule.Config.GetAllConfigFilesPath();
        File.WriteAllText(Application.streamingAssetsPath + @"/CfgPathCache.ini",paths, new UTF8Encoding(false));
        Debug.Log(paths);
    }
}
#endif