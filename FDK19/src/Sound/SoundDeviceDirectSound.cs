using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SlimDX;
using SlimDX.DirectSound;

namespace FDK
{
	internal class SoundDeviceDirectSound : ISoundDevice
	{
		// プロパティ

		public SoundDeviceType NowSoundDevice
		{
			get;
			protected set;
		}
		public long ActualOutputDelay
		{
			get;
			protected set;
		}
		public long ActualBufferSizeMs
		{
			get;
			protected set;
		}

		public static readonly BufferFlags DefaultFlags = BufferFlags.Defer | BufferFlags.GetCurrentPosition2 | BufferFlags.GlobalFocus | BufferFlags.ControlVolume | BufferFlags.ControlPan | BufferFlags.ControlFrequency;

		// CSoundTimer 用に公開しているプロパティ

		public long ElapsedTimeMs
		{
			get
			{
				if ( ctimer != null )
				{
					int nowPosition = this.FDKSoundForElapsedTimeMeasurement.DirectSoundBuffer.CurrentPlayPosition;
					long nowSystemTimeMs = this.SystemTimer.SystemTimeMs;


					// ループ回数を調整。

					long systemTimeIntervalMs = nowSystemTimeMs - this.SystemTimeMsWithElapsedTimeMeasuredBefore;

					while ( systemTimeIntervalMs >= UnitAdvanceIntervalMs )		// 前回から単位繰り上げ間隔以上経過してるなら確実にループしている。誤差は大きくないだろうから無視。
					{
						this.LoopCount++;
						systemTimeIntervalMs -= UnitAdvanceIntervalMs;
					}

					if ( nowPosition < this.PrevPosition )							// 単位繰り上げ間隔以内であっても、現在位置が前回より手前にあるなら1回ループしている。
						this.LoopCount++;


					// 経過時間を算出。

					long elapsedTMsime = (long) ( ( this.LoopCount * UnitAdvanceIntervalMs ) + ( nowPosition * 1000.0 / ( 44100.0 * 2 * 2 ) ) );


					// 今回の値を次回に向けて保存。

					this.SystemTimeMsWithElapsedTimeMeasuredBefore = nowSystemTimeMs;
					this.PrevPosition = nowPosition;

					return elapsedTMsime;
				}
				else
				{
					long nRet = ctimer.SystemTimeMs - this.SystemTimeMsWithElapsedTimeMeasuredBefore;
					if ( nRet < 0 )	// カウンタがループしたときは
					{
						nRet = ( ctimer.nシステム時刻 - long.MinValue ) + ( long.MaxValue - this.SystemTimeMsWithElapsedTimeMeasuredBefore ) + 1;
					}
					this.SystemTimeMsWithElapsedTimeMeasuredBefore = ctimer.SystemTimeMs;
	
					return nRet;
				}
			}
		}
		public long SystemTimeWithUpdatedElapsedTime
		{
			get { throw new NotImplementedException(); }
		}
		public FDKTimer SystemTimer
		{
			get;
			protected set;
		}

		public int MasterVolume
		{
			get
			{
				return (int) 100;
			}
			set
			{
				// 特に何もしない
			}
		}


		// メソッド

		public SoundDeviceDirectSound( IntPtr hWnd, long delatTimeMs, bool bUseOSTimer )
		{
			Trace.TraceInformation( "DirectSound の初期化を開始します。" );

			this.NowSoundDevice = SoundDeviceType.Unknown;
			this.ActualBufferSizeMs = this.ActualOutputDelay = delatTimeMs;
			this.SystemTimer = new FDKTimer( FDKTimer.TimerType.MultiMedia );

			#region [ DirectSound デバイスを作成する。]
			//-----------------
			this.DirectSound = new DirectSound();	// 失敗したら例外をそのまま発出。

			// デバイスの協調レベルを設定する。

			bool priority = true;
			try
			{
				this.DirectSound.SetCooperativeLevel( hWnd, CooperativeLevel.Priority );
			}
			catch( DirectSoundException )
			{
				this.DirectSound.SetCooperativeLevel( hWnd, CooperativeLevel.Normal );	// これでも失敗したら例外をそのまま発出。
				priority = false;
			}

			// デバイス作成完了。

			this.NowSoundDevice = SoundDeviceType.DirectSound;
			//-----------------
			#endregion

			if ( !bUseOSTimer )
			{
				#region [ 経過時間計測用サウンドバッファを作成し、ループ再生を開始する。]
				//-----------------

				// 単位繰り上げ間隔[秒]の長さを持つ無音のサウンドを作成。

				uint dataSizeByte = UnitAdvanceInterval * 44100 * 2 * 2;
				var ms = new MemoryStream();
				var bw = new BinaryWriter( ms );
				bw.Write( (uint) 0x46464952 );						// 'RIFF'
				bw.Write( (uint) ( 44 + dataSizeByte - 8 ) );	// ファイルサイズ - 8
				bw.Write( (uint) 0x45564157 );						// 'WAVE'
				bw.Write( (uint) 0x20746d66 );						// 'fmt '
				bw.Write( (uint) 16 );								// バイト数
				bw.Write( (ushort) 1 );								// フォーマットID(リニアPCM)
				bw.Write( (ushort) 2 );								// チャンネル数
				bw.Write( (uint) 44100 );							// サンプリング周波数
				bw.Write( (uint) ( 44100 * 2 * 2 ) );				// bytes/sec
				bw.Write( (ushort) ( 2 * 2 ) );						// blockサイズ
				bw.Write( (ushort) 16 );							// bit/sample
				bw.Write( (uint) 0x61746164 );						// 'data'
				bw.Write( (uint) dataSizeByte );				// データ長
				for ( int i = 0; i < dataSizeByte / sizeof( long ); i++ )	// PCMデータ
					bw.Write( (long) 0 );
				var byArrWaveFleImage = ms.ToArray();
				bw.Close();
				ms = null;
				bw = null;
				this.FDKSoundForElapsedTimeMeasurement = this.CreateFDKSound( byArrWaveFleImage, SoundGroup.Unknown );

				FDKSound.FDKSounds.Remove( this.FDKSoundForElapsedTimeMeasurement );	// 特殊用途なのでインスタンスリストからは除外する。

				// サウンドのループ再生開始。

				this.LoopCount = 0;
				this.PrevPosition = 0;
				this.FDKSoundForElapsedTimeMeasurement.DirectSoundBuffer.Play( 0, PlayFlags.Looping );
				this.SystemTimeMsWithElapsedTimeMeasuredBefore = this.SystemTimer.SystemTimeMs;
				//-----------------
				#endregion
			}
			else
			{
				ctimer = new FDKTimer( FDKTimer.TimerType.MultiMedia );
			}
			Trace.TraceInformation( "DirectSound を初期化しました。({0})({1})", ( priority ) ? "Priority" : "Normal", bUseOSTimer? "OStimer" : "FDKtimer" );
		}

		public FDKSound CreateFDKSound( string filePath, SoundGroup soundGroup )
		{
			var sound = new FDKSound(soundGroup);
			sound.CreateDirectSoundInstance( filePath, this.DirectSound );
			return sound;
		}

		private FDKSound CreateFDKSound( byte[] wavFileImage, SoundGroup soundGroup )
		{
			var sound = new FDKSound(soundGroup);
			sound.CreateDirectSoundInstance( wavFileImage, this.DirectSound );
			return sound;
		}

		// 既存のインスタンス（生成直後 or Dispose済み）に対してサウンドを生成する。
		public void CreateFDKSound( string filePath, FDKSound sound )
		{
			sound.CreateDirectSoundInstance( filePath, this.DirectSound );
		}
		public void CreateFDKSound( byte[] wavFileImage, FDKSound sound )
		{
			sound.CreateDirectSoundInstance( wavFileImage, this.DirectSound );
		}
		public void CreateFDKSound( byte[] wavFileImage, BufferFlags flags, FDKSound sound )
		{
			sound.CreateDirectSoundInstance( wavFileImage, this.DirectSound, flags );
		}

		#region [ Dispose-Finallizeパターン実装 ]
		//-----------------
		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}
		protected void Dispose( bool bManagedDispose )
		{
			this.NowSoundDevice = SoundDeviceType.Unknown;		// まず出力停止する(Dispose中にクラス内にアクセスされることを防ぐ)
			if ( bManagedDispose )
			{
				#region [ 経緯時間計測用サウンドバッファを解放。]
				//-----------------
				if ( this.FDKSoundForElapsedTimeMeasurement != null )
				{
					this.FDKSoundForElapsedTimeMeasurement.StopFDKSound();
					FDKCommon.Dispose( ref this.FDKSoundForElapsedTimeMeasurement );
				}
				//-----------------
				#endregion
				#region [ 単位繰り上げ用スレッド停止。]
				//-----------------
				if( this.ThreadForElapsedTimeMeasurement != null )
				{
					this.ThreadForElapsedTimeMeasurement.Abort();
					this.ThreadForElapsedTimeMeasurement = null;
				
				}
				//-----------------
				#endregion

				FDKCommon.Dispose( ref this.DirectSound );
				FDKCommon.Dispose( this.SystemTimer );
			}
			if ( ctimer != null )
			{
				FDKCommon.Dispose( ref this.ctimer );
			}
		}
		~SoundDeviceDirectSound()
		{
			this.Dispose( false );
		}
		//-----------------
		#endregion

		protected DirectSound DirectSound = null;
		protected FDKSound FDKSoundForElapsedTimeMeasurement = null;
		protected Thread ThreadForElapsedTimeMeasurement = null;
//		protected AutoResetEvent autoResetEvent = new AutoResetEvent( false );
		protected const uint UnitAdvanceInterval = 1;	// [秒]
		protected const uint UnitAdvanceIntervalMs = UnitAdvanceInterval * 1000;	// [ミリ秒]
		protected int LoopCount = 0;

		private long SystemTimeMsWithElapsedTimeMeasuredBefore = FDKTimer.Unused;
		private int PrevPosition = 0;

		private FDKTimer ctimer = null;
	}
}
