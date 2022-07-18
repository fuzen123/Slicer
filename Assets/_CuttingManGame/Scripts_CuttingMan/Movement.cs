using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class Movement : MonoBehaviour
    { 
        [SerializeField]private float forwardSpeed = 7f;
        [SerializeField]private Animator playerAnim;
        private bool isAccing = false;
        private float targetAcc = 0f;
        private float acc = 0f;
        private bool isMoving = false;
        public float speed;

        public void setForwardSpeed(float increaseVal)
        {
            speed = forwardSpeed + increaseVal;
        }
        public bool Moving() 
        { 
            if (isMoving)// && !isAccing)
                return true;
            return false;
        }
        public void StopMoving()
        {
            enabled = false;
            isMoving = false;
            playerAnim.SetBool("run", false);
            acc = 0f;
            isAccing = false;
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                playerAnim.SetBool("run", true);
                targetAcc = 1f;
                isAccing = true;
                isMoving = true;
            }
            if(Input.GetMouseButtonUp(0))
            {
                playerAnim.SetBool("run", false);
                targetAcc = 0f;
                isAccing = true;
                isMoving = false;
            }
            else if(Input.GetMouseButton(0))
            {
                transform.Translate(acc * speed * Time.deltaTime * Vector3.forward);
            }
            if (isAccing)
                CalcAcceleration();
        }
        private void CalcAcceleration()
        {
            acc = Mathf.MoveTowards(acc, targetAcc, 2f * Time.deltaTime);
            if (targetAcc == 0f)
                transform.Translate(acc * speed * Time.deltaTime * Vector3.forward);
            if (acc == targetAcc)
            {
                isAccing = false;
            }
        }
    }
}
