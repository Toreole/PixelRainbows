using System.Collections;
using UnityEngine.Serialization;
using UnityEngine;

namespace PixelRainbows
{
    public class PanelAnimator : Minigame.MinigameBaseClass
    {
        [SerializeField, Tooltip("Any Additional Delay. Optional."), FormerlySerializedAs("animationLength"), Range(0f, 10f)]
        private float delay = 0f;

        private float animationLength;
        Animator anim;
        private void Awake() 
        {
            anim = GetComponent<Animator>(); //just double check    
            anim.enabled = false;
            
            //see how long the clip to be played is.
            AnimatorClipInfo[] clipinfos = anim.GetCurrentAnimatorClipInfo(0);
            animationLength = clipinfos[0].clip.length;
        }
        public override void CancelMinigame()
        {
            //this isnt really a minigame so yeah.
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
                yield return new WaitForSeconds(animationLength + delay); IsDone = true;
            }
        }
    }
}