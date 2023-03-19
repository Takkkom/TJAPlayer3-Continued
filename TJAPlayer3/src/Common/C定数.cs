using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TJAPlayer3
{

    /// <summary>
    /// 難易度。
    /// </summary>
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard,
        Oni,
        Edit,
        Tower,
        Dan,
        Total
    }

    public enum EScrollMode
    {
        Normal,
        BMSCROLL,
        HBSCROLL
    }
    public enum Eジャンル
    {
        None = 0,
        JPOP = 1,
        ゲーム = 2,
        ナムコ = 3,
        クラシック = 4,
        バラエティ = 5,
        どうよう = 6,
        ボーカロイド = 7,
        アニメ = 8
    }
    public enum EGame
    {
        OFF = 0,
        完走叩ききりまショー = 1,
        完走叩ききりまショー激辛 = 2
    }
    public enum E難易度表示タイプ
    {
        OFF = 0,
        n曲目に表示 = 1,
        mtaikoに画像で表示 = 2,
    }
	public enum Eダークモード
	{
		OFF,
		HALF,
		FULL
	}
    public enum EWindowMovieMode
    {
        OFF = 0,
        左下 = 1,
        中央下 = 2
    }
	public enum Eダメージレベル
	{
		少ない	= 0,
		普通	= 1,
		大きい	= 2
	}
	public enum Eパッド			// 演奏用のenum。ここを修正するときは、次に出てくる EKeyConfigPad と EパッドFlag もセットで修正すること。
	{
		Cancel,
		Decide,
		LRed,
		RRed,
		LBlue,
		RBlue,
		LRed2P,
		RRed2P,
		LBlue2P,
		RBlue2P,
		MAX,			// 門番用として定義
		UNKNOWN = 99
	}
	public enum EKeyConfigPad		// #24609 キーコンフィグで使うenum。capture要素あり。
	{
		Cancel	= Eパッド.Cancel,
		Decide	= Eパッド.Decide,
        LRed    = Eパッド.LRed,
        RRed    = Eパッド.RRed,
        LBlue   = Eパッド.LBlue,
        RBlue   = Eパッド.RBlue,
        LRed2P  = Eパッド.LRed2P,
        RRed2P  = Eパッド.RRed2P,
        LBlue2P = Eパッド.LBlue2P,
        RBlue2P = Eパッド.RBlue2P,
		Capture,
		UNKNOWN = Eパッド.UNKNOWN
	}
	[Flags]
	public enum EパッドFlag		// #24063 2011.1.16 yyagi コマンド入力用 パッド入力のフラグ化
	{
		None,
		Cancel,
		Decide,
		LRed,
		RRed,
		LBlue,
		RBlue,
		LRed2P,
		RRed2P,
		LBlue2P,
		RBlue2P,
		UNKNOWN = 4096
	}
	public enum Eランダムモード
	{
		OFF,
		RANDOM,
        MIRROR,
		SUPERRANDOM,
		HYPERRANDOM
	}
	public enum E楽器パート		// ここを修正するときは、セットで次の EKeyConfigPart も修正すること。
	{
		DRUMS	= 0,
		GUITAR	= 1,
		BASS	= 2,
        TAIKO   = 3,
		UNKNOWN	= 99
	}
	public enum EKeyConfigPart	// : E楽器パート
	{
		DRUMS	= E楽器パート.DRUMS,
		GUITAR	= E楽器パート.GUITAR,
		BASS	= E楽器パート.BASS,
        TAIKO   = E楽器パート.TAIKO,
		SYSTEM,
		UNKNOWN	= E楽器パート.UNKNOWN
	}

	public enum E打ち分け時の再生の優先順位
	{
		ChipがPadより優先,
		PadがChipより優先
	}
	internal enum E入力デバイス
	{
		キーボード		= 0,
		MIDI入力		= 1,
		ジョイパッド	= 2,
		マウス			= 3,
		不明			= -1
	}
	public enum E判定
	{
		Perfect	= 0,
		Great	= 1,
		Good	= 2,
		Poor	= 3,
		Miss	= 4,
		Bad		= 5,
		Auto
	}
	internal enum E判定文字表示位置
	{
		表示OFF,
		レーン上,
		判定ライン上,
		コンボ下
	}
	internal enum E判定位置
	{
		標準	= 0,
		Lower,
		MAX
	}
	internal enum E判定表示優先度
	{
		Chipより下,
		Chipより上
	}
	internal enum EAVI種別
	{
		Unknown,
		AVI,
		AVIPAN
	}
	internal enum EBGA種別
	{
		Unknown,
		BMP,
		BMPTEX,
		BGA,
		BGAPAN
	}
	internal enum FadeModeType
	{
		FadeIn,
		FadeOut
	}
	internal enum Eレーン
	{
		LC = 0,
		HH,
		SD,
		BD,
		HT,
		LT,
		FT,
		CY,
        LP,
		RD,		// 将来の独立レーン化/独立AUTO設定を見越して追加
        LBD = 10,
		Guitar,	// AUTOレーン判定を容易にするため、便宜上定義しておく(未使用)
		Bass,	// (未使用)
		GtR,
		GtG,
		GtB,
		GtPick,
		GtW,
		BsR,
		BsG,
		BsB,
		BsPick,
		BsW,
		MAX,	// 要素数取得のための定義 ("BGM"は使わない前提で)
		BGM
	}
	internal enum Eレーン数
	{
		物理 = 8,	   // LC, HH,             SD, BD, HT, LT, FT, CY
		論理 = 10,	   // LC, HO, HC,         SD, BD, HT, LT, FT, RC, RD
        DTXG物理 = 10, // LC, HH,     LP,     SD, BD, HT, LT, FT, CY, RD
        DTXG論理 = 12  // LC, HO, HC, LP, LB, SD, BD, HT, LT, FT, CY, RD 
	}
	internal enum Eログ出力
	{
		OFF,
		ON通常,
		ON詳細あり
	}
	internal enum E演奏画面の戻り値
	{
		継続,
		演奏中断,
		ステージ失敗,
		ステージクリア,
		再読込_再演奏,
		再演奏
	}
	internal enum E曲読込画面の戻り値
	{
		継続 = 0,
		読込完了,
		読込中止
	}

    public enum ENoteState
    {
        none,
        wait,
        perfect,
        grade,
        bad
    }

    public enum E連打State
    {
        none,
        roll,
        rollB,
        balloon,
        potato
    }

    public enum Eステルスモード
    {
        OFF = 0,
        DORON = 1,
        STEALTH = 2
    }

	/// <summary>
	/// 透明チップの種類
	/// </summary>
	public enum EInvisible
	{
		OFF,		// チップを透明化しない
		SEMI,		// Poor/Miss時だけ、一時的に透明解除する
		FULL		// チップを常に透明化する
	}

	/// <summary>
	/// Drum/Guitar/Bass の値を扱う汎用の構造体。
	/// </summary>
	/// <typeparam name="T">値の型。</typeparam>
	[Serializable]
	[StructLayout( LayoutKind.Sequential )]
	public struct STDGBVALUE<T>			// indexはE楽器パートと一致させること
	{
		public T Drums;
		public T Guitar;
		public T Bass;
        public T Taiko;
		public T Unknown;
		public T this[ int index ]
		{
			get
			{
				switch( index )
				{
					case (int) E楽器パート.DRUMS:
						return this.Drums;

					case (int) E楽器パート.GUITAR:
						return this.Guitar;

					case (int) E楽器パート.BASS:
						return this.Bass;

                    case (int) E楽器パート.TAIKO:
                        return this.Taiko;

					case (int) E楽器パート.UNKNOWN:
						return this.Unknown;
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				switch( index )
				{
					case (int) E楽器パート.DRUMS:
						this.Drums = value;
						return;

					case (int) E楽器パート.GUITAR:
						this.Guitar = value;
						return;

					case (int) E楽器パート.BASS:
						this.Bass = value;
						return;

                    case (int) E楽器パート.TAIKO:
                        this.Taiko = value;
                        return;

					case (int) E楽器パート.UNKNOWN:
						this.Unknown = value;
						return;
				}
				throw new IndexOutOfRangeException();
			}
		}
	}


    #region[Ver.K追加]
    public enum Eレーンタイプ
    {
        TypeA,
        TypeB,
        TypeC,
        TypeD
    }
    public enum Eミラー
    {
        TypeA,
        TypeB
    }
    public enum EClipDispType
    {
        背景のみ           = 1,
        ウィンドウのみ     = 2,
        両方               = 3,
        OFF                = 0
    }
    #endregion

    internal class C定数
	{
	}
}
