using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETSGames
{
    public class NoteLaneController : MonoBehaviour
    {
        public Transform noteSpawnLocation;
        public Transform noteActivatorLocation;
        
        public Vector3 GetLanePositionAtTime( float time, float referenceTime )
        {
            float offset = time - referenceTime;

            float totalDistance = ( noteActivatorLocation.localPosition - noteSpawnLocation.localPosition ).magnitude;
            float noteSpeed = 1.0f * 1.0f;
            float noteTravelTime = totalDistance / noteSpeed;

            float percentage = offset / noteTravelTime;
            Vector3 ret = noteActivatorLocation.localPosition + ( noteSpawnLocation.localPosition - noteActivatorLocation.localPosition ) * percentage;
            
            return ret;
        }
    }
}
