using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using My3DGame.Manager;

namespace My3DGame.GameData
{
    /// <summary>
    /// 사운드 클립 리스트를 관리하는 클래스
    /// BaseData 상속 받아 BaseData의 속성과 기능을 상속
    /// 속성 : 사운드 클립 리스트
    /// 기능 : 사운드 클립 파일(xml, json)로 저장, 로드
    /// </summary>

    public class SoundData : BaseData
    {
        #region Variables
        private string xmlFilePath = string.Empty;
        private string xmlFileName = "soundData.xml";      // 저장 파일 이름
        private string dataPath = "Data/soundData";        // Resources.Load 저장 경로
        #endregion

        #region Property
        public List<SoundClip> soundClips { get; set; }
        #endregion

        #region Constructor
        public SoundData() { }
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
                var xs = new XmlSerializer(typeof(List<SoundClip>));
                // 데이터 클립 리스트의 아이디와 이름 세팅
                int length = GetDataCount();
                for (int i = 0; i < length; i++)
                {
                    soundClips[i].Id = i;
                    soundClips[i].Name = this.names[i];
                }
                // 저장
                xs.Serialize(xml, soundClips);
            }
        }

        // 이펙트 데이터 파일(xml) 로드
        public void LoadData()
        {
            // 데이터 파일(xml) 가져오기
            TextAsset asset = (TextAsset)ResourcesManager.Load(dataPath);

            // 데이터 파일 체크
            if (asset == null || asset.text == null)
            {
                // 데이터가 없을 때 무조건 새로운 데이터 하나 추가
                AddData("NewSound");
                return;
            }

            using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
            {
                // 직렬화
                var xs = new XmlSerializer(typeof(List<SoundClip>));
                soundClips = (List<SoundClip>)xs.Deserialize(reader);

                // 읽어들인 내용을 이름 목록에 세팅
                int length = soundClips.Count;
                this.names = new List<string>();
                for (int i = 0; i < length; i++)
                {
                    this.names.Add(soundClips[i].Name);
                }
            }
        }

        // 새로운 데이터 추가하기
        public override int AddData(string newName)
        {
            // 데이터 파일 체크
            if (this.names == null)
            {
                this.names = new List<string>() { newName };
                this.soundClips = new List<SoundClip>() { new SoundClip() };
            }
            else
            {
                this.names.Add(newName);                // 데이터 이름 리스트에 새로운 이름 추가
                soundClips.Add(new SoundClip());      // 데이터 클립 리스트에 새로운 클립 추가
            }

            return GetDataCount();
        }

        // 데이터 제거하기
        public override void RemoveData(int index)
        {
            this.names.Remove(names[index]);
            // 마지막 이름을 제거하면
            if (this.names.Count == 0)
            {
                this.names = null;
            }

            this.soundClips.Remove(soundClips[index]);
            // 마지막 클립 데이터를 제거하면
            if (this.soundClips.Count == 0)
            {
                this.soundClips = null;
            }
        }

        // 지정된 인덱스의 데이터 복사하기
        public override void CopyData(int index)
        {
            // names[index] 를 복사
            this.names.Add(names[index]);
            // soundClips[index] 를 복사
            soundClips.Add(GetCopy(index));
        }

        // soundClips[index] 의 속성을 복사한 soundClip 반환
        public SoundClip GetCopy(int index)
        {
            // index 체크
            if (index < 0 || index >= this.soundClips.Count)
            {
                return null;
            }

            SoundClip original = this.soundClips[index];
            SoundClip newClip = new SoundClip();
            newClip.Name = original.Name;
            newClip.Type = original.Type;
            newClip.ClipPath = original.ClipPath;
            newClip.ClipName = original.ClipName;
            // 추가된 속성도 복사

            return newClip;
        }

        // 데이터 클리어
        public void ClearData()
        {
            foreach (var clip in soundClips)
            {
                // soundClips 해제
                clip.ReleaseClip();
            }
            this.soundClips = null;

            this.names = null;
        }

        // 지정된 인덱스의 오디오 클립 가져오기
        public SoundClip GetSoundClip(int index)
        {
            // index 체크
            if (index < 0 || index >= soundClips.Count)
            {
                return null;
            }

            // effectPrefab 로드하기
            this.soundClips[index].PreLoad();
            return this.soundClips[index];
        }
        #endregion
    }
}

