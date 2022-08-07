using TMPro;
using UnityEngine;

namespace CuttingMan
{
    public class CutValueBonus : MonoBehaviour
    {
        public TextMeshPro text;
        private Color col;
        private GameObject localObj;
        public void ShowBonusScore(float bonus)
        {
            localObj = text.gameObject;
            localObj.transform.localRotation = Quaternion.Euler(0f, -55f, 0f);
            col = text.color;
            text.SetText(bonus.ToString());
            localObj.SetActive(true);
            AnimateCollected();
        }
        private void AnimateCollected()
        {
            LeanTween.moveLocal(localObj, localObj.transform.localPosition + new Vector3(6f, 20f, 2f), 1f);
            LeanTween.value(localObj, updateValueExampleCallback, 1f, 0f, 1.2f);//.setDestroyOnComplete(true);
        }
        void updateValueExampleCallback(float val)
        {
            col.a = val;
            text.color = col;
        }
    }
}
