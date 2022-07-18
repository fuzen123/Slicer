using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class KnifeTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<CutInteractives>().DoCut();
        }
    }
}
