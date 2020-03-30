using System.Collections.Generic;

using UnityEngine;

using HololensPETS;

namespace HololensPETSGames
{
    public class FlappyBirdGameState : MonoBehaviour
    {
        public ObservableValue<int> score = new ObservableValue<int>();

        public ObservableValue<bool> isGameOver = new ObservableValue<bool>();

        public bool isPlaying = false;
        
        public Dictionary<Finger, bool> ActiveFingers;

        private void Awake()
        {
            ActiveFingers = new Dictionary<Finger, bool>();

            for(int i = 0; i < 5; i++)
            {
                ActiveFingers.Add((Finger)i, false);
            }
        }
    }
}
