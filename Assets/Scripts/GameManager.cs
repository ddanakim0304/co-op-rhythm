using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public GameObject notePrefab;
    public Transform spawnPoint;
    public Transform targetZone;
    public AudioClip song;
    public AudioClip redHitSound;
    public AudioClip blueHitSound;

    [Header("Rhythm")]
    public float bpm = 113f;

    [Tooltip("Optional: delay before the song actually starts. Use if your audio has silence at the start.")]
    public float songStartDelay = 0f;

    [Header("Gameplay")]
    public float noteSpeed = 8f;
    public float hitRange = 1.0f;

    private AudioSource audioSource;
    private List<GameObject> activeNotes = new List<GameObject>();
    private float secondsPerBeat;
    private float noteTravelTime;
    private int nextBeatToSpawn;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = song;
    }

    void Start()
    {
        secondsPerBeat = 60f / bpm;

        float distance = Vector2.Distance(spawnPoint.position, targetZone.position);
        noteTravelTime = distance / noteSpeed;

        // 🔧 KEY: start from the first beat whose spawn time is >= 0
        // spawnTime = beatTime - travelTime >= 0  ⇒  beatTime >= travelTime
        // beatTime = beatIndex * spb
        nextBeatToSpawn = Mathf.CeilToInt(noteTravelTime / secondsPerBeat);

        // Start the song (optionally delayed if your audio file needs it)
        if (songStartDelay > 0f) audioSource.PlayDelayed(songStartDelay);
        else audioSource.Play();
    }

    void Update()
    {
        // 1) Player input
        if (Input.GetKeyDown(KeyCode.A)) CheckForHit("RED");
        if (Input.GetKeyDown(KeyCode.L)) CheckForHit("BLUE");

        // If not playing yet, don't spawn
        if (!audioSource.isPlaying) return;

        // 2) BPM-synced spawning
        float songTime = audioSource.time; // time since audio actually began
        float timeOfNextBeat = nextBeatToSpawn * secondsPerBeat;

        // Spawn when the note needs to LEAVE to arrive on the beat
        while (songTime >= timeOfNextBeat - noteTravelTime)
        {
            SpawnNote();
            nextBeatToSpawn++;
            timeOfNextBeat = nextBeatToSpawn * secondsPerBeat;
        }

        // 3) Move notes & cleanup
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            GameObject note = activeNotes[i];
            note.transform.Translate(Vector2.left * noteSpeed * Time.deltaTime);

            if (note.transform.position.x < -10f)
            {
                Debug.Log("Miss!");
                activeNotes.RemoveAt(i);
                Destroy(note);
            }
        }
    }

    void SpawnNote()
    {
        GameObject newNote = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);
        if (Random.value > 0.5f)
        {
            newNote.GetComponent<SpriteRenderer>().color = Color.red;
            newNote.name = "RED_Note";
        }
        else
        {
            newNote.GetComponent<SpriteRenderer>().color = Color.blue;
            newNote.name = "BLUE_Note";
        }
        activeNotes.Add(newNote);
    }

    void CheckForHit(string hitType)
    {
        for (int i = 0; i < activeNotes.Count; i++)
        {
            GameObject note = activeNotes[i];
            float distance = Vector2.Distance(note.transform.position, targetZone.position);
            if (distance < hitRange)
            {
                if (hitType == "RED" && note.name.Contains("RED"))
                {
                    Debug.Log("HIT on a RED note!");
                    audioSource.PlayOneShot(redHitSound);
                    activeNotes.RemoveAt(i);
                    Destroy(note);
                    return;
                }
                else if (hitType == "BLUE" && note.name.Contains("BLUE"))
                {
                    Debug.Log("HIT on a BLUE note!");
                    audioSource.PlayOneShot(blueHitSound);
                    activeNotes.RemoveAt(i);
                    Destroy(note);
                    return;
                }
            }
        }
        Debug.Log("Mismatched Hit!");
    }
}
