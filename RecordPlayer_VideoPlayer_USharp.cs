using UdonSharp;
using UdonSharp.Video;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.RecordPlayer
{

    public class RecordPlayer_VideoPlayer_USharp : RecordPlayer_VideoplayerClass
    {
        public USharpVideoPlayer USharpVideo;
        public VideoPlayerManager Internals;


        void Start()
        {
            USharpVideo.RegisterCallbackReceiver(this);
            recordPlayer.volumeSlider._Internal_UpdateValue(_GetVolume());
        }

        //##############################################################################################################
        #region USharp Callbacks

        public void OnUSharpVideoEnd()
        {
            Debug.LogError("OnUSharpVideoEnd");
            _PlaybackEnded();
        }

        public void OnUSharpVideoPlay()
        {
            _OnVideoLoad();
        }
        #endregion
        //##############################################################################################################
        #region Overrides
        public override void _LoadUrl(VRCUrl _url)
        {
            USharpVideo.PlayVideo(_url);
        }
        public override void _Pause()
        {
            USharpVideo.SetPaused(!USharpVideo.IsPaused());
        }
        public override void _Stop()
        {
            USharpVideo.StopVideo();
        }
        public override bool _IsPaused()
        {
            return USharpVideo.IsPaused();
        }
        public override void _loopCurrentUrl()
        {
            var record = recordPlayer.records[recordPlayer.currentRecord];
            USharpVideo.PlayVideo(record.urls[recordPlayer.currentTrack]);
        }
        public override bool _CanPlayVideo()
        {
            return USharpVideo.CanControlVideoPlayer();
        }

        public override bool _IsVideoplayerOwner()
        {
            return Networking.IsOwner(USharpVideo.gameObject);
        }
        public override void _SetVolume(float _volume)
        {
            USharpVideo.SetVolume(_volume);
        }
        public override float _GetVolume()
        {
            return USharpVideo.GetVolume();
        }

        public override void _ChangePlaybackTime(float _time)
        {
            float newValue = Remap(_time, 0, _GetPlaybackDuration_End(), 0, 1);
            recordPlayer._Log($"[USharp] ChangePlaybackTime: {newValue}");
            USharpVideo.SeekTo(newValue);
        }
        public override float _GetPlaybackTime()
        {
            float curTime = Internals.GetTime();
            float duration = Internals.GetDuration();
            return Remap(curTime, 0, duration, 0, 1);
        }
        public override float _GetPlaybackDuration_Current()
        {
            return Internals.GetTime();
        }
        public override float _GetPlaybackDuration_End()
        {
            return Internals.GetDuration();
        }

        #endregion
        //##############################################################################################################
    }
}