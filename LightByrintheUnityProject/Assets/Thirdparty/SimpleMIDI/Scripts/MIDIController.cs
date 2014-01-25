using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// Code by Eduardo Calcada
// Contact me at www.eduardocalcada.com

public class MIDIController : MonoBehaviour {
	
	// Creating new MIDIInput instance
	public MIDIInput midiInput = new MIDIInput();
	
	// Getting references to GUI labels
	public GUIText leftKey;
	public GUIText middleKey;
	public GUIText rightKey;
	
	// Creating a seperate thread
	private Thread midiThread;
	
	// Use this for initialization
	void Start () 
	{
		// Given a successful initialisation
		if(midiInput.Initialise())
		{
			// Create new thread for handling MIDI input with best perforamnce
			midiThread = new Thread(new ThreadStart(midiInput.run));
			midiThread.Start();
		}
		else
			Application.Quit();
		
		// Clearing GUI labels
		leftKey.text   = "";
		middleKey.text = "";
		rightKey.text  = "";
		
		// Prepare container for messages
		midiInput.keys.Clear();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// If there are three keys currently pressed
		if(midiInput.keys.Count > 2)
		{
			// Sort in ascending order
			midiInput.keys.Sort();
			
			// Display keys from left to right
			leftKey.text   = midiInput.keys[0].ToString();
			middleKey.text = midiInput.keys[1].ToString();
			rightKey.text  = midiInput.keys[2].ToString();
			
			// Clear container for next batch of keys
			midiInput.keys.Clear();
		}
	}
	
	void OnApplicationQuit()
	{
		// Stop thread loop and terminate it
		midiInput.done = true;
		midiInput.Terminate();
	}
}
