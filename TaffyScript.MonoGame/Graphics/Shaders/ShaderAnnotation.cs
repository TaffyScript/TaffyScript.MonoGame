using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    public class ShaderAnnotation : ITsInstance
    {
        public EffectAnnotation Source { get; }
        public string ObjectType => "TaffyScript.MonoGame.Graphics.ShaderAnnotation";

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public ShaderAnnotation(EffectAnnotation source)
        {
            Source = source;
        }

        public TsObject GetMember(string name)
        {
            switch(name)
            {
                case "column_count":
                    return Source.ColumnCount;
                case "name":
                    return Source.Name;
                case "parameter_class":
                    return (float)Source.ParameterClass;
                case "parameter_type":
                    return (float)Source.ParameterType;
                case "row_count":
                    return Source.RowCount;
                case "semantic":
                    return Source.Semantic;
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

        public static implicit operator TsObject(ShaderAnnotation shaderAnnotation)
        {
            return new TsInstanceWrapper(shaderAnnotation);
        }

        public static explicit operator ShaderAnnotation(TsObject obj)
        {
            return (ShaderAnnotation)obj.WeakValue;
        }
    }
}
