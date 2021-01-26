using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelRainbows
{
    public class PanelAnimator : Minigame.MinigameBaseClass
    {
        [SerializeField, Tooltip("Animation Length in seconds. Required for the last panel in a scene. Otherwise optional.")]
        private float animationLength = 0f;

        Animator anim;
        private void Awake() 
        {
            anim = GetComponent<Animator>(); //just double check    
            anim.enabled = false;
        }
        public override void CancelMinigame()
        {
            //this isnt really a minigame so yeah.
        }

        public override int UpdateProgress(int minimum, int maximum)
        {
            return 0;
        }

        public override void WakeUp()
        {
            anim.enabled = true;
            if(animationLength <= 0)
                IsDone = true;
            else 
                StartCoroutine(Wait());
            IEnumerator Wait()
            {
                yield return new WaitForSeconds(animationLength); IsDone = true;
            }
        }
    }
}