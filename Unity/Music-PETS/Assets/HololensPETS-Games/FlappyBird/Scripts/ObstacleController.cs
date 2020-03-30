using UnityEngine;

namespace HololensPETSGames
{
    public class ObstacleController : MonoBehaviour
    {
        public float gapSize = 1.0f;

        public GameObject upperPipe;
        public GameObject lowerPipe;

        private void Awake()
        {
        }

        private void Update()
        {
            Vector3 upperPipePos = upperPipe.transform.localPosition;
            Vector3 lowerPipePos = lowerPipe.transform.localPosition;

            upperPipePos.y = gapSize / 2.0f;
            lowerPipePos.y = gapSize / -2.0f;

            upperPipe.transform.localPosition = upperPipePos;
            lowerPipe.transform.localPosition = lowerPipePos;
        }
    }
}
