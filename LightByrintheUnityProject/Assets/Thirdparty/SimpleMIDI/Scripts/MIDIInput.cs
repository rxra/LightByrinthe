using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

// Code by Eduardo Calcada
// Contact me at www.eduardocalcada.com

public class MIDIInput
{
	int keyNumber = 0;
	int keyVelocity = 0;
	
	public bool done = false;

	public System.Object lockThis = new System.Object();

	// Holds most recently pressed keys
	public List<int> keys = new List<int>();
	public Hashtable velocities = new Hashtable();
	
	// Plugin function imports
	[DllImport ("MIDIPlugin")]
	 private static extern int GetMIDIDevices(); 
	
	[DllImport ("MIDIPlugin")]
	 private static extern bool SelectMIDIDevice(int id); 
	
	[DllImport ("MIDIPlugin")]
	 private static extern bool OpenMIDIPort(); 
	
	[DllImport ("MIDIPlugin")]
	 private static extern void StartListenMIDI(); 
	
	[DllImport ("MIDIPlugin")]
	 private static extern int GetMIDIMessageVelocity();
	
	[DllImport ("MIDIPlugin")]
	 private static extern int GetMIDIMessageKey();
	
	[DllImport ("MIDIPlugin")]
	private static extern bool CloseMIDI();
	
	// Use this for initialization
	public bool Initialise () 
	{
		// Clearing the key container
		keys.Clear();
		
		Debug.Log ("There are these many input devices: " + GetMIDIDevices().ToString());
		
		// Selecting the MIDI Keyboard (0 since it is the only MIDI Device connected)
		if(!SelectMIDIDevice(0))
			return false;
		
		// Setting up the MIDI connection
		if(!OpenMIDIPort())
			return false;
		
		// Now listening for any incoming MIDI Messages
		StartListenMIDI();
		
		return true;
	}
	
	public void Terminate()
	{
		// Clean up
		CloseMIDI ();
	}
	
	// Update is called once per frame
	public void run () 
	{
		// Loop until thread isn't finished
		while(!done)
		{
			lock (lockThis)
			{
				// Storing the most recent MIDI messages
				keyNumber   = GetMIDIMessageKey();
				keyVelocity = GetMIDIMessageVelocity();
			
				// If key is already being pressed
				if(keys.Contains(keyNumber))
				{
					// Key has been unpressed
					if(keyVelocity < 1)
					{
						// Key no longer needed, remove
						keys.Remove(keyNumber);
						velocities.Remove(keyNumber);
						continue;
					}
					else
					{
						//Debug.Log ("set velocity ( " + keyVelocity + ") for : " + keyNumber);
						velocities[keyNumber] = keyVelocity;
						continue;
					}
				}
				else
				{
					// If our Key container isn't full (less than 3 keys pressed)
					if(keys.Count < 3)
					{
						// If a key IS being pressed
						if(keyVelocity > 1)
						{
							// Add it to the container
							Debug.Log ("Key " + keyNumber + " pressed with velocity: " + keyVelocity);
							keys.Add(keyNumber);
							velocities.Add (keyNumber,keyVelocity);
							//Debug.Log ("add velocity ( " + keyVelocity + ") for : " + keyNumber);
						}
					}
				}
			}
		}
	}
}
