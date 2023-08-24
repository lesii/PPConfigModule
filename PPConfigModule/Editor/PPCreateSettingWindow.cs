using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;


public sealed partial class PPSettingWindowScope
{
    public class PPCreateSettingWindow : EditorWindow
    {
        public static string END_VARIABLE = "; //---Delete_This_Comment_To_Prevent_Change!";

        [Serializable]
        public enum InType
        {
            String,
            Integer,
            Single,
            Boolean
        }

        public class ContentData
        {
            public InType type;
            public string KeyName;
            public string value;

            public ContentData()
            {
                type = InType.String;
            }
            
        }

        public const string defaultClassPath = @"\PProjectSetting\";

        private static PPCreateSettingWindow window;


        private bool bCreateNewClass;
        private Vector2 contentScroll;
        private string settingName;

        private List<ContentData> data;

        [MenuItem("PPConfig/Create Setting", false, 100)]
        public static void OpenCreateWindow()
        {
            window = (PPCreateSettingWindow) EditorWindow.GetWindow(typeof(PPCreateSettingWindow), false,
                "PP Create Setting", true); //创建窗口
            window.Init();
            window.Show(); //展示

        }


        private void OnGUI()
        {
            GUILayout.Space(5f);
            GUILayout.BeginHorizontal(GUILayout.Height(21f));
            {
                GUILayout.Space(20f);

                if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUILayout.Width(21f)))
                {
                    AddNewElement();
                }

                GUILayout.FlexibleSpace();
                GUILayout.Label("Setting Name:",
                    new GUIStyle(GUI.skin.label) {fontSize = 12, fontStyle = FontStyle.Bold});
                settingName = GUILayout.TextField(settingName, GUILayout.Width(100f));
                GUILayout.Space(20f);
            }
            GUILayout.EndHorizontal();
            GUILayout.Box(string.Empty, "EyeDropperHorizontalLine", GUILayout.ExpandWidth(true), GUILayout.Height(1f));

            GUILayout.Space(10f);
            contentScroll = GUILayout.BeginScrollView(contentScroll);
            {
                if (data.Count != 0)
                {
                    GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
                }

                for (int i = 0; i < data.Count; i++)
                {
                    GUILayout.BeginHorizontal(GUILayout.Height(21f));
                    {

                        GUILayout.Label("Name:",
                            new GUIStyle(GUI.skin.label) {fontSize = 12, fontStyle = FontStyle.Bold});
                        string inputKeyName =
                            GUILayout.TextField(data[i].KeyName, GUILayout.MinWidth(120f));
                        data[i].KeyName = string.IsNullOrEmpty(inputKeyName)
                            ? inputKeyName
                            : inputKeyName.Replace(" ", "");
                        GUILayout.FlexibleSpace();
                        GUILayout.Box(string.Empty, GUILayout.ExpandHeight(true), GUILayout.Width(1f));
                        GUILayout.Label("PairType:",
                            new GUIStyle(GUI.skin.label) {fontSize = 12, fontStyle = FontStyle.Bold});
                        data[i].type = (InType) EditorGUILayout.EnumPopup(data[i].type, GUILayout.Width(80f));
                        GUILayout.FlexibleSpace();
                        GUILayout.Box(string.Empty, GUILayout.ExpandHeight(true), GUILayout.Width(1f));
                        GUILayout.Label("Value:",
                            new GUIStyle(GUI.skin.label) {fontSize = 12, fontStyle = FontStyle.Bold});
                        string inputValueName = GUILayout.TextField(data[i].value, GUILayout.MinWidth(120f));
                        //data[i].value = string.IsNullOrEmpty(inputValueName)? inputValueName:inputValueName.Replace(" ","");
                        data[i].value = inputValueName;
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
                }

                GUILayout.FlexibleSpace();

            }

            GUILayout.EndScrollView();

            GUILayout.Box(string.Empty, "EyeDropperHorizontalLine", GUILayout.ExpandWidth(true), GUILayout.Height(1f));

            GUILayout.BeginHorizontal(GUILayout.Height(21f));
            {
                GUILayout.Space(20f);
                bCreateNewClass = GUILayout.Toggle(bCreateNewClass, "Create Class");
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button("Create", GUILayout.Width(70f)))
                {
                    CreateNew();
                }
                GUILayout.Space(20f);

            }
            GUILayout.EndHorizontal();

        }

        private void CreateNew()
        {
            if (data.Count == 0) return;

            string projectPath = Path.GetDirectoryName(Application.dataPath);

            string defaultPath = projectPath + "\\" + "PPConfig";
            string dir = defaultPath;
            if (!Directory.Exists(dir))
            {
                dir = projectPath;
            }

            string fullPathName = EditorUtility.SaveFilePanel("Choice Config", dir, PPExtensionModule.Config.ConfigNameDefault, PPExtensionModule.Config.ConfigfileExtention);

            if (!string.IsNullOrEmpty(fullPathName))
            {
                SaveContent(fullPathName);

                if (bCreateNewClass)
                {
                    CreateNewClass(settingName,data);
                }
            }


            window.Close();
        }

        private void SaveContent(string fullPathName)
        {
            PPExtensionModule.PPCfgSection section = new PPExtensionModule.PPCfgSection();
            section.sectionName = settingName;

            for (int i = 0; i < data.Count; i++)
            {
                PPExtensionModule.PPCfgContent content = new PPExtensionModule.PPCfgContent();
                content.Key = data[i].KeyName;
                content.Value = data[i].value;
                section.AddContent(content);
            }

            string cfgName = Path.GetFileName(fullPathName);
            string cfgPath = Path.GetDirectoryName(fullPathName);

            PPExtensionModule.Config.SaveSection(section, cfgName, cfgPath);
        }

        internal static  void CreateNewClass(string _settingName,List<ContentData> _contentDatas)
        {
            string createDir = Application.dataPath + defaultClassPath;

            if (!Directory.Exists(createDir))
            {
                Directory.CreateDirectory(createDir);
            }

            string CreatePath = Application.dataPath + defaultClassPath + _settingName + ".cs";


            StringBuilder sb = new StringBuilder();

            sb.Append("using PPExtensionModule;\r\n");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append(string.Format("public class {0} :  TPPSettingBase<{0}>\r\n", _settingName));
            sb.Append("{\r\n");
            for (int i = 0; i < _contentDatas.Count; i++)
            {
                sb.Append("    public ");
                switch (_contentDatas[i].type)
                {
                    case InType.Integer:
                        sb.Append("int ");
                        break;
                    case InType.Single:
                        sb.Append("float ");
                        break;
                    case InType.String:
                        sb.Append("string ");
                        break;
                    case InType.Boolean:
                        sb.Append("bool ");
                        break;
                }

                sb.Append(_contentDatas[i].KeyName);
                sb.Append(PPCreateSettingWindow.END_VARIABLE + "\r\n");
            }

            sb.Append("//#\r\n");

            sb.Append("}\r\n");

            FileStream fileStream = File.Create(CreatePath);

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();

            AssetDatabase.Refresh();
        }

        private void AddNewElement()
        {
            data.Add(new ContentData());
        }

        private void Init()
        {
            data = new List<ContentData>();
            
            if (!Directory.Exists(PPExtensionModule.Config.ConfigPath))
            {
                Directory.CreateDirectory(PPExtensionModule.Config.ConfigPath);
            }
        }


    }
}