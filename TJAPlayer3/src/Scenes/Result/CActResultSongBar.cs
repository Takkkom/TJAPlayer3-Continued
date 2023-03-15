using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class CActResultSongBar : Activity
	{
		// コンストラクタ

		public CActResultSongBar()
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
            if( !string.IsNullOrEmpty( TJAPlayer3._MainConfig.FontName) )
            {
                this.pfMusicName = new CPrivateFastFont(new FontFamily(TJAPlayer3._MainConfig.FontName), TJAPlayer3.Skin.Result_MusicName_FontSize);
                this.pfStageText = new CPrivateFastFont(new FontFamily(TJAPlayer3._MainConfig.FontName), TJAPlayer3.Skin.Result_StageText_FontSize);
            }
            else
            {
                this.pfMusicName = new CPrivateFastFont(new FontFamily("MS UI Gothic"), TJAPlayer3.Skin.Result_MusicName_FontSize);
                this.pfStageText = new CPrivateFastFont(new FontFamily("MS UI Gothic"), TJAPlayer3.Skin.Result_StageText_FontSize);
            }

		    // After performing calibration, inform the player that
		    // calibration has been completed, rather than
		    // displaying the song title as usual.


		    var title = TJAPlayer3.IsPerformingCalibration
		        ? $"Calibration complete. InputAdjustTime is now {TJAPlayer3._MainConfig.nInputAdjustTimeMs}ms"
		        : TJAPlayer3.DTX.TITLE;

		    using (var bmpSongTitle = pfMusicName.DrawPrivateFont(title, TJAPlayer3.Skin.Result_MusicName_ForeColor, TJAPlayer3.Skin.Result_MusicName_BackColor))

		    {
		        this.txMusicName = TJAPlayer3.tテクスチャの生成(bmpSongTitle, false);
		        txMusicName.Scaling.X = TJAPlayer3.GetSongNameXScaling(ref txMusicName);
		    }

		    using (var bmpStageText = pfStageText.DrawPrivateFont(TJAPlayer3.Skin.Game_StageText, TJAPlayer3.Skin.Result_StageText_ForeColor, TJAPlayer3.Skin.Result_StageText_BackColor))
		    {
		        this.txStageText = TJAPlayer3.tテクスチャの生成(bmpStageText, false);
		    }

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
                TJAPlayer3.t安全にDisposeする(ref this.pfMusicName);
                TJAPlayer3.tテクスチャの解放( ref this.txMusicName );

                TJAPlayer3.t安全にDisposeする(ref this.pfStageText);
                TJAPlayer3.tテクスチャの解放(ref this.txStageText);
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
				this.ct登場用 = new Counter( 0, 270, 4, TJAPlayer3.Timer );
				base.JustStartedUpdate = false;
			}
			this.ct登場用.Tick();

            if (TJAPlayer3.Skin.Result_MusicName_ReferencePoint == CSkin.ReferencePoint.Center)
            {
                this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_MusicName_X - ((this.txMusicName.TextureSize.Width * txMusicName.Scaling.X) / 2), TJAPlayer3.Skin.Result_MusicName_Y);
            }
            else if (TJAPlayer3.Skin.Result_MusicName_ReferencePoint == CSkin.ReferencePoint.Left)
            {
                this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_MusicName_X, TJAPlayer3.Skin.Result_MusicName_Y);
            }
            else
            {
                this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_MusicName_X - this.txMusicName.TextureSize.Width * txMusicName.Scaling.X, TJAPlayer3.Skin.Result_MusicName_Y);
            }

            if(TJAPlayer3.stage選曲.n確定された曲の難易度 != (int)Difficulty.Dan)
            {
                if (TJAPlayer3.Skin.Result_StageText_ReferencePoint == CSkin.ReferencePoint.Center)
                {
                    this.txStageText.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_StageText_X - ((this.txStageText.TextureSize.Width * txStageText.Scaling.X) / 2), TJAPlayer3.Skin.Result_StageText_Y);
                }
                else if (TJAPlayer3.Skin.Result_StageText_ReferencePoint == CSkin.ReferencePoint.Right)
                {
                    this.txStageText.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_StageText_X - this.txStageText.TextureSize.Width, TJAPlayer3.Skin.Result_StageText_Y);
                }
                else
                {
                    this.txStageText.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_StageText_X, TJAPlayer3.Skin.Result_StageText_Y);
                }
            }


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

        private FDKTexture txMusicName;
        private CPrivateFastFont pfMusicName;

        private FDKTexture txStageText;
        private CPrivateFont pfStageText;
        //-----------------
		#endregion
	}
}
