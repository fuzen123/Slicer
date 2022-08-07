using System.Collections;
using UnityEngine;

namespace CuttingMan
{
    public class BonusCuttable : CutInteractives
    {
        [SerializeField] private Transform upObj;
        [SerializeField] private float ObjLength = 1f;
        [SerializeField] private float scoreMultiplier = 2f;
        [SerializeField] private ParticleSystem cuttingEffect;
        public Vector3 throwForce;
        public Vector3 torqVec;

        private float startCutPos = 0f;
        private float currPos = 1f;
        private bool isCutting = false;
        private Transform knife;
        private Vector3 startPos;
        private CutValueBonus cutScoreUI;
        private ParticleSystem.EmitParams emp = new ParticleSystem.EmitParams();
        private Vector3 particleeffectOffset = new Vector3(3f, 8f, 3f);

        private float cutFatiguePoint;
        private float currentFatPoint = 0f;
        private PlayerCtrl plyCtrl;
        

        public override void DoCut(PlayerCtrl pc)
        {
            plyCtrl = pc;
            knife = pc.transform;
            startCutPos = knife.transform.position.z;
            startPos = upObj.transform.localPosition;
            isCutting = true;
            emp.applyShapeToPosition = true;

            cutFatiguePoint = ObjLength / 5f;
            currentFatPoint = cutFatiguePoint;
        }
        private void Update()
        {
            if(isCutting)
            {
                float progress = ((knife.transform.position.z - startCutPos) * -1f / ObjLength) + 1;
                if (progress >= currPos)
                    return;

                if (progress <= 0f)
                {
                    //upObj.transform.parent = null;
                    upObj.localRotation = Quaternion.identity;
                    Rigidbody rigBody = upObj.GetComponent<Rigidbody>();
                    rigBody.isKinematic = false;
                    rigBody.AddForce(throwForce, ForceMode.Impulse);
                    rigBody.AddTorque(torqVec, ForceMode.Impulse);
                    isCutting = false;
                    float v = ScoreManager.Instance.AddBonusScore(scoreMultiplier);
                    cutScoreUI.ShowBonusScore(v);
                }
                //cut object visual
                float angle = (progress - 1f) * -1.35f;
                upObj.localRotation = Quaternion.Euler(angle, 0f, 0f);
                upObj.transform.localPosition = startPos + (Vector3.up * (progress - 1f) * -0.5f);
                particleeffectOffset.z = knife.transform.position.z + 3f;
                emp.position = particleeffectOffset;
                cuttingEffect.Emit(emp, 1);
                currPos = progress;
                if((ObjLength-currentFatPoint) >= (progress*ObjLength) )
                {
                    currentFatPoint += cutFatiguePoint;
                    plyCtrl.BonusIncreaseFatigue(0.7f);
                }
            }
        }
        private void Awake()
        {
            cutScoreUI = GetComponent<CutValueBonus>();
        }

    }
}