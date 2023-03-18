using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FDK
{
	// 構造体

	[StructLayout( LayoutKind.Sequential )]
	public struct STInputEvent
	{
		public int PressedKeyIndex { get; set; }
		public bool IsPressed { get; set; }
		public long TimeStamp { get; set; }
	}
}
