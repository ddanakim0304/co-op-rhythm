using UnityEngine;

public class NoteMatcher : MonoBehaviour {
    public Chart chart;
    [Range(0.05f, 0.25f)] public float matchWindowSec = 0.15f; // 150 ms
    int nextIdxLane0, nextIdxLane1;

    public void ResetAll() { nextIdxLane0 = nextIdxLane1 = 0; }

    public int? FindMatch(int lane, float inputTimeSec) {
        int start = lane == 0 ? nextIdxLane0 : nextIdxLane1;
        for (int i = start; i < chart.notes.Length; i++) {
            var n = chart.notes[i];
            if (n.lane != lane) continue;
            float dt = Mathf.Abs(n.timeSec - inputTimeSec);
            if (dt <= matchWindowSec) {
                if (lane == 0) nextIdxLane0 = i + 1; else nextIdxLane1 = i + 1;
                return i;
            }
            if (n.timeSec > inputTimeSec + matchWindowSec) break;
        }
        return null;
    }
}
