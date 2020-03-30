using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETS
{
    public class UITestScene : MonoBehaviour
    {
        public LineGraph lineGraph;

        private float t = 0.0f;

        private void Start()
        {
            lineGraph.AddSeries("Reference");
            lineGraph.SetSeriesColor("Reference", Color.green);
            lineGraph.Plot(0.0f, 0.0f, "Reference");
            lineGraph.Plot(5.0f, 0.0f, "Reference");
            lineGraph.Plot(10.0f, 10.0f, "Reference");
            lineGraph.Plot(15.0f, 10.0f, "Reference");
            lineGraph.Plot(20.0f, 0.0f, "Reference");
            lineGraph.Plot(25.0f, 0.0f, "Reference");

            lineGraph.AddSeries("SinSeries");
            lineGraph.SetSeriesColor("SinSeries", Color.red);
        }

        private void Update()
        {
            t += Time.deltaTime;
            lineGraph.Plot(t, 5 + Mathf.Sin(t) * 5.0f, "SinSeries");
        }
    }
}
