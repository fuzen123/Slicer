using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class Flag : MonoBehaviour
    {
        public TextMesh textM;

        private Vector3 flagPos;

        public void SetOnPlace(Vector3 pos)
        {
            int z = Mathf.FloorToInt(pos.z);
            textM.text = $"{z}m";
            flagPos = transform.position;
            flagPos.z = pos.z;
            transform.position = flagPos;
        }
    }
}
