using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TaffyScript.MonoGame.Graphics;

namespace TaffyScript.MonoGame.Collisions
{
    public abstract class Shape : ITsInstance
    {
        public virtual Vector2 Position { get; set; }
        public abstract RectangleF BoundingBox { get; }
        public abstract string ObjectType { get; }

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public abstract bool Overlaps(Shape other);
        public abstract bool CollidesWithShape(Shape other, out CollisionResult result);
        public abstract bool CollidesWithLine(Vector2 start, Vector2 end);
        public abstract bool CollidesWithLine(Vector2 start, Vector2 end, out RaycastHit hit);
        public abstract bool ContainsPoint(Vector2 point);
        public abstract bool CollidesWithPoint(Vector2 point, out CollisionResult result);
        public abstract void DebugDraw(SpriteBatch spriteBatch, Color color);

        public virtual TsObject Call(string name, params TsObject[] args)
        {
            switch (name)
            {
                case "overlaps":
                    return overlaps(args);
                case "collides_with":
                    return collides_with(args);
                case "collides_with_line":
                    return collides_with_line(args);
                case "contains_point":
                    return contains_point(args);
                case "collides_with_point":
                    return collides_with_point(args);
                case "debug_draw":
                    return debug_draw(args);
                default:
                    throw new MissingMethodException(ObjectType, name);
            }
        }

        public virtual TsObject GetMember(string name)
        {
            switch (name)
            {
                case "position":
                    return new TsObject[] { Position.X, Position.Y };
                case "position_x":
                    return Position.X;
                case "position_y":
                    return Position.Y;
                case "bounds":
                    // Get a copy of the BoundingBox in case the property
                    // does any heavy computations.
                    var bounds = BoundingBox;
                    return new TsObject[] { bounds.X, bounds.Y, bounds.Width, bounds.Height };
                case "bounds_x":
                    return BoundingBox.X;
                case "bounds_y":
                    return BoundingBox.Y;
                case "bounds_width":
                    return BoundingBox.Width;
                case "bounds_height":
                    return BoundingBox.Height;
                default:
                    if (TryGetDelegate(name, out var del))
                        return del;
                    throw new MissingMemberException(ObjectType, name);
            }
        }

        public virtual void SetMember(string name, TsObject value)
        {
            switch(name)
            {
                case "position":
                    var array = value.GetArray();
                    Position = new Vector2((float)array[0], (float)array[1]);
                    break;
                case "position_x":
                    Position = new Vector2((float)value, Position.Y);
                    break;
                case "position_y":
                    Position = new Vector2(Position.X, (float)value);
                    break;
                default:
                    throw new MissingFieldException(ObjectType, name);
            }
        }

        public virtual bool TryGetDelegate(string name, out TsDelegate del)
        {
            switch (name)
            {
                case "overlaps":
                    del = new TsDelegate(overlaps, name);
                    return true;
                case "collides_with":
                    del = new TsDelegate(collides_with, name);
                    return true;
                case "collides_with_line":
                    del = new TsDelegate(collides_with_line, name);
                    return true;
                case "contains_point":
                    del = new TsDelegate(contains_point, name);
                    return true;
                case "collides_with_point":
                    del = new TsDelegate(collides_with_point, name);
                    return true;
                case "debug_draw":
                    del = new TsDelegate(debug_draw, name);
                    return true;
                default:
                    del = null;
                    return false;
            }
        }

        public TsDelegate GetDelegate(string memberName)
        {
            if (TryGetDelegate(memberName, out var del))
                return del;
            throw new MissingMethodException(ObjectType, memberName);
        }

        private TsObject overlaps(TsObject[] args)
        {
            return Overlaps((Shape)args[0]);
        }

        private TsObject collides_with(TsObject[] args)
        {
            return CollidesWithShape((Shape)args[0], out _);
        }

        private TsObject collides_with_line(TsObject[] args)
        {
            return CollidesWithLine(new Vector2((float)args[0], (float)args[1]), new Vector2((float)args[2], (float)args[3]));
        }

        private TsObject contains_point(TsObject[] args)
        {
            return ContainsPoint(new Vector2((float)args[0], (float)args[1]));
        }

        private TsObject collides_with_point(TsObject[] args)
        {
            return CollidesWithPoint(new Vector2((float)args[0], (float)args[1]), out _);
        }

        private TsObject debug_draw(TsObject[] args)
        {
            DebugDraw(SpriteBatchManager.SpriteBatch, args is null || args.Length == 0 ? SpriteBatchManager.DrawColor : ((TsColor)args[0]).Source);
            return TsObject.Empty;
        }

        public static implicit operator TsObject(Shape shape)
        {
            return new TsInstanceWrapper(shape);
        }

        public static explicit operator Shape(TsObject obj)
        {
            return (Shape)obj.WeakValue;
        }
    }
}
