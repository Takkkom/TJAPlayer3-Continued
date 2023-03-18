using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.Bass.AddOn.Mix;

namespace FDK
{
	/// <summary>
	/// 全ASIOデバイスを列挙する静的クラス。
	/// BASS_Init()やBASS_ASIO_Init()の状態とは無関係に使用可能。
	/// </summary>
	public static class CEnumerateAllAsioDevices
	{
		public static string[] GetAllASIODevices()
		{
			//Debug.WriteLine( "BassAsio.BASS_ASIO_GetDeviceInfos():" );
			BASS_ASIO_DEVICEINFO[] bassAsioDevInfo = BassAsio.BASS_ASIO_GetDeviceInfos();

			List<string> asioDeviceList = new List<string>();

			if ( bassAsioDevInfo.Length == 0 )
			{
				asioDeviceList.Add( "None" );
			}
			else
			{
				for ( int i = 0; i < bassAsioDevInfo.Length; i++ )
				{
					asioDeviceList.Add( bassAsioDevInfo[ i ].name );
					//Trace.TraceInformation( "ASIO Device {0}: {1}", i, bassAsioDevInfo[ i ].name );
				}
			}

			return asioDeviceList.ToArray();
		}
	}

	internal class SoundDeviceASIO : ISoundDevice
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
		public int ASIODeviceIndex
		{
			get;
			set;
		}

		// CSoundTimer 用に公開しているプロパティ

		public long ElapsedTimeMs
		{
			get;
			protected set;
		}
		public long SystemTimeWithUpdatedElapsedTime
		{
			get;
			protected set;
		}
		public FDKTimer SystemTimer
		{
			get;
			protected set;
		}


		// マスターボリュームの制御コードは、WASAPI/ASIOで全く同じ。
		public int MasterVolume
		{
			get
			{
				float f音量 = 0.0f;
				bool b = Bass.BASS_ChannelGetAttribute( this.MixerStreamHandle, BASSAttribute.BASS_ATTRIB_VOL, ref f音量 );
				if ( !b )
				{
					BASSError be = Bass.BASS_ErrorGetCode();
					Trace.TraceInformation( "ASIO Master Volume Get Error: " + be.ToString() );
				}
				else
				{
					//Trace.TraceInformation( "ASIO Master Volume Get Success: " + (f音量 * 100) );

				}
				return (int) ( f音量 * 100 );
			}
			set
			{
				bool b = Bass.BASS_ChannelSetAttribute( this.MixerStreamHandle, BASSAttribute.BASS_ATTRIB_VOL, (float) ( value / 100.0 ) );
				if ( !b )
				{
					BASSError be = Bass.BASS_ErrorGetCode();
					Trace.TraceInformation( "ASIO Master Volume Set Error: " + be.ToString() );
				}
				else
				{
					// int n = this.nMasterVolume;	
					// Trace.TraceInformation( "ASIO Master Volume Set Success: " + value );
				}
			}
		}

		// メソッド

		public SoundDeviceASIO( long bufferSizeMs, int asioDevice )
		{
			// 初期化。

			Trace.TraceInformation( "BASS (ASIO) の初期化を開始します。" );
			this.NowSoundDevice = SoundDeviceType.Unknown;
			this.ActualOutputDelay = 0;
			this.ElapsedTimeMs = 0;
			this.SystemTimeWithUpdatedElapsedTime = FDKTimer.Unused;
			this.SystemTimer = new FDKTimer( FDKTimer.TimerType.MultiMedia );
			this.ASIODeviceIndex = asioDevice;

			#region [ BASS registration ]
			// BASS.NET ユーザ登録（BASSスプラッシュが非表示になる）。
			BassNet.Registration( "dtx2013@gmail.com", "2X9181017152222" );
			#endregion

			#region [ BASS Version Check ]
			// BASS のバージョンチェック。
			int nBASSVersion = Utils.HighWord( Bass.BASS_GetVersion() );
			if( nBASSVersion != Bass.BASSVERSION )
				throw new DllNotFoundException( string.Format( "bass.dll のバージョンが異なります({0})。このプログラムはバージョン{1}で動作します。", nBASSVersion, Bass.BASSVERSION ) );

			int nBASSMixVersion = Utils.HighWord( BassMix.BASS_Mixer_GetVersion() );
			if( nBASSMixVersion != BassMix.BASSMIXVERSION )
				throw new DllNotFoundException( string.Format( "bassmix.dll のバージョンが異なります({0})。このプログラムはバージョン{1}で動作します。", nBASSMixVersion, BassMix.BASSMIXVERSION ) );

			int nBASSASIO = Utils.HighWord( BassAsio.BASS_ASIO_GetVersion() );
			if( nBASSASIO != BassAsio.BASSASIOVERSION )
				throw new DllNotFoundException( string.Format( "bassasio.dll のバージョンが異なります({0})。このプログラムはバージョン{1}で動作します。", nBASSASIO, BassAsio.BASSASIOVERSION ) );
			#endregion

			// BASS の設定。

			this.IsBASSFree = true;

		    if (!Bass.BASS_SetConfig( BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0 )) // 0:BASSストリームの自動更新を行わない。
		    {
		        Trace.TraceWarning($"BASS_SetConfig({nameof(BASSConfig.BASS_CONFIG_UPDATEPERIOD)}) に失敗しました。[{Bass.BASS_ErrorGetCode()}]");
		    }
		    if (!Bass.BASS_SetConfig( BASSConfig.BASS_CONFIG_UPDATETHREADS, 0 )) // 0:BASSストリームの自動更新を行わない。
		    {
		        Trace.TraceWarning($"BASS_SetConfig({nameof(BASSConfig.BASS_CONFIG_UPDATETHREADS)}) に失敗しました。[{Bass.BASS_ErrorGetCode()}]");
		    }
		
			// BASS の初期化。

			int nデバイス = 0;		// 0:"no device" … BASS からはデバイスへアクセスさせない。アクセスは BASSASIO アドオンから行う。
			int n周波数 = 44100;	// 仮決め。最終的な周波数はデバイス（≠ドライバ）が決める。
			if( !Bass.BASS_Init( nデバイス, n周波数, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero ) )
				throw new Exception( string.Format( "BASS の初期化に失敗しました。(BASS_Init)[{0}]", Bass.BASS_ErrorGetCode().ToString() ) );

		    Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_CURVE_VOL, true);

//Debug.WriteLine( "BASS_Init()完了。" );
			#region [ デバッグ用: ASIOデバイスのenumerateと、ログ出力 ]
//			CEnumerateAllAsioDevices.GetAllASIODevices();
//Debug.WriteLine( "BassAsio.BASS_ASIO_GetDeviceInfo():" );
//            int a, count = 0;
//            BASS_ASIO_DEVICEINFO asioDevInfo;
//            for ( a = 0; ( asioDevInfo = BassAsio.BASS_ASIO_GetDeviceInfo( a ) ) != null; a++ )
//            {
//                Trace.TraceInformation( "ASIO Device {0}: {1}, driver={2}", a, asioDevInfo.name, asioDevInfo.driver );
//                count++; // count it
//            }
			#endregion

			// BASS ASIO の初期化。
			BASS_ASIO_INFO asioInfo = null;
			if ( BassAsio.BASS_ASIO_Init( ASIODeviceIndex, BASSASIOInit.BASS_ASIO_THREAD ) )	// 専用スレッドにて起動
			{
				#region [ ASIO の初期化に成功。]
				//-----------------
				this.NowSoundDevice = SoundDeviceType.ASIO;
				asioInfo = BassAsio.BASS_ASIO_GetInfo();
				this.OutputChannelCount = asioInfo.outputs;
				this.ASIOFrequency = BassAsio.BASS_ASIO_GetRate();
				this.NowASIODevideFormat = BassAsio.BASS_ASIO_ChannelGetFormat( false, 0 );

				Trace.TraceInformation( "BASS を初期化しました。(ASIO, デバイス:\"{0}\", 入力{1}, 出力{2}, {3}Hz, バッファ{4}～{6}sample ({5:0.###}～{7:0.###}ms), デバイスフォーマット:{8})",
					asioInfo.name,
					asioInfo.inputs,
					asioInfo.outputs,
					this.ASIOFrequency.ToString( "0.###" ),
					asioInfo.bufmin, asioInfo.bufmin * 1000 / this.ASIOFrequency,
					asioInfo.bufmax, asioInfo.bufmax * 1000 / this.ASIOFrequency,
					this.NowASIODevideFormat.ToString()
					);
				this.IsBASSFree = false;
				#region [ debug: channel format ]
				//BASS_ASIO_CHANNELINFO chinfo = new BASS_ASIO_CHANNELINFO();
				//int chan = 0;
				//while ( true )
				//{
				//    if ( !BassAsio.BASS_ASIO_ChannelGetInfo( false, chan, chinfo ) )
				//        break;
				//    Debug.WriteLine( "Ch=" + chan + ": " + chinfo.name.ToString() + ", " + chinfo.group.ToString() + ", " + chinfo.format.ToString() );
				//    chan++;
				//}
				#endregion
				//-----------------
				#endregion
			}
			else
			{
				#region [ ASIO の初期化に失敗。]
				//-----------------
				BASSError errcode = Bass.BASS_ErrorGetCode();
				string errmes = errcode.ToString();
				if ( errcode == BASSError.BASS_OK )
				{
					errmes = "BASS_OK; The device may be dissconnected";
				}
				Bass.BASS_Free();
				this.IsBASSFree = true;
				throw new Exception( string.Format( "BASS (ASIO) の初期化に失敗しました。(BASS_ASIO_Init)[{0}]", errmes ) );
				//-----------------
				#endregion
			}


			// ASIO 出力チャンネルの初期化。

			this._AsioProc = new ASIOPROC( this.ASIOProcess );		// アンマネージに渡す delegate は、フィールドとして保持しておかないとGCでアドレスが変わってしまう。
			if ( !BassAsio.BASS_ASIO_ChannelEnable( false, 0, this._AsioProc, IntPtr.Zero ) )		// 出力チャンネル0 の有効化。
			{
				#region [ ASIO 出力チャンネルの初期化に失敗。]
				//-----------------
				BassAsio.BASS_ASIO_Free();
				Bass.BASS_Free();
				this.IsBASSFree = true;
				throw new Exception( string.Format( "Failed BASS_ASIO_ChannelEnable() [{0}]", BassAsio.BASS_ASIO_ErrorGetCode().ToString() ) );
				//-----------------
				#endregion
			}
			for ( int i = 1; i < this.OutputChannelCount; i++ )		// 出力チャネルを全てチャネル0とグループ化する。
			{														// チャネル1だけを0とグループ化すると、3ch以上の出力をサポートしたカードでの動作がおかしくなる
				if ( !BassAsio.BASS_ASIO_ChannelJoin( false, i, 0 ) )
				{
					#region [ 初期化に失敗。]
					//-----------------
					BassAsio.BASS_ASIO_Free();
					Bass.BASS_Free();
					this.IsBASSFree = true;
					throw new Exception( string.Format( "Failed BASS_ASIO_ChannelJoin({1}) [{0}]", BassAsio.BASS_ASIO_ErrorGetCode().ToString(), i ) );
					//-----------------
					#endregion
				}
			}
			if ( !BassAsio.BASS_ASIO_ChannelSetFormat( false, 0, this.ASIOChannelFormat ) )	// 出力チャンネル0のフォーマット
			{
				#region [ ASIO 出力チャンネルの初期化に失敗。]
				//-----------------
				BassAsio.BASS_ASIO_Free();
				Bass.BASS_Free();
				this.IsBASSFree = true;
				throw new Exception( string.Format( "Failed BASS_ASIO_ChannelSetFormat() [{0}]", BassAsio.BASS_ASIO_ErrorGetCode().ToString() ) );
				//-----------------
				#endregion
			}

			// ASIO 出力と同じフォーマットを持つ BASS ミキサーを作成。

			var flag = BASSFlag.BASS_MIXER_NONSTOP | BASSFlag.BASS_STREAM_DECODE;	// デコードのみ＝発声しない。ASIO に出力されるだけ。
			if( this.NowASIODevideFormat == BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT )
				flag |= BASSFlag.BASS_SAMPLE_FLOAT;
			this.MixerStreamHandle = BassMix.BASS_Mixer_StreamCreate( (int) this.ASIOFrequency, this.OutputChannelCount, flag );

			if ( this.MixerStreamHandle == 0 )
			{
				BASSError err = Bass.BASS_ErrorGetCode(); 
				BassAsio.BASS_ASIO_Free();
				Bass.BASS_Free();
				this.IsBASSFree = true;
				throw new Exception( string.Format( "BASSミキサ(mixing)の作成に失敗しました。[{0}]", err ) );
			}

			// BASS ミキサーの1秒あたりのバイト数を算出。

			var mixerInfo = Bass.BASS_ChannelGetInfo( this.MixerStreamHandle );
			int sampleSizeByte = 0;
			switch( this.ASIOChannelFormat )
			{
				case BASSASIOFormat.BASS_ASIO_FORMAT_16BIT: sampleSizeByte = 2; break;
				case BASSASIOFormat.BASS_ASIO_FORMAT_24BIT: sampleSizeByte = 3; break;
				case BASSASIOFormat.BASS_ASIO_FORMAT_32BIT: sampleSizeByte = 4; break;
				case BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT: sampleSizeByte = 4; break;
			}
			//long nミキサーの1サンプルあたりのバイト数 = /*mixerInfo.chans*/ 2 * nサンプルサイズbyte;
			long sampleByteCount = mixerInfo.chans * sampleSizeByte;
			this.BytesPerSecondOsMixer = sampleByteCount * mixerInfo.freq;


			// 単純に、hMixerの音量をMasterVolumeとして制御しても、
			// ChannelGetData()の内容には反映されない。
			// そのため、もう一段mixerを噛ませて、一段先のmixerからChannelGetData()することで、
			// hMixerの音量制御を反映させる。
			this.MixerStreamHandleForChangingChannelCount = BassMix.BASS_Mixer_StreamCreate(
				(int) this.ASIOFrequency, this.OutputChannelCount, flag );
			if ( this.MixerStreamHandleForChangingChannelCount == 0 )
			{
				BASSError errcode = Bass.BASS_ErrorGetCode();
				BassAsio.BASS_ASIO_Free();
				Bass.BASS_Free();
				this.IsBASSFree = true;
				throw new Exception( string.Format( "BASSミキサ(最終段)の作成に失敗しました。[{0}]", errcode ) );
			}
			{
				bool b1 = BassMix.BASS_Mixer_StreamAddChannel( this.MixerStreamHandleForChangingChannelCount, this.MixerStreamHandle, BASSFlag.BASS_DEFAULT );
				if ( !b1 )
				{
					BASSError errcode = Bass.BASS_ErrorGetCode();
					BassAsio.BASS_ASIO_Free();
					Bass.BASS_Free();
					this.IsBASSFree = true;
					throw new Exception( string.Format( "BASSミキサ(最終段とmixing)の接続に失敗しました。[{0}]", errcode ) );
				};
			}


			// 出力を開始。

			this.SampleBufferSize = (int) ( bufferSizeMs * this.ASIOFrequency / 1000.0 );
			//this.nバッファサイズsample = (int)  nバッファサイズbyte;
			if ( !BassAsio.BASS_ASIO_Start( this.SampleBufferSize ) )		// 範囲外の値を指定した場合は自動的にデフォルト値に設定される。
			{
				BASSError err = BassAsio.BASS_ASIO_ErrorGetCode();
				BassAsio.BASS_ASIO_Free();
				Bass.BASS_Free();
				this.IsBASSFree = true;
				throw new Exception( "ASIO デバイス出力開始に失敗しました。" + err.ToString() );
			}
			else
			{
				int n遅延sample = BassAsio.BASS_ASIO_GetLatency( false );	// この関数は BASS_ASIO_Start() 後にしか呼び出せない。
				int n希望遅延sample = (int) ( bufferSizeMs * this.ASIOFrequency / 1000.0 );
				this.ActualBufferSizeMs = this.ActualOutputDelay = (long) ( n遅延sample * 1000.0f / this.ASIOFrequency );
				Trace.TraceInformation( "ASIO デバイス出力開始：バッファ{0}sample(希望{1}) [{2}ms(希望{3}ms)]", n遅延sample, n希望遅延sample, this.ActualOutputDelay, bufferSizeMs );
			}
		}

		#region [ tサウンドを作成する() ]
		public FDKSound CreateFDKSound( string filePath, SoundGroup soundGroup )
		{
			var sound = new FDKSound(soundGroup);
			sound.CreateASIOSound( filePath, this.MixerStreamHandle );
			return sound;
		}

		public void CreateFDKSound( string filePath, FDKSound sound )
		{
			sound.CreateASIOSound( filePath, this.MixerStreamHandle );
		}
		public void CreateFDKSound( byte[] wavFileImage, FDKSound sound )
		{
			sound.CreateASIOSound( wavFileImage, this.MixerStreamHandle );
		}
		#endregion


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
			if ( MixerStreamHandle != -1 )
			{
				Bass.BASS_StreamFree( this.MixerStreamHandle );
			}
			if ( !this.IsBASSFree )
			{
				BassAsio.BASS_ASIO_Free();	// システムタイマより先に呼び出すこと。（tAsio処理() の中でシステムタイマを参照してるため）
				Bass.BASS_Free();
			}

			if( bManagedDispose )
			{
				FDKCommon.Dispose( this.SystemTimer );
				this.SystemTimer = null;
			}
		}
		~SoundDeviceASIO()
		{
			this.Dispose( false );
		}
		//-----------------
		#endregion


		protected int MixerStreamHandle = -1;
		protected int MixerStreamHandleForChangingChannelCount = -1; 
		protected int OutputChannelCount = 0;
		protected double ASIOFrequency = 0.0;
		protected int SampleBufferSize = 0;
		protected BASSASIOFormat NowASIODevideFormat = BASSASIOFormat.BASS_ASIO_FORMAT_UNKNOWN;
		protected BASSASIOFormat ASIOChannelFormat = BASSASIOFormat.BASS_ASIO_FORMAT_16BIT;		// 16bit 固定
		//protected BASSASIOFormat fmtASIOチャンネルフォーマット = BASSASIOFormat.BASS_ASIO_FORMAT_32BIT;// 16bit 固定
		protected ASIOPROC _AsioProc = null;

		protected int ASIOProcess( bool input, int channel, IntPtr buffer, int length, IntPtr user )
		{
			if( input ) return 0;


			// BASSミキサからの出力データをそのまま ASIO buffer へ丸投げ。

			int num = Bass.BASS_ChannelGetData( this.MixerStreamHandleForChangingChannelCount, buffer, length );		// num = 実際に転送した長さ

			if ( num == -1 ) num = 0;


			// 経過時間を更新。
			// データの転送差分ではなく累積転送バイト数から算出する。

			this.ElapsedTimeMs = ( this.CumulativeTransferBytes * 1000 / this.BytesPerSecondOsMixer ) - this.ActualOutputDelay;
			this.SystemTimeWithUpdatedElapsedTime = this.SystemTimer.SystemTimeMs;


			// 経過時間を更新後に、今回分の累積転送バイト数を反映。

			this.CumulativeTransferBytes += num;
			return num;
		}

		private long BytesPerSecondOsMixer = 0;
		private long CumulativeTransferBytes = 0;
		private bool IsBASSFree = true;
	}
}
