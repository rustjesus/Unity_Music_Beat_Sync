using UnityEngine;

public class BeatLightManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public int spectrumIndex = 5;          // Which FFT band to sample (0–127)
    public float sensitivity = 50f;        // Boost multiplier

    [Header("Lights to Control")]
    public Light[] lights;

    [Header("Intensity Settings")]
    public float baseIntensity = 1f;       // Minimum intensity
    public float maxIntensity = 5f;        // Maximum intensity on beats
    public float smoothSpeed = 8f;         // Lerp speed for smooth flicker

    private float[] spectrum = new float[128];
    private float targetIntensity;

    void Update()
    {
        if (audioSource == null || lights.Length == 0)
            return;

        // Get frequency data
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // Read intensity from selected band
        float intensity = spectrum[spectrumIndex] * sensitivity;

        // Clamp intensity to safe range
        targetIntensity = Mathf.Clamp(baseIntensity + intensity, baseIntensity, maxIntensity);

        // Apply to each light
        foreach (var light in lights)
        {
            if (light == null) continue;

            float current = light.intensity;
            float newIntensity = Mathf.Lerp(current, targetIntensity, Time.deltaTime * smoothSpeed);

            light.intensity = newIntensity;
        }
    }
}
