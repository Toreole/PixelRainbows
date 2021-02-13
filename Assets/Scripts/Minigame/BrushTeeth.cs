using System;
using TMPro;
using UnityEngine;

namespace Minigame
{
    public class BrushTeeth : MinigameBaseClass
    {
        [Header("Scene References")] 
        [SerializeField] // Collider that checks if player brushed the right way
        private BoxCollider2D _brushTeethCheckLeft;
        [SerializeField] // Collider that checks if player brushed the right way
        private BoxCollider2D _brushTeethCheckRight;
        [SerializeField] 
        private Animator _anim;
        [SerializeField] 
        private TextMeshProUGUI _tmpUGUI;
        
        [SerializeField][Tooltip("Sprites that will be disabled")] 
        private GameObject[] _disableExtraSprites;

        private SpriteRenderer _spriteRenderer;
        private Camera _camera;
       
        // Required amount of collisions
        [SerializeField] private int _repetitions = 2;
        
        // Count for the collisions
        private int _leftCheck = 0;
        private int _rightCheck = 0;
        
        [SerializeField]
        private string _winMessage;
        private void Awake()
        {
            _camera = Camera.main;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void WakeUp()
        {
            if(!IsDone)
                _tmpUGUI.text = "Brush your teeth!";
        }

        private void Update()
        {
            BrushTeethAnim();
        }

        // Moves the object that the player has to move to brush the MC's teeth
        private void OnMouseDrag()
        {
            Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = pos;
        }

        // Counts the amount of times the player moved the object from side to side
        // If the player touches one of the colliders, then it will deactivate its collider until the other collider has been touched
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == _brushTeethCheckLeft)
            {
                _leftCheck += 1;
                _tmpUGUI.text = "Back to the right!";
                
                _brushTeethCheckLeft.enabled = false;
                if (_brushTeethCheckRight.enabled == false)
                {
                    _brushTeethCheckRight.enabled = true;
                }
            }

            if (other == _brushTeethCheckRight)
            {
                _rightCheck += 1;
                _tmpUGUI.text = "Back to the left!";
                
                _brushTeethCheckRight.enabled = false;
                if (_brushTeethCheckLeft.enabled == false)
                {
                    _brushTeethCheckLeft.enabled = true;
                }
            }
        }

        // Plays animation after swiping an n(_repetitions)-amount of times to left and right
        // After successfully starting the animation, the object and the helper sprites will disappear
        private void BrushTeethAnim()
        {
            if (_leftCheck >= _repetitions && _rightCheck >= _repetitions)
            {
                // Activate animation...
                _anim.SetBool(BrushTeethReqDone, true);
                // Set the grabbed objects sprite to null...
                _spriteRenderer.sprite = null;
                // display winMessage...
                _tmpUGUI.text = _winMessage;
                // Disable all objects in this array
                if(_disableExtraSprites.Length != 0) 
                {
                    foreach (var disable in _disableExtraSprites)
                    {
                        disable.SetActive(false);
                    }
                }
                // IsDone set to true...
                IsDone = true;
            }
        }
        
        public override void CancelMinigame()
        {
            if (!IsDone)
            {
                _leftCheck = 0;
                _rightCheck = 0;
                _tmpUGUI.text = "";
            }
            else
            {
                _tmpUGUI.text = "";
            }
        }
        
        public override int UpdateProgress(int minimum, int maximum)
        {
            if (IsDone)
            {
                return maximum;
            }

            float progress = (float)(_leftCheck + _rightCheck) / (_repetitions * 2)*maximum;
            return (int) progress;
        }
    }
}
