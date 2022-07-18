using TMPro;
using UnityEngine;

namespace CuttingMan
{
    public class CutValue : MonoBehaviour
    {
        public TextMeshPro text;
        LTDescr lt1;
        private Color col;

        private float camAngle;

        private void Start()
        {
            col = text.color;
            var v = ScoreManager.Instance.GetValue();
            text.SetText(v.ToString());
            lt1 = LeanTween.moveLocalY(gameObject, transform.position.y + 7f, 0.75f);
            LeanTween.value(gameObject, updateValueExampleCallback, 1f, 0f, 0.8f).setDestroyOnComplete(true);
        }
        void updateValueExampleCallback(float val)
        {
            col.a = val;
            text.color = col;
        }
        private void Awake()
        {
            text = GetComponent<TextMeshPro>();
            transform.localRotation = Quaternion.Euler(0f, -55f, 0f);
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (text == null)
                text = GetComponent<TextMeshPro>();
            Camera[] c1 = GameObject.FindObjectsOfType<Camera>();
            for (int i = 0; i < c1.Length; i++)
            {
                if (c1[i].gameObject.activeSelf)
                    camAngle = c1[i].transform.rotation.eulerAngles.y;
            }
            //camAngle = GameObject.FindObjectOfType<Camera>().transform.rotation.eulerAngles.y;
            transform.localRotation = Quaternion.Euler(0f, camAngle, 0f);
        }
#endif
    }
}
