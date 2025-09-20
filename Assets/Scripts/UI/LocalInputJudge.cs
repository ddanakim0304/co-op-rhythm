using UnityEngine;
using TMPro; // <-- TextMesh Pro

public class LocalInputJudge : MonoBehaviour {
    [Header("Refs")]
    public Conductor conductor;
    public NoteMatcher matcher;
    public TextMeshProUGUI debugText; // optional

    [Header("Controls")]
    public KeyCode p1Key = KeyCode.A;     // Player 1 (lane 0)
    public KeyCode p2Key = KeyCode.Quote; // Player 2 (lane 1)

    [Header("Offsets (ms)")]
    public float p1OffsetMs = 0f;
    public float p2OffsetMs = 0f;

    int p1Score, p2Score;

    void Update() {
        if (conductor == null || matcher == null) return;
        float t = conductor.SongPosSec;
        if (Input.GetKeyDown(p1Key)) HandleHit(0, t, ref p1Score, p1OffsetMs);
        if (Input.GetKeyDown(p2Key)) HandleHit(1, t, ref p2Score, p2OffsetMs);
    }

    void HandleHit(int lane, float inputT, ref int score, float offsetMs) {
        var idx = matcher.FindMatch(lane, inputT);
        string grade; float delta;
        if (idx.HasValue) {
            var noteTime = matcher.chart.notes[idx.Value].timeSec;
            (delta, grade) = Judge.GradeHit(noteTime, inputT, offsetMs);
            if (grade == "Perfect") score += 300;
            else if (grade == "Great")  score += 200;
            else if (grade == "Good")   score += 100;
        } else {
            grade = "Miss"; delta = 0;
        }

        if (debugText != null) {
            debugText.text = $"P1:{p1Score}  P2:{p2Score}\nLast: lane{lane} {grade} ({delta:+0;-0} ms)";
        }
    }
}
