using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

namespace My3DGame
{
    public class PlayerDataUI : MonoBehaviour
    {
        #region Variables
        public StatsSO statsObject;

        public Image healthBar;
        public Image manaBar;

        public TextMeshProUGUI levelText;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI goldText;
        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            statsObject.OnChangedStats += OnChangedStats;
        }

        private void OnDisable()
        {
            statsObject.OnChangedStats -= OnChangedStats;
        }

        private void Start()
        {
            UpdatePlayData();
        }
        #endregion

        #region Custom Method
        private void UpdatePlayData()
        {
            healthBar.fillAmount = statsObject.HealthPercentage;
            manaBar.fillAmount = statsObject.ManaPercentage;

            levelText.text = statsObject.Level.ToString();

            int needForLevelUp = statsObject.GetExpForLevelUp(statsObject.Level);
            expText.text = statsObject.Exp.ToString() + " / " + needForLevelUp.ToString();
            goldText.text = statsObject.Gold.ToString();
        }

        private void OnChangedStats(StatsSO stats)
        {
            UpdatePlayData();
        }
        #endregion
    }
}
