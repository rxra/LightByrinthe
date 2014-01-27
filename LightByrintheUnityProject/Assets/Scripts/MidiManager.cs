using UnityEngine;
using System.Collections;
using System.Threading;


public class MidiManager : MonoBehaviour {

#if UNITY_STANDALONE_WIN

	// Creating new MIDIInput instance
	private MIDIInput midiInput = new MIDIInput();
	// Creating a seperate thread
	private Thread midiThread;
	private bool _initialized = false;

	private static MidiManager s_Instance = null;

	public static MidiManager instance
	{
		get
		{
			return s_Instance;
		}
	}

	public bool initialized
	{
		get
		{
			return _initialized;
		}
	}

	public int devices
	{
		get
		{
			return _initialized?midiInput.devices:0;
		}
	}

	public bool GetKeyVelocity(int key, out int velocity)
	{
		lock (midiInput.lockThis)
		{
			if (midiInput.keys.Contains(key)==false) {
				velocity = 0;
				return false;
			}

			velocity = (int)midiInput.velocities[key];

			return true;
		}
	}

	void Awake()
	{
		s_Instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		// Given a successful initialisation
		if(midiInput.Initialise())
		{
			Debug.Log ("midi ok");
			// Create new thread for handling MIDI input with best perforamnce
			midiThread = new Thread(new ThreadStart(midiInput.run));
			midiThread.Start();
			_initialized = true;
		}
		else {
			_initialized = false;
			Debug.LogWarning ("failed to initialize midi");
			Application.Quit();
		}
		
		// Prepare container for messages
		midiInput.keys.Clear();
	}
	
	void OnApplicationQuit()
	{
		// Stop thread loop and terminate it
		midiInput.done = true;
		midiInput.Terminate();
		_initialized = false;
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		lock (midiInput.lockThis)
		{
			/*for (int i=0;i<midiInput.keys.Count;i++) {
				// Sort in ascending order
				//midiInput.keys.Sort();
				Debug.Log ("+ " + midiInput.keys[i] + " : " + midiInput.velocities[midiInput.keys[i]]);
			}*/

			// Clear container for next batch of keys
			midiInput.keys.Clear();
			midiInput.velocities.Clear();
		}
	}

	//public bool KeyValue(string str, out 

#endif
}

