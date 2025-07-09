using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace My3DGame
{
    // 데이터 파일을 읽어서 대화 정보 가져오기
    // 대화창 그리기
    public class DrawDialog : MonoBehaviour
    {
        #region Variables
        // 데이터 파일 읽기
        [SerializeField]
        private string xmlFile = "Dialog";          // 데이터 파일 이름
        private XmlNodeList allNodes;               // 모든 대화 리스트

        // 현재 대화
        private Queue<Dialog> dialogs;

        // UI
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI sentenceText;
        public GameObject npcImage;
        public GameObject nextButton;

        private bool isTyping = false;
        private string tmpSentence = "";
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 데이터 읽기
            LoadDialogXml(xmlFile);

            // 초기화
            dialogs = new Queue<Dialog>();
            InitDialog();
        }
        #endregion

        #region Custom Method
        // xml 파일 읽어서 XmlNodeList에 넣기
        private void LoadDialogXml(string filename)
        {
            var xmlTextFile = Resources.Load<TextAsset>("Dialog/" + filename);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlTextFile.text);
            allNodes = xmlDoc.SelectNodes("root/dialog");
        }
        
        // Dialog 초기화
        private void InitDialog()
        {
            // Queue 비우기
            dialogs.Clear();

            // UI 초기화
            npcImage.SetActive(false);
            nameText.text = "";
            sentenceText.text = "";

            nextButton.SetActive(false);
        }

        // 매개 변수로 들어온 인덱스의 Dialog 시작하기
        public void StartDialog(int dialogIndex)
        {
            // 모든 노드에서 현재 대화 노드를 찾아 Queue에 저장
            foreach (XmlNode node in allNodes)
            {
                int num = int.Parse(node["number"].InnerText);
                if(num == dialogIndex)
                {
                    Dialog dialog = new Dialog();
                    dialog.number = num;
                    dialog.character = int.Parse(node["character"].InnerText);
                    dialog.name = node["name"].InnerText;
                    dialog.sentence = node["sentence"].InnerText;

                    dialogs.Enqueue(dialog);
                }
            }

            // 다이얼로그 시작 연출
            /*if(dialogs.Count > 0)
            {

            }*/

            // 첫 번째 대화 보여 주기
            DrawNext();
        }

        // Queue에 있는 대화 내용을 꺼내 보여 준다
        public void DrawNext()
        {
            // dialogs 체크
            if(dialogs.Count <= 0)
            {
                EndDialog();
                return;
            }

            // 다음 버튼 안 보이기
            nextButton.SetActive(false);

            // 현재 보여 줄 대화를 큐에서 대화 내용 꺼내기
            Dialog dialog = dialogs.Dequeue();

            // npc 이미지 보여주기
            if(dialog.character > 0)
            {
                npcImage.SetActive(true);
                npcImage.GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Dialog/Npc/npc0" + dialog.character.ToString());
            }
            else
            {
                npcImage.SetActive(false);
            }

            // 대화 캐릭터 이름
            nameText.text = dialog.name;

            // 대화 내용
            tmpSentence = dialog.sentence;
            StartCoroutine(TypingSentence(dialog.sentence));
        }

        // 대화 내용 타이핑 연출
        IEnumerator TypingSentence(string typingText)
        {
            isTyping = true;
            sentenceText.text = "";

            foreach (var letter in typingText)
            {
                sentenceText.text += letter;
                yield return new WaitForSeconds(0.03f);
            }

            // 다음 대화 버튼 보이기
            nextButton.SetActive(true);
            isTyping = false;
        }

        public void SkipTyping()
        {
            // 현재 타이핑 연출
            if (isTyping == false)
                return;

            // 코루틴 종료
            StopAllCoroutines();

            sentenceText.text = tmpSentence;

            // 다음 대화 버튼 보이기
            nextButton.SetActive(true);
            isTyping = false;
        }

        // 대화 종료
        public void EndDialog()
        {
            // 다이얼로그 초기화
            InitDialog();

            // 다이얼로그 종료 연출
        }
        #endregion
    }
}

