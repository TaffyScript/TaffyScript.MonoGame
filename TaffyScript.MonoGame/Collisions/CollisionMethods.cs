using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaffyScript.Collections;
using Microsoft.Xna.Framework;
using TaffyScript.Reflection;

namespace TaffyScript.MonoGame.Collisions
{
    [TaffyScriptBaseType]
    public static class CollisionMethods
    {
        [TaffyScriptMethod]
        public static TsObject instance_collides_with(TsObject[] args)
        {
            var go = (GameObject)args[0];
            switch (args.Length)
            {
                case 1:
                    return go.Screen.CollisionWorld.Any(go);
                default:
                    var type = (string)args[1];
                    foreach (var item in go.Screen.CollisionWorld.Broadphase(go).Where(i => go.Collider.Overlaps(i.Collider)))
                    {
                        if (TsReflection.ObjectIs(item.ObjectType, type))
                            return true;
                    }
                    return false;
            }
        }
        
        [TaffyScriptMethod]
        public static TsObject instance_collides_with_place(TsObject[] args)
        {
            var go = (GameObject)args[0];
            var pos = go.Collider.Position;
            go.Collider.Position = new Vector2((float)args[1], (float)args[2]);

            switch(args.Length)
            {
                case 3:
                    var result = go.Screen.CollisionWorld.Any(go);
                    go.Collider.Position = pos;
                    return result;
                default:
                    var type = (string)args[3];
                    foreach (var item in go.Screen.CollisionWorld.Broadphase(go).Where(i => go.Collider.Overlaps(i.Collider)))
                    {
                        if (TsReflection.ObjectIs(item.ObjectType, type))
                        {
                            go.Collider.Position = pos;
                            return true;
                        }
                    }
                    break;
            }

            go.Collider.Position = pos;
            return false;
        }

        [TaffyScriptMethod]
        public static TsObject point_collides(TsObject[] args)
        {
            var point = new Vector2((float)args[0], (float)args[1]);
            switch(args.Length)
            {
                case 2:
                    return ScreenManager.CurrentScreen.CollisionWorld.Broadphase(ref point).Where(i => i.Collider.ContainsPoint(point)).Any();
                default:
                    var type = (string)args[2];
                    foreach (var item in ScreenManager.CurrentScreen.CollisionWorld.Broadphase(ref point).Where(i => i.Collider.ContainsPoint(point)))
                    {
                        if (TsReflection.ObjectIs(item.ObjectType, type))
                            return true;
                    }
                    break;
            }
            return false;
        }

        [TaffyScriptMethod]
        public static TsObject point_collision(TsObject[] args)
        {
            var point = new Vector2((float)args[0], (float)args[1]);
            switch(args.Length)
            {
                case 2:
                    var result = ScreenManager.CurrentScreen.CollisionWorld.Broadphase(ref point).Where(i => i.Collider.ContainsPoint(point)).FirstOrDefault();
                    if (result is null)
                        break;
                    return result;
                default:
                    var type = (string)args[2];
                    foreach (var item in ScreenManager.CurrentScreen.CollisionWorld.Broadphase(ref point).Where(i => i.Collider.ContainsPoint(point)))
                    {
                        if (TsReflection.ObjectIs(item.ObjectType, type))
                            return item;
                    }
                    break;
            }
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject point_collisions(TsObject[] args)
        {
            var point = new Vector2((float)args[0], (float)args[1]);
            switch(args.Length)
            {
                case 2:
                    return new TsList(ScreenManager.CurrentScreen.CollisionWorld.Broadphase(ref point)
                                          .Where(go => go.Collider.ContainsPoint(point))
                                          .Select(obj => new TsInstanceWrapper(obj)));
                default:
                    var type = (string)args[2];
                    return new TsList(ScreenManager.CurrentScreen.CollisionWorld.Broadphase(ref point)
                                          .Where(i => TsReflection.ObjectIs(i.ObjectType, type) && i.Collider.ContainsPoint(point))
                                          .Select(obj => new TsInstanceWrapper(obj)));
            }
        }
    }
}