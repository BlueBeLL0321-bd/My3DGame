using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MySample
{
    // 카드 정보를 가져와서 카드를 그린다
    public class DrawCard : MonoBehaviour
    {
        #region Variables
        // 카드 정보
        public Card card;       // 카드 정보를 담은 스크립터블 오브젝트

        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI manaCostText;
        public TextMeshProUGUI attackText;
        public TextMeshProUGUI healthText;

        public Image artWork;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 카드 정보 그리기
            UpdateCard();
        }
        #endregion

        #region Custom Method
        // 카드 그리기 - 카드 정보를 UI에 연결
        private void UpdateCard()
        {
            nameText.text = card.name;
            descriptionText.text = card.description;
            manaCostText.text = card.manaCost.ToString();
            attackText.text = card.attack.ToString();
            healthText.text = card.health.ToString();

            artWork.sprite = card.artImage;
        }
        #endregion
    }
}