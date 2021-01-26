using TMPro;
using UnityEngine;

namespace Minigame
{
    public class DragToTarget : MinigameBaseClass
    {
        // Object which defines the minimum distance for the object to be pulled out of the bag
        [SerializeField][Tooltip("Our target object we want to be dragged to")]
        private GameObject _target;

        [SerializeField][Tooltip("Percentage of distance to target at which point the object will pull back")]
        private float _minPercentageToTarget = 0.3f;
        
        [SerializeField][Tooltip("Percentage of distance to target at which the object will stop and the player can resume pulling")]
        private float _returnLimit = 0.8f;
        
        [SerializeField][Tooltip("Modifies the speed at which the object pulls back")]
        private float _returnSpeedModifer = 0.5f;

        [SerializeField][Tooltip("Used by returnSpeedModifer to change speed over time")]
        private AnimationCurve _pushCurve;
        
        [SerializeField][Tooltip("How often will the players confidence waver?")]
        private int _pushRepetitions;
        
        [SerializeField][Tooltip("If you want to switch images, turn this on")]
        private bool _switchOffAndOn;
        
        [SerializeField][Tooltip("Objects that will be disabled on win if switch is on")]
        private GameObject[] _disableObjects;
        
        [SerializeField][Tooltip("Objects that will be enabled on win if switch is on")]
        private GameObject[] _enableObjects;
        
        [SerializeField][Tooltip("The pos where the object will start from, so that we can clamp the min and the max value")]
        private GameObject _startPos;

        [SerializeField][Tooltip("Object that will restraint the drag of the object the player is dragging. Think of it as the corner of a box and place it on the bottom left")]
        private GameObject _leftRestraint;

        [SerializeField][Tooltip("Object that will restraint the drag of the object the player is dragging. Think of it as the corner of a box and place it on the top right")]
        private GameObject _rightRestraint;
        
        [SerializeField][Tooltip("What item are we dragging out of the bag?")]
        private string _message;

        [SerializeField][Tooltip("Any message we want to display at the end?")]
        private string _winMessage;
        
        // Position of mouse while dragging
        private Vector2 _pos;
        
        // Position of the blanket before being dragged
        private Vector2 _originalPos;
        
        [SerializeField][Tooltip("Minimum Distance between bag item and _minOutOfBagDist Object to win the game")]
        private float _minimumDist = 0.25f;
        
        [SerializeField][Tooltip("If the dragged object needs to be disabled once we reached the target, then check this as true")]
        private bool _needsDisabling;
       
        // Get Left Mouse Click Input
        private bool _dragging => Input.GetMouseButton(0);
      
        
        [SerializeField] 
        private TextMeshProUGUI _tmpUGUI;

        private SpriteRenderer _spriteRenderer;
        private Camera _camera;
        private bool _movingBack;
        private bool _isButtonStillHeld;
        private int _savedRepetitions;

        private void Awake()
        {
            // Reset position
            transform.position = _startPos.transform.position;
            // Get camera
            _camera = Camera.main;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Disable all objects, that we enable later
            if(_switchOffAndOn)
            {
                foreach (var enable in _enableObjects)
                {
                    enable.SetActive(false);
                }
            }
            // Save repetition amount to reset if incomplete
            _savedRepetitions = _pushRepetitions;
        }

        private void PushAgainst()
        {
            // Assign positions
            var pos = transform.position;
            var startPos = _startPos.transform.position;
            var target = _target.transform.position;
            
            // Calculate distances
            var distToTarget = Vector3.Distance(startPos, target);
            var remainingDistToTarget = Vector3.Distance(pos, target);
            var distToStart = Vector3.Distance(pos, startPos);
            var distPercentage = remainingDistToTarget / distToTarget;
          
            // Evaluate the curve from the inspector
            var speed = _pushCurve.Evaluate(distToStart/(distToTarget * _returnLimit));
            
            // If we are close enough and still need to do repetitions set to true
            if (distPercentage < _minPercentageToTarget)
            {
                _movingBack = true;
            }
            // If we didnt reach the breakpoint yet, keep lerping
            if (_movingBack && distPercentage < _returnLimit)
            {
                // Lerp pos back to start 
                pos = Vector3.Lerp(pos, startPos, speed * _returnSpeedModifer*Time.deltaTime);
            }
            // If we reached the breakpoint and the player releases the mousebutton 0, reduce repetitions by one and set bool false
            if (_movingBack && distPercentage >= _returnLimit && !_isButtonStillHeld)
            {
                _pushRepetitions -= 1;
                _movingBack = false;
            }
            // Move position to pos
            transform.position = pos;
        }
        // Reenable Mouse Visibility after letting go of the Left Mouse Button
        private void MouseInvis()
        {
            if (_dragging == false)
                Cursor.visible = true;
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Cursor.visible = true;
                _isButtonStillHeld = false;
            }
            if (Input.GetMouseButton(0)) 
            {
                _isButtonStillHeld = true;
            }
            if(_pushRepetitions != 0)
                PushAgainst();
            if (IsDone)
            {
                // Turn all objects off if length is not zero and the bool is true
                if (_switchOffAndOn && _disableObjects.Length != 0)
                {
                    foreach (var disable in _disableObjects)
                    {
                        disable.SetActive(false);
                    }
                }
                // Turn all objects on if length is not zero and the bool is true
                if(_switchOffAndOn && _enableObjects.Length !=0) 
                {
                    foreach (var enable in _enableObjects)
                    {
                        enable.SetActive(true);
                    }
                }
                // Cursor is visible
                Cursor.visible = true;
                // Disables this draggable objects sprite
                if (_needsDisabling)
                    _spriteRenderer.enabled = false;
                // Make sprite undraggable after winning the game by disabling this
                this.enabled = false;
            }
        }
        

        // Dragging the object
        private void OnMouseDrag()
        {
            if (_isButtonStillHeld && _movingBack)
            {
                return;
            }
                    
            MouseInvis();
            if (!IsDone && _dragging && !_movingBack)
            {
                // Lock it in the transforms of the other objects
                Cursor.visible = false;
                _pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                
                // Get rid of transform.position calls
                Vector3 leftPos = _leftRestraint.transform.position;
                Vector3 rightPos = _rightRestraint.transform.position;
                
                // Clamp values to their min and max
                float minX = Mathf.Min(leftPos.x, rightPos.x);
                float maxX = Mathf.Max(leftPos.x, rightPos.x);
                float minY = Mathf.Min(leftPos.y, rightPos.y);
                float maxY = Mathf.Max(leftPos.y, rightPos.y);
                _pos.x = Mathf.Clamp(_pos.x, minX, maxX);
                _pos.y = Mathf.Clamp(_pos.y, minY, maxY);
               
                // Move position to pos
                transform.position = _pos;
            }

            var distToTarget = Vector2.Distance(transform.position, _target.transform.position);
            Debug.Log(distToTarget);
            // Win the game if the player reached a minimum distance
            if (distToTarget < _minimumDist && _pushRepetitions == 0)
            {
                // Move the position of this object to the target to match better...
                transform.position = _target.transform.position;
                // display winMessage...
                _tmpUGUI.text = _winMessage;
                // Set IsDone true after moving the object
                if(transform.position == _target.transform.position)
                    IsDone = true;
            }
        }


        public override void WakeUp()
        {
            if(!IsDone && _tmpUGUI != null) 
                _tmpUGUI.text = _message;
        }

        public override void CancelMinigame()
        {
            if (!IsDone)
            {
                // Reset position to start
                transform.position = _startPos.transform.position;
                // Reset repetitions
                _pushRepetitions = _savedRepetitions;
                _tmpUGUI.text = "";
            }
            else
            {
                _tmpUGUI.text = "";
            }
        }
        
        public override int UpdateProgress(int minimum, int maximum)
        {
            var myTransformPosition = transform.position;
            var myStartPos = _startPos.transform.position;
            var myEndPos = _target.transform.position;
            if (IsDone)
            {
                return maximum;
            }
            float progress = Vector3.Distance(myStartPos, myTransformPosition)/Vector3.Distance(myStartPos, myEndPos)*100;
            return (int) progress;        }
    }
}