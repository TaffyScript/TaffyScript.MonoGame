using System;
using System.Linq;
using TaffyScript.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    public class ShaderParameter : ITsInstance
    {
        private TsList _annotations;
        private TsList _elements;
        private TsList _structureMembers;

        public EffectParameter Source { get; }
        public string ObjectType => "TaffyScript.MonoGame.Graphics.ShaderParameter";

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

        public TsList Elements
        {
            get
            {
                if (_elements is null)
                    _elements = new TsList(Source.Elements.Select(e => new TsInstanceWrapper(new ShaderParameter(e))));
                return _elements;
            }
        }

        public TsList StructureMembers
        {
            get
            {
                if (_structureMembers is null)
                    _structureMembers = new TsList(Source.StructureMembers.Select(m => new TsInstanceWrapper(new ShaderParameter(m))));
                return _structureMembers;
            }
        }

        public ShaderParameter(EffectParameter source)
        {
            Source = source;
        }

        public TsObject GetMember(string name)
        {
            switch(name)
            {
                case "annotations":
                    return Annotations;
                case "column_count":
                    return Source.ColumnCount;
                case "elements":
                    return Elements;
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
                case "structure_members":
                    return StructureMembers;
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
            throw new NotImplementedException();
        }

        public TsDelegate GetDelegate(string delegateName)
        {
            throw new NotImplementedException();
        }

        public TsObject Call(string scriptName, params TsObject[] args)
        {
            switch(scriptName)
            {
                case "get_bool":
                case "get_int":
                default:
                    throw new NotImplementedException();
            }
        }

        private TsObject get_bool(TsObject[] args)
        {
            return Source.GetValueBoolean();
        }

        private TsObject get_int(TsObject[] args)
        {
            return Source.GetValueInt32();
        }

        public static implicit operator TsObject(ShaderParameter parameter)
        {
            return new TsInstanceWrapper(parameter);
        }

        public static explicit operator ShaderParameter(TsObject obj)
        {
            return (ShaderParameter)obj.WeakValue;
        }
    }
}
