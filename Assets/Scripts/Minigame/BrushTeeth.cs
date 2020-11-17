using System;
using TMPro;
using UnityEngine;

namespace Minigame
{
    public class BrushTeeth : MonoBehaviour
    {
        [Header("Scene References")] 
        [SerializeField]
        private BoxCollider2D _brushTeethCheckLeft;
        [SerializeField]
        private BoxCollider2D _brushTeethCheckRight;
        [SerializeField]
        private Animator _anim;
        [SerializeField]
        private TextMeshProUGUI _tmpUGUI;
        
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private Camera _camera;
    
        private int leftCheck = 0;
        private int rightCheck = 0;

        private void Awake()
        {
            if(_tmpUGUI  == null)
                _tmpUGUI = FindObjectOfType<TextMeshProUGUI>();
            _tmpUGUI.text = "Brush your teeth!";
            
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _camera = Camera.main;
        }

        private void OnMouseDrag() 
        {
            Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            _rigidbody2D.position = pos;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            
            if (other == _brushTeethCheckLeft)
            {
                leftCheck += 1;
                _tmpUGUI.text = "Back to the right!";
            }

            if (other == _brushTeethCheckRight)
            {
                rightCheck += 1;
                _tmpUGUI.text = "Back to the left!";
            }
        }

        private void BrushTeethAnim()
        {
            if (leftCheck >= 2 && rightCheck >= 2)
            {
                _tmpUGUI.text = "";
                _anim.SetBool("BrushTeethReqDone", true);
                _spriteRenderer.sprite = null;
            }
        }

        private void Update()
        {
            BrushTeethAnim();
        }
    }
}
