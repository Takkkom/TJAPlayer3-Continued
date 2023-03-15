using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	public class FPSManager
	{
		// プロパティ

		public int NowFPS
		{
			get;
			private set;
		}
		public bool IsFPSChanged
		{
			get;
			private set;
		}


		// コンストラクタ

		public FPSManager()
		{
			this.NowFPS = 0;
			this.FPSTimer = new Timer( Timer.TimerType.MultiMedia );
			this.StartTimeMs = this.FPSTimer.n現在時刻;
			this.NowFrameCount = 0;
			this.IsFPSChanged = false;
		}


		// メソッド

		public void Update()
		{
			this.FPSTimer.Update();
			this.IsFPSChanged = false;

			const long INTERVAL = 1000;
			while( ( this.FPSTimer.n現在時刻 - this.StartTimeMs ) >= INTERVAL )
			{
				this.NowFPS = this.NowFrameCount;
				this.NowFrameCount = 0;
				this.IsFPSChanged = true;
				this.StartTimeMs += INTERVAL;
			}
			this.NowFrameCount++;
		}


		// その他

		#region [ private ]
		//-----------------
		private Timer FPSTimer;
		private long StartTimeMs;
		private int NowFrameCount;
		//-----------------
		#endregion
	}
}
