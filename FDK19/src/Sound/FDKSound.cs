using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Threading;
using FDK.ExtensionMethods;
using SlimDX.DirectSound;
using SlimDX.Multimedia;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Fx;


namespace FDK
{
	#region [ DTXMania用拡張 ]
	public class SoundManager	// : CSound
	{
		private static ISoundDevice SoundDevice
		{
			get; set;
		}
		private static SoundDeviceType SoundDeviceType
		{
			get; set;
		}
		public static FDKSoundTimer PlayTimer = null;
		public static bool bUseOSTimer = false;		// OSのタイマーを使うか、CSoundTimerを使うか。DTXCではfalse, DTXManiaではtrue。
													// DTXC(DirectSound)でCSoundTimerを使うと、内部で無音のループサウンドを再生するため
													// サウンドデバイスを占有してしまい、Viewerとして呼び出されるDTXManiaで、ASIOが使えなくなる。

													// DTXMania単体でこれをtrueにすると、WASAPI/ASIO時に演奏タイマーとしてFDKタイマーではなく
													// システムのタイマーを使うようになる。こうするとスクロールは滑らかになるが、音ズレが出るかもしれない。
		
		public static IntPtr WindowHandle;

		public static bool bIsTimeStretch = false;

		private static int _nMasterVolume;
		public int nMasterVolume
		{
			get
			{
				return _nMasterVolume;
			}
			//get
			//{
			//    if ( SoundDeviceType == ESoundDeviceType.ExclusiveWASAPI || SoundDeviceType == ESoundDeviceType.ASIO )
			//    {
			//        return Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_GVOL_STREAM ) / 100;
			//    }
			//    else
			//    {
			//        return 100;
			//    }
			//}
			//set
			//{
			//    if ( SoundDeviceType == ESoundDeviceType.ExclusiveWASAPI )
			//    {
			//			// LINEARでなくWINDOWS(2)を使う必要があるが、exclusive時は使用不可、またデバイス側が対応してないと使用不可
			//        bool b = BassWasapi.BASS_WASAPI_SetVolume( BASSWASAPIVolume.BASS_WASAPI_CURVE_LINEAR, value / 100.0f );
			//        if ( !b )
			//        {
			//            BASSError be = Bass.BASS_ErrorGetCode();
			//            Trace.TraceInformation( "WASAPI Master Volume Set Error: " + be.ToString() );
			//        }
			//    }
			//}
			//set
			//{
			//    if ( SoundDeviceType == ESoundDeviceType.ExclusiveWASAPI || SoundDeviceType == ESoundDeviceType.ASIO )
			//    {
			//        bool b = Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_GVOL_STREAM, value * 100 );
			//        if ( !b )
			//        {
			//            BASSError be = Bass.BASS_ErrorGetCode();
			//            Trace.TraceInformation( "Master Volume Set Error: " + be.ToString() );
			//        }
			//    }
			//}
			//set
			//{
			//    if ( SoundDeviceType == ESoundDeviceType.ExclusiveWASAPI || SoundDeviceType == ESoundDeviceType.ASIO )
			//    {
			//        var nodes = new BASS_MIXER_NODE[ 1 ] { new BASS_MIXER_NODE( 0, (float) value ) };
			//        BassMix.BASS_Mixer_ChannelSetEnvelope( SoundDevice.hMixer, BASSMIXEnvelope.BASS_MIXER_ENV_VOL, nodes );
			//    }
			//}
			set
			{
				SoundDevice.MasterVolume = value;
				_nMasterVolume = value;
			}
		}

		///// <summary>
		///// BASS時、mp3をストリーミング再生せずに、デコードしたraw wavをオンメモリ再生する場合はtrueにする。
		///// 特殊なmp3を使用時はシークが乱れるので、必要に応じてtrueにすること。(Config.iniのNoMP3Streamingで設定可能。)
		///// ただし、trueにすると、その分再生開始までの時間が長くなる。
		///// </summary>
		//public static bool bIsMP3DecodeByWindowsCodec = false;

		public static int nMixing = 0;
		public int GetMixingStreams()
		{
			return nMixing;
		}
		public static int nStreams = 0;
		public int GetStreams()
		{
			return nStreams;
		}
		#region [ WASAPI/ASIO/DirectSound設定値 ]
		/// <summary>
		/// <para>WASAPI 排他モード出力における再生遅延[ms]（の希望値）。最終的にはこの数値を基にドライバが決定する）。</para>
		/// <para>0以下の値を指定すると、この数値はWASAPI初期化時に自動設定する。正数を指定すると、その値を設定しようと試みる。</para>
		/// </summary>
		public static int SoundDelayExclusiveWASAPI = 0;		// SSTでは、50ms
		public int GetSoundExclusiveWASAPI()
		{
			return SoundDelayExclusiveWASAPI;
		}
		public void SetSoundDelayExclusiveWASAPI( int value )
		{
			SoundDelayExclusiveWASAPI = value;
		}
		/// <summary>
		/// <para>WASAPI 共有モード出力における再生遅延[ms]。ユーザが決定する。</para>
		/// </summary>
		public static int SoundDelaySharedWASAPI = 100;
		/// <summary>
		/// <para>排他WASAPIバッファの更新間隔。出力間隔ではないので注意。</para>
		/// <para>→ 自動設定されるのでSoundDelay よりも小さい値であること。（小さすぎる場合はBASSによって自動修正される。）</para>
		/// </summary>
		public static int SoundUpdatePeriodExclusiveWASAPI = 6;
		/// <summary>
		/// <para>共有WASAPIバッファの更新間隔。出力間隔ではないので注意。</para>
		/// <para>SoundDelay よりも小さい値であること。（小さすぎる場合はBASSによって自動修正される。）</para>
		/// </summary>
		public static int SoundUpdatePeriodSharedWASAPI = 6;
		///// <summary>
		///// <para>ASIO 出力における再生遅延[ms]（の希望値）。最終的にはこの数値を基にドライバが決定する）。</para>
		///// </summary>
		//public static int SoundDelayASIO = 0;					// SSTでは50ms。0にすると、デバイスの設定値をそのまま使う。
		/// <summary>
		/// <para>ASIO 出力におけるバッファサイズ。</para>
		/// </summary>
		public static int SoundDelayASIO = 0;						// 0にすると、デバイスの設定値をそのまま使う。
		public int GetSoundDelayASIO()
		{
			return SoundDelayASIO;
		}
		public void SetSoundDelayASIO(int value)
		{
			SoundDelayASIO = value;
		}
		public static int ASIODevice = 0;
		public int GetASIODevice()
		{
			return ASIODevice;
		}
		public void SetASIODevice(int value)
		{
			ASIODevice = value;
		}
		/// <summary>
		/// <para>DirectSound 出力における再生遅延[ms]。ユーザが決定する。</para>
		/// </summary>
		public static int SoundDelayDirectSound = 100;

		public long GetSoundDelay()
		{
			if ( SoundDevice != null )
			{
				return SoundDevice.ActualBufferSizeMs;
			}
			else
			{
				return -1;
			}
		}

		#endregion


		/// <summary>
		/// DTXMania用コンストラクタ
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="soundDeviceType"></param>
		/// <param name="nSoundDelayExclusiveWASAPI"></param>
		/// <param name="nSoundDelayASIO"></param>
		/// <param name="nASIODevice"></param>
		public SoundManager( IntPtr handle, SoundDeviceType soundDeviceType, int nSoundDelayExclusiveWASAPI, int nSoundDelayASIO, int nASIODevice, bool _bUseOSTimer )
		{
			WindowHandle = handle;
			SoundDevice = null;
			//bUseOSTimer = false;
			Init( soundDeviceType, nSoundDelayExclusiveWASAPI, nSoundDelayASIO, nASIODevice, _bUseOSTimer );
		}
		public void Dispose()
		{
			Terminate();
		}

		//public static void t初期化()
		//{
		//    t初期化( ESoundDeviceType.DirectSound, 0, 0, 0 );
		//}

		public void Init( SoundDeviceType soundDeviceType, int _nSoundDelayExclusiveWASAPI, int _nSoundDelayASIO, int _nASIODevice, IntPtr handle )
		{
			//if ( !bInitialized )
			{
				WindowHandle = handle;
				Init( soundDeviceType, _nSoundDelayExclusiveWASAPI, _nSoundDelayASIO, _nASIODevice );
				//bInitialized = true;
			}
		}
		public void Init( SoundDeviceType soundDeviceType, int _nSoundDelayExclusiveWASAPI, int _nSoundDelayASIO, int _nASIODevice )
		{
			Init( soundDeviceType, _nSoundDelayExclusiveWASAPI, _nSoundDelayASIO, _nASIODevice, false );
		}

		public void Init( SoundDeviceType soundDeviceType, int soundDelayExclusiveWASAPI, int soundDelayASIO, int asioDevice, bool useOSTimer )
		{
			//SoundDevice = null;						// 後で再初期化することがあるので、null初期化はコンストラクタに回す
			PlayTimer = null;						// Global.Bass 依存（つまりユーザ依存）
			nMixing = 0;

			SoundDelayExclusiveWASAPI = soundDelayExclusiveWASAPI;
			SoundDelayASIO = soundDelayASIO;
			ASIODevice = asioDevice;
			bUseOSTimer = useOSTimer;

			SoundDeviceType[] ESoundDeviceTypes = new SoundDeviceType[ 5 ]
			{
				SoundDeviceType.DirectSound,
				SoundDeviceType.ASIO,
				SoundDeviceType.SharedWASAPI,
				SoundDeviceType.ExclusiveWASAPI,
				SoundDeviceType.Unknown
			};

			int firstDevice = (int)soundDeviceType;
			for ( SoundDeviceType = ESoundDeviceTypes[ firstDevice ]; ; SoundDeviceType = ESoundDeviceTypes[ ++firstDevice ] )
			{
				try
				{
					RebuildSoundDevicesAndAllExistingSounds();
					break;
				}
				catch ( Exception e )
				{
					Trace.TraceError( e.ToString() );
					Trace.TraceError( "例外が発生しましたが処理を継続します。 (2609806d-23e8-45c2-9389-b427e80915bc)" );
					if ( ESoundDeviceTypes[ firstDevice ] == SoundDeviceType.Unknown )
					{
						Trace.TraceError( string.Format( "サウンドデバイスの初期化に失敗しました。" ) );
						break;
					}
				}
			}
			if ( soundDeviceType == SoundDeviceType.ExclusiveWASAPI || soundDeviceType == SoundDeviceType.ASIO )
			{
				//Bass.BASS_SetConfig( BASSConfig.BASS_CONFIG_UPDATETHREADS, 4 );
				//Bass.BASS_SetConfig( BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0 );

				Trace.TraceInformation( "BASS_CONFIG_UpdatePeriod=" + Bass.BASS_GetConfig( BASSConfig.BASS_CONFIG_UPDATEPERIOD ) );
				Trace.TraceInformation( "BASS_CONFIG_UpdateThreads=" + Bass.BASS_GetConfig( BASSConfig.BASS_CONFIG_UPDATETHREADS ) );
			}
		}

		public void tDisableUpdateBufferAutomatically()
		{
			//Bass.BASS_SetConfig( BASSConfig.BASS_CONFIG_UPDATETHREADS, 0 );
			//Bass.BASS_SetConfig( BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0 );

			//Trace.TraceInformation( "BASS_CONFIG_UpdatePeriod=" + Bass.BASS_GetConfig( BASSConfig.BASS_CONFIG_UPDATEPERIOD ) );
			//Trace.TraceInformation( "BASS_CONFIG_UpdateThreads=" + Bass.BASS_GetConfig( BASSConfig.BASS_CONFIG_UPDATETHREADS ) );
		}


		public static void Terminate()
		{
			FDKCommon.Dispose( SoundDevice ); SoundDevice = null;
			FDKCommon.Dispose( ref PlayTimer );	// Global.Bass を解放した後に解放すること。（Global.Bass で参照されているため）
		}


		public static void RebuildSoundDevicesAndAllExistingSounds()
		{
			#region [ すでにサウンドデバイスと演奏タイマが構築されていれば解放する。]
			//-----------------
			if ( SoundDevice != null )
			{
				// すでに生成済みのサウンドがあれば初期状態に戻す。

				FDKSound.DisposeAllSoundButKeepInstance();		// リソースは解放するが、CSoundのインスタンスは残す。


				// サウンドデバイスと演奏タイマを解放する。

				FDKCommon.Dispose( SoundDevice ); SoundDevice = null;
				FDKCommon.Dispose( ref PlayTimer );	// Global.SoundDevice を解放した後に解放すること。（Global.SoundDevice で参照されているため）
			}
			//-----------------
			#endregion

			#region [ 新しいサウンドデバイスを構築する。]
			//-----------------
			switch ( SoundDeviceType )
			{
				case SoundDeviceType.ExclusiveWASAPI:
					SoundDevice = new SoundDeviceWASAPI( SoundDeviceWASAPI.DeviceModeType.Exclusive, SoundDelayExclusiveWASAPI, SoundUpdatePeriodExclusiveWASAPI );
					break;

				case SoundDeviceType.SharedWASAPI:
					SoundDevice = new SoundDeviceWASAPI( SoundDeviceWASAPI.DeviceModeType.Share, SoundDelaySharedWASAPI, SoundUpdatePeriodSharedWASAPI );
					break;

				case SoundDeviceType.ASIO:
					SoundDevice = new SoundDeviceASIO( SoundDelayASIO, ASIODevice );
					break;

				case SoundDeviceType.DirectSound:
					SoundDevice = new SoundDeviceDirectSound( WindowHandle, SoundDelayDirectSound, bUseOSTimer );
					break;

				default:
					throw new Exception( string.Format( "未対応の SoundDeviceType です。[{0}]", SoundDeviceType.ToString() ) );
			}
			//-----------------
			#endregion
			#region [ 新しい演奏タイマを構築する。]
			//-----------------
			PlayTimer = new FDKSoundTimer( SoundDevice );
			//-----------------
			#endregion

			SoundDevice.MasterVolume = _nMasterVolume;					// サウンドデバイスに対して、マスターボリュームを再設定する

			FDKSound.RebuildAllExistingSounds( SoundDevice );		// すでに生成済みのサウンドがあれば作り直す。
		}
		public FDKSound CreateFDKSound( string filename, SoundGroup soundGroup )
		{
            if( !File.Exists( filename ) )
            {
                Trace.TraceWarning($"[i18n] File does not exist: {filename}");
                return null;
            }

			if ( SoundDeviceType == SoundDeviceType.Unknown )
			{
				throw new Exception( string.Format( "未対応の SoundDeviceType です。[{0}]", SoundDeviceType.ToString() ) );
			}
			return SoundDevice.CreateFDKSound( filename, soundGroup );
		}

		private static DateTime lastUpdateTime = DateTime.MinValue;
		public void t再生中の処理をする( object o )			// #26122 2011.9.1 yyagi; delegate経由の呼び出し用
		{
			t再生中の処理をする();
		}
		public void t再生中の処理をする()
		{
//★★★★★★★★★★★★★★★★★★★★★ダミー★★★★★★★★★★★★★★★★★★
//			Debug.Write( "再生中の処理をする()" );
			//DateTime now = DateTime.Now;
			//TimeSpan ts = now - lastUpdateTime;
			//if ( ts.Milliseconds > 5 )
			//{
			//    bool b = Bass.BASS_Update( 100 * 2 );
			//    lastUpdateTime = DateTime.Now;
			//    if ( !b )
			//    {
			//        Trace.TraceInformation( "BASS_UPdate() failed: " + Bass.BASS_ErrorGetCode().ToString() );
			//    }
			//}
		}

		public void DisposeSound( FDKSound csound )
		{
		    csound?.DisposeSound( true );			// インスタンスは存続→破棄にする。
		}

		public float GetCPUusage()
		{
			float f;
			switch ( SoundDeviceType )
			{
				case SoundDeviceType.ExclusiveWASAPI:
				case SoundDeviceType.SharedWASAPI:
					f = BassWasapi.BASS_WASAPI_GetCPU();
					break;
				case SoundDeviceType.ASIO:
					f = BassAsio.BASS_ASIO_GetCPU();
					break;
				case SoundDeviceType.DirectSound:
					f = 0.0f;
					break;
				default:
					f = 0.0f;
					break;
			}
			return f;
		}

		public string GetCurrentSoundDeviceType()
		{
			switch ( SoundDeviceType )
			{
				case SoundDeviceType.ExclusiveWASAPI:
				case SoundDeviceType.SharedWASAPI:
					return "WASAPI";
				case SoundDeviceType.ASIO:
					return "ASIO";
				case SoundDeviceType.DirectSound:
					return "DirectSound";
				default:
					return "Unknown";
			}
		}

		public void AddMixer( FDKSound cs, double playSpeed, bool continuesToPlayAfterTheEnd )
		{
			cs.IsContinueToPlayAfterTheEnd = continuesToPlayAfterTheEnd;
			cs.PlaySpped = playSpeed;
			cs.ADDBassSoundToTheMixer();
		}
		public void AddMixer( FDKSound cs, double playSpeed )
		{
			cs.PlaySpped = playSpeed;
			cs.ADDBassSoundToTheMixer();
		}
		public void AddMixer( FDKSound cs )
		{
			cs.ADDBassSoundToTheMixer();
		}
		public void RemoveMixer( FDKSound cs )
		{
			cs.RemoveBassSoundTheMixer();
		}
	}
	#endregion

	// CSound は、サウンドデバイスが変更されたときも、インスタンスを再作成することなく、新しいデバイスで作り直せる必要がある。
	// そのため、デバイスごとに別のクラスに分割するのではなく、１つのクラスに集約するものとする。

	public class FDKSound : IDisposable
	{
	    public const int MinimumSongVol = 0;
	    public const int MaximumSongVol = 200; // support an approximate doubling in volume.
	    public const int DefaultSongVol = 100;

	    // 2018-08-19 twopointzero: Note the present absence of a MinimumAutomationLevel.
	    // We will revisit this if/when song select BGM fade-in/fade-out needs
	    // updating due to changing the type or range of AutomationLevel
	    public const int MaximumAutomationLevel = 100;
	    public const int DefaultAutomationLevel = 100;

	    public const int MinimumGroupLevel = 0;
	    public const int MaximumGroupLevel = 100;
	    public const int DefaultGroupLevel = 100;
	    public const int DefaultSoundEffectLevel = 80;
	    public const int DefaultVoiceLevel = 90;
	    public const int DefaultSongPreviewLevel = 75;
	    public const int DefaultSongPlaybackLevel = 90;

	    public static readonly Lufs MinimumLufs = new Lufs(-100.0);
	    public static readonly Lufs MaximumLufs = new Lufs(10.0); // support an approximate doubling in volume.

	    private static readonly Lufs DefaultGain = new Lufs(0.0);

	    public readonly SoundGroup SoundGroup;

		#region [ DTXMania用拡張 ]

		public int TotalPlayTimeMs
		{
			get;
			private set;
		}
		public int nサウンドバッファサイズ		// 取りあえず0固定★★★★★★★★★★★★★★★★★★★★
		{
			get { return 0; }
		}
		public bool bストリーム再生する			// 取りあえずfalse固定★★★★★★★★★★★★★★★★★★★★
												// trueにすると同一チップ音の多重再生で問題が出る(4POLY音源として動かない)
		{
			get { return false; }
		}
		public double db周波数倍率
		{
			get
			{
				return _FrequencyMultiplier;
			}
			set
			{
				if ( _FrequencyMultiplier != value )
				{
					_FrequencyMultiplier = value;
					if ( IsBassSound )
					{
						Bass.BASS_ChannelSetAttribute( this._BassStreamHandle, BASSAttribute.BASS_ATTRIB_FREQ, ( float ) ( _FrequencyMultiplier * PlaySpeed * OriginalFrequency ) );
					}
					else
					{
//						if ( b再生中 )	// #30838 2012.2.24 yyagi (delete b再生中)
//						{
							this.Buffer.Frequency = ( int ) ( _FrequencyMultiplier * PlaySpeed * OriginalFrequency );
//						}
					}
				}
			}
		}
		public double PlaySpped
		{
			get
			{
				return PlaySpeed;
			}
			set
			{
				if ( PlaySpeed != value )
				{
					PlaySpeed = value;
					IsUniformVelocity = ( PlaySpeed == 1.000f );
					if ( IsBassSound )
					{
						if ( TempoStreamHandle != 0 && !this.IsUniformVelocity )	// 再生速度がx1.000のときは、TempoStreamを用いないようにして高速化する
				        {
							this._BassStreamHandle = TempoStreamHandle;
				        }
				        else
						{
							this._BassStreamHandle = BassStreamHandle;
				        }

						if ( SoundManager.bIsTimeStretch )
						{
							Bass.BASS_ChannelSetAttribute( this._BassStreamHandle, BASSAttribute.BASS_ATTRIB_TEMPO, (float) ( PlaySpped * 100 - 100 ) );
							//double seconds = Bass.BASS_ChannelBytes2Seconds( this.hTempoStream, nBytes );
							//this.n総演奏時間ms = (int) ( seconds * 1000 );
						}
						else
						{
							Bass.BASS_ChannelSetAttribute( this._BassStreamHandle, BASSAttribute.BASS_ATTRIB_FREQ, ( float ) ( _FrequencyMultiplier * PlaySpeed * OriginalFrequency ) );
						}
					}
					else
					{
//						if ( b再生中 )	// #30838 2012.2.24 yyagi (delete b再生中)
//						{
							this.Buffer.Frequency = ( int ) ( _FrequencyMultiplier * PlaySpeed * OriginalFrequency );
//						}
					}
				}
			}
		}
		#endregion

		public bool IsContinueToPlayAfterTheEnd = false;	// これがtrueなら、本サウンドの再生終了のコールバック時に自動でミキサーから削除する

		//private STREAMPROC _cbStreamXA;		// make it global, so that the GC can not remove it
		private SYNCPROC _cbEndofStream;	// ストリームの終端まで再生されたときに呼び出されるコールバック
//		private WaitCallback _cbRemoveMixerChannel;

	    /// <summary>
	    /// Gain is applied "first" to the audio data, much as in a physical or
	    /// software mixer. Later steps in the flow of audio apply "channel" level
	    /// (e.g. AutomationLevel) and mixing group level (e.g. GroupLevel) before
	    /// the audio is output.
	    /// 
	    /// This method, taking an integer representing a percent value, is used
	    /// for mixing in the SONGVOL value, when available. It is also used for
	    /// DTXViewer preview mode.
	    /// </summary>
	    public void SetGain(int songVol)
	    {
	        SetGain(LinearIntegerPercentToLufs(songVol), null);
	    }

	    private static Lufs LinearIntegerPercentToLufs(int percent)
	    {
	        // 2018-08-27 twopointzero: We'll use the standard conversion until an appropriate curve can be selected
	        return new Lufs(20.0 * Math.Log10(percent / 100.0));
	    }

	    /// <summary>
	    /// Gain is applied "first" to the audio data, much as in a physical or
	    /// software mixer. Later steps in the flow of audio apply "channel" level
	    /// (e.g. AutomationLevel) and mixing group level (e.g. GroupLevel) before
	    /// the audio is output.
	    /// 
	    /// This method, taking a LUFS gain value and a LUFS true audio peak value,
	    /// is used for mixing in the loudness-metadata-base gain value, when available.
	    /// </summary>
	    public void SetGain(Lufs gain, Lufs? truePeak)
	    {
	        if (Equals(_gain, gain))
	        {
	            return;
	        }

	        _gain = gain;
	        _truePeak = truePeak;

	        if (SoundGroup == SoundGroup.SongPlayback)
	        {
	            Trace.TraceInformation($"{nameof(FDKSound)}.{nameof(SetGain)}: Gain: {_gain}. True Peak: {_truePeak}");
	        }

	        SetVolume();
	    }

	    /// <summary>
	    /// AutomationLevel is applied "second" to the audio data, much as in a
	    /// physical or sofware mixer and its channel level. Before this Gain is
	    /// applied, and after this the mixing group level is applied.
	    ///
	    /// This is currently used only for automated fade in and out as is the
	    /// case right now for the song selection screen background music fade
	    /// in and fade out.
	    /// </summary>
	    public int AutomationLevel
	    {
	        get => _automationLevel;
	        set
	        {
	            if (_automationLevel == value)
	            {
	                return;
	            }

	            _automationLevel = value;

	            if (SoundGroup == SoundGroup.SongPlayback)
	            {
	                Trace.TraceInformation($"{nameof(FDKSound)}.{nameof(AutomationLevel)} set: {AutomationLevel}");
	            }

	            SetVolume();
	        }
	    }

	    /// <summary>
	    /// GroupLevel is applied "third" to the audio data, much as in the sub
	    /// mixer groups of a physical or software mixer. Before this both the
	    /// Gain and AutomationLevel are applied, and after this the audio
	    /// flows into the audio subsystem for mixing and output based on the
	    /// master volume.
	    ///
	    /// This is currently automatically managed for each sound based on the
	    /// configured and dynamically adjustable sound group levels for each of
	    /// sound effects, voice, song preview, and song playback.
	    ///
	    /// See the SoundGroupLevelController and related classes for more.
	    /// </summary>
	    public int GroupLevel
	    {
	        private get => _groupLevel;
	        set
	        {
	            if (_groupLevel == value)
	            {
	                return;
	            }

	            _groupLevel = value;

	            if (SoundGroup == SoundGroup.SongPlayback)
	            {
	                Trace.TraceInformation($"{nameof(FDKSound)}.{nameof(GroupLevel)} set: {GroupLevel}");
	            }

	            SetVolume();
	        }
	    }

	    private void SetVolume()
	    {
	        var automationLevel = LinearIntegerPercentToLufs(AutomationLevel);
	        var groupLevel = LinearIntegerPercentToLufs(GroupLevel);

	        var gain =
	            _gain +
	            automationLevel +
	            groupLevel;

	        var safeTruePeakGain = _truePeak?.Negate() ?? new Lufs(0);
	        var finalGain = gain.Min(safeTruePeakGain);

	        if (SoundGroup == SoundGroup.SongPlayback)
	        {
	            Trace.TraceInformation(
	                $"{nameof(FDKSound)}.{nameof(SetVolume)}: Gain:{_gain}. Automation Level: {automationLevel}. Group Level: {groupLevel}. Summed Gain: {gain}. Safe True Peak Gain: {safeTruePeakGain}. Final Gain: {finalGain}.");
	        }

	        LufsVolume = finalGain;
	    }

	    private Lufs LufsVolume
	    {
	        set
	        {
	            if (this.IsBassSound)
	            {
	                var db音量 = ((value.ToDouble() / 100.0) + 1.0).Clamp(0, 1);
	                Bass.BASS_ChannelSetAttribute(this._BassStreamHandle, BASSAttribute.BASS_ATTRIB_VOL, (float) db音量);
	            }
	            else if (this.IsDirectSound)
	            {
	                var db音量 = (value.ToDouble() * 100.0).Clamp(-10000, 0);
	                this.Buffer.Volume = (int) Math.Round(db音量);
	            }
	        }
	    }

		/// <summary>
		/// <para>左:-100～中央:0～100:右。set のみ。</para>
		/// </summary>
		public int NowPan
		{
			get
			{
				if( this.IsBassSound )
				{
					float pan = 0.0f;
					if ( !Bass.BASS_ChannelGetAttribute( this._BassStreamHandle, BASSAttribute.BASS_ATTRIB_PAN, ref pan ) )
						//if( BassMix.BASS_Mixer_ChannelGetEnvelopePos( this.hBassStream, BASSMIXEnvelope.BASS_MIXER_ENV_PAN, ref f位置 ) == -1 )
						return 0;
					return (int) ( pan * 100 );
				}
				else if( this.IsDirectSound )
				{
					return this._NowPan;
				}
				return -9999;
			}
			set
			{
				if( this.IsBassSound )
				{
					float f位置 = Math.Min( Math.Max( value, -100 ), 100 ) / 100.0f;	// -100～100 → -1.0～1.0
					//var nodes = new BASS_MIXER_NODE[ 1 ] { new BASS_MIXER_NODE( 0, f位置 ) };
					//BassMix.BASS_Mixer_ChannelSetEnvelope( this.hBassStream, BASSMIXEnvelope.BASS_MIXER_ENV_PAN, nodes );
					Bass.BASS_ChannelSetAttribute( this._BassStreamHandle, BASSAttribute.BASS_ATTRIB_PAN, f位置 );
				}
				else if( this.IsDirectSound )
				{
					this._NowPan = Math.Min( Math.Max( -100, value ), 100 );		// -100～100

					if( this._NowPan == 0 )
					{
						this._NowPan_Double = 0;
					}
					else if( this._NowPan == -100 )
					{
						this._NowPan_Double = -10000;
					}
					else if( this._NowPan == 100 )
					{
						this._NowPan_Double = 10000;
					}
					else if( this._NowPan < 0 )
					{
						this._NowPan_Double = (int) ( ( 20.0 * Math.Log10( ( (double) ( this._NowPan + 100 ) ) / 100.0 ) ) * 100.0 );
					}
					else
					{
						this._NowPan_Double = (int) ( ( -20.0 * Math.Log10( ( (double) ( 100 - this._NowPan ) ) / 100.0 ) ) * 100.0 );
					}

					this.Buffer.Pan = this._NowPan_Double;
				}
			}
		}

		/// <summary>
		/// <para>DirectSoundのセカンダリバッファ。</para>
		/// </summary>
		//public SecondarySoundBuffer DirectSoundBuffer
		public SoundBuffer DirectSoundBuffer
		{
			get { return this.Buffer; }
		}

		/// <summary>
		/// <para>DirectSoundのセカンダリバッファ作成時のフラグ。</para>
		/// </summary>
		public BufferFlags DirectSoundBufferFlags
		{
			get;
			protected set;
		}

		/// <summary>
		/// <para>全インスタンスリスト。</para>
		/// <para>～を作成する() で追加され、t解放する() or Dispose() で解放される。</para>
		/// </summary>
		public static readonly ObservableCollection<FDKSound> FDKSounds = new ObservableCollection<FDKSound>();

		public static void ShowAllCSoundFiles()
		{
			int i = 0;
			foreach ( FDKSound cs in FDKSounds )
			{
				Debug.WriteLine( i++.ToString( "d3" ) + ": " + Path.GetFileName( cs.FilePath ) );
			}
		}

		public FDKSound(SoundGroup soundGroup)
		{
		    SoundGroup = soundGroup;
			this.NowPan = 0;
			this._FrequencyMultiplier = 1.0;
			this.PlaySpeed = 1.0;
			this.DirectSoundBufferFlags = SoundDeviceDirectSound.DefaultFlags;
//			this._cbRemoveMixerChannel = new WaitCallback( RemoveMixerChannelLater );
			this.BassStreamHandle = -1;
			this.TempoStreamHandle = 0;
		}

		public void CreateASIOSound( string filePath, int hMixer )
		{
		    this.NowSoundDeviceType = SoundDeviceType.ASIO;		// 作成後に設定する。（作成に失敗してると例外発出されてここは実行されない）
			this.CreateBASSSound( filePath, hMixer, BASSFlag.BASS_STREAM_DECODE );
		}
		public void CreateASIOSound( byte[] wavFileImage, int hMixer )
		{
		    this.NowSoundDeviceType = SoundDeviceType.ASIO;		// 作成後に設定する。（作成に失敗してると例外発出されてここは実行されない）
			this.CreateBASSSound( wavFileImage, hMixer, BASSFlag.BASS_STREAM_DECODE );
		}
		public void CreateWASAPISound( string filePath, int hMixer, SoundDeviceType soundDevieType )
		{
		    this.NowSoundDeviceType = soundDevieType;		// 作成後に設定する。（作成に失敗してると例外発出されてここは実行されない）
			this.CreateBASSSound( filePath, hMixer, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT );
		}
		public void CreateWASAPISound( byte[] wavFileImage, int hMixer, SoundDeviceType soundDevieType )
		{
		    this.NowSoundDeviceType = soundDevieType;		// 作成後に設定する。（作成に失敗してると例外発出されてここは実行されない）
			this.CreateBASSSound( wavFileImage, hMixer, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT );
		}
		public void CreateDirectSoundInstance( string filePath, DirectSound DirectSound )
		{
			this.NowCreateType = CreateSoundType.File;
			this.FilePath = filePath;
			if ( String.Compare( Path.GetExtension( filePath ), ".xa", true ) == 0 ||
				 String.Compare( Path.GetExtension( filePath ), ".mp3", true ) == 0 ||
				 String.Compare( Path.GetExtension( filePath ), ".ogg", true ) == 0 )	// caselessで文字列比較
			{
				CreateDirectSoundFromXaOggMp3( filePath, DirectSound );
				return;
			}

			// すべてのファイルを DirectShow でデコードすると時間がかかるので、ファイルが WAV かつ PCM フォーマットでない場合のみ DirectShow でデコードする。

			byte[] wavFileImage = null;
			bool wavAndPCMFormat = true;

			{
				#region [ ファイルがWAVかつPCMフォーマットか否か調べる。]
				//-----------------
				try
				{
					using ( var ws = new WaveStream( filePath ) )
					{
						if ( ws.Format.FormatTag != WaveFormatTag.Pcm )
							wavAndPCMFormat = false;
					}
				}
				catch
				{
					wavAndPCMFormat = false;
				}
				//-----------------
				#endregion

				if ( wavAndPCMFormat )
				{
					#region [ ファイルを読み込んで byArrWAVファイルイメージへ格納。]
					//-----------------
					var fs = File.Open( filePath, FileMode.Open, FileAccess.Read );
					var br = new BinaryReader( fs );

					wavFileImage = new byte[ fs.Length ];
					br.Read( wavFileImage, 0, (int) fs.Length );

					br.Close();
					fs.Close();
					//-----------------
					#endregion
				}
				else
				{
					#region [ DirectShow でデコード変換し、 byArrWAVファイルイメージへ格納。]
					//-----------------
					DStoWAVFileImage.t変換( filePath, out wavFileImage );
					//-----------------
					#endregion
				}
			}

			// あとはあちらで。

			this.CreateDirectSoundInstance( wavFileImage, DirectSound );
		}
		public void CreateDirectSoundFromXaOggMp3( string filePath, DirectSound DirectSound )
		{
			this.NowCreateType = CreateSoundType.File;
			this.FilePath = filePath;


			WaveFormat wfx = new WaveFormat();
			int nPCMデータの先頭インデックス = 0;
//			int nPCMサイズbyte = (int) ( xa.xaheader.nSamples * xa.xaheader.nChannels * 2 );	// nBytes = Bass.BASS_ChannelGetLength( this.hBassStream );

			int nPCMサイズbyte;
			Win32Manager.WAVEFORMATEX cw32wfx;
			DecodeWithOnMemory( filePath, out this.WAVImages,
			out nPCMデータの先頭インデックス, out nPCMサイズbyte, out cw32wfx, false );

			wfx.AverageBytesPerSecond = (int) cw32wfx.nAvgBytesPerSec;
			wfx.BitsPerSample = (short) cw32wfx.wBitsPerSample;
			wfx.BlockAlignment = (short) cw32wfx.nBlockAlign;
			wfx.Channels = (short) cw32wfx.nChannels;
			wfx.FormatTag = WaveFormatTag.Pcm;	// xa.waveformatex.wFormatTag;
			wfx.SamplesPerSecond = (int) cw32wfx.nSamplesPerSec;

			// セカンダリバッファを作成し、PCMデータを書き込む。
			tDirectSoundサウンドを作成する_セカンダリバッファの作成とWAVデータ書き込み
				( ref this.WAVImages, DirectSound, SoundDeviceDirectSound.DefaultFlags, wfx,
				  nPCMサイズbyte, nPCMデータの先頭インデックス );
		}

		public void CreateDirectSoundInstance( byte[] byArrWAVファイルイメージ, DirectSound DirectSound )
		{
			this.CreateDirectSoundInstance(  byArrWAVファイルイメージ, DirectSound, SoundDeviceDirectSound.DefaultFlags );
		}
		public void CreateDirectSoundInstance( byte[] byArrWAVファイルイメージ, DirectSound DirectSound, BufferFlags flags )
		{
			if( this.NowCreateType == CreateSoundType.Unknown )
				this.NowCreateType = CreateSoundType.WavImage;

			WaveFormat wfx = null;
			int nPCMデータの先頭インデックス = -1;
			int nPCMサイズbyte = -1;
	
			#region [ byArrWAVファイルイメージ[] から上記３つのデータを取得。]
			//-----------------
			var ms = new MemoryStream( byArrWAVファイルイメージ );
			var br = new BinaryReader( ms );

			try
			{
				// 'RIFF'＋RIFFデータサイズ

				if( br.ReadUInt32() != 0x46464952 )
					throw new InvalidDataException( "RIFFファイルではありません。" );
				br.ReadInt32();

				// 'WAVE'
				if( br.ReadUInt32() != 0x45564157 )
					throw new InvalidDataException( "WAVEファイルではありません。" );

				// チャンク
				while( ( ms.Position + 8 ) < ms.Length )	// +8 は、チャンク名＋チャンクサイズ。残り8バイト未満ならループ終了。
				{
					uint chunkName = br.ReadUInt32();

					// 'fmt '
					if( chunkName == 0x20746D66 )
					{
						long chunkSize = (long) br.ReadUInt32();

						var tag = (WaveFormatTag) br.ReadUInt16();

						if( tag == WaveFormatTag.Pcm ) wfx = new WaveFormat();
						else if( tag == WaveFormatTag.Extensible ) wfx = new SlimDX.Multimedia.WaveFormatExtensible();	// このクラスは WaveFormat を継承している。
						else
							throw new InvalidDataException( string.Format( "未対応のWAVEフォーマットタグです。(Tag:{0})", tag.ToString() ) );

						wfx.FormatTag = tag;
						wfx.Channels = br.ReadInt16();
						wfx.SamplesPerSecond = br.ReadInt32();
						wfx.AverageBytesPerSecond = br.ReadInt32();
						wfx.BlockAlignment = br.ReadInt16();
						wfx.BitsPerSample = br.ReadInt16();

						long nフォーマットサイズbyte = 16;

						if( wfx.FormatTag == WaveFormatTag.Extensible )
						{
							br.ReadUInt16();	// 拡張領域サイズbyte
							var wfxEx = (SlimDX.Multimedia.WaveFormatExtensible) wfx;
							wfxEx.ValidBitsPerSample = br.ReadInt16();
							wfxEx.ChannelMask = (Speakers) br.ReadInt32();
							wfxEx.SubFormat = new Guid( br.ReadBytes( 16 ) );	// GUID は 16byte (128bit)

							nフォーマットサイズbyte += 24;
						}

						ms.Seek( chunkSize - nフォーマットサイズbyte, SeekOrigin.Current );
						continue;
					}

					// 'data'
					else if( chunkName == 0x61746164 )
					{
						nPCMサイズbyte = br.ReadInt32();
						nPCMデータの先頭インデックス = (int) ms.Position;

						ms.Seek( nPCMサイズbyte, SeekOrigin.Current );
						continue;
					}

					// その他
					else
					{
						long chunkSize = (long) br.ReadUInt32();
						ms.Seek( chunkSize, SeekOrigin.Current );
						continue;
					}
				}

				if( wfx == null )
					throw new InvalidDataException( "fmt チャンクが存在しません。不正なサウンドデータです。" );
				if( nPCMサイズbyte < 0 )
					throw new InvalidDataException( "data チャンクが存在しません。不正なサウンドデータです。" );
			}
			finally
			{
				ms.Close();
				br.Close();
			}
			//-----------------
			#endregion


			// セカンダリバッファを作成し、PCMデータを書き込む。
			tDirectSoundサウンドを作成する_セカンダリバッファの作成とWAVデータ書き込み(
				ref byArrWAVファイルイメージ, DirectSound, flags, wfx, nPCMサイズbyte, nPCMデータの先頭インデックス );
		}

		private void tDirectSoundサウンドを作成する_セカンダリバッファの作成とWAVデータ書き込み
			( ref byte[] byArrWAVファイルイメージ, DirectSound DirectSound, BufferFlags flags, WaveFormat wfx,
			int nPCMサイズbyte, int nPCMデータの先頭インデックス )
		{
			// セカンダリバッファを作成し、PCMデータを書き込む。

			this.Buffer = new SecondarySoundBuffer( DirectSound, new SoundBufferDescription()
			{
				Format = ( wfx.FormatTag == WaveFormatTag.Pcm ) ? wfx : (SlimDX.Multimedia.WaveFormatExtensible) wfx,
				Flags = flags,
				SizeInBytes = nPCMサイズbyte,
			} );
			this.Buffer.Write( byArrWAVファイルイメージ, nPCMデータの先頭インデックス, nPCMサイズbyte, 0, LockFlags.None );

			// 作成完了。

			this.NowSoundDeviceType = SoundDeviceType.DirectSound;
			this.DirectSoundBufferFlags = flags;
			this.WAVImages = byArrWAVファイルイメージ;
			this.DirectSound = DirectSound;

			// DTXMania用に追加
			this.OriginalFrequency = wfx.SamplesPerSecond;
			TotalPlayTimeMs = (int) ( ( (double) nPCMサイズbyte ) / ( this.Buffer.Format.AverageBytesPerSecond * 0.001 ) );


			// インスタンスリストに登録。

			FDKSound.FDKSounds.Add( this );
		}

		#region [ DTXMania用の変換 ]

		public void DisposeSound( FDKSound cs )
		{
			cs.DisposeSound();
		}
		public void t再生を開始する()
		{
			SetPlayPositionTheBegin();
			PlaySound();
		}
		public void t再生を開始する( bool bループする )
		{
			if ( IsBassSound )
			{
				if ( bループする )
				{
					Bass.BASS_ChannelFlags( this._BassStreamHandle, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP );
				}
				else
				{
					Bass.BASS_ChannelFlags( this._BassStreamHandle, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_DEFAULT );
				}
			}
			SetPlayPositionTheBegin();
			PlaySound( bループする );
		}
		public void t再生を停止する()
		{
			StopFDKSound();
			SetPlayPositionTheBegin();
		}
		public void t再生を一時停止する()
		{
			StopSound(true);
			this.PauseCount++;
		}
		public void t再生を再開する( long t )	// ★★★★★★★★★★★★★★★★★★★★★★★★★★★★
		{
			Debug.WriteLine( "t再生を再開する(long " + t + ")" );
			SetPosition( t );
			PlaySound();
			this.PauseCount--;
		}
		public bool b一時停止中
		{
			get
			{
				if ( this.IsBassSound )
				{
					bool ret = ( BassMix.BASS_Mixer_ChannelIsActive( this._BassStreamHandle ) == BASSActive.BASS_ACTIVE_PAUSED ) &
								( BassMix.BASS_Mixer_ChannelGetPosition( this._BassStreamHandle ) > 0 );
					return ret;
				}
				else
				{
					return ( this.PauseCount > 0 );
				}
			}
		}
		public bool b再生中
		{
			get
			{
				if ( this.NowSoundDeviceType == SoundDeviceType.DirectSound )
				{
					return ( ( this.Buffer.Status & BufferStatus.Playing ) != BufferStatus.None );
				}
				else
				{
					// 基本的にはBASS_ACTIVE_PLAYINGなら再生中だが、最後まで再生しきったchannelも
					// BASS_ACTIVE_PLAYINGのままになっているので、小細工が必要。
					bool ret = ( BassMix.BASS_Mixer_ChannelIsActive( this._BassStreamHandle ) == BASSActive.BASS_ACTIVE_PLAYING );
					if ( BassMix.BASS_Mixer_ChannelGetPosition( this._BassStreamHandle ) >= nBytes )
					{
						ret = false;
					}
					return ret;
				}
			}
		}
		//public lint t時刻から位置を返す( long t )
		//{
		//    double num = ( n時刻 * this.db再生速度 ) * this.db周波数倍率;
		//    return (int) ( ( num * 0.01 ) * this.nSamplesPerSecond );
		//}
		#endregion


		public void DisposeSound()
		{
			DisposeSound( false );
		}

		public void DisposeSound( bool isRemoveInstance )
		{
			if ( this.IsBassSound )		// stream数の削減用
			{
				RemoveBassSoundTheMixer();
				_cbEndofStream = null;
				//_cbStreamXA = null;
				SoundManager.nStreams--;
			}
			bool isDisposeManaged = true;
			bool _isRemoveInstance = isRemoveInstance;	// CSoundの再初期化時は、インスタンスは存続する。
			this.Dispose( isDisposeManaged, _isRemoveInstance );
//Debug.WriteLine( "Disposed: " + _bインスタンス削除 + " : " + Path.GetFileName( this.strファイル名 ) );
		}
		public void PlaySound()
		{
			PlaySound( false );
		}
		private void PlaySound( bool loop )
		{
			if ( this.IsBassSound )			// BASSサウンド時のループ処理は、t再生を開始する()側に実装。ここでは「bループする」は未使用。
			{
//Debug.WriteLine( "再生中?: " +  System.IO.Path.GetFileName(this.strファイル名) + " status=" + BassMix.BASS_Mixer_ChannelIsActive( this.hBassStream ) + " current=" + BassMix.BASS_Mixer_ChannelGetPosition( this.hBassStream ) + " nBytes=" + nBytes );
				bool b = BassMix.BASS_Mixer_ChannelPlay( this._BassStreamHandle );
				if ( !b )
				{
//Debug.WriteLine( "再生しようとしたが、Mixerに登録されていなかった: " + Path.GetFileName( this.strファイル名 ) + ", stream#=" + this.hBassStream + ", ErrCode=" + Bass.BASS_ErrorGetCode() );

					bool bb = ADDBassSoundToTheMixer();
					if ( !bb )
					{
Debug.WriteLine( "Mixerへの登録に失敗: " + Path.GetFileName( this.FilePath ) + ", ErrCode=" + Bass.BASS_ErrorGetCode() );
					}
					else
					{
//Debug.WriteLine( "Mixerへの登録に成功: " + Path.GetFileName( this.strファイル名 ) + ": " + Bass.BASS_ErrorGetCode() );
					}
					//this.t再生位置を先頭に戻す();

					bool bbb = BassMix.BASS_Mixer_ChannelPlay( this._BassStreamHandle );
					if (!bbb)
					{
Debug.WriteLine("更に再生に失敗: " + Path.GetFileName(this.FilePath) + ", ErrCode=" + Bass.BASS_ErrorGetCode() );
					}
					else
					{
//						Debug.WriteLine("再生成功(ミキサー追加後)                       : " + Path.GetFileName(this.strファイル名));
					}
				}
				else
				{
//Debug.WriteLine( "再生成功: " + Path.GetFileName( this.strファイル名 ) + " (" + hBassStream + ")" );
				}
			}
			else if( this.IsDirectSound )
			{
				PlayFlags pf = ( loop ) ? PlayFlags.Looping : PlayFlags.None;
				this.Buffer.Play( 0, pf );
			}
		}
		public void PlaySoundFromBegin()
		{
			this.SetPlayPositionTheBegin();
			this.PlaySound();
		}
		public void StopSoundAndRemoveMixer()
		{
			StopSound( false );
			if ( IsBassSound )
			{
				RemoveBassSoundTheMixer();
			}
		}
		public void StopFDKSound()
		{
			StopSound( false );
		}
		public void StopSound( bool pause )
		{
			if( this.IsBassSound )
			{
//Debug.WriteLine( "停止: " + System.IO.Path.GetFileName( this.strファイル名 ) + " status=" + BassMix.BASS_Mixer_ChannelIsActive( this.hBassStream ) + " current=" + BassMix.BASS_Mixer_ChannelGetPosition( this.hBassStream ) + " nBytes=" + nBytes );
				BassMix.BASS_Mixer_ChannelPause( this._BassStreamHandle );
				if ( !pause )
				{
			//		tBASSサウンドをミキサーから削除する();		// PAUSEと再生停止を区別できるようにすること!!
				}
			}
			else if( this.IsDirectSound )
			{
				try
				{
					this.Buffer.Stop();
				}
				catch ( Exception )
				{
					// WASAPI/ASIOとDirectSoundを同時使用すると、Bufferがlostしてここで例外発生する。→ catchして無視する。
					// DTXCからDTXManiaを呼び出すと、DTXC終了時にこの現象が発生する。
				}
			}
			this.PauseCount = 0;
		}
		
		public void SetPlayPositionTheBegin()
		{
			if( this.IsBassSound )
			{
				BassMix.BASS_Mixer_ChannelSetPosition( this._BassStreamHandle, 0 );
				//pos = 0;
			}
			else if( this.IsDirectSound )
			{
				this.Buffer.CurrentPlayPosition = 0;
			}
		}
		public void SetPosition( long positionMs )
		{
			if( this.IsBassSound )
			{
				bool b = true;
				try
				{
					b = BassMix.BASS_Mixer_ChannelSetPosition( this._BassStreamHandle, Bass.BASS_ChannelSeconds2Bytes( this._BassStreamHandle, positionMs * this.db周波数倍率 * this.PlaySpped / 1000.0 ), BASSMode.BASS_POS_BYTES );
				}
				catch( Exception e )
				{
					Trace.TraceError( e.ToString() );
					Trace.TraceInformation( Path.GetFileName( this.FilePath ) + ": Seek error: " + e.ToString() + ": " + positionMs + "ms" );
				}
				finally
				{
					if ( !b )
					{
						BASSError be = Bass.BASS_ErrorGetCode();
						Trace.TraceInformation( Path.GetFileName( this.FilePath ) + ": Seek error: " + be.ToString() + ": " + positionMs + "MS" );
					}
				}
				//if ( this.n総演奏時間ms > 5000 )
				//{
				//    Trace.TraceInformation( Path.GetFileName( this.strファイル名 ) + ": Seeked to " + n位置ms + "ms = " + Bass.BASS_ChannelSeconds2Bytes( this.hBassStream, n位置ms * this.db周波数倍率 * this.db再生速度 / 1000.0 ) );
				//}
			}
			else if( this.IsDirectSound )
			{
				int positionSample = (int) ( this.Buffer.Format.SamplesPerSecond * positionMs * 0.001 * _FrequencyMultiplier * PlaySpeed );	// #30839 2013.2.24 yyagi; add _db周波数倍率 and _db再生速度
				try
				{
					this.Buffer.CurrentPlayPosition = positionSample * this.Buffer.Format.BlockAlignment;
				}
				catch ( DirectSoundException e )
				{
					Trace.TraceError( "{0}: Seek error: {1}", Path.GetFileName( this.FilePath ), positionMs);
					Trace.TraceError( e.ToString() );
					Trace.TraceError( "例外が発生しましたが処理を継続します。 (95dee242-1f92-4fcf-aaf6-b162ad2bfc03)" );
				}
				//if ( this.n総演奏時間ms > 5000 )
				//{
				//    Trace.TraceInformation( Path.GetFileName( this.strファイル名 ) + ": Seeked to " + n位置ms + "ms = " + n位置sample );
				//}
			}
		}
		/// <summary>
		/// デバッグ用
		/// </summary>
		/// <param name="positionByte"></param>
		/// <param name="positionMs"></param>
		public void GetPlayPosition( out long positionByte, out double positionMs )
		{
			if ( this.IsBassSound )
			{
				positionByte = BassMix.BASS_Mixer_ChannelGetPosition( this._BassStreamHandle );
				positionMs = Bass.BASS_ChannelBytes2Seconds( this._BassStreamHandle, positionByte );
			}
			else if ( this.IsDirectSound )
			{
				positionByte = this.Buffer.CurrentPlayPosition;
				positionMs = positionByte / this.Buffer.Format.SamplesPerSecond / 0.001 / _FrequencyMultiplier / PlaySpeed;
			}
			else
			{
				positionByte = 0;
				positionMs = 0.0;
			}
		}


		public static void DisposeAllSoundButKeepInstance()
		{
			foreach ( var sound in FDKSound.FDKSounds )
			{
				sound.DisposeSound( false );
			}
		}
		internal static void RebuildAllExistingSounds( ISoundDevice device )
		{
			if( FDKSound.FDKSounds.Count == 0 )
				return;


			// サウンドを再生する際にインスタンスリストも更新されるので、配列にコピーを取っておき、リストはクリアする。

			var sounds = FDKSound.FDKSounds.ToArray();
			FDKSound.FDKSounds.Clear();
			

			// 配列に基づいて個々のサウンドを作成する。

			for( int i = 0; i < sounds.Length; i++ )
			{
				switch( sounds[ i ].NowCreateType )
				{
					#region [ ファイルから ]
					case CreateSoundType.File:
						string filePath = sounds[ i ].FilePath;
						sounds[ i ].Dispose( true, false );
						device.CreateFDKSound( filePath, sounds[ i ] );
						break;
					#endregion
					#region [ WAVファイルイメージから ]
					case CreateSoundType.WavImage:
						if( sounds[ i ].IsBassSound )
						{
							byte[] wavImage = sounds[ i ].WAVImages;
							sounds[ i ].Dispose( true, false );
							device.CreateFDKSound( wavImage, sounds[ i ] );
						}
						else if( sounds[ i ].IsDirectSound )
						{
							byte[] wavImage = sounds[ i ].WAVImages;
							var flags = sounds[ i ].DirectSoundBufferFlags;
							sounds[ i ].Dispose( true, false );
							( (SoundDeviceDirectSound) device ).CreateFDKSound( wavImage, flags, sounds[ i ] );
						}
						break;
					#endregion
				}
			}
		}

		#region [ Dispose-Finalizeパターン実装 ]
		//-----------------
		public void Dispose()
		{
			this.Dispose( true, true );
			GC.SuppressFinalize( this );
		}
		private void Dispose( bool isDisposeManaged, bool isRemoveInstance )
		{
			if( this.IsBassSound )
			{
				#region [ ASIO, WASAPI の解放 ]
				//-----------------
				if ( TempoStreamHandle != 0 )
				{
					BassMix.BASS_Mixer_ChannelRemove( this.TempoStreamHandle );
					Bass.BASS_StreamFree( this.TempoStreamHandle );
				}
				BassMix.BASS_Mixer_ChannelRemove( this.BassStreamHandle );
				Bass.BASS_StreamFree( this.BassStreamHandle );
				this._BassStreamHandle = -1;
				this.BassStreamHandle = -1;
				this.TempoStreamHandle = 0;
				//-----------------
				#endregion
			}

			if( isDisposeManaged )
			{
				//int freeIndex = -1;

				//if ( CSound.listインスタンス != null )
				//{
				//    freeIndex = CSound.listインスタンス.IndexOf( this );
				//    if ( freeIndex == -1 )
				//    {
				//        Debug.WriteLine( "ERR: freeIndex==-1 : Count=" + CSound.listインスタンス.Count + ", filename=" + Path.GetFileName( this.strファイル名 ) );
				//    }
				//}

				if( this.NowSoundDeviceType == SoundDeviceType.DirectSound )
				{
					#region [ DirectSound の解放 ]
					//-----------------
					if( this.Buffer != null )
					{
						try
						{
							this.Buffer.Stop();
						}
						catch (Exception e)
						{
							// 演奏終了後、長時間解放しないでいると、たまに AccessViolationException が発生することがある。
							Trace.TraceError( e.ToString() );
							Trace.TraceError( "例外が発生しましたが処理を継続します。 (19bcaa24-5259-4198-bf74-41eb1114ba28)" );
						}
						FDKCommon.Dispose( ref this.Buffer );
					}
					//-----------------
					#endregion
				}

				if( this.NowCreateType == CreateSoundType.WavImage &&
					this.NowSoundDeviceType != SoundDeviceType.DirectSound )	// DirectSound は hGC 未使用。
				{
					if ( this.hGC != null && this.hGC.IsAllocated )
					{
						this.hGC.Free();
						this.hGC = default( GCHandle );
					}
				}
				if ( this.WAVImages != null )
				{
					this.WAVImages = null;
				}

			    this.NowSoundDeviceType = SoundDeviceType.Unknown;

				if ( isRemoveInstance )
				{
					//try
					//{
					//    CSound.listインスタンス.RemoveAt( freeIndex );
					//}
					//catch
					//{
					//    Debug.WriteLine( "FAILED to remove CSound.listインスタンス: Count=" + CSound.listインスタンス.Count + ", filename=" + Path.GetFileName( this.strファイル名 ) );
					//}
					bool b = FDKSound.FDKSounds.Remove( this );	// これだと、Clone()したサウンドのremoveに失敗する
					if ( !b )
					{
						Debug.WriteLine( "FAILED to remove CSound.listインスタンス: Count=" + FDKSound.FDKSounds.Count + ", filename=" + Path.GetFileName( this.FilePath ) );
					}

				}
			}
		}
		~FDKSound()
		{
			this.Dispose( false, true );
		}
		//-----------------
		#endregion

		#region [ protected ]
		//-----------------
		protected enum CreateSoundType { File, WavImage, Unknown }
		protected CreateSoundType NowCreateType = CreateSoundType.Unknown;
		protected SoundDeviceType NowSoundDeviceType = SoundDeviceType.Unknown;
		public string FilePath = null;
		protected byte[] WAVImages = null;	// WAVファイルイメージ、もしくはchunkのDATA部のみ
		protected GCHandle hGC;
		protected int TempoStreamHandle = 0;
		protected int BassStreamHandle = -1;					// ASIO, WASAPI 用
		protected int _BassStreamHandle = 0;						// #31076 2013.4.1 yyagi; プロパティとして実装すると動作が低速になったため、
															// tBASSサウンドを作成する_ストリーム生成後の共通処理()のタイミングと、
															// 再生速度を変更したタイミングでのみ、
															// hBassStreamを更新するようにした。
		//{
		//    get
		//    {
		//        if ( _hTempoStream != 0 && !this.bIs1倍速再生 )	// 再生速度がx1.000のときは、TempoStreamを用いないようにして高速化する
		//        {
		//            return _hTempoStream;
		//        }
		//        else
		//        {
		//            return _hBassStream;
		//        }
		//    }
		//    set
		//    {
		//        _hBassStream = value;
		//    }
		//}
		protected SoundBuffer Buffer = null;			// DirectSound 用
		protected DirectSound DirectSound;
		protected int hMixer = -1;	// 設計壊してゴメン Mixerに後で登録するときに使う
		//-----------------
		#endregion

		#region [ private ]
		//-----------------
		private bool IsDirectSound
		{
			get { return ( this.NowSoundDeviceType == SoundDeviceType.DirectSound ); }
		}
		private bool IsBassSound
		{
			get
			{
				return (
					this.NowSoundDeviceType == SoundDeviceType.ASIO ||
					this.NowSoundDeviceType == SoundDeviceType.ExclusiveWASAPI ||
					this.NowSoundDeviceType == SoundDeviceType.SharedWASAPI );
			}
		}
		private int _NowPan = 0;
		private int _NowPan_Double;
		private Lufs _gain = DefaultGain;
	    private Lufs? _truePeak = null;
		private int _automationLevel = DefaultAutomationLevel;
		private int _groupLevel = DefaultGroupLevel;
		private long nBytes = 0;
		private int PauseCount = 0;
		private int OriginalFrequency = 0;
		private double _FrequencyMultiplier = 1.0;
		private double PlaySpeed = 1.0;
		private bool IsUniformVelocity = true;

		private void CreateBASSSound( string filePath, int mixer, BASSFlag flags )
		{
			#region [ xaとwav(RIFF chunked vorbis)に対しては専用の処理をする ]
			switch ( Path.GetExtension( filePath ).ToLower() )
			{
				case ".xa":
					CreateBASSSoundFromXA( filePath, mixer, flags );
					return;

				case ".wav":
					if ( DecodeWithDirectShowOfRIFFchunkedVorbis( filePath, ref WAVImages ) )
					{
						CreateBASSSound( WAVImages, mixer, flags );
						return;
					}
					break;

				default:
					break;
			}
			#endregion

			this.NowCreateType = CreateSoundType.File;
			this.FilePath = filePath;


			// BASSファイルストリームを作成。

			this.BassStreamHandle = Bass.BASS_StreamCreateFile( filePath, 0, 0, flags );
			if( this.BassStreamHandle == 0 )
				throw new Exception( string.Format( "サウンドストリームの生成に失敗しました。(BASS_StreamCreateFile)[{0}]", Bass.BASS_ErrorGetCode().ToString() ) );
			
			nBytes = Bass.BASS_ChannelGetLength( this.BassStreamHandle );
			
			CreateBASSSoundWithCommonProcessAfterStreamGenerate( mixer );
		}
		private void CreateBASSSound( byte[] wavImage, int mixer, BASSFlag flags )
		{
			this.NowCreateType = CreateSoundType.WavImage;
			this.WAVImages = wavImage;
			this.hGC = GCHandle.Alloc( wavImage, GCHandleType.Pinned );		// byte[] をピン留め


			// BASSファイルストリームを作成。

			this.BassStreamHandle = Bass.BASS_StreamCreateFile( hGC.AddrOfPinnedObject(), 0, wavImage.Length, flags );
			if ( this.BassStreamHandle == 0 )
				throw new Exception( string.Format( "サウンドストリームの生成に失敗しました。(BASS_StreamCreateFile)[{0}]", Bass.BASS_ErrorGetCode().ToString() ) );

			nBytes = Bass.BASS_ChannelGetLength( this.BassStreamHandle );
	
			CreateBASSSoundWithCommonProcessAfterStreamGenerate( mixer );
		}

		/// <summary>
		/// Decode "RIFF chunked Vorbis" to "raw wave"
		/// because BASE.DLL has two problems for RIFF chunked Vorbis;
		/// 1. time seek is not fine  2. delay occurs (about 10ms)
		/// </summary>
		/// <param name="filePath">wave filename</param>
		/// <param name="wavImage">wav file image</param>
		/// <returns></returns>
		private bool DecodeWithDirectShowOfRIFFchunkedVorbis( string filePath, ref byte[] wavImage )
		{
			bool existVoiceContainer = false;

			#region [ ファイルがWAVかつ、Vorbisコンテナが含まれているかを調べ、それに該当するなら、DirectShowでデコードする。]
			//-----------------
			try
			{
				using ( var ws = new WaveStream( filePath ) )
				{
					if ( ws.Format.FormatTag == (WaveFormatTag) 0x6770 ||	// Ogg Vorbis Mode 2+
						 ws.Format.FormatTag == (WaveFormatTag) 0x6771 )	// Ogg Vorbis Mode 3+
					{
						Trace.TraceInformation( Path.GetFileName( filePath ) + ": RIFF chunked Vorbis. Decode to raw Wave first, to avoid BASS.DLL troubles" );
						try
						{
							DStoWAVFileImage.t変換( filePath, out wavImage );
							existVoiceContainer = true;
						}
						catch
						{
							Trace.TraceWarning( "Warning: " + Path.GetFileName( filePath ) + " : RIFF chunked Vorbisのデコードに失敗しました。" );
						}
					}
				}
			}
			catch ( InvalidDataException )
			{
				// DirectShowのデコードに失敗したら、次はACMでのデコードを試すことになるため、ここではエラーログを出さない。
				// Trace.TraceWarning( "Warning: " + Path.GetFileName( strファイル名 ) + " : デコードに失敗しました。" );
			}
			catch ( Exception e )
			{
				Trace.TraceWarning( e.ToString() );
				Trace.TraceWarning( "Warning: " + Path.GetFileName( filePath ) + " : 読み込みに失敗しました。" );
			}
			#endregion

			return existVoiceContainer;
		}

		private void CreateBASSSoundFromXA( string filePath, int mixer, BASSFlag flags )
		{
			int pcmDataLeadingIndex;
			Win32Manager.WAVEFORMATEX wfx;
			int totalPCMSize;

			DecodeWithOnMemory( filePath, out this.WAVImages,
				out pcmDataLeadingIndex, out totalPCMSize, out wfx, true );

			nBytes = totalPCMSize;

			this.NowCreateType = CreateSoundType.WavImage;		//.ファイルから;	// 再構築時はデコード後のイメージを流用する&Dispose時にhGCを解放する
			this.FilePath = filePath;
			this.hGC = GCHandle.Alloc( this.WAVImages, GCHandleType.Pinned );		// byte[] をピン留め

			//_cbStreamXA = new STREAMPROC( CallbackPlayingXA );

			// BASSファイルストリームを作成。

			//this.hBassStream = Bass.BASS_StreamCreate( xa.xaheader.nSamplesPerSec, xa.xaheader.nChannels, BASSFlag.BASS_STREAM_DECODE, _myStreamCreate, IntPtr.Zero );
			//this._hBassStream = Bass.BASS_StreamCreate( (int) wfx.nSamplesPerSec, (int) wfx.nChannels, BASSFlag.BASS_STREAM_DECODE, _cbStreamXA, IntPtr.Zero );

			// StreamCreate()で作成したstreamはseek不可のため、StreamCreateFile()を使う。
			this.BassStreamHandle = Bass.BASS_StreamCreateFile( this.hGC.AddrOfPinnedObject(), 0L, totalPCMSize, flags );
			if ( this.BassStreamHandle == 0 )
			{
				hGC.Free();
				throw new Exception( string.Format( "サウンドストリームの生成に失敗しました。(BASS_SampleCreate)[{0}]", Bass.BASS_ErrorGetCode().ToString() ) );
			}

			nBytes = Bass.BASS_ChannelGetLength( this.BassStreamHandle );


			CreateBASSSoundWithCommonProcessAfterStreamGenerate( mixer );
		}


		private void CreateBASSSoundWithCommonProcessAfterStreamGenerate( int mixer )
		{
			SoundManager.nStreams++;

			// 個々のストリームの出力をテンポ変更のストリームに入力する。テンポ変更ストリームの出力を、Mixerに出力する。

//			if ( CSound管理.bIsTimeStretch )	// TimeStretchのON/OFFに関わりなく、テンポ変更のストリームを生成する。後からON/OFF切り替え可能とするため。
			{
				this.TempoStreamHandle = BassFx.BASS_FX_TempoCreate( this.BassStreamHandle, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_FX_FREESOURCE );
				if ( this.TempoStreamHandle == 0 )
				{
					hGC.Free();
					throw new Exception( string.Format( "サウンドストリームの生成に失敗しました。(BASS_FX_TempoCreate)[{0}]", Bass.BASS_ErrorGetCode().ToString() ) );
				}
				else
				{
					Bass.BASS_ChannelSetAttribute( this.TempoStreamHandle, BASSAttribute.BASS_ATTRIB_TEMPO_OPTION_USE_QUICKALGO, 1f );	// 高速化(音の品質は少し落ちる)
				}
			}

			if ( TempoStreamHandle != 0 && !this.IsUniformVelocity )	// 再生速度がx1.000のときは、TempoStreamを用いないようにして高速化する
			{
				this._BassStreamHandle = TempoStreamHandle;
			}
			else
			{
				this._BassStreamHandle = BassStreamHandle;
			}

			// #32248 再生終了時に発火するcallbackを登録する (演奏終了後に再生終了するチップを非同期的にミキサーから削除するため。)
			_cbEndofStream = new SYNCPROC( CallbackEndofStream );
			Bass.BASS_ChannelSetSync( _BassStreamHandle, BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_MIXTIME, 0, _cbEndofStream, IntPtr.Zero );

			// n総演奏時間の取得; DTXMania用に追加。
			double seconds = Bass.BASS_ChannelBytes2Seconds( this.BassStreamHandle, nBytes );
			this.TotalPlayTimeMs = (int) ( seconds * 1000 );
			//this.pos = 0;
			this.hMixer = mixer;
			float freq = 0.0f;
			if ( !Bass.BASS_ChannelGetAttribute( this.BassStreamHandle, BASSAttribute.BASS_ATTRIB_FREQ, ref freq ) )
			{
				hGC.Free();
				throw new Exception( string.Format( "サウンドストリームの周波数取得に失敗しました。(BASS_ChannelGetAttribute)[{0}]", Bass.BASS_ErrorGetCode().ToString() ) );
			}
			this.OriginalFrequency = (int) freq;

		    // インスタンスリストに登録。

		    FDKSound.FDKSounds.Add( this );
		}
		//-----------------

		//private int pos = 0;
		//private int CallbackPlayingXA( int handle, IntPtr buffer, int length, IntPtr user )
		//{
		//    int bytesread = ( pos + length > Convert.ToInt32( nBytes ) ) ? Convert.ToInt32( nBytes ) - pos : length;

		//    Marshal.Copy( byArrWAVファイルイメージ, pos, buffer, bytesread );
		//    pos += bytesread;
		//    if ( pos >= nBytes )
		//    {
		//        // set indicator flag
		//        bytesread |= (int) BASSStreamProc.BASS_STREAMPROC_END;
		//    }
		//    return bytesread;
		//}
		/// <summary>
		/// ストリームの終端まで再生したときに呼び出されるコールバック
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="channel"></param>
		/// <param name="data"></param>
		/// <param name="user"></param>
		private void CallbackEndofStream( int handle, int channel, int data, IntPtr user )	// #32248 2013.10.14 yyagi
		{
// Trace.TraceInformation( "Callback!(remove): " + Path.GetFileName( this.strファイル名 ) );
			if ( IsContinueToPlayAfterTheEnd )			// 演奏終了後に再生終了するチップ音のミキサー削除は、再生終了のコールバックに引っ掛けて、自前で行う。
			{													// そうでないものは、ミキサー削除予定時刻に削除する。
				RemoveBASSSoundFromMixer( channel );
			}
		}

// mixerからの削除

		public bool RemoveBassSoundTheMixer()
		{
			return RemoveBASSSoundFromMixer( this._BassStreamHandle );
		}
		public bool RemoveBASSSoundFromMixer( int channel )
		{
			bool b = BassMix.BASS_Mixer_ChannelRemove( channel );
			if ( b )
			{
				Interlocked.Decrement( ref SoundManager.nMixing );
//				Debug.WriteLine( "Removed: " + Path.GetFileName( this.strファイル名 ) + " (" + channel + ")" + " MixedStreams=" + CSound管理.nMixing );
			}
			return b;
		}


// mixer への追加
		
		public bool ADDBassSoundToTheMixer()
		{
			if ( BassMix.BASS_Mixer_ChannelGetMixer( _BassStreamHandle ) == 0 )
			{
				BASSFlag bf = BASSFlag.BASS_SPEAKER_FRONT | BASSFlag.BASS_MIXER_NORAMPIN | BASSFlag.BASS_MIXER_PAUSE;
				Interlocked.Increment( ref SoundManager.nMixing );

				// preloadされることを期待して、敢えてflagからはBASS_MIXER_PAUSEを外してAddChannelした上で、すぐにPAUSEする
				// -> ChannelUpdateでprebufferできることが分かったため、BASS_MIXER_PAUSEを使用することにした

				bool b1 = BassMix.BASS_Mixer_StreamAddChannel( this.hMixer, this._BassStreamHandle, bf );
				//bool b2 = BassMix.BASS_Mixer_ChannelPause( this.hBassStream );
				SetPlayPositionTheBegin();	// StreamAddChannelの後で再生位置を戻さないとダメ。逆だと再生位置が変わらない。
//Trace.TraceInformation( "Add Mixer: " + Path.GetFileName( this.strファイル名 ) + " (" + hBassStream + ")" + " MixedStreams=" + CSound管理.nMixing );
				Bass.BASS_ChannelUpdate( this._BassStreamHandle, 0 );	// pre-buffer
				return b1;	// &b2;
			}
			return true;
		}

		#region [ tオンメモリ方式でデコードする() ]
		public void DecodeWithOnMemory( string filePath, out byte[] buffer,
			out int pcmDataLeadingIndex, out int totalPCMSize, out Win32Manager.WAVEFORMATEX wfx,
			bool integrateWaveHeader )
		{
			pcmDataLeadingIndex = 0;
			//int nPCMサイズbyte = (int) ( xa.xaheader.nSamples * xa.xaheader.nChannels * 2 );	// nBytes = Bass.BASS_ChannelGetLength( this.hBassStream );

			SoundDecoder sounddecoder;

			if ( String.Compare( Path.GetExtension( filePath ), ".xa", true ) == 0 )
			{
				sounddecoder = new XaDecoder();
			}
			else if ( String.Compare( Path.GetExtension( filePath ), ".ogg", true ) == 0 )
			{
				sounddecoder = new OGGDecoder();
			}
			else if ( String.Compare( Path.GetExtension( filePath ), ".mp3", true ) == 0 )
			{
				sounddecoder = new MP3Decoder();
			}
			else
			{
				throw new NotImplementedException();
			}

			if ( !File.Exists( filePath ) )
			{
				throw new Exception( string.Format( "ファイルが見つかりませんでした。({0})", filePath ) );
			}
			int nHandle = sounddecoder.Open( filePath );
			if ( nHandle < 0 )
			{
				throw new Exception( string.Format( "Open() に失敗しました。({0})({1})", nHandle, filePath ) );
			}
			wfx = new Win32Manager.WAVEFORMATEX();
			if ( sounddecoder.GetFormat( nHandle, ref wfx ) < 0 )
			{
				sounddecoder.Close( nHandle );
				throw new Exception( string.Format( "GetFormat() に失敗しました。({0})", filePath ) );
			}
			//totalPCMSize = (int) sounddecoder.nTotalPCMSize;		//  tデコード後のサイズを調べる()で既に取得済みの値を流用する。ms単位の高速化だが、チップ音がたくさんあると塵積で結構効果がある
			totalPCMSize = (int) sounddecoder.GetTotalPCMSize( nHandle );
			if ( totalPCMSize == 0 )
			{
				sounddecoder.Close( nHandle );
				throw new Exception( string.Format( "GetTotalPCMSize() に失敗しました。({0})", filePath ) );
			}
			totalPCMSize += ( ( totalPCMSize % 2 ) != 0 ) ? 1 : 0;
			int wavheadersize = ( integrateWaveHeader ) ? 44 : 0;
			byte[] buffer_rawdata = new byte[ totalPCMSize ];
			buffer = new byte[ wavheadersize + totalPCMSize ];
			GCHandle handle = GCHandle.Alloc( buffer_rawdata, GCHandleType.Pinned );
			try
			{
				if ( sounddecoder.Decode( nHandle, handle.AddrOfPinnedObject(), (uint) totalPCMSize, 0 ) < 0 )
				{
					buffer = null;
					throw new Exception( string.Format( "デコードに失敗しました。({0})", filePath ) );
				}
				if ( integrateWaveHeader )
				{
					// wave headerを書き込む

					int wfxExtensionLength = 0;
					var ms = new MemoryStream();
					var bw = new BinaryWriter( ms );
					bw.Write( new byte[] { 0x52, 0x49, 0x46, 0x46 } );		// 'RIFF'
					bw.Write( (UInt32) totalPCMSize + 44 - 8 );				// ファイルサイズ - 8 [byte]；今は不明なので後で上書きする。
					bw.Write( new byte[] { 0x57, 0x41, 0x56, 0x45 } );		// 'WAVE'
					bw.Write( new byte[] { 0x66, 0x6D, 0x74, 0x20 } );		// 'fmt '
					bw.Write( (UInt32) ( 16 + ( ( wfxExtensionLength > 0 ) ? ( 2/*sizeof(WAVEFORMATEX.cbSize)*/ + wfxExtensionLength ) : 0 ) ) );	// fmtチャンクのサイズ[byte]
					bw.Write( (UInt16) wfx.wFormatTag );					// フォーマットID（リニアPCMなら1）
					bw.Write( (UInt16) wfx.nChannels );						// チャンネル数
					bw.Write( (UInt32) wfx.nSamplesPerSec );				// サンプリングレート
					bw.Write( (UInt32) wfx.nAvgBytesPerSec );				// データ速度
					bw.Write( (UInt16) wfx.nBlockAlign );					// ブロックサイズ
					bw.Write( (UInt16) wfx.wBitsPerSample );				// サンプルあたりのビット数
					//if ( wfx拡張領域_Length > 0 )
					//{
					//    bw.Write( (UInt16) wfx拡張領域.Length );			// 拡張領域のサイズ[byte]
					//    bw.Write( wfx拡張領域 );							// 拡張データ
					//}
					bw.Write( new byte[] { 0x64, 0x61, 0x74, 0x61 } );		// 'data'
					//int nDATAチャンクサイズ位置 = (int) ms.Position;
					bw.Write( (UInt32) totalPCMSize );						// dataチャンクのサイズ[byte]

					byte[] bs = ms.ToArray();

					bw.Close();
					ms.Close();

					for ( int i = 0; i < bs.Length; i++ )
					{
						buffer[ i ] = bs[ i ];
					}
				}
				int s = ( integrateWaveHeader ) ? 44 : 0;
				for ( int i = 0; i < totalPCMSize; i++ )
				{
					buffer[ i + s ] = buffer_rawdata[ i ];
				}
				totalPCMSize += wavheadersize;
				pcmDataLeadingIndex = wavheadersize;
			}
			finally
			{
				handle.Free();
				sounddecoder.Close( nHandle );
				sounddecoder = null;
			}
		}
		#endregion
		#endregion
	}
}
