using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class KnifeTrigger : MonoBehaviour
    {
        private PlayerCtrl plyCtrl;
        public void Init(PlayerCtrl pc)
        {
            plyCtrl = pc;
        }
        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<CutInteractives>().DoCut(plyCtrl);
            plyCtrl.IncreaseFatigue();
        }
    }
}
