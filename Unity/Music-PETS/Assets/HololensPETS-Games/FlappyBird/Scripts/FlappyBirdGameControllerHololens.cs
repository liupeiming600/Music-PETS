using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using HololensPETS;

namespace HololensPETSGames
{
    public class FlappyBirdGameControllerHololens : MonoBehaviour
    {
        public GameObject worldObj;

        public List<Indicator> fingerIndicators;

        private FlappyBirdGameState m_gameState;

        private PlayerController m_playerController;

        private ObstacleSpawnerController m_obstacleSpawner;

        private UDPHelper m_udpHelper;

        private HololensAppState m_appState;

        private void Awake()
        {
            m_gameState = GameObject.FindObjectOfType<FlappyBirdGameState>();

            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

            m_playerController = GameObject.FindObjectOfType<PlayerController>();

            m_obstacleSpawner = GameObject.FindObjectOfType<ObstacleSpawnerController>();

            m_appState = GameObject.FindObjectOfType<HololensAppState>();
        }

        private void OnEnable()
        {
            m_udpHelper.MessageReceived += UDPMessageReceivedHandler;

            m_gameState.isGameOver.OnObservedValueChanged += IsGameOverValueChangedHandler;

            worldObj.transform.position = m_appState.FingerTransforms[2].position + Vector3.up * 0.1f;

            for( int i = 0; i < 5; i++ )
            {
                fingerIndicators[i].transform.SetParent( m_appState.FingerTransforms[i] );
                fingerIndicators[i].transform.localPosition = Vector3.zero;
                fingerIndicators[i].IsVisible = false;
            }
        }

        private void OnDisable()
        {
            m_gameState.isGameOver.Value = false;

            m_udpHelper.MessageReceived -= UDPMessageReceivedHandler;

            m_gameState.isGameOver.OnObservedValueChanged -= IsGameOverValueChangedHandler;

            for( int i = 0; i < 5; i++ )
            {
                fingerIndicators[i].IsVisible = false;
            }

            m_obstacleSpawner.ResetState();
        }

        private void IsGameOverValueChangedHandler( bool oldValue, bool newValue )
        {
            Time.timeScale = newValue ? 0.0f : 1.0f;
        }

        private void UDPMessageReceivedHandler( NetInMessage message )
        {
            MessageType.Command command = (MessageType.Command)message.ReadInt32();
            if( command == MessageType.Command.Control )
            {
                MessageType.ControlType controlType = (MessageType.ControlType)message.ReadInt32();
                if( controlType == MessageType.ControlType.SpawnObstacle )
                {
                    float yPos = message.ReadFloat();
                    float gapSize = message.ReadFloat();

                    GameObject obstacleObj = m_obstacleSpawner.SpawnObstacle( yPos, gapSize );

                    Vector3 upperPipeScale = message.ReadVector3();
                    Vector3 lowerPipeScale = message.ReadVector3();

                    ObstacleController obstacle = obstacleObj.GetComponent<ObstacleController>();
                    obstacle.upperPipe.transform.localScale = upperPipeScale;
                    obstacle.lowerPipe.transform.localScale = lowerPipeScale;
                }
                else if( controlType == MessageType.ControlType.PlayerPosition )
                {
                    Vector3 playerPosition = message.ReadVector3();

                    m_playerController.transform.localPosition = playerPosition;
                }
                else if( controlType == MessageType.ControlType.Start )
                {
                    m_gameState.isGameOver.Value = false;
                    m_gameState.score.Value = 0;

                    m_obstacleSpawner.ResetState();
                }
                else if( controlType == MessageType.ControlType.TriggerGameOver )
                {
                    m_gameState.isGameOver.Value = true;
                }
                else if( controlType == MessageType.ControlType.UpdateScore )
                {
                    int score = message.ReadInt32();
                    m_gameState.score.Value = score;
                }
                else if( controlType == MessageType.ControlType.ChangeActiveFingers )
                {
                    int numActiveFingers = message.ReadInt32();
                    List<int> activeFingers = new List<int>();
                    for( int i = 0; i < numActiveFingers; i++ )
                    {
                        int activeFingerIndex = message.ReadInt32();
                        activeFingers.Add( activeFingerIndex );
                    }

                    for( int i = 0; i < fingerIndicators.Count; i++ )
                    {
                        bool isFingerActive = activeFingers.Contains( i );
                        fingerIndicators[i].IsVisible = isFingerActive;
                        if( isFingerActive )
                        {
                            fingerIndicators[i].SetIsOn( true );
                        }
                    }
                }
            }
        }
    }
}
