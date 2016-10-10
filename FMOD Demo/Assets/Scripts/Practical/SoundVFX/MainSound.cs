﻿/*===============================================================================================
|   Project:		            FMOD Demo                                                       |
|   Developer:	                Cameron Baron                                                   |
|   Company:		            Firelight Technologies                                          |
|   Date:		                01/08/2016                                                      |
|   Scene:                      Sound VFX                                                       |
|   Fmod Related Scripting:     Yes                                                             |
|   Description:                A demonstration of how to get the fft data from a studio event  |
|	by adding a dsp via code to the channel group. Then storing and updating the data for use	|
|	in other objects.																			|
|	Also, using the beat callback/detection to control other obects.							|
===============================================================================================*/

using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class MainSound : MonoBehaviour
{
	// Public Vars
	public float[] m_fftArray;

	[HideInInspector]
	public Texture2D m_soundTex;

	// Private Vars
	[FMODUnity.EventRef]	public string m_eventRef;
	FMOD.Studio.EventInstance m_eventInstance;
	
	FMOD.ChannelGroup m_channelGroup;
	FMOD.DSP m_fftDsp;

	#region FMOD Beat Callback/Detection	
    /*===============================================Fmod====================================================
    |     		This script demonstrates how to use timeline markers in your game code. 					|
	|																										|
	|			Timeline markers can be implicit - such as beats and bars. Or they 							|
	|			can be explicity placed by sound designers, in which case they have 						|
	|			a sound designer specified name attached to them.											|
	|																										|
	|			Timeline markers can be useful for syncing game events to sound	events. 					|
    =======================================================================================================*/
	
    /*===============================================Fmod====================================================
    |      		Variables that are modified in the callback need to be part of a seperate class.       		|
    |      		   This class needs to be 'blittable' otherwise it can't be pinned in memory.        		|
    =======================================================================================================*/
	[StructLayout(LayoutKind.Sequential)]
	class TimelineInfo
	{
		public int currentMusicBar = 0;
		public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
	}

	FMOD.Studio.EVENT_CALLBACK m_beatCallback;
	TimelineInfo m_timelineInfo;
	GCHandle m_timelineHandle;

#endregion

	bool m_isPlaying = false;
	bool m_dspAdded = false;

	int WINDOWSIZE = 1024;

	/*===============================================Fmod====================================================
    |			      		Used for error checking a large number of FMOD functions.		      			|
    =======================================================================================================*/
	FMOD.RESULT result;              

	void Awake()
	{
		m_fftArray = new float[WINDOWSIZE];
		m_soundTex = new Texture2D(WINDOWSIZE, 1, TextureFormat.RGB24, false);
		m_soundTex.name = "Image";
		m_soundTex.wrapMode = TextureWrapMode.Clamp;

		/*===============================================Fmod====================================================
    	|						      		Create an instance of the event.       								|
    	=======================================================================================================*/
		m_eventInstance = FMODUnity.RuntimeManager.CreateInstance(m_eventRef);

		/*===============================================Fmod====================================================
    	|	Explicitly create the delegate object and assign it to a member so it doesn't get freed by the		|
		|								garbage collector while it's being used.								|
    	=======================================================================================================*/
		m_beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
		
		m_timelineInfo = new TimelineInfo();
		// Pin the class that will store the data modified during the callback.
		m_timelineHandle = GCHandle.Alloc(m_timelineInfo, GCHandleType.Pinned);
		// Pass the object through the userdata of the instance.
		result = m_eventInstance.setUserData(GCHandle.ToIntPtr(m_timelineHandle));
		// Assign the callback to the studio event.
		result = m_eventInstance.setCallback(m_beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

		/*===============================================Fmod====================================================
    	|				The dsp has to be added after the sound is playing! Otherwise errors!					|
    	=======================================================================================================*/
		PlaySound();
		StartCoroutine(AddDspToChannel());
	}

	void OnDestroy()
	{
		m_eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		m_eventInstance.release();
		m_timelineHandle.Free();
	}

	void Update()
	{
		/*===============================================Fmod====================================================
    	|	Check to make sure the channel group is both assigned and playing otherwise trying to get			|
		|				information from a the DSP effect will not work and give us errors.						|
    	=======================================================================================================*/
		if (m_channelGroup != null)
			m_channelGroup.isPlaying(out m_isPlaying);
		if (m_isPlaying && m_dspAdded)
		{
			/*===============================================Fmod====================================================
    		|	The data stored in the DSP is stored as an unmanaged block of data, we can Marshal from an IntPtr	|
			|						into a predifined struct to hold all the data we need.       					|
   	 		=======================================================================================================*/
			IntPtr unmanagedData;
			uint length;

			result = m_fftDsp.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
			FMOD.DSP_PARAMETER_FFT m_fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));

			/*===============================================Fmod====================================================
    		|	If the number of channels in the data is less than one, either there is nothing playing or    		|
			|										something went wrong!
    		=======================================================================================================*/
            if (m_fftData.numchannels < 1)
				return;

			/*===============================================Fmod====================================================
    		|      		For stereo sounds the will be 2 channels (left & right) with a default of 2048 "bins". 		|
    		=======================================================================================================*/            
            for (int bin = 0; bin < WINDOWSIZE; bin++)
			{
				float temp = lin2DB(m_fftData.spectrum[0][bin]);
				temp = ((temp + 80.0f) * (1 / 80.0f));
				m_fftArray[bin] = Mathf.Lerp(m_fftArray[bin], temp, 0.6f);
				m_soundTex.SetPixel(bin, 1, new Color(m_fftArray[bin], m_fftArray[bin], m_fftArray[bin]));
			}
			m_soundTex.Apply();
            
            float newValue = m_fftArray[1] * 10;
        }
	}

	#region Private Functions

	bool PlaySound()
	{
		/*===============================================Fmod====================================================
    	|      					Start the event playing and set the position to this gameobject.        				|
    	=======================================================================================================*/
		m_eventInstance.start();
        m_eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
		return true;
	}

	float lin2DB(float linear)
	{
		return (Mathf.Clamp(Mathf.Log10(linear) * 20, -80.0f, 0.0f));
	}

	IEnumerator AddDspToChannel()
	{
		/*===============================================Fmod====================================================
    	|	Before adding a DSP effect to the channel group, we need to make sure the event is playing,    		|
		|				otherwise we get an error because the channel isn't set correctly yet.					|
    	=======================================================================================================*/
		while (m_channelGroup == null)
		{
			m_eventInstance.getChannelGroup(out m_channelGroup);
			yield return null;
		}

		/*===============================================Fmod====================================================
    	|      		Here we create the FFt DSP effect and tell it how much data we want to store.        		|
    	=======================================================================================================*/
		FMODUnity.RuntimeManager.LowlevelSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out m_fftDsp);
		m_fftDsp.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.RECT);
		m_fftDsp.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, WINDOWSIZE * 2);

		/*===============================================Fmod====================================================
	    |	Now we add it to the channel group and set the bool to say it has been added, so that it doesn't    |
		|											get added again.											|
    	=======================================================================================================*/
		m_channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.TAIL, m_fftDsp);
		m_dspAdded = true;
	}

	void OnGUI()
	{
		GUILayout.Box(String.Format("Current Bar = {0}, Last Marker = {1}", m_timelineInfo.currentMusicBar, (string)m_timelineInfo.lastMarker));
	}
	
	[AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
	static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
	{		
		/*===============================================Fmod====================================================
    	|						      		Recreate the event instance.        								|
    	=======================================================================================================*/
		FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

		// Retrieve the user data
		IntPtr timelineInfoPtr;
		instance.getUserData(out timelineInfoPtr);

		// Get the object to store beat and marker details
		GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
		TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

		switch (type)
		{
			case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
				{
					var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
					timelineInfo.currentMusicBar = parameter.bar;
				}
				break;
			case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
				{
					var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
					timelineInfo.lastMarker = parameter.name;
				}
				break;
		}
		return FMOD.RESULT.OK;
	}

	#endregion
}
