using System.Collections.Generic;

namespace HololensPETS
{
    [System.Serializable]
    public class HandData
    {
        private Dictionary<Finger, double> m_maxForces;
        private Dictionary<Finger, double> m_baseForces;

        public HandData()
        {
            m_maxForces = new Dictionary<Finger, double>
            {
                { Finger.Thumb, double.NaN },
                { Finger.Index, double.NaN },
                { Finger.Middle, double.NaN },
                { Finger.Ring, double.NaN },
                { Finger.Pinky, double.NaN }
            };
            m_baseForces = new Dictionary<Finger, double>
            {
                { Finger.Thumb, 0.0 },
                { Finger.Index, 0.0 },
                { Finger.Middle, 0.0 },
                { Finger.Ring, 0.0 },
                { Finger.Pinky, 0.0 }
            };
        }

        public void SetFingerMaxForce( Finger finger, double maxForce )
        {
            if( m_maxForces.ContainsKey( finger ) )
            {
                m_maxForces[finger] = maxForce;
            }
        }

        public double GetFingerMaxForce( Finger finger )
        {
            if( m_maxForces.ContainsKey( finger ) )
            {
                return m_maxForces[finger];
            }

            return double.NaN;
        }

        public void SetFingerBaseForce( Finger finger, double baseForce )
        {
            if ( m_baseForces.ContainsKey( finger ) )
            {
                m_baseForces[finger] = baseForce;
            }
        }

        public double GetFingerBaseForce( Finger finger )
        {
            if ( m_baseForces.ContainsKey( finger ) )
            {
                return m_baseForces[finger];
            }

            return 0.0;
        }
    }
}
