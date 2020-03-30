using UnityEngine;

namespace HololensPETSGames
{
    public class FlappyBirdUIManager : MonoBehaviour
    {
        public GameObject gameOverPanel;

        private FlappyBirdGameState m_gameState;

        private void Awake()
        {
            m_gameState = GameObject.FindObjectOfType<FlappyBirdGameState>();
        }

        private void Start()
        {
            m_gameState.isGameOver.OnObservedValueChanged += IsGameOverValueChangedHandler;
            
            gameOverPanel.SetActive( m_gameState.isGameOver.Value );
        }
        
        private void IsGameOverValueChangedHandler(bool oldValue, bool newValue)
        {
            gameOverPanel.SetActive(newValue);
        }
    }
}
