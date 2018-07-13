using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaffyScript;
using Microsoft.Xna.Framework;

namespace TaffyScript.MonoGame.Collisions
{
    [WeakBaseType]
    public static class CollisionMethods
    {
        [WeakMethod]
        public static TsObject collides_with_any(ITsInstance inst, TsObject[] args)
        {
            var go = (GameObject)inst;
            return go.Screen.CollisionWorld.Any(go);
        }

        [WeakMethod]
        public static TsObject collides_with(ITsInstance inst, TsObject[] args)
        {
            var go = (GameObject)inst;
            foreach(var item in go.Screen.CollisionWorld.Broadphase(go).Where(i => go.Collider.Overlaps(i.Collider)))
            {
                if (TsReflection.ObjectIs(item.ObjectType, (string)args[0]))
                    return true;
            }
            return false;
        }
        
        [WeakMethod]
        public static TsObject collides_with_place(ITsInstance inst, TsObject[] args)
        {
            var go = (GameObject)inst;
            var pos = go.Collider.Position;
            go.Collider.Position = new Vector2((float)args[0], (float)args[1]);
            foreach (var item in go.Screen.CollisionWorld.Broadphase(go).Where(i => go.Collider.Overlaps(i.Collider)))
            {
                if (TsReflection.ObjectIs(item.ObjectType, (string)args[2]))
                {
                    go.Collider.Position = pos;
                    return true;
                }
            }

            go.Collider.Position = pos;
            return false;
        }


        [WeakMethod]
        public static TsObject point_direction(ITsInstance inst, TsObject[] args)
        {
            return MathF.Atan2((float)args[1] - (float)args[3], (float)args[2] - (float)args[0]);
        }
    }
}
