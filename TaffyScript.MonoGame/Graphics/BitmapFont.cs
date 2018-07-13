using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    public class BitmapFont : Font
    {
        private Dictionary<char, BitmapChar> _characterMap = new Dictionary<char, BitmapChar>();

        public override int LineSpacing { get; }
        public Texture2D FontTexture { get; }
        public IReadOnlyDictionary<char, BitmapChar> CharacterMap => _characterMap;

        public BitmapFont(Texture2D fontTexture, IEnumerable<BitmapChar> characters)
        {
            FontTexture = fontTexture;
            LineSpacing = 0;
            foreach (var bmpChar in characters)
            {
                _characterMap.Add(bmpChar.Character, bmpChar);
                if (bmpChar.Source?.Height > LineSpacing)
                    LineSpacing = bmpChar.Source.Value.Height;
            }
        }

        public BitmapFont(Texture2D fontTexture, IEnumerable<BitmapChar> characters, int lineHeight)
        {
            FontTexture = fontTexture;
            foreach (var bmpChar in characters)
                _characterMap.Add(bmpChar.Character, bmpChar);
            LineSpacing = lineHeight;
        }

        public static BitmapFont FromStrip(Texture2D strip, string characterMap, int width, int padding, int spaceSize, bool autoSize = false)
        {
            var list = new List<BitmapChar>();
            var x = 0;
            Rectangle src;
            Color[] rawData = null;
            if (autoSize)
                rawData = new Color[width * strip.Height];
            foreach (var chars in characterMap)
            {
                src = new Rectangle(x, 0, width, strip.Height);
                if (chars == ' ')
                    src.Width = spaceSize;
                else if (autoSize)
                    src.Width = GetLetterWidth(strip, rawData, ref src);

                var bmpChar = new BitmapChar(chars, src);
                x += width + padding;
                list.Add(bmpChar);
            }
            return new BitmapFont(strip, list, strip.Height);
        }

        private static int GetLetterWidth(Texture2D texture, Color[] rawData, ref Rectangle src)
        {
            texture.GetData(0, src, rawData, 0, src.Width * src.Height);
            int width = 0;
            for (var row = src.Height - 1; row >= 0; row--)
            {
                for (var column = src.Width - 1; column >= 0; column--)
                {
                    if (column <= width)
                        continue;
                    var color = rawData[row * src.Width + column];
                    if (color.A != 0)
                        width = column;
                }
            }
            return width + 1;
        }

        public override void DrawString(string text, Vector2 position, Color color)
        {
            BitmapChar bmp;
            var y = position.Y;
            var x = position.X;
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    y += LineSpacing;
                    x = position.X;
                }
                else if (_characterMap.TryGetValue(text[i], out bmp) && bmp.Source.HasValue)
                {
                    var src = bmp.Source.Value;
                    SpriteBatchManager.SpriteBatch.Draw(FontTexture, new Rectangle((int)x, (int)y, src.Width, src.Height), src, color);
                    x += src.Width;
                }
            }
        }

        public override void DrawString(string text, Vector2 position, int start, int count, Color color)
        {
            BitmapChar bmp;
            var y = position.Y;
            var x = position.X;
            var end = start + count;
            for(var i = start; i < end; i++)
            {
                if (text[i] == '\n')
                {
                    y += LineSpacing;
                    x = position.X;
                }
                else if (_characterMap.TryGetValue(text[i], out bmp) && bmp.Source.HasValue)
                {
                    var src = bmp.Source.Value;
                    SpriteBatchManager.SpriteBatch.Draw(FontTexture, new Rectangle((int)x, (int)y, src.Width, src.Height), src, color);
                    x += src.Width;
                }
            }
        }

        public override void DrawString(StringBuilder text, Vector2 position, Color color)
        {
            BitmapChar bmp;
            var y = position.Y;
            var x = position.X;
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    y += LineSpacing;
                    x = position.X;
                }
                else if (_characterMap.TryGetValue(text[i], out bmp) && bmp.Source.HasValue)
                {
                    var src = bmp.Source.Value;
                    SpriteBatchManager.SpriteBatch.Draw(FontTexture, new Rectangle((int)x, (int)y, src.Width, src.Height), src, color);
                    x += src.Width;
                }
            }
        }

        public override void DrawString(StringBuilder text, Vector2 position, int start, int count, Color color)
        {
            BitmapChar bmp;
            var y = position.Y;
            var x = position.X;
            var end = start + count;
            for (var i = start; i < end; i++)
            {
                if (text[i] == '\n')
                {
                    y += LineSpacing;
                    x = position.X;
                }
                else if (_characterMap.TryGetValue(text[i], out bmp) && bmp.Source.HasValue)
                {
                    var src = bmp.Source.Value;
                    SpriteBatchManager.SpriteBatch.Draw(FontTexture, new Rectangle((int)x, (int)y, src.Width, src.Height), src, color);
                    x += src.Width;
                }
            }
        }

        public override Vector2 MeasureString(string text)
        {
            var width = 0;
            var height = LineSpacing;
            BitmapChar bmp;
            foreach (var chars in text)
            {
                if (_characterMap.TryGetValue(chars, out bmp))
                    width += bmp.Source.Value.Width;
                else if (chars == '\n')
                    height += LineSpacing;
            }
            return new Vector2(width, height);
        }

        public override Vector2 MeasureString(string text, int start, int count)
        {
            var width = 0;
            var height = LineSpacing;
            BitmapChar bmp;
            var end = start + count;
            for(var i = start; i < end; i++)
            {
                if (_characterMap.TryGetValue(text[i], out bmp))
                    width += bmp.Source.Value.Width;
                else if (text[i] == '\n')
                    height += LineSpacing;
            }
            return new Vector2(width, height);
        }

        public override Vector2 MeasureString(StringBuilder text)
        {
            var width = 0;
            var height = LineSpacing;
            BitmapChar bmp;
            for (var i = 0; i < text.Length; i++)
            {
                if (_characterMap.TryGetValue(text[i], out bmp) && bmp.Source.HasValue)
                    width += bmp.Source.Value.Width;
                else if (text[i] == '\n')
                    height += LineSpacing;
            }
            return new Vector2(width, height);
        }

        public override Vector2 MeasureString(StringBuilder text, int start, int count)
        {
            var width = 0;
            var height = LineSpacing;
            BitmapChar bmp;
            var end = start + count;
            for (var i = start; i < count; i++)
            {
                if (_characterMap.TryGetValue(text[i], out bmp) && bmp.Source.HasValue)
                    width += bmp.Source.Value.Width;
                else if (text[i] == '\n')
                    height += LineSpacing;
            }
            return new Vector2(width, height);
        }

        public static explicit operator BitmapFont(TsObject obj)
        {
            return (BitmapFont)obj.Value.WeakValue;
        }
    }

    public struct BitmapChar
    {
        public char Character { get; }
        public Rectangle? Source { get; }

        public BitmapChar(char character, Rectangle? source)
        {
            Character = character;
            Source = source;
        }
    }
}
