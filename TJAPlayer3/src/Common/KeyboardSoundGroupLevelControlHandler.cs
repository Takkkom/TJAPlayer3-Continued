﻿using FDK;

namespace TJAPlayer3
{
    /// <summary>
    /// KeyboardSoundGroupLevelControlHandler is called by the song selection
    /// and song play stages when handling keyboard input. By delegating to
    /// this class they are able to support a centrally-managed and consistent
    /// set of keyboard shortcuts for dynamically adjusting four sound group
    /// levels:
    /// - sound effect level, via Ctrl and either of the Minus or Equals keys
    /// - voice level, via Shift and either of the Minus or Equals keys
    /// - song preview and song playback level, via the Minus or Equals key
    ///
    /// When the sound group levels are adjusted in this manner, the
    /// SoundGroupLevelController (and handlers bound to its events) ensure
    /// that both the sound object group levels are updated and the application
    /// configuration is updated. See ConfigIniToSoundGroupLevelControllerBinder
    /// for more details on the latter.
    /// </summary>
    internal static class KeyboardSoundGroupLevelControlHandler
    {
        internal static void Handle(
            IInputDevice keyboard,
            SoundGroupLevelController soundGroupLevelController,
            SkinManager skin,
            bool isSongPreview)
        {
            var isAdjustmentPositive = keyboard.GetKeyPressed((int) SlimDX.DirectInput.Key.RightBracket);
            if (!(isAdjustmentPositive || keyboard.GetKeyPressed((int) SlimDX.DirectInput.Key.LeftBracket)))
            {
                return;
            }

            SoundGroup soundGroup;
            SkinManager.Cシステムサウンド システムサウンド = null;

            if (keyboard.GetKeyKeepPressed((int) SlimDX.DirectInput.Key.LeftControl) ||
                keyboard.GetKeyKeepPressed((int) SlimDX.DirectInput.Key.RightControl))
            {
                soundGroup = SoundGroup.SoundEffect;
                システムサウンド = skin.sound決定音;
            }
            else if (keyboard.GetKeyKeepPressed((int) SlimDX.DirectInput.Key.LeftShift) ||
                     keyboard.GetKeyKeepPressed((int) SlimDX.DirectInput.Key.RightShift))
            {
                soundGroup = SoundGroup.Voice;
                システムサウンド = skin.soundゲーム開始音;
            }
            else if (isSongPreview)
            {
                soundGroup = SoundGroup.SongPreview;
            }
            else
            {
                soundGroup = SoundGroup.SongPlayback;
            }

            soundGroupLevelController.AdjustLevel(soundGroup, isAdjustmentPositive);
            システムサウンド?.t再生する();
        }
    }
}
