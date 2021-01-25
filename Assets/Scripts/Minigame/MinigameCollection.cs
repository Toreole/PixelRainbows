using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame
{
    public class MinigameCollection : MinigameBaseClass
    { 
        [SerializeField]
        private MinigameBaseClass[] _minigames;
        
        private int _isDoneCount;

        private List<GameObject> _isDoneList = new List<GameObject>();
        public override void WakeUp()
        {
            foreach (var Done in _minigames)
            {
                Done.WakeUp();
                if (!_isDoneList.Contains(Done.gameObject))
                {
                    _isDoneList.Add(Done.gameObject);
                }
            }
        }

        private void Update()
        {
            foreach (var Done in _minigames)
            {
                if (Done.IsDone && _isDoneList.Contains(Done.gameObject))
                {
                    _isDoneCount++;
                    _isDoneList.Remove(Done.gameObject);
                }
            }

            if (_isDoneCount == _minigames.Length)
            {
                IsDone = true;
            }
        }

        public override void CancelMinigame()
        {
            _isDoneCount = 0;
            foreach (var Done in _minigames)
            {
               Done.CancelMinigame();
            }
        }
    }
}
