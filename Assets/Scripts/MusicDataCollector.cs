using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicDataCollector : MonoBehaviour
{
    private const int SAMPLE_SIZE = 1024;

    [SerializeField] float rmsValue;
    [SerializeField] float dbValue;
    [SerializeField] float pitchValue;
    float[] waveValues = new float[10];
    //[SerializeField] Transform[] cubes = new Transform[10];
    [SerializeField] float waveValuesModifier = 10.0f;
    [SerializeField] float smoothSpeed = 10.0f;
    [SerializeField] float rmsAlertValue = 0.0f;
    [SerializeField] UnityEvent rmsAlertEvents;
    bool a = false;
    AudioSource musicSource;
    float[] samples;
    float[] spectrum;
    float sampleRate;

    public float RmsValue { get => rmsValue; }
    public float DbValue { get => dbValue; }
    public float PitchValue { get => pitchValue; }

    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;
    }

    private void Update()
    {
        AnalyzeSound();
        UpdateWaveValues();
        RmsValueVerification();
    }
    private void AnalyzeSound()
    {
        musicSource.GetOutputData(samples, 0);

        //Get the RMS
        int i = 0;
        float sum = 0;
        for (; i < SAMPLE_SIZE; i++)
        {
            sum = samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        //Get the DB value
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        //Get sound spectrum
        musicSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        //Find pitch
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < SAMPLE_SIZE; ++i)
        {
            if (!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f))
                continue;

            maxV = spectrum[i];
            maxN = i;

        }

        float freqN = maxN;
        if (maxN > 0 && maxN < SAMPLE_SIZE - 1)
        {
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;
    }

    private void UpdateWaveValues()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int averageValues = SAMPLE_SIZE / waveValues.Length;

        for(visualIndex = 0; visualIndex < waveValues.Length; visualIndex++)
        {
            int j = 0;
            float sum = 0;
            for(j = 0; j < averageValues; j++)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }

            float scaleY = sum / averageValues * waveValuesModifier;
            waveValues[visualIndex] -= Time.deltaTime * smoothSpeed;
            if(waveValues[visualIndex] < scaleY)
                waveValues[visualIndex] = scaleY;
            //cubes[visualIndex].localScale = new Vector3(1, waveValues[visualIndex], 1);
        }
    }

    private void RmsValueVerification()
    {
        if(!a && waveValues[0] >= rmsAlertValue)
        {
            rmsAlertEvents.Invoke();
            a = true;
        }
        else if(waveValues[0] < rmsAlertValue)
        {
            a = false;
        }
    }
}
