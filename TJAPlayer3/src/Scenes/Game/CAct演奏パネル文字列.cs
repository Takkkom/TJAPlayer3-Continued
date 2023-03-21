using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏パネル文字列 : Activity
	{

		// コンストラクタ

		public CAct演奏パネル文字列()
		{
			base.NotActivated = true;
			this.Start();
		}


        // メソッド

        /// <summary>
        /// 右上の曲名、曲数表示の更新を行います。
        /// </summary>
        /// <param name="songName">曲名</param>
        /// <param name="genreName">ジャンル名</param>
        /// <param name="stageText">曲数</param>
        public void SetPanelString(string songName, string genreName, string stageText = null)
		{
			if( base.IsActivated )
			{
				TJAPlayer3.DisposeFDKTexture( ref this.txPanel );
				if( (songName != null ) && (songName.Length > 0 ) )
				{
					try
					{
					    using (var bmpSongTitle = pfMusicName.DrawPrivateFont(songName, TJAPlayer3.Skin.SkinValue.Game_MusicName_ForeColor.GetColor(), TJAPlayer3.Skin.SkinValue.Game_MusicName_BackColor.GetColor()))
					    {
					        this.txMusicName = TJAPlayer3.CreateFDKTexture( bmpSongTitle, false );
					    }
                        if (txMusicName != null)
                        {
                            this.txMusicName.Scaling.X = TJAPlayer3.GetSongNameXScaling(ref txMusicName);
                        }
                    
                        Bitmap bmpDiff;
                        string strDiff = "";
                        if (TJAPlayer3.Skin.eDiffDispMode == E難易度表示タイプ.n曲目に表示)
                        {
                            switch (TJAPlayer3.stage選曲.n確定された曲の難易度[0])
                            {
                                case 0:
                                    strDiff = "かんたん ";
                                    break;
                                case 1:
                                    strDiff = "ふつう ";
                                    break;
                                case 2:
                                    strDiff = "むずかしい ";
                                    break;
                                case 3:
                                    strDiff = "おに ";
                                    break;
                                case 4:
                                    strDiff = "えでぃと ";
                                    break;
                                default:
                                    strDiff = "おに ";
                                    break;
                            }
                            bmpDiff = pfMusicName.DrawPrivateFont(strDiff + stageText, TJAPlayer3.Skin.SkinValue.Game_StageText_ForeColor.GetColor(), TJAPlayer3.Skin.SkinValue.Game_StageText_BackColor.GetColor());
                        }
                        else
                        {
                            bmpDiff = pfMusicName.DrawPrivateFont(stageText, TJAPlayer3.Skin.SkinValue.Game_StageText_ForeColor.GetColor(), TJAPlayer3.Skin.SkinValue.Game_StageText_BackColor.GetColor());
                        }

					    using (bmpDiff)
					    {
					        this.tx難易度とステージ数 = TJAPlayer3.CreateFDKTexture( bmpDiff, false );
					    }
					}
					catch( TextureCreateFailedException e )
					{
						Trace.TraceError( e.ToString() );
						Trace.TraceError( "パネル文字列テクスチャの生成に失敗しました。" );
						this.txPanel = null;
					}
				}
                if( !string.IsNullOrEmpty(genreName) )
                {
                    if(genreName.Equals( "アニメ" ) )
                    {
                        this.txGENRE = TJAPlayer3.Tx.TxCGen("Anime");
                    }
                    else if(genreName.Equals( "J-POP" ) )
                    {
                        this.txGENRE = TJAPlayer3.Tx.TxCGen("J-POP");
                    }
                    else if(genreName.Equals( "ゲームミュージック" ) )
                    {
                        this.txGENRE = TJAPlayer3.Tx.TxCGen("Game");
                    }
                    else if(genreName.Equals( "ナムコオリジナル" ) )
                    {
                        this.txGENRE = TJAPlayer3.Tx.TxCGen("Namco");
                    }
                    else if(genreName.Equals( "クラシック" ) )
                    {
                        this.txGENRE = TJAPlayer3.Tx.TxCGen("Classic");
                    }
                    else if(genreName.Equals( "どうよう" ) )
                    {
                        this.txGENRE = TJAPlayer3.Tx.TxCGen("Child");
                    }
                    else if(genreName.Equals( "バラエティ" ) )
                    {
                        this.txGENRE = TJAPlayer3.Tx.TxCGen("Variety");
                    }
                    else if(genreName.Equals( "ボーカロイド" ) || genreName.Equals( "VOCALOID" ) )
                    {
                        this.txGENRE = TJAPlayer3.Tx.TxCGen("Vocaloid");
                    }
                    else
                    {
                        using (var bmpDummy = new Bitmap( 1, 1 ))
                        {
                            this.txGENRE = TJAPlayer3.CreateFDKTexture( bmpDummy, true );
                        }
                    }
                }

			    this.ct進行用 = new Counter( 0, 2000, 2, TJAPlayer3.Timer );
				this.Start();



			}
		}

        public void t歌詞テクスチャを生成する( string str歌詞 )
        {
            using (var bmpleric = this.pf歌詞フォント.DrawPrivateFont( str歌詞, TJAPlayer3.Skin.SkinValue.Game_Lyric_ForeColor.GetColor(), TJAPlayer3.Skin.SkinValue.Game_Lyric_BackColor.GetColor()))
            {
                this.tx歌詞テクスチャ = TJAPlayer3.CreateFDKTexture( bmpleric, false );
            }
        }

        /// <summary>
        /// レイヤー管理のため、On進行描画から分離。
        /// </summary>
        public void t歌詞テクスチャを描画する()
        {
            if( this.tx歌詞テクスチャ != null )
            {
                if (TJAPlayer3.Skin.SkinValue.Game_Lyric_ReferencePoint == SkinManager.ReferencePoint.Left)
                {
                this.tx歌詞テクスチャ.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Lyric_X , TJAPlayer3.Skin.SkinValue.Game_Lyric_Y);
                }
                else if (TJAPlayer3.Skin.SkinValue.Game_Lyric_ReferencePoint == SkinManager.ReferencePoint.Right)
                {
                this.tx歌詞テクスチャ.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Lyric_X - this.tx歌詞テクスチャ.TextureSize.Width, TJAPlayer3.Skin.SkinValue.Game_Lyric_Y);
                }
                else
                {
                this.tx歌詞テクスチャ.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Lyric_X - (this.tx歌詞テクスチャ.TextureSize.Width / 2), TJAPlayer3.Skin.SkinValue.Game_Lyric_Y);
                }
            }
        }

		public void Stop()
		{
			this.bMute = true;
		}
		public void Start()
		{
			this.bMute = false;
		}


		// CActivity 実装

		public override void Activate()
		{
            if( !string.IsNullOrEmpty( TJAPlayer3._MainConfig.FontName ) )
            {
                this.pfMusicName = new CachePrivateFont( new FontFamily( TJAPlayer3._MainConfig.FontName), TJAPlayer3.Skin.SkinValue.Game_MusicName_FontSize);
                //this.pf縦書きテスト = new CPrivateFastFont( new FontFamily( CDTXMania.ConfigIni.strPrivateFontで使うフォント名 ), 22 );
            }
            else
                this.pfMusicName = new CachePrivateFont( new FontFamily("MS UI Gothic"), TJAPlayer3.Skin.SkinValue.Game_MusicName_FontSize);

            if( !string.IsNullOrEmpty(TJAPlayer3.Skin.SkinValue.Game_Lyric_FontName))
            {
                this.pf歌詞フォント = new CachePrivateFont(new FontFamily(TJAPlayer3.Skin.SkinValue.Game_Lyric_FontName), TJAPlayer3.Skin.SkinValue.Game_Lyric_FontSize);
            }
            else
            {
                this.pf歌詞フォント = new CachePrivateFont(new FontFamily("MS UI Gothic"), TJAPlayer3.Skin.SkinValue.Game_Lyric_FontSize);
            }

			this.txPanel = null;
			this.ct進行用 = new Counter();
			this.Start();
            this.bFirst = true;
			base.Activate();
		}
		public override void Deactivate()
		{
			this.ct進行用 = null;
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
				TJAPlayer3.DisposeFDKTexture( ref this.txPanel );
				TJAPlayer3.DisposeFDKTexture( ref this.txMusicName );
                TJAPlayer3.DisposeFDKTexture( ref this.txGENRE );
                TJAPlayer3.DisposeFDKTexture(ref this.txPanel);
                TJAPlayer3.DisposeFDKTexture(ref this.tx歌詞テクスチャ);
                TJAPlayer3.t安全にDisposeする(ref this.pfMusicName);
                TJAPlayer3.t安全にDisposeする(ref this.pf歌詞フォント);
                base.ManagedReleaseResources();
			}
		}
		public override int Draw()
		{
			throw new InvalidOperationException( "t進行描画(x,y)のほうを使用してください。" );
		}
		public int t進行描画( int x, int y )
		{
            if (TJAPlayer3.stage演奏ドラム画面.actDan.IsAnimating) return 0;
			if( !base.NotActivated && !this.bMute )
			{
				this.ct進行用.TickLoop();
                if( this.bFirst )
                {
                    this.ct進行用.NowValue = 300;
                }
                if( this.txGENRE != null )
                    this.txGENRE.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Genre_X, TJAPlayer3.Skin.SkinValue.Game_Genre_Y );

                if( TJAPlayer3.Skin.b現在のステージ数を表示しない )
                {
                    if( this.txMusicName != null )
                    {
                        float fRate = 660.0f / this.txMusicName.TextureSize.Width;
                        if (this.txMusicName.TextureSize.Width <= 660.0f)
                            fRate = 1.0f;
                        this.txMusicName.Scaling.X = fRate;
                        if (TJAPlayer3.Skin.SkinValue.Game_MusicName_ReferencePoint == SkinManager.ReferencePoint.Center)
                        {
                            this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_MusicName_X - ((this.txMusicName.TextureSize.Width * fRate) / 2), TJAPlayer3.Skin.SkinValue.Game_MusicName_Y);
                        }
                        else if (TJAPlayer3.Skin.SkinValue.Game_MusicName_ReferencePoint == SkinManager.ReferencePoint.Left)
                        {
                            this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_MusicName_X, TJAPlayer3.Skin.SkinValue.Game_MusicName_Y);
                        }
                        else
                        {
                            this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_MusicName_X - (this.txMusicName.TextureSize.Width * fRate), TJAPlayer3.Skin.SkinValue.Game_MusicName_Y);
                        }
                    }
                }
                else
                {
                    #region[ 透明度制御 ]

                    if( this.ct進行用.NowValue < 745 )
                    {
                        this.bFirst = false;
                        this.txMusicName.Opacity = 255;
                        if( this.txGENRE != null )
                            this.txGENRE.Opacity = 255;
                        this.tx難易度とステージ数.Opacity = 0;
                    }
                    else if( this.ct進行用.NowValue >= 745 && this.ct進行用.NowValue < 1000 )
                    {
                        this.txMusicName.Opacity = 255 - ( this.ct進行用.NowValue - 745 );
                        if( this.txGENRE != null )
                            this.txGENRE.Opacity = 255 - ( this.ct進行用.NowValue - 745 );
                        this.tx難易度とステージ数.Opacity = this.ct進行用.NowValue - 745;
                    }
                    else if( this.ct進行用.NowValue >= 1000 && this.ct進行用.NowValue <= 1745 )
                    {
                        this.txMusicName.Opacity = 0;
                        if( this.txGENRE != null )
                            this.txGENRE.Opacity = 0;
                        this.tx難易度とステージ数.Opacity = 255;
                    }
                    else if( this.ct進行用.NowValue >= 1745 )
                    {
                        this.txMusicName.Opacity = this.ct進行用.NowValue - 1745;
                        if( this.txGENRE != null )
                            this.txGENRE.Opacity = this.ct進行用.NowValue - 1745;
                        this.tx難易度とステージ数.Opacity = 255 - ( this.ct進行用.NowValue - 1745 );
                    }
                    #endregion
                    if( this.txMusicName != null )
                    {
                        if(this.JustStartedUpdate)
                        {
                            JustStartedUpdate = false;
                        }
                        if (TJAPlayer3.Skin.SkinValue.Game_MusicName_ReferencePoint == SkinManager.ReferencePoint.Center)
                        {
                            this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_MusicName_X - ((this.txMusicName.TextureSize.Width * txMusicName.Scaling.X) / 2), TJAPlayer3.Skin.SkinValue.Game_MusicName_Y);
                        }
                        else if (TJAPlayer3.Skin.SkinValue.Game_MusicName_ReferencePoint == SkinManager.ReferencePoint.Left)
                        {
                            this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_MusicName_X, TJAPlayer3.Skin.SkinValue.Game_MusicName_Y);
                        }
                        else
                        {
                            this.txMusicName.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_MusicName_X - (this.txMusicName.TextureSize.Width * txMusicName.Scaling.X), TJAPlayer3.Skin.SkinValue.Game_MusicName_Y);
                        }
                    }
                    if (this.tx難易度とステージ数 != null)
                        if (TJAPlayer3.Skin.SkinValue.Game_MusicName_ReferencePoint == SkinManager.ReferencePoint.Center)
                        {
                            this.tx難易度とステージ数.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_MusicName_X - (this.tx難易度とステージ数.TextureSize.Width / 2), TJAPlayer3.Skin.SkinValue.Game_MusicName_Y);
                        }
                        else if (TJAPlayer3.Skin.SkinValue.Game_MusicName_ReferencePoint == SkinManager.ReferencePoint.Left)
                        {
                            this.tx難易度とステージ数.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_MusicName_X, TJAPlayer3.Skin.SkinValue.Game_MusicName_Y);
                        }
                        else
                        {
                            this.tx難易度とステージ数.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_MusicName_X - this.tx難易度とステージ数.TextureSize.Width, TJAPlayer3.Skin.SkinValue.Game_MusicName_Y);
                        }
                }

                //CDTXMania.act文字コンソール.tPrint( 0, 0, C文字コンソール.Eフォント種別.白, this.ct進行用.n現在の値.ToString() );

				//this.txMusicName.t2D描画( CDTXMania.app.Device, 1250 - this.txMusicName.szテクスチャサイズ.Width, 14 );
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private Counter ct進行用;

		private FDKTexture txPanel;
		private bool bMute;
        private bool bFirst;

        private FDKTexture txMusicName;
        private FDKTexture tx難易度とステージ数;
        private FDKTexture txGENRE;
        private FDKTexture tx歌詞テクスチャ;
        private CachePrivateFont pfMusicName;
        private CachePrivateFont pf歌詞フォント;
		//-----------------
		#endregion
	}
}
　
