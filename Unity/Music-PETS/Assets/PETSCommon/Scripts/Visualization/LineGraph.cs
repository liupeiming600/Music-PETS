using System.Collections.Generic;

using UnityEngine;

using Vectrosity;

namespace HololensPETS
{
    /**
     * The LineGraph class is a class used to render a line graph
     * with the ability to be updated in real time. This class
     * also supports scrolling of the line graph as more data is being
     * plotted.
     */
    public class LineGraph : MonoBehaviour
    {
        // Minimum y value that will be displayed by the graph.
        public float minY = -5.0f;

        // Maximum y value that will be displayed by the graph.
        public float maxY = 5.0f;

        public float minX = 0.0f;

        public float maxX = 20.0f;

        public bool is3D = false;

        // RectTransform of the panel where the line graph will be rendered.
        public RectTransform graphPanelRect;

        public Canvas graphLineCanvas;

        private Dictionary<string, List<Vector2>> m_plots;

        private Dictionary<string, List<Vector2>> m_points;
        private Dictionary<string, List<Vector3>> m_points3D;
        private Dictionary<string, VectorLine> m_plotLines;
        private string m_defaultSeriesName = "Default";

        private void Awake()
        {
            m_plots = new Dictionary<string, List<Vector2>>();

            if (is3D)
            {
                m_points3D = new Dictionary<string, List<Vector3>>();
            }
            else
            {
                m_points = new Dictionary<string, List<Vector2>>();
            }
            m_plotLines = new Dictionary<string, VectorLine>();
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        private void LateUpdate()
        {
            foreach (string series in m_plotLines.Keys)
            {
                if (is3D)
                {
                    m_plotLines[series].Draw3D();
                }
                else
                {
                    m_plotLines[series].Draw();
                }
            }
        }

        private void OnEnable()
        {
            foreach(VectorLine vectorLine in m_plotLines.Values)
            {
                vectorLine.active = true;
            }
        }

        private void OnDisable()
        {
            foreach (VectorLine vectorLine in m_plotLines.Values)
            {
                vectorLine.active = false;
            }
        }

        public void Refresh()
        {
            if (is3D)
            {
                foreach (string s in m_points3D.Keys)
                {
                    Refresh(s);
                }
            }
            else
            {
                foreach (string s in m_points.Keys)
                {
                    Refresh(s);
                }
            }
        }

        public void Refresh(string series)
        {
            List<Vector2> values = m_plots[series];
            values.Sort((a, b) => a.x.CompareTo(b.x));

            if(is3D)
            {
                m_points3D[series].Clear();
            }
            else
            {
                m_points[series].Clear();
            }

            for (int i = 0; i < values.Count; i++)
            {
                Vector2 value = values[i];

                AddPoint(value.x, value.y, series);
            }
        }

        public void AddSeries(string series)
        {
            AddSeries(series, Color.white);
        }

        public void AddSeries(string series, Color color)
        {
            if( m_plots.ContainsKey( series ) )
            {
                return;
            }

            VectorLine vectorLine = null;
            if ( is3D )
            {
                List<Vector3> temp = new List<Vector3>();
                vectorLine = new VectorLine(series, temp, 2.0f);
                m_points3D.Add(series, temp);
            }
            else
            {
                List<Vector2> temp = new List<Vector2>();
                vectorLine = new VectorLine(series, temp, 2.0f);
                m_points.Add(series, temp);
            }
            
            vectorLine.color = color;
            vectorLine.lineType = LineType.Continuous;
            vectorLine.drawTransform = graphPanelRect;
            vectorLine.lineWidth = 3.0f;
            
            m_plots.Add(series, new List<Vector2>());
            m_plotLines.Add(series, vectorLine);
        }

        public void Plot( double x, double y )
        {
            Plot(x, y, m_defaultSeriesName);
        }

        public void Plot( double x, double y, string series )
        {
            if( !m_plots.ContainsKey( series ) )
            {
                AddSeries(series);
            }

            AddValue(x, y, series);
            AddPoint(x, y, series);
        }

        private void AddValue(double x, double y, string series)
        {
            if(!m_plots.ContainsKey(series))
            {
                return;
            }

            List<Vector2> plots = m_plots[series];
            plots.Add(new Vector2((float)x, (float)y));
            plots.Sort((a, b) => a.x.CompareTo(b.x));
        }

        private void AddPoint(double x, double y, string series)
        {
            float xRange = maxX - minX;
            float yRange = maxY - minY;

            float relX = (float)((x - minX) / xRange);
            float relY = (float)((y - minY) / yRange);

            float px = relX * graphPanelRect.rect.width;
            float py = relY * graphPanelRect.rect.height;

            if (is3D)
            {
                m_points3D[series].Add(new Vector3(px, py, 0.0f));
                m_points3D[series].Sort((a, b) => a.x.CompareTo(b.x));

                m_plotLines[series].Draw3D();
            }
            else
            {
                m_points[series].Add(new Vector2(px, py));
                m_points[series].Sort((a, b) => a.x.CompareTo(b.x));

                m_plotLines[series].Draw();
            }
        }

        public void SetSeriesColor( string series, Color color )
        {
            if( m_plotLines.ContainsKey( series ) )
            {
                m_plotLines[series].color = color;
            }
        }
        
        public void ClearPlot( string series )
        {
            if( m_plots.ContainsKey( series ) )
            {
                m_plots[series].Clear();

                if (is3D)
                {
                    m_points3D[series].Clear();
                }
                else
                {
                    m_points[series].Clear();
                }
            }
        }
    }
}
