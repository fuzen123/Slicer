using UnityEngine;

namespace CuttingMan
{
    public class EndTrigger : CutInteractives
    {
        private void Start()
        {
            GameManager.Instance.LevelLength = transform.position.z - 1.5f;
        }
        public override void DoCut(PlayerCtrl pc)
        {
            GameManager.Instance.OnEndTrigger();
        }
    }
}
