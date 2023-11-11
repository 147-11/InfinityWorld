using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTableMIDI : MonoBehaviour
{
 public AudioSource audioSource;
 private float[] wavetableDetune;
private List<float> frecuencias = new List<float>();

    //public float FM=44100;
    public float FM = 48000f;  // Experimenta con valores más altos, por ejemplo, 96000f
    public float[] wavetable;
    //public int wavetableSize=2048;
    public int wavetableSize = 8192;  // Experimenta con valores más altos, por ejemplo, 8192
    //public float freqDetune = 10.0f;
    public float freqDetune = 40.0f;  // Experimenta con valores más altos
    float Amplitud;
    [Range(-60,0)]
    //public float dbfsValue = -20f;
    public float dbfsValue = -10f;  // Ajusta la ganancia global
    [Range(0.01f, 0.9f)]
    public float A = 1;
    [Range(0.1f, 5f)]
    public float D = 2;
    [Range(0.1f, 5f)]
    public float S = 3;
    [Range(0.1f, 5f)]
    public float R = 3;
    [Range(0.1f, 5f)]
    public float SustainLevel = 0.5f;
    public int ADSRindex { get; set; }
    float[] env;
    private float DBFSToLinearValue(float dbfs){
        return Mathf.Pow(10f, dbfs/20f);
    }
    public enum WaveformType{
        Sine,
        Square,
        Triangle,
        Sawtooth,
        PadWave,
        Noise,
        Kick,
        Snare 
    }
    public WaveformType waveformType=WaveformType.Square;
    public float SineWave(int timeindex, float frecuencia){
        return Mathf.Sin(2 * Mathf.PI * timeindex * frecuencia /FM);
    }
    public float SquareWave(int timeindex, float frecuencia){
        return Mathf.Sin(Mathf.Sin(2 * Mathf.PI * timeindex * frecuencia / FM));
    
    }
    public float TriangleWave(int timeIndex, float frecuencia){
        int Tm = (int)(FM / frecuencia);
        float m1 = 1 / ((Tm / 4.0f));
        float m2 = -2 / ((Tm * (3 / 4.0f)) - ((Tm / 4.0f)));
        float m3 = 1 / (Tm - ((Tm * 3) / 4.0f));
        float b1 = 1 - (m1 * (Tm / 4));
        float b2 = 1 - (m2 * (Tm / 4));
        float b3 = 0 - (m3 * Tm);
        int x = timeIndex - ((int)(timeIndex / Tm) * Tm);
        if (x <= (Tm / 4)) return (m1 * x + b1);
        else if (x > (Tm / 4) && x <= ((Tm * 3) / 4)) return (m2 * x + b2);
        else return (m3 * x + b3);
    }
    public float SawtoothWave(int timeIndex, float frecuencia){
        int Tm = (int)(FM / frecuencia);
        float m1 = 1 / ((Tm / 2.0f));
        float m2 = 1 / (Tm - ((Tm) / 2.0f));
        float b1 = 1 - (m1 * (Tm / 4));
        float b2 = 0 - (m2 * Tm);
        int x = timeIndex - ((int)(timeIndex / Tm) * Tm);
        if (x <= (Tm / 2)) return ((m1 * x + b1));
        else return ((m2 * x + b2));
    }
    private float WhiteNoise(){
        System.Random random = new System.Random();
        return (float)(random.NextDouble() * 2.0 - 1.0);

    }
        private float PadWave(){
        System.Random random = new System.Random();
        return (float)(random.NextDouble() * 2.0 - 1.0);

    }
    

    private void GenerateWavetable(){
        
        wavetable= new float[wavetableSize];
        wavetableDetune = new float[wavetableSize];
        float f = FM / wavetableSize;
        for (int i = 0; i < wavetableSize; i++){
            switch (waveformType){
                case WaveformType.Sine:
                    wavetable[i] += SineWave(i, f);
                    wavetableDetune[i] += SineWave(i, f+freqDetune);
                    break;
                case WaveformType.Square:
                    wavetable[i] += SquareWave(i, f);
                    wavetableDetune[i] += SquareWave(i, f + freqDetune);
                    break;
                case WaveformType.Triangle:
                    wavetable[i] += TriangleWave(i, f);
                    wavetableDetune[i] += SineWave(i, f + freqDetune);
                    break;
                case WaveformType. Sawtooth:
                    wavetable[i] += SawtoothWave(i, f);
                    wavetableDetune[i] += SawtoothWave(i, f + freqDetune);
                    break;
                case WaveformType.PadWave:
                    wavetable[i] += PadWave();
                    wavetableDetune[i] += PadWave();
                    break;
                case WaveformType.Noise:
                    wavetable[i] += WhiteNoise();
                    break;
                case WaveformType.Kick:
                    //wavetable[i] += KickNoise(i, f);
                     // Implementa la función de generación de ruido de golpe (Kick) si es necesario.
                    break;
                case 
                    WaveformType. Snare:
                    wavetable[i] += WhiteNoise();
                    break;   
                default:
                    break;
            }
            
        }
        
    }

    float[] ADSR() {
        int totalADSRSize = (int)(FM + (A + D + R + S));
        float[] envelope = new float[totalADSRSize];
        int ASamples = (int)(FM + A);
        int DSamples = (int)(FM * D);
        int SSamples = (int)(FM + S);
        int RSamples = (int)(FM * R);
        for (int i = 0; i < totalADSRSize; i++){
            float value = 0f;
            if (i < ASamples) value = Mathf.Lerp(0f, 1f, (float)i / ASamples);
            else if (i < ASamples + DSamples) value = Mathf.Lerp(1f, SustainLevel, (float)i / DSamples);
            else if (i < ASamples + DSamples + SSamples) value = SustainLevel;
            else if (i < ASamples + DSamples + SSamples + RSamples) value = Mathf.Lerp( SustainLevel, 0f, (float)i / RSamples);
        envelope[i] = value;
        }
        
        return envelope;
   }
/*    //versión Monofónica
    public float frecuencia;
    float phaseM;
    private void OnAudioFilterRead(float[] data, int channels){
        for (int i = 0; i < data.Length; i += channels){
            float E;
            if (ADSRindex < env.Length) E = env[ADSRindex];
            else E = 0f;
            float currentsample = 0;
                try{
                currentsample += wavetable[(int)(phaseM * wavetableSize)] * Amplitud * E;
                    for (int channel = 0; channel < channels; channel++) data[i + channel] = currentsample;
                
                }
                catch (System.IndexOutOfRangeException ex){
                  print("An Indexoutofrangeexception occurred.");
                print("Error message: " + ex.Message);  
                }
                
                try{
                    phaseM += frecuencia / FM;
                    if (phaseM > 1f) phaseM -= 1f;
                }
                
                catch (System.IndexOutOfRangeException ex){
                    print("An Indexoutofrangeexception occurred.");
                    print(" Error message: "+ ex.Message);
                            
                }
                ADSRindex++;
        }
            
    } */

// Versión Estéreo
public float frecuencia;
float phaseM;

private void OnAudioFilterRead(float[] data, int channels)
{
        for (int i = 0; i < data.Length; i += channels)
    {
        float E;
        if (ADSRindex < env.Length)
        {
            E = env[ADSRindex];
        }
        else
        {
            E = 0f;
        }

        float currentSample = 0;

        // Aplicar la envolvente a las muestras generadas
        try
        {
            currentSample += wavetable[(int)(phaseM * wavetableSize)] * Amplitud * E;

            for (int channel = 0; channel < channels; channel++)
            {
                // Distribuir el sonido en canales izquierdo y derecho.
                data[i + channel] = currentSample;
            }
        }
        catch (System.IndexOutOfRangeException ex)
        {
            Debug.LogError("An IndexOutOfRangeException occurred.");
            Debug.LogError("Error message: " + ex.Message);
        }

        try
        {
            // Asegúrate de que phaseM esté en el rango [0, 1)
            phaseM += frecuencia / FM;
            if (phaseM >= 1f) phaseM -= 1f;
        }
        catch (System.IndexOutOfRangeException ex)
        {
            Debug.LogError("An IndexOutOfRangeException occurred.");
            Debug.LogError("Error message: " + ex.Message);
        }

        ADSRindex++;
    }
}


    public void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.playOnAwake = true;
        audioSource.spatialBlend = 0;
        GenerateWavetable();
        env = ADSR();
        Application.runInBackground = true;
        audioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        Amplitud =DBFSToLinearValue(dbfsValue);
        frecuencias.Add(0f);
   

    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
