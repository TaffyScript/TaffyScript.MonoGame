using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    [TaffyScriptObject]
    public class Surface : ITsInstance
    {
        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public string ObjectType => "TaffyScript.MonoGame.Graphics.Surface";
        public RenderTarget2D Source { get; }

        public Surface(TsObject[] args)
        {
            Source = new RenderTarget2D(SpriteBatchManager.SpriteBatch.GraphicsDevice, (int)args[0], (int)args[1]);
        }

        public Surface(RenderTarget2D source)
        {
            Source = source;
        }

        public TsObject Call(string scriptName, params TsObject[] args)
        {
            switch (scriptName)
            {
                case "draw":
                    SpriteBatchManager.SpriteBatch.Draw(Source, new Vector2((float)args[0], (float)args[1]), SpriteBatchManager.DrawColor);
                    break;
                case "draw_stretched":
                    SpriteBatchManager.SpriteBatch.Draw(Source,
                                                        new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                        null,
                                                        SpriteBatchManager.DrawColor);
                    break;
                case "draw_ext":
                    SpriteBatchManager.SpriteBatch.Draw(Source,
                                                        new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                        null,
                                                        ((TsColor)args[5]).Source,
                                                        (float)args[4],
                                                        Vector2.Zero,
                                                        SpriteEffects.None,
                                                        0f);
                    break;
                case "draw_part":
                    SpriteBatchManager.SpriteBatch.Draw(Source,
                                                        new Rectangle((int)args[0], (int)args[1], (int)args[4], (int)args[5]),
                                                        new Rectangle((int)args[2], (int)args[3], (int)args[4], (int)args[5]),
                                                        SpriteBatchManager.DrawColor);
                    break;
                case "draw_part_stretched":
                    SpriteBatchManager.SpriteBatch.Draw(Source,
                                                        new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                        new Rectangle((int)args[4], (int)args[5], (int)args[6], (int)args[7]),
                                                        SpriteBatchManager.DrawColor);
                    break;
                case "draw_part_ext":
                    SpriteBatchManager.SpriteBatch.Draw(Source,
                                                        new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                        new Rectangle((int)args[4], (int)args[5], (int)args[6], (int)args[7]),
                                                        ((TsColor)args[9]).Source,
                                                        (float)args[8],
                                                        Vector2.Zero,
                                                        SpriteEffects.None,
                                                        0f);
                    break;
                case "dispose":
                    Source.Dispose();
                    break;
                default:
                    throw new MemberAccessException();
            }
            return TsObject.Empty;
        }

        public TsDelegate GetDelegate(string delegateName)
        {
            if (TryGetDelegate(delegateName, out var del))
                return del;

            throw new MemberAccessException();
        }

        public TsObject GetMember(string name)
        {
            switch (name)
            {
                case "width":
                    return Source.Width;
                case "height":
                    return Source.Height;
                default:
                    throw new MemberAccessException();
            }
        }

        public void SetMember(string name, TsObject value)
        {
            throw new MemberAccessException();
        }

        public bool TryGetDelegate(string delegateName, out TsDelegate del)
        {
            switch (delegateName)
            {
                case "draw":
                    del = new TsDelegate(draw, "draw");
                    return true;
                case "draw_stretched":
                    del = new TsDelegate(draw_stretched, "draw_stretched");
                    return true;
                case "draw_ext":
                    del = new TsDelegate(draw_ext, "draw_ext");
                    return true;
                case "draw_part":
                    del = new TsDelegate(draw_part, "draw_part");
                    return true;
                case "draw_part_stretched":
                    del = new TsDelegate(draw_part_stretched, "draw_part_stretched");
                    return true;
                case "draw_part_ext":
                    del = new TsDelegate(draw_part_ext, "draw_part_ext");
                    return true;
                case "dispose":
                    del = new TsDelegate(dispose, "dispose");
                    return true;
                default:
                    del = null;
                    return false;
            }
        }

        public static implicit operator TsObject(Surface surface)
        {
            return new TsInstanceWrapper(surface);
        }

        public static explicit operator Surface(TsObject obj)
        {
            return (Surface)obj.WeakValue;
        }

        public TsObject draw(params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source, new Vector2((float)args[0], (float)args[1]), SpriteBatchManager.DrawColor);
            return TsObject.Empty;
        }

        public TsObject draw_stretched(params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                null,
                                                SpriteBatchManager.DrawColor);
            return TsObject.Empty;
        }

        public TsObject draw_ext(params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                null,
                                                ((TsColor)args[5]).Source,
                                                (float)args[4],
                                                Vector2.Zero,
                                                SpriteEffects.None,
                                                0f);
            return TsObject.Empty;
        }

        public TsObject draw_part(params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[4], (int)args[5]),
                                                new Rectangle((int)args[2], (int)args[3], (int)args[4], (int)args[5]),
                                                SpriteBatchManager.DrawColor);
            return TsObject.Empty;
        }

        public TsObject draw_part_stretched(params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                new Rectangle((int)args[4], (int)args[5], (int)args[6], (int)args[7]),
                                                SpriteBatchManager.DrawColor);
            return TsObject.Empty;
        }

        public TsObject draw_part_ext(params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                new Rectangle((int)args[4], (int)args[5], (int)args[6], (int)args[7]),
                                                ((TsColor)args[9]).Source,
                                                (float)args[8],
                                                Vector2.Zero,
                                                SpriteEffects.None,
                                                0f);
            return TsObject.Empty;
        }

        public TsObject dispose(params TsObject[] args)
        {
            Source.Dispose();
            return TsObject.Empty;
        }
    }
}
