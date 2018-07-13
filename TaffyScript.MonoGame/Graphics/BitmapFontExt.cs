using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    public static class BitmapFontExt
    {
        public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, string text, Vector2 position, Color color)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            BitmapChar bmp;
            var y = position.Y;
            var x = position.X;
            foreach (var chars in text)
            {
                if (chars == '\n')
                {
                    y += font.LineSpacing;
                    x = position.X;
                }
                else if (font.CharacterMap.TryGetValue(chars, out bmp) && bmp.Source.HasValue)
                {
                    var src = bmp.Source.Value;
                    spriteBatch.Draw(font.FontTexture, new Rectangle((int)x, (int)y, src.Width, src.Height), src, color);
                    x += src.Width;
                }
            }
        }

        public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, string text, int start, int count, Vector2 position, Color color)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));
            BitmapChar bmp;
            var y = position.Y;
            var x = position.X;
            var end = start + count;
            for(var i = start; i < end; i++)
            {
                if(text[i] == '\n')
                {
                    y += font.LineSpacing;
                    x = position.X;
                }
                else if(font.CharacterMap.TryGetValue(text[i], out bmp) && bmp.Source.HasValue)
                {
                    var src = bmp.Source.Value;
                    spriteBatch.Draw(font.FontTexture, new Rectangle((int)x, (int)y, src.Width, src.Height), src, color);
                    x += src.Width;
                }
            }
        }

        public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, StringBuilder text, Vector2 position, Color color)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            BitmapChar bmp;
            var x = position.X;
            var y = position.Y;
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    y += font.LineSpacing;
                    x = position.X;
                }
                else if (font.CharacterMap.TryGetValue(text[i], out bmp) && bmp.Source.HasValue)
                {
                    var src = bmp.Source.Value;
                    spriteBatch.Draw(font.FontTexture, new Rectangle((int)x, (int)y, src.Width, src.Height), src, color);
                    x += src.Width;
                }
            }
        }

        public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, StringBuilder text, int start, int count, Vector2 position, Color color)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            BitmapChar bmp;
            var x = position.X;
            var y = position.Y;
            var end = start + count;
            for (var i = start; i < end; i++)
            {
                if (text[i] == '\n')
                {
                    y += font.LineSpacing;
                    x = position.X;
                }
                else if (font.CharacterMap.TryGetValue(text[i], out bmp) && bmp.Source.HasValue)
                {
                    var src = bmp.Source.Value;
                    spriteBatch.Draw(font.FontTexture, new Rectangle((int)x, (int)y, src.Width, src.Height), src, color);
                    x += src.Width;
                }
            }
        }
    }
}