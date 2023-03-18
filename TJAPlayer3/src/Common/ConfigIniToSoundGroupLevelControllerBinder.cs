using System;
using FDK;

namespace TJAPlayer3
{
    /// <summary>
    /// The ConfigIniToSoundGroupLevelControllerBinder allows for updated sound
    /// group level values, and keyboard sound level adjustment increment
    /// values, to flow between CConfigIni and the SoundGroupLevelController
    /// without either of those two classes being aware of one another.
    /// See those classes properties, methods, and events for more details. 
    /// </summary>
    internal static class ConfigIniToSoundGroupLevelControllerBinder
    {
        internal static void Bind(MainConfig configIni, SoundGroupLevelController soundGroupLevelController)
        {
            soundGroupLevelController.SetLevel(SoundGroup.SoundEffect, configIni.SoundEffectLevel);
            soundGroupLevelController.SetLevel(SoundGroup.Voice, configIni.VoiceLevel);
            soundGroupLevelController.SetLevel(SoundGroup.SongPreview, configIni.SongPreviewLevel);
            soundGroupLevelController.SetLevel(SoundGroup.SongPlayback, configIni.SongPlaybackLevel);
            soundGroupLevelController.SetKeyboardSoundLevelIncrement(configIni.KeyboardSoundLevelIncrement);

            configIni.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(MainConfig.SoundEffectLevel):
                        soundGroupLevelController.SetLevel(SoundGroup.SoundEffect, configIni.SoundEffectLevel);
                        break;
                    case nameof(MainConfig.VoiceLevel):
                        soundGroupLevelController.SetLevel(SoundGroup.Voice, configIni.VoiceLevel);
                        break;
                    case nameof(MainConfig.SongPreviewLevel):
                        soundGroupLevelController.SetLevel(SoundGroup.SongPreview, configIni.SongPreviewLevel);
                        break;
                    case nameof(MainConfig.SongPlaybackLevel):
                        soundGroupLevelController.SetLevel(SoundGroup.SongPlayback, configIni.SongPlaybackLevel);
                        break;
                    case nameof(MainConfig.KeyboardSoundLevelIncrement):
                        soundGroupLevelController.SetKeyboardSoundLevelIncrement(configIni.KeyboardSoundLevelIncrement);
                        break;
                }
            };

            soundGroupLevelController.LevelChanged += (sender, args) =>
            {
                switch (args.SoundGroup)
                {
                    case SoundGroup.SoundEffect:
                        configIni.SoundEffectLevel = args.Level;
                        break;
                    case SoundGroup.Voice:
                        configIni.VoiceLevel = args.Level;
                        break;
                    case SoundGroup.SongPreview:
                        configIni.SongPreviewLevel = args.Level;
                        break;
                    case SoundGroup.SongPlayback:
                        configIni.SongPlaybackLevel = args.Level;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }
    }
}
