/* using System;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
	private sealed class LogOutputDevice : IOutputDevice
	{
		public event EventHandler<MidiEventSentEventArgs> EventSent;

        public RCV rcv;
		public void PrepareForEventsSending()
		{
            LoadSynths();
		}

		public void SendEvent(MidiEvent midiEvent)
		{
            rcv.PlayEvents(midiEvent);
			Debug.Log($"Event sent: {midiEvent}");
		}

		public void Dispose()
		{
		}

        public void LoadSynths()
        {
            rcv = GameObject.Find("SoundManager").GetComponent<RCV>();
        }
	}

    private IOutputDevice _outputDevice;
    private Playback _playback;

    private string[] midiFilePaths;
    private int currentSongIndex = 0;

    private void Start()
    {
        midiFilePaths = new string[]
        {
            Application.dataPath + "/midis/InfinityWorld_Gameplay_fine.mid",
            Application.dataPath + "/midis/InfinityWorld_Menu.mid",
            Application.dataPath + "/midis/overworld.mid"
        };

        InitializeOutputDevice();
        var midiFile = CreateTestFile();
        InitializeFilePlayback(midiFile);
        StartPlayback();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePlayback();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            StopPlayback();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Releasing playback and device...");

        if (_playback != null)
        {
            _playback.NotesPlaybackStarted -= OnNotesPlaybackStarted;
            _playback.NotesPlaybackFinished -= OnNotesPlaybackFinished;
            _playback.Dispose();
        }

        if (_outputDevice != null)
            _outputDevice.Dispose();

        Debug.Log("Playback and device released.");
    }

    private void InitializeOutputDevice()
    {
        Debug.Log($"Initializing output device...");
        _outputDevice = new LogOutputDevice();
        Debug.Log($"Output device initialized.");
    }

    private MidiFile CreateTestFile()
    {
        Debug.Log("Creating test MIDI file...");

        var patternBuilder = new PatternBuilder()
            .SetNoteLength(MusicalTimeSpan.Eighth)
            .SetVelocity(SevenBitNumber.MaxValue)
            .ProgramChange(GeneralMidiProgram.Harpsichord);

        foreach (var noteNumber in SevenBitNumber.Values)
        {
            patternBuilder.Note(Melanchall.DryWetMidi.MusicTheory.Note.Get(noteNumber));
        }

        var midiFile = patternBuilder.Build().ToFile(TempoMap.Default);
        Debug.Log("Test MIDI file created.");

        return midiFile;
    }

    private void InitializeFilePlayback(MidiFile midiFile)
    {
        Debug.Log("Initializing playback...");

        _playback = midiFile.GetPlayback(_outputDevice);
        _playback.Loop = true;
        _playback.NotesPlaybackStarted += OnNotesPlaybackStarted;
        _playback.NotesPlaybackFinished += OnNotesPlaybackFinished;
       
        Debug.Log("Playback initialized.");
    }

    private void StartPlayback()
    {
        //Debug.Log("Starting playback...");
        //_playback.Start();
        Debug.Log("Starting playback...");
        if (_playback != null)
         {
            _playback.Start();
         }
         else
         {
            Debug.LogError("Playback is null. Ensure that the MIDI file is loaded.");
         }
    }

    private void TogglePlayback()
    {
        if (_playback.IsRunning)
        {
            StopPlayback();
        }
        else
        {
            currentSongIndex = (currentSongIndex + 1) % midiFilePaths.Length;
            //var midiFile = MidiFile.Read(midiFilePaths[currentSongIndex]);
            var midiFilePath = midiFilePaths[currentSongIndex];
            Debug.Log($"Loading MIDI file: {midiFilePath}");
            var midiFile = MidiFile.Read(midiFilePath);
            InitializeFilePlayback(midiFile);
            StartPlayback();
        }
        
    }

    private void StopPlayback()
    {
        Debug.Log("Stopping playback...");
        _playback.Stop();
    }

    private void OnNotesPlaybackFinished(object sender, NotesEventArgs e)
    {
        LogNotes("Notes finished:", e);
    }

    private void OnNotesPlaybackStarted(object sender, NotesEventArgs e)
    {
        LogNotes("Notes started:", e);
    }

    private void LogNotes(string title, NotesEventArgs e)
    {
        var message = new StringBuilder()
            .AppendLine(title)
            .AppendLine(string.Join(Environment.NewLine, e.Notes.Select(n => $"  {n}")))
            .ToString();
        Debug.Log(message.Trim());
    }
} */
using System;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DemoScript : MonoBehaviour
{
    private sealed class LogOutputDevice : IOutputDevice
    {
        public event EventHandler<MidiEventSentEventArgs> EventSent;

        public RCV rcv;
        public void PrepareForEventsSending()
        {
            LoadSynths();
        }

        public void SendEvent(MidiEvent midiEvent)
        {
            rcv.PlayEvents(midiEvent);
            Debug.Log($"Event sent: {midiEvent}");
        }

        public void Dispose()
        {
        }

        public void LoadSynths()
        {
            rcv = GameObject.Find("SoundManager").GetComponent<RCV>();
        }
    }

    private IOutputDevice _outputDevice;
    private Playback _playback;

    private string[] midiFilePaths;
    private int currentSongIndex = 0;

    private void Start()
    {
        // Obtén el índice actual de la canción desde PersistentManager
        int storedIndex = PersistentManager.Instance.GetCurrentSongIndex();

        midiFilePaths = new string[]
        {
            Application.dataPath + "/midis/InfinityWorld_Menu.mid",
            Application.dataPath +  "/midis/InfinityWorld_Gameplay_fine.mid"
        };

        InitializeOutputDevice();

        // Asegúrate de que el índice actual esté dentro de los límites
        currentSongIndex = Mathf.Clamp(storedIndex, 0, midiFilePaths.Length - 1);

        var midiFile = MidiFile.Read(midiFilePaths[currentSongIndex]); // Lee el archivo MIDI según el índice actual
        InitializeFilePlayback(midiFile);

        // Inicia la reproducción
        //StartPlayback();
                // Si no estás en la escena "Configuracion", inicia la reproducción
        if (SceneManager.GetActiveScene().name != "Configuracion")
        {
            StartPlayback();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Asegúrate de que el índice de la canción no se pierda al cargar una nueva escena
        if (scene.name != "Play" && scene.name != "Configuracion")
        {
            currentSongIndex = PersistentManager.Instance.GetCurrentSongIndex();
        }

        // Si la nueva escena no es "Configuracion", inicia la reproducción
        if (scene.name != "Configuracion")
        {
            StartPlayback();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePlayback();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            StopPlayback();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Releasing playback and device...");

        if (_playback != null)
        {
            _playback.NotesPlaybackStarted -= OnNotesPlaybackStarted;
            _playback.NotesPlaybackFinished -= OnNotesPlaybackFinished;
            _playback.Dispose();
        }

        if (_outputDevice != null)
            _outputDevice.Dispose();

        Debug.Log("Playback and device released.");
    }

    private void InitializeOutputDevice()
    {
        Debug.Log($"Initializing output device...");
        _outputDevice = new LogOutputDevice();
        Debug.Log($"Output device initialized.");
    }

    private void InitializeFilePlayback(MidiFile midiFile)
    {
        Debug.Log("Initializing playback...");

        _playback = midiFile.GetPlayback(_outputDevice);
        _playback.Loop = true;
        _playback.NotesPlaybackStarted += OnNotesPlaybackStarted;
        _playback.NotesPlaybackFinished += OnNotesPlaybackFinished;

        Debug.Log("Playback initialized.");
    }

    private void StartPlayback()
    {
        Debug.Log("Starting playback...");
        if (_playback != null)
        {
            _playback.Start();
        }
        else
        {
            Debug.LogError("Playback is null. Ensure that the MIDI file is loaded.");
        }
    }

private void TogglePlayback()
{
    if (_playback.IsRunning)
    {
        StopPlayback();
    }
    else
    {
        if (SceneManager.GetActiveScene().name == "Play") // Solo cambia de canción en la escena "Play"
        {
            currentSongIndex = (currentSongIndex + 1) % midiFilePaths.Length;
            PersistentManager.Instance.SetCurrentSongIndex(currentSongIndex);
        }

        var midiFilePath = midiFilePaths[currentSongIndex];
        Debug.Log($"Loading MIDI file: {midiFilePath}");
        var midiFile = MidiFile.Read(midiFilePath);
        InitializeFilePlayback(midiFile);
        StartPlayback();
    }
}




    private void StopPlayback()
    {
        Debug.Log("Stopping playback...");
        _playback.Stop();
    }

    private void OnNotesPlaybackFinished(object sender, NotesEventArgs e)
    {
        LogNotes("Notes finished:", e);
    }

    private void OnNotesPlaybackStarted(object sender, NotesEventArgs e)
    {
        LogNotes("Notes started:", e);
    }

    private void LogNotes(string title, NotesEventArgs e)
    {
        var message = new StringBuilder()
            .AppendLine(title)
            .AppendLine(string.Join(Environment.NewLine, e.Notes.Select(n => $"  {n}")))
            .ToString();
        Debug.Log(message.Trim());
    }
}
