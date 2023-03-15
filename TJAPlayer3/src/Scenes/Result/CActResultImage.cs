using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;

namespace TJAPlayer3
{
	internal class CActResultImage : Activity
	{
		// コンストラクタ
        /// <summary>
        /// リザルト画像を表示させるクラス。XG化するにあたって動画は廃止。
        /// また、中央の画像も表示する。(STAGE表示、STANDARD_CLASSICなど)
        /// </summary>
		public CActResultImage()
		{
			base.NotActivated = true;
		}


		// メソッド

		public void tアニメを完了させる()
		{
			this.ct登場用.NowValue = this.ct登場用.EndValue;
		}


		// CActivity 実装

		public override void Activate()
		{

			base.Activate();
		}
		public override void Deactivate()
		{
			if( this.ct登場用 != null )
			{
				this.ct登場用 = null;
			}
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
		public override unsafe int Draw()
		{
			if( base.NotActivated )
			{
				return 0;
			}
			if( base.JustStartedUpdate )
			{
				this.ct登場用 = new Counter( 0, 100, 5, TJAPlayer3.Timer );
				base.JustStartedUpdate = false;
			}
			this.ct登場用.Tick();

			if( !this.ct登場用.IsEndValueReached )
			{
				return 0;
			}
			return 1;
		}


		// その他

		#region [ private ]
		//-----------------
		private Counter ct登場用;
		private FDKTexture r表示するリザルト画像;
		private FDKTexture txリザルト画像;

		private bool tプレビュー画像の指定があれば構築する()
		{
			if( string.IsNullOrEmpty( TJAPlayer3.DTX.PREIMAGE ) )
			{
				return false;
			}
			TJAPlayer3.tテクスチャの解放( ref this.txリザルト画像 );
			this.r表示するリザルト画像 = null;
			string path = TJAPlayer3.DTX.strフォルダ名 + TJAPlayer3.DTX.PREIMAGE;
			if( !File.Exists( path ) )
			{
				Trace.TraceWarning( "ファイルが存在しません。({0})", new object[] { path } );
				return false;
			}
			this.txリザルト画像 = TJAPlayer3.tテクスチャの生成( path );
			this.r表示するリザルト画像 = this.txリザルト画像;
			return ( this.r表示するリザルト画像 != null );
		}
		//-----------------
		#endregion
	}
}
