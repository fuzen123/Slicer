using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace CuttingMan
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI lvlTxt;
        [SerializeField]private TextMeshProUGUI priceTxt;
        [SerializeField]private Image availabilityIcon;

        public void SetLevelTxt(int lvlNum)
        {
            lvlTxt.SetText($"lvl {lvlNum}");
        }
        public void SetPriceTxt(int priceValue)
        {
            priceTxt.text = priceValue.ToString();
        }
        public void ButtonInteractable(bool state)
        {
            availabilityIcon.enabled = !state;
            gameObject.GetComponentInChildren<Button>().interactable = state;
        }
    }
}
