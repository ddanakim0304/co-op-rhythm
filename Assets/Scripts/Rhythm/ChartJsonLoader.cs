using UnityEngine;
using System;
using System.IO;

[Serializable]
public class JsonNote { public float t; public int lane; }

[Serializable]
public class JsonChart { public float leadInSec = 1.5f; public JsonNote[] notes; }

public class ChartJsonLoader : MonoBehaviour {
    [Header("Assign your JSON as a TextAsset (drag from Assets/Charts/)")]
    public TextAsset jsonFile;

    [Header("Hook your existing components")]
    public Conductor conductor;
    public NoteMatcher matcher;
    public SimpleNoteRenderer rendererRef;

    [Header("Audio (clip must be set here)")]
    public AudioSource audioSrc; // assign your song clip in Inspector

    void Awake() {
        if (jsonFile == null) { Debug.LogError("ChartJsonLoader: jsonFile is null"); return; }
        if (conductor == null || matcher == null || rendererRef == null || audioSrc == null) {
            Debug.LogError("ChartJsonLoader: missing refs"); return;
        }

        // Parse JSON
        var data = JsonUtility.FromJson<JsonChart>(jsonFile.text);
        if (data == null || data.notes == null || data.notes.Length == 0) {
            Debug.LogError("ChartJsonLoader: invalid or empty JSON"); return;
        }

        // Build a runtime Chart ScriptableObject for existing systems to use
        var chart = ScriptableObject.CreateInstance<Chart>();
        chart.leadInSec = data.leadInSec;
        chart.clip = audioSrc.clip; // we use the AudioSource’s clip
        chart.notes = new Chart.Note[data.notes.Length];
        for (int i = 0; i < data.notes.Length; i++) {
            chart.notes[i] = new Chart.Note { timeSec = data.notes[i].t, lane = data.notes[i].lane };
        }

        // Wire it everywhere
        conductor.chart = chart;
        conductor.audioSrc = audioSrc;
        matcher.chart = chart;
        rendererRef.chart = chart;
        rendererRef.conductor = conductor;
    }
}
