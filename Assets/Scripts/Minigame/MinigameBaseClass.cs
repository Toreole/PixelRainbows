using UnityEngine;

namespace Minigame
{
    
    ///<summary>
    ///The MinigameBaseClass lets every other Minigame Script inherit from it
    /// so that we can call specific entry and exit methods through our other scripts.
    ///</summary>
    public abstract class MinigameBaseClass : MonoBehaviour
    {
        ///<summary>
        /// Bool that will check if the minigame in this panel was already completed by the player.
        /// </summary> 
        public bool IsDone {  get; protected set; } = false;

        ///<summary>
        /// Method to call after switching to a panel with a minigame.
        /// </summary> 
        public abstract void WakeUp();

        ///<summary>
        /// Method to call after switching from a panel with a minigame to check if we need to reset the progress on the
        /// minigame.
        /// </summary> 
        public abstract void CancelMinigame();

        // A List of required variables for the minigames
        protected static readonly int BrushTeethReqDone = Animator.StringToHash("BrushTeethReqDone");
    }
}
