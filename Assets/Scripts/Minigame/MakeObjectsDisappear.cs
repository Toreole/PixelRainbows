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

        [SerializeField][Tooltip("If this is enabled, the last sprite in the switch mode will still be displayed")]
        private bool _keepLastSprite;

        private bool _lastSpriteKeptAndDone;
        
        // Counter that goes lower if we make the 
        private int _counter;

        
        // Start is called before the first frame update
        void Awake()
        {
            // Ensure we dont get mixing modes which will break the code
            if (_switchMode)
            {
                _enableMode = false;
            }
            #if UNITY_EDITOR
            WakeUp();
            #endif
        }

        // Update is called once per frame
        void Update()
        {
            // Shoot ray if Left Mouse Click is pressed
            if (Input.GetMouseButtonDown(0))
            {
                CastRay();
            }

            if (_counter == _sprites.Length || _lastSpriteKeptAndDone)
            {
                IsDone = true;
                _tmpUGUI.text = $"{_winMessage}";
            }
          
        }

        // Cast a ray that checks which object we are hitting
        private void CastRay()
        {
            // Get the mouse position on the screen
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Shoot towards the clicked direction
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            // If it hits nothing, return
            if (!hit) return;
            
            // DisableMode: Disable clicked object in the array
            if (_sprites.Contains(hit.collider.gameObject) && !_enableMode && !_switchMode)
            {
                hit.collider.gameObject.SetActive(false);
                _counter += 1;
            }
            // EnableMode: Enable clicked object's spriterenderer in the array
            else if (_sprites.Contains(hit.collider.gameObject) && _enableMode)
            {
                var spriteRenderer = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.enabled = true;
                _counter += 1;
            }
            // SwitchMode: Switch to the next object in the array
            else if (_sprites.Contains(hit.collider.gameObject) && _switchMode)
            {
                if(_keepLastSprite && _counter + 1 == _sprites.Length)
                {
                    _lastSpriteKeptAndDone = true;
                    return;  
                }
                hit.collider.gameObject.SetActive(false);
                _counter += 1;
                if (_counter < _sprites.Length)
                {
                    _sprites[_counter].gameObject.SetActive(true);
                }
            }
        }

        public override void WakeUp()
        {
            _counter = 0;
            _tmpUGUI.text = $"{_message}";
            var count = 0;
            if (!IsDone && _tmpUGUI != null && !_switchMode)
            {
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
                // Disable all objects except the first one
                foreach (var sprite in _sprites)
                {
                    sprite.gameObject.SetActive(false);
                    _sprites[0].gameObject.SetActive(true);
                }
            }
            else
            {
                // If we already won the game, set text to nothing
                _tmpUGUI.text = "";
            }
        }

        public override void CancelMinigame()
        {
            _tmpUGUI.text = "";
        }
    }
}
