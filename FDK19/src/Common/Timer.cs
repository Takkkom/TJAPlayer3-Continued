using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DirectShowLib;

namespace FDK
{
	public class Timer : BaseTimer
	{
		public enum TimerType
		{
			Unknown = -1,
			PerformanceCounter = 0,
			MultiMedia = 1,
			GetTickCount = 2,
		}
		public TimerType NowTimerType
		{
			get;
			protected set;
		}


		public override long SystemTimeMs
		{
			get
			{
				switch( this.NowTimerType )
				{
					case TimerType.PerformanceCounter:
						{
							double num = 0.0;
							if( this.CurFrequency != 0L )
							{
								long x = 0L;
								QueryPerformanceCounter( ref x );
								num = ( (double) x ) / ( ( (double) this.CurFrequency ) / 1000.0 );
							}
							return (long) num;
						}
					case TimerType.MultiMedia:
						return (long) timeGetTime();

					case TimerType.GetTickCount:
						return (long) Environment.TickCount;
				}
				return 0;
			}
		}

		public Timer( TimerType timerType )
			:base()
		{
			this.NowTimerType = timerType;

			if( ReferenceCount[ (int) this.NowTimerType ] == 0 )
			{
				switch( this.NowTimerType )
				{
					case TimerType.PerformanceCounter:
						if( !this.b確認と設定_PerformanceCounter() && !this.b確認と設定_MultiMedia() )
							this.b確認と設定_GetTickCount();
						break;

					case TimerType.MultiMedia:
						if( !this.b確認と設定_MultiMedia() && !this.b確認と設定_PerformanceCounter() )
							this.b確認と設定_GetTickCount();
						break;

					case TimerType.GetTickCount:
						this.b確認と設定_GetTickCount();
						break;

					default:
						throw new ArgumentException( string.Format( "未知のタイマ種別です。[{0}]", this.NowTimerType ) );
				}
			}
	
			base.Reset();

			ReferenceCount[ (int) this.NowTimerType ]++;
		}
		
		public override void Dispose()
		{
			if( this.NowTimerType == TimerType.Unknown )
				return;

			int type = (int) this.NowTimerType;

			ReferenceCount[ type ] = Math.Max( ReferenceCount[ type ] - 1, 0 );

			if( ReferenceCount[ type ] == 0 )
			{
				if( this.NowTimerType == TimerType.MultiMedia )
					timeEndPeriod( this.timeCaps.wPeriodMin );
			}

			this.NowTimerType = TimerType.Unknown;
		}

		#region [ protected ]
		//-----------------
		protected long CurFrequency;
		protected static int[] ReferenceCount = new int[ 3 ];
		protected TimeCaps timeCaps;

		protected bool b確認と設定_GetTickCount()
		{
			this.NowTimerType = TimerType.GetTickCount;
			return true;
		}
		protected bool b確認と設定_MultiMedia()
		{
			this.timeCaps = new TimeCaps();
			if( ( timeGetDevCaps( out this.timeCaps, (uint) Marshal.SizeOf( typeof( TimeCaps ) ) ) == 0 ) && ( this.timeCaps.wPeriodMin < 10 ) )
			{
				this.NowTimerType = TimerType.MultiMedia;
				timeBeginPeriod( this.timeCaps.wPeriodMin );
				return true;
			}
			return false;
		}
		protected bool b確認と設定_PerformanceCounter()
		{
			if( QueryPerformanceFrequency( ref this.CurFrequency ) != 0 )
			{
				this.NowTimerType = TimerType.PerformanceCounter;
				return true;
			}
			return false;
		}
		//-----------------
		#endregion

		#region [ DllImport ]
		//-----------------
		[DllImport( "kernel32.dll" )]
		protected static extern short QueryPerformanceCounter( ref long x );
		[DllImport( "kernel32.dll" )]
		protected static extern short QueryPerformanceFrequency( ref long x );
		[DllImport( "winmm.dll" )]
		protected static extern void timeBeginPeriod( uint x );
		[DllImport( "winmm.dll" )]
		protected static extern void timeEndPeriod( uint x );
		[DllImport( "winmm.dll" )]
		protected static extern uint timeGetDevCaps( out TimeCaps timeCaps, uint size );
		[DllImport( "winmm.dll" )]
		protected static extern uint timeGetTime();

		[StructLayout( LayoutKind.Sequential )]
		protected struct TimeCaps
		{
			public uint wPeriodMin;
			public uint wPeriodMax;
		}
		//-----------------
		#endregion
	}
}
