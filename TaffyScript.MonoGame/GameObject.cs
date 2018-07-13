using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TaffyScript.MonoGame.Collisions;

namespace TaffyScript.MonoGame
{
    public delegate void PositionChangedDelegate(RectangleF previousPosition, Shape currentPosition);
    public delegate void DepthChangedDelegate(int previousDepth, int currentDepth);

    [WeakObject]
    public class GameObject : ITsInstance
    {
        private Shape _collider = new NullCollider();
        private int _depth = 0;
        protected Dictionary<string, TsObject> _members = new Dictionary<string, TsObject>();

        public event PositionChangedDelegate PositionChanged;
        public event DepthChangedDelegate DepthChanged;

        public virtual TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public Shape Collider
        {
            get => _collider;
            set => SetCollider(_collider);
        }

        public int Depth
        {
            get => _depth;
            set
            {
                if(_depth != value)
                {
                    var depth = _depth;
                    _depth = value;
                    DepthChanged?.Invoke(depth, _depth);
                }
            }
        }

        public virtual string ObjectType => "TaffyScript.MonoGame.GameObject";

        public Screen Screen { get; set; }

        public GameObject(TsObject[] args)
        {
        }

        public virtual TsObject Call(string scriptName, params TsObject[] args)
        {
            switch(scriptName)
            {
                default:
                    if (TryGetDelegate(scriptName, out var del))
                        return del.Invoke(args);
                    throw new MissingMemberException(ObjectType, scriptName);
            }
        }

        public virtual TsDelegate GetDelegate(string delegateName)
        {
            if (TryGetDelegate(delegateName, out var del))
                return del;
            throw new MissingMemberException(ObjectType, delegateName);
        }

        public virtual TsObject GetMember(string name)
        {
            switch(name)
            {
                case "x":
                    return _collider.Position.X;
                case "y":
                    return _collider.Position.Y;
                case "position":
                    return new TsObject[] { _collider.Position.X, _collider.Position.Y };
                case "collider":
                    return _collider;
                case "bbox":
                    var bounds = _collider.BoundingBox;
                    return new TsObject[] { bounds.X, bounds.Y, bounds.Width, bounds.Height };
                case "screen":
                    return Screen ?? TsObject.Empty();
                case "depth":
                    return Depth;
                default:
                    return _members[name];
            }
        }

        public virtual void SetMember(string name, TsObject value)
        {
            switch (name)
            {
                case "x":
                    SetPosition(new Vector2((float)value, _collider.Position.Y));
                    break;
                case "y":
                    SetPosition(new Vector2(_collider.Position.X, (float)value));
                    break;
                case "position":
                    var array = value.GetArray1D();
                    SetPosition(new Vector2((float)array[0], (float)array[1]));
                    break;
                case "collider":
                    SetCollider(value.Value.WeakValue as Shape);
                    break;
                case "depth":
                    Depth = (int)value;
                    break;
                default:
                    _members[name] = value;
                    break;
            }
        }

        public virtual bool TryGetDelegate(string delegateName, out TsDelegate del)
        {
            switch (delegateName)
            {
                default:
                    if (_members.TryGetValue(delegateName, out var member) && member.Type == VariableType.Delegate)
                    {
                        del = member.GetDelegateUnchecked();
                        return true;
                    }
                    del = null;
                    return false;
            }
        }

        private void SetPosition(Vector2 position)
        {
            var prev = _collider.BoundingBox;
            _collider.Position = position;
            PositionChanged?.Invoke(prev, _collider);
        }

        private void SetCollider(Shape collider)
        {
            var pos = _collider.Position;
            var prev = _collider.BoundingBox;
            _collider = collider ?? new NullCollider();
            _collider.Position = pos;
            PositionChanged?.Invoke(prev, collider);
        }
    }
}
