using System.Collections.Generic;

using UnityEngine;

namespace HololensPETS
{
    public class AppState : MonoBehaviour
    {
        public HandData LeftHandData { get; private set; }
        public HandData RightHandData { get; private set; }

        public ObservableValue<bool> IsLeftHand;

        public double ForceTrackMVCRatio { get; set; }

        public AppMode Mode { get; set; }

        public float StaticPhaseDuration { get; set; }
        public float IncreasingPhaseDuration { get; set; }
        public float PlateauPhaseDuration { get; set; }
        public float DecreasingPhaseDuration { get; set; }
        public float StaticEndPhaseDuration { get; set; }

        public string HololensIP { get; set; }

        private void Awake()
        {
            LeftHandData = new HandData();
            RightHandData = new HandData();

            IsLeftHand = new ObservableValue<bool>( true );
            ForceTrackMVCRatio = 1.0;

            Mode = AppMode.Dynamic;

            StaticPhaseDuration = 5.0f;
            IncreasingPhaseDuration = 5.0f;
            PlateauPhaseDuration = 5.0f;
            DecreasingPhaseDuration = 5.0f;
            StaticEndPhaseDuration = 5.0f;
            
            HololensIP = "";
        }

        public HandData GetCurrentHandData()
        {
            return IsLeftHand.Value ? LeftHandData : RightHandData;
        }

        public void SetHandData( HandData leftHand, HandData rightHand )
        {
            LeftHandData = leftHand;
            RightHandData = rightHand;
        }
    }
}
