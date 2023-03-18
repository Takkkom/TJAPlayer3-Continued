using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	public interface IInputDevice : IDisposable
	{
		// プロパティ

		InputTypes NowInputDeviceType
		{
			get;
		}
		string GUID 
		{
			get; 
		}
		int ID 
		{
			get;
		}
		List<STInputEvent> InputEvents
		{
			get;
		}


		// メソッドインターフェース

		void Polling( bool bWindowがアクティブ中, bool bバッファ入力を使用する );
		bool GetKeyPressed( int nKey );
		bool GetKeyKeepPressed( int nKey );
		bool GetKeyReleased( int nKey );
		bool GetNoKeyPressed( int nKey );
	}
}
