using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class MenuCanvas : MonoBehaviour
    {
        public List<UpgradeButton> upgradeBtns;
        public void SetButtons(UpgradedData[] data)
        {
            for (int i = 0; i < 3; i++)
            {
                upgradeBtns[i].ButtonInteractable(data[i].Available);
                upgradeBtns[i].SetLevelTxt(data[i].Level);
                upgradeBtns[i].SetPriceTxt((int)data[i].Price);
            }
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }
        //on UI button
        public void BuyStamina()
        {
            GameManager.Instance.BuyStamina();
        }

        //on Ui button
        public void  BuyIncome()
        {
            GameManager.Instance.BuyScoreValueIncrease();
        }

        //on Ui button
        public void BuySpeed()
        {
            GameManager.Instance.BuySpeed();
        }
    }
}
