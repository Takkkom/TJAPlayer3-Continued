using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Un4seen.Bass;
using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Mix;

namespace FDK
{
	internal class SoundDeviceWASAPI : ISoundDevice
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

		public enum DeviceModeType { Exclusive, Share }

		public int MasterVolume
		{
			get
			{
				float volume = 0.0f;
				//if ( BassMix.BASS_Mixer_ChannelGetEnvelopePos( this.hMixer, BASSMIXEnvelope.BASS_MIXER_ENV_VOL, ref f音量 ) == -1 )
				//    return 100;
				//bool b = Bass.BASS_ChannelGetAttribute( this.hMixer, BASSAttribute.BASS_ATTRIB_VOL, ref f音量 );
				bool b = Bass.BASS_ChannelGetAttribute( this.hMixer, BASSAttribute.BASS_ATTRIB_VOL, ref volume );
				if ( !b )
				{
					BASSError be = Bass.BASS_ErrorGetCode();
					Trace.TraceInformation( "WASAPI Master Volume Get Error: " + be.ToString() );
				}
				else
				{
					Trace.TraceInformation( "WASAPI Master Volume Get Success: " + (volume * 100) );

				}
				return (int) ( volume * 100 );
			}
			set
			{
				// bool b = Bass.BASS_SetVolume( value / 100.0f );
				// →Exclusiveモード時は無効

//				bool b = BassWasapi.BASS_WASAPI_SetVolume( BASSWASAPIVolume.BASS_WASAPI_VOL_SESSION, (float) ( value / 100 ) );
//				bool b = BassWasapi.BASS_WASAPI_SetVolume( BASSWASAPIVolume.BASS_WASAPI_CURVE_WINDOWS, (float) ( value / 100 ) );
				bool b = Bass.BASS_ChannelSetAttribute( this.hMixer, BASSAttribute.BASS_ATTRIB_VOL, (float) ( value / 100.0 ) );
				// If you would like to have a volume control in exclusive mode too, and you're using the BASSmix add-on,
				// you can adjust the source's BASS_ATTRIB_VOL setting via BASS_ChannelSetAttribute.
				// しかし、hMixerに対するBASS_ChannelSetAttribute()でBASS_ATTRIB_VOLを変更: なぜか出力音量に反映されず

				// Bass_SetVolume(): BASS_ERROR_NOTAVIL ("no sound" deviceには適用不可)

				// Mixer_ChannelSetEnvelope():

				//var nodes = new BASS_MIXER_NODE[ 1 ] { new BASS_MIXER_NODE( 0, (float) value ) };
				//bool b = BassMix.BASS_Mixer_ChannelSetEnvelope( this.hMixer, BASSMIXEnvelope.BASS_MIXER_ENV_VOL, nodes );
				//bool b = Bass.BASS_ChannelSetAttribute( this.hMixer, BASSAttribute.BASS_ATTRIB_VOL, value / 100.0f );
				if ( !b )
				{
					BASSError be = Bass.BASS_ErrorGetCode();
					Trace.TraceInformation( "WASAPI Master Volume Set Error: " + be.ToString() );
				}
				else
				{
					// int n = this.nMasterVolume;	
					// Trace.TraceInformation( "WASAPI Master Volume Set Success: " + value );

				}
			}
		}
		// メソッド

		/// <summary>
		/// WASAPIの初期化
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="bufferSizeMs">(未使用; 本メソッド内で自動設定する)</param>
		/// <param name="updateIntervalMs">(未使用; 本メソッド内で自動設定する)</param>
		public SoundDeviceWASAPI( DeviceModeType mode, long bufferSizeMs, long updateIntervalMs )
		{
			// 初期化。

			Trace.TraceInformation( "BASS (WASAPI) の初期化を開始します。" );

			this.NowSoundDevice = SoundDeviceType.Unknown;
			this.ActualOutputDelay = 0;
			this.ElapsedTimeMs = 0;
			this.SystemTimeWithUpdatedElapsedTime = FDKTimer.Unused;
			this.SystemTimer = new FDKTimer( FDKTimer.TimerType.MultiMedia );
			this.FirstActualOutpuDelayCalculation = true;

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

			int nBASSWASAPIVersion = Utils.HighWord( BassWasapi.BASS_WASAPI_GetVersion() );
			if( nBASSWASAPIVersion != BassWasapi.BASSWASAPIVERSION )
				throw new DllNotFoundException( string.Format( "basswasapi.dll のバージョンが異なります({0})。このプログラムはバージョン{1}で動作します。", nBASSWASAPIVersion, BassWasapi.BASSWASAPIVERSION ) );
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

			int device = 0;		// 0:"no device" … BASS からはデバイスへアクセスさせない。アクセスは BASSWASAPI アドオンから行う。
			int frequency = 44100;	// 仮決め。lデバイス（≠ドライバ）がネイティブに対応している周波数であれば何でもいい？ようだ。BASSWASAPIでデバイスの周波数は変えられる。いずれにしろBASSMXで自動的にリサンプリングされる。
			if( !Bass.BASS_Init( device, frequency, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero ) )
				throw new Exception( string.Format( "BASS (WASAPI) の初期化に失敗しました。(BASS_Init)[{0}]", Bass.BASS_ErrorGetCode().ToString() ) );

		    Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_CURVE_VOL, true);

			#region [ デバッグ用: WASAPIデバイスのenumerateと、ログ出力 ]
			// (デバッグ用)
			//Trace.TraceInformation( "WASAPIデバイス一覧:" );
			//int a, count = 0;
			//BASS_WASAPI_DEVICEINFO wasapiDevInfo;
			//for ( a = 0; ( wasapiDevInfo = BassWasapi.BASS_WASAPI_GetDeviceInfo( a ) ) != null; a++ )
			//{
			//    if ( ( wasapiDevInfo.flags & BASSWASAPIDeviceInfo.BASS_DEVICE_INPUT ) == 0 // device is an output device (not input)
			//            && ( wasapiDevInfo.flags & BASSWASAPIDeviceInfo.BASS_DEVICE_ENABLED ) != 0 ) // and it is enabled
			//    {
			//        Trace.TraceInformation( "WASAPI Device #{0}: {1}", a, wasapiDevInfo.name );
			//        count++; // count it
			//    }
			//}
			#endregion

			// BASS WASAPI の初期化。

			device = -1;
			frequency = 0;			// デフォルトデバイスの周波数 (0="mix format" sample rate)
			int channel = 0;	// デフォルトデバイスのチャンネル数 (0="mix format" channels)
			this.tWasapiProc = new WASAPIPROC( this.WASAPIProcess );		// アンマネージに渡す delegate は、フィールドとして保持しておかないとGCでアドレスが変わってしまう。

			// WASAPIの更新間隔(period)は、バッファサイズにも影響を与える。
			// 更新間隔を最小にするには、BassWasapi.BASS_WASAPI_GetDeviceInfo( ndevNo ).minperiod の値を使えばよい。
			// これをやらないと、更新間隔ms=6ms となり、バッファサイズを 6ms x 4 = 24msより小さくできない。
			#region [ 既定の出力デバイスと設定されているWASAPIデバイスを検索し、更新間隔msを設定できる最小値にする ]
			int nDevNo = -1;
			BASS_WASAPI_DEVICEINFO deviceInfo;
			for ( int n = 0; ( deviceInfo = BassWasapi.BASS_WASAPI_GetDeviceInfo( n ) ) != null; n++ )
			{
				if ( deviceInfo.IsDefault )
				{
					nDevNo = n;
					break;
				}
			}
			if ( nDevNo != -1 )
			{
				// Trace.TraceInformation( "Selected Default WASAPI Device: {0}", deviceInfo.name );
				// Trace.TraceInformation( "MinPeriod={0}, DefaultPeriod={1}", deviceInfo.minperiod, deviceInfo.defperiod );
				updateIntervalMs = (long) ( deviceInfo.minperiod * 1000 );
				if ( bufferSizeMs <= 0 || bufferSizeMs < updateIntervalMs + 1 )
				{
					bufferSizeMs = updateIntervalMs + 1;	// 2013.4.25 #31237 yyagi; バッファサイズ設定の完全自動化。更新間隔＝バッファサイズにするとBASS_ERROR_UNKNOWNになるので+1する。
				}
			}
			else
			{
				Trace.TraceError( "Error: Default WASAPI Device is not found." );
			}
			#endregion

//Retry:
			var flags = ( mode == DeviceModeType.Exclusive ) ? BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT | BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE : BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT;
			//var flags = ( mode == Eデバイスモード.排他 ) ? BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT | BASSWASAPIInit.BASS_WASAPI_EVENT | BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE : BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT | BASSWASAPIInit.BASS_WASAPI_EVENT;
			if ( BassWasapi.BASS_WASAPI_Init( device, frequency, channel, flags, ( bufferSizeMs / 1000.0f ), ( updateIntervalMs / 1000.0f ), this.tWasapiProc, IntPtr.Zero ) )
			{
				if( mode == DeviceModeType.Exclusive )
				{
					#region [ 排他モードで作成成功。]
					//-----------------
					this.NowSoundDevice = SoundDeviceType.ExclusiveWASAPI;

					nDevNo = BassWasapi.BASS_WASAPI_GetDevice();
					deviceInfo = BassWasapi.BASS_WASAPI_GetDeviceInfo( nDevNo );
					var wasapiInfo = BassWasapi.BASS_WASAPI_GetInfo();
					int sampleByteCount = 2 * wasapiInfo.chans;	// default;
					switch( wasapiInfo.format )		// BASS WASAPI で扱うサンプルはすべて 32bit float で固定されているが、デバイスはそうとは限らない。
					{
						case BASSWASAPIFormat.BASS_WASAPI_FORMAT_8BIT: sampleByteCount = 1 * wasapiInfo.chans; break;
						case BASSWASAPIFormat.BASS_WASAPI_FORMAT_16BIT: sampleByteCount = 2 * wasapiInfo.chans; break;
						case BASSWASAPIFormat.BASS_WASAPI_FORMAT_24BIT: sampleByteCount = 3 * wasapiInfo.chans; break;
						case BASSWASAPIFormat.BASS_WASAPI_FORMAT_32BIT: sampleByteCount = 4 * wasapiInfo.chans; break;
						case BASSWASAPIFormat.BASS_WASAPI_FORMAT_FLOAT: sampleByteCount = 4 * wasapiInfo.chans; break;
					}
					int byteCount = sampleByteCount * wasapiInfo.freq;
					this.ActualBufferSizeMs = (long) ( wasapiInfo.buflen * 1000.0f / byteCount );
					this.ActualOutputDelay = 0;	// 初期値はゼロ
					Trace.TraceInformation( "使用デバイス: #" + nDevNo + " : " + deviceInfo.name + ", flags=" + deviceInfo.flags );
					Trace.TraceInformation( "BASS を初期化しました。(WASAPI排他モード, {0}Hz, {1}ch, フォーマット:{2}, バッファ{3}bytes [{4}ms(希望{5}ms)], 更新間隔{6}ms)",
						wasapiInfo.freq,
						wasapiInfo.chans,
						wasapiInfo.format.ToString(),
						wasapiInfo.buflen,
						ActualBufferSizeMs.ToString(),
						bufferSizeMs.ToString(),
						updateIntervalMs.ToString() );
					Trace.TraceInformation( "デバイスの最小更新時間={0}ms, 既定の更新時間={1}ms", deviceInfo.minperiod * 1000, deviceInfo.defperiod * 1000 );
					this.IsBASSFree = false;
					//-----------------
					#endregion
				}
				else
				{
					#region [ 共有モードで作成成功。]
					//-----------------
					this.NowSoundDevice = SoundDeviceType.SharedWASAPI;
					
					this.ActualOutputDelay = 0;	// 初期値はゼロ
					var devInfo = BassWasapi.BASS_WASAPI_GetDeviceInfo( BassWasapi.BASS_WASAPI_GetDevice() );	// 共有モードの場合、更新間隔はデバイスのデフォルト値に固定される。
					Trace.TraceInformation( "BASS を初期化しました。(WASAPI共有モード, {0}ms, 更新間隔{1}ms)", bufferSizeMs, devInfo.defperiod * 1000.0f );
					this.IsBASSFree = false;
					//-----------------
					#endregion
				}
			}
			#region [ #31737 WASAPI排他モードのみ利用可能とし、WASAPI共有モードは使用できないようにするために、WASAPI共有モードでの初期化フローを削除する。 ]
			//else if ( mode == Eデバイスモード.排他 )
			//{
			//    Trace.TraceInformation("Failed to initialize setting BASS (WASAPI) mode [{0}]", Bass.BASS_ErrorGetCode().ToString() );
			//    #region [ 排他モードに失敗したのなら共有モードでリトライ。]
			//    //-----------------
			//    mode = Eデバイスモード.共有;
			//    goto Retry;
			//    //-----------------
			//    #endregion
			//}
			#endregion
			else
			{
				#region [ それでも失敗したら例外発生。]
				//-----------------
				BASSError errcode = Bass.BASS_ErrorGetCode();
				Bass.BASS_Free();
				this.IsBASSFree = true;
				throw new Exception( string.Format( "BASS (WASAPI) の初期化に失敗しました。(BASS_WASAPI_Init)[{0}]", errcode ) );
				//-----------------
				#endregion
			}


			// WASAPI出力と同じフォーマットを持つ BASS ミキサーを作成。

			var info = BassWasapi.BASS_WASAPI_GetInfo();
			this.hMixer = BassMix.BASS_Mixer_StreamCreate(
				info.freq,
				info.chans,
				BASSFlag.BASS_MIXER_NONSTOP | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE );	// デコードのみ＝発声しない。WASAPIに出力されるだけ。
			if ( this.hMixer == 0 )
			{
				BASSError errcode = Bass.BASS_ErrorGetCode();
				BassWasapi.BASS_WASAPI_Free();
				Bass.BASS_Free();
				this.IsBASSFree = true;
				throw new Exception( string.Format( "BASSミキサ(mixing)の作成に失敗しました。[{0}]", errcode ) );
			}


			// BASS ミキサーの1秒あたりのバイト数を算出。

			var mixerInfo = Bass.BASS_ChannelGetInfo( this.hMixer );
			long bytesSampleOfMixer = mixerInfo.chans * 4;	// 4 = sizeof(FLOAT)
			this.BytesPerSecondOfMixer = bytesSampleOfMixer * mixerInfo.freq;



			// 単純に、hMixerの音量をMasterVolumeとして制御しても、
			// ChannelGetData()の内容には反映されない。
			// そのため、もう一段mixerを噛ませて、一段先のmixerからChannelGetData()することで、
			// hMixerの音量制御を反映させる。
			this.hMixer_DeviceOut = BassMix.BASS_Mixer_StreamCreate(
				info.freq,
				info.chans,
				BASSFlag.BASS_MIXER_NONSTOP | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE );	// デコードのみ＝発声しない。WASAPIに出力されるだけ。
			if ( this.hMixer_DeviceOut == 0 )
			{
				BASSError errcode = Bass.BASS_ErrorGetCode();
				BassWasapi.BASS_WASAPI_Free();
				Bass.BASS_Free();
				this.IsBASSFree = true;
				throw new Exception( string.Format( "BASSミキサ(最終段)の作成に失敗しました。[{0}]", errcode ) );
			}

			{
				bool b1 = BassMix.BASS_Mixer_StreamAddChannel( this.hMixer_DeviceOut, this.hMixer, BASSFlag.BASS_DEFAULT );
				if ( !b1 )
				{
					BASSError errcode = Bass.BASS_ErrorGetCode();
					BassWasapi.BASS_WASAPI_Free();
					Bass.BASS_Free();
					this.IsBASSFree = true;
					throw new Exception( string.Format( "BASSミキサ(最終段とmixing)の接続に失敗しました。[{0}]", errcode ) );
				};
			}


			// 出力を開始。

			BassWasapi.BASS_WASAPI_Start();
		}
		#region [ tサウンドを作成する() ]
		public FDKSound CreateFDKSound( string filePath, SoundGroup soundGroup )
		{
			var sound = new FDKSound(soundGroup);
			sound.CreateWASAPISound( filePath, this.hMixer, this.NowSoundDevice );
			return sound;
		}

		public void CreateFDKSound( string filePath, FDKSound sound )
		{
			sound.CreateWASAPISound( filePath, this.hMixer, this.NowSoundDevice );
		}
		public void CreateFDKSound( byte[] wavFileImage, FDKSound sound )
		{
			sound.CreateWASAPISound( wavFileImage, this.hMixer, this.NowSoundDevice );
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
			if ( hMixer != -1 )
			{
				Bass.BASS_StreamFree( this.hMixer );
			}
			if ( !this.IsBASSFree )
			{
				BassWasapi.BASS_WASAPI_Free();	// システムタイマより先に呼び出すこと。（tWasapi処理() の中でシステムタイマを参照してるため）
				Bass.BASS_Free();
			}
			if( bManagedDispose )
			{
				FDKCommon.Dispose( this.SystemTimer );
				this.SystemTimer = null;
			}
		}
		~SoundDeviceWASAPI()
		{
			this.Dispose( false );
		}
		//-----------------
		#endregion

		protected int hMixer = -1;
		protected int hMixer_DeviceOut = -1;
		protected WASAPIPROC tWasapiProc = null;

		protected int WASAPIProcess( IntPtr buffer, int length, IntPtr user )
		{
			// BASSミキサからの出力データをそのまま WASAPI buffer へ丸投げ。

			int num = Bass.BASS_ChannelGetData( this.hMixer_DeviceOut, buffer, length );		// num = 実際に転送した長さ
			if ( num == -1 ) num = 0;


			// 経過時間を更新。
			// データの転送差分ではなく累積転送バイト数から算出する。

			int unplayedBytes = BassWasapi.BASS_WASAPI_GetData( null, (int) BASSData.BASS_DATA_AVAILABLE );	// 誤差削減のため、必要となるギリギリ直前に取得する。
			this.ElapsedTimeMs = ( this.CumulativeTransferBytes - unplayedBytes ) * 1000 / this.BytesPerSecondOfMixer;
			this.SystemTimeWithUpdatedElapsedTime = this.SystemTimer.SystemTimeMs;

			// 実出力遅延を更新。
			// 未再生バイト数の平均値。

			long nowDelayMs = unplayedBytes * 1000 / this.BytesPerSecondOfMixer;
			this.ActualOutputDelay = ( this.FirstActualOutpuDelayCalculation ) ? nowDelayMs : ( this.ActualOutputDelay + nowDelayMs ) / 2;
			this.FirstActualOutpuDelayCalculation = false;

			
			// 経過時間を更新後に、今回分の累積転送バイト数を反映。
			
			this.CumulativeTransferBytes += num;
			return num;
		}

		private long BytesPerSecondOfMixer = 0;
		private long CumulativeTransferBytes = 0;
		private bool FirstActualOutpuDelayCalculation = true;
		private bool IsBASSFree = true;
	}
}
