using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	/// <summary>
	/// <para>タイマの抽象クラス。</para>
	/// <para>このクラスを継承し、override したクラスを作成することで、任意のクロックを持つタイマを作成できる。</para>
	/// </summary>
	public abstract class BaseTimer : IDisposable
	{
		public const long Unused = -1;

		// この２つを override する。
		public abstract long SystemTimeMs
		{
			get;
		}
        public double SystemTimeMs_Double
        {
            get;
            set;
        }
		public abstract void Dispose();

		#region [ DTXMania用に、語尾にmsのつかない宣言を追加 ]
		public long nシステム時刻
		{
			get { return SystemTimeMs; }
		}
		public long n現在時刻
		{
			get { return NowTimeMs; }
			set { NowTimeMs = value; }
		}
		public long n前回リセットした時のシステム時刻
		{
			get { return SystemTimeAtLastResetMs; }
		}

        //double
        public double dbシステム時刻
		{
			get { return SystemTimeMs_Double; }
		}
		public double db現在時刻
		{
			get { return NowTimeMs_Double; }
			set { NowTimeMs_Double = value; }
		}
		public double db前回リセットした時のシステム時刻
		{
			get { return SystemTimeAtLastResetMs_Double; }
		}
		#endregion

		public long NowTimeMs
		{
			get
			{
				if( this.StopCount > 0 )
					return ( this.n一時停止システム時刻ms - this.SystemTimeAtLastResetMs );

				return ( this.n更新システム時刻ms - this.SystemTimeAtLastResetMs );
			}
			set
			{
				if( this.StopCount > 0 )
					this.SystemTimeAtLastResetMs = this.n一時停止システム時刻ms - value;
				else
					this.SystemTimeAtLastResetMs = this.n更新システム時刻ms - value;
			}
		}
		public long RealNowTimeMs
		{
			get
			{
				if( this.StopCount > 0 )
					return ( this.n一時停止システム時刻ms - this.SystemTimeAtLastResetMs );

				return ( this.SystemTimeMs - this.SystemTimeAtLastResetMs );
			}
		}
		public long SystemTimeAtLastResetMs
		{
			get;
			protected set;
		}


        public double NowTimeMs_Double
		{
			get
			{
				if( this.StopCount > 0 )
					return ( this.db一時停止システム時刻ms - this.SystemTimeAtLastResetMs_Double );

				return ( this.db更新システム時刻ms - this.SystemTimeAtLastResetMs_Double );
			}
			set
			{
				if( this.StopCount > 0 )
					this.SystemTimeAtLastResetMs_Double = this.db一時停止システム時刻ms - value;
				else
					this.SystemTimeAtLastResetMs_Double = this.db更新システム時刻ms - value;
			}
		}
		public double RealTimeMs_Double
		{
			get
			{
				if( this.StopCount > 0 )
					return ( this.db一時停止システム時刻ms - this.SystemTimeAtLastResetMs_Double );

				return ( this.SystemTimeMs_Double - this.SystemTimeAtLastResetMs_Double );
			}
		}
		public double SystemTimeAtLastResetMs_Double
		{
			get;
			protected set;
		}

        public bool IsNotStoped
        {
            get
            {
                return ( this.StopCount == 0 );
            }
        }

		public void Reset()
		{
			this.Update();
			this.SystemTimeAtLastResetMs = this.n更新システム時刻ms;
			this.n一時停止システム時刻ms = this.n更新システム時刻ms;
			this.StopCount = 0;
		}
		public void Stop()
		{
			if( this.StopCount == 0 )
            {
				this.n一時停止システム時刻ms = this.n更新システム時刻ms;
                this.db一時停止システム時刻ms = this.db更新システム時刻ms;
            }

			this.StopCount++;
		}
		public void Update()
		{
			this.n更新システム時刻ms = this.SystemTimeMs;
            this.db更新システム時刻ms = this.SystemTimeMs_Double;
		}
		public void Resume()
		{
			if( this.StopCount > 0 )
			{
				this.StopCount--;
				if( this.StopCount == 0 )
				{
					this.Update();
					this.SystemTimeAtLastResetMs += this.n更新システム時刻ms - this.n一時停止システム時刻ms;
                    this.SystemTimeAtLastResetMs_Double += this.db更新システム時刻ms - this.db一時停止システム時刻ms;
				}
			}
		}
		
		#region [ protected ]
		//-----------------
		protected long n一時停止システム時刻ms = 0;
		protected long n更新システム時刻ms = 0;
        protected double db一時停止システム時刻ms = 0;
        protected double db更新システム時刻ms = 0;
		protected int StopCount = 0;
		//-----------------
		#endregion
	}
}
