using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class CActSelectShowCurrentPosition : Activity
	{
		// メソッド

		public CActSelectShowCurrentPosition()
		{
			base.NotActivated = true;
		}

		// CActivity 実装

		public override void Activate()
		{
			if ( this.IsActivated )
				return;

			base.Activate();
		}
		public override void Deactivate()
		{
			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if ( !base.NotActivated )
			{
				string pathScrollBar = SkinManager.Path( @"Graphics\5_scrollbar.png" );
				string pathScrollPosition = SkinManager.Path( @"Graphics\5_scrollbar.png" );
				if ( File.Exists( pathScrollBar ) )
				{
					this.txScrollBar = TJAPlayer3.CreateFDKTexture( pathScrollBar, false );
				}
				if ( File.Exists( pathScrollPosition ) )
				{
					this.txScrollPosition = TJAPlayer3.CreateFDKTexture( pathScrollPosition, false );
				}
				base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if ( !base.NotActivated )
			{
				TJAPlayer3.t安全にDisposeする( ref this.txScrollBar );
				TJAPlayer3.t安全にDisposeする( ref this.txScrollPosition );

				base.ManagedReleaseResources();
			}
		}
		public override int Draw()
		{
			if ( this.txScrollBar != null )
			{
			    #region [ スクロールバーの描画 #27648 ]
                //this.txScrollBar.t2D描画( CDTXMania.app.Device, (int)(1280 - ((429.0f / 100.0f ) * CDTXMania.stage選曲.ct登場時アニメ用共通.n現在の値)), 164, new Rectangle( 0, 0, 352, 26 ) ); //移動後のxは851
			    #endregion
			    #region [ スクロール地点の描画 (計算はCActSelect曲リストで行う。スクロール位置と選曲項目の同期のため。)#27648 ]
				int py = TJAPlayer3.stage選曲.nスクロールバー相対y座標;
				if( py <= 336 && py >= 0 )
				{
					//this.txScrollBar.t2D描画( CDTXMania.app.Device, (int)( 1280 - 4 - (( 424.0f / 100.0f ) * CDTXMania.stage選曲.ct登場時アニメ用共通.n現在の値 ) ) + py, 164, new Rectangle( 352, 0, 26, 26 ) );//856
				}
			    #endregion
            }

			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private FDKTexture txScrollPosition;
		private FDKTexture txScrollBar;
		//-----------------
		#endregion
	}
}
