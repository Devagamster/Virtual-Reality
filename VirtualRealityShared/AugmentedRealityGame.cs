using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualReality
{
    public abstract class AugmentedRealityGame
    {
        public virtual void Load(ContentManager content) { }
        public virtual void Start() { }
        public virtual void Reset() { }
        public virtual Vector3 Update(GameTime gameTime, Vector3 cameraOrientation, bool cameraHalfClicked) { return Vector3.Zero; }
        public virtual Color Draw(GameTime gameTime) { return Color.Black; }
        public abstract void DrawIdentifier(GameTime gameTime, Vector3 centerPosition, bool hovering, float scaleAmount);
    }
}
