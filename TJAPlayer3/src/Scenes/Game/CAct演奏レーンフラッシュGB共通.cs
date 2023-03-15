using System;
using System.Collections.Generic;
using System.Text;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏レーンフラッシュGB共通 : Activity
	{
		// プロパティ

		// コンストラクタ

		public CAct演奏レーンフラッシュGB共通()
		{
			base.NotActivated = true;
		}


		// メソッド


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
	}
}
