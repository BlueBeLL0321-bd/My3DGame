using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using My3DGame.Manager;

namespace My3DGame.GameData
{
    /// <summary>
    /// 이펙트 데이터 리스트를 관리하는 클래스
    /// BaseData 상속 받아 BaseData의 속성과 기능을 상속
    /// 속성 : 이펙트 데이터 리스트
    /// 기능 : 이펙트 데이터 파일(xml, json)로 저장, 로드
    /// </summary>
    public class EffectData : BaseData
    {
        #region Variables
        private string xmlFilePath = string.Empty;
        private string xmlFileName = "effectData.xml";      // 저장 파일 이름
        private string dataPath = "Data/effectData";        // Resources.Load 저장 경로
        #endregion

        #region Property
        public List<EffectClip> effectClips { get; set; }
        #endregion

        #region Contractor
        public EffectData() { }
        #endregion

        #region Custom Method
        // 이펙트 데이터 파일(xml)로 저장
        public void SaveData()
        {
            // 저장 경로
            xmlFilePath = Application.dataPath + dataDirectory;
            Debug.Log($"xmlFilePath : {xmlFilePath}");

            using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode))
            {
                // 직렬화
                var xs = new XmlSerializer(typeof(List<EffectClip>));
                // 데이터 클립 리스트의 아이디와 이름 세팅
                int length = GetDataCount();
                for (int i = 0; i < length; i++)
                {
                    effectClips[i].Id = i;
                    effectClips[i].Name = this.names[i];
                }
                // 저장
                xs.Serialize(xml, effectClips);
            }
        }

        // 이펙트 데이터 파일(xml) 로드
        public void LoadData()
        {
            // 데이터 파일(xml) 가져오기
            TextAsset asset = (TextAsset)ResourcesManager.Load(dataPath);

            // 데이터 파일 체크
            if(asset == null || asset.text == null)
            {
                // 데이터가 없을 때 무조건 새로운 데이터 하나 추가
                AddData("NewEffect");
                return;
            }

            using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
            {
                // 직렬화
                var xs = new XmlSerializer(typeof(List<EffectClip>));
                effectClips = (List<EffectClip>)xs.Deserialize(reader);

                // 읽어들인 내용을 이름 목록에 세팅
                int length = effectClips.Count;
                this.names = new List<string>();
                for (int i = 0; i < length; i++)
                {
                    this.names.Add(effectClips[i].Name);
                }
            }
        }

        // 새로운 데이터 추가하기
        public override int AddData(string newName)
        {
            // 데이터 파일 체크
            if(this.names == null)
            {
                this.names = new List<string>() { newName };
                this.effectClips = new List<EffectClip>() { new EffectClip() };
            }
            else
            {
                this.names.Add(newName);                // 데이터 이름 리스트에 새로운 이름 추가
                effectClips.Add(new EffectClip());      // 데이터 클립 리스트에 새로운 클립 추가
            }

            return GetDataCount();
        }

        // 데이터 제거하기
        public override void RemoveData(int index)
        {
            this.names.Remove(names[index]);
            // 마지막 이름을 제거하면
            if(this.names.Count == 0)
            {
                this.names = null;
            }

            this.effectClips.Remove(effectClips[index]);
            // 마지막 클립 데이터를 제거하면
            if(this.effectClips.Count == 0)
            {
                this.effectClips = null;
            }
        }

        // 지정된 인덱스의 데이터 복사하기
        public override void CopyData(int index)
        {
            // names[index] 를 복사
            this.names.Add(names[index]);
            // effectClips[index] 를 복사
            effectClips.Add(GetCopy(index));
        }

        // effectClips[index] 의 속성을 복사한 effectClip 반환
        public EffectClip GetCopy(int index)
        {
            // index 체크
            if(index < 0 || index >= this.effectClips.Count)
            {
                return null;
            }

            EffectClip original = this.effectClips[index];
            EffectClip newClip = new EffectClip();
            newClip.Name = original.Name;
            newClip.Type = original.Type;
            newClip.EffectPath = original.EffectPath;
            newClip.EffectName = original.EffectName;
            return newClip;
        }

        // 데이터 클리어
        public void ClearData()
        {
            foreach (var clip in effectClips)
            {
                // effectPrefab 해제
                clip.ReleaseEffect();
            }
            this.effectClips = null;

            this.names = null;
        }

        // 지정된 인덱스의 이펙트 클립 가져오기
        public EffectClip GetEffectClip(int index)
        {
            // index 체크
            if(index < 0 || index >= effectClips.Count)
            {
                return null;
            }

            // effectPrefab 로드하기
            this.effectClips[index].PreLoad();
            return this.effectClips[index];
        }
        #endregion
    }
}