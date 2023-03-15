using System;
using System.Collections.Generic;
using System.Text;
using FDK;
using System.Diagnostics;

namespace TJAPlayer3
{
    internal class CAct演奏演奏情報 : Activity
    {
        // プロパティ

        public double dbBPM;
        public readonly int[] NowMeasure = new int[2];
        public double dbSCROLL;

		// コンストラクタ

		public CAct演奏演奏情報()
		{
			base.NotActivated = true;
		}

				
		// CActivity 実装

		public override void Activate()
		{
            for (int i = 0; i < 2; i++)
            {
                NowMeasure[i] = 0;
            }
			this.dbBPM = TJAPlayer3.DTX.BASEBPM;
            this.dbSCROLL = 1.0;
			base.Activate();
		}
		public override int Draw()
		{
			throw new InvalidOperationException( "t進行描画(int x, int y) のほうを使用してください。" );
		}
		public void t進行描画( int x, int y )
		{
			if ( !base.NotActivated )
			{
				y += 0x153;
				TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "BGM/Taiko Adj: {0:####0}/{1:####0} ms", TJAPlayer3.DTX.nBGMAdjust, TJAPlayer3._MainConfig.nInputAdjustTimeMs ) );
				y -= 0x10;
				int num = ( TJAPlayer3.DTX.listChip.Count > 0 ) ? TJAPlayer3.DTX.listChip[ TJAPlayer3.DTX.listChip.Count - 1 ].n発声時刻ms : 0;
				string str = "Time:          " + ( ( ( ( double ) TJAPlayer3.Timer.n現在時刻 ) / 1000.0 ) ).ToString( "####0.00" ) + " / " + ( ( ( ( double ) num ) / 1000.0 ) ).ToString( "####0.00" );
				TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, str );
				y -= 0x10;
				TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "Part:          {0:####0}/{1:####0}", NowMeasure[0], NowMeasure[1] ) );
				y -= 0x10;
				TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "BPM:           {0:####0.0000}", this.dbBPM ) );
				y -= 0x10;
				TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "Frame:         {0:####0} fps", TJAPlayer3.FPS.NowFPS ) );
				y -= 0x10;
				TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "NoteN:         {0:####0}", TJAPlayer3.DTX.nノーツ数[0] ) );
				y -= 0x10;
				TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "NoteE:         {0:####0}", TJAPlayer3.DTX.nノーツ数[1] ) );
				y -= 0x10;
				TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "NoteM:         {0:####0}", TJAPlayer3.DTX.nノーツ数[2] ) );
				y -= 0x10;
				TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "NoteC:         {0:####0}", TJAPlayer3.DTX.nノーツ数[3] ) );
				y -= 0x10;
				TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "SCROLL:        {0:####0.00}", this.dbSCROLL ) );
                y -= 0x10;
                TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "SCOREMODE:     {0:####0}", TJAPlayer3.DTX.nScoreModeTmp ) );
                y -= 0x10;
                TJAPlayer3._ConsoleText.tPrint( x, y, ConsoleText.FontType.White, string.Format( "SCROLLMODE:    {0:####0}", Enum.GetName(typeof(EScrollMode), TJAPlayer3._MainConfig.eScrollMode ) ) );


				//CDTXMania.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Sound CPU :    {0:####0.00}%", CDTXMania.Sound管理.GetCPUusage() ) );
				//y -= 0x10;
				//CDTXMania.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Sound Mixing:  {0:####0}", CDTXMania.Sound管理.GetMixingStreams() ) );
				//y -= 0x10;
				//CDTXMania.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Sound Streams: {0:####0}", CDTXMania.Sound管理.GetStreams() ) );
				//y -= 0x10;
			}
		}
	}
}
