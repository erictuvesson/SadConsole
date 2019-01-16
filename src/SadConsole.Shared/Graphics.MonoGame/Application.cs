namespace SadConsole
{
    using Microsoft.Xna.Framework;
    using System;

    public abstract class Application : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        /// <summary>
        /// Indicates the window is going to resize itself.
        /// </summary>
        public bool ResizeBusy = false;

        /// <summary>
        /// Raised when the window is resized and the render area has been calculated.
        /// </summary>
        public event EventHandler WindowResized;

        public int ConsoleWidth { get; private set; }
        public int ConsoleHeight { get; private set; }

        private string font;

        protected Application(string font, int consoleWidth, int consoleHeight)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.GraphicsProfile = Settings.GraphicsProfile;
            Content.RootDirectory = "Content";

            this.font = font;
            this.ConsoleHeight = consoleHeight;
            this.ConsoleWidth = consoleWidth;

            IsMouseVisible = true;

#if MONOGAME
            GraphicsDeviceManager.HardwareModeSwitch = Settings.UseHardwareFullScreen;
#endif
        }

        protected override void UnloadContent()
        {

        }

        protected override void Initialize()
        {
            if (Settings.UnlimitedFPS)
            {
                GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
                IsFixedTimeStep = false;
            }

            // Initialize the SadConsole engine with a font, and a screen size that mirrors MS-DOS.
            Components.Add(new SadConsole.Game.SadConsoleGameComponent(this));

            base.Initialize();

            // Hook window change for resolution fixes
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            Window.AllowUserResizing = SadConsole.Settings.AllowWindowResize;

            Global.GraphicsDevice = GraphicsDevice;
            Global.GraphicsDeviceManager = GraphicsDeviceManager;
            Global.SpriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

            if (string.IsNullOrEmpty(font))
            {
                Global.LoadEmbeddedFont();
            }
            else
            {
                Global.FontDefault = Global.LoadFont(font).GetFont(Font.FontSizes.One);
            }

            Global.FontDefault.ResizeGraphicsDeviceManager(GraphicsDeviceManager, ConsoleWidth, ConsoleHeight, 0, 0);
            Global.ResetRendering();

            Global.CurrentScreen = new ContainerConsole();
            Global.GraphicsDevice.SetRenderTarget(null);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Settings.ClearColor);
            base.Draw(gameTime);
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (!ResizeBusy)
            {
                if (!Global.GraphicsDeviceManager.IsFullScreen && Settings.WindowMinimumSize != Point.Zero)
                {
                    if (GraphicsDeviceManager.PreferredBackBufferWidth < Settings.WindowMinimumSize.X
                        || GraphicsDeviceManager.PreferredBackBufferHeight < Settings.WindowMinimumSize.Y)
                    {
                        ResizeBusy = true;
                        GraphicsDeviceManager.PreferredBackBufferWidth = Global.WindowWidth = Settings.WindowMinimumSize.X;
                        GraphicsDeviceManager.PreferredBackBufferHeight = Global.WindowHeight = Settings.WindowMinimumSize.Y;
                        GraphicsDeviceManager.ApplyChanges();
                    }
                }
            }
            else
            {
                ResizeBusy = false;
            }

            Global.ResetRendering();

            if (!ResizeBusy)
                WindowResized?.Invoke(this, EventArgs.Empty);
        }
    }
}
