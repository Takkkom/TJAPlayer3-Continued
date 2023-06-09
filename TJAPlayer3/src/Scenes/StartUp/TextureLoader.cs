using FDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TJAPlayer3
{
    class TextureLoader
    {
        const string BASE = @"Graphics\";

        // Stage
        const string TITLE = @"Title\";
        const string CONFIG = @"Config\";
        const string SONGSELECT = @"SongSelect\";
        const string SONGLOADING = @"SongLoading\";
        const string GAME = @"Game\";
        const string RESULT = @"Result\";
        const string EXIT = @"Exit\";

        // InGame
        const string CHARA = @"Chara\";
        const string DANCER = @"Dancer\";
        const string MOB = @"Mob\";
        const string COURSESYMBOL = @"CourseSymbol\";
        const string BACKGROUND = @"Background\";
        const string TAIKO = @"Taiko\";
        const string GAUGE = @"Gauge\";
        const string FOOTER = @"Footer\";
        const string END = @"End\";
        const string EFFECTS = @"Effects\";
        const string BALLOON = @"Balloon\";
        const string LANE = @"Lane\";
        const string GENRE = @"Genre\";
        const string GAMEMODE = @"GameMode\";
        const string FAILED = @"Failed\";
        const string RUNNER = @"Runner\";
        const string PUCHICHARA = @"PuchiChara\";
        const string DANC = @"DanC\";

        // InGame_Effects
        const string FIRE = @"Fire\";
        const string HIT = @"Hit\";
        const string ROLL = @"Roll\";
        const string SPLASH = @"Splash\";


        public TextureLoader()
        {
            // コンストラクタ

        }

        internal FDKTexture TxC(string FileName)
        {
            return TJAPlayer3.CreateFDKTexture(SkinManager.Path(BASE + FileName));
        }
        internal FDKTextureAf TxCAf(string FileName)
        {
            return TJAPlayer3.CreateFDKTextureAf(SkinManager.Path(BASE + FileName));
        }
        internal FDKTexture TxCGen(string FileName)
        {
            return TJAPlayer3.CreateFDKTexture(SkinManager.Path(BASE + GAME + GENRE + FileName + ".png"));
        }

        public void LoadTexture()
        {
            #region 共通
            Tile_Black = TxC(@"Tile_Black.png");
            Tile_White = TxC(@"Tile_White.png");
            Menu_Title = TxC(@"Menu_Title.png");
            Menu_Highlight = TxC(@"Menu_Highlight.png");
            Scanning_Loudness = TxC(@"Scanning_Loudness.png");
            Overlay = TxC(@"Overlay.png");
            NamePlate = new FDKTexture[2];
            NamePlate[0] = TxC(@"1P_NamePlate.png");
            NamePlate[1] = TxC(@"2P_NamePlate.png");
            #endregion
            #region 1_タイトル画面
            /*
            Title_Background = TxC(TITLE + @"Background.png");
            Title_Menu = TxC(TITLE + @"Menu.png");
            */
            #endregion

            #region 2_コンフィグ画面
            /*
            Config_Background = TxC(CONFIG + @"Background.png");
            Config_Cursor = TxC(CONFIG + @"Cursor.png");
            Config_ItemBox = TxC(CONFIG + @"ItemBox.png");
            Config_Arrow = TxC(CONFIG + @"Arrow.png");
            Config_KeyAssign = TxC(CONFIG + @"KeyAssign.png");
            Config_Font = TxC(CONFIG + @"Font.png");
            Config_Font_Bold = TxC(CONFIG + @"Font_Bold.png");
            Config_Enum_Song = TxC(CONFIG + @"Enum_Song.png");
            */
            #endregion

            #region 3_選曲画面
            /*
            SongSelect_Background = TxC(SONGSELECT + @"Background.png");
            SongSelect_Header = TxC(SONGSELECT + @"Header.png");
            SongSelect_Footer = TxC(SONGSELECT + @"Footer.png");
            SongSelect_Difficulty = TxC(SONGSELECT + @"Difficulty.png");
            SongSelect_Auto = TxC(SONGSELECT + @"Auto.png");
            SongSelect_Level = TxC(SONGSELECT + @"Level.png");
            SongSelect_Branch = TxC(SONGSELECT + @"Branch.png");
            SongSelect_Branch_Text = TxC(SONGSELECT + @"Branch_Text.png");
            SongSelect_Bar_Center = TxC(SONGSELECT + @"Bar_Center.png");
            SongSelect_Frame_Score = TxC(SONGSELECT + @"Frame_Score.png");
            SongSelect_Frame_Box = TxC(SONGSELECT + @"Frame_Box.png");
            SongSelect_Frame_BackBox = TxC(SONGSELECT + @"Frame_BackBox.png");
            SongSelect_Frame_Random = TxC(SONGSELECT + @"Frame_Random.png");
            SongSelect_Score_Select = TxC(SONGSELECT + @"Score_Select.png");
            //SongSelect_Frame_Dani = TxC(SONGSELECT + @"Frame_Dani.png");
            SongSelect_GenreText = TxC(SONGSELECT + @"GenreText.png");
            SongSelect_Cursor_Left = TxC(SONGSELECT + @"Cursor_Left.png");
            SongSelect_Cursor_Right = TxC(SONGSELECT + @"Cursor_Right.png");
            for (int i = 0; i < SongSelect_Bar_Genre.Length; i++)
            {
                SongSelect_Bar_Genre[i] = TxC(SONGSELECT + @"Bar_Genre_" + i.ToString() + ".png");
            }
            for (int i = 0; i < (int)Difficulty.Total; i++)
            {
                SongSelect_ScoreWindow[i] = TxC(SONGSELECT + @"ScoreWindow_" + i.ToString() + ".png");
            }

            for (int i = 0; i < SongSelect_GenreBack.Length; i++)
            {
                SongSelect_GenreBack[i] = TxC(SONGSELECT + @"GenreBackground_" + i.ToString() + ".png");
            }
            SongSelect_ScoreWindow_Text = TxC(SONGSELECT + @"ScoreWindow_Text.png");
            */
            #endregion

            #region 4_読み込み画面
            SongLoading_Plate = TxC(SONGLOADING + @"Plate.png");
            SongLoading_FadeIn = TxC(SONGLOADING + @"FadeIn.png");
            SongLoading_FadeOut = TxC(SONGLOADING + @"FadeOut.png");
            #endregion

            #region 5_演奏画面
            #region 共通
            Notes = TxC(GAME + @"Notes.png");
            Judge_Frame = TxC(GAME + @"Notes.png");
            SENotes = TxC(GAME + @"SENotes.png");
            Notes_Arm = TxC(GAME + @"Notes_Arm.png");
            Judge = TxC(GAME + @"Judge.png");

            Judge_Meter = TxC(GAME + @"Judge_Meter.png");
            Bar = TxC(GAME + @"Bar.png");
            Bar_Branch = TxC(GAME + @"Bar_Branch.png");

            #endregion
            #region キャラクター
            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Normal = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"Normal\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Normal != 0)
            {
                Chara_Normal = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Normal];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Normal; i++)
                {
                    Chara_Normal[i] = TxC(GAME + CHARA + @"Normal\" + i.ToString() + ".png");
                }
            }
            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Clear = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"Clear\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Clear != 0)
            {
                Chara_Normal_Cleared = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Clear];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Clear; i++)
                {
                    Chara_Normal_Cleared[i] = TxC(GAME + CHARA + @"Clear\" + i.ToString() + ".png");
                }
            }
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Clear != 0)
            {
                Chara_Normal_Maxed = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Clear];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Clear; i++)
                {
                    Chara_Normal_Maxed[i] = TxC(GAME + CHARA + @"Clear_Max\" + i.ToString() + ".png");
                }
            }
            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGo = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"GoGo\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGo != 0)
            {
                Chara_GoGoTime = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGo];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGo; i++)
                {
                    Chara_GoGoTime[i] = TxC(GAME + CHARA + @"GoGo\" + i.ToString() + ".png");
                }
            }
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGo != 0)
            {
                Chara_GoGoTime_Maxed = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGo];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGo; i++)
                {
                    Chara_GoGoTime_Maxed[i] = TxC(GAME + CHARA + @"GoGo_Max\" + i.ToString() + ".png");
                }
            }

            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_10combo = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"10combo\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_10combo != 0)
            {
                Chara_10Combo = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_10combo];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_10combo; i++)
                {
                    Chara_10Combo[i] = TxC(GAME + CHARA + @"10combo\" + i.ToString() + ".png");
                }
            }
            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_10combo_Max = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"10combo_Max\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_10combo_Max != 0)
            {
                Chara_10Combo_Maxed = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_10combo_Max];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_10combo_Max; i++)
                {
                    Chara_10Combo_Maxed[i] = TxC(GAME + CHARA + @"10combo_Max\" + i.ToString() + ".png");
                }
            }

            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGoStart = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"GoGoStart\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGoStart != 0)
            {
                Chara_GoGoStart = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGoStart];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGoStart; i++)
                {
                    Chara_GoGoStart[i] = TxC(GAME + CHARA + @"GoGoStart\" + i.ToString() + ".png");
                }
            }
            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGoStart_Max = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"GoGoStart_Max\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGoStart_Max != 0)
            {
                Chara_GoGoStart_Maxed = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGoStart_Max];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGoStart_Max; i++)
                {
                    Chara_GoGoStart_Maxed[i] = TxC(GAME + CHARA + @"GoGoStart_Max\" + i.ToString() + ".png");
                }
            }
            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_ClearIn = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"ClearIn\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_ClearIn != 0)
            {
                Chara_Become_Cleared = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_ClearIn];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_ClearIn; i++)
                {
                    Chara_Become_Cleared[i] = TxC(GAME + CHARA + @"ClearIn\" + i.ToString() + ".png");
                }
            }
            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_SoulIn = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"SoulIn\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_SoulIn != 0)
            {
                Chara_Become_Maxed = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_SoulIn];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_SoulIn; i++)
                {
                    Chara_Become_Maxed[i] = TxC(GAME + CHARA + @"SoulIn\" + i.ToString() + ".png");
                }
            }
            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Breaking = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"Balloon_Breaking\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Breaking != 0)
            {
                Chara_Balloon_Breaking = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Breaking];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Breaking; i++)
                {
                    Chara_Balloon_Breaking[i] = TxC(GAME + CHARA + @"Balloon_Breaking\" + i.ToString() + ".png");
                }
            }
            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Broke = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"Balloon_Broke\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Broke != 0)
            {
                Chara_Balloon_Broke = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Broke];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Broke; i++)
                {
                    Chara_Balloon_Broke[i] = TxC(GAME + CHARA + @"Balloon_Broke\" + i.ToString() + ".png");
                }
            }
            TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Miss = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + CHARA + @"Balloon_Miss\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Miss != 0)
            {
                Chara_Balloon_Miss = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Miss];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Miss; i++)
                {
                    Chara_Balloon_Miss[i] = TxC(GAME + CHARA + @"Balloon_Miss\" + i.ToString() + ".png");
                }
            }
            #endregion
            #region 踊り子
            TJAPlayer3.Skin.SkinValue.Game_Dancer_Ptn = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + DANCER + @"1\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Dancer_Ptn != 0)
            {
                Dancer = new FDKTexture[5][];
                for (int i = 0; i < 5; i++)
                {
                    Dancer[i] = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Dancer_Ptn];
                    for (int p = 0; p < TJAPlayer3.Skin.SkinValue.Game_Dancer_Ptn; p++)
                    {
                        Dancer[i][p] = TxC(GAME + DANCER + (i + 1) + @"\" + p.ToString() + ".png");
                    }
                }
            }
            #endregion
            #region モブ
            TJAPlayer3.Skin.SkinValue.Game_Mob_Ptn = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + MOB));
            Mob = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Mob_Ptn];
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Mob_Ptn; i++)
            {
                Mob[i] = TxC(GAME + MOB + i.ToString() + ".png");
            }
            #endregion
            #region フッター
            Mob_Footer = TxC(GAME + FOOTER + @"0.png");
            #endregion
            #region 背景
            Background = TxC(GAME + Background + @"0\" + @"Background.png");
            Background_Up = new FDKTexture[2];
            Background_Up[0] = TxC(GAME + BACKGROUND + @"0\" + @"1P_Up.png");
            Background_Up[1] = TxC(GAME + BACKGROUND + @"0\" + @"2P_Up.png");
            Background_Up_Clear = new FDKTexture[2];
            Background_Up_Clear[0] = TxC(GAME + BACKGROUND + @"0\" + @"1P_Up_Clear.png");
            Background_Up_Clear[1] = TxC(GAME + BACKGROUND + @"0\" + @"2P_Up_Clear.png");
            Background_Down = TxC(GAME + BACKGROUND + @"0\" + @"Down.png");
            Background_Down_Clear = TxC(GAME + BACKGROUND + @"0\" + @"Down_Clear.png");
            Background_Down_Scroll = TxC(GAME + BACKGROUND + @"0\" + @"Down_Scroll.png");

            #endregion
            #region 太鼓
            Taiko_Background = new FDKTexture[2];
            Taiko_Background[0] = TxC(GAME + TAIKO + @"1P_Background.png");
            Taiko_Background[1] = TxC(GAME + TAIKO + @"2P_Background.png");
            Taiko_Frame = new FDKTexture[2];
            Taiko_Frame[0] = TxC(GAME + TAIKO + @"1P_Frame.png");
            Taiko_Frame[1] = TxC(GAME + TAIKO + @"2P_Frame.png");
            Taiko_PlayerNumber = new FDKTexture[2];
            Taiko_PlayerNumber[0] = TxC(GAME + TAIKO + @"1P_PlayerNumber.png");
            Taiko_PlayerNumber[1] = TxC(GAME + TAIKO + @"2P_PlayerNumber.png");
            Taiko_NamePlate = new FDKTexture[2];
            Taiko_NamePlate[0] = TxC(GAME + TAIKO + @"1P_NamePlate.png");
            Taiko_NamePlate[1] = TxC(GAME + TAIKO + @"2P_NamePlate.png");
            Taiko_Base = TxC(GAME + TAIKO + @"Base.png");
            Taiko_Don_Left = TxC(GAME + TAIKO + @"Don.png");
            Taiko_Don_Right = TxC(GAME + TAIKO + @"Don.png");
            Taiko_Ka_Left = TxC(GAME + TAIKO + @"Ka.png");
            Taiko_Ka_Right = TxC(GAME + TAIKO + @"Ka.png");
            Taiko_LevelUp = TxC(GAME + TAIKO + @"LevelUp.png");
            Taiko_LevelDown = TxC(GAME + TAIKO + @"LevelDown.png");
            Couse_Symbol = new FDKTexture[(int)Difficulty.Total + 1]; // +1は真打ちモードの分
            string[] Couse_Symbols = new string[(int)Difficulty.Total + 1] { "Easy", "Normal", "Hard", "Oni", "Edit", "Tower", "Dan", "Shin" };
            for (int i = 0; i < (int)Difficulty.Total + 1; i++)
            {
                Couse_Symbol[i] = TxC(GAME + COURSESYMBOL + Couse_Symbols[i] + ".png");
            }
            Taiko_Score = new FDKTexture[3];
            Taiko_Score[0] = TxC(GAME + TAIKO + @"Score.png");
            Taiko_Score[1] = TxC(GAME + TAIKO + @"Score_1P.png");
            Taiko_Score[2] = TxC(GAME + TAIKO + @"Score_2P.png");
            Taiko_Combo = new FDKTexture[2];
            Taiko_Combo[0] = TxC(GAME + TAIKO + @"Combo.png");
            Taiko_Combo[1] = TxC(GAME + TAIKO + @"Combo_Big.png");
            Taiko_Combo_Effect = TxC(GAME + TAIKO + @"Combo_Effect.png");
            Taiko_Combo_Text = TxC(GAME + TAIKO + @"Combo_Text.png");
            #endregion
            #region ゲージ
            Gauge = new FDKTexture[2];
            Gauge[0] = TxC(GAME + GAUGE + @"1P.png");
            Gauge[1] = TxC(GAME + GAUGE + @"2P.png");
            Gauge_Base = new FDKTexture[2];
            Gauge_Base[0] = TxC(GAME + GAUGE + @"1P_Base.png");
            Gauge_Base[1] = TxC(GAME + GAUGE + @"2P_Base.png");
            Gauge_Line = new FDKTexture[2];
            Gauge_Line[0] = TxC(GAME + GAUGE + @"1P_Line.png");
            Gauge_Line[1] = TxC(GAME + GAUGE + @"2P_Line.png");
            TJAPlayer3.Skin.SkinValue.Game_Gauge_Rainbow_Ptn = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + GAUGE + @"Rainbow\"));
            if (TJAPlayer3.Skin.SkinValue.Game_Gauge_Rainbow_Ptn != 0)
            {
                Gauge_Rainbow = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Gauge_Rainbow_Ptn];
                for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Gauge_Rainbow_Ptn; i++)
                {
                    Gauge_Rainbow[i] = TxC(GAME + GAUGE + @"Rainbow\" + i.ToString() + ".png");
                }
            }
            Gauge_Soul = TxC(GAME + GAUGE + @"Soul.png");
            Gauge_Soul_Fire = TxC(GAME + GAUGE + @"Fire.png");
            Gauge_Soul_Explosion = new FDKTexture[2];
            Gauge_Soul_Explosion[0] = TxC(GAME + GAUGE + @"1P_Explosion.png");
            Gauge_Soul_Explosion[1] = TxC(GAME + GAUGE + @"2P_Explosion.png");
            #endregion
            #region 吹き出し
            Balloon_Combo = new FDKTexture[2];
            Balloon_Combo[0] = TxC(GAME + BALLOON + @"Combo_1P.png");
            Balloon_Combo[1] = TxC(GAME + BALLOON + @"Combo_2P.png");
            Balloon_Roll = TxC(GAME + BALLOON + @"Roll.png");
            Balloon_Balloon = TxC(GAME + BALLOON + @"Balloon.png");
            Balloon_Number_Roll = TxC(GAME + BALLOON + @"Number_Roll.png");
            Balloon_Number_Combo = TxC(GAME + BALLOON + @"Number_Combo.png");

            Balloon_Breaking = new FDKTexture[6];
            for (int i = 0; i < 6; i++)
            {
                Balloon_Breaking[i] = TxC(GAME + BALLOON + @"Breaking_" + i.ToString() + ".png");
            }
            #endregion
            #region エフェクト
            Effects_Hit_Explosion = TxCAf(GAME + EFFECTS + @"Hit\Explosion.png");
            if (Effects_Hit_Explosion != null) Effects_Hit_Explosion.IsAddBlend = TJAPlayer3.Skin.SkinValue.Game_Effect_HitExplosion_AddBlend;
            Effects_Hit_Explosion_Big = TxC(GAME + EFFECTS + @"Hit\Explosion_Big.png");
            if (Effects_Hit_Explosion_Big != null) Effects_Hit_Explosion_Big.IsAddBlend = TJAPlayer3.Skin.SkinValue.Game_Effect_HitExplosionBig_AddBlend;
            Effects_Hit_FireWorks = TxC(GAME + EFFECTS + @"Hit\FireWorks.png");
            if (Effects_Hit_FireWorks != null) Effects_Hit_FireWorks.IsAddBlend = TJAPlayer3.Skin.SkinValue.Game_Effect_FireWorks_AddBlend;


            Effects_Fire = TxC(GAME + EFFECTS + @"Fire.png");
            if (Effects_Fire != null) Effects_Fire.IsAddBlend = TJAPlayer3.Skin.SkinValue.Game_Effect_Fire_AddBlend;

            Effects_Rainbow = TxC(GAME + EFFECTS + @"Rainbow.png");

            Effects_GoGoSplash = TxC(GAME + EFFECTS + @"GoGoSplash.png");
            if (Effects_GoGoSplash != null) Effects_GoGoSplash.IsAddBlend = TJAPlayer3.Skin.SkinValue.Game_Effect_GoGoSplash_AddBlend;
            Effects_Hit_Great = new FDKTexture[15];
            Effects_Hit_Great_Big = new FDKTexture[15];
            Effects_Hit_Good = new FDKTexture[15];
            Effects_Hit_Good_Big = new FDKTexture[15];
            for (int i = 0; i < 15; i++)
            {
                Effects_Hit_Great[i] = TxC(GAME + EFFECTS + @"Hit\" + @"Great\" + i.ToString() + ".png");
                Effects_Hit_Great_Big[i] = TxC(GAME + EFFECTS + @"Hit\" + @"Great_Big\" + i.ToString() + ".png");
                Effects_Hit_Good[i] = TxC(GAME + EFFECTS + @"Hit\" + @"Good\" + i.ToString() + ".png");
                Effects_Hit_Good_Big[i] = TxC(GAME + EFFECTS + @"Hit\" + @"Good_Big\" + i.ToString() + ".png");
            }
            TJAPlayer3.Skin.SkinValue.Game_Effect_Roll_Ptn = TJAPlayer3.t連番画像の枚数を数える(SkinManager.Path(BASE + GAME + EFFECTS + @"Roll\"));
            Effects_Roll = new FDKTexture[TJAPlayer3.Skin.SkinValue.Game_Effect_Roll_Ptn];
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Effect_Roll_Ptn; i++)
            {
                Effects_Roll[i] = TxC(GAME + EFFECTS + @"Roll\" + i.ToString() + ".png");
            }
            #endregion
            #region レーン
            Lane_Base = new FDKTexture[3];
            Lane_Text = new FDKTexture[3];
            string[] Lanes = new string[3] { "Normal", "Expert", "Master" };
            for (int i = 0; i < 3; i++)
            {
                Lane_Base[i] = TxC(GAME + LANE + "Base_" + Lanes[i] + ".png");
                Lane_Text[i] = TxC(GAME + LANE + "Text_" + Lanes[i] + ".png");
            }
            Lane_Red = TxC(GAME + LANE + @"Red.png");
            Lane_Blue = TxC(GAME + LANE + @"Blue.png");
            Lane_Yellow = TxC(GAME + LANE + @"Yellow.png");
            Lane_Background_Main = TxC(GAME + LANE + @"Background_Main.png");
            Lane_Background_Sub = TxC(GAME + LANE + @"Background_Sub.png");
            Lane_Background_GoGo = TxC(GAME + LANE + @"Background_GoGo.png");

            #endregion
            #region 終了演出
            End_Clear_L = new FDKTexture[5];
            End_Clear_R = new FDKTexture[5];
            for (int i = 0; i < 5; i++)
            {
                End_Clear_L[i] = TxC(GAME + END + @"Clear_L_" + i.ToString() + ".png");
                End_Clear_R[i] = TxC(GAME + END + @"Clear_R_" + i.ToString() + ".png");
            }
            End_Clear_Text = TxC(GAME + END + @"Clear_Text.png");
            End_Clear_Text_Effect = TxC(GAME + END + @"Clear_Text_Effect.png");
            if (End_Clear_Text_Effect != null) End_Clear_Text_Effect.IsAddBlend = true;
            #endregion
            #region ゲームモード
            GameMode_Timer_Tick = TxC(GAME + GAMEMODE + @"Timer_Tick.png");
            GameMode_Timer_Frame = TxC(GAME + GAMEMODE + @"Timer_Frame.png");
            #endregion
            #region ステージ失敗
            Failed_Game = TxC(GAME + FAILED + @"Game.png");
            Failed_Stage = TxC(GAME + FAILED + @"Stage.png");
            #endregion
            #region ランナー
            Runner = TxC(GAME + RUNNER + @"0.png");
            #endregion
            #region DanC
            DanC_Background = TxC(GAME + DANC + @"Background.png");
            DanC_Gauge = new FDKTexture[4];
            var type = new string[] { "Normal", "Reach", "Clear", "Flush" };
            for (int i = 0; i < 4; i++)
            {
                DanC_Gauge[i] = TxC(GAME + DANC + @"Gauge_" + type[i] + ".png");
            }
            DanC_Base = TxC(GAME + DANC + @"Base.png");
            DanC_Failed = TxC(GAME + DANC + @"Failed.png");
            DanC_Number = TxC(GAME + DANC + @"Number.png");
            DanC_ExamType = TxC(GAME + DANC + @"ExamType.png");
            DanC_ExamRange = TxC(GAME + DANC + @"ExamRange.png");
            DanC_ExamUnit = TxC(GAME + DANC + @"ExamUnit.png");
            DanC_Screen = TxC(GAME + DANC + @"Screen.png");
            #endregion
            #region PuichiChara
            PuchiChara = TxC(GAME + PUCHICHARA + @"0.png");
            #endregion
            #endregion

            #region 6_結果発表
            Result_Background = TxC(RESULT + @"Background.png");
            Result_FadeIn = TxC(RESULT + @"FadeIn.png");
            Result_Gauge = TxC(RESULT + @"Gauge.png");
            Result_Gauge_Base = TxC(RESULT + @"Gauge_Base.png");
            Result_Judge = TxC(RESULT + @"Judge.png");
            Result_Header = TxC(RESULT + @"Header.png");
            Result_Number = TxC(RESULT + @"Number.png");
            Result_Panel = TxC(RESULT + @"Panel.png");
            Result_Score_Text = TxC(RESULT + @"Score_Text.png");
            Result_Score_Number = TxC(RESULT + @"Score_Number.png");
            Result_Dan = TxC(RESULT + @"Dan.png");
            #endregion

            #region 7_終了画面
            //Exit_Background = TxC(EXIT + @"Background.png");
            #endregion

        }

        public void DisposeTexture()
        {
            //TJAPlayer3.tテクスチャの解放(ref Title_Background);
            //TJAPlayer3.tテクスチャの解放(ref Title_Menu);
            #region 共通
            TJAPlayer3.DisposeFDKTexture(ref Tile_Black);
            TJAPlayer3.DisposeFDKTexture(ref Tile_White);
            TJAPlayer3.DisposeFDKTexture(ref Menu_Title);
            TJAPlayer3.DisposeFDKTexture(ref Menu_Highlight);
            TJAPlayer3.DisposeFDKTexture(ref Scanning_Loudness);
            TJAPlayer3.DisposeFDKTexture(ref Overlay);
            for (int i = 0; i < 2; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref NamePlate[i]);
            }

            #endregion
            #region 1_タイトル画面
            /*
            TJAPlayer3.tテクスチャの解放(ref Title_Background);
            TJAPlayer3.tテクスチャの解放(ref Title_Menu);
            */
            #endregion

            #region 2_コンフィグ画面
            /*
            TJAPlayer3.tテクスチャの解放(ref Config_Background);
            TJAPlayer3.tテクスチャの解放(ref Config_Cursor);
            TJAPlayer3.tテクスチャの解放(ref Config_ItemBox);
            TJAPlayer3.tテクスチャの解放(ref Config_Arrow);
            TJAPlayer3.tテクスチャの解放(ref Config_KeyAssign);
            TJAPlayer3.tテクスチャの解放(ref Config_Font);
            TJAPlayer3.tテクスチャの解放(ref Config_Font_Bold);
            TJAPlayer3.tテクスチャの解放(ref Config_Enum_Song);
            */
            #endregion

            #region 3_選曲画面
            /*
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Background);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Header);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Footer);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Difficulty);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Auto);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Level);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Branch);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Branch_Text);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Bar_Center);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Frame_Score);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Frame_Box);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Frame_BackBox);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Frame_Random);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Score_Select);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_GenreText);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Cursor_Left);
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_Cursor_Right);
            for (int i = 0; i < SongSelect_Bar_Genre.Length; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref SongSelect_Bar_Genre[i]);
            }
            for (int i = 0; i < (int)Difficulty.Total; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref SongSelect_ScoreWindow[i]);
            }

            for (int i = 0; i < SongSelect_GenreBack.Length; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref SongSelect_GenreBack[i]);
            }
            TJAPlayer3.DisposeFDKTexture(ref SongSelect_ScoreWindow_Text);
            */
            #endregion

            #region 4_読み込み画面
            TJAPlayer3.DisposeFDKTexture(ref SongLoading_Plate);
            TJAPlayer3.DisposeFDKTexture(ref SongLoading_FadeIn);
            TJAPlayer3.DisposeFDKTexture(ref SongLoading_FadeOut);
            #endregion

            #region 5_演奏画面
            #region 共通
            TJAPlayer3.DisposeFDKTexture(ref Notes);
            TJAPlayer3.DisposeFDKTexture(ref Judge_Frame);
            TJAPlayer3.DisposeFDKTexture(ref SENotes);
            TJAPlayer3.DisposeFDKTexture(ref Notes_Arm);
            TJAPlayer3.DisposeFDKTexture(ref Judge);

            TJAPlayer3.DisposeFDKTexture(ref Judge_Meter);
            TJAPlayer3.DisposeFDKTexture(ref Bar);
            TJAPlayer3.DisposeFDKTexture(ref Bar_Branch);

            #endregion
            #region キャラクター

            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Normal; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_Normal[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Clear; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_Normal_Cleared[i]);
                TJAPlayer3.DisposeFDKTexture(ref Chara_Normal_Maxed[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGo; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_GoGoTime[i]);
                TJAPlayer3.DisposeFDKTexture(ref Chara_GoGoTime_Maxed[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_10combo; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_10Combo[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_10combo_Max; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_10Combo_Maxed[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGoStart; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_GoGoStart[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_GoGoStart_Max; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_GoGoStart_Maxed[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_ClearIn; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_Become_Cleared[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_SoulIn; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_Become_Maxed[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Breaking; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_Balloon_Breaking[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Broke; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_Balloon_Broke[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Chara_Ptn_Balloon_Miss; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Chara_Balloon_Miss[i]);
            }
            #endregion
            #region 踊り子
            for (int i = 0; i < 5; i++)
            {
                for (int p = 0; p < TJAPlayer3.Skin.SkinValue.Game_Dancer_Ptn; p++)
                {
                    TJAPlayer3.DisposeFDKTexture(ref Dancer[i][p]);
                }
            }
            #endregion
            #region モブ
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Mob_Ptn; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Mob[i]);
            }
            #endregion
            #region フッター
            TJAPlayer3.DisposeFDKTexture(ref Mob_Footer);
            #endregion
            #region 背景
            TJAPlayer3.DisposeFDKTexture(ref Background);
            TJAPlayer3.DisposeFDKTexture(ref Background_Up[0]);
            TJAPlayer3.DisposeFDKTexture(ref Background_Up[1]);
            TJAPlayer3.DisposeFDKTexture(ref Background_Up_Clear[0]);
            TJAPlayer3.DisposeFDKTexture(ref Background_Up_Clear[1]);
            TJAPlayer3.DisposeFDKTexture(ref Background_Down);
            TJAPlayer3.DisposeFDKTexture(ref Background_Down_Clear);
            TJAPlayer3.DisposeFDKTexture(ref Background_Down_Scroll);

            #endregion
            #region 太鼓
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Background[0]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Background[1]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Frame[0]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Frame[1]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_PlayerNumber[0]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_PlayerNumber[1]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_NamePlate[0]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_NamePlate[1]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Base);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Don_Left);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Don_Right);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Ka_Left);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Ka_Right);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_LevelUp);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_LevelDown);
            for (int i = 0; i < 6; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Couse_Symbol[i]);
            }
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Score[0]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Score[1]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Score[2]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Combo[0]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Combo[1]);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Combo_Effect);
            TJAPlayer3.DisposeFDKTexture(ref Taiko_Combo_Text);
            #endregion
            #region ゲージ
            TJAPlayer3.DisposeFDKTexture(ref Gauge[0]);
            TJAPlayer3.DisposeFDKTexture(ref Gauge[1]);
            TJAPlayer3.DisposeFDKTexture(ref Gauge_Base[0]);
            TJAPlayer3.DisposeFDKTexture(ref Gauge_Base[1]);
            TJAPlayer3.DisposeFDKTexture(ref Gauge_Line[0]);
            TJAPlayer3.DisposeFDKTexture(ref Gauge_Line[1]);
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Gauge_Rainbow_Ptn; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Gauge_Rainbow[i]);
            }
            TJAPlayer3.DisposeFDKTexture(ref Gauge_Soul);
            TJAPlayer3.DisposeFDKTexture(ref Gauge_Soul_Fire);
            TJAPlayer3.DisposeFDKTexture(ref Gauge_Soul_Explosion[0]);
            TJAPlayer3.DisposeFDKTexture(ref Gauge_Soul_Explosion[1]);
            #endregion
            #region 吹き出し
            TJAPlayer3.DisposeFDKTexture(ref Balloon_Combo[0]);
            TJAPlayer3.DisposeFDKTexture(ref Balloon_Combo[1]);
            TJAPlayer3.DisposeFDKTexture(ref Balloon_Roll);
            TJAPlayer3.DisposeFDKTexture(ref Balloon_Balloon);
            TJAPlayer3.DisposeFDKTexture(ref Balloon_Number_Roll);
            TJAPlayer3.DisposeFDKTexture(ref Balloon_Number_Combo);

            for (int i = 0; i < 6; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Balloon_Breaking[i]);
            }
            #endregion
            #region エフェクト
            TJAPlayer3.DisposeFDKTexture(ref Effects_Hit_Explosion);
            TJAPlayer3.DisposeFDKTexture(ref  Effects_Hit_Explosion_Big);
            TJAPlayer3.DisposeFDKTexture(ref Effects_Hit_FireWorks);

            TJAPlayer3.DisposeFDKTexture(ref Effects_Fire);
            TJAPlayer3.DisposeFDKTexture(ref Effects_Rainbow);

            TJAPlayer3.DisposeFDKTexture(ref Effects_GoGoSplash);

            for (int i = 0; i < 15; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Effects_Hit_Great[i]);
                TJAPlayer3.DisposeFDKTexture(ref Effects_Hit_Great_Big[i]);
                TJAPlayer3.DisposeFDKTexture(ref Effects_Hit_Good[i]);
                TJAPlayer3.DisposeFDKTexture(ref Effects_Hit_Good_Big[i]);
            }
            for (int i = 0; i < TJAPlayer3.Skin.SkinValue.Game_Effect_Roll_Ptn; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Effects_Roll[i]);
            }
            #endregion
            #region レーン
            for (int i = 0; i < 3; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Lane_Base[i]);
                TJAPlayer3.DisposeFDKTexture(ref Lane_Text[i]);
            }
            TJAPlayer3.DisposeFDKTexture(ref Lane_Red);
            TJAPlayer3.DisposeFDKTexture(ref Lane_Blue);
            TJAPlayer3.DisposeFDKTexture(ref Lane_Yellow);
            TJAPlayer3.DisposeFDKTexture(ref Lane_Background_Main);
            TJAPlayer3.DisposeFDKTexture(ref Lane_Background_Sub);
            TJAPlayer3.DisposeFDKTexture(ref Lane_Background_GoGo);

            #endregion
            #region 終了演出
            for (int i = 0; i < 5; i++)
            {
                TJAPlayer3.DisposeFDKTexture(ref End_Clear_L[i]);
                TJAPlayer3.DisposeFDKTexture(ref End_Clear_R[i]);
            }
            TJAPlayer3.DisposeFDKTexture(ref End_Clear_Text);
            TJAPlayer3.DisposeFDKTexture(ref End_Clear_Text_Effect);
            #endregion
            #region ゲームモード
            TJAPlayer3.DisposeFDKTexture(ref GameMode_Timer_Tick);
            TJAPlayer3.DisposeFDKTexture(ref GameMode_Timer_Frame);
            #endregion
            #region ステージ失敗
            TJAPlayer3.DisposeFDKTexture(ref Failed_Game);
            TJAPlayer3.DisposeFDKTexture(ref Failed_Stage);
            #endregion
            #region ランナー
            TJAPlayer3.DisposeFDKTexture(ref Runner);
            #endregion
            #region DanC
            DanC_Background?.Dispose();
            for (int i = 0; i < 4; i++)
            {
                DanC_Gauge[i]?.Dispose();
            }
            DanC_Base?.Dispose();
            DanC_Failed?.Dispose();
            DanC_Number?.Dispose();
            DanC_ExamRange?.Dispose();
            DanC_ExamUnit?.Dispose();
            DanC_ExamType?.Dispose();
            DanC_Screen?.Dispose();
            #endregion
            #region PuchiChara
            TJAPlayer3.DisposeFDKTexture(ref PuchiChara);
            #endregion
            #endregion

            #region 6_結果発表
            TJAPlayer3.DisposeFDKTexture(ref Result_Background);
            TJAPlayer3.DisposeFDKTexture(ref Result_FadeIn);
            TJAPlayer3.DisposeFDKTexture(ref Result_Gauge);
            TJAPlayer3.DisposeFDKTexture(ref Result_Gauge_Base);
            TJAPlayer3.DisposeFDKTexture(ref Result_Judge);
            TJAPlayer3.DisposeFDKTexture(ref Result_Header);
            TJAPlayer3.DisposeFDKTexture(ref Result_Number);
            TJAPlayer3.DisposeFDKTexture(ref Result_Panel);
            TJAPlayer3.DisposeFDKTexture(ref Result_Score_Text);
            TJAPlayer3.DisposeFDKTexture(ref Result_Score_Number);
            TJAPlayer3.DisposeFDKTexture(ref Result_Dan);
            #endregion

            #region 7_終了画面
            //TJAPlayer3.DisposeFDKTexture(ref Exit_Background);
            #endregion

        }

        #region 共通
        public FDKTexture Tile_Black,
            Tile_White,
            Menu_Title,
            Menu_Highlight,
            Scanning_Loudness,
            Overlay;
        public FDKTexture[] NamePlate;
        #endregion
        #region 1_タイトル画面
        /*
        public FDKTexture Title_Background,
            Title_Menu;
        */
        #endregion

        #region 2_コンフィグ画面
        /*
        public FDKTexture Config_Background,
            Config_Cursor,
            Config_ItemBox,
            Config_Arrow,
            Config_KeyAssign,
            Config_Font,
            Config_Font_Bold,
            Config_Enum_Song;
        */
        #endregion

        #region 3_選曲画面
        /*
        public FDKTexture SongSelect_Background,
            SongSelect_Header,
            SongSelect_Footer,
            SongSelect_Difficulty,
            SongSelect_Auto,
            SongSelect_Level,
            SongSelect_Branch,
            SongSelect_Branch_Text,
            SongSelect_Frame_Score,
            SongSelect_Frame_Box,
            SongSelect_Frame_BackBox,
            SongSelect_Frame_Random,
            SongSelect_Score_Select,
            SongSelect_Bar_Center,
            SongSelect_GenreText,
            SongSelect_Cursor_Left,
            SongSelect_Cursor_Right,
            SongSelect_ScoreWindow_Text;
        public FDKTexture[] SongSelect_GenreBack = new FDKTexture[9],
            SongSelect_ScoreWindow = new FDKTexture[(int)Difficulty.Total],
            SongSelect_Bar_Genre = new FDKTexture[9],
            SongSelect_NamePlate = new FDKTexture[1];
        */
        #endregion

        #region 4_読み込み画面
        public FDKTexture SongLoading_Plate,
            SongLoading_FadeIn,
            SongLoading_FadeOut;
        #endregion

        #region 5_演奏画面
        #region 共通
        public FDKTexture Notes,
            Judge_Frame,
            SENotes,
            Notes_Arm,
            Judge;
        public FDKTexture Judge_Meter,
            Bar,
            Bar_Branch;
        #endregion
        #region キャラクター
        public FDKTexture[] Chara_Normal,
            Chara_Normal_Cleared,
            Chara_Normal_Maxed,
            Chara_GoGoTime,
            Chara_GoGoTime_Maxed,
            Chara_10Combo,
            Chara_10Combo_Maxed,
            Chara_GoGoStart,
            Chara_GoGoStart_Maxed,
            Chara_Become_Cleared,
            Chara_Become_Maxed,
            Chara_Balloon_Breaking,
            Chara_Balloon_Broke,
            Chara_Balloon_Miss;
        #endregion
        #region 踊り子
        public FDKTexture[][] Dancer;
        #endregion
        #region モブ
        public FDKTexture[] Mob;
        public FDKTexture Mob_Footer;
        #endregion
        #region 背景
        public FDKTexture Background,
            Background_Down,
            Background_Down_Clear,
            Background_Down_Scroll;
        public FDKTexture[] Background_Up,
            Background_Up_Clear;
        #endregion
        #region 太鼓
        public FDKTexture[] Taiko_Frame, // MTaiko下敷き
            Taiko_Background;
        public FDKTexture Taiko_Base,
            Taiko_Don_Left,
            Taiko_Don_Right,
            Taiko_Ka_Left,
            Taiko_Ka_Right,
            Taiko_LevelUp,
            Taiko_LevelDown,
            Taiko_Combo_Effect,
            Taiko_Combo_Text;
        public FDKTexture[] Couse_Symbol, // コースシンボル
            Taiko_PlayerNumber,
            Taiko_NamePlate; // ネームプレート
        public FDKTexture[] Taiko_Score,
            Taiko_Combo;
        #endregion
        #region ゲージ
        public FDKTexture[] Gauge,
            Gauge_Base,
            Gauge_Line,
            Gauge_Rainbow,
            Gauge_Soul_Explosion;
        public FDKTexture Gauge_Soul,
            Gauge_Soul_Fire;
        #endregion
        #region 吹き出し
        public FDKTexture[] Balloon_Combo;
        public FDKTexture Balloon_Roll,
            Balloon_Balloon,
            Balloon_Number_Roll,
            Balloon_Number_Combo/*,*/
                                /*Balloon_Broken*/;
        public FDKTexture[] Balloon_Breaking;
        #endregion
        #region エフェクト
        public FDKTexture Effects_Hit_Explosion,
            Effects_Hit_Explosion_Big,
            Effects_Fire,
            Effects_Rainbow,
            Effects_GoGoSplash,
            Effects_Hit_FireWorks;
        public FDKTexture[] Effects_Hit_Great,
            Effects_Hit_Good,
            Effects_Hit_Great_Big,
            Effects_Hit_Good_Big;
        public FDKTexture[] Effects_Roll;
        #endregion
        #region レーン
        public FDKTexture[] Lane_Base,
            Lane_Text;
        public FDKTexture Lane_Red,
            Lane_Blue,
            Lane_Yellow;
        public FDKTexture Lane_Background_Main,
            Lane_Background_Sub,
            Lane_Background_GoGo;
        #endregion
        #region 終了演出
        public FDKTexture[] End_Clear_L,
            End_Clear_R;
        public FDKTexture End_Clear_Text,
            End_Clear_Text_Effect;
        #endregion
        #region ゲームモード
        public FDKTexture GameMode_Timer_Frame,
            GameMode_Timer_Tick;
        #endregion
        #region ステージ失敗
        public FDKTexture Failed_Game,
            Failed_Stage;
        #endregion
        #region ランナー
        public FDKTexture Runner;
        #endregion
        #region DanC
        public FDKTexture DanC_Background;
        public FDKTexture[] DanC_Gauge;
        public FDKTexture DanC_Base;
        public FDKTexture DanC_Failed;
        public FDKTexture DanC_Number,
            DanC_ExamType,
            DanC_ExamRange,
            DanC_ExamUnit;
        public FDKTexture DanC_Screen;
        #endregion
        #region PuchiChara
        public FDKTexture PuchiChara;
        #endregion
        #endregion

        #region 6_結果発表
        public FDKTexture Result_Background,
            Result_FadeIn,
            Result_Gauge,
            Result_Gauge_Base,
            Result_Judge,
            Result_Header,
            Result_Number,
            Result_Panel,
            Result_Score_Text,
            Result_Score_Number,
            Result_Dan;
        #endregion

        #region 7_終了画面
        //public FDKTexture Exit_Background
        /* , */
                                       /*Exit_Text */ //;
        #endregion

    }
}
