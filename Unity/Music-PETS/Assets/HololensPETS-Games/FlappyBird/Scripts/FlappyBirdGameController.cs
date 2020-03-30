using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using HololensPETS;

namespace HololensPETSGames
{
    public class FlappyBirdGameController : MonoBehaviour
    {
        private FlappyBirdGameState m_gameState;

        private ObstacleSpawnerController m_obstacleSpawner;

        private FingerForceDataProvider m_fingerForceDataProvider;

        private AppState m_appState;

        private UDPHelper m_udpHelper;

        private PlayerController m_playerController;

        private float m_startTime = 0.0f;

        public Transform topBorderTransform;
        public Transform bottomBorderTransform;

        public float gapSizeLowerBound = 3;
        public float gapSizeUpperBound = 6;

        public float minDistanceBetweenPipes = 3.0f;
        public float maxDistanceBetweenPipes = 5.0f;

        private bool m_isSpawning = true;

        private float m_timeToNextSpawn = 0.0f;

        public void StartGame()
        {
            ResetState();

            m_gameState.isPlaying = true;

            m_isSpawning = true;

            NetOutMessage outMessage = new NetOutMessage();
            outMessage.WriteInt32( (int)MessageType.Command.Control );
            outMessage.WriteInt32( (int)MessageType.ControlType.Start );

            m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        public void StopGame()
        {
            m_isSpawning = false;

            m_gameState.isPlaying = false;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void Awake()
        {
            m_gameState = GameObject.FindObjectOfType<FlappyBirdGameState>();

            m_obstacleSpawner = GameObject.FindObjectOfType<ObstacleSpawnerController>();

            m_fingerForceDataProvider = GameObject.FindObjectOfType<FingerForceDataProvider>();

            m_appState = GameObject.FindObjectOfType<AppState>();

            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

            m_playerController = GameObject.FindObjectOfType<PlayerController>();
        }

        private void Start()
        {
            m_isSpawning = false;
        }

        private void Update()
        {
            if( !m_isSpawning )
            {
                return;
            }

            m_startTime += Time.deltaTime;
            m_gameState.score.Value = (int)m_startTime;

            m_timeToNextSpawn -= Time.deltaTime;
            if( m_isSpawning && ( m_timeToNextSpawn <= 0.0f ) )
            {
                float distanceToNextPipe = Random.Range( minDistanceBetweenPipes, maxDistanceBetweenPipes );
                m_timeToNextSpawn = distanceToNextPipe / ( m_obstacleSpawner.scroller.scrollSpeed * m_obstacleSpawner.scroller.scrollSpeedMultiplier );
                
                float gapSize = Random.Range( gapSizeLowerBound, gapSizeUpperBound );
                float obstacleY = Random.Range( bottomBorderTransform.localPosition.y + gapSize / 2.0f, topBorderTransform.localPosition.y - gapSize / 2.0f );
                GameObject obstacleObj = m_obstacleSpawner.SpawnObstacle( obstacleY, gapSize );

                ObstacleController obstacle = obstacleObj.GetComponent<ObstacleController>();
                Vector3 upperPipeScale = obstacle.upperPipe.transform.localScale;
                Vector3 lowerPipeScale = obstacle.lowerPipe.transform.localScale;
                upperPipeScale.y = topBorderTransform.localPosition.y - obstacleObj.transform.localPosition.y - obstacle.gapSize / 2.0f;
                lowerPipeScale.y = obstacleObj.transform.localPosition.y - bottomBorderTransform.localPosition.y - obstacle.gapSize / 2.0f;
                obstacle.upperPipe.transform.localScale = upperPipeScale;
                obstacle.lowerPipe.transform.localScale = lowerPipeScale;

                NetOutMessage outMessage = new NetOutMessage();
                outMessage.WriteInt32( (int)MessageType.Command.Control );
                outMessage.WriteInt32( (int)MessageType.ControlType.SpawnObstacle );
                outMessage.WriteFloat( obstacleY );
                outMessage.WriteFloat( gapSize );
                outMessage.WriteVector3( upperPipeScale );
                outMessage.WriteVector3( lowerPipeScale );

                m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
            }
        }

        private void IsGameOverValueChangedHandler(bool oldValue, bool newValue)
        {
            Time.timeScale = newValue ? 0.0f : 1.0f;

            if( newValue )
            {
                m_isSpawning = false;
                m_timeToNextSpawn = 0.0f;

                m_gameState.isPlaying = false;

                NetOutMessage outMessage = new NetOutMessage();
                outMessage.WriteInt32( (int)MessageType.Command.Control );
                outMessage.WriteInt32( (int)MessageType.ControlType.TriggerGameOver );

                m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
            }
        }

        private void OnEnable()
        {
            m_gameState.isGameOver.OnObservedValueChanged += IsGameOverValueChangedHandler;

            m_fingerForceDataProvider.OnFingerDataReceived += FingerForceDataReceivedHandler;

            m_gameState.score.OnObservedValueChanged += ScoreValueChangedHandler;
        }

        private void OnDisable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived -= FingerForceDataReceivedHandler;

            m_gameState.isGameOver.Value = false;

            m_gameState.isGameOver.OnObservedValueChanged -= IsGameOverValueChangedHandler;
        }

        private void ScoreValueChangedHandler( int oldScore, int newScore )
        {
            NetOutMessage outMessage = new NetOutMessage();
            outMessage.WriteInt32( (int)MessageType.Command.Control );
            outMessage.WriteInt32( (int)MessageType.ControlType.UpdateScore );
            outMessage.WriteInt32( newScore );

            m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        private void ResetState()
        {
            m_gameState.isGameOver.Value = false;
            m_gameState.score.Value = 0;

            m_timeToNextSpawn = 0.0f;

            m_startTime = 0.0f;

            m_obstacleSpawner.ResetState();
        }

        private void FingerForceDataReceivedHandler( Dictionary<Finger, double> forceData )
        {
            if( m_gameState.isGameOver.Value )
            {
                return;
            }

            double sumMax = 0;
            double sum = 0;

            foreach( Finger finger in forceData.Keys )
            {
                if( m_gameState.ActiveFingers[finger] )
                {
                    sumMax += m_appState.GetCurrentHandData().GetFingerMaxForce( finger );
                    sum += forceData[finger] - m_appState.GetCurrentHandData().GetFingerBaseForce( finger );
                }
            }

            if( sum < 0.0 )
            {
                sum = 0.0;
            }

            if( sumMax > 0.0 )
            {
                float percentage = (float)( sum / sumMax );
                
                float topY = topBorderTransform.localPosition.y;
                float bottomY = bottomBorderTransform.localPosition.y;

                float playerY = bottomY + ( topY - bottomY ) * percentage;
                float playerCurrentY = m_playerController.transform.localPosition.y;

                Vector3 translateVector = new Vector3( 0.0f, playerY - playerCurrentY, 0.0f );
                m_playerController.transform.localPosition += translateVector;

                NetOutMessage outMessage = new NetOutMessage();
                outMessage.WriteInt32( (int)MessageType.Command.Control );
                outMessage.WriteInt32( (int)MessageType.ControlType.PlayerPosition );
                outMessage.WriteVector3( m_playerController.transform.localPosition );

                m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
            }
        }
    }
}
