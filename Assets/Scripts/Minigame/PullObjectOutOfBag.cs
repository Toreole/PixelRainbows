using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Minigame
{
    public class PullObjectOutOfBag : MinigameBaseClass
    {
        // Object which defines the minimum distance for the object to be pulled out of the bag
        [SerializeField]
        private GameObject _minOutOfBagDist;
       
        // The pos where the object will start from in the bag, so that we can clamp the min and the start value
        [SerializeField]
        private GameObject _startPos;

        // What item are we dragging out of the bag?
        [SerializeField]
        private string _item;
        
        // Any message we want to display at the end?
        [SerializeField]
        private string _winMessage;
        
        // Position of mouse while dragging
        private Vector2 _pos;
        
        // Position of the blanket before being dragged
        private Vector2 _originalPos;
        
        // Minimum Distance between bag item and _minOutOfBagDist Object to win the game
        [SerializeField]
        private float _minimumDist = 0.25f;

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
            WakeUp();
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
                _pos.x = Mathf.Clamp(_pos.x, _startPos.transform.position.x, _minOutOfBagDist.transform.position.x);
                _pos.y = Mathf.Clamp(_pos.y, _startPos.transform.position.y, _minOutOfBagDist.transform.position.y);
                _rigidbody2D.position = _pos;
            }

            // Win the game if the player reached a minimum distance
            if (Vector2.Distance(transform.position , _minOutOfBagDist.transform.position) < _minimumDist)
            {
                _tmpUGUI.text = "" + _winMessage;
                IsDone = true;
            }
        }


        public override void WakeUp()
        {
            _startPos.transform.position = transform.position;
            _minOutOfBagDist.transform.position = new Vector3(transform.position.x, _minOutOfBagDist.transform.position.y, _minOutOfBagDist.transform.position.z);
            if(!IsDone && _tmpUGUI != null) 
                _tmpUGUI.text = $"Drag your {_item} out of your bag!";
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
