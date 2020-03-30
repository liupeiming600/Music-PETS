using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HololensPETS;

namespace HololensPETSGames
{
    public class PlayerController : MonoBehaviour
    {
        private FlappyBirdGameState m_gameState;

        private void Awake()
        {
            m_gameState = GameObject.FindObjectOfType<FlappyBirdGameState>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            
        }

        public void Move( Vector3 direction )
        {

        }

        private void FingerDataReceivedHandler( Dictionary<Finger, double> data )
        {
            if( m_gameState.isGameOver.Value )
            {
                return;
            }
        }
    }
}
