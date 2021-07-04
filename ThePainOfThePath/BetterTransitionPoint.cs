using GlobalEnums;
using UnityEngine;

namespace ThePainOfThePath
{
    internal class BetterTransitionPoint : MonoBehaviour
    {
        private bool _triggered;
        
        public string SceneName { private get; set; }
        public string EntryGateName { private get; set; }
        public bool SetBackwards { private get; set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (GameManager.instance.gameState != GameState.PLAYING) return;

            if (SetBackwards)
            {
                ThePainOfThePath.Backwards = true;
            }

            HeroController.instance.MaxHealth();
            HeroController.instance.AddMPChargeSpa(999);
            
            GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
            {
                SceneName = SceneName,
                EntryGateName = EntryGateName,
                HeroLeaveDirection = GatePosition.door,
                EntryDelay = 0,
                WaitForSceneTransitionCameraFade = true,
                Visualization = GameManager.SceneLoadVisualizations.GrimmDream,
                AlwaysUnloadUnusedAssets = true
            });

            _triggered = true;
        }
        
        private void OnTriggerStay2D(Collider2D obj)
        {
            if (_triggered) 
                return;
            
            OnTriggerEnter2D(obj);
        }
    }
}