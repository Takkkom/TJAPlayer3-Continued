using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class CActResultRank : Activity
	{
		// コンストラクタ

		public CActResultRank()
		{
			base.NotActivated = true;
		}

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
			if( base.NotActivated )
			{
				return 0;
			}
			if( base.JustStartedUpdate )
			{
				base.JustStartedUpdate = false;
			}

			return 1;
		}
		

		// その他

		#region [ private ]
		//-----------------
		//-----------------
		#endregion
	}
}
