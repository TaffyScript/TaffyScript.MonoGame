using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.Graphics
{
    public static class SpriteBatchManager
    {
        private static GraphicsDevice _graphicsDevice;
        private static BlendState _blend;
        private static SpriteSortMode _sortMode;
        private static SamplerState _samplerState = SamplerState.PointClamp;
        private static DepthStencilState _depthStencilState;
        private static RasterizerState _rasterizerState;
        private static Effect _effect;
        private static Matrix? _transformMatrix;
        private static Matrix? _savedTransform;
        private static Viewport? _savedViewport;

        public static bool Drawing { get; private set; }
        public static Color DrawColor { get; set; } = Color.White;
        public static SpriteBatch SpriteBatch { get; private set; }

        public static BlendState Blend
        {
            get => _blend;
            set
            {
                _blend = value;
                ApplySettings();
            }
        }

        public static SpriteSortMode SortMode
        {
            get => _sortMode;
            set
            {
                _sortMode = value;
                ApplySettings();
            }
        }

        public static SamplerState SamplerState
        {
            get => _samplerState;
            set
            {
                _samplerState = value;
                ApplySettings();
            }
        }

        public static DepthStencilState DepthStencilState
        {
            get => _depthStencilState;
            set
            {
                _depthStencilState = value;
                ApplySettings();
            }
        }

        public static RasterizerState RasterizerState
        {
            get => _rasterizerState;
            set
            {
                _rasterizerState = value;
                ApplySettings();
            }
        }

        public static Effect Effect
        {
            get => _effect;
            set
            {
                _effect = value;
                ApplySettings();
            }
        }

        public static Matrix? TransformMatrix
        {
            get => _transformMatrix;
            set
            {
                _transformMatrix = value;
                ApplySettings();
            }
        }

        public static void Initialize(Game game)
        {
            _graphicsDevice = game.GraphicsDevice;
            SpriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public static void Begin()
        {
            SpriteBatch.Begin(SortMode, Blend, SamplerState, DepthStencilState, RasterizerState, Effect, TransformMatrix);
            Drawing = true;
        }

        public static void End()
        {
            Drawing = false;
            SpriteBatch.End();
        }

        public static void SetRenderTarget(RenderTarget2D renderTarget)
        {
            var state = Drawing;

            if (Drawing)
                End();

            if (renderTarget == null && _savedTransform != null)
            {
                _transformMatrix = _savedTransform;
                _graphicsDevice.Viewport = _savedViewport.Value;
                _savedViewport = null;
                _savedTransform = null;
            }
            else
            {
                _savedViewport = _graphicsDevice.Viewport;
                _savedTransform = _transformMatrix;
                _transformMatrix = null;
            }

            SpriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);

            if (state)
                Begin();
        }

        private static void ApplySettings()
        {
            if (Drawing)
            {
                End();
                Begin();
            }
        }
    }

    /// <summary>
    /// Wraps a draw begin/end call in an IDisposable interface to be used within a using block.
    /// <para>
    /// Should be used when drawing outside of a Draw method or when the draw state is unknown.
    /// </para>
    /// </summary>
    public struct DrawBlock : IDisposable
    {
        private bool _wasDrawing;
        private bool _disposed;

        public static DrawBlock Create()
        {
            var block = new DrawBlock()
            {
                _wasDrawing = SpriteBatchManager.Drawing
            };
            if (!SpriteBatchManager.Drawing)
                SpriteBatchManager.Begin();

            return block;
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (!_wasDrawing)
                    SpriteBatchManager.End();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
