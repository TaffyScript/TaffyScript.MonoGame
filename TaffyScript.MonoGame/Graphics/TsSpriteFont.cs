using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    public class TsSpriteFont : Font
    {
        private SpriteFont _font;

        public override int LineSpacing => _font.LineSpacing;

        public TsSpriteFont(SpriteFont font)
        {
            _font = font;
        }

        public override void DrawString(string text, Vector2 position, Color color)
        {
            SpriteBatchManager.SpriteBatch.DrawString(_font, text, position, color);
        }

        public override void DrawString(string text, Vector2 position, int start, int count, Color color)
        {
            SpriteBatchManager.SpriteBatch.DrawString(_font, text.Substring(start, count), position, color);
        }

        public override void DrawString(StringBuilder text, Vector2 position, Color color)
        {
            SpriteBatchManager.SpriteBatch.DrawString(_font, text, position, color);
        }

        public override void DrawString(StringBuilder text, Vector2 position, int start, int count, Color color)
        {
            var buffer = new char[count];
            text.CopyTo(start, buffer, 0, count);
            SpriteBatchManager.SpriteBatch.DrawString(_font, new string(buffer), position, color);
        }

        public override Vector2 MeasureString(string text)
        {
            return _font.MeasureString(text);
        }

        public override Vector2 MeasureString(string text, int start, int count)
        {
            return _font.MeasureString(text.Substring(start, count));
        }

        public override Vector2 MeasureString(StringBuilder text)
        {
            return _font.MeasureString(text);
        }

        public override Vector2 MeasureString(StringBuilder text, int start, int count)
        {
            var buffer = new char[count];
            text.CopyTo(start, buffer, 0, count);
            return _font.MeasureString(new string(buffer));
        }

        public static explicit operator TsSpriteFont(TsObject obj)
        {
            return (TsSpriteFont)obj.Value.WeakValue;
        }
    }
}
