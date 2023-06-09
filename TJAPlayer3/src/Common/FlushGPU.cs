﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using SlimDX.Direct3D9;
using FDK;

namespace TJAPlayer3
{
	/// <summary>
	/// 描画フレーム毎にGPUをフラッシュして、描画遅延を防ぐ。
	/// DirectX9の、Occlusion Queryを用いる。(Flush属性付きでGetDataする)
	/// Device Lost対策のため、QueueをCActivitiyのManagedリソースとして扱う。
	/// On進行描画()を呼び出すことで、GPUをフラッシュする。
	/// </summary>
	internal class FlushGPU : Activity
	{
		// CActivity 実装

		public override void ManagedCreateResources()
		{
			if ( !base.NotActivated )
			{
				try			// #xxxxx 2012.12.31 yyagi: to prepare flush, first of all, I create q queue to the GPU.
				{
					IDirect3DQuery9 = new SlimDX.Direct3D9.Query( TJAPlayer3.app.Device.UnderlyingDevice, QueryType.Occlusion );
				}
				catch ( Exception e )
				{
					Trace.TraceError( e.ToString() );
					Trace.TraceError( "例外が発生しましたが処理を継続します。 (e5c7cd0b-f7bb-4bf1-9ad9-db27b43ff63d)" );
				}
				base.ManagedCreateResources();
			}
		}
		public override void  ManagedReleaseResources()
		{
			IDirect3DQuery9.Dispose();
			IDirect3DQuery9 = null;
			base.ManagedReleaseResources();
		}
		public override int Draw()
		{
			if ( !base.NotActivated )
			{
				IDirect3DQuery9.Issue( Issue.End );
				DWM.Flush();
				IDirect3DQuery9.GetData<int>( true );	// flush GPU queue
			}
			return 0;
		}

		// その他

		#region [ private ]
		//-----------------
		private SlimDX.Direct3D9.Query IDirect3DQuery9;
		//-----------------
		#endregion
	}
}
