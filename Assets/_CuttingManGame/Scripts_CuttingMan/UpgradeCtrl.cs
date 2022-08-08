using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class UpgradeCtrl : MonoBehaviour
    {
        public DataSerialization dataserial;

        public UpgradedData[] upgradedData; //serialize to file, and SCORE

        [Header(" for testing")]
        public bool useTestValue;
        public float score = 0;
        [Range(1, 50)]
        public int level = 1;

        private LevelsData savedLvlData = new LevelsData();
        public void Init()
        {
            upgradedData = new UpgradedData[3];
            if(useTestValue)
            {
                for (int i = 0; i < 3; i++)
                {
                    upgradedData[i] = new UpgradedData { Level = level, Price = PriceCalc(level)};
                    upgradedData[i].Available = score >= upgradedData[i].Price;
                }
            }
            else
            {
                //read from file
                savedLvlData = dataserial.LoadData();
                score = savedLvlData.score;
                for (int i = 0; i < 3; i++)
                {
                    upgradedData[i] = new UpgradedData();
                }
                upgradedData[0].Level = savedLvlData.stamina;
                upgradedData[1].Level = savedLvlData.income;
                upgradedData[2].Level = savedLvlData.speed;
                for (int i = 0; i < 3; i++)
                {
                    upgradedData[i].Price = PriceCalc(upgradedData[i].Level);
                    upgradedData[i].Available = score >= upgradedData[i].Price;
                }
            }
        }
        public void UpgradeStat(int statIndex)
        {
            upgradedData[statIndex].Level++;
            upgradedData[statIndex].Price = PriceCalc(upgradedData[statIndex].Level);
            UpdateAvailabilityAfterUpgrade();
            savedLvlData.score = Mathf.Round(ScoreManager.Instance.Score() * 10f) / 10f;
            savedLvlData.stamina = upgradedData[0].Level;
            savedLvlData.income = upgradedData[1].Level;
            savedLvlData.speed = upgradedData[2].Level;
        }
        public void UpdateOnReplay()
        {
            UpdateAvailabilityAfterUpgrade();
            savedLvlData.score = Mathf.Round(ScoreManager.Instance.Score() * 10f) / 10f;
            savedLvlData.stamina = upgradedData[0].Level;
            savedLvlData.income = upgradedData[1].Level;
            savedLvlData.speed = upgradedData[2].Level;
        }

        private void UpdateAvailabilityAfterUpgrade()
        {
            for (int i = 0; i < 3; i++)
            {
                upgradedData[i].Available = ScoreManager.Instance.Score() > upgradedData[i].Price;
            }
        }
        private float PriceCalc(int level)
        {
            float x = Mathf.Pow(level, 1.618f);
            x = Mathf.Floor(x);
            return x;
        }
        private void OnDisable()
        {
            dataserial.SaveData(savedLvlData);
        }
    }
}
