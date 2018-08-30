using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TaffyScript.MonoGame.Graphics
{
    public enum SpriteUpdateMode
    {
        PingPong,
        Cycle
    }

    [XmlRoot(ElementName = "Sprite")]
    [TaffyScriptObject]
    public class Sprite : IXmlSerializable, ITsInstance
    {
        #region Fields

        private string _state;
        private int _frame = 0;
        private int _ticks = 0;
        private int _frameDirection = 1;

        #endregion

        #region Properties

        public Texture2D Texture { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Dictionary<string, List<Rectangle>> States { get; } = new Dictionary<string, List<Rectangle>>();
        public List<Rectangle> Frames { get; private set; }

        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public Color Color { get; set; } = Color.White;
        public float Rotation { get; set; } = 0;
        public float Speed { get; set; } = 0;
        public SpriteUpdateMode UpdateMode { get; set; } = SpriteUpdateMode.Cycle;
        public string ObjectType => "TaffyScript.MonoGame.Graphics.Sprite";

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public string State
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;

                _state = value;
                Frames = States[value];
                _frame = 0;
                _ticks = 0;
            }
        }

        public int Frame
        {
            get => _frame;
            set
            {
                if (value < 0 || value >= Frames.Count)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _frame = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Should not be called directly. To be used from XmlSerializer
        /// </summary>
        public Sprite()
        {
        }

        public Sprite(string spriteFilePath)
        {
            var settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };
            using(var reader = XmlReader.Create(spriteFilePath, settings))
            {
                ReadXml(reader);
            }
        }

        public Sprite(TsObject[] args)
            : this((string)args[0])
        {
        }

        #endregion

        public void Update()
        {
            if (Speed != -1 && _ticks++ >= Speed)
            {
                var next = _frame + _frameDirection;
                if (next < 0 || next >= Frames.Count)
                {
                    switch (UpdateMode)
                    {
                        case SpriteUpdateMode.Cycle:
                            next = 0;
                            break;
                        case SpriteUpdateMode.PingPong:
                            _frameDirection *= -1;
                            next = _frame + _frameDirection;
                            break;
                    }
                }
                _frame = next;
                _ticks = 0;
            }
        }

        #region TaffyScript

        public TsObject Call(string scriptName, params TsObject[] args)
        {
            switch(scriptName)
            {
                case "draw":
                    return draw(args);
                case "draw_stretched":
                    return draw_stretched(args);
                case "draw_ext":
                    return draw_ext(args);
                case "draw_part":
                    return draw_part(args);
                case "draw_part_stretched":
                    return draw_part_stretched(args);
                case "draw_part_ext":
                    return draw_part_ext(args);
                case "update":
                    return update(args);
                default:
                    throw new MissingMemberException(ObjectType, scriptName);
            }
        }

        public TsDelegate GetDelegate(string delegateName)
        {
            if (TryGetDelegate(delegateName, out var del))
                return del;
            throw new MissingMemberException(ObjectType, delegateName);
        }

        public TsObject GetMember(string name)
        {
            switch(name)
            {
                case "texture":
                    return new TsTexture(Texture);
                case "width":
                    return Width;
                case "height":
                    return Height;
                case "origin":
                    return new TsObject[] { Origin.X, Origin.Y };
                case "scale":
                    return new TsObject[] { Scale.X, Scale.Y };
                case "color":
                    return new TsColor(Color);
                case "rotation":
                    return Rotation;
                case "speed":
                    return Speed;
                case "update_mode":
                    return (float)UpdateMode;
                case "state":
                    return State;
                case "frame":
                    return _frame;
                case "frame_count":
                    return Frames.Count;
                default:
                    if (TryGetDelegate(name, out var del))
                        return del;
                    throw new MissingMemberException(ObjectType, name);
            }
        }

        public void SetMember(string name, TsObject value)
        {
            switch (name)
            {
                case "origin":
                    var array = value.GetArray();
                    Origin = new Vector2((float)array[0], (float)array[1]);
                    break;
                case "scale":
                    array = value.GetArray();
                    Scale = new Vector2((float)array[0], (float)array[2]);
                    break;
                case "color":
                    Color = ((TsColor)value).Source;
                    break;
                case "rotation":
                    Rotation = (float)value;
                    break;
                case "speed":
                    Speed = (float)value;
                    break;
                case "update_mode":
                    UpdateMode = (SpriteUpdateMode)(float)value;
                    break;
                case "state":
                    State = (string)value;
                    break;
                case "frame":
                    Frame = (int)value;
                    break;
                default:
                    throw new MissingMemberException(ObjectType, name);
            }
        }

        public bool TryGetDelegate(string scriptName, out TsDelegate del)
        {
            switch (scriptName)
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
                case "update":
                    del = new TsDelegate(update, "update");
                    return true;
                default:
                    del = null;
                    return false;
            }
        }

        public TsObject draw(params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Texture, new Vector2((float)args[0], (float)args[1]), Frames[Frame], Color, Rotation, Origin, Scale, SpriteEffects.None, 0f);
            return TsObject.Empty;
        }

        public TsObject draw_stretched(params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Texture,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                Frames[Frame],
                                                Color,
                                                Rotation,
                                                Origin,
                                                SpriteEffects.None,
                                                0f);
            return TsObject.Empty;
        }

        public TsObject draw_ext(params TsObject[] args)
        {
            SpriteBatchManager.SpriteBatch.Draw(Texture,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                Frames[Frame],
                                                ((TsColor)args[5]).Source,
                                                (float)args[4],
                                                Origin,
                                                SpriteEffects.None,
                                                0f);
            return TsObject.Empty;
        }

        public TsObject draw_part(params TsObject[] args)
        {
            var frame = Frames[Frame];
            SpriteBatchManager.SpriteBatch.Draw(Texture,
                                                new Vector2((float)args[0], (float)args[1]),
                                                new Rectangle((int)args[2] + frame.X, (int)args[3] + frame.Y, (int)args[4], (int)args[5]),
                                                Color,
                                                Rotation,
                                                Origin,
                                                Scale,
                                                SpriteEffects.None,
                                                0f);
            return TsObject.Empty;
        }

        public TsObject draw_part_stretched(params TsObject[] args)
        {
            var frame = Frames[Frame];
            SpriteBatchManager.SpriteBatch.Draw(Texture,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                new Rectangle((int)args[4] + frame.X, (int)args[5] + frame.Y, (int)args[6], (int)args[7]),
                                                Color,
                                                Rotation,
                                                Origin,
                                                SpriteEffects.None,
                                                0f);
            return TsObject.Empty;
        }

        public TsObject draw_part_ext(params TsObject[] args)
        {
            var frame = Frames[Frame];
            SpriteBatchManager.SpriteBatch.Draw(Texture,
                                                new Rectangle((int)args[0], (int)args[1], (int)args[2], (int)args[3]),
                                                new Rectangle((int)args[4] + frame.X, (int)args[5] + frame.Y, (int)args[6], (int)args[7]),
                                                ((TsColor)args[9]).Source,
                                                (float)args[8],
                                                Origin,
                                                SpriteEffects.None,
                                                0f);
            return TsObject.Empty;
        }

        public TsObject update(params TsObject[] args)
        {
            Update();
            return TsObject.Empty;
        }

        public static implicit operator TsObject(Sprite sprite)
        {
            return new TsInstanceWrapper(sprite);
        }

        public static explicit operator Sprite(TsObject obj)
        {
            return (Sprite)obj.WeakValue;
        }

        #endregion

        #region Xml

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadToFollowing("Texture");

            Texture = ContentLoader.Load<Texture2D>(reader.ReadElementContentAsString());
            Width = reader.ReadElementContentAsInt();
            Height = reader.ReadElementContentAsInt();
            reader.Read();
            Origin = new Vector2(reader.ReadElementContentAsInt(), reader.ReadElementContentAsInt());
            reader.ReadToFollowing("UpdateMode");
            UpdateMode = (SpriteUpdateMode)Enum.Parse(typeof(SpriteUpdateMode), reader.ReadElementContentAsString());
            Speed = reader.ReadElementContentAsInt();
            while (reader.ReadToFollowing("State"))
            {
                var key = reader.GetAttribute("Name");
                var list = new List<Rectangle>();
                if (reader.ReadToDescendant("Frame"))
                {
                    do
                    {
                        var rect = new Rectangle(int.Parse(reader.GetAttribute(0)), int.Parse(reader.GetAttribute(1)), Width, Height);
                        list.Add(rect);
                    }
                    while (reader.ReadToNextSibling("Frame"));
                }
                States.Add(key, list);
                if (Frames is null)
                {
                    _state = key;
                    Frames = list;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Texture");
            writer.WriteValue(Texture.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("Width");
            writer.WriteValue(Width);
            writer.WriteEndElement();

            writer.WriteStartElement("Height");
            writer.WriteValue(Height);
            writer.WriteEndElement();

            writer.WriteStartElement("Origin");

            writer.WriteStartElement("X");
            writer.WriteValue(Origin.X);
            writer.WriteEndElement();

            writer.WriteStartElement("Y");
            writer.WriteValue(Origin.Y);
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.WriteStartElement("UpdateMode");
            writer.WriteValue(UpdateMode.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Speed");
            writer.WriteValue(Speed);
            writer.WriteEndElement();

            writer.WriteStartElement("States");

            foreach (var kvp in States)
            {
                writer.WriteStartElement("State");

                writer.WriteAttributeString("Name", kvp.Key);
                foreach (var frame in kvp.Value)
                {
                    writer.WriteStartElement("Frame");
                    writer.WriteAttributeString("X", frame.X.ToString());
                    writer.WriteAttributeString("Y", frame.Y.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
