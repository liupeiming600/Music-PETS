using System.Collections.Generic;

namespace HololensPETS
{
    public class Constants
    {
        public static readonly string FORCE_PLOT_SERIES_NAME = "ForcePlot";
        public static readonly string TARGET_FORCE_PLOT_SERIES_NAME = "TargetForcePlot";

        public static readonly int NETWORK_PORT = 11000;

        public static readonly List<Finger> FINGER_LIST = new List<Finger>{ Finger.Thumb, Finger.Index, Finger.Middle, Finger.Ring, Finger.Pinky };
    }
}
