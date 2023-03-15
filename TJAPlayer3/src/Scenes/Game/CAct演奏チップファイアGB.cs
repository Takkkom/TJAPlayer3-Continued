using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal abstract class CAct演奏チップファイアGB : Activity
	{
		// コンストラクタ

		public CAct演奏チップファイアGB()
		{
			base.NotActivated = true;
		}


		// メソッド

		public virtual void Start( int nLane, int n中央X, int n中央Y, C演奏判定ライン座標共通 演奏判定ライン座標 )
		{
		}

		public abstract void Start( int nLane, C演奏判定ライン座標共通 演奏判定ライン座標 );
//		public abstract void Start( int nLane );

		// CActivity 実装

		public override void Activate()
		{
			base.Activate();
		}
		public override void Deactivate()
		{
			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
				base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if( !base.NotActivated )
			{
				base.ManagedReleaseResources();
			}
		}
		public override int Draw()
		{
			if( !base.NotActivated )
			{
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		//private STDGBVALUE<int> nJudgeLinePosY_delta = new STDGBVALUE<int>();
		C演奏判定ライン座標共通 _演奏判定ライン座標 = new C演奏判定ライン座標共通();
		//-----------------
		#endregion
	}
}
