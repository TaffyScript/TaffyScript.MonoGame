using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Myst.Extensions;
using TaffyScript.MonoGame.ViewportAdapters;
using Microsoft.Xna.Framework;

namespace TaffyScript.MonoGame
{
    [WeakBaseType]
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
        [WeakMethod]
        public static TsObject screen_push(ITsInstance inst, TsObject[] args)
        {
            PushScreen((Screen)args[0]);
            return TsObject.Empty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChangeScreen(Screen screen)
        {
            while (_screens.Count > 0)
                RemoveScreen();
            AddScreen(screen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [WeakMethod]
        public static TsObject screen_change(ITsInstance inst, TsObject[] args)
        {
            ChangeScreen((Screen)args[0]);
            return TsObject.Empty();
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
        [WeakMethod]
        public static TsObject screen_pop(ITsInstance inst, TsObject[] args)
        {
            PopScreen();
            return TsObject.Empty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsScreen(Screen screen)
        {
            return _screens.Contains(screen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [WeakMethod]
        public static TsObject screen_exists(ITsInstance inst, TsObject[] args)
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
            TsInstance.Global["screen"] = _screens.Count > 0 ? _screens.Peek() : TsObject.Empty();
        }

        [WeakMethod]
        public static TsObject gui_get_width(ITsInstance inst, TsObject[] args)
        {
            return ViewportAdapter.VirtualWidth;
        }

        [WeakMethod]
        public static TsObject gui_get_height(ITsInstance inst, TsObject[] args)
        {
            return ViewportAdapter.VirtualHeight;
        }

        [WeakMethod]
        public static TsObject gui_mouse_pos(ITsInstance inst, TsObject[] args)
        {
            return new TsObject[] { Input.InputManager.MousePosition.X, Input.InputManager.MousePosition.Y };
        }

        [WeakMethod]
        public static TsObject gui_mouse_x(ITsInstance inst, TsObject[] args)
        {
            return Input.InputManager.MousePosition.X;
        }

        [WeakMethod]
        public static TsObject gui_mouse_y(ITsInstance inst, TsObject[] args)
        {
            return Input.InputManager.MousePosition.Y;
        }

        [WeakMethod]
        public static TsObject game_end(ITsInstance inst, TsObject[] args)
        {
            _game.Exit();
            return TsObject.Empty();
        }
    }
}
