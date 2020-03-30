using UnityEngine;

namespace HololensPETSGames
{
    public class GameOverTrigger : MonoBehaviour
    {
        private FlappyBirdGameState m_gameState;

        private void Awake()
        {
            m_gameState = GameObject.FindObjectOfType<FlappyBirdGameState>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                m_gameState.isGameOver.Value = true;
            }
        }

        private void OnTriggerEnter2D( Collider2D other )
        {
            if( !m_gameState.isPlaying )
            {
                return;
            }

            if( other.gameObject.CompareTag( "Player" ) )
            {
                m_gameState.isGameOver.Value = true;
            }
        }
    }
}
