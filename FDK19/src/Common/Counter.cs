using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	/// <summary>
	/// 一定間隔で単純増加する整数（カウント値）を扱う。
	/// </summary>
    /// <remarks>
    /// ○使い方
    /// 1.CCounterの変数をつくる。
    /// 2.CCounterを生成
    ///   ctCounter = new CCounter( 0, 3, 10, CDTXMania.Timer );
    /// 3.進行メソッドを使用する。
    /// 4.ウマー。
    ///
    /// double値を使う場合、t進行db、t進行LoopDbを使うこと。
    /// また、double版では間隔の値はミリ秒単位ではなく、通常の秒単位になります。
    /// </remarks>
	public class Counter
	{
		// 値プロパティ

		public int StartValue
		{
			get;
			private set;
		}
		public int EndValue
		{
			get;
			private set;
		}
		public int NowValue
		{
			get;
			set;
		}
		public long NowElapsedTimeMs
		{
			get;
			set;
		}

        public double StartValue_Double
        {
            get;
            private set;
        }
        public double EndValue_Double
        {
            get;
            private set;
        }
        public double NowValue_Double
        {
            get;
            set;
        }
        public double NowElapsedTime_Double
        {
            get;
            set;
        }


		// 状態プロパティ

		public bool IsProcessed
		{
			get { return ( this.NowElapsedTimeMs != -1 ); }
		}
		public bool IsStoped
		{
			get { return !this.IsProcessed; }
		}
		public bool IsEndValueReached
		{
			get { return ( this.NowValue >= this.EndValue ); }
		}
		public bool IsEndValueNotReached
		{
			get { return !this.IsEndValueReached; }
		}

        /// <summary>通常のCCounterでは使用できません。</summary>
        public bool IsProcessed_Double
        {
            get { return ( this.NowElapsedTime_Double != -1 ); }
        }

        /// <summary>通常のCCounterでは使用できません。</summary>
        public bool IsStoped_Double
        {
            get { return !this.IsProcessed_Double; }
        }

        /// <summary>通常のCCounterでは使用できません。</summary>
        public bool IsEndValueReached_Double
        {
            get { return ( this.NowValue_Double >= this.EndValue_Double ); }
        }

        /// <summary>通常のCCounterでは使用できません。</summary>
        public bool IsEndValueNotReached_Double
        {
            get { return !this.IsEndValueReached_Double; }
        }


		// コンストラクタ

		public Counter()
		{
			this.timer = null;
			this.StartValue = 0;
			this.EndValue = 0;
			this.IntervalMs = 0;
			this.NowValue = 0;
			this.NowElapsedTimeMs = Timer.Unused;

            this.StartValue_Double = 0;
            this.EndValue_Double = 0;
            this.Interval_Double = 0;
            this.NowValue_Double = 0;
            this.NowElapsedTime_Double = CSoundTimer.Unused;
		}

		/// <summary>生成と同時に開始する。</summary>
		public Counter( int start, int end, int interval, Timer timer )
			: this()
		{
			this.Start( start, end, interval, timer );
		}

        /// <summary>生成と同時に開始する。(double版)</summary>
        public Counter( double start, double end, double interval, CSoundTimer timer )
            : this()
        {
            this.Start( start, end, interval * 1000.0, timer );
        }


		// 状態操作メソッド

		/// <summary>
		/// カウントを開始する。
		/// </summary>
		/// <param name="n開始値">最初のカウント値。</param>
		/// <param name="n終了値">最後のカウント値。</param>
		/// <param name="n間隔ms">カウント値を１増加させるのにかける時間（ミリ秒単位）。</param>
		/// <param name="timer">カウントに使用するタイマ。</param>
		public void Start( int n開始値, int n終了値, int n間隔ms, Timer timer )
		{
			this.StartValue = n開始値;
			this.EndValue = n終了値;
			this.IntervalMs = n間隔ms;
			this.timer = timer;
			this.NowElapsedTimeMs = this.timer.n現在時刻;
			this.NowValue = n開始値;
		}

        /// <summary>
		/// カウントを開始する。(double版)
		/// </summary>
		/// <param name="db開始値">最初のカウント値。</param>
		/// <param name="db終了値">最後のカウント値。</param>
		/// <param name="db間隔">カウント値を１増加させるのにかける時間（秒単位）。</param>
		/// <param name="timer">カウントに使用するタイマ。</param>
		public void Start( double db開始値, double db終了値, double db間隔, CSoundTimer timer )
		{
			this.StartValue_Double = db開始値;
			this.EndValue_Double = db終了値;
			this.Interval_Double = db間隔;
			this.timerdb = timer;
			this.NowElapsedTime_Double = this.timerdb.dbシステム時刻;
			this.NowValue_Double = db開始値;
		}

		/// <summary>
		/// 前回の t進行() の呼び出しからの経過時間をもとに、必要なだけカウント値を増加させる。
		/// カウント値が終了値に達している場合は、それ以上増加しない（終了値を維持する）。
		/// </summary>
		public void Tick()
		{
			if ( ( this.timer != null ) && ( this.NowElapsedTimeMs != Timer.Unused ) )
			{
				long num = this.timer.n現在時刻;
				if ( num < this.NowElapsedTimeMs )
					this.NowElapsedTimeMs = num;

				while ( ( num - this.NowElapsedTimeMs ) >= this.IntervalMs )
				{
					if ( ++this.NowValue > this.EndValue )
						this.NowValue = this.EndValue;

					this.NowElapsedTimeMs += this.IntervalMs;
				}
			}
		}

        /// <summary>
		/// 前回の t進行() の呼び出しからの経過時間をもとに、必要なだけカウント値を増加させる。
		/// カウント値が終了値に達している場合は、それ以上増加しない（終了値を維持する）。
		/// </summary>
		public void Tick_Double()
		{
			if ( ( this.timerdb != null ) && ( this.NowElapsedTime_Double != CSoundTimer.Unused ) )
			{
				double num = this.timerdb.n現在時刻;
				if ( num < this.NowElapsedTime_Double )
					this.NowElapsedTime_Double = num;

				while ( ( num - this.NowElapsedTime_Double ) >= this.Interval_Double )
				{
					if ( ++this.NowValue_Double > this.EndValue_Double )
						this.NowValue_Double = this.EndValue_Double;

					this.NowElapsedTime_Double += this.Interval_Double;
				}
			}
		}

		/// <summary>
		/// 前回の t進行Loop() の呼び出しからの経過時間をもとに、必要なだけカウント値を増加させる。
		/// カウント値が終了値に達している場合は、次の増加タイミングで開始値に戻る（値がループする）。
		/// </summary>
		public void TickLoop()
		{
			if ( ( this.timer != null ) && ( this.NowElapsedTimeMs != Timer.Unused ) )
			{
				long num = this.timer.n現在時刻;
				if ( num < this.NowElapsedTimeMs )
					this.NowElapsedTimeMs = num;

				while ( ( num - this.NowElapsedTimeMs ) >= this.IntervalMs )
				{
					if ( ++this.NowValue > this.EndValue )
						this.NowValue = this.StartValue;

					this.NowElapsedTimeMs += this.IntervalMs;
				}
			}
		}

        /// <summary>
		/// 前回の t進行Loop() の呼び出しからの経過時間をもとに、必要なだけカウント値を増加させる。
		/// カウント値が終了値に達している場合は、次の増加タイミングで開始値に戻る（値がループする）。
		/// </summary>
		public void TickLoop_Double()
		{
			if ( ( this.timerdb != null ) && ( this.NowElapsedTime_Double != CSoundTimer.Unused ) )
			{
				double num = this.timerdb.n現在時刻;
				if ( num < this.NowElapsedTimeMs )
					this.NowElapsedTime_Double = num;

				while ( ( num - this.NowElapsedTime_Double ) >= this.Interval_Double )
				{
					if ( ++this.NowValue_Double > this.EndValue_Double )
						this.NowValue_Double = this.StartValue_Double;

					this.NowElapsedTime_Double += this.Interval_Double;
				}
			}
		}

		/// <summary>
		/// カウントを停止する。
		/// これ以降に t進行() や t進行Loop() を呼び出しても何も処理されない。
		/// </summary>
		public void Stop()
		{
			this.NowElapsedTimeMs = Timer.Unused;
            this.NowElapsedTime_Double = CSoundTimer.Unused;
		}


		// その他

		#region [ 応用：キーの反復入力をエミュレーションする ]
		//-----------------

		/// <summary>
		/// <para>「bキー押下」引数が true の間中、「tキー処理」デリゲート引数を呼び出す。</para>
		/// <para>ただし、2回目の呼び出しは1回目から 200ms の間を開けてから行い、3回目以降の呼び出しはそれぞれ 30ms の間隔で呼び出す。</para>
		/// <para>「bキー押下」が false の場合は何もせず、呼び出し回数を 0 にリセットする。</para>
		/// </summary>
		/// <param name="pushing">キーが押下されている場合は true。</param>
		/// <param name="keyProcess">キーが押下されている場合に実行する処理。</param>
		public void RepeatKey( bool pushing, KeyProcess keyProcess )
		{
			const int _1st = 0;
			const int _2nd = 1;
			const int _3rd = 2;

			if ( pushing )
			{
				switch ( this.NowValue )
				{
					case _1st:

						keyProcess();
						this.NowValue = _2nd;
						this.NowElapsedTimeMs = this.timer.n現在時刻;
						return;

					case _2nd:

						if ( ( this.timer.n現在時刻 - this.NowElapsedTimeMs ) > 200 )
						{
							keyProcess();
							this.NowElapsedTimeMs = this.timer.n現在時刻;
							this.NowValue = _3rd;
						}
						return;

					case _3rd:

						if ( ( this.timer.n現在時刻 - this.NowElapsedTimeMs ) > 30 )
						{
							keyProcess();
							this.NowElapsedTimeMs = this.timer.n現在時刻;
						}
						return;
				}
			}
			else
			{
				this.NowValue = _1st;
			}
		}
		public delegate void KeyProcess();

		//-----------------
		#endregion

		#region [ private ]
		//-----------------
		private Timer timer;
        private CSoundTimer timerdb;
		private int IntervalMs;
        private double Interval_Double;
		//-----------------
		#endregion
	}
}