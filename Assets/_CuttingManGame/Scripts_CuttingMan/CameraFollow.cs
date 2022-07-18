using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform player;
        public float smTm = 0.3f;
        private Vector3 refVel = Vector3.zero;
        private Vector3 followOffset = Vector3.zero;

        void Start()
        {
            followOffset = player.position - transform.position;
        }
        private void LateUpdate()
        {
            //transform.position = player.position - followOffset;
            transform.position = Vector3.SmoothDamp(transform.position, player.position-followOffset, ref refVel, smTm);
        }
    }
}
