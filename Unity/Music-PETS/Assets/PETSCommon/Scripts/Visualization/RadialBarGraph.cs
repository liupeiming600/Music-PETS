using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HololensPETS
{
    public class RadialBarGraph : MonoBehaviour
    {
        public Image fillImage;
        public Color fillColor = Color.blue;

        public Image rangeFillImage;
        public Color rangeFillColor = new Color( 1.0f, 0.0f, 0.0f, 0.5f );

        public Text valueText;
        public string valueTextFormat = "{0}";

        public Text rangeText;
        public string rangeTextFormat = "{0} ~ {1}";

        public float minValue = 0.0f;
        public float maxValue = 1.0f;
        public float value = 0.0f;

        public bool showTargetRange = false;
        public float minTargetValue = 0.0f;
        public float maxTargetValue = 1.0f;

        private void Awake()
        {
        }

        private void Start()
        {
        }

        private void Update()
        {
            value = Mathf.Clamp( value, minValue, maxValue );
            float fillAmount = ( value - minValue ) / maxValue;
            if( float.IsNaN( fillAmount ) )
            {
                fillAmount = 0.0f;
            }
            fillImage.fillAmount = fillAmount;
            fillImage.color = fillColor;
            
            if( showTargetRange )
            {
                float rangePercentage = ( maxTargetValue - minTargetValue ) / maxValue;
                rangeFillImage.fillAmount = rangePercentage;

                // Need to set rotation
				Vector3 euler = rangeFillImage.rectTransform.eulerAngles;
				euler.z = 360.0f * ( ( minTargetValue - minValue ) / maxValue );
                rangeFillImage.rectTransform.rotation = Quaternion.Euler( euler );
                rangeFillImage.color = rangeFillColor;

                if( rangeText != null )
                {
                    rangeText.text = string.Format( rangeTextFormat, minTargetValue, maxTargetValue );
                }
            }
            else
            {
                rangeFillImage.color = Color.clear;
            }

            // Update text of the radial bar graph
            if( valueText != null )
            {
                valueText.text = string.Format( valueTextFormat, value );
            }
        }
    }
}
