using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelRainbows
{
    public class PanelAnimator : Minigame.MinigameBaseClass
    {
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

        public override void WakeUp()
        {
            anim.enabled = true;
            IsDone = true;
        }
    }
}