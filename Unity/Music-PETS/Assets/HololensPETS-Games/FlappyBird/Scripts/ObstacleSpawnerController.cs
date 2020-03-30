using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETSGames
{
    public class ObstacleSpawnerController : MonoBehaviour
    {
        public GameObject obstaclePrefab;

        public Scroller scroller;

        public Transform spawnPosition;
        
        private GameObject m_obstacleContainer;

        public void ResetState()
        {
            DestroyAllObstacles();
        }

        public GameObject SpawnObstacle( float y, float gapSize )
        {
            GameObject obstacle = Instantiate( obstaclePrefab );
            Vector3 spawnPos = spawnPosition.transform.localPosition;
            spawnPos.y = y;

            ObstacleController obstacleController = obstacle.GetComponent<ObstacleController>();
            obstacleController.gapSize = gapSize;

            obstacle.transform.SetParent( m_obstacleContainer.transform, false );
            obstacle.transform.localPosition = spawnPos;
            obstacle.transform.localScale = Vector3.one;

            scroller.Add( obstacle );

            return obstacle;
        }

        private void Awake()
        {
            m_obstacleContainer = new GameObject("Obstacles");
            m_obstacleContainer.transform.SetParent(this.transform, false);
        }

        private void DestroyAllObstacles()
        {
            int numObstacles = m_obstacleContainer.transform.childCount;
            for( int i = 0; i < numObstacles; i++ )
            {
                Destroy( m_obstacleContainer.transform.GetChild(i).gameObject );
            }
        }
    }
}
