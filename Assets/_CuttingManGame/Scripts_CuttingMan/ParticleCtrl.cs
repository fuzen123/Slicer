using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class ParticleCtrl : MonoBehaviour
    {
        public ParticleSystem[] cuteffects;

        private ParticleSystem.EmitParams emp = new ParticleSystem.EmitParams();
        private Vector3 offset = new Vector3(0f, 4.5f, -4.5f);
        public void PlayCutEffect(Vector3 pos, int indexId)
        {
            emp.position = pos + offset;
            cuteffects[indexId].Emit(emp, 10);
        }

        private void Start()
        {
            emp.applyShapeToPosition = true;
            //Cuttables.OnCutEffect += PlayCutEffect;
        }
        private void OnDestroy()
        {
            //Cuttables.OnCutEffect -= PlayCutEffect;
        }
    }
}
