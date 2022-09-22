using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace PPExtensionModule
{
    public static class Config
    {

        public static string ConfigPathRoot
        {
            get
            {
                
#if UNITY_ANDROID && !UNITY_EDITOR
                return Application.persistentDataPath;
#else
            return  Directory.GetParent(Application.dataPath).ToString();
#endif
            }
        }

        private static string configPathDirName = @"\PPConfig";

        private static string configPathDefault = ConfigPathRoot + configPathDirName;

        private static string configfileNameDefault = "PPConfig";
        private static string configfileExtention = "cfg";

        public static string ConfigPathDirName => configPathDirName;
        public static string ConfigPath => configPathDefault;
        public static string ConfigNameDefault => configfileNameDefault;
        
        public static string ConfigfileExtention => configfileExtention;
        
        public static string GetAllConfigFilesPath()
        {
            string paths = "";
            GetAllConfigFilesPath(ConfigPath,ref paths);
            return paths;
        }
        
        private static void GetAllConfigFilesPath(string srcPath,ref string contentPath)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
            foreach (FileSystemInfo i in fileinfo){
                if (i is DirectoryInfo) {     //判断是否文件夹
                    GetAllConfigFilesPath(i.FullName,ref contentPath);    //递归调用复制子文件夹
                } 
                else
                {
                    string removed =i.FullName.Replace(ConfigPath,"");
                    contentPath =  removed+";"+contentPath;
                }
            }

        }
        
        
        public static void CopyTo(string srcPath, string destPath)
        {
            string LogContent = "CopyConfigFiles    :";

            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
                Debug.Log("Create Config Directory:"+destPath);
            }
        
            try{
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo){
                    if (i is DirectoryInfo) {     //判断是否文件夹
                        if (!Directory.Exists(destPath+"\\"+i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                            LogContent = LogContent + "    " + "CreateDir-:" + i.Name;
                        }
                        CopyTo(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                    } else {
                        
                        File.Copy(i.FullName, destPath + "\\" + i.Name,true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                        LogContent = LogContent + "    " + "CopyFile-:" + i.Name;
                    }
                }
            } catch (Exception e) {
                Debug.LogError(e.ToString());
            }
        
            Debug.Log(LogContent);
        }
        
        private static string CombineFileName(string inPath = "", string inFileName = "")
        {
            return Path.Combine(ConvertConfigPath(inPath), ConvertConfigFileName(inFileName));
        }

        private static string ConvertConfigPath(string inPath)
        {
            string path = inPath == "" ? ConfigPath : inPath;
#if UNITY_ANDROID
            path = path.Replace("\\", "/");
#endif
            return path;
        }

        private static string ConvertConfigFileName(string inFileName)
        {
            if(string.IsNullOrEmpty( inFileName))
                return ConfigNameDefault+"."+ ConfigfileExtention;
            
            return (inFileName.LastIndexOf('.') == -1) ? (inFileName + "." + ConfigfileExtention) : inFileName;

        }

        private static string FileExists(string inConfigFileName, string inConfigFilePath)
        {
            inConfigFileName = ConvertConfigFileName(inConfigFileName);
            inConfigFilePath = ConvertConfigPath(inConfigFilePath);
            
            if (!Directory.Exists(inConfigFilePath))
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                CopyAllCfgToPersistentPath();
#else
                return "";
#endif
            }
            string filePath = CombineFileName(inConfigFilePath, inConfigFileName);
            filePath = File.Exists(filePath) ? filePath : "";
            return filePath;
        }

        private static void CopyAllCfgToPersistentPath()
        {

            string cachePath = Application.streamingAssetsPath + @"/CfgPathCache.ini";
            
            UnityWebRequest request = UnityWebRequest.Get(cachePath);
            request.timeout = 5;
            request.SendWebRequest();
            int whileI = 0;
            while (true)
            {
                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.LogError(request.error);
                    break;
                }
                
                whileI++;
                if (request.downloadHandler.isDone)//是否读取完数据
                {
                    
                    string cfgPath = request.downloadHandler.text;
                    string[] paths = cfgPath.Split(';');
                    
                    request.Dispose();
                    
                    for (int i = 0; i < paths.Length; i++)
                    {
                        if (string.IsNullOrEmpty(paths[i])) continue;

                        string loadPath = Application.streamingAssetsPath+ ConfigPathDirName+ paths[i];
                        loadPath = ConvertConfigPath(loadPath);
                        UnityWebRequest requestLoad = UnityWebRequest.Get(loadPath);
                        requestLoad.timeout = 5;
                        requestLoad.SendWebRequest();
                        
                        string toPath = ConfigPath + paths[i];
                        toPath = ConvertConfigPath(toPath);
                        while (true)
                        {
                            if (requestLoad.isHttpError || requestLoad.isNetworkError)
                            {
                                Debug.LogError(requestLoad.error);
                                Debug.LogError(i+"[][]"+"Error To Copy:" +loadPath+"--To:"+ toPath);
                                break;
                            }
                            
                            if (requestLoad.downloadHandler.isDone)
                            {
                                string dirPath =Path.GetDirectoryName(toPath);
                                if (!Directory.Exists(dirPath))
                                {
                                    Directory.CreateDirectory(dirPath);
                                }
                                
                                File.WriteAllBytes(toPath, requestLoad.downloadHandler.data);
                                requestLoad.Dispose();
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 获取配置表某一个段落所有的匹配对
        /// </summary>
        /// <param name="pairs">返回的配置项</param>
        /// <param name="inSectionName">段落名</param>
        /// <param name="inConfigFileName">文件名，有默认文件名</param>
        /// <param name="inConfigFilePath">配置表加载路径</param>
        /// <returns>查找结果</returns>
        public static bool GetConfigPairs(ref Dictionary<string, string> pairs, string inSectionName, string inConfigFileName = "", string inConfigFilePath = "")
        {

            List<string> sectionLines = new List<string>();

            if (GetSectionContent(ref sectionLines, inSectionName, inConfigFileName, inConfigFilePath))
            {
                foreach (string item in sectionLines)
                {
                    string[] KV = item.Split('=');
                    pairs.Add(KV[0], KV[1]);
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// 获取配置表某一个段落所有的匹配对
        /// </summary>
        /// <param name="section">返回段落结构体</param>
        /// <param name="inConfigFileName">文件名，有默认文件名</param>
        /// <param name="inConfigFilePath">配置表加载路径</param>
        /// <returns>查找结果</returns>
        public static bool GetConfigSection(ref PPCfgSection section, string inConfigFileName = "", string inConfigFilePath = "")
        {

            List<string> sectionLines = new List<string>();

            if (GetSectionContent(ref sectionLines, section.sectionName, inConfigFileName, inConfigFilePath))
            {
                foreach (string item in sectionLines)
                {
                    string[] KV = item.Split('=');
                    section.AddContent(new PPCfgContent() { Key = KV[0], Value = KV[1] });
                }

                return true;
            }

            return false;
        }






        /// <summary>
        /// 获取配置表所有的段落数
        /// </summary>
        /// <param name="outNum">返回数量</param>
        /// <param name="inConfigFileName">文件名，有默认文件名</param>
        /// <param name="inConfigFilePath">配置表加载路径</param>
        /// <returns>查找结果</returns>
        public static int GetConfigSectionNum(string inConfigFileName = "", string inConfigFilePath = "")
        {
            List<string> sectionNames = new List<string>();
            bool res = GetAllSectionNames(ref sectionNames, inConfigFileName, inConfigFilePath);

            if (!res) return -1;

            return sectionNames.Count;
        }


        /// <summary>
        /// 获取配置表中的某一项
        /// </summary>
        /// <param name="outItem">返回查询结果</param>
        /// <param name="inSectionName">段落名</param>
        /// <param name="inKeyName">配置项名</param>
        /// <param name="inConfigFileName">文件名，有默认文件名</param>
        /// <param name="inConfigFilePath">配置表加载路径</param>
        /// <returns>查找结果</returns>
        public static bool GetConfigItem(ref string outItem, string inSectionName, string inKeyName, string inConfigFileName = "", string inConfigFilePath = "")
        {

            PPCfgSection section = new PPCfgSection();
            section.sectionName = inSectionName;
            bool res = GetConfigSection(ref section, inConfigFileName, inConfigFilePath);

            if (!res) return false;

            return section.TryGetPairValue(inKeyName, ref outItem);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cfgData">需要保存的配置数据</param>
        /// <param name="inConfigFileName">文件名</param>
        /// <param name="inConfigFilePath">文件路径</param>
        /// <returns></returns>
        public static bool SaveConfig(PPCfgData cfgData, string inConfigFileName = "", string inConfigFilePath = "")
        {
            inConfigFileName = ConvertConfigFileName(inConfigFileName);
            inConfigFilePath = ConvertConfigPath(inConfigFilePath);

            if (!Directory.Exists(inConfigFilePath))
            {
                Directory.CreateDirectory(inConfigFilePath);
            }

            string filePath = CombineFileName(inConfigFilePath, inConfigFileName);

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }


            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

                sw.WriteLine("@ Line Comment");
                sw.WriteLine("##  Line Block Comment ##");
                sw.WriteLine("");
                
                sw.WriteLine(cfgData.ToString());

                sw.Flush();
                sw.Close();
            }
            
            return true;

        }


        private static bool WriteSection(PPCfgSection sectionData, string inConfigFileName = "", string inConfigFilePath = "")
        {
            inConfigFileName = ConvertConfigFileName(inConfigFileName);
            inConfigFilePath = ConvertConfigPath(inConfigFilePath);

            FileExists(inConfigFileName, inConfigFilePath);

            if (!Directory.Exists(inConfigFilePath))
            {
                Directory.CreateDirectory(inConfigFilePath);
            }

            string filePath = CombineFileName(inConfigFilePath, inConfigFileName);
            
            if (!File.Exists(filePath))
            {
                //File.Create(filePath).Dispose();
                
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

                    sw.WriteLine("@ Line Comment");
                    sw.WriteLine("##  Line Block Comment ##");
                    sw.WriteLine("");
                    sw.Flush();
                    sw.Close();
                }
                
            }


            using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

                sw.WriteLine(sectionData.ToString());
                
                sw.Flush();
                sw.Close();
            }
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionData">新增的段落数据</param>
        /// <param name="inConfigFileName">文件名</param>
        /// <param name="inConfigFilePath">文件路径</param>
        /// <returns></returns>
        public static bool SaveSection(PPCfgSection sectionData, string inConfigFileName = "", string inConfigFilePath = "", bool bOverrideExist = true)
        {
            if (!bOverrideExist)
            {
                return WriteSection(sectionData, inConfigFileName, inConfigFilePath);
            }

            PPCfgData data = new PPCfgData();
            GetConfig(ref data, inConfigFileName, inConfigFilePath);
            data.AddSection(sectionData);

            return SaveConfig(data, inConfigFileName, inConfigFilePath);

        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="outItem">返回加载的配置表</param>
        /// <param name="inConfigFileName">文件名，有默认文件名</param>
        /// <param name="inConfigFilePath">配置表加载路径</param>
        /// <returns></returns>
        public static bool GetConfig(ref PPCfgData outItem, string inConfigFileName = "", string inConfigFilePath = "")
        {
            string filePath = FileExists(inConfigFileName, inConfigFilePath);
            if (string.IsNullOrEmpty(filePath)) return false;


            outItem.configFullName = Path.GetFileNameWithoutExtension(ConvertConfigFileName(inConfigFileName));


            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);

                string strLine = null;
                string curSectioName = "";
                string dealLine = "";
                do
                {
                    strLine = sr.ReadLine();

                    if (strLine == "") continue;
                    if (strLine == null) break;
                    if (strLine[0] == '@') continue;

                    string[] splitlineComments = strLine.Split('@');
                    if (splitlineComments.Length == 1)
                    {
                        dealLine = strLine;
                    }
                    else
                    {
                        dealLine = splitlineComments[0];
                    }

                    if (dealLine.Contains("##"))
                    {
                        int startIndex = dealLine.IndexOf("##", StringComparison.Ordinal);
                        int endIndex = dealLine.LastIndexOf("##", StringComparison.Ordinal);

                        if (startIndex == endIndex)
                        {
                            dealLine =  dealLine.Replace("##", "");
                        }
                        else
                        {
                            string lStr = dealLine.Substring(0, startIndex);
                            string rStr = dealLine.Substring(endIndex+2, dealLine.Length-endIndex-2);
                            dealLine = lStr + rStr;
                        }
                        
                    }

                    dealLine = dealLine.Trim();
                    if (dealLine == "") continue;
                    
                    if ((dealLine[0] == '[') && (dealLine[dealLine.Length - 1] == ']'))
                    {
                        curSectioName = dealLine.Substring(1, dealLine.Length - 2);
                        outItem.CreateNewSection( curSectioName );
                    }
                    else if (outItem.ContainsSection(curSectioName))
                    {
                        string[] KV = dealLine.Split('=');

                        if (KV.Length != 2) continue;

                        outItem.AddContent(curSectioName,new PPCfgContent() { Key = KV[0], Value = KV[1] });
                    }
                } while (strLine != null);

                sr.Close();
            }

            return true;
        }

        /// <summary>
        /// 获取配置表中所有的段落名称
        /// </summary>
        /// <param name="outSectionNames">返回所有段落名</param>
        /// <param name="inConfigFileName">文件名，有默认文件名</param>
        /// <param name="inConfigFilePath">配置表加载路径</param>
        /// <returns>查找结果</returns>
        public static bool GetAllSectionNames(ref List<string> outSectionNames, string inConfigFileName = "", string inConfigFilePath = "")
        {
            PPCfgData data = new PPCfgData();
            if (!GetConfig(ref data, inConfigFileName, inConfigFilePath)) return false;

            data.GetAllSectionNames(ref outSectionNames);

            return true;
        }




        /// <summary>
        /// 查找某一个段落的所有内容（不包含空行）
        /// </summary>
        /// <param name="outContents">返回所有内容</param>
        /// <param name="inSectionName">查询段落名称</param>
        /// <param name="inConfigFileName">文件名，有默认文件名</param>
        /// <param name="inConfigFilePath">配置表加载路径</param>
        /// <returns>查找结果</returns>
        private static bool GetSectionContent(ref List<string> outContents, string inSectionName, string inConfigFileName = "", string inConfigFilePath = "")
        {
            string filePath = FileExists(inConfigFileName, inConfigFilePath);
            if (string.IsNullOrEmpty(filePath)) return false;

            outContents.Clear();

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);

                string strLine = null;
                bool bSearchedSection = false;
                string dealLine = "";

                do
                {
                    strLine = sr.ReadLine();

                    if (strLine == "") continue;
                    if (strLine == null) break;
                    if (strLine[0] == '@') continue;


                    string[] splitlineComments = strLine.Split('@');
                    if (splitlineComments.Length == 1)
                    {
                        dealLine = strLine;
                    }
                    else
                    {
                        dealLine = splitlineComments[0];
                    }
                    
                    if (dealLine.Contains("##"))
                    {
                        int startIndex = dealLine.IndexOf("##", StringComparison.Ordinal);
                        int endIndex = dealLine.LastIndexOf("##", StringComparison.Ordinal);

                        if (startIndex == endIndex)
                        {
                            dealLine =  dealLine.Replace("##", "");
                        }
                        else
                        {
                            string lStr = dealLine.Substring(0, startIndex);
                            string rStr = dealLine.Substring(endIndex+2, dealLine.Length-endIndex-2);
                            dealLine = lStr + rStr;
                        }
                        
                    }
                    
                    dealLine = dealLine.Trim();
                    if (dealLine == "") continue;
                    

                    if ((dealLine[0] == '[') && (dealLine[dealLine.Length - 1] == ']'))
                    {
                        if (bSearchedSection)
                        {
                            sr.Close();
                            return true;
                        }

                        string sectionTitle = dealLine.Substring(1, dealLine.Length - 2);

                        if (sectionTitle == inSectionName)
                        {
                            bSearchedSection = true;
                        }
                    }
                    else if (bSearchedSection)
                    {

                        outContents.Add(dealLine);
                    }


                } while (strLine != null);

                sr.Close();

                return bSearchedSection;

            }

        }
    }
    
}



