using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.DirectInput;

namespace FDK
{
	public class InputManager : IDisposable
	{
		// プロパティ

		public List<IInputDevice> InputDevices 
		{
			get;
			private set;
		}
		public IInputDevice Keyboard
		{
			get
			{
				if( this._Keyboard != null )
				{
					return this._Keyboard;
				}
				foreach( IInputDevice device in this.InputDevices )
				{
					if( device.NowInputDeviceType == InputTypes.Keyboard )
					{
						this._Keyboard = device;
						return device;
					}
				}
				return null;
			}
		}
		public IInputDevice Mouse
		{
			get
			{
				if( this._Mouse != null )
				{
					return this._Mouse;
				}
				foreach( IInputDevice device in this.InputDevices )
				{
					if( device.NowInputDeviceType == InputTypes.Mouse )
					{
						this._Mouse = device;
						return device;
					}
				}
				return null;
			}
		}


		// コンストラクタ
		public InputManager( IntPtr hWnd )
		{
			Init( hWnd, true );
		}
		public InputManager( IntPtr hWnd, bool useMidiInput )
		{
			Init( hWnd, useMidiInput );
		}

		public void Init( IntPtr hWnd, bool useMidiInput )
		{
			this.directInput = new DirectInput();
			// this.timer = new CTimer( CTimer.E種別.MultiMedia );

			this.InputDevices = new List<IInputDevice>( 10 );
			this.InputDevices.Add( new InputKeyboard( hWnd, directInput ) );
			this.InputDevices.Add( new InputMouse( hWnd, directInput ) );
			foreach( DeviceInstance instance in this.directInput.GetDevices( DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly ) )
			{
				this.InputDevices.Add( new InputJoystick( hWnd, instance, directInput ) );
			}

			if ( useMidiInput )
			{
				this.proc = new Win32Manager.MidiInProc( this.MidiInCallback );
				uint nMidiDevices = Win32Manager.midiInGetNumDevs();
				Trace.TraceInformation( "MIDI入力デバイス数: {0}", nMidiDevices );
				for ( uint i = 0; i < nMidiDevices; i++ )
				{
					InputMIDI item = new InputMIDI( i );
					this.InputDevices.Add( item );
					Win32Manager.MIDIINCAPS lpMidiInCaps = new Win32Manager.MIDIINCAPS();
					uint num3 = Win32Manager.midiInGetDevCaps( i, ref lpMidiInCaps, (uint) Marshal.SizeOf( lpMidiInCaps ) );
					if ( num3 != 0 )
					{
						Trace.TraceError( "MIDI In: Device{0}: midiInDevCaps(): {1:X2}: ", i, num3 );
					}
					else if ( ( Win32Manager.midiInOpen( ref item.hMidiIn, i, this.proc, 0, 0x30000 ) == 0 ) && ( item.hMidiIn != 0 ) )
					{
						Win32Manager.midiInStart( item.hMidiIn );
						Trace.TraceInformation( "MIDI In: [{0}] \"{1}\" の入力受付を開始しました。", i, lpMidiInCaps.szPname );
					}
					else
					{
						Trace.TraceError( "MIDI In: [{0}] \"{1}\" の入力受付の開始に失敗しました。", i, lpMidiInCaps.szPname );
					}
				}
			}
			else
			{
				Trace.TraceInformation( "DTXVモードのため、MIDI入力は使用しません。" );
			}
		}
		
		
		// メソッド

		public IInputDevice Joystick( int ID )
		{
			foreach( IInputDevice device in this.InputDevices )
			{
				if( ( device.NowInputDeviceType == InputTypes.Joystick ) && ( device.ID == ID ) )
				{
					return device;
				}
			}
			return null;
		}
		public IInputDevice Joystick( string GUID )
		{
			foreach( IInputDevice device in this.InputDevices )
			{
				if( ( device.NowInputDeviceType == InputTypes.Joystick ) && device.GUID.Equals( GUID ) )
				{
					return device;
				}
			}
			return null;
		}
		public IInputDevice MidiIn( int ID )
		{
			foreach( IInputDevice device in this.InputDevices )
			{
				if( ( device.NowInputDeviceType == InputTypes.MidiIn ) && ( device.ID == ID ) )
				{
					return device;
				}
			}
			return null;
		}
		public void Polling( bool isActiveWindow, bool useBufferInput )
		{
			lock( this.MidiInputForExclusion )
			{
//				foreach( IInputDevice device in this.list入力デバイス )
				for (int i = this.InputDevices.Count - 1; i >= 0; i--)	// #24016 2011.1.6 yyagi: change not to use "foreach" to avoid InvalidOperation exception by Remove().
				{
					IInputDevice device = this.InputDevices[i];
					try
					{
						device.Polling(isActiveWindow, useBufferInput);
					}
					catch (DirectInputException e)							// #24016 2011.1.6 yyagi: catch exception for unplugging USB joystick, and remove the device object from the polling items.
					{
						Trace.TraceError( e.ToString() );
						this.InputDevices.Remove(device);
						device.Dispose();
						Trace.TraceError("tポーリング時に対象deviceが抜かれており例外発生。同deviceをポーリング対象からRemoveしました。");
					}
				}
			}
		}

		#region [ IDisposable＋α ]
		//-----------------
		public void Dispose()
		{
			this.Dispose( true );
		}
		public void Dispose( bool disposeManagedObjects )
		{
			if( !this.IsDisposed )
			{
				if( disposeManagedObjects )
				{
					foreach( IInputDevice device in this.InputDevices )
					{
						InputMIDI tmidi = device as InputMIDI;
						if( tmidi != null )
						{
							Win32Manager.midiInStop( tmidi.hMidiIn );
							Win32Manager.midiInReset( tmidi.hMidiIn );
							Win32Manager.midiInClose( tmidi.hMidiIn );
							Trace.TraceInformation( "MIDI In: [{0}] を停止しました。", new object[] { tmidi.ID } );
						}
					}
					foreach( IInputDevice device2 in this.InputDevices )
					{
						device2.Dispose();
					}
					lock( this.MidiInputForExclusion )
					{
						this.InputDevices.Clear();
					}

					this.directInput.Dispose();

					//if( this.timer != null )
					//{
					//    this.timer.Dispose();
					//    this.timer = null;
					//}
				}
				this.IsDisposed = true;
			}
		}
		~InputManager()
		{
			this.Dispose( false );
			GC.KeepAlive( this );
		}
		//-----------------
		#endregion


		// その他

		#region [ private ]
		//-----------------
		private DirectInput directInput;
		private IInputDevice _Keyboard;
		private IInputDevice _Mouse;
		private bool IsDisposed;
		private List<uint> MidiInputs = new List<uint>( 8 );
		private object MidiInputForExclusion = new object();
		private Win32Manager.MidiInProc proc;
//		private CTimer timer;

		private void MidiInCallback( uint hMidiIn, uint wMsg, int dwInstance, int dwParam1, int dwParam2 )
		{
			int p = dwParam1 & 0xF0;
			if( wMsg != Win32Manager.MIM_DATA || ( p != 0x80 && p != 0x90 ) )
				return;

            long time = SoundManager.PlayTimer.nシステム時刻;	// lock前に取得。演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。

			lock( this.MidiInputForExclusion )
			{
				if( ( this.InputDevices != null ) && ( this.InputDevices.Count != 0 ) )
				{
					foreach( IInputDevice device in this.InputDevices )
					{
						InputMIDI tmidi = device as InputMIDI;
						if( ( tmidi != null ) && ( tmidi.hMidiIn == hMidiIn ) )
						{
							tmidi.ReceiveOnlyMIDISignalsFromMessages( wMsg, dwInstance, dwParam1, dwParam2, time );
							break;
						}
					}
				}
			}
		}
		//-----------------
		#endregion
	}
}
