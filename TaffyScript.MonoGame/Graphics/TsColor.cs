using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TaffyScript.MonoGame.Graphics
{
    public struct TsColor : ITsInstance
    {
        public Color Source { get; }

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public string ObjectType => "TaffyScript.MonoGame.Graphics.Color";

        public TsColor(TsObject[] args)
        {
            switch(args.Length)
            {
                case 1:
                    Source = new Microsoft.Xna.Framework.Color((uint)args[0]);
                    break;
                case 3:
                    Source = new Microsoft.Xna.Framework.Color((int)args[0], (int)args[1], (int)args[2]);
                    break;
                case 4:
                    Source = new Microsoft.Xna.Framework.Color((int)args[0], (int)args[1], (int)args[2], (int)args[3]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public TsColor(Color color)
        {
            Source = color;
        }

        public TsObject Call(string scriptName, params TsObject[] args)
        {
            throw new MissingMethodException(ObjectType, scriptName);
        }

        public TsDelegate GetDelegate(string delegateName)
        {
            if (TryGetDelegate(delegateName, out var del))
                return del;
            throw new MissingMethodException(ObjectType, delegateName);
        }

        public TsObject GetMember(string name)
        {
            switch(name)
            {
                case "r":
                    return Source.R;
                case "g":
                    return Source.G;
                case "b":
                    return Source.B;
                case "a":
                    return Source.A;
                case "packed_value":
                    return Source.PackedValue;
                default:
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

        public static implicit operator TsObject(TsColor color)
        {
            return new TsInstanceWrapper(color);
        }

        public static explicit operator TsColor(TsObject obj)
        {
            return (TsColor)obj.WeakValue;
        }
    }
}
