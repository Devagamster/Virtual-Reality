using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace VirtualReality
{
    public class HillAssault : AugmentedRealityGame
    {
        public const float HILL_RADIUS_INCREASE = 1f;
        public const float HILL_DEPTH_INCREASE = 0.5f;
        public const float HILL_CIRCLE_COUNT = 20;
        public const float INITIAL_HILL_RADIUS = 1;
        public const float BADDIE_SPAWN_POINT = 20f;

        List<HillAssaultBaddie> baddies = new List<HillAssaultBaddie>();

        Texture2D grassTexture;
        Texture2D baddieTexture;
        public override void Load(ContentManager content)
        {
            grassTexture = content.Load<Texture2D>("Grass");
            baddieTexture = content.Load<Texture2D>("VrBaddie");

            idBaddies = new List<HillAssaultBaddie>();
            for (int i = 0; i < 10; i++)
            {
                idBaddies.Add(new HillAssaultBaddie(baddieTexture, BADDIE_SPAWN_POINT*0.5f));
            }
        }

        public override void Start()
        {
            for (var i = 0; i < 2; i++)
            {
                baddies.Add(new HillAssaultBaddie(baddieTexture, BADDIE_SPAWN_POINT));
            }
        }

        public override void Reset()
        {
            baddies.Clear();
        }

        public override Vector3 Update(GameTime gameTime, Vector3 cameraOrientation, bool cameraHalfClicked)
        {
            foreach (var baddie in baddies.ToList())
            {
                if (baddie.Move(cameraOrientation, cameraHalfClicked))
                {
                    baddies.Remove(baddie);
                }
            }

            var spawn = Game.Rand.Next(4500);
            if (spawn == 1)
            {
                baddies.Add(new HillAssaultBaddie(baddieTexture, BADDIE_SPAWN_POINT));
            }

            return new Vector3(0, 2, 0);
        }

        public override Color Draw(GameTime gameTime)
        {
            var torchFlicker = new Color(Game.Rand.Next(10) + 56, Game.Rand.Next(10) + 20, 12);
            for (int i = 0; i < HILL_CIRCLE_COUNT; i ++)
            {
                var radius = INITIAL_HILL_RADIUS + (float)i * HILL_RADIUS_INCREASE;
                var depth = -i * HILL_DEPTH_INCREASE;
                var modifiedTorchFlicker = torchFlicker * (1 - (float)i / HILL_CIRCLE_COUNT);
                AddGroundCircle(Vector3.Zero, radius, depth, modifiedTorchFlicker);
            }

            foreach (var baddie in baddies)
            {
                DrawBaddie(gameTime, baddie, torchFlicker);
            }

            return new Color(0, 0, 0);
        }

        private void DrawBaddie(GameTime gameTime, HillAssaultBaddie baddie, Color torchFlicker)
        {
            var translationX = (float)Math.Cos(baddie.AngleOfApproach) * baddie.Distance;
            var translationZ = (float)Math.Sin(baddie.AngleOfApproach) * baddie.Distance;
            var translationY = -baddie.Distance * HILL_DEPTH_INCREASE +
                (float)Math.Sin((gameTime.TotalGameTime.TotalMilliseconds + baddie.TimeOffSet) / 5000f)* 
                HillAssaultBaddie.HOVER_AMOUNT + 0.5f;

            var modifiedTorchFlicker = torchFlicker * (1 - baddie.Distance / HILL_CIRCLE_COUNT);

            var translationVector = new Vector3(translationX, translationY, translationZ);
            Game.VertexManager.AddRectangle(baddieTexture, modifiedTorchFlicker,
                baddie.BaseTopLeft + translationVector, baddie.BaseTopRight + translationVector,
                baddie.BaseBottomRight + translationVector, baddie.BaseBottomLeft + translationVector);
        }

        private void AddGroundCircle(Vector3 center, float radius, float depth, Color color)
        {
            var worldList = new List<Vector3>();
            var textureList = new List<Vector2>();
            for (float theta = 0; theta < MathHelper.TwoPi; theta += MathHelper.Pi / 16)
            {
                var normalX = (float)Math.Cos(theta);
                var normalZ = (float)Math.Sin(theta);

                worldList.Add(new Vector3(center.X + normalX * radius, center.Y + depth, center.Z + normalZ * radius));
                textureList.Add(new Vector2(normalX * radius, normalZ * radius));
            }

            Game.VertexManager.AddPolygon(grassTexture, color, worldList.Count, worldList, textureList);
        }

        List<HillAssaultBaddie> idBaddies;
        public override void DrawIdentifier(GameTime gameTime, Vector3 centerPosition, bool hovering, float scaleAmount)
        {
            var torchFlicker = new Color(Game.Rand.Next(10) + 56, Game.Rand.Next(10) + 20, 12) * 4;
            if (!hovering)
            {
                torchFlicker *= 0.5f;
            }
            for (var i = 0; i < 20; i++)
            {
                var radius = (INITIAL_HILL_RADIUS / scaleAmount) + i * HILL_RADIUS_INCREASE / scaleAmount;
                var depth = -i * HILL_DEPTH_INCREASE / scaleAmount;
                var modifiedTorchFlicker = torchFlicker * (1 - i / HILL_CIRCLE_COUNT);
                AddGroundCircle(centerPosition, radius, depth, modifiedTorchFlicker);
            }

            foreach (var baddie in idBaddies)
            {

                var translationX = (float)Math.Cos(baddie.AngleOfApproach) * baddie.Distance / scaleAmount + centerPosition.X;
                var translationZ = (float)Math.Sin(baddie.AngleOfApproach) * baddie.Distance / scaleAmount + centerPosition.Z;
                var translationY = -baddie.Distance * HILL_DEPTH_INCREASE / scaleAmount +
                    (float)Math.Sin((gameTime.TotalGameTime.TotalMilliseconds + baddie.TimeOffSet) / 5000f) *
                    HillAssaultBaddie.HOVER_AMOUNT / scaleAmount + 0.5f / scaleAmount + centerPosition.Y;

                var modifiedTorchFlicker = torchFlicker * (1 - baddie.Distance / HILL_CIRCLE_COUNT);

                var translationVector = new Vector3(translationX, translationY, translationZ);
                Game.VertexManager.AddRectangle(baddieTexture, modifiedTorchFlicker,
                    baddie.BaseTopLeft * 1.5f / scaleAmount + translationVector, baddie.BaseTopRight * 1.5f / scaleAmount + translationVector,
                    baddie.BaseBottomRight * 1.5f / scaleAmount + translationVector, baddie.BaseBottomLeft * 1.5f / scaleAmount + translationVector);
            }
        }
    }
}
