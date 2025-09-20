using UnityEngine;

public class StartSongButton : MonoBehaviour {
    public Conductor conductor;
    public NoteMatcher matcher;
    public SimpleNoteRenderer rendererRef;

    public void StartSong() {
        matcher.ResetAll();
        rendererRef.ResetAll();
        conductor.ScheduleStart(1.0); // schedule 1s in the future + lead-in
    }
}
