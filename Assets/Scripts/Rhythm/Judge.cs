using UnityEngine;

public static class Judge {
    // returns (deltaMs, grade)
    public static (float, string) GradeHit(float noteTimeSec, float inputTimeSec, float userOffsetMs = 0f) {
        float deltaMs = 1000f * (inputTimeSec - noteTimeSec) - userOffsetMs;
        float ad = Mathf.Abs(deltaMs);
        string g = ad <= 40 ? "Perfect" : ad <= 80 ? "Great" : ad <= 120 ? "Good" : "Miss";
        return (deltaMs, g);
    }
}
