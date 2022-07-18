using UnityEngine;

namespace CuttingMan
{
    public class EndTrigger : CutInteractives
    {
        public override void DoCut()
        {
            GameManager.Instance.OnEndTrigger();
        }
    }
}
