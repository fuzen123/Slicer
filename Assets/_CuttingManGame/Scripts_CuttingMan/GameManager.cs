using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [HideInInspector]public UpgradeCtrl upgCtrl;
        public MenuCanvas menuCanvas;
        public Canvas end;
        public Canvas winCanvas;
        public float SecondDelayToShowEnd = 3f;
        public Canvas start;
        public PlayerCtrl playerctrl;
        public GameObject CuttablesLvlPrefab;
        public Flag flag;
        private GameObject cutlvl;

        private Dictionary<string, object> parameters;
        private int level = 1;
        private int currentAttempt = 0;
        [SerializeField] GamePlayClikEvents clickEvents;
        
        //on UI button
        public void TaptoStart()
        {
            currentAttempt++;
            start.enabled = false;
            menuCanvas.Close();
            playerctrl.GoCcut();
            parameters = new Dictionary<string, object>();
            int currscore = Mathf.FloorToInt(ScoreManager.Instance.Score());
            parameters.Add("Current money collected", $"{currscore}");
            parameters.Add("Attempted level pass", $"{currentAttempt}");
            clickEvents.StartLevel(level, parameters);
        }
        //on UI button
        public void PlayAgainLevel()
        {
            upgCtrl.UpdateOnReplay();
            menuCanvas.SetButtons(upgCtrl.upgradedData);
            end.enabled = false;
            winCanvas.enabled = false;
            menuCanvas.Show();
            start.enabled = true;
            if (cutlvl != null)
                Destroy(cutlvl);
            cutlvl = Instantiate(CuttablesLvlPrefab);
            playerctrl.ResetToStart();
        }

        public void OnFatigueOver()
        {
            flag.SetOnPlace(playerctrl.transform.position);
            StartCoroutine(CanvasDelay());
            clickEvents.SendFullFatigue(parameters);
        }
        public void OnEndTrigger()
        {
            clickEvents.SendLevelWin(parameters);
            level++;
            winCanvas.enabled = true;
            playerctrl.ReachEnd();
        }

        public void BuyScoreValueIncrease()
        {
            ScoreManager.Instance.BuyedStat(upgCtrl.upgradedData[1].Price);
            upgCtrl.UpgradeStat(1);
            ScoreManager.Instance.UpgradeScoreValue(upgCtrl.upgradedData[1].Level);
            menuCanvas.SetButtons(upgCtrl.upgradedData);
        }
        public void BuyStamina()
        {
            ScoreManager.Instance.BuyedStat(upgCtrl.upgradedData[0].Price);
            upgCtrl.UpgradeStat(0);
            playerctrl.IncreaseStaminaRate(upgCtrl.upgradedData[0].Level);
            menuCanvas.SetButtons(upgCtrl.upgradedData);
        }
        public void BuySpeed()
        {
            ScoreManager.Instance.BuyedStat(upgCtrl.upgradedData[2].Price);
            upgCtrl.UpgradeStat(2);
            playerctrl.IncreaseMoveSpeed(upgCtrl.upgradedData[2].Level);
            menuCanvas.SetButtons(upgCtrl.upgradedData);
        }
        private void OnGameStart()
        {
            winCanvas.enabled = false;
            end.enabled = false;
            start.enabled = true;
            menuCanvas.Show();
            cutlvl = Instantiate(CuttablesLvlPrefab);
            upgCtrl.Init();
            playerctrl.Init(upgCtrl.upgradedData[2].Level, upgCtrl.upgradedData[0].Level);
            ScoreManager.Instance.Init(upgCtrl.score, upgCtrl.upgradedData[1].Level);
            menuCanvas.SetButtons(upgCtrl.upgradedData);
        }
        private void Start()
        {
            OnGameStart();
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            upgCtrl = GetComponent<UpgradeCtrl>();
            clickEvents = GetComponent<GamePlayClikEvents>();
        }
        private IEnumerator CanvasDelay()
        {
            yield return new WaitForSeconds(SecondDelayToShowEnd);
            end.enabled = true;
        }
    }
}
