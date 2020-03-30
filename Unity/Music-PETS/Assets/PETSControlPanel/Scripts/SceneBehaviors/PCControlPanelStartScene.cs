using UnityEngine;
using UnityEngine.SceneManagement;

namespace HololensPETS
{
    public class PCControlPanelStartScene : MonoBehaviour
    {
        private bool m_firstFrame = true;

        private void Awake()
        {
            
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if( m_firstFrame )
            {
                m_firstFrame = false;

                SceneManager.LoadScene( "PCControlPanelScene", LoadSceneMode.Additive );
            }
        }
    }
}
