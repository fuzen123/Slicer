using System;
using UnityEngine;

namespace CuttingMan
{
    public class Cuttables : CutInteractives
    {
        [SerializeField] private GameObject mainmodel;
        [SerializeField] private GameObject[] slicedmodels;
        [SerializeField] private Rigidbody[] slicedRB;
        [SerializeField] private float force = 10f;
        [SerializeField] private float angularForce = 10f;
        [SerializeField] private bool useSlicedMatColor = false;
        [SerializeField] private int innerMaterialIndex = 1;
        [SerializeField] private ParticleSystem cutEffect;
        public Color[] slicedColors;
        
        private static int currentColor = 0;

        private Material innerSlicedMats;
        private Vector3 forcedir = new Vector3(1.1f, 0.9f, -0.5f);
        private Vector3 rotateforcedir = new Vector3(0f, 0f, -0.3f);

        private ParticleSystem.MainModule mmPs = new ParticleSystem.MainModule();

        public override void DoCut()
        {
            mainmodel.SetActive(false);
            for (int i = 0; i < slicedmodels.Length; i++)
            {
                slicedmodels[i].SetActive(true);
            }

            slicedRB[0].AddForce(forcedir * force, ForceMode.Impulse);
            slicedRB[0].AddRelativeTorque(rotateforcedir * angularForce, ForceMode.VelocityChange);
            if (useSlicedMatColor)
            {
                currentColor = (currentColor + 1) % slicedColors.Length;
                innerSlicedMats.color = slicedColors[currentColor];
                mmPs.startColor = slicedColors[currentColor];
            }
            ScoreManager.Instance.AddScore();
            cutEffect.Play();
        }
        private void Start()
        {
            if(useSlicedMatColor)
            {
                //Material[] m = new Material[5];
                Material[] m = slicedmodels[1].GetComponent<MeshRenderer>().materials;
                innerSlicedMats = m[innerMaterialIndex];
            }
            if (useSlicedMatColor)
                mmPs = cutEffect.main;
        }
    }
}
