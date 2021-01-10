using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections;

namespace Minigame
{
    public class DragBlanket : MinigameBaseClass
    {
        [Header("Scene References")]
        [SerializeField] private TextMeshProUGUI _tmpUGUI;
        [SerializeField] private SpriteRenderer _blanket;
        [SerializeField][Tooltip("Top limit of the blanket")] private Transform _startTarget;
        [SerializeField][Tooltip("Bottom limit of the blanket")] private Transform _endTarget;

        [Header("Changable Variables")]
        [SerializeField] [Range(0.01f, 0.1f)][Tooltip("The speed at which the MC drags the blanket")] 
        private float _dragSpeed = 0.1f;
        
        [SerializeField][Range(3.5f, 5f)][Tooltip("The distance the player has to drag the blanket to win")] 
        private float _winDistance;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider2D;
        private Camera _camera;

        // Position of mouse while dragging
        private Vector2 _pos;
        
        // Position of the blanket before being dragged
        private Vector2 _originalPos;
        
        // Distance between the _originalPos and _pos
        [SerializeField]
        private float _distance;
        
        [SerializeField][Tooltip("Distance at which point the MC will drag the blanket back")]
        private float _dragResistanceDistance = 2.5f;
        
        // Checks if the blanket is being dragged
        private bool _draggingBack;
        
        // Checks if the left mouse button is still being held after the blanket has been dragged back by the MC
        private bool _isButtonStillHeld;
        
        // Check it if you want the MC to stand up, else the MC is going to bed instead of waking up
        [SerializeField]
        private bool _standingUp;
        
        // Counts the amount of times the blanket has been dragged back
        private int _counter = 0;
        
        // Max amount of times the blanket has to be dragged back
        [SerializeField]
        private int _maxAmount;
        
        // Any message we want to display at the end?
        [SerializeField]
        private string _winMessage;
        public override void WakeUp()
        {
            if(!IsDone) 
                _tmpUGUI.text = "Drag your blanket!";
        }
        
        private void Awake()
        {
            _collider2D = GetComponent<BoxCollider2D>();
            if (!_standingUp)
            {
                transform.position = _endTarget.transform.position;
                _pos = _endTarget.position;
            }

            _originalPos = transform.position;
            _pos = _originalPos;
            _camera = Camera.main;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody2D.position = _originalPos;
        }

        // Update is called once per frame
        void Update()
        {
            if(_counter <=_maxAmount)
                DragBack();
            
            
            _distance = _standingUp ? Vector3.Distance(_startTarget.position, transform.position) : Vector2.Distance(_pos, _endTarget.transform.position);
            
            if (Input.GetMouseButtonUp(0))
            {
                Cursor.visible = true;
                _isButtonStillHeld = false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                _isButtonStillHeld = true;
            }
            
            if (IsDone && _standingUp)
            {
              
                Cursor.visible = true;
                _blanket.enabled = false;
                _spriteRenderer.enabled = false;
                _collider2D.enabled = false;
            }
            else if(IsDone && !_standingUp)
            {
                Cursor.visible = true;
                _collider2D.enabled = false;
            }
        }

        // Drags the blanket by holding left click
        private void OnMouseDrag()
        {
            if(_standingUp)
                if(!_isButtonStillHeld) 
                    return;
                        
            if (!_draggingBack && _standingUp)
            {
                Cursor.visible = false;
                _pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                _pos.x = Mathf.Clamp(_pos.x, _startTarget.position.x, _endTarget.position.x);
                _pos.y = Mathf.Clamp(_pos.y, _endTarget.position.y, _startTarget.position.y);
                _rigidbody2D.position = _pos;
            }
            else if(!_standingUp)
            {
                Cursor.visible = false;
                _pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                _pos.x = Mathf.Clamp(_pos.x, _startTarget.position.x, _endTarget.position.x);
                _pos.y = Mathf.Clamp(_pos.y, _endTarget.position.y, _startTarget.position.y);
                _rigidbody2D.position = _pos;
            }

            if (_standingUp)
            {
                if (_distance >= _winDistance && _counter == _maxAmount)
                {
                    _tmpUGUI.text = "" + _winMessage;
                    IsDone = true;
                }
            }
            else if(!_standingUp)
            {
                if (_distance >= _winDistance)
                {
                    _tmpUGUI.text = "" + _winMessage;
                    IsDone = true;
                }
            }
        }

        // Drags the blanket back after the player drags it too far away from mc. Wont be called after n(_counter)-amount of times
        private void DragBack()
        {
            if(!_standingUp)
                return;
            Cursor.visible = true;
            if (_distance > _dragResistanceDistance && _counter != _maxAmount)
            {
                _draggingBack = true;
            }

            if (_draggingBack)
            {
                transform.position = Vector3.Lerp(transform.position, _startTarget.transform.position, _dragSpeed);
            }

            if (Vector3.Distance(transform.position, _startTarget.transform.position) < 0.1f)
            {
                if(!_isButtonStillHeld)
                    _draggingBack = false;
                transform.position = _startTarget.transform.position;
            }
        }

        // Increases counter
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(_draggingBack)
                _counter += 1;
        }

        public override void CancelMinigame()
        {
            if (!IsDone)
            {
                _counter = 0;
                _tmpUGUI.text = "";
            }
            else
            {
                _tmpUGUI.text = "";
            }
        }
    }
}
