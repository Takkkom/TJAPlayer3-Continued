using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace FDK
{
	public class InputMIDI : IInputDevice, IDisposable
	{
		// プロパティ

		public uint hMidiIn;
		public List<STInputEvent> listEventBuffer;


		// コンストラクタ

		public InputMIDI( uint nID )
		{
			this.hMidiIn = 0;
			this.listEventBuffer = new List<STInputEvent>( 32 );
			this.InputEvents = new List<STInputEvent>( 32 );
			this.NowInputDeviceType = InputTypes.MidiIn;
			this.GUID = "";
			this.ID = (int) nID;
		}


		// メソッド

		public void ReceiveOnlyMIDISignalsFromMessages( uint wMsg, int dwInstance, int dwParam1, int dwParam2, long n受信システム時刻 )
		{
			if( wMsg == Win32Manager.MIM_DATA )
			{
				int nMIDIevent = dwParam1 & 0xF0;
				int nPara1 = ( dwParam1 >> 8 ) & 0xFF;
				int nPara2 = ( dwParam1 >> 16 ) & 0xFF;

// Trace.TraceInformation( "MIDIevent={0:X2} para1={1:X2} para2={2:X2}", nMIDIevent, nPara1, nPara2 );
			
				if( ( nMIDIevent == 0x90 ) && ( nPara2 != 0 ) )
				{
					STInputEvent item = new STInputEvent();
					item.PressedKeyIndex = nPara1;
					item.IsPressed = true;
					item.TimeStamp = n受信システム時刻;
					this.listEventBuffer.Add( item );
				}
			}
		}

		#region [ IInputDevice 実装 ]
		//-----------------
		public InputTypes NowInputDeviceType { get; private set; }
		public string GUID { get; private set; }
		public int ID { get; private set; }
		public List<STInputEvent> InputEvents { get; private set; }

		public void Polling( bool bWindowがアクティブ中, bool bバッファ入力を使用する )
		{
			// this.list入力イベント = new List<STInputEvent>( 32 );
			this.InputEvents.Clear();								// #xxxxx 2012.6.11 yyagi; To optimize, I removed new();

			for( int i = 0; i < this.listEventBuffer.Count; i++ )
				this.InputEvents.Add( this.listEventBuffer[ i ] );

			this.listEventBuffer.Clear();
		}
		public bool GetKeyPressed( int nKey )
		{
			foreach( STInputEvent event2 in this.InputEvents )
			{
				if( ( event2.PressedKeyIndex == nKey ) && event2.IsPressed )
				{
					return true;
				}
			}
			return false;
		}
		public bool GetKeyKeepPressed( int nKey )
		{
			return false;
		}
		public bool GetKeyReleased( int nKey )
		{
			return false;
		}
		public bool GetNoKeyPressed( int nKey )
		{
			return false;
		}
		//-----------------
		#endregion

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if ( this.listEventBuffer != null )
			{
				this.listEventBuffer = null;
			}
			if ( this.InputEvents != null )
			{
				this.InputEvents = null;
			}
		}
		//-----------------
		#endregion
	}
}
