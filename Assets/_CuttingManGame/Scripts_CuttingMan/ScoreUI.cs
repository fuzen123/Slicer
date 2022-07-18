using TMPro;
using UnityEngine;

namespace CuttingMan
{
    public class ScoreUI : MonoBehaviour
    {
        public TextMeshProUGUI scoreTxt;

        public void SetScoreTxt(float score)
        {
            scoreTxt.SetText(score.ToString());
        }
    }
}
