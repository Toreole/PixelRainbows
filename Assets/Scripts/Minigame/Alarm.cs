using System;
using Audio;
using PixelRainbows.Panels;
using TMPro;
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
        
        // TMP that is somewhere in the scene
        [SerializeField] 
        private TextMeshProUGUI _tmpUGUI;

        // Bool that prevents the player from triggering the clip again after going back and forth (calling the WakeUp Method)
        private bool _isPlaying;

        [SerializeField]
        private string _winMessage;
        private void OnMouseDown()
        {
            _audioSource.Stop();
            _tmpUGUI.text = "" + _winMessage;
            IsDone = true;
        }

        public override void WakeUp()
        {
            if (!IsDone)
            {
                // Plays the soundclip. It will continue to ring even if the player tries to escape the sound by going back to another panel.
                if(!_isPlaying)
                    _alarmClock.Play(_audioSource);
                _tmpUGUI.text = "Stop the alarm!";
                _isPlaying = true;
            }
        }

        public override void CancelMinigame()
        {
            _tmpUGUI.text = "";
        }
    }
}
