using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETSGames
{
    public class ForceScalingNoteActivatorController : NoteActivatorController
    {
        public GameObject barObject;
        public GameObject rangeBarObject;

        private SpriteRenderer m_barSpriteRenderer;

        protected override void Awake()
        {
            base.Awake();

            m_barSpriteRenderer = barObject.GetComponentInChildren<SpriteRenderer>();
        }

        protected override void UpdateGraphics()
        {
            bool isPressed = CustomInput.GetKey( key );

            m_barSpriteRenderer.color = isPressed ? pressedColor : defaultColor;
        }

        public void SetForcePercentage( float forcePercentage )
        {
            Vector3 barScale = barObject.transform.localScale;
            barScale.x = Mathf.Clamp01( forcePercentage / 100.0f );
            barObject.transform.localScale = barScale;
        }

        public void SetForceRangePercentage( float minForcePercentage, float maxForcePercentage )
        {
            Vector3 rangeBarScale = rangeBarObject.transform.localScale;
            rangeBarScale.x = ( maxForcePercentage - minForcePercentage ) / 100.0f;
            rangeBarObject.transform.localScale = rangeBarScale;

            Vector3 rangeBarPosition = rangeBarObject.transform.localPosition;
            rangeBarPosition.x = -0.5f + ( minForcePercentage + ( maxForcePercentage - minForcePercentage ) / 2 ) / 100.0f;
            rangeBarObject.transform.localPosition = rangeBarPosition;
        }
    }
}
