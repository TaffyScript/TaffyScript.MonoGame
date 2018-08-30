using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Path = System.IO.Path;
using TaffyScript.MonoGame.Graphics;
using TaffyScript.MonoGame.Sound;

namespace TaffyScript.MonoGame
{
    [TaffyScriptBaseType]
    public class ContentLoader
    {
        private static Dictionary<string, ContentManager> _managers = new Dictionary<string, ContentManager>();
        private static IServiceProvider _serviceProvider;
        private static Game _game;

        public static void Initialize(Game game)
        {
            _serviceProvider = game.Content.ServiceProvider;
            _game = game;
            //_managers.Add(game.Content.RootDirectory, game.Content);
        }

        public static T Load<T>(string assetName)
        {
            var folder = Path.GetDirectoryName(assetName);
            if (!_managers.TryGetValue(folder, out var manager))
            {
                manager = new ContentManager(_serviceProvider);
                manager.RootDirectory = "";
                _managers.Add(folder, manager);
            }
            return manager.Load<T>(assetName);
        }

        public static T Load<T>(string folder, string assetName)
        {
            if (!_managers.TryGetValue(folder, out var manager))
            {
                manager = new ContentManager(_serviceProvider);
                manager.RootDirectory = "";
                _managers.Add(folder, manager);
            }
            return manager.Load<T>($"{folder}/{assetName}");
        }

        public static void Unload(string folder)
        {
            _managers[folder].Unload();
        }

        [TaffyScriptMethod]
        public static TsObject unload_folder(TsObject[] args)
        {
            _managers[(string)args[0]].Unload();
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject load_texture(TsObject[] args)
        {
            return new TsTexture(args.Length == 2 ? Load<Texture2D>((string)args[0], (string)args[1]) : Load<Texture2D>((string)args[0]));
        }

        [TaffyScriptMethod]
        public static TsObject load_bmp_font_strip(TsObject[] args)
        {
            return BitmapFont.FromStrip(((TsTexture)args[0]).Source, (string)args[1], (int)args[2], (int)args[3], (int)args[4], args.Length > 5 ? (bool)args[5] : false);
        }

        [TaffyScriptMethod]
        public static TsObject load_sprite_font(TsObject[] args)
        {
            return new TsSpriteFont(args.Length == 2 ? Load<SpriteFont>((string)args[0], (string)args[1]) : Load<SpriteFont>((string)args[0]));
        }

        [TaffyScriptMethod]
        public static TsObject load_song(TsObject[] args)
        {
            return new TsSong(Load<Song>((string)args[0]));
        }

        [TaffyScriptMethod]
        public static TsObject load_sound(TsObject[] args)
        {
            return new TsSound(Load<SoundEffect>((string)args[0]));
        }
    }
}
