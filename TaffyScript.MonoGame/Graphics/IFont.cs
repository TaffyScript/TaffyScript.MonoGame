using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    public interface IFont
    {
        int LineSpacing { get; }
        Vector2 MeasureString(string text);
        Vector2 MeasureString(string text, int start, int count);
        Vector2 MeasureString(StringBuilder text);
        Vector2 MeasureString(StringBuilder text, int start, int count);
        void DrawString(string text, Vector2 position, Color color);
        void DrawString(string text, Vector2 position, int start, int count, Color color);
        void DrawString(StringBuilder text, Vector2 position, Color color);
        void DrawString(StringBuilder text, Vector2 position, int start, int count, Color color);
    }

    public abstract class Font : IFont, ITsInstance
    {
        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public string ObjectType => "TaffyScript.MonoGame.Graphics.Font";
        public abstract int LineSpacing { get; }

        public TsObject Call(string scriptName, params TsObject[] args)
        {
            switch(scriptName)
            {
                case "draw_string":
                    return draw_string(args);
                case "draw_string_color":
                    return draw_string_color(args);
                case "draw_string_part":
                    return draw_string_part(args);
                case "draw_string_part_color":
                    return draw_string_part_color(args);
                case "measure_string":
                    return measure_string(args);
                case "measure_string_width":
                    return measure_string_width(args);
                case "measure_string_height":
                    return measure_string_height(args);
                case "measure_string_part":
                    return measure_string_part(args);
                case "measure_string_part_width":
                    return measure_string_part_width(args);
                case "measure_string_part_height":
                    return measure_string_part_height(args);
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
            switch (name)
            {
                case "line_spacing":
                    return LineSpacing;
                default:
                    if (TryGetDelegate(name, out var del))
                        return del;
                    throw new MissingFieldException(ObjectType, name);
            }
        }

        public void SetMember(string name, TsObject value)
        {
            throw new MissingFieldException(ObjectType, name);
        }

        public bool TryGetDelegate(string delegateName, out TsDelegate del)
        {
            switch (delegateName)
            {
                case "draw_string":
                    del = new TsDelegate(draw_string, "draw_string");
                    return true;
                case "draw_string_color":
                    del = new TsDelegate(draw_string_color, "draw_string_color");
                    return true;
                case "draw_string_part":
                    del = new TsDelegate(draw_string_part, "draw_string_part");
                    return true;
                case "draw_string_part_color":
                    del = new TsDelegate(draw_string_part_color, "draw_string_part_color");
                    return true;
                case "measure_string":
                    del = new TsDelegate(measure_string, "measure_string");
                    return true;
                case "measure_string_width":
                    del = new TsDelegate(measure_string_width, "measure_string_width");
                    return true;
                case "measure_string_height":
                    del = new TsDelegate(measure_string_height, "measure_string_height");
                    return true;
                case "measure_string_part":
                    del = new TsDelegate(measure_string_part, "measure_string_part");
                    return true;
                case "measure_string_part_width":
                    del = new TsDelegate(measure_string_part_width, "measure_string_part_width");
                    return true;
                case "measure_string_part_height":
                    del = new TsDelegate(measure_string_part_height, "measure_string_part_height");
                    return true;
                default:
                    del = null;
                    return false;
            }
        }

        private TsObject draw_string(TsObject[] args)
        {
            DrawString((string)args[0], new Vector2((float)args[1], (float)args[2]), SpriteBatchManager.DrawColor);
            return TsObject.Empty;
        }

        private TsObject draw_string_color(TsObject[] args)
        {
            DrawString((string)args[0], new Vector2((float)args[1], (float)args[2]), ((TsColor)args[3]).Source);
            return TsObject.Empty;
        }

        private TsObject draw_string_part(TsObject[] args)
        {
            DrawString((string)args[0], new Vector2((float)args[1], (float)args[2]), (int)args[3], (int)args[4], SpriteBatchManager.DrawColor);
            return TsObject.Empty;
        }

        private TsObject draw_string_part_color(TsObject[] args)
        {
            DrawString((string)args[0], new Vector2((float)args[1], (float)args[2]), (int)args[3], (int)args[4], ((TsColor)args[5]).Source);
            return TsObject.Empty;
        }

        private TsObject measure_string(TsObject[] args)
        {
            var size = MeasureString((string)args[0]);
            return new TsObject[] { size.X, size.Y };
        }

        private TsObject measure_string_width(TsObject[] args)
        {
            return MeasureString((string)args[0]).X;
        }

        private TsObject measure_string_height(TsObject[] args)
        {
            return MeasureString((string)args[0]).Y;
        }

        private TsObject measure_string_part(TsObject[] args)
        {
            var size = MeasureString((string)args[0], (int)args[1], (int)args[2]);
            return new TsObject[] { size.X, size.Y };
        }

        private TsObject measure_string_part_width(TsObject[] args)
        {
            return MeasureString((string)args[0], (int)args[1], (int)args[2]).X;
        }

        private TsObject measure_string_part_height(TsObject[] args)
        {
            return MeasureString((string)args[0], (int)args[1], (int)args[2]).Y;
        }

        public static implicit operator TsObject(Font font)
        {
            return new TsInstanceWrapper(font);
        }

        public static explicit operator Font(TsObject obj)
        {
            return (Font)obj.WeakValue;
        }

        public abstract Vector2 MeasureString(string text);
        public abstract Vector2 MeasureString(string text, int start, int count);
        public abstract Vector2 MeasureString(StringBuilder text);
        public abstract Vector2 MeasureString(StringBuilder text, int start, int count);
        public abstract void DrawString(string text, Vector2 position, Color color);
        public abstract void DrawString(string text, Vector2 position, int start, int count, Color color);
        public abstract void DrawString(StringBuilder text, Vector2 position, Color color);
        public abstract void DrawString(StringBuilder text, Vector2 position, int start, int count, Color color);
    }
}
