using UnityEngine;
using System;

[CreateAssetMenu(menuName="Rhythm/Chart")]
public class Chart : ScriptableObject {
    [Serializable] public struct Note { public float timeSec; public int lane; } // lane 0 or 1
    public AudioClip clip;
    public Note[] notes;
    [Range(0,5f)] public float leadInSec = 1.5f; // pre-roll before first note
}
