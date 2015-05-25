using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualReality
{
    public class MainMenu : AugmentedRealityGame
    {
        const float DRAW_DISTANCE = 2f;
        const float SCALE_AMOUNT = 50;
        const float WALL_DISTANCE = 3f;

        Dictionary<float, AugmentedRealityGame> gameAngles = new Dictionary<float, AugmentedRealityGame>();
        float angleSeperation;
        float selectedAngle;

        private Texture2D wallTexture;

        public MainMenu()
        {
            var gameCount = Game.Games.Count;
            angleSeperation = MathHelper.TwoPi / gameCount;
            var currentAngle = MathHelper.PiOver2;
            foreach (var game in Game.Games)
            {
                gameAngles[currentAngle] = game;
                currentAngle += angleSeperation;
            }
        }

        public override void Load(ContentManager content)
        {
            wallTexture = content.Load<Texture2D>("Debug");
        }

        public override Vector3 Update(GameTime gameTime, Vector3 cameraOrientation, bool cameraHalfClicked)
        {
            var cameraAngle = Math.Atan2(-cameraOrientation.Z, cameraOrientation.X);
            foreach (var angle in gameAngles.Keys)
            {
                var angleDifference = (float)Math.Abs(angle - cameraAngle);
                if (angleDifference > MathHelper.Pi) angleDifference = MathHelper.TwoPi - angleDifference;
                if (angleDifference < angleSeperation / 4)
                {
                    selectedAngle = angle;
                }
            }

            return Vector3.Zero;
        }

        public override Color Draw(GameTime gameTime)
        {
            foreach (var angle in gameAngles.Keys)
            {
                var center = new Vector3((float)Math.Cos(angle) * DRAW_DISTANCE, 0, -(float)Math.Sin(angle) * DRAW_DISTANCE);
                var game = gameAngles[angle];
                var hovering = angle == selectedAngle;
                game.DrawIdentifier(gameTime, center, hovering, SCALE_AMOUNT);
            }

            Game.VertexManager.AddRectangle(wallTexture, Color.White,
                new Vector3(-WALL_DISTANCE, WALL_DISTANCE, -WALL_DISTANCE), new Vector3(WALL_DISTANCE, WALL_DISTANCE, -WALL_DISTANCE),
                new Vector3(WALL_DISTANCE, -WALL_DISTANCE, -WALL_DISTANCE), new Vector3(-WALL_DISTANCE, -WALL_DISTANCE, -WALL_DISTANCE));

            return Color.Black;
        }

        public override void DrawIdentifier(GameTime gameTime, Vector3 centerPosition, bool hovering, float scaleAmount)
        {
        }
    }
}
