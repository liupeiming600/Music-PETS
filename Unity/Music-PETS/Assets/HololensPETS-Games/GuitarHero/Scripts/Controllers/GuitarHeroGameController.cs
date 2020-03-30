using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using HololensPETS;

namespace HololensPETSGames
{
    public class GuitarHeroGameController : MonoBehaviour
    {
        public Text scoreText;
        
        public ObservableValue<int> score;

        public bool forceScalingMode = true;

        public SongListController songList;
        public SongPlayerController songPlayer;

        private void Awake()
        {
            score = new ObservableValue<int>();
        }

        private void Start()
        {
            score.OnObservedValueChanged += OnScoreChanged;
        }

        private void OnScoreChanged( int oldValue, int newValue )
        {
            scoreText.text = "Score: " + newValue;
        }
    }
}
