using Tabtale.TTPlugins;
using UnityEngine;
using System.Collections.Generic;

namespace CuttingMan
{
    public class GamePlayClikEvents : MonoBehaviour
    {
        public void StartLevel(int level, Dictionary<string, object> parameters)
        {
            //TTPGameProgression.FirebaseEvents.MissionStarted(level, parameters);
        }
        public void SendFullFatigue(Dictionary<string, object> parameters)
        {
            //TTPGameProgression.FirebaseEvents.MissionFailed(parameters);
        }
        public void SendLevelWin(Dictionary<string, object> parameters)
        {
            //TTPGameProgression.FirebaseEvents.MissionComplete(parameters);
        }
    }
}
