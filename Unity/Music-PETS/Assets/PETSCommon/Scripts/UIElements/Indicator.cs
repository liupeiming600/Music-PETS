using UnityEngine;
using UnityEngine.UI;

namespace HololensPETS
{
    public class Indicator : MonoBehaviour
    {
        public Color onColor = Color.green;
        public Color offColor = Color.red;

        private Image m_image;

        private bool m_isOn;
        private bool m_isVisible;

        public bool IsVisible
        {
            get
            {
                return m_isVisible;
            }

            set
            {
                m_isVisible = value;

                UpdateColor();
            }
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void Awake()
        {
            m_image = GetComponent<Image>();
        }

        private void Start()
        {
            SetIsOn(false);
        }

        public void SetIsOn( bool isOn )
        {
            m_isOn = isOn;

            UpdateColor();
        }

        public void Toggle()
        {
            SetIsOn( !m_isOn );

            UpdateColor();
        }

        private void UpdateColor()
        {
            if( m_isVisible )
            {
                m_image.color = m_isOn ? onColor : offColor;
            }
            else
            {
                m_image.color = Color.clear;
            }
        }
    }
}
