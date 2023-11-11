/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;

public class RCV : MonoBehaviour
{
    public WaveTableMIDI Osc0;
    public WaveTableMIDI Osc1;
    public WaveTableMIDI Osc2;
    public WaveTableMIDI Osc3;

    // Start is called before the first frame update
    void Start()
    {
        if (Osc0 != null) Osc0.Awake();
        if (Osc1 != null) Osc1.Awake();
        if (Osc2 != null) Osc2.Awake();
        if (Osc3 != null) Osc3.Awake();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private float MidiToFrequency(int midiNote)
    {
        float semitoneRatio = Mathf.Pow(2f, 1f / 12f);
        float semitoneOffset = midiNote - 69;

        return 440f * Mathf.Pow(semitoneRatio, semitoneOffset);
    }

    public void PlayEvents(MidiEvent midiEvent)
    {
        Debug.Log($"Received MIDI Event: {midiEvent}");
        float note;
        switch (midiEvent)
        {
            case NoteOnEvent noteOnEvent when noteOnEvent.Channel == 0:
                note = MidiToFrequency(noteOnEvent.NoteNumber);
                if (Osc0 != null)
  
                {
                    Osc0.ADSRindex = 0; // Asegúrate de que AudioGen1 tenga una propiedad ADSRindex
                    Osc0.frecuencia = note;
                }
                break;

            case NoteOnEvent noteOnEvent when noteOnEvent.Channel == 1:
                note = MidiToFrequency(noteOnEvent.NoteNumber);
                if (Osc1 != null)
      
                {
                    Osc1.ADSRindex = 1; // Asegúrate de que AudioGen1 tenga una propiedad ADSRindex
                    Osc1.frecuencia = note;
                }
                break;

            case NoteOnEvent noteOnEvent when noteOnEvent.Channel == 2:
                note = MidiToFrequency(noteOnEvent.NoteNumber);
                if (Osc2 != null)
                
                {
                    Osc2.ADSRindex = 2; // Asegúrate de que AudioGen1 tenga una propiedad ADSRindex
                    Osc2.frecuencia = note;
                }
                break;

            case NoteOnEvent noteOnEvent when noteOnEvent.Channel == 3:
                note = MidiToFrequency(noteOnEvent.NoteNumber);
                if (Osc3 != null)
                
                {
                    Osc3.ADSRindex = 3; // Asegúrate de que AudioGen1 tenga una propiedad ADSRindex
                    Osc3.frecuencia = note;
                }
                break;


            case NoteOffEvent noteOffEvent when noteOffEvent.Channel == 0:
                if (Osc0 != null)
                
                {
                    Osc0.frecuencia = 0;
                }
                break;

            case NoteOffEvent noteOffEvent when noteOffEvent.Channel == 1:
                if (Osc1 != null)
                
                {
                    Osc1.frecuencia = 0;
                }
                break;

            case NoteOffEvent noteOffEvent when noteOffEvent.Channel == 2:
                if (Osc2 != null)
                {
                    Osc2.frecuencia = 0;
                }
                break;

            case NoteOffEvent noteOffEvent when noteOffEvent.Channel == 3:
                if (Osc3 != null)
                {
                    Osc3.frecuencia = 0;
                }
                break;

            default:
                break;
        }
    }
}
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;

public class RCV : MonoBehaviour
{
    public List<WaveTableMIDI> Oscillators;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var oscillator in Oscillators)
        {
            if (oscillator != null) oscillator.Awake();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private float MidiToFrequency(int midiNote)
    {
        float semitoneRatio = Mathf.Pow(2f, 1f / 12f);
        float semitoneOffset = midiNote - 69;

        return 440f * Mathf.Pow(semitoneRatio, semitoneOffset);
    }

    public void PlayEvents(MidiEvent midiEvent)
    {
        Debug.Log($"Received MIDI Event: {midiEvent}");

        float note;

        switch (midiEvent)
        {
            case NoteOnEvent noteOnEvent when noteOnEvent.Channel >= 0 && noteOnEvent.Channel < Oscillators.Count:
                note = MidiToFrequency(noteOnEvent.NoteNumber);
                var oscillator = Oscillators[noteOnEvent.Channel];

                if (oscillator != null)
                {
                    oscillator.ADSRindex = noteOnEvent.Channel; // Puedes ajustar la asignación según tus necesidades
                    oscillator.frecuencia = note;
                }
                break;

            case NoteOffEvent noteOffEvent when noteOffEvent.Channel >= 0 && noteOffEvent.Channel < Oscillators.Count:
                var offOscillator = Oscillators[noteOffEvent.Channel];

                if (offOscillator != null)
                {
                    offOscillator.frecuencia = 0;
                }
                break;

            default:
                break;
        }
    }
}
