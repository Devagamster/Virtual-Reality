using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualReality
{
    public class VertexRenderer
    {
        GraphicsDevice graphics;
        BasicEffect basicEffect;

        public VertexRenderer(GraphicsDevice graphics)
        {
            this.graphics = graphics;
            this.basicEffect = new BasicEffect(graphics);
        }

        public void Draw(Matrix view, Matrix projection, VertexManager vertexManager)
        {
            basicEffect.VertexColorEnabled = true;
            basicEffect.TextureEnabled = true;
            basicEffect.World = Matrix.Identity;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            graphics.BlendState = BlendState.AlphaBlend;
            graphics.BlendState.AlphaSourceBlend = Blend.SourceAlpha;
            graphics.RasterizerState = RasterizerState.CullCounterClockwise;
            graphics.SamplerStates[0] = SamplerState.PointWrap;
            graphics.DepthStencilState = DepthStencilState.Default; 

            foreach (var texture in vertexManager.TextureOrder)
            {
                var manager = vertexManager.Managers[texture];
                basicEffect.Texture = texture;
                if (manager.VertexCount > 0)
                {
                    foreach (var pass in basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        graphics.DrawUserIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            manager.Vertices,
                            0,
                            manager.VertexCount,
                            manager.Indices,
                            0,
                            manager.IndexCount / 3);
                    }
                }
            }

            basicEffect.TextureEnabled = false;
        }
    }
}
