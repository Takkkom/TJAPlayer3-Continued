using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	public enum SoundDeviceType
	{
		DirectSound,
		ASIO,
		SharedWASAPI,
		ExclusiveWASAPI,
		Unknown,
	}
}
