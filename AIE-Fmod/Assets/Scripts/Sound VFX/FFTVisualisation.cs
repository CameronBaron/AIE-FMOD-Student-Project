﻿using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class FFTVisualisation : MonoBehaviour
{
	[FMODUnity.EventRef]
		public string musicString;
	FMOD.Studio.EventInstance instance;
	FMOD.DSP dsp_fft;

	Material material;
	public RenderTexture texture;

	void Start ()
	{
		material = GetComponent<Material>();
		instance = FMODUnity.RuntimeManager.CreateInstance(musicString);
		FMODUnity.RuntimeManager.LowlevelSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out dsp_fft);
		
		instance.start();
	}

	const float WIDTH = 10.0f;
	const float HEIGHT = 0.1f;

	void Update ()
	{
		FMOD.Studio.PLAYBACK_STATE playBackState;
		instance.getPlaybackState(out playBackState);
		if (playBackState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
		{
			// The data returned from the FFT is an unmanaged block of memory
			IntPtr unmanagedData;
			uint length;
			dsp_fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
			// Which stores the spectrum data of each channel in an array array. (eg. spectrum[channel][bin])
			FMOD.DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
			float[][] spectrum = fftData.spectrum;

			texture = new RenderTexture((int)length, 1, 0);
			texture.name = "Render Texture";
			
			//material.setFloat
		}
	}
}
