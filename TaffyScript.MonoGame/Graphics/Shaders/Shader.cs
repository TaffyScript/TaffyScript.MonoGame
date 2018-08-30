using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TaffyScript.Collections;

namespace TaffyScript.MonoGame.Graphics
{
    public class Shader : ITsInstance
    {
        private TsList _parameters;
        private TsList _techniques;
        public Effect Source { get; }

        public string ObjectType => "TaffyScript.MonoGame.Graphics.Shader";

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public TsList Parameters
        {
            get
            {
                if (_parameters is null)
                    _parameters = new TsList(Source.Parameters.Select(p => new TsInstanceWrapper(new ShaderParameter(p))));
                return _parameters;
            }
        }

        public TsList Techniques
        {
            get
            {
                if(_techniques is null)
                    _techniques = new TsList(Source.Techniques.Select(t => new TsInstanceWrapper(new ShaderTechnique(t))));
                return _techniques;
            }
        }

        public Shader(Effect source)
        {
            Source = source;
        }

        public TsObject GetMember(string name)
        {
            switch(name)
            {
                case "current_technique":
                    return new ShaderTechnique(Source.CurrentTechnique);
                case "is_disposed":
                    return Source.IsDisposed;
                case "name":
                    return Source.Name;
                case "parameters":
                    return Parameters;
                case "techniques":
                    return Techniques;
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
                case "current_technique":
                    Source.CurrentTechnique = ((ShaderTechnique)value).Source;
                    break;
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
                case "clone":
                    del = new TsDelegate(clone, delegateName);
                    return true;
                case "dispose":
                    del = new TsDelegate(dispose, delegateName);
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
                case "clone":
                    return clone(args);
                case "dispose":
                    return dispose(args);
                default:
                    throw new MissingMethodException(ObjectType, scriptName);
            }
        }

        private TsObject clone(TsObject[] args)
        {
            return new Shader(Source.Clone());
        }

        private TsObject dispose(TsObject[] args)
        {
            if(!Source.IsDisposed)
            {
                Source.Dispose();
                return true;
            }
            return false;
        }

        public static implicit operator TsObject(Shader shader)
        {
            return new TsInstanceWrapper(shader);
        }

        public static explicit operator Shader(TsObject obj)
        {
            return (Shader)obj.WeakValue;
        }
    }
}
