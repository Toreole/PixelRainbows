using PixelRainbows.Audio;
using UnityEngine;

namespace PixelRainbows
{
    public class ActivateSound : MonoBehaviour
    {
        [SerializeField]
        private float _delay;

        [SerializeField][Tooltip("If sound should play only once even if the player goes back check this")]
        private bool _playOnlyOnce;

        // Blocks the sound from being played again
        private bool _blockSound;

        [SerializeField][Tooltip("If sound should play after minigame check this")]
        private bool _waitForMinigame;
        
        [SerializeField][Tooltip("Determines how many panels this sound will be played for after this one")]
        private int _panelContinuation;
        
        [SerializeField][Tooltip("Scriptable Sound that will be played once the panel is reached")]
        private ScriptableAudioEvent _playedSound;

        public int Duration => _panelContinuation;
        public bool IsBeingPlayed { get; set; } = false;
        public bool CanBePlayed => !_blockSound; //if this blocks sound, it can not be played.
        
        public void PlaySound(AudioSource source)
        {
            //failcheck.
            if(source.isPlaying || _blockSound)
                return; 
            
            _blockSound = _playOnlyOnce;
            IsBeingPlayed = true;
            _playedSound.Play(source);
        }
    }
}