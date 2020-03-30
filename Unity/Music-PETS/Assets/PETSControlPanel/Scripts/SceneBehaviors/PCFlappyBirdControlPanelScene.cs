using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using HololensPETS;

namespace HololensPETSGames
{
    public class PCFlappyBirdControlPanelScene : MonoBehaviour
    {
        public InputField minGapSizeInputField;
        public InputField maxGapSizeInputField;

        public InputField minDistanceBetweenPipesField;
        public InputField maxDistanceBetweenPipesField;

        public Button startButton;
        public Button stopButton;

        public List<Toggle> activeFingerToggles;

        private FlappyBirdGameController m_flappyBirdGameController;

        private FlappyBirdGameState m_gameState;

        private UDPHelper m_udpHelper;

        private AppState m_appState;

        public void StartRun()
        {
            m_flappyBirdGameController.gapSizeLowerBound = float.Parse( minGapSizeInputField.text );
            m_flappyBirdGameController.gapSizeUpperBound = float.Parse( maxGapSizeInputField.text );

            m_flappyBirdGameController.minDistanceBetweenPipes = float.Parse( minDistanceBetweenPipesField.text );
            m_flappyBirdGameController.maxDistanceBetweenPipes = float.Parse( maxDistanceBetweenPipesField.text );

            ApplyChangeActiveFingers();
            m_flappyBirdGameController.StartGame();

            startButton.interactable = false;
            stopButton.interactable = true;
        }

        public void StopRun()
        {
            m_flappyBirdGameController.StopGame();

            startButton.interactable = true;
            stopButton.interactable = false;
        }

        public void ApplyChangeActiveFingers()
        {
            int numActiveFingers = 0;
            List<int> activeFingers = new List<int>();
            for( int i = 0; i < activeFingerToggles.Count; i++ )
            {
                Finger finger = (Finger)i;
                m_gameState.ActiveFingers[finger] = activeFingerToggles[i].isOn;

                if( activeFingerToggles[i].isOn )
                {
                    numActiveFingers++;

                    activeFingers.Add( i );
                }
            }

            NetOutMessage message = new NetOutMessage();
            message.WriteInt32( (int)MessageType.Command.Control );
            message.WriteInt32( (int)MessageType.ControlType.ChangeActiveFingers );
            message.WriteInt32( numActiveFingers );

            for( int i = 0; i < activeFingers.Count; i++ )
            {
                message.WriteInt32( activeFingers[i] );
            }

            m_udpHelper.Send( message, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        public void ReturnToMainMenu()
        {

        }

        private void Awake()
        {
            m_flappyBirdGameController = GameObject.FindObjectOfType<FlappyBirdGameController>();

            m_gameState = GameObject.FindObjectOfType<FlappyBirdGameState>();

            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

            m_appState = GameObject.FindObjectOfType<AppState>();
        }

        private void Start()
        {

        }

        private void Update()
        {

        }

        private void OnEnable()
        {
            m_gameState.isGameOver.OnObservedValueChanged += GameOverValueChangedHandler;

            startButton.interactable = true;
            stopButton.interactable = false;
        }

        private void OnDisable()
        {
            m_gameState.isGameOver.OnObservedValueChanged -= GameOverValueChangedHandler;
        }

        private void GameOverValueChangedHandler( bool oldValue, bool newValue )
        {
            startButton.interactable = true;
            stopButton.interactable = false;
        }
    }
}
