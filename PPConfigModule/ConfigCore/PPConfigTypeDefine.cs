
using System;
using System.Collections.Generic;

namespace PPExtensionModule
{

    [Serializable]
    public struct PPCfgData
    {
        public string configFullName;
        private Dictionary<string, PPCfgSection> m_kv;

        
        private Dictionary<string, PPCfgSection> KV
        {
            get
            {
                if (m_kv == null)
                    m_kv = new Dictionary<string, PPCfgSection>();
                return m_kv;
            }


        }
        
        public int Count
        {
            get { return KV.Count; }
        }
        

        public void CopyTo(ref PPCfgData _other)
        {
            _other.configFullName = this.configFullName;

            foreach (var item in this.KV)
            {
                PPCfgSection cfgSection = new PPCfgSection();
                item.Value.CopyTo(ref cfgSection);

                _other.AddSection(cfgSection);
            }

        }

        public Dictionary<string, PPCfgSection> GetCopyPairs()
        {
            if (KV == null) return null;

            Dictionary<string, PPCfgSection> res = new Dictionary<string, PPCfgSection>();

            foreach (var item in this.KV)
            {
                PPCfgSection cfgContent = new PPCfgSection();
                item.Value.CopyTo(ref cfgContent);

                res.Add(cfgContent.sectionName, cfgContent);
            }

            return res;
        }

        
        public void RemoveSection(PPCfgSection _inSection)
        {
            if (!KV.ContainsKey(_inSection.sectionName)) return;
            
            KV.Remove(_inSection.sectionName);
        }
        
        public void RemoveSection(string _inSectionName)
        {
            if (!KV.ContainsKey(_inSectionName)) return;
            
            KV.Remove(_inSectionName);
        }
        


        public void AddSection( PPCfgSection _inSection)
        {

            if (KV.ContainsKey(_inSection.sectionName))
            {
                KV[_inSection.sectionName] = _inSection;

            }
            else
            {
                KV.Add(_inSection.sectionName, _inSection);
            }
        }

        public bool AddContent(string _sectionName, PPCfgContent _content)
        {

            if (!KV.ContainsKey(_sectionName)) return false;

            var getVal = KV[_sectionName];
            getVal.AddContent(_content);
            KV[_sectionName] = getVal;

            return true;
        }


        public bool GetSection(string _sectionName,ref PPCfgSection _outSection)
        {
            if (!KV.ContainsKey(_sectionName)) return false;

            KV[_sectionName].CopyTo(ref _outSection);

            return true;
        }

        public void CreateNewSection(string _sectionName)
        {

            KV[_sectionName] = new PPCfgSection() { sectionName = _sectionName };
        }

        public bool ContainsSection(string _sectionName)
        {
            return KV.ContainsKey(_sectionName);
        }

        public void GetAllSectionNames(ref List<string> outSectionNames)
        {
            outSectionNames.Clear();

            outSectionNames.AddRange(KV.Keys);
        }

        public override string ToString()
        {
            string res="";

            foreach (var item in KV)
            {
                res += (item.Value.ToString() + "\r\n");
            }

            if (string.IsNullOrEmpty(res)) return res;
            
            res = res.Substring(0, res.Length - 2);

            return res;
        }
        
        

    }
    
    

    [Serializable]
    public struct PPCfgSection
    {
        public string sectionName;
        private Dictionary<string, PPCfgContent> m_kv ;

        public Dictionary<string, PPCfgContent> KV
        {
            get
            {
                if (m_kv == null) m_kv = new Dictionary<string, PPCfgContent>();

                return m_kv;
            }
        }

        public string this[string _key]
        {
            get
            {
               return KV[_key].Value;
            }
        }
        

        public int Count
        {
            get { return KV.Count; }
        }

        public string ContentString
        {
            get
            {
                string res="";
                foreach (var contentItem in KV)
                {
                    string contentLine = contentItem.Value.ToString();
                    res += (contentLine + "\r\n");
                }
                return res;
                
            }
            
        }

        public bool Contains(string _contentName)
        {
            return KV.ContainsKey(_contentName);
        }
        
        
        
        public void CopyTo(ref PPCfgSection _other)
        {
            _other.sectionName = this.sectionName;

            foreach (var item in this.KV)
            {
                PPCfgContent cfgContent = new PPCfgContent();
                item.Value.CopyTo(ref cfgContent);

                _other.AddContent(cfgContent);
            }

        }

        public void AddContent(PPCfgContent _inContent)
        {

            if (KV.ContainsKey(_inContent.Key))
            {
                KV[_inContent.Key] = _inContent;

            }
            else
            {
                KV.Add(_inContent.Key, _inContent);
            }
        }
        
        public void RemoveContent(PPCfgContent _inContent)
        {

            if (!KV.ContainsKey(_inContent.Key)) return;
            
            KV.Remove(_inContent.Key);
        }

        public void AddContent(string _inKey, string _inValue)
        {
            AddContent(new PPCfgContent(_inKey, _inValue));
        }
        
        public void RemoveContent(string _inKey)
        {
            if (!KV.ContainsKey(_inKey)) return;
            
            KV.Remove(_inKey);
        }



        public bool TryGetContent(string _inKey, ref PPCfgContent _outContent)
        {
            if (!KV.ContainsKey(_inKey)) return false;

            return true;
        }

        public bool TryGetPairValue(string _inKey, ref string _outValue)
        {
            if (!KV.ContainsKey(_inKey)) return false;

            _outValue = KV[_inKey].Value;


            return true;
        }

        public Dictionary<string, PPCfgContent> GetCopyPairs()
        {
            if (KV == null) return null;

            Dictionary<string, PPCfgContent> res = new Dictionary<string, PPCfgContent>();

            foreach (var item in this.KV)
            {
                PPCfgContent cfgContent = new PPCfgContent();
                item.Value.CopyTo(ref cfgContent);

                res.Add(cfgContent.Key,cfgContent);
            }

            return res;
        }
        


        public override string ToString()
        {
            string res="";
            res = string.Format("[{0}]", sectionName) + "\r\n" + ContentString;

            return res;
        }


        public void Clear()
        {
            KV.Clear();
        }
        
    }

    [Serializable]
    public struct PPCfgContent
    {
        public string Key;
        public string Value;


        public PPCfgContent(string _inKey,string _inValue)
        {
            Key = _inKey;
            Value = _inValue;
        }

        public void CopyTo(ref PPCfgContent _other)
        {
           _other. Key = this.Key;
           _other. Value = this.Value;
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", Key, Value);
        }

    }


}