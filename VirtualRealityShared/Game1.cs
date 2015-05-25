using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Net;
using System.Linq;
using HardwareWrapper;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;

namespace VirtualReality
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager Graphics { get; set; }
        public static SpriteBatch SpriteBatch { get; set;}

        RenderTarget2D leftEye;
        RenderTarget2D rightEye;

        public static VertexManager VertexManager { get; set; }
        public static VertexRenderer VertexRenderer { get; set; }

        Quaternion baseRotation = Quaternion.Identity;
        Quaternion currentRotation;

        Vector3 cameraUp;
        Vector3 cameraOrientation;
        Vector3 cameraPosition;

        HardwareManager hardwareManager;
        MadgwickAHRS madgwick;

        Effect oculusEffect;

        MainMenu menu;
        public static List<AugmentedRealityGame> Games { get; set; }
        public static AugmentedRealityGame CurrentGame { get; set; }
        public static Random Rand { get; set; }

        public Game()
        {
            Rand = new Random();

            Graphics = new GraphicsDeviceManager(this); 
            Content.RootDirectory = "Content";

            madgwick = new MadgwickAHRS(1f/60f, 0.01f);

            hardwareManager = new HardwareManager(() => 
            {
                madgwick.Update(
                    hardwareManager.GyroX, -hardwareManager.GyroY, -hardwareManager.GyroZ,
                    hardwareManager.AccelX, -hardwareManager.AccelY, -hardwareManager.AccelZ,
                    hardwareManager.CompX, -hardwareManager.CompY, -hardwareManager.CompZ);
            });

            Games = new List<AugmentedRealityGame>();
            Games.Add(new HillAssault());
            Games.Add(new Maze());
            menu = new MainMenu();

            CurrentGame = menu;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Content.RootDirectory = "Content/CompiledContent";

            oculusEffect = Content.Load<Effect>("OculusRift");
            oculusEffect.Parameters["factor"].SetValue(0.8f);
            oculusEffect.Parameters["xCenter"].SetValue(0.5f);
            oculusEffect.Parameters["yCenter"].SetValue(0.5f);

            VertexManager = new VertexManager();
            VertexRenderer = new VertexRenderer(GraphicsDevice);

            leftEye = new RenderTarget2D(GraphicsDevice, 700, 700, false, SurfaceFormat.Color, DepthFormat.Depth24);
            rightEye = new RenderTarget2D(GraphicsDevice, 700, 700, false, SurfaceFormat.Color, DepthFormat.Depth24);

            foreach (var game in Games)
            {
                game.Load(Content);
            }
            menu.Load(Content);

            CurrentGame.Start();
        }

        bool touching = false;
        protected override void Update(GameTime gameTime)
        {
            if (hardwareManager.SecondStop)
            {
                baseRotation = currentRotation;
            }

            currentRotation = new Quaternion(madgwick.Quaternion[2], madgwick.Quaternion[1], -madgwick.Quaternion[3], madgwick.Quaternion[0]);

            var matrix = Matrix.CreateFromQuaternion(currentRotation) * Matrix.Invert(Matrix.CreateFromQuaternion(baseRotation));
            cameraOrientation = Vector3.Transform(new Vector3(0, 0, -1), matrix);
            cameraUp = Vector3.Transform(new Vector3(0, 1, 0), matrix);

            cameraPosition = CurrentGame.Update(gameTime, cameraOrientation, hardwareManager.FirstStop);

            if (TouchPanel.GetState().Count > 0)
            {
                if (!touching)
                {
                    CurrentGame.Reset();
                    var index = Games.IndexOf(CurrentGame);
                    index++;
                    if (index >= Games.Count)
                    {
                        index = 0;
                    }
                    CurrentGame = Games[index];
                    CurrentGame.Start();
                }
                touching = true;
            }
            else
            {
                touching = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Matrix view;
            Matrix projection;
            var eyeTranslation = Vector3.Normalize(Vector3.Cross(cameraOrientation, cameraUp)) * 0.06f;
            var eyeTarget = cameraPosition + cameraOrientation;

            var backgroundColor = CurrentGame.Draw(gameTime);

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 3, 1, 0.1f, 600);

            GraphicsDevice.SetRenderTarget(leftEye);
            GraphicsDevice.Clear(backgroundColor);
            // Draw left Eye
            view = Matrix.CreateLookAt(cameraPosition - eyeTranslation, eyeTarget - eyeTranslation, cameraUp);
            VertexRenderer.Draw(view, projection, VertexManager);

            GraphicsDevice.SetRenderTarget(rightEye);
            GraphicsDevice.Clear(backgroundColor);
            // Draw right Eye
            view = Matrix.CreateLookAt(cameraPosition + eyeTranslation, eyeTarget + eyeTranslation, cameraUp);
            VertexRenderer.Draw(view, projection, VertexManager);

            VertexManager.Clear();

            // Draw transformed textures
            GraphicsDevice.SetRenderTarget(null);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, null);
            oculusEffect.Techniques[0].Passes[0].Apply();
            SpriteBatch.Draw(leftEye, new Rectangle(
                (int)(((float)GraphicsDevice.Viewport.Width / 4f) - 350f) + 10,
                (int)(((float)GraphicsDevice.Viewport.Height / 2f) - 350f) + 25,
                700, 700), Color.White);
            SpriteBatch.End();

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, null);
            oculusEffect.Techniques[0].Passes[0].Apply();
            SpriteBatch.Draw(rightEye, new Rectangle(
                (int)(((float)GraphicsDevice.Viewport.Width * 3f / 4f) - 350f) - 45,
                (int)(((float)GraphicsDevice.Viewport.Height / 2f) - 350f),
                700, 700), Color.White);
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

