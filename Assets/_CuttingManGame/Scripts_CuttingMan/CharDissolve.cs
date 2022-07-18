using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class CharDissolve : MonoBehaviour
    {
        public SkinnedMeshRenderer skmr;

        private Material dsmat;
        public void Show(Vector3 pos)
        {
            transform.position = pos;
            gameObject.SetActive(true);
            DissolveMatA();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
            dsmat.SetFloat("_DissolveValue", 0);
        }
        public void Init()
        {
            dsmat = skmr.material;
            gameObject.SetActive(false);
            dsmat.SetFloat("_DissolveValue", 0);
        }

        private void DissolveMatA()
        {            
            LeanTween.value(gameObject, updateDis, 0f, 1f, 1f);
        }
        private void updateDis(float val)
        {
            dsmat.SetFloat("_DissolveValue", val);
        }
    }
}
