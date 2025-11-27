using UnityEngine;

public class BeatManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public int spectrumIndex = 5;          // Which frequency band to read (0–127)
    public float sensitivity = 50f;        // Scale multiplier

    [Header("Objects to Scale")]
    public Transform[] objects;

    [Header("Scale Settings")]
    public float baseScale = 1f;           // Resting scale
    public float maxScale = 2f;            // Maximum scale on strong beats
    public float smoothSpeed = 8f;         // How fast scale reacts

    private float[] spectrum = new float[128];
    private float targetScale;

    void Update()
    {
        if (audioSource == null || objects.Length == 0)
            return;

        // Get spectrum data (FFT)
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // Use selected band as intensity
        float intensity = spectrum[spectrumIndex] * sensitivity;

        // Clamp scale
        targetScale = Mathf.Clamp(baseScale + intensity, baseScale, maxScale);

        // Smoothly scale all objects
        foreach (var obj in objects)
        {
            if (obj == null) continue;

            float current = obj.localScale.x;
            float newScale = Mathf.Lerp(current, targetScale, Time.deltaTime * smoothSpeed);

            obj.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}
