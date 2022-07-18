using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class PlayerCtrl : MonoBehaviour
    {
        public SkinnedMeshRenderer playerMesh;
        public Movement pMove;
        public Animator plyAnim;
        public ParticleSystem LowSweatEffect, HighSweatEffect;
        public CharDissolve deadChar;

        private Material skinMat;
        private Color targetCol = new Color(0.66f, 0f, 0f, 1f);
        private float fatigueUpdateMultplier = 0.1f;
        public float fatigue = 0f;
        private bool hiFatigue = false;

        public void GoCcut()
        {
            pMove.enabled = true;
            enabled = true;
        }
        public void ReachEnd()
        {
            enabled = false;
            pMove.StopMoving();
            HighSweatEffect.Stop();
            LowSweatEffect.Stop();
        }
        public void ResetToStart()
        {
            enabled = false;
            pMove.enabled = false;
            plyAnim.gameObject.SetActive(true);
            deadChar.Hide();
            transform.position = Vector3.zero;
            skinMat.color = Color.white;
            plyAnim.SetFloat("Blend", 0);
            fatigue = 0f;
            hiFatigue = false;
        }
        public void Init(int speedLvl, int staminaLvl)
        {
            //set speed movement and fatiguerate
            IncreaseMoveSpeed(speedLvl);
            IncreaseStaminaRate(staminaLvl);
            skinMat = playerMesh.material;
            deadChar.Init();
            enabled = false;
            pMove.enabled = false;
        }
        public void IncreaseStaminaRate(int lvl)
        {
            fatigueUpdateMultplier = 1f / Mathf.Sqrt(lvl + lvl + lvl) * 1.618f;
        }
        public void IncreaseMoveSpeed(int lvl)
        {
            float inc = (lvl-1) / 10f;
            pMove.setForwardSpeed(inc);
        }
        private void Update()
        {
            SetTiredVal();
            UpdateFatigue();
        }

        private void SetTiredVal()
        {
            if(pMove.Moving())
            {
                IncreaseFatigue();
            }
            else
            {
                DecreaseFatigue();
            }
        }
        private void IncreaseFatigue()
        {
            fatigue += fatigueUpdateMultplier * 0.26f * Time.deltaTime;
            if (fatigue > 1f)
                FatigueOver();
        }
        private void DecreaseFatigue()
        {
            fatigue -= (fatigueUpdateMultplier * 0.1f ) * Time.deltaTime;
            if (fatigue < 0f)
                fatigue = 0f;
        }
        private void UpdateFatigue()
        {
            plyAnim.SetFloat("Blend", fatigue);
            ColorUpdate();
            FatigueParticleUpdate();
        }
        private void FatigueParticleUpdate()
        {
            if(fatigue > 0.38f && !LowSweatEffect.isPlaying)
            {
                LowSweatEffect.Play();
            }
            else if(fatigue < 0.3f && LowSweatEffect.isPlaying)
            {
                LowSweatEffect.Stop();
                if (HighSweatEffect.isPlaying)
                { HighSweatEffect.Stop(); hiFatigue = false; }
            }
            if(fatigue > 0.55 && !hiFatigue)
            {
                hiFatigue = true;
                HighSweatEffect.Play();
            }
        }
        private void ColorUpdate()
        {
            Color col = Color.Lerp(Color.white, targetCol, fatigue-0.4f);
            skinMat.color = col;
        }
        private void FatigueOver()
        {
            enabled = false;
            pMove.StopMoving();
            plyAnim.gameObject.SetActive(false);
            deadChar.Show(transform.position);
            HighSweatEffect.Stop();
            LowSweatEffect.Stop();
            GameManager.Instance.OnFatigueOver();//todo: use event
        }

    }
}
