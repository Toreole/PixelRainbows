using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minigame
{
    public class DragToTarget : MinigameBaseClass
    {
        // Object which defines the minimum distance for the object to be pulled out of the bag
        [SerializeField][Tooltip("Our target object we want to be dragged to")]
        private GameObject _target;
        
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

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private Camera _camera;
        
        private void Awake()
        {
            _camera = Camera.main;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Reenable Mouse Visibility after letting go of the Left Mouse Button
        private void MouseInvis()
        {
            if (_dragging == false)
                Cursor.visible = true;
        }

        private void Update()
        {
            MouseInvis();

            // Make sprite undraggable after winning the game
            if (IsDone)
            {
                _rigidbody2D.simulated = false;
                if (_needsDisabling)
                    _spriteRenderer.enabled = false;
            }
        }
        

        // Dragging the object
        private void OnMouseDrag()
        {
            if (!IsDone && _dragging)
            {
                // Lock it in the transforms of the other objects
                Cursor.visible = false;
                _pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                _pos.x = Mathf.Clamp(_pos.x, _leftRestraint.transform.position.x, _rightRestraint.transform.position.x);
                _pos.y = Mathf.Clamp(_pos.y, _leftRestraint.transform.position.y, _rightRestraint.transform.position.y);
                _rigidbody2D.position = _pos;
            }

            // Win the game if the player reached a minimum distance
            if (Vector2.Distance(transform.position , _target.transform.position) < _minimumDist)
            {
                Vector2.Lerp(transform.position, _target.transform.position, 0.5f * Time.deltaTime);
                _tmpUGUI.text = "" + _winMessage;
                if(transform.position == _target.transform.position)
                    IsDone = true;
            }
        }


        public override void WakeUp()
        {
            if(!IsDone && _tmpUGUI != null) 
                _tmpUGUI.text = $"{_message}";
        }

        public override void CancelMinigame()
        {
            if (!IsDone)
            {
                transform.position = _startPos.transform.position;
                _tmpUGUI.text = "";
            }
            else
            {
                _tmpUGUI.text = "";
            }
        }
    }
}