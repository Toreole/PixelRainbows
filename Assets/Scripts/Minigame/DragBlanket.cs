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
        private float _distance;
        
        // Distance at which point the MC will drag the blanket back
        private float _dragResistanceDistance = 2.5f;
        
        // Checks if the blanket is being dragged
        private bool _draggingBack;
        
        // Checks if the left mouse button is still being held after the blanket has been dragged back by the MC
        private bool _isButtonStillHeld;
        
        // Counts the amount of times the blanket has been dragged back
        private  int _counter = 0;
        
        public override void WakeUp()
        {
            _tmpUGUI.text = "Drag your blanket!";
        }
        
        private void Awake()
        {
            _collider2D = GetComponent<BoxCollider2D>();
            _originalPos = transform.position;
            _camera = Camera.main;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            DragBack();
           _distance = Vector2.Distance(_pos, _originalPos);
           if (Input.GetMouseButtonUp(0))
           {
               Cursor.visible = true;
               _isButtonStillHeld = false;
           }
           
           if (IsDone)
           {
               Cursor.visible = true;
               _blanket.enabled = false;
               _spriteRenderer.enabled = false;
               _collider2D.enabled = false;
           }
        }

        // Drags the blanket by holding left click
        private void OnMouseDrag()
        {
            if(!_isButtonStillHeld)
                return;
                        
            if (!_draggingBack)
            {
                Cursor.visible = false;
                _pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                _pos.x = Mathf.Clamp(_pos.x, _startTarget.position.x, _endTarget.position.x);
                _pos.y = Mathf.Clamp(_pos.y, _endTarget.position.y, _startTarget.position.y);
                _rigidbody2D.position = _pos;
            }

            if (_distance >= _winDistance && _counter == 3)
            {
                IsDone = true;
            }
        }

        // Drags the blanket back after the player drags it too far away from mc. Wont be called after n(_counter)-amount of times
        private void DragBack()
        {
            Cursor.visible = true;
            if (_distance > _dragResistanceDistance && _counter != 3)
            {
                _draggingBack = true;
            }
            _rigidbody2D.position = Vector2.Lerp(_rigidbody2D.position, _originalPos, _dragSpeed);
            if (_rigidbody2D.position == _originalPos)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _isButtonStillHeld = true;
                }
                if(!_isButtonStillHeld)
                    _draggingBack = false;
                _pos = _originalPos;
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
            if (IsDone == false)
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
