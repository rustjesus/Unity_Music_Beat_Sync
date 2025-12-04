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

    [Header("Color Settings")]
    public bool changeColorWithAudio = false;
    public Color lowColor = Color.blue;
    public Color highColor = Color.red;

    private float[] spectrum = new float[128];
    private float targetIntensity;

    void Update()
    {
        if (audioSource == null || lights.Length == 0)
            return;

        // Get frequency data
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // Compute audio-based intensity
        float rawIntensity = spectrum[spectrumIndex] * sensitivity;
        targetIntensity = Mathf.Clamp(baseIntensity + rawIntensity, baseIntensity, maxIntensity);

        // Normalized "audio amount" for color shifting (0–1)
        float audioLevelNormalized = Mathf.Clamp01(rawIntensity / (maxIntensity - baseIntensity));

        foreach (var light in lights)
        {
            if (light == null) continue;

            // Smooth intensity
            light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime * smoothSpeed);

            // Color reaction
            if (changeColorWithAudio)
            {
                Color targetColor = Color.Lerp(lowColor, highColor, audioLevelNormalized);
                light.color = Color.Lerp(light.color, targetColor, Time.deltaTime * smoothSpeed);
            }
        }
    }
}
