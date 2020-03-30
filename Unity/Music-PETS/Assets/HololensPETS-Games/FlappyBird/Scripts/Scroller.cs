using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETSGames
{
    public class Scroller : MonoBehaviour
    {
        public float scrollSpeed = 1.0f;
        public float scrollSpeedMultiplier = 1.0f;
        public Vector3 scrollDirection = -Vector3.right;

        private List<GameObject> m_scrollingObjects;

        private void Awake()
        {
            m_scrollingObjects = new List<GameObject>();
        }

        private void Update()
        {
            Vector3 normalizedScrollDirection = scrollDirection.normalized;
            for( int i = m_scrollingObjects.Count - 1; i >= 0; i-- )
            {
                if( m_scrollingObjects[i] != null )
                {
                    m_scrollingObjects[i].transform.localPosition += normalizedScrollDirection * scrollSpeed * scrollSpeedMultiplier * Time.deltaTime;
                }
                else
                {
                    m_scrollingObjects.RemoveAt( i );
                }
            }
        }

        public void Add( GameObject obj )
        {
            m_scrollingObjects.Add( obj );
        }

        public void Remove( GameObject obj )
        {
            m_scrollingObjects.Remove( obj );
        }
    }
}
