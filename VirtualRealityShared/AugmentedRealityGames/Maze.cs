using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualReality
{
    public class Maze : AugmentedRealityGame
    {
        const float TILE_SIZE = 5;

        readonly int[,] MAP = new int[,] {
            { 5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5 },
            { 5, -1, -1, -1,  5, -1, -1, -1,  2, -1, -1, -1, -1, -1, -1,  5 },
            { 5, -1, -2, -1,  5, -1,  0, -1,  2, -1,  4,  4,  4,  4, -1,  5 },
            { 5, -1, -1, -1,  5, -1, -1, -1,  2, -1,  4, -1, -1,  4, -1,  5 },
            { 5,  5, -1,  5,  5, -1,  2,  2,  2, -1,  4, -1, -1,  4, -1,  5 },
            { 5, -1, -1,  1, -1, -1, -1, -1,  4, -1,  4,  4, -1,  4, -1,  5 },
            { 5, -1,  1,  1, -1, -1, -1, -1,  4, -1, -1, -1, -1,  4, -1,  5 },
            { 5, -1,  1, -1, -1, -1, -1, -1,  4,  4,  4,  4,  4,  4, -1,  5 },
            { 5, -1,  1, -1, -1,  3, -1,  3, -1,  3, -1, -1, -1, -1, -1,  5 },
            { 5, -1,  1, -1, -1, -1, -1, -1, -1, -1, -1,  4,  4,  4,  4,  5 },
            { 5, -1,  1, -1, -1,  3, -1,  3, -1,  3, -1, -1, -1, -1, -1,  5 },
            { 5, -1,  1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  5 },
            { 5, -1,  1, -1, -1,  3, -1,  3, -1,  3, -1, -1, -1,  0, -1,  5 },
            { 5, -1,  1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  5 },
            { 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  5 },
            { 5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5 }
        };

        Dictionary<int, Texture2D> wallTypes = new Dictionary<int, Texture2D>();
        Texture2D boards;
        Texture2D cobbleStone;

        private Vector2 playerPosition; 

        public override void Load(ContentManager content)
        {
            wallTypes[0] = content.Load<Texture2D>("MazeBanner");
            wallTypes[1] = content.Load<Texture2D>("MazeBlueStone");
            wallTypes[2] = content.Load<Texture2D>("MazeBrick");
            wallTypes[3] = content.Load<Texture2D>("MazeMossyStone");
            wallTypes[4] = content.Load<Texture2D>("MazeStone");
            wallTypes[5] = content.Load<Texture2D>("MazeVines");

            boards = content.Load<Texture2D>("MazeBoards");
            cobbleStone = content.Load<Texture2D>("MazeCobbleStone");
        }

        public override void Start()
        {
            for (int x = 0; x < MAP.GetLength(0); x++)
            {
                for (int y = 0; y < MAP.GetLength(1); y++)
                {
                    if (MAP[x, y] == -2)
                    {
                        playerPosition = new Vector2(x * TILE_SIZE + TILE_SIZE / 2, y * TILE_SIZE + TILE_SIZE / 2);
                    }
                }
            }
        }

        public override void Reset()
        {
        }

        public override Vector3 Update(GameTime gameTime, Vector3 cameraOrientation, bool cameraHalfClicked)
        {
            float confirmedPositionX = playerPosition.X;
            float confirmedPositionY = playerPosition.Y;
            if (cameraHalfClicked)
            {
                var targetPosition = playerPosition + new Vector2(cameraOrientation.X, cameraOrientation.Z) * 0.3f;
                if (!wallTypes.ContainsKey(MAP[(int)(targetPosition.X / TILE_SIZE), (int)(playerPosition.Y / TILE_SIZE)]))
                    confirmedPositionX = targetPosition.X;
                else
                    confirmedPositionX = playerPosition.X;

                if (!wallTypes.ContainsKey(MAP[(int)(playerPosition.X / TILE_SIZE), (int)(targetPosition.Y / TILE_SIZE)]))
                    confirmedPositionY = targetPosition.Y;
                else
                    confirmedPositionY = playerPosition.Y;

            }
            playerPosition = new Vector2(confirmedPositionX, confirmedPositionY);
            return new Vector3(playerPosition.X, 3, playerPosition.Y);
        }

        public override Color Draw(GameTime gameTime)
        {
            for (int x = 0; x < MAP.GetLength(0); x++)
            {
                for (int y = 0; y < MAP.GetLength(1); y++)
                {
                    float left = x * TILE_SIZE;
                    float right = left + TILE_SIZE;
                    float top = y * TILE_SIZE;
                    float bottom = top + TILE_SIZE;
                    var tileType = MAP[x, y];
                    if (wallTypes.ContainsKey(tileType))
                    {
                        var wallTexture = wallTypes[tileType];
                        // top
                        if (y - 1 > 0 && !wallTypes.ContainsKey(MAP[x, y - 1]))
                        {
                            Game.VertexManager.AddRectangle(wallTexture, Color.White,
                                new Vector3(right, TILE_SIZE, top), new Vector3(left, TILE_SIZE, top),
                                new Vector3(left, 0, top), new Vector3(right, 0, top));
                        }
                        // right
                        if (x + 1 < MAP.GetLength(0) && !wallTypes.ContainsKey(MAP[x + 1, y]))
                        {
                            Game.VertexManager.AddRectangle(wallTexture, Color.White,
                                new Vector3(right, TILE_SIZE, bottom), new Vector3(right, TILE_SIZE, top),
                                new Vector3(right, 0, top), new Vector3(right, 0, bottom));
                        }
                        // bottom
                        if (y + 1 < MAP.GetLength(1) && !wallTypes.ContainsKey(MAP[x, y + 1]))
                        {
                            Game.VertexManager.AddRectangle(wallTexture, Color.White,
                                new Vector3(left, TILE_SIZE, bottom), new Vector3(right, TILE_SIZE, bottom),
                                new Vector3(right, 0, bottom), new Vector3(left, 0, bottom));
                        }
                        // left
                        if (x - 1 > 0 && !wallTypes.ContainsKey(MAP[x - 1, y]))
                        {
                            Game.VertexManager.AddRectangle(wallTexture, Color.White,
                                new Vector3(left, TILE_SIZE, top), new Vector3(left, TILE_SIZE, bottom),
                                new Vector3(left, 0, bottom), new Vector3(left, 0, top));
                        }
                    }
                    else
                    {
                        Game.VertexManager.AddRectangle(boards, Color.White,
                            new Vector3(right, TILE_SIZE, top), new Vector3(left, TILE_SIZE, top),
                            new Vector3(left, TILE_SIZE, bottom), new Vector3(right, TILE_SIZE, bottom));
                        Game.VertexManager.AddRectangle(cobbleStone, Color.White,
                            new Vector3(left, 0, top), new Vector3(right, 0, top),
                            new Vector3(right, 0, bottom), new Vector3(left, 0, bottom));
                    }
                }
            }

            return Color.Black;
        }

        public override void DrawIdentifier(GameTime gameTime, Vector3 centerPosition, bool hovering, float scaleAmount)
        {
            Color tint;
            if (hovering)
            {
                tint = Color.White;
            }
            else
            {
                tint = Color.DarkGray;
            }

            float left = centerPosition.X - TILE_SIZE / 2 / scaleAmount;
            float right = centerPosition.X + TILE_SIZE / 2 / scaleAmount;
            float front = centerPosition.Z + TILE_SIZE / 2 / scaleAmount;
            float back = centerPosition.Z - TILE_SIZE / 2 / scaleAmount;
            float top = centerPosition.Y + TILE_SIZE / 2 / scaleAmount;
            float bottom = centerPosition.Y - TILE_SIZE / 2 / scaleAmount;
        }
    }
}
