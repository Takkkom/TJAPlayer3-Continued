using FDK;

namespace TJAPlayer3
{
    /// <summary>
    /// The ConfigIniToSongGainControllerBinder allows for SONGVOL and/or other
    /// properties related to the Gain levels applied to song preview and
    /// playback, to be applied conditionally based on settings flowing from
    /// ConfigIni. This binder class allows that to take place without either
    /// ConfigIni or SongGainController having awareness of one another.
    /// See those classes properties, methods, and events for more details. 
    /// </summary>
    internal static class ConfigIniToSongGainControllerBinder
    {
        internal static void Bind(MainConfig configIni, SongGainController songGainController)
        {
            songGainController.ApplyLoudnessMetadata = configIni.ApplyLoudnessMetadata;
            songGainController.TargetLoudness = new Lufs(configIni.TargetLoudness);
            songGainController.ApplySongVol = configIni.ApplySongVol;

            configIni.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(MainConfig.ApplyLoudnessMetadata):
                        songGainController.ApplyLoudnessMetadata = configIni.ApplyLoudnessMetadata;
                        break;
                    case nameof(MainConfig.TargetLoudness):
                        songGainController.TargetLoudness = new Lufs(configIni.TargetLoudness);
                        break;
                    case nameof(MainConfig.ApplySongVol):
                        songGainController.ApplySongVol = configIni.ApplySongVol;
                        break;
                }
            };
        }
    }
}