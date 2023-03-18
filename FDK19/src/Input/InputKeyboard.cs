using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using SlimDX;
using SlimDX.DirectInput;

namespace FDK
{
	public class InputKeyboard : IInputDevice, IDisposable
	{
		// コンストラクタ

		public InputKeyboard( IntPtr hWnd, DirectInput directInput )
		{
			this.NowInputDeviceType = InputTypes.Keyboard;
			this.GUID = "";
			this.ID = 0;
			try
			{
				this.DevKeyboard = new Keyboard( directInput );
				this.DevKeyboard.SetCooperativeLevel( hWnd, CooperativeLevel.NoWinKey | CooperativeLevel.Foreground | CooperativeLevel.Nonexclusive );
				this.DevKeyboard.Properties.BufferSize = _rawBufferedDataArray.Length;
				Trace.TraceInformation( this.DevKeyboard.Information.ProductName + " を生成しました。" );
			}
			catch( DirectInputException )
			{
				if( this.DevKeyboard != null )
				{
					this.DevKeyboard.Dispose();
					this.DevKeyboard = null;
				}
				Trace.TraceWarning( "Keyboard デバイスの生成に失敗しました。" );
				throw;
			}
			try
			{
				this.DevKeyboard.Acquire();
			}
			catch( DirectInputException e)
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "例外が発生しましたが処理を継続します。 (21a6c966-e27d-48f6-ac89-125ea4bc1a06)" );
			}

			for( int i = 0; i < this.KeyStates.Length; i++ )
				this.KeyStates[ i ] = false;

			//this.timer = new CTimer( CTimer.E種別.MultiMedia );
			this.InputEvents = new List<STInputEvent>( 32 );
			// this.ct = new CTimer( CTimer.E種別.PerformanceCounter );
		}


		// メソッド

		#region [ IInputDevice 実装 ]
		//-----------------
		public InputTypes NowInputDeviceType { get; private set; }
		public string GUID { get; private set; }
		public int ID { get; private set; }
		public List<STInputEvent> InputEvents { get; private set; }

		public void Polling( bool isActiveWindow, bool useBufferInput )
		{
			for ( int i = 0; i < 256; i++ )
			{
				this.KeyPushDowns[ i ] = false;
				this.KeyPullUps[ i ] = false;
			}

			if ( ( ( isActiveWindow && ( this.DevKeyboard != null ) ) && !this.DevKeyboard.Acquire().IsFailure ) && !this.DevKeyboard.Poll().IsFailure )
			{
				//this.list入力イベント = new List<STInputEvent>( 32 );
				this.InputEvents.Clear();			// #xxxxx 2012.6.11 yyagi; To optimize, I removed new();
				int posEnter = -1;
				//string d = DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss.ffff" );

				if ( useBufferInput )
				{
					#region [ a.バッファ入力 ]
					//-----------------------------

                    var length = this.DevKeyboard.GetDeviceData(_rawBufferedDataArray, false);
                    if (!Result.Last.IsSuccess)
                    {
                        return;
                    }
                    for (int i = 0; i < length; i++)
                    {
                        var rawBufferedData = _rawBufferedDataArray[i];
                        var key = DeviceConstantConverter.DIKtoKey(rawBufferedData.Offset);
                        var wasPressed = (rawBufferedData.Data & 128) == 128;

                        STInputEvent item = new STInputEvent()
                        {
                            PressedKeyIndex = (int) key,
                            IsPressed = wasPressed,
                            TimeStamp = SoundManager.PlayTimer.TimeStampToSoundTimeMs( rawBufferedData.Timestamp ),
                        };
                        this.InputEvents.Add( item );

                        this.KeyStates[ item.PressedKeyIndex ] = wasPressed;
                        this.KeyPushDowns[ item.PressedKeyIndex ] = wasPressed;
                        this.KeyPullUps[ item.PressedKeyIndex ] = !wasPressed;
                    }

					//-----------------------------
					#endregion
				}
				else
				{
					#region [ b.状態入力 ]
					//-----------------------------
					KeyboardState currentState = this.DevKeyboard.GetCurrentState();
					if ( Result.Last.IsSuccess && currentState != null )
					{
						foreach ( Key key in currentState.PressedKeys )
						{
							if ( this.KeyStates[ (int) key ] == false )
							{
								var ev = new STInputEvent()
								{
									PressedKeyIndex = (int) key,
									IsPressed = true,
									TimeStamp = SoundManager.PlayTimer.nシステム時刻,	// 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								};
								this.InputEvents.Add( ev );

								this.KeyStates[ (int) key ] = true;
								this.KeyPushDowns[ (int) key ] = true;

								//if ( (int) key == (int) SlimDX.DirectInput.Key.Space )
								//{
								//    Trace.TraceInformation( "FDK(direct): SPACE key registered. " + ct.nシステム時刻 );
								//}
							}
						}
						foreach ( Key key in currentState.ReleasedKeys )
						{
							if ( this.KeyStates[ (int) key ] == true )
							{
								var ev = new STInputEvent()
								{
									PressedKeyIndex = (int) key,
									IsPressed = false,
									TimeStamp = SoundManager.PlayTimer.nシステム時刻,	// 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								};
								this.InputEvents.Add( ev );

								this.KeyStates[ (int) key ] = false;
								this.KeyPullUps[ (int) key ] = true;
							}
						}
					}
					//-----------------------------
					#endregion
				}
				#region [#23708 2011.4.8 yyagi Altが押されているときは、Enter押下情報を削除する -> 副作用が見つかり削除]
				//if ( this.bKeyState[ (int) SlimDX.DirectInput.Key.RightAlt ] ||
				//     this.bKeyState[ (int) SlimDX.DirectInput.Key.LeftAlt ] )
				//{
				//    int cr = (int) SlimDX.DirectInput.Key.Return;
				//    this.bKeyPushDown[ cr ] = false;
				//    this.bKeyPullUp[ cr ] = false;
				//    this.bKeyState[ cr ] = false;
				//}
				#endregion
			}
		}
		public bool GetKeyPressed( int nKey )
		{
			return this.KeyPushDowns[ nKey ];
		}
		public bool GetKeyKeepPressed( int nKey )
		{
			return this.KeyStates[ nKey ];
		}
		public bool GetKeyReleased( int nKey )
		{
			return this.KeyPullUps[ nKey ];
		}
		public bool GetNoKeyPressed( int nKey )
		{
			return !this.KeyStates[ nKey ];
		}
		//-----------------
		#endregion

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if( !this.IsDisposed )
			{
				if( this.DevKeyboard != null )
				{
					this.DevKeyboard.Dispose();
					this.DevKeyboard = null;
				}
				//if( this.timer != null )
				//{
				//    this.timer.Dispose();
				//    this.timer = null;
				//}
				if ( this.InputEvents != null )
				{
					this.InputEvents = null;
				}
				this.IsDisposed = true;
			}
		}
		//-----------------
		#endregion


		// その他

		#region [ private ]
		//-----------------
	    private readonly RawBufferedData[] _rawBufferedDataArray = new RawBufferedData[256];
		private readonly bool[] KeyPullUps = new bool[256];
		private readonly bool[] KeyPushDowns = new bool[256];
		private readonly bool[] KeyStates = new bool[256];

	    private bool IsDisposed;
		private Keyboard DevKeyboard;

	    //private CTimer timer;
		//private CTimer ct;
		//-----------------
		#endregion
	}
}
