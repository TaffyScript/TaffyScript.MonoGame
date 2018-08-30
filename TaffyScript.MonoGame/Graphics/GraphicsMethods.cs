using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    [TaffyScriptBaseType]
    public static class GraphicsMethods
    {
        [TaffyScriptMethod]
        public static TsObject render_target_set(TsObject[] args)
        {
            SpriteBatchManager.SetRenderTarget(((Surface)args[0]).Source);
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject render_target_reset(TsObject[] args)
        {
            SpriteBatchManager.SetRenderTarget(null);
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject draw_begin(TsObject[] args)
        {
            if (!SpriteBatchManager.Drawing)
            {
                SpriteBatchManager.Begin();
                return true;
            }
            return false;
        }

        [TaffyScriptMethod]
        public static TsObject draw_end(TsObject[] args)
        {
            if (SpriteBatchManager.Drawing)
            {
                SpriteBatchManager.End();
                return true;
            }
            return false;
        }

        [TaffyScriptMethod]
        public static TsObject draw_is_active(TsObject[] args)
        {
            return SpriteBatchManager.Drawing;
        }

        [TaffyScriptMethod]
        public static TsObject draw_set_color(TsObject[] args)
        {
            SpriteBatchManager.DrawColor = ((TsColor)args[0]).Source;
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject draw_get_color(TsObject[] args)
        {
            return new TsColor(SpriteBatchManager.DrawColor);
        }

        [TaffyScriptMethod]
        public static TsObject make_color(TsObject[] args)
        {
            return new TsColor(args);
        }

        [TaffyScriptMethod]
        public static TsObject draw_rectangle(TsObject[] args)
        {
            Primitives2D.FillRectangle(SpriteBatchManager.SpriteBatch,
                                       new Vector2((float)args[0], (float)args[1]),
                                       new Vector2((float)args[2], (float)args[3]),
                                       SpriteBatchManager.DrawColor,
                                       args.Length > 4 ? (float)args[4] * MathF.Deg2Rad : 0f);
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject draw_rectangle_color(TsObject[] args)
        {
            Primitives2D.FillRectangle(SpriteBatchManager.SpriteBatch,
                                       new Vector2((float)args[0], (float)args[1]),
                                       new Vector2((float)args[2], (float)args[3]),
                                       ((TsColor)args[4]).Source,
                                       args.Length > 5 ? (float)args[5] * MathF.Deg2Rad : 0f);
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject draw_circle(TsObject[] args)
        {
            var radius = (float)args[2];
            Primitives2D.DrawCircle(SpriteBatchManager.SpriteBatch,
                                    new Vector2((float)args[0], (float)args[1]),
                                    radius,
                                    (int)args[3],
                                    SpriteBatchManager.DrawColor,
                                    radius);
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject draw_circle_color(TsObject[] args)
        {
            var radius = (float)args[2];
            Primitives2D.DrawCircle(SpriteBatchManager.SpriteBatch,
                                    new Vector2((float)args[0], (float)args[1]),
                                    radius,
                                    (int)args[3],
                                    ((TsColor)args[4]).Source,
                                    radius);
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject draw_set_background_color(TsObject[] args)
        {
            SpriteBatchManager.BackgroundColor = ((TsColor)args[0]).Source;
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject draw_get_background_color(TsObject[] args)
        {
            return new TsColor(SpriteBatchManager.BackgroundColor);
        }
    }
}