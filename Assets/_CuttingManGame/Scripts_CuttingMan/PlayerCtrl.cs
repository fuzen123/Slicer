using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class PlayerCtrl : MonoBehaviour
    {
        public SkinnedMeshRenderer playerMesh;
        public Movement pMove;
        public KnifeTrigger knife;
        public Animator plyAnim;
        public ParticleSystem LowSweatEffect, HighSweatEffect;
        public GameObject WarningSign;
        public CharDissolve deadChar;

        private Material skinMat;
        private Color targetCol = new Color(0.66f, 0f, 0f, 1f);
        private Color startCol;
        private float fatigueUpdateMultplier = 0.1f;
        public float fatigue = 0f;
        private bool hiFatigue = false;
        private float lowboundWarning, hiboundWarning;
        private float x, y;
        private bool dercresedFat = false;
        private int timesFatdecresed = 0;

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
            WarningSign.SetActive(false);
            deadChar.Hide();
            transform.position = Vector3.zero;
            skinMat.color = startCol;
            plyAnim.SetFloat("Blend", 0);
            fatigue = 0f;
            hiFatigue = false;
            timesFatdecresed = 0;
            x = 0f;
            dercresedFat = false;
        }
        public void Init(int speedLvl, int staminaLvl)
        {
            //set speed movement and fatiguerate
            SetMoveSpeed(speedLvl);
            SetStaminaRate(staminaLvl);
            skinMat = playerMesh.material;
            startCol = skinMat.GetColor("_Color");
            deadChar.Init();
            enabled = false;
            pMove.enabled = false;
            knife.Init(this);
        }
        public void SetStaminaRate(int lvl)
        {
            var lvlbonus = ((lvl / 10) % 10);
            var gamelevelFac = 1f / (GameManager.Instance.GameLevel * GameManager.Instance.GameLevel);
            gamelevelFac *= Mathf.Sqrt(GameManager.Instance.GameLevel);
            fatigueUpdateMultplier = 22f + ((lvl*lvl*(1.4f * gamelevelFac)) + (lvlbonus*10f));
            fatigueUpdateMultplier = 1f / fatigueUpdateMultplier;
            CalcWarningBounds();
        }
        public void SetMoveSpeed(int lvl)
        {
            float inc = ((lvl + lvl) * 4f / 9f) - ((GameManager.Instance.GameLevel - 1) * lvl * 2f / 9f);
            pMove.setForwardSpeed(inc);
        }
        private void Update()
        {
            SetTiredVal();
            UpdateFatigueVisual();
            UpdateUIProgress();
        }

        private void SetTiredVal()
        {
            if(!pMove.Moving())
            {
                DecreaseFatigue();
            }
            else
            {
                if (timesFatdecresed < 3 && dercresedFat)
                {
                    x = 0f;
                    dercresedFat = false;
                }
            }
        }
        public void IncreaseFatigue()
        {
            fatigue += fatigueUpdateMultplier;
            if (fatigue > 1f)
                FatigueOver();
        }
        public void BonusIncreaseFatigue(float bonusMultMod)
        {
            fatigue += fatigueUpdateMultplier * bonusMultMod;
            if (fatigue > 1f)
                FatigueOver();
        }
        private void DecreaseFatigue()
        {
            if (dercresedFat)
                return;
            x += Time.deltaTime*(4f+timesFatdecresed);
            if (x > 1)
            {
                dercresedFat = true;
                timesFatdecresed++;
                return;
            }
            y = Mathf.Pow(1f - x, 5);
            fatigue -= fatigueUpdateMultplier * y;
            if (fatigue < 0f)
                fatigue = 0f;
        }
        private void UpdateUIProgress()
        {
            if(Time.frameCount % 4 == 0)
            {
                GameManager.Instance.UpdateUIProgres(transform.position.z);
            }
        }
        private void UpdateFatigueVisual()
        {
            plyAnim.SetFloat("Blend", fatigue);
            ColorUpdate();
            FatigueParticleUpdate();
            WarningSignUpdateShow();
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

        private void WarningSignUpdateShow()
        {
            if(WarningSign.activeSelf && fatigue < lowboundWarning)
            {
                WarningSign.SetActive(false);
            }
            else if(!WarningSign.activeSelf && fatigue > hiboundWarning)
            {
                WarningSign.SetActive(true);
            }
        }
        private void CalcWarningBounds()
        {
            hiboundWarning = 1f - 3f * fatigueUpdateMultplier;
            lowboundWarning = hiboundWarning - fatigueUpdateMultplier;
        }
        private void ColorUpdate()
        {
            Color col = Color.Lerp(startCol, targetCol, fatigue-0.61f);
            skinMat.color = col;
            skinMat.SetFloat("_Emission1", fatigue);
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
