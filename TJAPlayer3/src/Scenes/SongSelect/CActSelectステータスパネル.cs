using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class CActSelectステータスパネル : Activity
	{
		// メソッド

		public CActSelectステータスパネル()
		{
			base.NotActivated = true;
		}
		public void t選択曲が変更された()
		{

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

			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		//-----------------
		#endregion
	}
}
