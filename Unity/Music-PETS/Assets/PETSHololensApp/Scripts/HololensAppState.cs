using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETS
{
    public class HololensAppState : MonoBehaviour
    {
        public enum State
        {
            Start,
            WaitingForAnchor,
            InitializingVuforia,
            Calibrating,
            ConfirmCalibration,
            Running
        }
        public ObservableValue<HololensAppState.State> CurrentState { get; private set; }

        public List<Transform> FingerTransforms { get; set; }
        
        private void Awake()
        {
            FingerTransforms = new List<Transform>();

            CurrentState = new ObservableValue<HololensAppState.State>();
            CurrentState.Set( HololensAppState.State.Start, false );
        }
    }
}
