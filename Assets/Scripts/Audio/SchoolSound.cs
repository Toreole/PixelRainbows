using UnityEngine;

namespace Audio
{
    public class SchoolSound : AudioData
    {
        [SerializeField]
        private ScriptableAudioEvent[] _audio;

        [SerializeField][Range(1, 30)]
        private int _length;
        
        [SerializeField][Range(1, 30)]
        private int _lengthForExtra;

        public override int StartSound(AudioSource audioSource, int panel)
        {
            _audio[0].Play(audioSource);
            PanelNumber = panel;
            return _length;
        }

        public override int StartExtraSound(AudioSource audioSource, int panel)
        {
            if(!HasMultipleSounds) return 0;
            _audio[1].Play(audioSource);
            ExtraPanelNumber = panel;
            return _lengthForExtra;
        }
        
        public override void CancelSound(AudioSource audioSource, int endPanel)
        {
            EndPanelNumber = endPanel - 1;
            audioSource.Stop();
        }

        public override void CancelExtraSound(AudioSource audioSource, int endPanel)
        {
            if(!HasMultipleSounds) return;
            ExtraEndPanelNumber = endPanel - 1;
            audioSource.Stop();
        }
    }
}
