using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    public class ShaderPass : ITsInstance
    {
        public EffectPass Source { get; }

        public string ObjectType => throw new NotImplementedException();

        public TsObject this[string memberName] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ShaderPass(EffectPass source)
        {
            Source = source;
        }

        public TsObject GetMember(string name)
        {
            throw new NotImplementedException();
        }

        public void SetMember(string name, TsObject value)
        {
            throw new NotImplementedException();
        }

        public bool TryGetDelegate(string delegateName, out TsDelegate del)
        {
            throw new NotImplementedException();
        }

        public TsDelegate GetDelegate(string delegateName)
        {
            throw new NotImplementedException();
        }

        public TsObject Call(string scriptName, params TsObject[] args)
        {
            throw new NotImplementedException();
        }

        public static implicit operator TsObject(ShaderPass shaderPass)
        {
            return new TsInstanceWrapper(shaderPass);
        }

        public static explicit operator ShaderPass(TsObject obj)
        {
            return (ShaderPass)obj.WeakValue;
        }
    }
}
