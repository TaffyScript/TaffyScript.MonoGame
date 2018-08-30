using System;
using System.Linq;
using TaffyScript.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    public class ShaderTechnique : ITsInstance
    {
        private TsList _annotations;
        private TsList _passes;

        public EffectTechnique Source { get; }
        public string ObjectType => "TaffyScript.MonoGame.Graphics.ShaderTechnique";

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public TsList Annotations
        {
            get
            {
                if (_annotations is null)
                    _annotations = new TsList(Source.Annotations.Select(a => new TsInstanceWrapper(new ShaderAnnotation(a))));
                return _annotations;
            }
        }

        public TsList Passes
        {
            get
            {
                if (_passes is null)
                    _passes = new TsList(Source.Passes.Select(p => new TsInstanceWrapper(new ShaderPass(p))));
                return _passes;
            }
        }

        public ShaderTechnique(EffectTechnique source)
        {
            Source = source;
        }

        public TsObject GetMember(string name)
        {
            switch(name)
            {
                case "annotations":
                    return Annotations;
                case "name":
                    return Source.Name;
                case "passes":
                    return Passes;
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
            del = null;
            return false;
        }

        public TsDelegate GetDelegate(string delegateName)
        {
            if (TryGetDelegate(delegateName, out var del))
                return del;
            throw new MissingMethodException(ObjectType, delegateName);
        }

        public TsObject Call(string scriptName, params TsObject[] args)
        {
            throw new MissingMethodException(ObjectType, scriptName);
        }

        public static implicit operator TsObject(ShaderTechnique shaderTechnique)
        {
            return new TsInstanceWrapper(shaderTechnique);
        }

        public static explicit operator ShaderTechnique(TsObject obj)
        {
            return (ShaderTechnique)obj.WeakValue;
        }
    }
}
