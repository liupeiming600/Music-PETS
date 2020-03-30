namespace HololensPETSGames
{
    [System.Serializable]
    public class Note
    {
        public NoteAction noteAction = NoteAction.None;

        // Time this note is in sync with a part of the song
        public float timeInSong;

        // Amount of time this note will take
        public float duration;

        // Which note lane will this spawn
        public int noteLaneIndex;
    }
}
