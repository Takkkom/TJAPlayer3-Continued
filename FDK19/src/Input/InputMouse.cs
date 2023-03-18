﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using SlimDX;
using SlimDX.DirectInput;

namespace FDK
{
	public class InputMouse : IInputDevice, IDisposable
	{
		// 定数

		public const int nマウスの最大ボタン数 = 8;


		// コンストラクタ

		public InputMouse( IntPtr hWnd, DirectInput directInput )
		{
			this.NowInputDeviceType = InputTypes.Mouse;
			this.GUID = "";
			this.ID = 0;
			try
			{
				this.devMouse = new Mouse( directInput );
				this.devMouse.SetCooperativeLevel( hWnd, CooperativeLevel.Foreground | CooperativeLevel.Nonexclusive );
				this.devMouse.Properties.BufferSize = _rawBufferedDataArray.Length;
				Trace.TraceInformation( this.devMouse.Information.ProductName + " を生成しました。" );
			}
			catch( DirectInputException )
			{
				if( this.devMouse != null )
				{
					this.devMouse.Dispose();
					this.devMouse = null;
				}
				Trace.TraceWarning( "Mouse デバイスの生成に失敗しました。" );
				throw;
			}
			try
			{
				this.devMouse.Acquire();
			}
			catch( DirectInputException e )
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "例外が発生しましたが処理を継続します。 (803ffb80-d2ca-425f-8f95-05d7c7cc8d90)" );
			}

			for( int i = 0; i < this.bMouseState.Length; i++ )
				this.bMouseState[ i ] = false;

			//this.timer = new CTimer( CTimer.E種別.MultiMedia );
			this.InputEvents = new List<STInputEvent>( 32 );
		}


		// メソッド

		#region [ IInputDevice 実装 ]
		//-----------------
		public InputTypes NowInputDeviceType { get; private set; }
		public string GUID { get; private set; }
		public int ID { get; private set; }
		public List<STInputEvent> InputEvents { get; private set; }

		public void Polling( bool bWindowがアクティブ中, bool bバッファ入力を使用する )
		{
			for( int i = 0; i < 8; i++ )
			{
				this.bMousePushDown[ i ] = false;
				this.bMousePullUp[ i ] = false;
			}

			if( ( ( bWindowがアクティブ中 && ( this.devMouse != null ) ) && !this.devMouse.Acquire().IsFailure ) && !this.devMouse.Poll().IsFailure )
			{
				// this.list入力イベント = new List<STInputEvent>( 32 );
				this.InputEvents.Clear();			// #xxxxx 2012.6.11 yyagi; To optimize, I removed new();

				if( bバッファ入力を使用する )
				{
					#region [ a.バッファ入力 ]
					//-----------------------------

                    var length = this.devMouse.GetDeviceData(_rawBufferedDataArray, false);
                    if (!Result.Last.IsSuccess)
                    {
	                    return;
                    }
                    for (int i = 0; i < length; i++)
                    {
                        var rawBufferedData = _rawBufferedDataArray[i];
                        var key = rawBufferedData.Offset - 12;
                        var wasPressed = (rawBufferedData.Data & 128) == 128;

                        if (!(-1 < key && key < 8))
                        {
                            continue;
                        }

                        STInputEvent item = new STInputEvent()
                        {
                            PressedKeyIndex = key,
                            IsPressed = wasPressed,
                            TimeStamp = SoundManager.PlayTimer.TimeStampToSoundTimeMs( rawBufferedData.Timestamp ),
                        };
                        this.InputEvents.Add( item );

                        this.bMouseState[ item.PressedKeyIndex ] = wasPressed;
                        this.bMousePushDown[ item.PressedKeyIndex ] = wasPressed;
                        this.bMousePullUp[ item.PressedKeyIndex ] = !wasPressed;
                    }

					//-----------------------------
					#endregion
				}
				else
				{
					#region [ b.状態入力 ]
					//-----------------------------
					MouseState currentState = this.devMouse.GetCurrentState();
					if( Result.Last.IsSuccess && currentState != null )
					{
						bool[] buttons = currentState.GetButtons();
						for( int j = 0; ( j < buttons.Length ) && ( j < 8 ); j++ )
						{
							if( this.bMouseState[ j ] == false && buttons[ j ] == true )
							{
								var ev = new STInputEvent() {
									PressedKeyIndex = j,
									IsPressed = true,
									TimeStamp = SoundManager.PlayTimer.nシステム時刻,	// 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								};
								this.InputEvents.Add( ev );

								this.bMouseState[ j ] = true;
								this.bMousePushDown[ j ] = true;
							}
							else if( this.bMouseState[ j ] == true && buttons[ j ] == false )
							{
								var ev = new STInputEvent() {
									PressedKeyIndex = j,
									IsPressed = false,
									TimeStamp = SoundManager.PlayTimer.nシステム時刻,	// 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								};
								this.InputEvents.Add( ev );

								this.bMouseState[ j ] = false;
								this.bMousePullUp[ j ] = true;
							}
						}
					}
					//-----------------------------
					#endregion
				}
			}
		}
		public bool GetKeyPressed( int nButton )
		{
			return ( ( ( 0 <= nButton ) && ( nButton < 8 ) ) && this.bMousePushDown[ nButton ] );
		}
		public bool GetKeyKeepPressed( int nButton )
		{
			return ( ( ( 0 <= nButton ) && ( nButton < 8 ) ) && this.bMouseState[ nButton ] );
		}
		public bool GetKeyReleased( int nButton )
		{
			return ( ( ( 0 <= nButton ) && ( nButton < 8 ) ) && this.bMousePullUp[ nButton ] );
		}
		public bool GetNoKeyPressed( int nButton )
		{
			return ( ( ( 0 <= nButton ) && ( nButton < 8 ) ) && !this.bMouseState[ nButton ] );
		}
		//-----------------
		#endregion

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if( !this.bDispose完了済み )
			{
				if( this.devMouse != null )
				{
					this.devMouse.Dispose();
					this.devMouse = null;
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
				this.bDispose完了済み = true;
			}
		}
		//-----------------
		#endregion


		// その他

		#region [ private ]
		//-----------------
	    private readonly RawBufferedData[] _rawBufferedDataArray = new RawBufferedData[256];
        private readonly bool[] bMousePullUp = new bool[ 8 ];
		private readonly bool[] bMousePushDown = new bool[ 8 ];
		private readonly bool[] bMouseState = new bool[ 8 ];

	    private bool bDispose完了済み;
		private Mouse devMouse;

		//private CTimer timer;
		//-----------------
		#endregion
	}
}
