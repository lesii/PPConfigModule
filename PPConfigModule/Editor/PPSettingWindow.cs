using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;



public partial class PPSettingWindowScope
{

    public class PPSettingWindow : EditorWindow
    {

        public class PPShowCfgData
        {
            public string ConfigPath;
            public string ConfigName;

            public List<PPSettingPackage> settings;

            public bool Equals(PPShowCfgData other)
            {
                return (this.ConfigPath == other.ConfigPath) && (this.ConfigName == other.ConfigName);
            }

            public override string ToString()
            {
                return ConfigName;
            }
        }


        public class PPSettingPackage
        {
            public PPShowCfgData cfgData;
            public PPExtensionModule.PPCfgSection loadSection;

            public bool Equals(PPSettingPackage other)
            {
                return (cfgData.Equals(other.cfgData)) && (loadSection.sectionName == other.loadSection.sectionName);
            }
        }


        private string projectPath;

        private static PPSettingWindow window;
        private Dictionary<string, bool> foldoutDic;
        private PPSettingPackage currentSelected;
        private List<PPShowCfgData> configs;

        private List<string> curSectionNames;
        private List<string> curSectionValue;

        private Vector2 listScroll;
        private Vector2 detailScroll;

        private List<PPCreateSettingWindow.ContentData> addNewContents;

        private bool bFreshClass;
        private bool bDeleteElements;


        [MenuItem("PPConfig/Project Setting", false, 100)]
        private static void OpenSetting()
        {
            window = (PPSettingWindow) EditorWindow.GetWindow(typeof(PPSettingWindow), false, "PP Setting",
                true); //创建窗口
            window.Init();
            window.Show(); //展示

        }

        private void OnEnable()
        {

        }


        private void OnGUI()
        {
            GUILayout.BeginHorizontal("Toolbar");
            {
                //在此处添加一个搜索栏
            }
            GUILayout.EndHorizontal();

            //水平布局
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            {
                //垂直布局 设置左侧列表宽度
                GUILayout.BeginVertical(GUILayout.Width(200f));
                {
                    OnListGUI();
                }
                GUILayout.EndVertical();

                //分割线
                GUILayout.Box(string.Empty, "EyeDropperVerticalLine", GUILayout.ExpandHeight(true),
                    GUILayout.Width(1f));

                //垂直布局
                GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                {
                    OnDetailGUI();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

        }


        private void OnListGUI()
        {
            if (configs == null) return;

            GUILayout.BeginHorizontal(GUILayout.Height(21f));
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUILayout.Width(25f)))
                {
                    CreateNewConfig();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Box(string.Empty, "EyeDropperHorizontalLine", GUILayout.ExpandWidth(true), GUILayout.Height(1f));


            //滚动视图
            listScroll = GUILayout.BeginScrollView(listScroll);
            {
                GUIStyle versionStyle = new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Italic};
                for (int i = 0; i < configs.Count; i++)
                {
                    List<PPSettingPackage> list = configs[i].settings;
                    PPSettingPackage first = list[0];
                    //if (!string.IsNullOrEmpty(searchContent) && !first.name.ToLower().Contains(searchContent.ToLower())) continue;
                    if (foldoutDic[first.cfgData.ToString()])
                    {
                        foldoutDic[first.cfgData.ToString()] =
                            EditorGUILayout.Foldout(foldoutDic[first.cfgData.ToString()], first.cfgData.ToString());
                        for (int n = 0; n < list.Count; n++)
                        {
                            GUILayout.BeginHorizontal((currentSelected != null && currentSelected.Equals(list[n]))
                                ? "SelectionRect"
                                : "IN Title");
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(list[n].loadSection.sectionName);
                            GUILayout.Space(30f);
                            GUILayout.EndHorizontal();

                            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) &&
                                Event.current.type == EventType.MouseDown)
                            {
                                currentSelected = list[n];
                                RevertSelected();
                                Event.current.Use();
                            }
                        }
                    }
                    else
                    {
                        GUILayout.BeginHorizontal((currentSelected != null && currentSelected.Equals(first))
                            ? "SelectionRect"
                            : "Toolbar");
                        {
                            foldoutDic[first.cfgData.ToString()] =
                                EditorGUILayout.Foldout(foldoutDic[first.cfgData.ToString()], first.cfgData.ToString());
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(first.cfgData.ConfigPath, versionStyle);
                        }
                        GUILayout.EndHorizontal();

                        //鼠标点击选中
                        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) &&
                            Event.current.type == EventType.MouseDown)
                        {
                            currentSelected = first;
                            RevertSelected();
                            Event.current.Use();
                        }
                    }
                }
            }
            GUILayout.EndScrollView();

            //分割线
            GUILayout.Box(string.Empty, "EyeDropperHorizontalLine", GUILayout.ExpandWidth(true), GUILayout.Height(1f));


            GUILayout.BeginHorizontal(GUILayout.Height(23f));
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), GUILayout.Width(30f)))
                {
                    window.Init();
                }
            }
            GUILayout.EndHorizontal();

        }

        private void CreateNewConfig()
        {
            Debug.Log("Create New Cfg Data");
            PPCreateSettingWindow.OpenCreateWindow();

        }



        private void OnDetailGUI()
        {
            if (currentSelected != null)
            {
                //名称
                GUILayout.Label(currentSelected.loadSection.sectionName,
                    new GUIStyle(GUI.skin.label) {fontSize = 25, fontStyle = FontStyle.Bold});
                EditorGUILayout.Space();

                detailScroll = GUILayout.BeginScrollView(detailScroll);
                {
                    GUILayout.BeginHorizontal(GUILayout.Height(21f));
                    {
                        if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUILayout.Width(21f)))
                        {
                            AddNewContent();
                        }

                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1f));

                    for (int i = 0; i < curSectionNames.Count; i++)
                    {
                        int index = i;
                        GUILayout.BeginHorizontal(GUILayout.Height(21f));
                        {
                            if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUILayout.Width(21f)))
                            {
                                RemoveContent(index);
                            }
                            else
                            {
                                //GUILayout.FlexibleSpace();
                                GUILayout.Label(curSectionNames[index]);
                                GUILayout.FlexibleSpace();
                                //GUILayout.Space(30f);
                                curSectionValue[index] =
                                    GUILayout.TextField(curSectionValue[index], GUILayout.MinWidth(100f));
                            }
                        }
                        GUILayout.EndHorizontal();

                    }

                    GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1f));

                    for (int i = 0; i < addNewContents.Count; i++)
                    {
                        int index = i;
                        GUILayout.BeginHorizontal(GUILayout.Height(21f));
                        {
                            GUILayout.Space(21f);
                            string inputKeyName =
                                GUILayout.TextField(addNewContents[index].KeyName, GUILayout.MinWidth(100f));
                            addNewContents[index].KeyName = string.IsNullOrEmpty(inputKeyName)
                                ? inputKeyName
                                : inputKeyName.Replace(" ", "");
                            addNewContents[index].type =
                                (PPCreateSettingWindow.InType) EditorGUILayout.EnumPopup(addNewContents[index].type,
                                    GUILayout.Width(80f));
                            GUILayout.FlexibleSpace();
                            //GUILayout.Space(30f);
                            string inputValueName =
                                GUILayout.TextField(addNewContents[index].value, GUILayout.MinWidth(100f));
                            //addNewContents[index].value = string.IsNullOrEmpty(inputValueName)? inputValueName:inputValueName.Replace(" ","");
                            addNewContents[index].value = inputValueName;
                        }
                        GUILayout.EndHorizontal();
                    }


                    GUILayout.FlexibleSpace();

                }
                GUILayout.EndScrollView();

                //分割线
                //GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1f));


                //分割线
                GUILayout.Box(string.Empty, "EyeDropperHorizontalLine", GUILayout.ExpandWidth(true),
                    GUILayout.Height(1f));

                //水平布局 设置高度
                GUILayout.BeginHorizontal(GUILayout.Height(21f));
                {

                    if (addNewContents.Count > 0 || bDeleteElements)
                    {
                        GUILayout.Space(5f);
                        bFreshClass = GUILayout.Toggle(bFreshClass, "Fresh Class");
                    }

                    GUILayout.FlexibleSpace();
                    //下载并导入
                    if (GUILayout.Button("Save", GUILayout.Width(50f)))
                    {
                        if (currentSelected != null)
                        {
                            SaveSelected();
                            //ResetData();
                        }
                    }

                    if (GUILayout.Button("Revert", GUILayout.Width(50f)))
                    {
                        if (currentSelected != null)
                        {
                            RevertSelected();
                        }
                    }

                    GUILayout.Space(5f);
                }
                GUILayout.EndHorizontal();

            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("PP Setting Config",
                    new GUIStyle(GUI.skin.label) {fontSize = 25, fontStyle = FontStyle.Bold});
                GUILayout.EndHorizontal();
            }


        }

        private void AddNewContent()
        {
            addNewContents.Add(new PPCreateSettingWindow.ContentData());
        }

        private void RemoveContent(int index)
        {
            curSectionNames.RemoveAt(index);
            curSectionValue.RemoveAt(index);
            bDeleteElements = true;
        }

        private void RevertSelected()
        {
            if (currentSelected == null) return;

            curSectionNames = currentSelected.loadSection.KV.Keys.ToList();
            curSectionValue.Clear();

            for (int j = 0; j < curSectionNames.Count; j++)
            {
                curSectionValue.Add(currentSelected.loadSection.KV[curSectionNames[j]].Value);
            }

            addNewContents.Clear();
            bDeleteElements = false;
        }

        private void SaveSelected()
        {
            if (currentSelected == null) return;

            PPExtensionModule.PPCfgSection sec = new PPExtensionModule.PPCfgSection()
                {sectionName = currentSelected.loadSection.sectionName};

            for (int i = 0; i < curSectionNames.Count; i++)
            {
                sec.AddContent(curSectionNames[i], curSectionValue[i]);
            }

            for (int i = 0; i < addNewContents.Count; i++)
            {
                sec.AddContent(addNewContents[i].KeyName, addNewContents[i].value);
                curSectionNames.Add(addNewContents[i].KeyName);
                curSectionValue.Add(addNewContents[i].value);
            }

            PPExtensionModule.Config.SaveSection(sec, currentSelected.cfgData.ConfigName,
                projectPath + "\\" + currentSelected.cfgData.ConfigPath);


            if (bFreshClass)
            {
                FreshClassData(sec);
            }

            addNewContents.Clear();
            bDeleteElements = false;

            currentSelected.loadSection = sec;
            RevertSelected();

        }

        private void FreshClassData(PPExtensionModule.PPCfgSection sec)
        {
            if (addNewContents.Count == 0 && !bDeleteElements) return;

            string createDir = Application.dataPath + PPCreateSettingWindow.defaultClassPath;

            if (!Directory.Exists(createDir))
            {
                Directory.CreateDirectory(createDir);
            }

            string CreatePath = Application.dataPath + PPCreateSettingWindow.defaultClassPath + sec.sectionName + ".cs";

            if (!File.Exists(CreatePath))
            {
                Debug.LogError("Can't Find Class" + sec.sectionName + ".cs,Create New Class File");
                GenerateClassData();
                
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder sb_Residue = new StringBuilder();
            bool findEnd = false;
            foreach (string line in File.ReadAllLines(CreatePath))
            {
                if (line == "//#")
                {
                    findEnd = true;
                    sb_Residue.Append(line + "\r\n");
                    continue;
                }

                if (findEnd)
                {
                    sb_Residue.Append(line + "\r\n");
                    continue;
                }



                if (!line.Contains(PPCreateSettingWindow.END_VARIABLE))
                {
                    sb.Append(line + "\r\n");
                    continue;
                }

                int startContentIndex = line.IndexOf("public");
                string subLine = line.Substring(startContentIndex, line.Length - startContentIndex);
                string[] splitVarNames = subLine.Split(' ');


                if (splitVarNames.Length != 4)
                {
                    sb.Append(line + "\r\n");
                    continue;
                }

                splitVarNames[2] = splitVarNames[2].Replace(";", "");
                if (curSectionNames.Contains(splitVarNames[2]))
                {
                    sb.Append(line + "\r\n");
                }

            }


            for (int i = 0; i < addNewContents.Count; i++)
            {
                sb.Append("    public ");
                switch (addNewContents[i].type)
                {
                    case PPCreateSettingWindow.InType.Integer:
                        sb.Append("int ");
                        break;
                    case PPCreateSettingWindow.InType.Single:
                        sb.Append("float ");
                        break;
                    case PPCreateSettingWindow.InType.String:
                        sb.Append("string ");
                        break;
                    case PPCreateSettingWindow.InType.Boolean:
                        sb.Append("bool ");
                        break;
                }

                sb.Append(addNewContents[i].KeyName);
                sb.Append(PPCreateSettingWindow.END_VARIABLE + "\r\n");
            }

            sb.Append(sb_Residue);

            FileStream fileStream = File.Create(CreatePath);

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();

            AssetDatabase.Refresh();
        }

        private void GenerateClassData()
        {
            List<PPCreateSettingWindow.ContentData> contents = new List<PPCreateSettingWindow.ContentData>();

            for (int i = 0; i < curSectionNames.Count; i++)
            {
                PPCreateSettingWindow.ContentData data = new PPCreateSettingWindow.ContentData();
                data.KeyName = curSectionNames[i];
                data.value = curSectionValue[i];

                int iVal = 0;
                float fVal = 0;
                bool bVal = false;

                if (int.TryParse(curSectionValue[i], out iVal))
                {
                    data.type = PPCreateSettingWindow.InType.Integer;
                }
                else if (float.TryParse(curSectionValue[i], out fVal))
                {
                    data.type = PPCreateSettingWindow.InType.Single;
                }
                else if (bool.TryParse(curSectionValue[i], out bVal))
                {
                    data.type = PPCreateSettingWindow.InType.Boolean;
                }
                else
                {
                    data.type = PPCreateSettingWindow.InType.String;
                }
                contents.Add(data);
            }

            PPCreateSettingWindow.CreateNewClass(currentSelected.loadSection.sectionName, contents);
        }

        private void ResetData()
        {
            bDeleteElements = false;
            configs = new List<PPShowCfgData>();
            string targetPath = Path.GetDirectoryName(Application.dataPath) +
                                PPExtensionModule.Config.ConfigPathDirName;

            window.LoadConfig(targetPath);
        }
        
        private void Init()
        {
            projectPath = Path.GetDirectoryName(Application.dataPath);

            //是否已存在文件夹
            if (!Directory.Exists(PPExtensionModule.Config.ConfigPath))
            {
                Directory.CreateDirectory(PPExtensionModule.Config.ConfigPath);
            }

            bDeleteElements = false;
            configs = new List<PPShowCfgData>();
            addNewContents = new List<PPCreateSettingWindow.ContentData>();
            curSectionValue = new List<string>();
            currentSelected = null;
            string targetPath = Path.GetDirectoryName(Application.dataPath) +
                                PPExtensionModule.Config.ConfigPathDirName;

            window.LoadConfig(targetPath);

            foldoutDic = new Dictionary<string, bool>();
            foreach (var item in configs)
            {
                foldoutDic.Add(item.ToString(), false);
            }
        }

        private void LoadConfig(string srcPath)
        {

            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos(); //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)
                    {
                        LoadConfig(i.FullName); //递归调用查找子文件夹
                    }
                    else
                    {
                        PPExtensionModule.PPCfgData data = new PPExtensionModule.PPCfgData();
                        string path = Path.GetDirectoryName(i.FullName);
                        PPExtensionModule.Config.GetConfig(ref data, i.Name, path);
                        Dictionary<string, PPExtensionModule.PPCfgSection> pairs = data.GetCopyPairs();
                        PPShowCfgData cfgData = new PPShowCfgData();

                        path = path.Replace(projectPath, "");
                        cfgData.ConfigPath = path;
                        cfgData.ConfigName = i.Name;
                        cfgData.settings = new List<PPSettingPackage>();
                        configs.Add(cfgData);
                        foreach (var item in pairs)
                        {
                            PPSettingPackage setting = new PPSettingPackage();
                            setting.cfgData = cfgData;
                            setting.loadSection = item.Value;
                            cfgData.settings.Add(setting);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

        }
    }
}