using Microsoft.Xna.Framework.Media;
using System;

namespace TaffyScript.MonoGame.Sound
{
    public class TsSong : ITsInstance
    {
        public Song Source { get; }

        public string ObjectType => "TaffyScript.MonoGame.Sound.Song";

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public TsSong(Song song)
        {
            Source = song;
        }

        public TsObject GetMember(string name)
        {
            switch(name)
            {
                case "album":
                    return Source.Album?.Name ?? "";
                case "artist":
                    return Source.Artist?.Name ?? "";
                case "duration":
                    return Source.Duration.Milliseconds;
                case "genre":
                    return Source.Genre?.Name ?? "";
                case "is_protected":
                    return Source.IsProtected;
                case "is_rated":
                    return Source.IsRated;
                case "name":
                    return Source.Name;
                case "play_count":
                    return Source.PlayCount;
                case "rating":
                    return Source.Rating;
                case "track_number":
                    return Source.TrackNumber;
                default:
                    if (TryGetDelegate(name, out var del))
                        return del;
                    throw new MissingMemberException(ObjectType, name);
            }
        }

        public void SetMember(string name, TsObject value)
        {
            throw new MissingMemberException(ObjectType, name);
        }

        public bool TryGetDelegate(string delegateName, out TsDelegate del)
        {
            switch(delegateName)
            {
                case "dispose":
                    del = new TsDelegate(dispose, "dispose", this);
                    return true;
                default:
                    del = null;
                    return false;
            }
        }

        public TsDelegate GetDelegate(string delegateName)
        {
            if (TryGetDelegate(delegateName, out var del))
                return del;
            throw new MissingMethodException(ObjectType, delegateName);
        }

        public TsObject Call(string scriptName, params TsObject[] args)
        {
            switch (scriptName)
            {
                case "dispose":
                    return dispose(null, args);
                default:
                    throw new MissingMethodException(ObjectType, scriptName);
            }
        }

        private TsObject dispose(ITsInstance inst, TsObject[] args)
        {
            if (!Source.IsDisposed)
            {
                Source.Dispose();
                return true;
            }
            return false;
        }

        public static implicit operator TsObject(TsSong song)
        {
            return new TsObject(song);
        }

        public static explicit operator TsSong(TsObject obj)
        {
            return (TsSong)obj.Value.WeakValue;
        }
    }
}
