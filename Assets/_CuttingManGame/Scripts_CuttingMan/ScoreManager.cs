using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance;

        public ScoreUI scoreui;

        private float score = 0;
        private float cutScoreValue = 0.1f;

        public void AddScore()
        {
            score += cutScoreValue;
            scoreui.SetScoreTxt(Mathf.Floor(score));
        }
        public float AddBonusScore(float multiplier)
        {
            float v = cutScoreValue * multiplier;
            score += v;
            scoreui.SetScoreTxt(Mathf.Floor(score));
            return v;
        }
        public void BuyedStat(float costAmount)
        {
            score -= costAmount;
            scoreui.SetScoreTxt(Mathf.Floor(score));
        }
        public float Score()
        {
            return score;
        }
        public float GetValue()
        {
            return cutScoreValue;
        }
        public void UpgradeScoreValue(int lvl)
        {
            var bonusLvl = (lvl / 10) % 10;
            cutScoreValue = lvl / 10f + ((bonusLvl*lvl) / 20f);
        }

        public void Init(float savedScore, int scoreValueLevel)
        {
            score = savedScore;
            scoreui.SetScoreTxt(Mathf.Floor(score));
            UpgradeScoreValue(scoreValueLevel);
        }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }
}
