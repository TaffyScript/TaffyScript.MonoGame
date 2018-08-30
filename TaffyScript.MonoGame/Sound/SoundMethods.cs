using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;
using TaffyScript.Collections;

namespace TaffyScript.MonoGame.Sound
{
    [TaffyScriptBaseType]
    public static class SoundMethods
    {
        #region Properties

        [TaffyScriptMethod]
        public static TsObject media_game_has_control(TsObject[] args)
        {
            return MediaPlayer.GameHasControl;
        }

        [TaffyScriptMethod]
        public static TsObject media_get_muted(TsObject[] args)
        {
            return MediaPlayer.IsMuted;
        }

        [TaffyScriptMethod]
        public static TsObject media_set_muted(TsObject[] args)
        {
            MediaPlayer.IsMuted = (bool)args[0];
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject media_get_repeating(TsObject[] args)
        {
            return MediaPlayer.IsRepeating;
        }

        [TaffyScriptMethod]
        public static TsObject media_set_repeating(TsObject[] args)
        {
            MediaPlayer.IsRepeating = (bool)args[0];
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject media_get_shuffled(TsObject[] args)
        {
            return MediaPlayer.IsShuffled;
        }

        [TaffyScriptMethod]
        public static TsObject media_set_shuffled(TsObject[] args)
        {
            MediaPlayer.IsShuffled = (bool)args[0];
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject media_get_visualization_ended(TsObject[] args)
        {
            return MediaPlayer.IsVisualizationEnabled;
        }

        [TaffyScriptMethod]
        public static TsObject media_get_position(TsObject[] args)
        {
            return MediaPlayer.PlayPosition.Milliseconds;
        }

        [TaffyScriptMethod]
        public static TsObject media_get_state(TsObject[] args)
        {
            return (float)MediaPlayer.State;
        }

        [TaffyScriptMethod]
        public static TsObject media_get_volume(TsObject[] args)
        {
            return MediaPlayer.Volume;
        }

        [TaffyScriptMethod]
        public static TsObject media_set_volume(TsObject[] args)
        {
            MediaPlayer.Volume = (float)args[0];
            return TsObject.Empty;
        }

        #endregion

        #region Methods

        [TaffyScriptMethod]
        public static TsObject media_move_next(TsObject[] args)
        {
            MediaPlayer.MoveNext();
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject media_move_previous(TsObject[] args)
        {
            MediaPlayer.MovePrevious();
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject media_pause(TsObject[] args)
        {
            MediaPlayer.Pause();
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject media_play(TsObject[] args)
        {
            MediaPlayer.Play(((TsSong)args[0]).Source);
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject media_resume(TsObject[] args)
        {
            MediaPlayer.Resume();
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject media_stop(TsObject[] args)
        {
            MediaPlayer.Stop();
            return TsObject.Empty;
        }

        #endregion

        #region Events

        private static EventCache<MediaEventType> _cache = new EventCache<MediaEventType>();

        [TaffyScriptMethod]
        public static TsObject media_subscribe(TsObject[] args)
        {
            var type = (MediaEventType)(float)args[0];
            var del = (TsDelegate)args[1];
            switch(type)
            {
                case MediaEventType.ActiveSongChanged:
                    ActiveSongChanged(del);
                    break;
                case MediaEventType.MediaStateChanged:
                    MediaStateChanged(del);
                    break;
                default:
                    throw new ArgumentException("type");
            }
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject media_unsubscribe(TsObject[] args)
        {
            var type = (MediaEventType)(float)args[0];
            var del = (TsDelegate)args[1];
            if(_cache.TryRemove(type, del, out var handler))
            {
                switch(type)
                {
                    case MediaEventType.ActiveSongChanged:
                        MediaPlayer.ActiveSongChanged -= (EventHandler<EventArgs>)handler;
                        break;
                    case MediaEventType.MediaStateChanged:
                        MediaPlayer.MediaStateChanged -= (EventHandler<EventArgs>)handler;
                        break;
                    default:
                        return false;
                }
                return true;
            }
            return false;
        }

        private static void ActiveSongChanged(TsDelegate del)
        {
            EventHandler<EventArgs> handler = (s, e) => del.Invoke();
            MediaPlayer.ActiveSongChanged += handler;
            _cache.Cache(MediaEventType.ActiveSongChanged, del, handler);
        }

        private static void MediaStateChanged(TsDelegate del)
        {
            EventHandler<EventArgs> handler = (s, e) => del.Invoke();
            MediaPlayer.MediaStateChanged += handler;
            _cache.Cache(MediaEventType.MediaStateChanged, del, handler);
        }

        #endregion
    }

    public enum MediaEventType
    {
        ActiveSongChanged,
        MediaStateChanged
    }
}
