using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;

namespace TJAPlayer3
{
	internal class PluginHost : IPluginHost
	{
		// コンストラクタ

		public PluginHost()
		{
			this._DTXManiaVersion = new DTXVersionManager( TJAPlayer3.VERSION );
		}


		// IPluginHost 実装

		public DTXVersionManager DTXManiaVersion
		{
			get { return this._DTXManiaVersion; }
		}
		public Device D3D9Device
		{
			get { return (TJAPlayer3.app != null ) ? TJAPlayer3.app.Device.UnderlyingDevice : null; }
		}
		public Format TextureFormat
		{
			get { return TJAPlayer3.TextureFormat; }
		}
		public FDKTimer Timer
		{
			get { return TJAPlayer3.Timer; }
		}
		public SoundManager _SoundManager
		{
			get { return TJAPlayer3._SoundManager; }
		}
		public Size ClientSize
		{
			get { return TJAPlayer3.app.Window.ClientSize; }
		}
		public BaseScene.Eステージ NowScene
		{
			get { return ( TJAPlayer3.r現在のステージ != null ) ? TJAPlayer3.r現在のステージ.eステージID : BaseScene.Eステージ.何もしない; }
		}
		public BaseScene.Eフェーズ NowPhase
		{
			get { return ( TJAPlayer3.r現在のステージ != null ) ? TJAPlayer3.r現在のステージ.eフェーズID : BaseScene.Eフェーズ.共通_通常状態; }
		}
		public bool OccupyInput(IPluginActivity act)
		{
			if (TJAPlayer3.CurrentOccupyingInputPlugin != null)
				return false;

			TJAPlayer3.CurrentOccupyingInputPlugin = act;
			return true;
		}
		public bool UnoccupyInput(IPluginActivity act)
		{
			if (TJAPlayer3.CurrentOccupyingInputPlugin == null || TJAPlayer3.CurrentOccupyingInputPlugin != act)
				return false;

			TJAPlayer3.CurrentOccupyingInputPlugin = null;
			return true;
		}
		public void PlaySystemSound( Eシステムサウンド sound )
		{
			if( TJAPlayer3.Skin != null )
				TJAPlayer3.Skin[ sound ].t再生する();
		}
		
		
		// その他

		#region [ private ]
		//-----------------
		private DTXVersionManager _DTXManiaVersion;
		//-----------------
		#endregion
	}
}
