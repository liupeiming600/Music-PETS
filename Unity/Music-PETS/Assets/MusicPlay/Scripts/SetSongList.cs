using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace MusicPlay
{
    public class SetSongList : MonoBehaviour
    {
        Dropdown drop;
        TextInput textin;

        // Use this for initialization
        void Start()
        {
            drop = GetComponent<Dropdown>();
            textin = GameObject.Find("SettingCanvas").GetComponent<TextInput>();
            //Debug.Log(textin.SongListPath);
            string[] songlist = Directory.GetDirectories(textin.SongListPath);

            foreach (string songpath in songlist)
            {
                string song = Path.GetFileName(songpath);
                drop.options.Add(new Dropdown.OptionData { text = song });
            }

            drop.RefreshShownValue();

            textin.ChooseSong();
        }
    }
}
