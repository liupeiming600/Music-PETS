using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETSGames
{
    public class BorderColliderController : MonoBehaviour
    {
        private void OnTriggerEnter2D( Collider2D other )
        {
            ObstacleController obstacleController = other.gameObject.GetComponent<ObstacleController>();
            if( obstacleController != null )
            {
                Destroy( other.gameObject );
            }
        }
    }
}
