using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using My3DGame.Manager;
using System;
using My3DGame.Manager;

namespace My3DGame
{
    // 데이터 파일을 읽어서 대화 정보 가져오기
    // 대화창 그리기
    public class DrawDialog : MonoBehaviour
    {
        #region Variables
        // 현재 대화
        private Queue<Dialog> dialogs = new Queue<Dialog>();

        // UI
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI sentenceText;
        public GameObject npcImage;
        public GameObject nextButton;

        private bool isTyping = false;
        private string tmpSentence = "";

        public Action OnCloseDialog;
        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            InitDialog();
        }
        #endregion

        #region Custom Method
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

            OnCloseDialog = null;
        }

        // 매개 변수로 들어온 인덱스의 Dialog 시작하기
        public void StartDialog(int dialogIndex)
        {
            // 모든 노드에서 현재 대화 노드를 찾아 Queue에 저장
            foreach (var dialog in DataManager.GetDialogData().dialogs.dialogs)
            {
                if(dialog.number == dialogIndex)
                {
                    dialogs.Enqueue(dialog);
                }
            }

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
            // 대화 종료 시 등록된 함수 호출 이벤트 처리
            OnCloseDialog?.Invoke();

            // 다이얼로그 종료 연출

            // 다이얼로그 초기화
            InitDialog();
        }
        #endregion
    }
}

