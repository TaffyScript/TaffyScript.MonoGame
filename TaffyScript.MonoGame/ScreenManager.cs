using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Myst.Extensions;
using TaffyScript.MonoGame.Graphics;
using TaffyScript.MonoGame.ViewportAdapters;
using Microsoft.Xna.Framework;

namespace TaffyScript.MonoGame
{
    [TaffyScriptBaseType]
    public static class ScreenManager
    {
        private static readonly List<Screen> _screens = new List<Screen>();
        private static Game _game;

        public static Screen CurrentScreen => _screens.Peek();
        public static ViewportAdapter ViewportAdapter { get; set; }

        public static void Initialize(Game game, ViewportAdapter viewport)
        {
            _game = game;
            ViewportAdapter = viewport;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PushScreen(Screen screen)
        {
            AddScreen(screen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TaffyScriptMethod]
        public static TsObject screen_push(TsObject[] args)
        {
            PushScreen((Screen)args[0]);
            return TsObject.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChangeScreen(Screen screen)
        {
            while (_screens.Count > 0)
                RemoveScreen();
            AddScreen(screen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TaffyScriptMethod]
        public static TsObject screen_change(TsObject[] args)
        {
            ChangeScreen((Screen)args[0]);
            return TsObject.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PopScreen()
        {
            if (_screens.Count != 0)
            {
                RemoveScreen();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TaffyScriptMethod]
        public static TsObject screen_pop(TsObject[] args)
        {
            PopScreen();
            return TsObject.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsScreen(Screen screen)
        {
            return _screens.Contains(screen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TaffyScriptMethod]
        public static TsObject screen_exists(TsObject[] args)
        {
            return ContainsScreen((Screen)args[0]);
        }

        public static void Update(GameTime gameTime)
        {
            if (_screens.Count > 0)
                _screens.Peek().Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            _game.GraphicsDevice.Clear(SpriteBatchManager.BackgroundColor);

            if (_screens.Count > 0)
                _screens.Peek().Draw();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddScreen(Screen screen)
        {
            _screens.Add(screen);
            TsInstance.Global["screen"] = screen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RemoveScreen()
        {
            _screens.Pop();
            TsInstance.Global["screen"] = _screens.Count > 0 ? _screens.Peek() : TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject gui_get_width(TsObject[] args)
        {
            return ViewportAdapter.VirtualWidth;
        }

        [TaffyScriptMethod]
        public static TsObject gui_get_height(TsObject[] args)
        {
            return ViewportAdapter.VirtualHeight;
        }

        [TaffyScriptMethod]
        public static TsObject gui_mouse_pos(TsObject[] args)
        {
            return new TsObject[] { Input.InputManager.MousePosition.X, Input.InputManager.MousePosition.Y };
        }

        [TaffyScriptMethod]
        public static TsObject gui_mouse_x(TsObject[] args)
        {
            return Input.InputManager.MousePosition.X;
        }

        [TaffyScriptMethod]
        public static TsObject gui_mouse_y(TsObject[] args)
        {
            return Input.InputManager.MousePosition.Y;
        }

        [TaffyScriptMethod]
        public static TsObject game_end(TsObject[] args)
        {
            _game.Exit();
            return TsObject.Empty;
        }
    }
}
