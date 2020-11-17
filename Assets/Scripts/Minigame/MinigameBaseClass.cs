using UnityEngine;

namespace Minigame
{
    public class MinigameBaseClass : MonoBehaviour
    {
        public bool IsDone {  get; protected set; } = false;

        public virtual void WakeUp()
        {
        
        }

        public virtual void CancelMinigame()
        {
            
        }

      
    
    
    }
}
