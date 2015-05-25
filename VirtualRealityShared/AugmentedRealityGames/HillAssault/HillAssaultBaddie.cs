using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualReality
{
    public class HillAssaultBaddie
    {
        public const float RETREAT_SPEED = 0.05f;
        public const float MOVEMENT_SPEED = 2f;
        public const float HOVER_AMOUNT = 0.25f;
        public const float HEIGHT = 3f;
        public const float KNIFE_RANGE = 2f;

        public int TimeOffSet { get; set; }
        public float AngleOfApproach { get; set; }
        public float Distance { get; set; }
        public bool Retreating { get; set; }

        public Vector3 BaseTopLeft { get; set; }
        public Vector3 BaseTopRight { get; set; }
        public Vector3 BaseBottomRight { get; set; }
        public Vector3 BaseBottomLeft { get; set; }

        public HillAssaultBaddie(Texture2D baddieTexture, float distance)
        {
            AngleOfApproach = (float)(Game.Rand.NextDouble() * MathHelper.TwoPi);
            TimeOffSet = Game.Rand.Next(1000000);
            Distance = distance;

            var height = HEIGHT;
            var width = (int)((float)baddieTexture.Width * height / (float)baddieTexture.Height);

            var leftX = (float)Math.Cos(AngleOfApproach - MathHelper.PiOver2) * width / 2;
            var leftZ = (float)Math.Sin(AngleOfApproach - MathHelper.PiOver2) * width / 2;
            var rightX = -leftX;
            var rightZ = -leftZ;

            BaseTopLeft = new Vector3(leftX, height, leftZ);
            BaseTopRight = new Vector3(rightX, height, rightZ);
            BaseBottomRight = new Vector3(rightX, 0, rightZ);
            BaseBottomLeft = new Vector3(leftX, 0, leftZ);
        }

        public bool Move(Vector3 cameraOrientation, bool cutting)
        {
            if (!Retreating)
            {
                var cameraAngle = Math.Atan2(cameraOrientation.Z, cameraOrientation.X);
                var angleDifference = (float)Math.Abs(AngleOfApproach - cameraAngle);
                if (angleDifference > MathHelper.Pi) angleDifference = MathHelper.TwoPi - angleDifference;
                if (angleDifference > MathHelper.Pi / 3)
                {
                    var advance = Game.Rand.Next(200);
                    if (advance == 1)
                    {
                        if (Distance >= MOVEMENT_SPEED * 1.5f)
                        {
                            Distance -= (float)Game.Rand.NextDouble() * MOVEMENT_SPEED;
                        }
                    }
                }
                else
                {
                    if (cutting)
                    {
                        if (Distance < MOVEMENT_SPEED * 2f)
                        {
                            Retreating = true;
                        }
                    }
                }
                return false;
            }
            else
            {
                Distance += RETREAT_SPEED;
                if (Distance > 50)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
