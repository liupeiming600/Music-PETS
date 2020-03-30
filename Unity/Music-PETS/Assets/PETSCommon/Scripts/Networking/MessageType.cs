namespace HololensPETS
{
    public class MessageType
    {
        public enum Command
        {
            FingerData = 0,
            Data = 1,
            ModeSet = 2,
            Control = 3
        }

        public enum Mode
        {
            Static = 0,
            Dynamic = 1,
            FlappyBird = 2,
            GuitarHero = 3,
            MusicPlay = 4
        }

        public enum ControlType
        {
            Start = 0,
            Stop = 1,
            Next = 2,
            PlaySong = 3,
            StopSong = 4,
            SpawnNote = 5,
            SpawnForceScalingNote = 6,
            PressKey = 7,
            ReleaseKey = 8,

            SpawnObstacle = 9,
            PlayerPosition = 10,
            UpdateScore = 11,
            TriggerGameOver = 12,
            ResetGame = 13,
            ChangeActiveFingers = 14,

            InitVisualizations = 15,
            ChangeForceScalingMode = 16,

            ChangeColor = 17,
            DestroyNote = 18,
            ChangeTempo = 19
        }
    }
}
