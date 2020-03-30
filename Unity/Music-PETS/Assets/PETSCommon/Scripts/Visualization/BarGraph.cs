using UnityEngine;
using UnityEngine.UI;

namespace HololensPETS
{
    public class BarGraph : MonoBehaviour
    {
        public Image bar;
        public Image targetLine;

        public RectTransform elementsRect;

        public double min = 0.0;
        public double max = 5.0;
        public double value = 0.0;
        
        public double targetValue = 0.0;
        public double targetValueDeviation = 0.0f;
        public bool showTargetLine = true;
        
        private void Awake()
        {
        }

        private void Start()
        {
            targetLine.gameObject.SetActive(showTargetLine);
        }

        private void Update()
        {
            if( ( targetLine != null ) && ( targetLine.gameObject.activeSelf ) )
            {
                targetValue = MathUtils.Clamp(targetValue, min, max);
                
                RectTransform rectTransform = targetLine.gameObject.GetComponent<RectTransform>();
                Vector3 targetLinePos = rectTransform.localPosition;
                targetLinePos.y = ValueToYPosition( targetValue );
                rectTransform.localPosition = targetLinePos;
                
                if( MathUtils.Abs( targetValueDeviation ) > 0.0f )
                {
                    float upperY = ValueToYPosition( targetValue + targetValueDeviation / 2 );
                    
                    float deviationHeight = ( upperY - ValueToYPosition( targetValue ) ) * 2.0f;

                    Vector2 sizeDelta = rectTransform.sizeDelta;
                    sizeDelta.y = deviationHeight;
                    rectTransform.sizeDelta = sizeDelta;
                }
            }
            
            if( bar != null )
            {
                value = MathUtils.Clamp(value, min, max);
                double ratio = value / (max - min);

                Vector3 barScale = bar.rectTransform.localScale;
                barScale.y = (float)ratio;
                bar.rectTransform.localScale = barScale;
            }
        }

        private float ValueToYPosition( double val )
        {
            double targetRatio = val / ( max - min );
            return 1 + (float)( ( elementsRect.rect.height - 2 ) * targetRatio );
        }

        public void PlotY( double val )
        {
            if( val < min )
            {
                val = min;
            }
            else if( val > max )
            {
                val = max;
            }

            value = val;
        }
    }
}
