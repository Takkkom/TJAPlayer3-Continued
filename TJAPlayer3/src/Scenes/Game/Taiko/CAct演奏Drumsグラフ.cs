using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏Drumsグラフ : Activity
	{
	
		// コンストラクタ

		public CAct演奏Drumsグラフ()
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
			if( !base.NotActivated )
			{
				if( base.JustStartedUpdate )
				{
                }
                
			}
			return 0;
		}


		// その他

		#region [ private ]
		//----------------
		//-----------------
		#endregion
	}
}
