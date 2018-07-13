using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    [WeakObject]
    public class TsTexture : ITsInstance
    {
        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public string ObjectType => "TaffyScript.MonoGame.Graphics.Texture2D";
        public Texture2D Source { get; }

        public TsTexture(Texture2D texture)
        {
            Source = texture;
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
                case "get_data":
                    var size = Source.Width * Source.Height;
                    var data = new uint[size];
                    Source.GetData(data);
                    var result = new TsObject[size];
                    for (var i = 0; i < size; i++)
                        result[i] = data[i];
                    return result;
                case "set_data":
                    var src = (TsObject[])args[0];
                    size = src.Length;
                    data = new uint[size];
                    for (var i = 0; i < size; i++)
                        data[i] = (uint)src[i];
                    Source.SetData(data);
                    break;
                default:
                    throw new MemberAccessException();
            }

            return TsObject.Empty();
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
                case "name":
                    return Source.Name;
                case "bounds":
                    return new TsObject[] { 0, 0, Source.Width, Source.Height };
                default:
                    throw new MemberAccessException($"Tried to access non-existant member \"{name}\" on imported type {ObjectType}");
            }
        }

        public void SetMember(string name, TsObject value)
        {
            throw new MemberAccessException($"Cannot set any members on imported type {ObjectType}");
        }

        public bool TryGetDelegate(string delegateName, out TsDelegate del)
        {
            switch (delegateName)
            {
                case "draw":
                    del = new TsDelegate(draw, "draw", this);
                    return true;
                case "draw_stretched":
                    del = new TsDelegate(draw_stretched, "draw_stretched", this);
                    return true;
                case "draw_ext":
                    del = new TsDelegate(draw_ext, "draw_ext", this);
                    return true;
                case "draw_part":
                    del = new TsDelegate(draw_part, "draw_part", this);
                    return true;
                case "draw_part_stretched":
                    del = new TsDelegate(draw_part_stretched, "draw_part_stretched", this);
                    return true;
                case "draw_part_ext":
                    del = new TsDelegate(draw_part_ext, "draw_part_ext", this);
                    return true;
                case "get_data":
                    del = new TsDelegate(get_data, "get_data", this);
                    return true;
                case "set_data":
                    del = new TsDelegate(set_data, "set_data", this);
                    return true;
                default:
                    del = null;
                    return false;
            }
        }

        public static implicit operator TsObject(TsTexture self)
        {
            return new TsObject(self);
        }

        public static explicit operator TsTexture(TsObject obj)
        {
            return (TsTexture)obj.Value.WeakValue;
        }

        public TsObject draw(ITsInstance target, params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source, new Vector2((float)args[0], (float)args[1]), SpriteBatchManager.DrawColor);
            return TsObject.Empty();
        }

        public TsObject draw_stretched(ITsInstance target, params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                null,
                                                SpriteBatchManager.DrawColor);
            return TsObject.Empty();
        }

        public TsObject draw_ext(ITsInstance target, params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                null,
                                                new Color((uint)args[5]),
                                                (float)args[4],
                                                Vector2.Zero,
                                                SpriteEffects.None,
                                                0f);
            return TsObject.Empty();
        }

        public TsObject draw_part(ITsInstance target, params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[4], (int)args[5]),
                                                new Rectangle((int)args[2], (int)args[3], (int)args[4], (int)args[5]),
                                                SpriteBatchManager.DrawColor);
            return TsObject.Empty();
        }

        public TsObject draw_part_stretched(ITsInstance target, params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                new Rectangle((int)args[4], (int)args[5], (int)args[6], (int)args[7]),
                                                SpriteBatchManager.DrawColor);
            return TsObject.Empty();
        }

        public TsObject draw_part_ext(ITsInstance target, params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Source,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                new Rectangle((int)args[4], (int)args[5], (int)args[6], (int)args[7]),
                                                new Color((uint)args[9]),
                                                (float)args[8],
                                                Vector2.Zero,
                                                SpriteEffects.None,
                                                0f);
            return TsObject.Empty();
        }

        public TsObject get_data(ITsInstance target, params TsObject[] args)
        {
            var size = Source.Width * Source.Height;
            var data = new uint[size];
            Source.GetData(data);
            var result = new TsObject[size];
            for (var i = 0; i < size; i++)
                result[i] = data[i];
            return result;
        }

        public TsObject set_data(ITsInstance target, params TsObject[] args)
        {
            var src = (TsObject[])args[0];
            var size = src.Length;
            var data = new uint[size];
            for (var i = 0; i < size; i++)
                data[i] = (uint)src[i];
            Source.SetData(data);
            return TsObject.Empty();
        }
    }
}
