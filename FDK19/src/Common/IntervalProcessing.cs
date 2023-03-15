using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	/// <summary>
	/// <para>一定の間隔で処理を行うテンプレートパターンの定義。</para>
	/// <para>たとえば、t進行() で 5ms ごとに行う処理を前回のt進行()の呼び出しから 15ms 後に呼び出した場合は、処理が 3回 実行される。</para>
	/// </summary>
	public class IntervalProcessing : IDisposable
	{
		public delegate void IntervalProcess();
		public void Tick( long interval, IntervalProcess process )
		{
			// タイマ更新

			if( this._Timer == null )
				return;
			this._Timer.Update();


			// 初めての進行処理

			if( this.PrevTime == Timer.Unused )
				this.PrevTime = this._Timer.NowTimeMs;


			// タイマが一回りしてしまった時のため……

			if( this._Timer.NowTimeMs < this.PrevTime )
				this.PrevTime = this._Timer.NowTimeMs;

	
			// 時間内の処理を実行。

			while( ( this._Timer.NowTimeMs - this.PrevTime ) >= interval )
			{
				process();

				this.PrevTime += interval;
			}
		}

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			FDKCommon.Dispose( ref this._Timer );
		}
		//-----------------
		#endregion

		#region [ protected ]
		//-----------------
		protected Timer _Timer = new Timer( Timer.TimerType.MultiMedia );
		protected long PrevTime = Timer.Unused;
		//-----------------
		#endregion
	}
}
