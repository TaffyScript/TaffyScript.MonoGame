using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace TaffyScript.MonoGame.Sound
{
    public class TsSoundInstance : ITsInstance
    {
        public SoundEffectInstance Source { get; }

        public string ObjectType => "TaffyScript.MonoGame.Sound.SoundInstance";

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public TsSoundInstance(SoundEffectInstance source)
        {
            Source = source;
        }

        public TsObject GetMember(string name)
        {
            switch(name)
            {
                case "is_disposed":
                    return Source.IsDisposed;
                case "is_looped":
                    return Source.IsLooped;
                case "pan":
                    return Source.Pan;
                case "pitch":
                    return Source.Pitch;
                case "state":
                    return (float)Source.State;
                case "volume":
                    return Source.Volume;
                default:
                    if (TryGetDelegate(name, out var del))
                        return del;
                    throw new MissingMemberException(ObjectType, name);
            }
        }

        public void SetMember(string name, TsObject value)
        {
            switch (name)
            {
                case "is_looped":
                    Source.IsLooped = (bool)value;
                    break;
                case "pan":
                    Source.Pan = (float)value;
                    break;
                case "pitch":
                    Source.Pitch = (float)value;
                    break;
                case "volume":
                    Source.Volume = (float)value;
                    break;
                default:
                    throw new MissingMemberException(ObjectType, name);
            }
        }

        public bool TryGetDelegate(string delegateName, out TsDelegate del)
        {
            switch(delegateName)
            {
                case "apply_3d":
                    del = new TsDelegate(apply_3d, delegateName, this);
                    return true;
                case "dispose":
                    del = new TsDelegate(dispose, delegateName, this);
                    return true;
                case "pause":
                    del = new TsDelegate(pause, delegateName, this);
                    return true;
                case "play":
                    del = new TsDelegate(play, delegateName, this);
                    return true;
                case "resume":
                    del = new TsDelegate(resume, delegateName, this);
                    return true;
                case "stop":
                    del = new TsDelegate(stop, delegateName, this);
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
                case "apply_3d":
                    return apply_3d(null, args);
                case "dispose":
                    return dispose(null, args);
                case "pause":
                    return pause(null, args);
                case "play":
                    return play(null, args);
                case "resume":
                    return resume(null, args);
                case "stop":
                    return stop(null, args);
                default:
                    throw new MissingMethodException(ObjectType, scriptName);
            }
        }

        private TsObject apply_3d(ITsInstance inst, TsObject[] args)
        {
            throw new NotImplementedException();
        }

        private TsObject dispose(ITsInstance inst, TsObject[] args)
        {
            Source.Dispose();
            return TsObject.Empty();
        }

        private TsObject pause(ITsInstance inst, TsObject[] args)
        {
            Source.Pause();
            return TsObject.Empty();
        }

        private TsObject play(ITsInstance inst, TsObject[] args)
        {
            Source.Play();
            return TsObject.Empty();
        }

        private TsObject resume(ITsInstance inst, TsObject[] args)
        {
            Source.Resume();
            return TsObject.Empty();
        }

        private TsObject stop(ITsInstance inst, TsObject[] args)
        {
            Source.Stop();
            return TsObject.Empty();
        }

        public static implicit operator TsObject(TsSoundInstance soundInstance)
        {
            return new TsObject(soundInstance);
        }

        public static explicit operator TsSoundInstance(TsObject obj)
        {
            return (TsSoundInstance)obj.Value.WeakValue;
        }
    }
}
