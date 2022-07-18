using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingMan
{
    public class DataSerialization : MonoBehaviour
    {
        public void SaveData(LevelsData ld)
        {
            string json = JsonUtility.ToJson(ld);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/sessionData.dat", json);
        }
        public LevelsData LoadData()
        {
            string path = Application.persistentDataPath + "/sessionData.dat";
            LevelsData ld = new LevelsData();
            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);

                ld = JsonUtility.FromJson<LevelsData>(json);
                //ld.income = data.income;
                //ld.score = data.score;
                //ld.speed = data.speed;
                //ld.stamina = data.stamina;
            }
            else
            {
                ld.income = 1;
                ld.score = 0f;
                ld.speed = 1;
                ld.stamina = 1;
            }

            return ld;
        }
    }
}
