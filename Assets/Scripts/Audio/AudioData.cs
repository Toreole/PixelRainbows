using UnityEngine;

namespace Audio
{
    public abstract class AudioData : MonoBehaviour
    {
        public int PanelNumber { get; protected set; } = 0;
        
        public int ExtraPanelNumber{ get; protected set; } = 0;
        public int EndPanelNumber { get; protected set; } = 0;
        public int ExtraEndPanelNumber { get; protected set; } = 0;
        public bool HasMultipleSounds {  get; protected set; } = false;
        
        public abstract int StartSound(AudioSource audioSource, int panel);
        
        public abstract int StartExtraSound(AudioSource audioSource, int panel);
        
        public abstract void CancelSound(AudioSource audioSource, int endPanel);
        
        public abstract void CancelExtraSound(AudioSource audioSource,  int endPanel);
    }
}
