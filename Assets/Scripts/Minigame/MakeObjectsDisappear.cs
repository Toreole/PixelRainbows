using System.Linq;
using TMPro;
using UnityEngine;

namespace Minigame
{
    public class MakeObjectsDisappear : MinigameBaseClass
    {
        [SerializeField][Tooltip("Objects that we want to change. If their collider does not match, just reset collider.")]
        private GameObject[] _sprites;

        [SerializeField] 
        private TextMeshProUGUI _tmpUGUI;
        
        [SerializeField][Tooltip("What item are we dragging out of the bag?")]
        private string _message;
        
        [SerializeField][Tooltip("Any message we want to display at the end?")]
        private string _winMessage;

        [SerializeField][Tooltip("If this is enabled, the sprite will be enabled instead of disabled")]
        private bool _enableMode;
        
        [SerializeField][Tooltip("If this is enabled, the sprite will be changed to the next sprite in the list instead of disabled")]
        private bool _switchMode;
        
        // Counter that goes lower if we make the 
        private int _counter;

        
        // Start is called before the first frame update
        void Awake()
        {
            if(!_switchMode)
                _counter = _sprites.Length;

            if (_switchMode)
            {
                _enableMode = false;
            }

            WakeUp();
        }

        // Update is called once per frame
        void Update()
        {
            // Shoot ray if Left Mouse Click is pressed
            if (Input.GetMouseButtonDown(0))
            {
                CastRay();
            }

            if (_counter == 0)
            {
                IsDone = true;
                _tmpUGUI.text = $"{_winMessage}";
            }
        }

        // Cast a ray that checks which object we are hitting
        private void CastRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            if (hit)
            {
                // Disable clicked object in the array
                if (_sprites.Contains(hit.collider.gameObject) && !_enableMode && !_switchMode)
                {
                    hit.collider.gameObject.SetActive(false);
                    _counter -= 1;
                }
                // Enable clicked object's spriterenderer in the array
                else if (_sprites.Contains(hit.collider.gameObject) && _enableMode)
                {
                    var spriteRenderer = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.enabled = true;
                    _counter -= 1;
                }
                // Switch to the next object in the array
                else if (_sprites.Contains(hit.collider.gameObject) && _switchMode)
                {
                    hit.collider.gameObject.SetActive(false);
                    _counter += 1;
                    if (_counter < _sprites.Length)
                    {
                        _sprites[_counter].gameObject.SetActive(true);
                    }
                }
            }
        }

        public override void WakeUp()
        {
            if (!IsDone && _tmpUGUI != null && !_switchMode)
            {
                _tmpUGUI.text = $"{_message}";
                _counter = _sprites.Length;
                var count = 0;
                // Enable all objects but disable their SpriteRenderer
                if (_enableMode)
                {
                    foreach (var sprite in _sprites)
                    {
                        sprite.SetActive(true);
                        var spriteRenderer = _sprites[count].gameObject.GetComponent<SpriteRenderer>();
                        spriteRenderer.enabled = false;
                        count += 1;
                    }
                }
                else if(!_enableMode)
                {
                    // Enable all objects and enable their SpriteRenderer
                    foreach (var sprite in _sprites)
                    {
                        sprite.gameObject.SetActive(true);
                        var spriteRenderer = _sprites[count].gameObject.GetComponent<SpriteRenderer>();
                        spriteRenderer.enabled = true;
                        count += 1;
                    }
                }
            }
            else if (!IsDone && _tmpUGUI != null && _switchMode)
            {
                _tmpUGUI.text = $"{_message}";
                _counter = 0;
                
                foreach (var sprite in _sprites)
                {
                    sprite.gameObject.SetActive(false);
                    _sprites[0].gameObject.SetActive(true);
                }
            }
            else
            {
                _tmpUGUI.text = "";
            }
        }

        public override void CancelMinigame()
        {
            if (!IsDone && _enableMode)
            {
                foreach (var sprite in _sprites)
                {
                    sprite.gameObject.SetActive(true);
                }
            }
            else if (!IsDone && !_enableMode && !_switchMode)
            {
                foreach (var sprite in _sprites)
                {
                    sprite.gameObject.SetActive(false);
                }
            }
            else if (!IsDone && _switchMode)
            {
                foreach (var sprite in _sprites)
                {
                    sprite.gameObject.SetActive(false);
                    _sprites[0].gameObject.SetActive(true);
                }
            }
            else
            {
                _tmpUGUI.text = "";
            }
        }
    }
}
