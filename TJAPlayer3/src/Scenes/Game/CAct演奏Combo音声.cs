﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏Combo音声 : Activity
	{
		// コンストラクタ

		public CAct演奏Combo音声()
		{
			base.NotActivated = true;
		}
		
		// メソッド
        public void t再生( int nCombo, int player )
        {
            if(VoiceIndex[player] < ListCombo[player].Count)
            {
                
                var index = ListCombo[player][VoiceIndex[player]];
                if (nCombo == index.nCombo)
                {
                    index.soundComboVoice?.PlaySoundFromBegin();
                    VoiceIndex[player]++;
                }
                
            }
        }

        /// <summary>
        /// カーソルを戻す。
        /// コンボが切れた時に使う。
        /// </summary>
        public void tReset(int nPlayer)
        {
            VoiceIndex[nPlayer] = 0;
        }

		// CActivity 実装

		public override void Activate()
		{
            for (int i = 0; i < 2; i++)
            {
                ListCombo[i] = new List<CComboVoice>();
            }
            VoiceIndex = new int[] { 0, 0 };
			base.Activate();
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
                // フォルダ内を走査してコンボボイスをListに入れていく
                // 1P、2P コンボボイス
                for (int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++)
                {
                    var currentDir = SkinManager.Path(string.Format(@"Sounds\Combo_{0}P\", i + 1));
                    if (Directory.Exists(currentDir))
                    {
                        foreach (var item in Directory.GetFiles(currentDir))
                        {
                            var comboVoice = new CComboVoice();
                            comboVoice.bFileFound = true;
                            comboVoice.nPlayer = i;
                            comboVoice.strFilePath = item;
                            comboVoice.soundComboVoice = TJAPlayer3._SoundManager.CreateFDKSound(item, SoundGroup.Voice);
                            comboVoice.nCombo = int.Parse(Path.GetFileNameWithoutExtension(item));
                            ListCombo[i].Add(comboVoice);
                        }
                        if(ListCombo[i].Count > 0)
                        {
                            ListCombo[i].Sort();
                        }
                    }
                }
    			base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if( !base.NotActivated )
			{
                for (int i = 0; i < 2; i++)
                {
                    foreach (var item in ListCombo[i])
                    {
                        TJAPlayer3._SoundManager.DisposeSound(item.soundComboVoice);
                    }
                    ListCombo[i].Clear();
                }

				base.ManagedReleaseResources();
			}
		}

		#region [ private ]
		//-----------------
        int[] VoiceIndex;
        readonly List<CComboVoice>[] ListCombo = new List<CComboVoice>[2];
		//-----------------
		#endregion
	}

    public class CComboVoice : IComparable<CComboVoice>
    {
        public bool bFileFound;
        public int nCombo;
        public int nPlayer;
        public string strFilePath;
        public FDKSound soundComboVoice;

        public CComboVoice()
        {
            bFileFound = false;
            nCombo = 0;
            nPlayer = 0;
            strFilePath = "";
            soundComboVoice = null;
        }

        public int CompareTo( CComboVoice other )
        {
            if( this.nCombo > other.nCombo ) return 1;
            else if( this.nCombo < other.nCombo ) return -1;

            return 0;
        }
    }
}
