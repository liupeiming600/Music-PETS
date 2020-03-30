using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicPlay
{
    public partial class OSC_Send
    {
        public Dropdown TrackDrop;

        public void TrackSelect()
        {
            int value = TrackDrop.GetComponent<Dropdown>().value;
            client.Send("/track", value);
            //for (int i = 0; i < 128; i++) NoteOn(i, 0);
        }
    }
}
