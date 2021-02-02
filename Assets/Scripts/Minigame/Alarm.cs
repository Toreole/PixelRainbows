using System;
using PixelRainbows.Audio;
using PixelRainbows.Panels;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Minigame
{
    public class Alarm : MinigameBaseClass
    {
        // SO that has the associated audioclip
        [SerializeField][Tooltip("Put the desired ScriptableAudioEvent in here. You can adjust its settings by clicking on the SerializedObject in the folder.")]
        private ScriptableAudioEvent _alarmClock;

        // SO that is somewhere in the scene
        [SerializeField]
        private AudioSource _audioSource;

        // Gets the animator from an object
        private Animator _animator;

        // Resets the rotation after an animation back to zero
        [SerializeField]
        private GameObject _resetAnimGameObject;
        
        // TMP that is somewhere in the scene
        [SerializeField] 
        private TextMeshProUGUI _tmpUGUI;

        // Bool that prevents the player from triggering the clip again after going back and forth (calling the WakeUp Method)
        private bool _isPlaying;

        [SerializeField]
        private string _winMessage;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        private void OnMouseDown()
        {
            // On Mouse Down disable animator...
            _animator.enabled = false;
            // reset the animated object's euler rotation to 0...
            _resetAnimGameObject.transform.localRotation = Quaternion.identity;
            // stop the sound...
            _audioSource.Stop();
            // display winMessage...
            _tmpUGUI.text = _winMessage;
            // set IsDone to true
            IsDone = true;
            // and disable this script
            this.enabled = false;
        }
        public override void WakeUp()
        {
            if (!IsDone)
            {
                // Plays the soundclip. It will continue to ring even if the player tries to escape the sound by going back to another panel...
                if(!_isPlaying)
                    _alarmClock.Play(_audioSource);
                // Turn animator on...
                _animator.enabled = true;
                // Tell player what to do if not obvious enough...
                _tmpUGUI.text = "Stop the alarm!";
                // Set this bool true
                _isPlaying = true;
            }
        }

        public override void CancelMinigame()
        {
            _tmpUGUI.text = "";
        }
    }
}
