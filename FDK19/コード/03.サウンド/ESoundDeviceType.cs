using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	public enum ESoundDeviceType
	{
		DirectSound,
		ASIO,
		SharedWASAPI,
		ExclusiveWASAPI,
		Unknown,
	}
}
