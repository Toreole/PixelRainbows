using System;
using Audio;
using PixelRainbows;
using PixelRainbows.Panels;
using UnityEngine;

namespace Panels
{
    [RequireComponent(typeof(AudioSource))]
    public class ActivateSound : MonoBehaviour
    {
        [SerializeField][Tooltip("Number of this panel in the list of panels")]
        private int _thisPanel; //No. This info is in the panelmanager and should not be a requirement for things to work. Bind it to the PanelManager and CameraController.

        [SerializeField]
        private float _delay;

        [SerializeField][Tooltip("If sound should play only once even if the player goes back check this")]
        private bool _playOnlyOnce;

        // Blocks the sound from being played again
        private bool _blockSound;

        [SerializeField][Tooltip("If sound should play after minigame check this")]
        private bool _waitForMinigame;
        
        private PanelData _panelData;

        private PanelManager _panelManager;

        // The AudioSource from which we play the sound
        private AudioSource _audio;
        
        [SerializeField][Tooltip("Determines how many panels this sound will be played for after this one")]
        private int _panelContinuation;
        
        // Difference of panel to GameProgress.Current
        private int _panelDiff;
        
        [SerializeField][Tooltip("Scriptable Sound that will be played once the panel is reached")]
        private ScriptableAudioEvent _playedSound;
        
        private void Awake()
        {
            //_panelManager = PanelManager.Instance; //-- PanelManager.Instance can NOT be used like this.
            //-- PanelManager.Instance was a EDITOR ONLY thing, that was ultimately never used.
            //_panelData = _panelManager.GetPanel(_thisPanel);

            //we do not need a lot of audio sources in a scene, if we will only have one or two audio clips playing at any time.
            //which is why sound should be handled by the CameraController and the PanelData with a Helper Script at most.
            //I know that this is partly my fault for not communicating enough and giving you more detailed information about the system
            //But ultimately, you should have been aware, that so far anything to do with panels has been handled by the PanelManager.
            //Just like the Minigame.Awake method is called by the CameraController upon showing the Panel that has the minigame.
            _audio = GetComponent<AudioSource>();
            _audio.playOnAwake = false;
        }

        private void SoundManagement()
        {
            // Only do if sound is playing
            if (_audio.isPlaying)
            {
                // If we dont want to play this sound more than once then _blockSound will prevent the sound from playing again
                _blockSound = _playOnlyOnce;
                // We dont want _panelDiff to be bigger than it should be so we clamp it
                //-- What???
                _panelDiff = Mathf.Clamp(_panelDiff, GameProgress.Current - _thisPanel, _panelContinuation);
                // Once we reach a panel that goes beyond our set continuation, we stop the sound
                //-- This is absolutely NOT the way to go.
                //-- You wouldnt load a save-file every frame to check whether some flag is set; you load it once and use it.
                if (_thisPanel + _panelContinuation < GameProgress.Current || _thisPanel > GameProgress.Current)
                {
                    _audio.Stop();
                }
            }
            // Only do if sound is not playing
            if(_audio.isPlaying)
                return; 
            //-- Bro MULTI-LINE YOUR COMMENTS, you cant read things that are off-screen:
            // Plays the sound if the current panel is this panel or the difference is still within the bounds 
            // that we set so even if the player goes back after being outside the range, the sound will play again
            if (GameProgress.Current == _thisPanel + _panelDiff && !_blockSound)
            {
                if (_delay >= 0)
                {
                    _delay -= Time.deltaTime;
                    return;
                }
                //-- If you're going to wait for the minigame to finish...
                //-- you might aswell add the sound *to the minigame*.
                if(_panelData?.Minigame && _waitForMinigame)
                    if(!_panelData.Minigame.IsDone) return;
                
                _playedSound.Play(_audio);
            }
        }
        //Sorry, but this is utter garbage.
        void FixedUpdate()
        {
            SoundManagement();
        }
    }
}
