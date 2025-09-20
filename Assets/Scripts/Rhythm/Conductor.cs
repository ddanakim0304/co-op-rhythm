using UnityEngine;

public class Conductor : MonoBehaviour {
    public Chart chart;
    public AudioSource audioSrc;

    public double songStartDsp { get; private set; }
    public double DspNow => AudioSettings.dspTime;
    public float SongPosSec => (float)(DspNow - songStartDsp);

    [ContextMenu("TEST: ScheduleStart Now")]
    void TestStartNow() { ScheduleStart(1.0); }

    public void ScheduleStart(double startInFutureSec = 1.0) {
        if (chart == null) { Debug.LogError("Conductor: chart is NULL"); return; }
        if (audioSrc == null) { Debug.LogError("Conductor: audioSrc is NULL"); return; }
        if (chart.clip == null) { Debug.LogError("Conductor: chart.clip is NULL (assign an AudioClip on your Chart asset)"); return; }

        songStartDsp = AudioSettings.dspTime + startInFutureSec + chart.leadInSec;
        audioSrc.clip = chart.clip;
        audioSrc.PlayScheduled(songStartDsp);
        Debug.Log($"Conductor: scheduled at dsp={songStartDsp:F3}");
    }
}
