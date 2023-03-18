using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Mix;

namespace FDK
{
	internal interface ISoundDevice : IDisposable
	{
		SoundDeviceType NowSoundDevice { get; }
		int MasterVolume { get; set; }
		long ActualOutputDelay { get; }
		long ActualBufferSizeMs { get; }
		long ElapsedTimeMs { get; }
		long SystemTimeWithUpdatedElapsedTime { get; }
		FDKTimer SystemTimer { get; }

		FDKSound CreateFDKSound( string filePath, SoundGroup soundGroup );
		void CreateFDKSound( string filePath, FDKSound sound );
		void CreateFDKSound( byte[] wavFileImage, FDKSound sound );
	}
}
