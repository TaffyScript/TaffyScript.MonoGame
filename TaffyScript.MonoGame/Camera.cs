using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TaffyScript.MonoGame.ViewportAdapters;

namespace TaffyScript.MonoGame
{
    public class Camera : ITsInstance
    {
        private float _maximumZoom = float.MaxValue;
        private float _minimumZoom = 0.00000000001f;
        private float _zoom;

        public Vector2 Origin { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public ViewportAdapter Viewport { get; }
        public string ObjectType => "TaffyScript.MonoGame.Camera";

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public float Zoom
        {
            get => _zoom;
            set
            {
                if (value < MinimumZoom || value > MaximumZoom)
                    throw new ArgumentException("Zoom must be between MinimumZoom and MaximumZoom");
                _zoom = value;
            }
        }

        public float MinimumZoom
        {
            get => _minimumZoom;
            set
            {
                if (value < 0)
                    throw new ArgumentException("MinimumZoom must be greater than zero");

                _minimumZoom = value;

                if (Zoom < value)
                    Zoom = value;
            }
        }

        public float MaximumZoom
        {
            get => _maximumZoom;
            set
            {
                if (value < MinimumZoom)
                    throw new ArgumentException("MaximumZoom must be greater than MinumumZoom");

                _maximumZoom = value;

                if (Zoom > value)
                    Zoom = value;
            }
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                var frustram = GetBoundingFrustum();
                var corners = frustram.GetCorners();
                var topLeft = corners[0];
                var bottomRight = corners[2];
                var width = bottomRight.X - topLeft.X;
                var height = bottomRight.Y - topLeft.Y;
                return new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)width, (int)height);
            }
        }

        public Camera(ViewportAdapter viewportAdapter)
        {
            Viewport = viewportAdapter;
            Rotation = 0;
            Zoom = 1;
            Origin = new Vector2(viewportAdapter.VirtualWidth / 2f, viewportAdapter.VirtualHeight / 2f);
            Position = Vector2.Zero;
        }

        public void Move(Vector2 direction)
        {
            Position += Vector2.Transform(direction, Matrix.CreateRotationZ(-Rotation));
        }

        public void Rotate(float deltaRadians)
        {
            Rotation += deltaRadians;
        }

        public void ZoomIn(float deltaZoom)
        {
            ClampZoom(Zoom + deltaZoom);
        }

        public void ZoomOut(float deltaZoom)
        {
            ClampZoom(Zoom - deltaZoom);
        }

        public void LookAt(Vector2 position)
        {
            Position = position - new Vector2(Viewport.VirtualWidth / 2f, Viewport.VirtualHeight / 2f);
        }

        public Vector2 WorldToScreen(float x, float y)
        {
            return WorldToScreen(new Vector2(x, y));
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            var viewport = Viewport.Viewport;
            return Vector2.Transform(worldPosition + new Vector2(viewport.X, viewport.Y), GetViewMatrix());
        }

        public Vector2 ScreenToWorld(int x, int y)
        {
            return ScreenToWorld(new Vector2(x, y));
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            var viewport = Viewport.Viewport;
            return Vector2.Transform(screenPosition - new Vector2(viewport.X, viewport.Y), Matrix.Invert(GetViewMatrix()));
        }

        public Matrix GetViewMatrix()
        {
            return GetViewMatrix(Vector2.One);
        }

        public Matrix GetViewMatrix(Vector2 parallaxFactor)
        {
            return GetVirtualViewMatrix(parallaxFactor) * Viewport.GetScaleMatrix();
        }

        public Matrix GetInverseViewMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        public BoundingFrustum GetBoundingFrustum()
        {
            var viewMatrix = GetVirtualViewMatrix();
            var projectionMatrix = GetProjectionMatrix(viewMatrix);
            return new BoundingFrustum(projectionMatrix);
        }

        public ContainmentType ContainsPoint(Point point)
        {
            return Contains(point.ToVector2());
        }

        public ContainmentType Contains(Vector2 position)
        {
            return GetBoundingFrustum().Contains(new Vector3(position, 0));
        }

        public ContainmentType Contains(Rectangle rectangle)
        {
            var max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
            var min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
            var boundingBox = new BoundingBox(min, max);
            return GetBoundingFrustum().Contains(boundingBox);
        }

        private void ClampZoom(float value)
        {
            Zoom = MathF.Clamp(value, MinimumZoom, MaximumZoom);
        }


        private Matrix GetVirtualViewMatrix()
        {
            return GetVirtualViewMatrix(Vector2.One);
        }

        private Matrix GetVirtualViewMatrix(Vector2 parallaxFactor)
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        private Matrix GetProjectionMatrix(Matrix viewMatrix)
        {
            var projection = Matrix.CreateOrthographicOffCenter(0, Viewport.VirtualWidth, Viewport.VirtualHeight, 0, -1, 0);
            Matrix.Multiply(ref viewMatrix, ref projection, out projection);
            return projection;
        }

        #region TaffyScript

        public TsObject Call(string scriptName, TsObject[] args)
        {
            switch (scriptName)
            {
                case "move":
                    return move(args);
                case "rotate":
                    return rotate(args);
                case "zoom_in":
                    return zoom_in(args);
                case "zoom_out":
                    return zoom_out(args);
                case "look_at":
                    return look_at(args);
                case "screen_to_world":
                    return screen_to_world(args);
                case "world_to_screen":
                    return world_to_screen(args);
                case "get_bounds":
                    return get_bounds(args);
                default:
                    throw new MissingMethodException(ObjectType, scriptName);
            }
        }

        public bool TryGetDelegate(string scriptName, out TsDelegate del)
        {
            switch (scriptName)
            {
                case "move":
                    del = new TsDelegate(move, "move");
                    return true;
                case "rotate":
                    del = new TsDelegate(rotate, "rotate");
                    return true;
                case "zoom_in":
                    del = new TsDelegate(zoom_in, "zoom_in");
                    return true;
                case "zoom_out":
                    del = new TsDelegate(zoom_out, "zoom_out");
                    return true;
                case "look_at":
                    del = new TsDelegate(look_at, "look_at");
                    return true;
                case "screen_to_world":
                    del = new TsDelegate(screen_to_world, "screen_to_world");
                    return true;
                case "world_to_screen":
                    del = new TsDelegate(world_to_screen, "world_to_screen");
                    return true;
                case "get_bounds":
                    del = new TsDelegate(get_bounds, "get_bounds");
                    return true;
                default:
                    del = null;
                    return false;
            }
        }

        public TsDelegate GetDelegate(string scriptName)
        {
            if (TryGetDelegate(scriptName, out var del))
                return del;
            throw new MissingMethodException(ObjectType, scriptName);
        }

        public TsObject GetMember(string name)
        {
            switch(name)
            {
                case "origin":
                    return new TsObject[] { Origin.X, Origin.Y };
                case "origin_x":
                    return Origin.X;
                case "origin_y":
                    return Origin.Y;
                case "position":
                    return new TsObject[] { Position.X, Position.Y };
                case "position_x":
                    return Position.X;
                case "position_y":
                    return Position.Y;
                case "rotation":
                    return Rotation;
                case "minimum_zoom":
                    return MinimumZoom;
                case "maximum_zoom":
                    return MaximumZoom;
                case "zoom":
                    return Zoom;
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
                case "origin":
                    var array = value.GetArray();
                    Origin = new Vector2((float)array[0], (float)array[1]);
                    break;
                case "origin_x":
                    Origin = new Vector2((float)value, Origin.Y);
                    break;
                case "origin_y":
                    Origin = new Vector2(Origin.X, (float)value);
                    break;
                case "position":
                    array = value.GetArray();
                    Position = new Vector2((float)array[0], (float)array[1]);
                    break;
                case "position_x":
                    Position = new Vector2((float)value, Position.Y);
                    break;
                case "position_y":
                    Position = new Vector2(Position.X, (float)value);
                    break;
                case "rotation":
                    Rotation = (float)value;
                    break;
                case "minimum_zoom":
                    MinimumZoom = (float)value;
                    break;
                case "maximum_zoom":
                    MaximumZoom = (float)value;
                    break;
                case "zoom":
                    Zoom = (float)value;
                    break;
                default:
                    throw new MissingMemberException(ObjectType, name);
            }
        }

        private TsObject move(TsObject[] args)
        {
            Move(new Vector2((float)args[0], (float)args[1]));
            return TsObject.Empty;
        }

        private TsObject rotate(TsObject[] args)
        {
            Rotate((float)args[0]);
            return TsObject.Empty;
        }

        private TsObject zoom_in(TsObject[] args)
        {
            ZoomIn((float)args[0]);
            return TsObject.Empty;
        }

        private TsObject zoom_out(TsObject[] args)
        {
            ZoomOut((float)args[0]);
            return TsObject.Empty;
        }

        private TsObject look_at(TsObject[] args)
        {
            LookAt(new Vector2((float)args[0], (float)args[1]));
            return TsObject.Empty;
        }

        private TsObject screen_to_world(TsObject[] args)
        {
            var vec = ScreenToWorld(new Vector2((float)args[0], (float)args[1]));
            return new TsObject[] { vec.X, vec.Y };
        }

        private TsObject world_to_screen(TsObject[] args)
        {
            var vec = WorldToScreen(new Vector2((float)args[0], (float)args[1]));
            return new TsObject[] { vec.X, vec.Y };
        }

        private TsObject get_bounds(TsObject[] args)
        {
            var bounds = BoundingRectangle;
            return new TsObject[] { bounds.X, bounds.Y, bounds.Width, bounds.Height };
        }

        public static implicit operator TsObject(Camera cam)
        {
            return new TsInstanceWrapper(cam);
        }

        public static explicit operator Camera(TsObject obj)
        {
            return (Camera)obj.WeakValue;
        }

        #endregion
    }
}
