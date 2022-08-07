using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CuttingMan
{
    public class UIProgresBar : MonoBehaviour
    {
        public Slider slid;

        public void SetSlider(float val)
        {
            slid.value = val;
        }
    }
}
