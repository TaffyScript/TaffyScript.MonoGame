using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    [WeakBaseType]
    public static class GraphicsMethods
    {
        [WeakMethod]
        public static TsObject render_target_set(ITsInstance inst, TsObject[] args)
        {
            SpriteBatchManager.SetRenderTarget(((Surface)args[0]).Source);
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject render_target_reset(ITsInstance inst, TsObject[] args)
        {
            SpriteBatchManager.SetRenderTarget(null);
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject draw_begin(ITsInstance inst, TsObject[] args)
        {
            if (!SpriteBatchManager.Drawing)
            {
                SpriteBatchManager.Begin();
                return true;
            }
            return false;
        }

        [WeakMethod]
        public static TsObject draw_end(ITsInstance isnt, TsObject[] args)
        {
            if (SpriteBatchManager.Drawing)
            {
                SpriteBatchManager.End();
                return true;
            }
            return false;
        }

        [WeakMethod]
        public static TsObject draw_is_active(ITsInstance inst, TsObject[] args)
        {
            return SpriteBatchManager.Drawing;
        }

        [WeakMethod]
        public static TsObject draw_set_color(ITsInstance inst, TsObject[] args)
        {
            SpriteBatchManager.DrawColor = ((TsColor)args[0]).Source;
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject draw_get_color(ITsInstance inst, TsObject[] args)
        {
            return new TsColor(SpriteBatchManager.DrawColor);
        }

        [WeakMethod]
        public static TsObject make_color(ITsInstance inst, TsObject[] args)
        {
            return new TsColor(args);
        }

        [WeakMethod]
        public static TsObject draw_rectangle(ITsInstance inst, TsObject[] args)
        {
            Primitives2D.FillRectangle(SpriteBatchManager.SpriteBatch,
                                       new Vector2((float)args[0], (float)args[1]),
                                       new Vector2((float)args[2], (float)args[3]),
                                       SpriteBatchManager.DrawColor,
                                       args.Length > 4 ? (float)args[4] * MathF.Deg2Rad : 0f);
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject draw_rectangle_color(ITsInstance inst, TsObject[] args)
        {
            Primitives2D.FillRectangle(SpriteBatchManager.SpriteBatch,
                                       new Vector2((float)args[0], (float)args[1]),
                                       new Vector2((float)args[2], (float)args[3]),
                                       ((TsColor)args[4]).Source,
                                       args.Length > 5 ? (float)args[5] * MathF.Deg2Rad : 0f);
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject draw_circle(ITsInstance inst, TsObject[] args)
        {
            var radius = (float)args[2];
            Primitives2D.DrawCircle(SpriteBatchManager.SpriteBatch,
                                    new Vector2((float)args[0], (float)args[1]),
                                    radius,
                                    (int)args[3],
                                    SpriteBatchManager.DrawColor,
                                    radius);
            return TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject draw_circle_color(ITsInstance inst, TsObject[] args)
        {
            var radius = (float)args[2];
            Primitives2D.DrawCircle(SpriteBatchManager.SpriteBatch,
                                    new Vector2((float)args[0], (float)args[1]),
                                    radius,
                                    (int)args[3],
                                    ((TsColor)args[4]).Source,
                                    radius);
            return TsObject.Empty();
        }
    }
}