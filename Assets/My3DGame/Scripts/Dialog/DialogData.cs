using My3DGame.Manager;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace My3DGame
{
    /// <summary>
    /// 대화창 데이터 파일 읽어 오기
    /// </summary>
    public class DialogData : ScriptableObject
    {
        #region Variables
        public Dialogs dialogs;             // 대화창 데이터베이스
        private string dataPath = "Data/DialogData";
        #endregion

        // 생성자
        public DialogData() { }

        // 대화창 데이터 읽어 오기
        public void LoadData()
        {
            TextAsset asset = (TextAsset)ResourcesManager.Load(dataPath);
            // asset 체크
            if(asset == null || asset.text == null)
            {
                return;
            }

            using(XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
            {
                var xs = new XmlSerializer(typeof(Dialogs));
                dialogs = (Dialogs)xs.Deserialize(reader);
            }
        }
    }
}