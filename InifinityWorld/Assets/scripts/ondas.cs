using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class ondas : MonoBehaviour
{
    //slidres
    public Slider[] sliders;
    public Text[] valueTexts;
    public Dropdown formaDropdown; // Referencia al Dropdown
    public GameObject boton;
   // public Forma formaSeleccionada; // Opción seleccionada del Dropdown

    //fin


    //variables
    [Range(0, 1)]
    public float[] Amplitudes = new float[10] { 1, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f, 0.3f, 0.2f };

    int ar = 10;
    int tm;

    public float fr;
    public float mu = 44100;
    public float t1 = 2.0f;
    public float am;
    AudioSource audio;
    int t0 = 0;
    int Funcion = 0;

    public float TSegs = 1.0f;

    //ads
    [Range(5, 100)]
    public float A=100;

    [Range(20, 300)]
    public float D = 200;

    [Range(200, 1000)]
    public float S = 300;

     [Range(20, 100)]
    public float R = 800;

    int tA;
    int tD;
    int tS;
    int tR;
    int ADSRindex = 0;

    public float[] env;

    //Manejo importante de los hilos de Unity (Hilo de juego)
    Dictionary<KeyCode, bool> keyStates = new Dictionary<KeyCode, bool>();
    Dictionary<KeyCode, float> noteFrequencies = new Dictionary<KeyCode, float>
    {
        //Notas en la Octava #4
        { KeyCode.Q, 261.63f }, // Do
        { KeyCode.W, 293.66f }, // Re
        { KeyCode.E, 329.63f }, // Mi
        { KeyCode.R, 349.23f }, // Fa
        { KeyCode.T, 392.00f }, // Sol
        { KeyCode.Y, 440.00f }, // La
        { KeyCode.U, 493.88f }, // Si
        { KeyCode.A, 277.18f }, // Do#
        { KeyCode.S, 311.13f }, // Re#
        { KeyCode.D, 369.99f }, // Mi#
        { KeyCode.F, 415.30f }, // Fa#
        { KeyCode.G, 466.16f }, // Sol#
    };

    //ONDAS:
    //seno
    public float fseno(int t0, float fr, float mu, float am)
    {

        return am * Mathf.Sin(2 * Mathf.PI * t0 * fr / mu);

    }

    //cuadro
    public float fcuadrada(int t0, float fr, float mu, float am)
    {

        return Mathf.Sign(Mathf.Sin(2 * Mathf.PI * t0 * fr / mu)) * am;
    }

    //triangulo
    public float ftriangular(int t0, float fr, float mu, float am, int tm)
    {

        float m1 = 1 / ((tm / 4.0f));
        float m2 = -2 / ((tm * (3 / 4.0f)) - ((tm / 4.0f)));
        float m3 = 1 / (tm - ((tm * 3) / 4.0f));

        float b1 = 1 - (m1 * (tm / 4));
        float b2 = 1 - (m2 * (tm / 4));
        float b3 = 0 - (m3 * tm);

        int x = t0 - ((int)(t0 / tm) * tm);

        if (x <= (tm / 4))
        {
            return am * (m1 * x + b1);
        }
        else if (x > (tm / 4) && x <= ((tm * 3) / 4))
        {
            return am * (m2 * x + b2);
        }
        else
        {
            return am * (m3 * x + b3);
        }
    }

    //Diente sierra
    public float fsierra(int t0, float fr, float mu, float am, int tm)
    {

        float m1 = 1 / ((tm / 2.0f));
        float m2 = 1 / (tm - ((tm) / 2.0f));

        float b1 = 1 - (m1 * (tm / 4));
        float b2 = 0 - (m2 * tm);

        int x = t0 - ((int)(t0 / tm) * tm);

        if (x <= (tm / 2))
        {
            return (am * (m1 * x + b1));
        }
        else
        {
            return (am * (m2 * x + b2));
        }
    }

    public enum Forma
    {
        seno,
        cuadrada,
        triangular,
        sierra
    }

    public Forma forma = Forma.cuadrada;


    //filtos:
    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            float x = 0;

            for (int j = 1; j <= ar; j++)
            {
                tm = (int)(mu / (fr * j));
                switch (forma)
                {
                    case Forma.seno:
                        x += fseno(t0, fr * j, mu, Amplitudes[j - 1]);
                        break;
                    case Forma.cuadrada:
                        x += fcuadrada(t0, fr * j, mu, Amplitudes[j - 1]);
                        break;
                    case Forma.triangular:
                        x += ftriangular(t0, fr, mu, 1, tm);
                        break;
                    case Forma.sierra:
                        x += fsierra(t0, fr * j, mu, Amplitudes[j - 1], tm);
                        break;
                }
            }

            float E;

            if (ADSRindex < env.Length) E = env[ADSRindex];
            else E = 0f;

            data[i] = E * x / (float)ar;

            if (channels == 2)
                data[i + 1] = E * x / (float)ar;


            t0++;
            if (t0 >= (mu * TSegs)) t0 = 0;

            ADSRindex++;

        }
    }

    void Start()
    {
        audio = gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.spatialBlend = 0;

 boton= GameObject.FindWithTag("Boton");
        Button btn = boton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
        
      // del Dropdown
      formaDropdown.onValueChanged.AddListener(ActualizarForma);

         // Suscribirse al evento de cambio de valor del Slider
           //ataque
            sliders[0].value = A; 
       // Suscribirse a los eventos de cambio de valor de los Sliders
        sliders[0].onValueChanged.AddListener(delegate { UpdateValueText(0); });
          UpdateValueText(0); // Actualizar el texto inicial
            // Configurar el valor inicial del Slider
            //deacaimiento
            sliders[1].value = D;
        sliders[1].onValueChanged.AddListener(delegate { UpdateValueText(1); });
            UpdateValueText(1); 
            //sostenido
                        sliders[2].value = S;
        sliders[2].onValueChanged.AddListener(delegate { UpdateValueText(2); });
            UpdateValueText(2); 
            //liberacion
                            sliders[3].value = R;
        sliders[3].onValueChanged.AddListener(delegate { UpdateValueText(3); });
            UpdateValueText(3); 
            //Amplitudes 1  { 1, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f, 0.3f, 0.2f };
                            sliders[4].value = Amplitudes[0];
        sliders[4].onValueChanged.AddListener(delegate { UpdateValueText(4); });
            UpdateValueText(4); 
                        //Amplitudes 0.9f
                            sliders[5].value = Amplitudes[1];
        sliders[5].onValueChanged.AddListener(delegate { UpdateValueText(5); });
            UpdateValueText(5); 
                           //Amplitudes 0.8f
                            sliders[6].value = Amplitudes[2];
        sliders[6].onValueChanged.AddListener(delegate { UpdateValueText(6); });
            UpdateValueText(6); 
                           //Amplitudes 0.7f
                            sliders[7].value = Amplitudes[3];
        sliders[7].onValueChanged.AddListener(delegate { UpdateValueText(7); });
            UpdateValueText(7); 
                           //Amplitudes 0.6f
                            sliders[8].value = Amplitudes[4];
        sliders[8].onValueChanged.AddListener(delegate { UpdateValueText(8); });
            UpdateValueText(8); 
                           //Amplitudes 0.5f
                            sliders[9].value = Amplitudes[5];
        sliders[9].onValueChanged.AddListener(delegate { UpdateValueText(9); });
            UpdateValueText(9); 
                                       //Amplitudes 0.4f
                            sliders[10].value = Amplitudes[6];
        sliders[10].onValueChanged.AddListener(delegate { UpdateValueText(10); });
            UpdateValueText(10); 
                                                   //Amplitudes 0.3f
                            sliders[11].value = Amplitudes[7];
        sliders[11].onValueChanged.AddListener(delegate { UpdateValueText(11); });
            UpdateValueText(11);  
                                                   //Amplitudes 0.3f
                            sliders[12].value = Amplitudes[8];
        sliders[12].onValueChanged.AddListener(delegate { UpdateValueText(12); });
            UpdateValueText(12);  
                                                               //Amplitudes 0.2f
                      sliders[13].value = Amplitudes[9];
       sliders[13].onValueChanged.AddListener(delegate { UpdateValueText(13); });
            UpdateValueText(13);  

        
     
        audio.Stop();
    }

     void Update()
    {
        ADSRvalues();
        ADSR();
        //Por cada tecla que se presione, se tiene en cuenta si está levantada o no
        foreach (var kvp in noteFrequencies)
        {
            KeyCode key = kvp.Key;
            float noteFrequency = 261.63f;

            if (Input.GetKeyDown(key))
            {
                t0 = 0;
                ADSRindex = 0;
                keyStates[key] = true;

                // basado en la tecla presionada
                fr = noteFrequency;
            }

            if (Input.GetKeyUp(key))
            {
                keyStates[key] = false;
            }
        }

        //Para iniciar el piano con la tecla espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!audio.isPlaying)
            {
                t0 = 0;
                audio.Play();
            }
            else
            {
                audio.Stop();
            }
        }
    }

void TaskOnClick(){
		//Debug.Log ("You have clicked the button!");
 //{ KeyCode.Q, 261.63f }, // Do
         ADSRvalues();
        ADSR();
        //Por cada tecla que se presione, se tiene en cuenta si está levantada o no
        foreach (var kvp in noteFrequencies)
        {
            KeyCode key =  KeyCode.Q;
            float noteFrequency = kvp.Value;

           

                t0 = 0;
                ADSRindex = 0;
                keyStates[key] = true;

                // basado en la tecla presionada
                fr = noteFrequency;
            

           
        }

         if (!audio.isPlaying)
            {
                t0 = 0;
                ADSRindex = 0;
              
                audio.Play();
            
            }
            else
            {
                audio.Stop();
            }
         
	}

    private void UpdateValueText(int index)
    {
        float value = sliders[index].value;
        //ataque
       if (index == 0)
        {
            A = value;
            valueTexts[0].text = "Ataque: " + A.ToString("F2");
        }
        else if (index == 1)
        {
            D = value;
            valueTexts[1].text = "Decaimiento: " + D.ToString("F1");
        }
          else if (index == 2)
        {
            S = value;
            valueTexts[2].text = "Sostenido: " + S.ToString("F1");
        }
         else if (index == 3)
        {
            R = value;
            valueTexts[3].text = "Liberación: " + R.ToString("F1");
        }
          else if (index == 4)
        {
            Amplitudes[0] = value;
            valueTexts[4].text = "Amplitud 1: " + Amplitudes[0].ToString("F1");
        }
                  else if (index == 5)
        {
            Amplitudes[1] = value;
            valueTexts[5].text = "Amplitud 0.9: " + Amplitudes[1].ToString("F1");
        }
                else if (index == 6)
        {
            Amplitudes[2] = value;
            valueTexts[6].text = "Amplitud 0.8: " + Amplitudes[2].ToString("F1");
        }
                else if (index == 7)
        {
            Amplitudes[3] = value;
            valueTexts[7].text = "Amplitud 0.7: " + Amplitudes[3].ToString("F1");
        }
                else if (index == 8)
        {
            Amplitudes[4] = value;
            valueTexts[8].text = "Amplitud 0.6: " + Amplitudes[4].ToString("F1");
        }
                else if (index == 9)
        {
            Amplitudes[5] = value;
            valueTexts[9].text = "Amplitud 0.5: " + Amplitudes[5].ToString("F1");
        }
                        else if (index == 10)
        {
            Amplitudes[6] = value;
            valueTexts[10].text = "Amplitud 0.4: " + Amplitudes[6].ToString("F1");
        }                
        else if (index == 11)
        {
            Amplitudes[7] = value;
            valueTexts[11].text = "Amplitud 0.3: " + Amplitudes[7].ToString("F1");
        }
                         else if (index == 12)
        {
            Amplitudes[8] = value;
            valueTexts[12].text = "Amplitud 0.3: " + Amplitudes[8].ToString("F1");
        }
                                 else if (index == 13)
        {
            Amplitudes[9] = value;
            valueTexts[13].text = "Amplitud 0.2: " + Amplitudes[9].ToString("F1");
        }
                                   
        
    }
    private void ActualizarForma(int indice)
    {
        
        forma = (Forma)indice;
       // Debug.Log("Forma seleccionada: " + forma.ToString());
    }

    void ADSRvalues()
    {
        tA = (int)(A * (mu / 1000));
        tD = (int)(D * (mu / 1000));
        tS = (int)(S * (mu / 1000));
        tR = (int)(R * (mu / 1000));
    }
    void ADSR()
    {
        float[] As = new float[tA];
        float[] Ds = new float[tD];
        float[] Ss = new float[tS];
        float[] Rs = new float[tR];

        //ataque
        for (int i = 0; i < tA; i++)
        {
            As[i] = (Mathf.Pow((i / tA), (1.0f / 3.0f)));
        }

        //decay
        float ID = 0.5f / tD;
        float VD = 1f;
        for (int i = 0; i < tD; i++)
        {
            Ds[i] = (Mathf.Pow(VD, (1.0f / 3.0f)));
            VD -= ID;
        }

        //sustain
        float m = -(0.8f / tS);
        for (int i = 0; i < tS; i++)
        {
            Ss[i] = m * i + 0.8f;
        }
         // Liberación (Release)
    for (int i = 0; i < tR; i++)
    {
        Rs[i] = Mathf.Pow((1.0f - (i / (float)tR)), 1.5f); // Curva de liberación más pronunciada
    }


        //unir ADSs en un solo arreglo
        float[] result = new float[As.Length + Ds.Length + Ss.Length+ Rs.Length];

        As.CopyTo(result, 0);
        Ds.CopyTo(result, As.Length);
        Ss.CopyTo(result, As.Length + Ds.Length);
        Rs.CopyTo(result, As.Length + Ds.Length + Ss.Length);
        env = new float[result.Length];
        result.CopyTo(env, 0);
    }




}
