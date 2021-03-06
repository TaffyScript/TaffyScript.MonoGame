﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace TaffyScript.MonoGame.Sound
{
    public class TsSound : ITsInstance
    {
        public SoundEffect Source { get; }

        public string ObjectType => "TaffyScript.MonoGame.Sound.Sound";

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public TsSound(SoundEffect source)
        {
            Source = source;
        }

        public TsObject GetMember(string name)
        {
            switch(name)
            {
                case "duration":
                    return Source.Duration.Milliseconds;
                case "is_disposed":
                    return Source.IsDisposed;
                case "name":
                    return Source.Name;
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
                case "name":
                    Source.Name = (string)value;
                    break;
                default:
                    throw new MissingMemberException(ObjectType, name);
            }
        }

        public bool TryGetDelegate(string delegateName, out TsDelegate del)
        {
            switch(delegateName)
            {
                case "create_instance":
                    del = new TsDelegate(create_instance, delegateName);
                    return true;
                case "dispose":
                    del = new TsDelegate(dispose, delegateName);
                    return true;
                case "play":
                    del = new TsDelegate(play, delegateName);
                    return true;
                case "play_ext":
                    del = new TsDelegate(play_ext, delegateName);
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
                case "create_instance":
                    return create_instance(args);
                case "dispose":
                    return dispose(args);
                case "play":
                    return play(args);
                case "play_ext":
                    return play_ext(args);
                default:
                    throw new MissingMethodException(ObjectType, scriptName);
            }
        }

        private TsObject create_instance(TsObject[] args)
        {
            return new TsSoundInstance(Source.CreateInstance());
        }

        private TsObject dispose(TsObject[] args)
        {
            if (!Source.IsDisposed)
            {
                Source.Dispose();
                return true;
            }
            return false;
        }

        private TsObject play(TsObject[] args)
        {
            return Source.Play();
        }

        private TsObject play_ext(TsObject[] args)
        {
            return Source.Play((float)args[0], (float)args[1], (float)args[2]);
        }

        public static implicit operator TsObject(TsSound sound)
        {
            return new TsInstanceWrapper(sound);
        }

        public static explicit operator TsSound(TsObject obj)
        {
            return (TsSound)obj.WeakValue;
        }
    }
}
