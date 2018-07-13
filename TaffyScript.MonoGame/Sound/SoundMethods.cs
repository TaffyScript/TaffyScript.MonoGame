using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;
using TaffyScript.Collections;

namespace TaffyScript.MonoGame.Sound
{
    [WeakBaseType]
    public static class SoundMethods
    {
        #region Properties

        [WeakMethod]
        public static TsObject media_game_has_control(ITsInstance inst, TsObject[] args)
        {
            return MediaPlayer.GameHasControl;
        }

        [WeakMethod]
        public static TsObject media_get_muted(ITsInstance inst, TsObject[] args)
        {
            return MediaPlayer.IsMuted;
        }

        [WeakMethod]
        public static TsObject media_set_muted(ITsInstance inst, TsObject[] args)
        {
            MediaPlayer.IsMuted = (bool)args[0];
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject media_get_repeating(ITsInstance inst, TsObject[] args)
        {
            return MediaPlayer.IsRepeating;
        }

        [WeakMethod]
        public static TsObject media_set_repeating(ITsInstance inst, TsObject[] args)
        {
            MediaPlayer.IsRepeating = (bool)args[0];
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject media_get_shuffled(ITsInstance inst, TsObject[] args)
        {
            return MediaPlayer.IsShuffled;
        }

        [WeakMethod]
        public static TsObject media_set_shuffled(ITsInstance inst, TsObject[] args)
        {
            MediaPlayer.IsShuffled = (bool)args[0];
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject media_get_visualization_ended(ITsInstance inst, TsObject[] args)
        {
            return MediaPlayer.IsVisualizationEnabled;
        }

        [WeakMethod]
        public static TsObject media_get_position(ITsInstance inst, TsObject[] args)
        {
            return MediaPlayer.PlayPosition.Milliseconds;
        }

        [WeakMethod]
        public static TsObject media_get_state(ITsInstance inst, TsObject[] args)
        {
            return (float)MediaPlayer.State;
        }

        [WeakMethod]
        public static TsObject media_get_volume(ITsInstance inst, TsObject[] args)
        {
            return MediaPlayer.Volume;
        }

        [WeakMethod]
        public static TsObject media_set_volume(ITsInstance inst, TsObject[] args)
        {
            MediaPlayer.Volume = (float)args[0];
            return TsObject.Empty();
        }

        #endregion

        #region Methods

        [WeakMethod]
        public static TsObject media_move_next(ITsInstance inst, TsObject[] args)
        {
            MediaPlayer.MoveNext();
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject media_move_previous(ITsInstance inst, TsObject[] args)
        {
            MediaPlayer.MovePrevious();
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject media_pause(ITsInstance inst, TsObject[] args)
        {
            MediaPlayer.Pause();
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject media_play(ITsInstance inst, TsObject[] args)
        {
            MediaPlayer.Play(((TsSong)args[0]).Source);
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject media_resume(ITsInstance inst, TsObject[] args)
        {
            MediaPlayer.Resume();
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject media_stop(ITsInstance inst, TsObject[] args)
        {
            MediaPlayer.Stop();
            return TsObject.Empty();
        }

        #endregion

        #region Events

        private static EventCache<MediaEventType> _cache = new EventCache<MediaEventType>();

        [WeakMethod]
        public static TsObject media_subscribe(ITsInstance inst, TsObject[] args)
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
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject media_unsubscribe(ITsInstance inst, TsObject[] args)
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
