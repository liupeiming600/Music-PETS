using UnityEngine;
using UnityEngine.UI;

namespace HololensPETSGames
{
    public class ScoreText : MonoBehaviour
    {
        private FlappyBirdGameState m_gameState;

        private Text m_text;

        private void Awake()
        {
            m_text = GetComponent<Text>();

            m_gameState = GameObject.FindObjectOfType<FlappyBirdGameState>();
        }

        private void OnEnable()
        {
            if(m_gameState.score == null)
            {
                Debug.Log("Score null");
            }
            m_gameState.score.OnObservedValueChanged += ScoreChangedHandler;
        }

        private void OnDisable()
        {
            m_gameState.score.OnObservedValueChanged -= ScoreChangedHandler;
        }

        private void ScoreChangedHandler(int oldScore, int newScore)
        {
            m_text.text = "Score: " + newScore;
        }
    }
}
