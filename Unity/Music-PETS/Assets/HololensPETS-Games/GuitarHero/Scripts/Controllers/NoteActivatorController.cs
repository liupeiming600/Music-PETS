using UnityEngine;

namespace HololensPETSGames
{
    public class NoteActivatorController : MonoBehaviour
    {
        public KeyCode key;

        public Color defaultColor = Color.white;
        public Color pressedColor = Color.red;

        public ParticleSystem activatorParticleSystem;

        protected NoteController m_collidingNote = null;

        private SpriteRenderer m_spriteRenderer;

        protected GuitarHeroGameController m_gameController;

        protected virtual void Awake()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();

            m_gameController = GameObject.FindObjectOfType<GuitarHeroGameController>();
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            bool isPressed = CustomInput.GetKey( key );
            UpdateGraphics();

            if( m_collidingNote != null )
            {
                if( m_collidingNote.action == NoteAction.Hold )
                {
                    m_collidingNote.duration -= Time.deltaTime;
                    if( m_collidingNote.duration <= 0.0f )
                    {
                        Destroy( m_collidingNote.gameObject );
                        m_collidingNote = null;

                        if( activatorParticleSystem != null )
                        {
                            activatorParticleSystem.Stop();
                        }
                    }
                    else
                    {
                        Vector3 scale = m_collidingNote.gameObject.transform.localScale;
                        scale.y -= Time.deltaTime;

                        m_collidingNote.gameObject.transform.localScale = scale;

                        Vector3 position = m_collidingNote.gameObject.transform.localPosition;
                        position.y += Time.deltaTime;

                        m_collidingNote.gameObject.transform.localPosition = position;

                        if( isPressed )
                        {
                            if( m_gameController != null )
                            {
                                // Add score, make effect, whatever
                                m_gameController.score.Value += m_collidingNote.baseScore;
                            }

                            if( activatorParticleSystem != null )
                            {
                                ParticleSystem.MainModule main = activatorParticleSystem.main;
                                main.startColor = Color.cyan;
                            }
                        }
                        else
                        {
                            ParticleSystem.MainModule main = activatorParticleSystem.main;
                            main.startColor = Color.red;
                        }
                    }
                }
                else if( m_collidingNote.action == NoteAction.Tap )
                {
                    if( CustomInput.GetKey( key ) )
                    {
                        // Add score, make effect, whatever
                        if( m_gameController != null )
                        {
                            m_gameController.score.Value += m_collidingNote.baseScore;
                        }
                    }

                    Destroy( m_collidingNote.gameObject );
                    m_collidingNote = null;
                }
            }
        }

        protected virtual void OnTriggerEnter2D( Collider2D other )
        {
            if( activatorParticleSystem != null )
            {
                activatorParticleSystem.Play();
            }

            m_collidingNote = other.gameObject.GetComponent<NoteController>();
        }

        protected virtual void OnTriggerExit2D( Collider2D other )
        {
            if( activatorParticleSystem != null )
            {
                activatorParticleSystem.Stop();
            }

            m_collidingNote = null;
        }

        protected virtual void UpdateGraphics()
        {
            bool isPressed = CustomInput.GetKey( key );
            m_spriteRenderer.color = isPressed ? pressedColor : defaultColor;
        }
    }
}