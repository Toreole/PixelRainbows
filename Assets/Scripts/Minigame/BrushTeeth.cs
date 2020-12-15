using System;
using TMPro;
using UnityEngine;

namespace Minigame
{
    public class BrushTeeth : MinigameBaseClass
    {
        [Header("Scene References")]
        [SerializeField] private BoxCollider2D _brushTeethCheckLeft;
        [SerializeField] private BoxCollider2D _brushTeethCheckRight;
        [SerializeField] private Animator _anim;
        [SerializeField] private TextMeshProUGUI _tmpUGUI;

        private Rigidbody2D _rigidbody2D;
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
            _rigidbody2D = GetComponent<Rigidbody2D>();
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
            _rigidbody2D.position = pos;
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
        // After successfully starting the animation, the object will disappear
        private void BrushTeethAnim()
        {
            if (_leftCheck >= _repetitions && _rightCheck >= _repetitions)
            {
                _anim.SetBool(BrushTeethReqDone, true);
                _spriteRenderer.sprite = null;
                _tmpUGUI.text = "" + _winMessage;
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
    }
}
