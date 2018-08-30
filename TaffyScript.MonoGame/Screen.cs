using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Myst.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TaffyScript.Collections;
using TaffyScript.MonoGame.Input;
using TaffyScript.MonoGame.Graphics;
using TaffyScript.MonoGame.Collisions;
using TaffyScript.Reflection;

namespace TaffyScript.MonoGame
{
    [TaffyScriptObject]
    public class Screen : ITsInstance
    {
        private const int DefaultCellSize = 100;

        private Dictionary<GameObject, InstanceInfo> _instances = new Dictionary<GameObject, InstanceInfo>();
        private SortedDictionary<int, FastList<TsDelegate>> _updatable = new SortedDictionary<int, FastList<TsDelegate>>();
        private SortedDictionary<int, FastList<TsDelegate>> _drawable = new SortedDictionary<int, FastList<TsDelegate>>();
        private SortedDictionary<int, FastList<TsDelegate>> _gui = new SortedDictionary<int, FastList<TsDelegate>>();
        private Dictionary<GameObject, InstanceInfo> _addUpdatable = new Dictionary<GameObject, InstanceInfo>();
        private List<InstanceInfo> _removeUpdatable = new List<InstanceInfo>();
        private Dictionary<GameObject, InstanceInfo> _addDrawable = new Dictionary<GameObject, InstanceInfo>();
        private List<InstanceInfo> _removeDrawable = new List<InstanceInfo>();

        public Camera Camera { get; private set; }
        public SpatialHash CollisionWorld { get; }
        public string ObjectType => "TaffyScript.MonoGame.Screen";

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public Screen()
        {
            Camera = new Camera(ScreenManager.ViewportAdapter);
            CollisionWorld = new SpatialHash(DefaultCellSize);
        }

        public Screen(Camera camera)
        {
            Camera = camera;
            CollisionWorld = new SpatialHash(DefaultCellSize);
        }

        public Screen(TsObject[] args)
        {
            Camera = new Camera(ScreenManager.ViewportAdapter);
            var cellSize = args is null || args.Length == 0 ? DefaultCellSize : (int)args[0];
            CollisionWorld = new SpatialHash(cellSize);
        }

        public void AddInstance(GameObject inst, int depth, bool callScript)
        {
            if (_instances.ContainsKey(inst))
                return;

            _addUpdatable.Remove(inst);

            var info = new InstanceInfo();
            info.Depth = depth;

            if (inst.TryGetDelegate("update", out var update))
                info.Update = update;

            if (inst.TryGetDelegate("draw", out var draw))
                info.Draw = draw;

            if (inst.TryGetDelegate("draw_gui", out var drawGui))
                info.DrawGui = drawGui;

            if(!(inst.Collider is NullCollider))
                CollisionWorld.Add(inst);

            info.DepthChanged = (prev, current) => { RemoveInstance(inst, prev, false); AddInstance(inst, current, false); };
            inst.DepthChanged += info.DepthChanged;

            info.PositionChanged = (prev, current) => 
            {
                CollisionWorld.PrepForMove(inst, prev);
                if (!(inst.Collider is NullCollider))
                    CollisionWorld.Add(inst);
            };

            inst.PositionChanged += info.PositionChanged;

            _addUpdatable.Add(inst, info);
            _addDrawable.Add(inst, info);
            _instances.Add(inst, info);

            inst.Screen = this;

            if(callScript && inst.TryGetDelegate("screen_added", out var screenAdded))
                screenAdded.Invoke();
        }

        public void AddTile(Texture2D texture, Rectangle source, Vector2 position, int depth)
        {
            if(!_drawable.TryGetValue(depth, out var list))
            {
                list = new FastList<TsDelegate>();
                _drawable.Add(depth, list);
            }
            list.Add(new TsDelegate((a) => { SpriteBatchManager.SpriteBatch.Draw(texture, position, source, Color.White); return TsObject.Empty; }, "draw_tile"));
        }

        public void RemoveInstance(GameObject inst, int depth, bool callScript)
        {
            if (!_instances.TryGetValue(inst, out var info))
            {
                _addUpdatable.Remove(inst);
                _addDrawable.Remove(inst);
                return;
            }
            else
                _instances.Remove(inst);

            CollisionWorld.Remove(inst);

            _removeDrawable.Add(info);
            _removeUpdatable.Add(info);
            inst.DepthChanged -= info.DepthChanged;
            inst.PositionChanged -= info.PositionChanged;

            if (callScript && inst.TryGetDelegate("screen_removed", out var screenRemoved))
                screenRemoved.Invoke();

            inst.Screen = null;
        }

        public void Update(GameTime gameTime)
        {
            foreach(var removable in _removeUpdatable)
            {
                if (removable.Update != null)
                    _updatable[removable.Depth].Remove(removable.Update);
            }
            foreach(var addable in _addUpdatable.Values)
            {
                if(addable.Update != null)
                {
                    if(!_updatable.TryGetValue(addable.Depth, out var layer))
                    {
                        layer = new FastList<TsDelegate>();
                        _updatable[addable.Depth] = layer;
                    }
                    layer.Add(addable.Update);
                }
            }
            _addUpdatable.Clear();
            _removeUpdatable.Clear();

            Vector2 pos = Camera != null ? Camera.ScreenToWorld(InputManager.MousePosition) : InputManager.MousePosition;
            TsInstance.Global["mouse_x"] = pos.X;
            TsInstance.Global["mouse_y"] = pos.Y;

            foreach(var layer in _updatable.Values)
            {
                for (var i = 0; i < layer.Count; i++)
                    layer.Buffer[i].Invoke(gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        public void Draw()
        {
            foreach(var removable in _removeDrawable)
            {
                if (removable.Draw != null)
                    _drawable[removable.Depth].Remove(removable.Draw);
                if (removable.DrawGui != null)
                    _gui[removable.Depth].Remove(removable.DrawGui);
            }
            foreach(var addable in _addDrawable.Values)
            {
                if(addable.Draw != null)
                {
                    if(!_drawable.TryGetValue(addable.Depth, out var layer))
                    {
                        layer = new FastList<TsDelegate>();
                        _drawable.Add(addable.Depth, layer);
                    }
                    layer.Add(addable.Draw);
                }
                if(addable.DrawGui != null)
                {
                    if(!_gui.TryGetValue(addable.Depth, out var layer))
                    {
                        layer = new FastList<TsDelegate>();
                        _gui.Add(addable.Depth, layer);
                    }
                    layer.Add(addable.DrawGui);
                }
            }

            _removeDrawable.Clear();
            _addDrawable.Clear();

            if (Camera != null)
                SpriteBatchManager.TransformMatrix = Camera.GetViewMatrix();

            SpriteBatchManager.Begin();

            foreach(var layer in _drawable.Values)
            {
                for (var i = 0; i < layer.Count; i++)
                    layer.Buffer[i].Invoke();
            }

            SpriteBatchManager.End();

            SpriteBatchManager.TransformMatrix = Camera.Viewport.GetScaleMatrix();

            SpriteBatchManager.Begin();

            foreach (var layer in _gui.Values)
            {
                for (var i = 0; i < layer.Count; i++)
                    layer.Buffer[i].Invoke();
            }

            SpriteBatchManager.End();
        }

        private struct InstanceInfo
        {
            public int Depth;
            public TsDelegate Update;
            public TsDelegate Draw;
            public TsDelegate DrawGui;
            public DepthChangedDelegate DepthChanged;
            public PositionChangedDelegate PositionChanged;
        }

        public TsObject Call(string scriptName, params TsObject[] args)
        {
            switch(scriptName)
            {
                case "add":
                    return add(args);
                case "add_tile":
                    return add_tile(args);
                case "find":
                    return find(args);
                case "find_all":
                    return find_all(args);
                case "remove":
                    return remove(args);
                default:
                    throw new MissingMethodException(ObjectType, scriptName);
            }
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
                case "camera":
                    return Camera;
                case "instance_count":
                    return _instances.Count;
                default:
                    if (TryGetDelegate(name, out var del))
                        return del;
                    throw new MissingMemberException(ObjectType, name);
            }
        }

        public void SetMember(string name, TsObject value)
        {
            throw new MissingFieldException(ObjectType, name);
        }

        public bool TryGetDelegate(string scriptName, out TsDelegate del)
        {
            switch(scriptName)
            {
                case "add":
                    del = new TsDelegate(add, "add");
                    return true;
                case "add_tile":
                    del = new TsDelegate(add_tile, "add_tile");
                    return true;
                case "find":
                    del = new TsDelegate(find, "find");
                    return true;
                case "find_all":
                    del = new TsDelegate(find_all, "find_all");
                    return true;
                case "remove":
                    del = new TsDelegate(remove, "remove");
                    return true;
                default:
                    del = null;
                    return false;
            }
        }

        private TsObject add(TsObject[] args)
        {
            var go = args[0].WeakValue as GameObject;
            if (go is null)
                throw new ArgumentException("Expected a GameObject, got " + args[0].ToString(), "argument0");
            AddInstance(go, go.Depth, true);
            return TsObject.Empty;
        }

        private TsObject add_tile(TsObject[] args)
        {
            AddTile(((TsTexture)args[0]).Source, new Rectangle((int)args[1], (int)args[2], (int)args[3], (int)args[4]), new Vector2((float)args[5], (float)args[6]), (int)args[7]);
            return TsObject.Empty;
        }

        private TsObject remove(TsObject[] args)
        {
            var go = args[0].WeakValue as GameObject;
            if (go is null)
                throw new ArgumentException("Expected a GameObject, got " + args[0].ToString(), "argument0");
            RemoveInstance(go, go.Depth, true);
            return TsObject.Empty;
        }

        private TsObject find(TsObject[] args)
        {
            var type = (string)args[0];
            foreach (var item in _instances.Keys)
                if (TsReflection.ObjectIs(item.ObjectType, type))
                    return item;
            return TsObject.Empty;
        }

        private TsObject find_all(TsObject[] args)
        {
            var type = (string)args[0];
            return new TsList(_instances.Keys.Where(i => TsReflection.ObjectIs(i.ObjectType, type)).Select(i => new TsInstanceWrapper(i)));
        }

        public static implicit operator TsObject(Screen screen)
        {
            return new TsInstanceWrapper(screen);
        }

        public static explicit operator Screen(TsObject obj)
        {
            return (Screen)obj.WeakValue;
        }
    }
}
