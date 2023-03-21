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
                this.pfMusicName = new CachePrivateFont(new FontFamily(TJAPlayer3._MainConfig.FontName), TJAPlayer3.Skin.SkinValue.Result_MusicName_FontSize);
                this.pfStageText = new CachePrivateFont(new FontFamily(TJAPlayer3._MainConfig.FontName), TJAPlayer3.Skin.SkinValue.Result_StageText_FontSize);
            }
            else
            {
                this.pfMusicName = new CachePrivateFont(new FontFamily("MS UI Gothic"), TJAPlayer3.Skin.SkinValue.Result_MusicName_FontSize);
                this.pfStageText = new CachePrivateFont(new FontFamily("MS UI Gothic"), TJAPlayer3.Skin.SkinValue.Result_StageText_FontSize);
            }

		    // After performing calibration, inform the player that
		    // calibration has been completed, rather than
		    // displaying the song title as usual.


		    var title = TJAPlayer3.IsPerformingCalibration
		        ? $"Calibration complete. InputAdjustTime is now {TJAPlayer3._MainConfig.nInputAdjustTimeMs}ms"
		        : TJAPlayer3.DTX.TITLE;

		    using (var bmpSongTitle = pfMusicName.DrawPrivateFont(title, TJAPlayer3.Skin.SkinValue.Result_MusicName_ForeColor.GetColor(), TJAPlayer3.Skin.SkinValue.Result_MusicName_BackColor.GetColor()))

		    {
		        this.txMusicName = TJAPlayer3.CreateFDKTexture(bmpSongTitle, false);
		        txMusicName.Scaling.X = TJAPlayer3.GetSongNameXScaling(ref txMusicName);
		    }

		    using (var bmpStageText = pfStageText.DrawPrivateFont(TJAPlayer3.Skin.SkinValue.Game_StageText, TJAPlayer3.Skin.SkinValue.Result_StageText_ForeColor.GetColor(), TJAPlayer3.Skin.SkinValue.Result_StageText_BackColor.GetColor()))
		    {
		        this.txStageText = TJAPlayer3.CreateFDKTexture(bmpStageText, false);
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
                TJAPlayer3.DisposeFDKTexture( ref this.txMusicName );

                TJAPlayer3.t安全にDisposeする(ref this.pfStageText);
                TJAPlayer3.DisposeFDKTexture(ref this.txStageText);
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

            if (TJAPlayer3.Skin.SkinValue.Result_MusicName_ReferencePoint == SkinManager.ReferencePoint.Center)
            {
                this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Result_MusicName_X - ((this.txMusicName.TextureSize.Width * txMusicName.Scaling.X) / 2), TJAPlayer3.Skin.SkinValue.Result_MusicName_Y);
            }
            else if (TJAPlayer3.Skin.SkinValue.Result_MusicName_ReferencePoint == SkinManager.ReferencePoint.Left)
            {
                this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Result_MusicName_X, TJAPlayer3.Skin.SkinValue.Result_MusicName_Y);
            }
            else
            {
                this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Result_MusicName_X - this.txMusicName.TextureSize.Width * txMusicName.Scaling.X, TJAPlayer3.Skin.SkinValue.Result_MusicName_Y);
            }

            if(TJAPlayer3.stage選曲.n確定された曲の難易度[0] != (int)Difficulty.Dan)
            {
                if (TJAPlayer3.Skin.SkinValue.Result_StageText_ReferencePoint == SkinManager.ReferencePoint.Center)
                {
                    this.txStageText.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Result_StageText_X - ((this.txStageText.TextureSize.Width * txStageText.Scaling.X) / 2), TJAPlayer3.Skin.SkinValue.Result_StageText_Y);
                }
                else if (TJAPlayer3.Skin.SkinValue.Result_StageText_ReferencePoint == SkinManager.ReferencePoint.Right)
                {
                    this.txStageText.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Result_StageText_X - this.txStageText.TextureSize.Width, TJAPlayer3.Skin.SkinValue.Result_StageText_Y);
                }
                else
                {
                    this.txStageText.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Result_StageText_X, TJAPlayer3.Skin.SkinValue.Result_StageText_Y);
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
        private CachePrivateFont pfMusicName;

        private FDKTexture txStageText;
        private PrivateFont pfStageText;
        //-----------------
		#endregion
	}
}
