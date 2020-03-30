using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETS
{
    public class PETSCalibrationTask
    {
        private Dictionary<Finger, double> m_calibrationResult;
        private Dictionary<Finger, int> m_currentNumSamplesPerFinger;
        private bool m_isRunning;

        private Finger m_currentFinger = Finger.Thumb;
        private int m_numSamplesPerFinger = 0;
        private int m_currentNumSamples = 0;

        public PETSCalibrationTask()
        {
            m_calibrationResult = new Dictionary<Finger, double>()
            {
                { Finger.Thumb, double.NaN },
                { Finger.Index, double.NaN },
                { Finger.Middle, double.NaN },
                { Finger.Ring, double.NaN },
                { Finger.Pinky, double.NaN }
            };

            m_currentNumSamplesPerFinger = new Dictionary<Finger, int>()
            {
                { Finger.Thumb, 0 },
                { Finger.Index, 0 },
                { Finger.Middle, 0 },
                { Finger.Ring, 0 },
                { Finger.Pinky, 0 }
            };
            
            m_isRunning = false;
        }

        public void Start( int numSamplesPerFinger )
        {
            m_isRunning = true;
            
            foreach( Finger finger in Constants.FINGER_LIST )
            {
                m_calibrationResult[finger] = 0.0;
                m_currentNumSamplesPerFinger[finger] = 0;
            }

            m_numSamplesPerFinger = numSamplesPerFinger;
            m_currentFinger = Finger.Thumb;
            m_currentNumSamples = 1;
        }

        public void FeedSample( Finger finger, double sampleValue )
        {
            if( !m_isRunning )
            {
                return;
            }
            
            m_calibrationResult[finger] += sampleValue;
            m_currentNumSamplesPerFinger[finger]++;

            if( IsDone() )
            {
                foreach( Finger finger2 in Constants.FINGER_LIST )
                {
                    m_calibrationResult[finger2] /= m_currentNumSamplesPerFinger[finger2];
                }

                m_isRunning = false;
            }
        }

        public bool IsRunning()
        {
            return m_isRunning;
        }

        public bool IsDone()
        {
            foreach( Finger finger in m_currentNumSamplesPerFinger.Keys )
            {
                if( m_currentNumSamplesPerFinger[finger] != m_numSamplesPerFinger )
                {
                    return false;
                }
            }

            return true;
        }

        public Dictionary<Finger, double> GetCalibrationResult()
        {
            return IsDone() ? m_calibrationResult : null;
        }
    }
}
