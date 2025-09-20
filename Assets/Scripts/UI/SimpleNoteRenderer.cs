using UnityEngine;

public class SimpleNoteRenderer : MonoBehaviour {
    public Conductor conductor;
    public Chart chart;

    [Header("Visuals")]
    public GameObject notePrefab; // a small circle sprite with SpriteRenderer
    public Transform lane0Parent, lane1Parent;
    public float pixelsPerSecond = 300f;
    public float spawnAheadSec = 3f; // how far ahead notes appear
    public float hitLineX = 0f;      // world X of the hit line

    int spawnedCount;

    void Update() {
        if (chart == null || conductor == null || chart.notes == null) return;

        float songT = conductor.SongPosSec;

        // spawn ahead
        while (spawnedCount < chart.notes.Length &&
               chart.notes[spawnedCount].timeSec <= songT + spawnAheadSec) {
            Spawn(chart.notes[spawnedCount]);
            spawnedCount++;
        }

        // move all notes towards hit line based on (time - songT)
        MoveChildren(lane0Parent, songT);
        MoveChildren(lane1Parent, songT);
    }

    void Spawn(Chart.Note n) {
        var parent = n.lane == 0 ? lane0Parent : lane1Parent;
        var go = Instantiate(notePrefab, parent);
        go.name = $"Note_{n.lane}_{n.timeSec:0.00}";
        // initial position placed such that it reaches hitLineX at its time
        float dx = (n.timeSec - conductor.SongPosSec) * pixelsPerSecond;
        var p = parent.position;
        p.x = hitLineX + dx;
        go.transform.position = p;
        var sr = go.GetComponent<SpriteRenderer>();
        if (sr) sr.color = (n.lane == 0) ? new Color(0.2f,0.6f,1f) : new Color(1f,0.4f,0.3f);
    }

    void MoveChildren(Transform parent, float songT) {
        foreach (Transform c in parent) {
            // parse time from name OR store on component; here we store time in z for quick hack
            if (!c.TryGetComponent<NoteTimeTag>(out var tag)) continue;
            float dx = (tag.timeSec - songT) * pixelsPerSecond;
            var pos = c.position;
            pos.x = hitLineX + dx;
            c.position = pos;
        }
    }

    public void ResetAll() {
        spawnedCount = 0;
        foreach (Transform t in lane0Parent) Destroy(t.gameObject);
        foreach (Transform t in lane1Parent) Destroy(t.gameObject);
    }
}