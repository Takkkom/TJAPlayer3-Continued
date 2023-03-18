using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using SlimDX;
using System.Drawing.Text;
using FDK;

namespace TJAPlayer3
{
	internal class SongLoadingScene : BaseScene
	{
		// コンストラクタ

		public SongLoadingScene()
		{
			base.eステージID = BaseScene.Eステージ.曲読み込み;
			base.eフェーズID = BaseScene.Eフェーズ.共通_通常状態;
			base.NotActivated = true;
			//base.list子Activities.Add( this.actFI = new CActFIFOBlack() );	// #27787 2012.3.10 yyagi 曲読み込み画面のフェードインの省略
			//base.list子Activities.Add( this.actFO = new CActFIFOBlack() );
		}


		// CStage 実装

		public override void Activate()
		{
			Trace.TraceInformation( "曲読み込みステージを活性化します。" );
			Trace.Indent();
			try
			{
				this.str曲タイトル = "";
				this.strSTAGEFILE = "";
                if( !string.IsNullOrEmpty( TJAPlayer3._MainConfig.FontName ) )
                {
                    this.pfTITLE = new CachePrivateFont( new FontFamily( TJAPlayer3._MainConfig.FontName ), TJAPlayer3.Skin.SongLoading_Title_FontSize );
                    this.pfSUBTITLE = new CachePrivateFont( new FontFamily( TJAPlayer3._MainConfig.FontName ), TJAPlayer3.Skin.SongLoading_SubTitle_FontSize);
                }
                else
                {
                    this.pfTITLE = new CachePrivateFont( new FontFamily("MS UI Gothic"), TJAPlayer3.Skin.SongLoading_Title_FontSize);
                    this.pfSUBTITLE = new CachePrivateFont( new FontFamily("MS UI Gothic" ), TJAPlayer3.Skin.SongLoading_SubTitle_FontSize);
                }
				this.nBGM再生開始時刻 = -1;
				this.nBGMの総再生時間ms = 0;
				if( this.sd読み込み音 != null )
				{
					TJAPlayer3._SoundManager.DisposeSound( this.sd読み込み音 );
					this.sd読み込み音 = null;
				}

			    if (TJAPlayer3.bコンパクトモード)
			    {
			        string strDTXファイルパス = TJAPlayer3.strコンパクトモードファイル;
				
			        Chart cdtx = new Chart( strDTXファイルパス, true, 1.0, 0, 0 );
			        if( File.Exists( cdtx.strフォルダ名 + @"set.def" ) )
			            cdtx = new Chart( strDTXファイルパス, true, 1.0, 0, 1 );

			        this.str曲タイトル = cdtx.TITLE;
			        this.strサブタイトル = cdtx.SUBTITLE;

			        cdtx.Deactivate();
			    }
			    else
			    {
			        string strDTXファイルパス = TJAPlayer3.stage選曲.r確定されたスコア.ファイル情報.ファイルの絶対パス;

			        var strフォルダ名 = Path.GetDirectoryName(strDTXファイルパス) + @"\";

			        if (File.Exists(strフォルダ名 + @"set.def"))
			        {
			            var cdtx = new Chart(strDTXファイルパス, true, 1.0, 0, 1);

			            this.str曲タイトル = cdtx.TITLE;
			            this.strサブタイトル = cdtx.SUBTITLE;

			            cdtx.Deactivate();
			        }
			        else
			        {
			            var 譜面情報 = TJAPlayer3.stage選曲.r確定されたスコア.譜面情報;
			            this.str曲タイトル = 譜面情報.タイトル;
			            this.strサブタイトル = 譜面情報.strサブタイトル;
			        }
			    }

			    // For the moment, detect that we are performing
			    // calibration via there being an actual single
			    // player and the special song title and subtitle
			    // of the .tja used to perform input calibration
			    TJAPlayer3.IsPerformingCalibration =
			        !TJAPlayer3._MainConfig.b太鼓パートAutoPlay &&
			        TJAPlayer3._MainConfig.nPlayerCount == 1 &&
			        str曲タイトル == "Input Calibration" &&
			        strサブタイトル == "TJAPlayer3 Developers";

				this.strSTAGEFILE = SkinManager.Path(@"Graphics\SongLoading\Background.png");

				base.Activate();
			}
			finally
			{
				Trace.TraceInformation( "曲読み込みステージの活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void Deactivate()
		{
			Trace.TraceInformation( "曲読み込みステージを非活性化します。" );
			Trace.Indent();
			try
			{
                TJAPlayer3.t安全にDisposeする(ref this.pfTITLE);
                TJAPlayer3.t安全にDisposeする(ref this.pfSUBTITLE);
                base.Deactivate();
			}
			finally
			{
				Trace.TraceInformation( "曲読み込みステージの非活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
				this.tx背景 = TJAPlayer3.tテクスチャの生成( this.strSTAGEFILE, false );
                //this.txSongnamePlate = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\6_SongnamePlate.png" ) );
                this.ct待機 = new Counter( 0, 600, 5, TJAPlayer3.Timer );
                this.ct曲名表示 = new Counter( 1, 30, 30, TJAPlayer3.Timer );
				try
				{
				    // When performing calibration, inform the player that
				    // calibration is about to begin, rather than
				    // displaying the song title and subtitle as usual.

				    var タイトル = TJAPlayer3.IsPerformingCalibration
				        ? "Input calibration is about to begin."
				        : this.str曲タイトル;

				    var サブタイトル = TJAPlayer3.IsPerformingCalibration
				        ? "Please play as accurately as possible."
				        : this.strサブタイトル;

				    if( !string.IsNullOrEmpty(タイトル) )
					{
                        //this.txタイトル = new CTexture( CDTXMania.app.Device, image, CDTXMania.TextureFormat );
                        //this.txタイトル.vc拡大縮小倍率 = new Vector3( 0.5f, 0.5f, 1f );


					    using (var bmpSongTitle = this.pfTITLE.DrawPrivateFont( タイトル, TJAPlayer3.Skin.SongLoading_Title_ForeColor, TJAPlayer3.Skin.SongLoading_Title_BackColor ))

					    {
					        this.txタイトル = new FDKTexture( TJAPlayer3.app.Device, bmpSongTitle, TJAPlayer3.TextureFormat, false );
					        txタイトル.Scaling.X = TJAPlayer3.GetSongNameXScaling(ref txタイトル, 710);
					    }

					    using (var bmpSongSubTitle = this.pfSUBTITLE.DrawPrivateFont( サブタイトル, TJAPlayer3.Skin.SongLoading_SubTitle_ForeColor, TJAPlayer3.Skin.SongLoading_SubTitle_BackColor ))


					    {
					        this.txサブタイトル = new FDKTexture( TJAPlayer3.app.Device, bmpSongSubTitle, TJAPlayer3.TextureFormat, false );
					    }
                    }
					else
					{
						this.txタイトル = null;
                        this.txサブタイトル = null;
                    }

                }
                catch ( TextureCreateFailedException e )
				{
					Trace.TraceError( e.ToString() );
					Trace.TraceError( "テクスチャの生成に失敗しました。({0})", new object[] { this.strSTAGEFILE } );
					this.txタイトル = null;
                    this.txサブタイトル = null;
                    this.tx背景 = null;
				}
				base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if( !base.NotActivated )
			{
				TJAPlayer3.tテクスチャの解放( ref this.tx背景 );
				TJAPlayer3.tテクスチャの解放( ref this.txタイトル );
				//CDTXMania.tテクスチャの解放( ref this.txSongnamePlate );
                TJAPlayer3.tテクスチャの解放( ref this.txサブタイトル );
				base.ManagedReleaseResources();
			}
		}
		public override int Draw()
		{
			string str;

			if( base.NotActivated )
				return 0;

			#region [ 初めての進行描画 ]
			//-----------------------------
			if( base.JustStartedUpdate )
			{
				ScoreInfo cスコア1 = TJAPlayer3.stage選曲.r確定されたスコア;
				if( this.sd読み込み音 != null )
				{
					if( TJAPlayer3.Skin.sound曲読込開始音.b排他 && ( SkinManager.Cシステムサウンド.r最後に再生した排他システムサウンド != null ) )
					{
						SkinManager.Cシステムサウンド.r最後に再生した排他システムサウンド.t停止する();
					}
					this.sd読み込み音.t再生を開始する();
					this.nBGM再生開始時刻 = SoundManager.PlayTimer.n現在時刻;
					this.nBGMの総再生時間ms = this.sd読み込み音.TotalPlayTimeMs;
				}
				else
				{
					TJAPlayer3.Skin.sound曲読込開始音.t再生する();
					this.nBGM再生開始時刻 = SoundManager.PlayTimer.n現在時刻;
					this.nBGMの総再生時間ms = TJAPlayer3.Skin.sound曲読込開始音.n長さ_現在のサウンド;
				}
				//this.actFI.tフェードイン開始();							// #27787 2012.3.10 yyagi 曲読み込み画面のフェードインの省略
				base.eフェーズID = BaseScene.Eフェーズ.共通_フェードイン;
				base.JustStartedUpdate = false;

				nWAVcount = 1;
				bitmapFilename = new Bitmap( 640, 24 );
				graphicsFilename = Graphics.FromImage( bitmapFilename );
				graphicsFilename.TextRenderingHint = TextRenderingHint.AntiAlias;
				ftFilename = new Font("MS UI Gothic", 24f, FontStyle.Bold, GraphicsUnit.Pixel );
			}
			//-----------------------------
			#endregion
            this.ct待機.Tick();



			#region [ ESC押下時は選曲画面に戻る ]
			if ( tキー入力() )
			{
				if ( this.sd読み込み音 != null )
				{
					this.sd読み込み音.StopFDKSound();
					this.sd読み込み音.DisposeSound();
				}
				return (int) E曲読込画面の戻り値.読込中止;
			}
			#endregion

			#region [ 背景、音符＋タイトル表示 ]
			//-----------------------------
            this.ct曲名表示.Tick();
			if( this.tx背景 != null )
				this.tx背景.Draw2D( TJAPlayer3.app.Device, 0, 0 );
            //CDTXMania.act文字コンソール.tPrint( 0, 0, C文字コンソール.Eフォント種別.灰, this.ct曲名表示.n現在の値.ToString() );

            if (TJAPlayer3.Tx.SongLoading_Plate != null)
            {
                TJAPlayer3.Tx.SongLoading_Plate.IsScreenBlend = TJAPlayer3.Skin.SongLoading_Plate_ScreenBlend; //あまりにも出番が無い
                TJAPlayer3.Tx.SongLoading_Plate.Opacity = ConvertUtility.ParsentTo255((this.ct曲名表示.NowValue / 30.0));
                if(TJAPlayer3.Skin.SongLoading_Plate_ReferencePoint == SkinManager.ReferencePoint.Left)
                {
                TJAPlayer3.Tx.SongLoading_Plate.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongLoading_Plate_X, TJAPlayer3.Skin.SongLoading_Plate_Y - (TJAPlayer3.Tx.SongLoading_Plate.ImageSize.Height / 2));
                }
                else if(TJAPlayer3.Skin.SongLoading_Plate_ReferencePoint == SkinManager.ReferencePoint.Right)
                {
                TJAPlayer3.Tx.SongLoading_Plate.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongLoading_Plate_X - TJAPlayer3.Tx.SongLoading_Plate.ImageSize.Width, TJAPlayer3.Skin.SongLoading_Plate_Y - (TJAPlayer3.Tx.SongLoading_Plate.ImageSize.Height / 2));
                }
                else
                {
                TJAPlayer3.Tx.SongLoading_Plate.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongLoading_Plate_X - (TJAPlayer3.Tx.SongLoading_Plate.ImageSize.Width / 2), TJAPlayer3.Skin.SongLoading_Plate_Y - (TJAPlayer3.Tx.SongLoading_Plate.ImageSize.Height / 2));
                }
            }
            //CDTXMania.act文字コンソール.tPrint( 0, 16, C文字コンソール.Eフォント種別.灰, C変換.nParsentTo255( ( this.ct曲名表示.n現在の値 / 30.0 ) ).ToString() );


			int y = 720 - 45;
			if( this.txタイトル != null )
			{
                int nサブタイトル補正 = string.IsNullOrEmpty(TJAPlayer3.stage選曲.r確定されたスコア.譜面情報.strサブタイトル) ? 15 : 0;

                this.txタイトル.Opacity = ConvertUtility.ParsentTo255( ( this.ct曲名表示.NowValue / 30.0 ) );
                if(TJAPlayer3.Skin.SongLoading_Title_ReferencePoint == SkinManager.ReferencePoint.Left)
                {
                    this.txタイトル.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongLoading_Title_X, TJAPlayer3.Skin.SongLoading_Title_Y - (this.txタイトル.ImageSize.Height / 2) + nサブタイトル補正);
                }
                else if(TJAPlayer3.Skin.SongLoading_Title_ReferencePoint == SkinManager.ReferencePoint.Right)
                {
                    this.txタイトル.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongLoading_Title_X - (this.txタイトル.ImageSize.Width * txタイトル.Scaling.X), TJAPlayer3.Skin.SongLoading_Title_Y - (this.txタイトル.ImageSize.Height / 2) + nサブタイトル補正);
                }
                else
                {
                    this.txタイトル.Draw2D(TJAPlayer3.app.Device, (TJAPlayer3.Skin.SongLoading_Title_X - ((this.txタイトル.ImageSize.Width * txタイトル.Scaling.X) / 2)), TJAPlayer3.Skin.SongLoading_Title_Y - (this.txタイトル.ImageSize.Height / 2) + nサブタイトル補正);
                }
            }
			if( this.txサブタイトル != null )
			{
                this.txサブタイトル.Opacity = ConvertUtility.ParsentTo255( ( this.ct曲名表示.NowValue / 30.0 ) );
                if(TJAPlayer3.Skin.SongLoading_SubTitle_ReferencePoint == SkinManager.ReferencePoint.Left)
                {
                    this.txサブタイトル.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongLoading_SubTitle_X, TJAPlayer3.Skin.SongLoading_SubTitle_Y - (this.txサブタイトル.ImageSize.Height / 2));
                }
                else if(TJAPlayer3.Skin.SongLoading_Title_ReferencePoint == SkinManager.ReferencePoint.Right)
                {
                    this.txサブタイトル.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongLoading_SubTitle_X - (this.txサブタイトル.ImageSize.Width * txタイトル.Scaling.X), TJAPlayer3.Skin.SongLoading_SubTitle_Y - (this.txサブタイトル.ImageSize.Height / 2));
                }
                else
                {
                    this.txサブタイトル.Draw2D(TJAPlayer3.app.Device, (TJAPlayer3.Skin.SongLoading_SubTitle_X - ((this.txサブタイトル.ImageSize.Width * txサブタイトル.Scaling.X) / 2)), TJAPlayer3.Skin.SongLoading_SubTitle_Y - (this.txサブタイトル.ImageSize.Height / 2));
                }
            }
			//-----------------------------
			#endregion

			switch( base.eフェーズID )
			{
				case BaseScene.Eフェーズ.共通_フェードイン:
					//if( this.actFI.On進行描画() != 0 )			    // #27787 2012.3.10 yyagi 曲読み込み画面のフェードインの省略
																		// 必ず一度「CStaeg.Eフェーズ.共通_フェードイン」フェーズを経由させること。
																		// さもないと、曲読み込みが完了するまで、曲読み込み画面が描画されない。
						base.eフェーズID = BaseScene.Eフェーズ.NOWLOADING_DTXファイルを読み込む;
					return (int) E曲読込画面の戻り値.継続;

				case BaseScene.Eフェーズ.NOWLOADING_DTXファイルを読み込む:
					{
						timeBeginLoad = DateTime.Now;
						TimeSpan span;
						str = null;
						if( !TJAPlayer3.bコンパクトモード )
							str = TJAPlayer3.stage選曲.r確定されたスコア.ファイル情報.ファイルの絶対パス;
						else
							str = TJAPlayer3.strコンパクトモードファイル;

						ScoreIni ini = new ScoreIni( str + ".score.ini" );
						ini.t全演奏記録セクションの整合性をチェックし不整合があればリセットする();

						if( ( TJAPlayer3.DTX != null ) && TJAPlayer3.DTX.IsActivated )
							TJAPlayer3.DTX.Deactivate();

                        //if( CDTXMania.DTX == null )
                        {
						    TJAPlayer3.DTX = new Chart( str, false, ( (double) TJAPlayer3._MainConfig.n演奏速度 ) / 20.0, ini.stファイル.BGMAdjust, 0, 0, true );
                            if( TJAPlayer3._MainConfig.nPlayerCount == 2 )
						        TJAPlayer3.DTX_2P = new Chart( str, false, ( (double) TJAPlayer3._MainConfig.n演奏速度 ) / 20.0, ini.stファイル.BGMAdjust, 0, 1, true );
                            if( File.Exists( TJAPlayer3.DTX.strフォルダ名 + @"\\set.def" ) )
                            {
						        TJAPlayer3.DTX = new Chart( str, false, ( (double) TJAPlayer3._MainConfig.n演奏速度 ) / 20.0, ini.stファイル.BGMAdjust, 0, 1, true );
                                if( TJAPlayer3._MainConfig.nPlayerCount == 2 )
						            TJAPlayer3.DTX_2P = new Chart( str, false, ( (double) TJAPlayer3._MainConfig.n演奏速度 ) / 20.0, ini.stファイル.BGMAdjust, 0, 1, true );
                            }

					    	Trace.TraceInformation( "----曲情報-----------------" );
				    		Trace.TraceInformation( "TITLE: {0}", TJAPlayer3.DTX.TITLE );
			    			Trace.TraceInformation( "FILE: {0}",  TJAPlayer3.DTX.strファイル名の絶対パス );
		    				Trace.TraceInformation( "---------------------------" );

	    					span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
    						Trace.TraceInformation( "DTX読込所要時間:           {0}", span.ToString() );

                            // 段位認定モード用。
                            if (TJAPlayer3.stage選曲.n確定された曲の難易度 == (int)Difficulty.Dan && TJAPlayer3.DTX.List_DanSongs != null)
                            {
                                var pfTitle = new PrivateFont();
                                var pfSubTitle = new PrivateFont();
                                if (!string.IsNullOrEmpty(TJAPlayer3._MainConfig.FontName))
                                {
                                    pfTitle = new PrivateFont(new FontFamily(TJAPlayer3._MainConfig.FontName), 30);
                                    pfSubTitle = new PrivateFont(new FontFamily(TJAPlayer3._MainConfig.FontName), 22);
                                }
                                else
                                {
                                    pfTitle = new PrivateFont(new FontFamily("MS UI Gothic"), 30);
                                    pfSubTitle = new PrivateFont(new FontFamily("MS UI Gothic"), 22);
                                }

                                var titleForeColor = TJAPlayer3.Skin.Game_DanC_Title_ForeColor;
                                var titleBackColor = TJAPlayer3.Skin.Game_DanC_Title_BackColor;
                                var subtitleForeColor = TJAPlayer3.Skin.Game_DanC_SubTitle_ForeColor;
                                var subtitleBackColor = TJAPlayer3.Skin.Game_DanC_SubTitle_BackColor;

                                for (int i = 0; i < TJAPlayer3.DTX.List_DanSongs.Count; i++)
                                {
                                    if (!string.IsNullOrEmpty(TJAPlayer3.DTX.List_DanSongs[i].Title))
                                    {
                                        using (var bmpSongTitle = pfTitle.DrawPrivateFont(TJAPlayer3.DTX.List_DanSongs[i].Title, titleForeColor, titleBackColor))
                                        {
                                            TJAPlayer3.DTX.List_DanSongs[i].TitleTex = TJAPlayer3.tテクスチャの生成(bmpSongTitle, false);
                                            TJAPlayer3.DTX.List_DanSongs[i].TitleTex.Scaling.X = TJAPlayer3.GetSongNameXScaling(ref TJAPlayer3.DTX.List_DanSongs[i].TitleTex, 710);
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(TJAPlayer3.DTX.List_DanSongs[i].SubTitle))
                                    {
                                        using (var bmpSongSubTitle = pfSubTitle.DrawPrivateFont(TJAPlayer3.DTX.List_DanSongs[i].SubTitle, subtitleForeColor, subtitleBackColor))
                                        {
                                            TJAPlayer3.DTX.List_DanSongs[i].SubTitleTex = TJAPlayer3.tテクスチャの生成(bmpSongSubTitle, false);
                                            TJAPlayer3.DTX.List_DanSongs[i].SubTitleTex.Scaling.X = TJAPlayer3.GetSongNameXScaling(ref TJAPlayer3.DTX.List_DanSongs[i].SubTitleTex, 710);
                                        }
                                    }

                                }

                                pfTitle?.Dispose();
                                pfSubTitle?.Dispose();
                            }
                        }

                        //2017.01.28 DD Config.iniに反映しないように変更
                        /*
                        switch( CDTXMania.DTX.nScoreModeTmp )
                        {
                            case 0:
                                CDTXMania.ConfigIni.nScoreMode = 0;
                                break;
                            case 1:
                                CDTXMania.ConfigIni.nScoreMode = 1;
                                break;
                            case 2:
                                CDTXMania.ConfigIni.nScoreMode = 2;
                                break;
                            case -1:
                                CDTXMania.ConfigIni.nScoreMode = 1;
                                break;
                        }
                        */

                        base.eフェーズID = BaseScene.Eフェーズ.NOWLOADING_WAV読み込み待機;
						timeBeginLoadWAV = DateTime.Now;
						return (int) E曲読込画面の戻り値.継続;
					}

                case BaseScene.Eフェーズ.NOWLOADING_WAV読み込み待機:
                    {
                        if( this.ct待機.NowValue > 260 )
                        {
						    base.eフェーズID = BaseScene.Eフェーズ.NOWLOADING_WAVファイルを読み込む;
                        }
						return (int) E曲読込画面の戻り値.継続;
                    }

				case BaseScene.Eフェーズ.NOWLOADING_WAVファイルを読み込む:
					{
						if ( nWAVcount == 1 && TJAPlayer3.DTX.listWAV.Count > 0 )			// #28934 2012.7.7 yyagi (added checking Count)
						{
							ShowProgressByFilename( TJAPlayer3.DTX.listWAV[ nWAVcount ].strファイル名 );
						}
						int looptime = (TJAPlayer3._MainConfig.b垂直帰線待ちを行う)? 3 : 1;	// VSyncWait=ON時は1frame(1/60s)あたり3つ読むようにする
						for ( int i = 0; i < looptime && nWAVcount <= TJAPlayer3.DTX.listWAV.Count; i++ )
						{
							if ( TJAPlayer3.DTX.listWAV[ nWAVcount ].listこのWAVを使用するチャンネル番号の集合.Count > 0 )	// #28674 2012.5.8 yyagi
							{
								TJAPlayer3.DTX.tWAVの読み込み( TJAPlayer3.DTX.listWAV[ nWAVcount ] );
							}
							nWAVcount++;
						}
						if ( nWAVcount <= TJAPlayer3.DTX.listWAV.Count )
						{
							ShowProgressByFilename( TJAPlayer3.DTX.listWAV[ nWAVcount ].strファイル名 );
						}
						if ( nWAVcount > TJAPlayer3.DTX.listWAV.Count )
						{
							TimeSpan span = ( TimeSpan ) ( DateTime.Now - timeBeginLoadWAV );
							Trace.TraceInformation( "WAV読込所要時間({0,4}):     {1}", TJAPlayer3.DTX.listWAV.Count, span.ToString() );
							timeBeginLoadWAV = DateTime.Now;

							if ( TJAPlayer3._MainConfig.bDynamicBassMixerManagement )
							{
								TJAPlayer3.DTX.PlanToAddMixerChannel();
							}
                            TJAPlayer3.DTX.t太鼓チップのランダム化( TJAPlayer3._MainConfig.eRandom.Taiko );

							TJAPlayer3.stage演奏ドラム画面.Activate();

							span = (TimeSpan) ( DateTime.Now - timeBeginLoadWAV );

							base.eフェーズID = BaseScene.Eフェーズ.NOWLOADING_BMPファイルを読み込む;
						}
						return (int) E曲読込画面の戻り値.継続;
					}

				case BaseScene.Eフェーズ.NOWLOADING_BMPファイルを読み込む:
					{
						TimeSpan span;
						DateTime timeBeginLoadBMPAVI = DateTime.Now;

						if ( TJAPlayer3._MainConfig.bAVI有効 )
							TJAPlayer3.DTX.tAVIの読み込み();
						span = ( TimeSpan ) ( DateTime.Now - timeBeginLoadBMPAVI );

						span = ( TimeSpan ) ( DateTime.Now - timeBeginLoad );
						Trace.TraceInformation( "総読込時間:                {0}", span.ToString() );

                        if(TJAPlayer3._MainConfig.FastRender)
                        {
                            var fastRender = new FastRender();
                            fastRender.Render();
                            fastRender = null;
                        }


						if ( bitmapFilename != null )
						{
							bitmapFilename.Dispose();
							bitmapFilename = null;
						}
						if ( graphicsFilename != null )
						{
							graphicsFilename.Dispose();
							graphicsFilename = null;
						}
						if ( ftFilename != null )
						{
							ftFilename.Dispose();
							ftFilename = null;
						}
						TJAPlayer3.Timer.Update();
                        //CSound管理.rc演奏用タイマ.t更新();
						base.eフェーズID = BaseScene.Eフェーズ.NOWLOADING_システムサウンドBGMの完了を待つ;
						return (int) E曲読込画面の戻り値.継続;
					}

				case BaseScene.Eフェーズ.NOWLOADING_システムサウンドBGMの完了を待つ:
					{
						long nCurrentTime = TJAPlayer3.Timer.n現在時刻;
						if( nCurrentTime < this.nBGM再生開始時刻 )
							this.nBGM再生開始時刻 = nCurrentTime;

//						if ( ( nCurrentTime - this.nBGM再生開始時刻 ) > ( this.nBGMの総再生時間ms - 1000 ) )
						if ( ( nCurrentTime - this.nBGM再生開始時刻 ) >= ( this.nBGMの総再生時間ms ) )	// #27787 2012.3.10 yyagi 1000ms == フェードイン分の時間
						{
							if ( !TJAPlayer3.DTXVmode.Enabled )
							{
							}
							base.eフェーズID = BaseScene.Eフェーズ.共通_フェードアウト;
						}
						return (int) E曲読込画面の戻り値.継続;
					}

				case BaseScene.Eフェーズ.共通_フェードアウト:
					if ( this.ct待機.IsEndValueNotReached )		// DTXVモード時は、フェードアウト省略
						return (int)E曲読込画面の戻り値.継続;

					if ( txFilename != null )
					{
						txFilename.Dispose();
					}
					if ( this.sd読み込み音 != null )
					{
						this.sd読み込み音.DisposeSound();
					}
					return (int) E曲読込画面の戻り値.読込完了;
			}
			return (int) E曲読込画面の戻り値.継続;
		}

		/// <summary>
		/// ESC押下時、trueを返す
		/// </summary>
		/// <returns></returns>
		protected bool tキー入力()
		{
			IInputDevice keyboard = TJAPlayer3.Input管理.Keyboard;
			if 	( keyboard.GetKeyPressed( (int) SlimDX.DirectInput.Key.Escape ) )		// escape (exit)
			{
				return true;
			}
			return false;
		}


		private void ShowProgressByFilename(string strファイル名 )
		{
			if ( graphicsFilename != null && ftFilename != null )
			{
				graphicsFilename.Clear( Color.Transparent );
				graphicsFilename.DrawString( strファイル名, ftFilename, Brushes.White, new RectangleF( 0, 0, 640, 24 ) );
				if ( txFilename != null )
				{
					txFilename.Dispose();
				}
				txFilename = new FDKTexture( TJAPlayer3.app.Device, bitmapFilename, TJAPlayer3.TextureFormat );
				txFilename.Scaling = new Vector3( 0.5f, 0.5f, 1f );
				txFilename.Draw2D( TJAPlayer3.app.Device, 0, 720 - 16 );
			}
		}

		// その他

		#region [ private ]
		//-----------------
		//private CActFIFOBlack actFI;
		//private CActFIFOBlack actFO;
		private long nBGMの総再生時間ms;
		private long nBGM再生開始時刻;
		private FDKSound sd読み込み音;
		private string strSTAGEFILE;
		private string str曲タイトル;
        private string strサブタイトル;
		private FDKTexture txタイトル;
        private FDKTexture txサブタイトル;
		private FDKTexture tx背景;
        //private CTexture txSongnamePlate;
		private DateTime timeBeginLoad;
		private DateTime timeBeginLoadWAV;
		private int nWAVcount;
		private FDKTexture txFilename;
		private Bitmap bitmapFilename;
		private Graphics graphicsFilename;
		private Font ftFilename;
        private Counter ct待機;
        private Counter ct曲名表示;

        private CachePrivateFont pfTITLE;
        private CachePrivateFont pfSUBTITLE;
		//-----------------
		#endregion
	}
}
