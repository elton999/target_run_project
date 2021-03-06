﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CareerOpportunities.Level
{
    public class Render : GameObject
    {

        float TileMapWidth;

        Vector2[] PositionGround;
        Vector2[] PositionBackground;

        int scale;
        

        public Render(int scale, int BufferHeight, int start)
        {
            this.scale = scale;
            this.start = start;
            this.currentPositionX = start;
            this.BufferHeight = BufferHeight;

            this.PositionGround = new Vector2[] {
                    new Vector2(0,0),
                    new Vector2(start,0)
            };

            this.PositionBackground = new Vector2[] {
                    new Vector2(0,-(100*this.scale)),
                    new Vector2(start,-(100*this.scale))
            };

            // sprite height
            int spriteHeight = 33 * scale;
            this.LinesBox = new int[] {
                this.BufferHeight - (spriteHeight + scale),
                this.BufferHeight - (spriteHeight * 2) + (10 * scale),
                this.BufferHeight - (spriteHeight * 3) + (20 * scale),
                this.BufferHeight - (spriteHeight * 4) + (30 * scale),
                this.BufferHeight - (spriteHeight * 5) + (40 * scale),
            };
        }


        #region Set sprites

        Texture2D CoinTexture;
        List<Texture2D> Ground;
        List<Texture2D> Background;

        public void SetBackground(ContentManager content, int Level)
        {
            if (Level > 3) this.SetBackgroundTexture(2, content);
            else this.SetBackgroundTexture(1, content);

            switch (Level)
            {
                case 1:
                    this.setGroundTexture(1, content);
                    break;
                case 2:
                    this.setGroundTexture(1, content);
                    break;
                case 3:
                    this.setGroundTexture(2, content);
                    break;
                case 4:
                    this.setGroundTexture(2, content);
                    break;
                case 5:
                    this.setGroundTexture(2, content);
                    break;
                case 6:
                    this.setGroundTexture(1, content);
                    break;
            }
        }

        private void SetBackgroundTexture(int BN, ContentManager Content)
        {
            this.Background = new List<Texture2D>();
            this.Background.Add(Content.Load<Texture2D>("prototype/background_"+BN+"_1"));
            this.Background.Add(Content.Load<Texture2D>("prototype/background_"+BN+"_2"));
        }

        private void setGroundTexture(int BN, ContentManager Content)
        {
            this.Ground = new List<Texture2D>();
            this.Ground.Add(Content.Load<Texture2D>("prototype/ground_" + BN + "_1"));
            this.Ground.Add(Content.Load<Texture2D>("prototype/ground_" + BN + "_2"));
        }

        public void setCoinTexture(Texture2D coin, string jsonFile)
        {
            this.CoinTexture = coin;
            this.setJsonFile(jsonFile);
            this.setSprite(this.CoinTexture);
        }
        #endregion

        #region TileMap
        
        Texture2D TileMap;
        TypeOfItems[,] MapItems;
        Vector2[] positionBoxs;
        TypeOfItems[] SpritesColors;

        public enum TypeOfItems
        {
            NONE,
            BOX,
            BOX_EFFECT,
            CONE,
            COIN,
            RAMP,
            HEART,
            THORN,
            THORN_OFF,
            THORN_ON,
            HIT_BOX,
        };


        public void setTileMap(Texture2D map)
        {
            this.TileMap = map;
            this.TileMapWidth = map.Width;
            this.readMap();
        }

        public TypeOfItems ColorToType(Color color)
        {
            if (color == Color.Red) return Render.TypeOfItems.BOX;
            else if (color == Color.Pink) return Render.TypeOfItems.BOX_EFFECT;
            else if (color == Color.Yellow) return Render.TypeOfItems.COIN;
            else if (color == Color.Blue) return Render.TypeOfItems.HEART;
            else if (color == Color.Green) return Render.TypeOfItems.RAMP;
            else if (color == Color.Purple) return Render.TypeOfItems.HIT_BOX;
            else if (color == Color.Tomato) return Render.TypeOfItems.CONE;
            else if (color == Color.SteelBlue) return Render.TypeOfItems.THORN;
            else if (color == Color.SkyBlue) return Render.TypeOfItems.THORN_ON;
            else if (color == Color.PowderBlue) return Render.TypeOfItems.THORN_OFF;
            else return Render.TypeOfItems.NONE;
        }

        public void readMap()
        {
            if (TileMap != null)
            {
                Color[] colors1D = new Color[this.TileMap.Width * this.TileMap.Height];
                this.TileMap.GetData(colors1D);
                this.MapItems = new Render.TypeOfItems[this.TileMap.Width, this.TileMap.Height];

                for (int x = 0; x < this.TileMap.Width; x++)
                {
                    for (int y = this.TileMap.Height - 1; y >= 0 ; y--)
                    {
                        this.MapItems[x, this.TileMap.Height - 1 - y] = this.ColorToType(colors1D[x + y * this.TileMap.Width]);
                    }
                }
            }

            this.PositionTile();
        }


        private float ChangeThornGameTimeTotal = 0;
        private bool ChangeThornTurn = true;
        private void ChangeThorn(GameTime gameTime)
        {
            if (!this.isStoped)
            {
                this.ChangeThornGameTimeTotal += gameTime.ElapsedGameTime.Milliseconds;
                if (this.ChangeThornGameTimeTotal % 300 == 0)
                {
                    ChangeThornTurn = !ChangeThornTurn;
                }
            }
        }

        private void PositionTile()
        {
            List<Vector2> termsList = new List<Vector2>();
            List<Render.TypeOfItems> termsListColors = new List<Render.TypeOfItems>();
            for (int x = 0; x < this.TileMapWidth; x++)
            {
                for (int y = 5 - 1; y >= 0; y--)
                {
                    if (this.MapItems[x, y] == Render.TypeOfItems.BOX)
                    {
                        termsList.Add(new Vector2(x, y));
                        termsListColors.Add(Render.TypeOfItems.BOX);
                    }
                    else if (this.MapItems[x, y] != Render.TypeOfItems.BOX)
                    {
                        termsList.Add(new Vector2(x, y));
                        termsListColors.Add(this.MapItems[x,y]);
                    }
                }
            }
            this.SpritesColors = termsListColors.ToArray();
            this.positionBoxs = termsList.ToArray();
            this.TileMap = null; // destroy
        }
        #endregion

        #region Collision

        public Vector2 CollisionPosition;

        public bool Collision(Rectangle body, Vector2 position, int line, bool item = true)
        {
            bool any_collision = false;
            for (int x = 0; x < this.TileMapWidth; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    if (this.MapItems[x, y] != Render.TypeOfItems.NONE) {
                        float x_position = ((x) * 25) * this.scale + this.currentPositionX;
                        float x_width = x_position + (40 * this.scale);

                        /*bool x_overlaps = (((body.X + position.X < x_position) && (body.X + position.X + body.Width > x_position) && (body.X + position.X + body.Width < x_width)) || 
                                           ((body.X + position.X > x_position) && (body.X + position.X + body.Width < x_width)) ||
                                           ((body.X + position.X > x_position) && (body.X + position.X < x_width) && (body.X + position.X + body.Width > x_width)));*/

                        bool x_overlaps = (((body.X < x_position) && (body.X + body.Width > x_position) && (body.X + body.Width < x_width)) ||
                        ((body.X > x_position) && (body.X + body.Width < x_width)) ||
                        ((body.X > x_position) && (body.X < x_width) && (body.X + body.Width > x_width)));

                        bool y_overlaps = line == y;

                        if (x_overlaps && y_overlaps)
                        {
                            //if (this.MapColors[x, y] != this.BoxColor) this.CollisionItem(new Vector2(x, y));
                            if(!item && ( this.MapItems[x, y] == Render.TypeOfItems.BOX || this.MapItems[x, y] == Render.TypeOfItems.CONE || this.MapItems[x, y] == Render.TypeOfItems.THORN || (this.MapItems[x, y] == Render.TypeOfItems.THORN_ON && this.ChangeThornTurn) || (this.MapItems[x, y] == Render.TypeOfItems.THORN_OFF && !this.ChangeThornTurn))) this.CollisionPosition = new Vector2(x, y);
                            else if (item) this.CollisionPosition = new Vector2(x, y);
                            if (this.MapItems[x, y] == Render.TypeOfItems.BOX || this.MapItems[x, y] == Render.TypeOfItems.CONE || this.MapItems[x, y] == Render.TypeOfItems.THORN || (this.MapItems[x, y] == Render.TypeOfItems.THORN_ON && this.ChangeThornTurn) || (this.MapItems[x, y] == Render.TypeOfItems.THORN_OFF && !this.ChangeThornTurn)) any_collision = true;
                        }

                    }
                }
            }
            return any_collision;
        }

        public void DestroyHitBox(Vector2 position)
        {
            for (int i = 0; i < 5; i++) this.MapItems[(int)position.X, i] = Render.TypeOfItems.NONE;
        }

        public string CollisionItem(Vector2 position, bool item = false, bool fireCollision = false, bool bossCollision = false)
        {
            Render.TypeOfItems ReturnColor = this.MapItems[(int)position.X, (int)position.Y];
            string ReturnItem = "";
            

            for (int i = 0; i < this.positionBoxs.Length; i++)
            {
                if (this.positionBoxs[i] == position)
                {
                    if (Render.TypeOfItems.BOX != ReturnColor && Render.TypeOfItems.BOX_EFFECT != ReturnColor && Render.TypeOfItems.NONE != ReturnColor && item)
                    {
                        if (ReturnColor == Render.TypeOfItems.HEART) ReturnItem = "heart";
                        if (ReturnColor == Render.TypeOfItems.COIN) ReturnItem = "coin";
                        if (ReturnColor == Render.TypeOfItems.RAMP) ReturnItem = "ramp";
                        if (ReturnColor == Render.TypeOfItems.HIT_BOX) ReturnItem = "hit_box";
                        if (Render.TypeOfItems.RAMP != ReturnColor  &&  Render.TypeOfItems.HIT_BOX != ReturnColor && ((Render.TypeOfItems.THORN_OFF != ReturnColor && this.ChangeThornTurn) || (Render.TypeOfItems.THORN_ON != ReturnColor && !this.ChangeThornTurn)))
                        {
                            this.SpritesColors[i] = Render.TypeOfItems.NONE;
                            this.MapItems[(int)position.X, (int)position.Y] = Render.TypeOfItems.NONE;
                        }
                    }
                    else if (!item || bossCollision)
                    {
                        if (ReturnColor == Render.TypeOfItems.BOX) ReturnItem = "box";
                        if (ReturnColor == Render.TypeOfItems.BOX)
                        {
                            this.SpritesColors[i] = Render.TypeOfItems.BOX_EFFECT;
                            this.MapItems[(int)position.X, (int)position.Y] = Render.TypeOfItems.BOX_EFFECT;
                        } else if (ReturnColor == Render.TypeOfItems.CONE)
                        {
                            this.SpritesColors[i] = Render.TypeOfItems.NONE;
                            this.MapItems[(int)position.X, (int)position.Y] = Render.TypeOfItems.NONE;
                        }
                        else if(!fireCollision && !bossCollision)
                        {
                            this.SpritesColors[i] = Render.TypeOfItems.NONE;
                            this.MapItems[(int)position.X, (int)position.Y] = Render.TypeOfItems.NONE;
                        }
                    }
                   
                }
            }

            return ReturnItem;
        }

        #endregion

        #region Level Management

        int BufferHeight;
        int tileWidth = 34;

        public int CurrentlyLevel;
        public int LastLevel = 6;

        int[] LinesBox;

        int stopFramesNum = 0;
        int CurrentStopFramesNum = 1;

        int start;
        int currentPositionX;

        public void StopFor(int frames = 15)
        {
            stopFramesNum = frames;
            CurrentStopFramesNum = 0;
        }

        public bool isStoped
        {
            get
            {
                if (CurrentStopFramesNum > stopFramesNum) return false;
                else return true;
            }
        }

        public void setLevel(int level)
        {
            this.CurrentlyLevel = level;
        }

        public int NextLevel()
        {
            return this.CurrentlyLevel + 1;
        }

        public int LinePosition(float Y)
        {
            int line = 0;
            if (Y > this.LinesBox[4] && Y < this.LinesBox[3]) line = 4;
            else if (Y > this.LinesBox[3] && Y < this.LinesBox[2]) line = 3;
            else if (Y > this.LinesBox[2] && Y < this.LinesBox[1]) line = 2;
            else if (Y > this.LinesBox[1] && Y < this.LinesBox[0]) line = 1;
            else if (Y > this.LinesBox[0]) line = 0;
            return line;
        }

        public bool Finished()
        {
            if (-this.currentPositionX + (this.start * this.scale) > ((this.TileMapWidth * 30) + (100)) * this.scale) return true;
            return false;
        }

        public float FinishedPerCent()
        {
            float total = ((this.TileMapWidth * 30) + (100)) * this.scale;
            float total_finished = (-this.currentPositionX + (this.start * this.scale));
            return total_finished / total * 100;
        }
        #endregion
        
        int velocity = 0;
        public Hud.Countdown countdown;

        public void Update(GameTime gameTime, PlayerController Player)
        {

            this.play(gameTime, "round", AnimationDirection.LOOP);
            if (Player.isGrounded) this.velocity = 145;
            else this.velocity = 130;

            if (!countdown.isCountdown && this.CurrentlyLevel > 1)
            {
                if (CurrentStopFramesNum > stopFramesNum)
                {
                    float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    int velocityCurrent = (int)(velocity * delta * this.scale);
                    int velocityCurrentbackground = (int)((velocity - 10) * delta * this.scale);
                    // int velocityCurrent = (int)(2 * this.scale);
                    this.currentPositionX -= velocityCurrent;

                    for (int i = 0; i < this.PositionGround.Length; i++)
                    {
                        if (this.PositionGround[i].X + Ground[0].Width * scale <= 0)
                        {
                            if (i == 0) this.PositionGround[i] = new Vector2(this.PositionGround[this.PositionGround.Length - 1].X + (Ground[0].Width * scale), 0);
                            else if (i == this.PositionGround.Length - 1) this.PositionGround[i] = new Vector2(this.PositionGround[i - 1].X + (Ground[0].Width * scale), 0);
                            //else this.PositionGround[i] = this.PositionGround[i] = new Vector2(this.PositionGround[i + 1].X + (Ground[0].Width * scale), 0);
                        }

                        if (this.PositionBackground[i].X + Background[0].Width * scale <= 0)
                        {
                            if (i == 0) this.PositionBackground[i] = new Vector2(this.PositionBackground[this.PositionBackground.Length - 1].X + (Background[0].Width * scale), this.PositionBackground[this.PositionBackground.Length - 1].Y);
                            else if (i == this.PositionBackground.Length - 1) this.PositionBackground[i] = new Vector2(this.PositionBackground[i - 1].X + (Background[0].Width * scale), this.PositionBackground[i - 1].Y);
                            //else this.PositionGround[i] = this.PositionGround[i] = new Vector2(this.PositionGround[i + 1].X + (Ground[0].Width * scale), 0);
                        }
                        this.PositionGround[i] = new Vector2(this.PositionGround[i].X - velocityCurrent, this.PositionGround[i].Y);
                        this.PositionBackground[i] = new Vector2(this.PositionBackground[i].X - velocityCurrentbackground, this.PositionBackground[i].Y);
                    }
                }
                else CurrentStopFramesNum += 1;
                this.ChangeThorn(gameTime);
            }
            
        }

        #region Draw
        public void DrawGround(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this.PositionBackground.Length; i++)
            {
                spriteBatch.Draw(Background[i], this.PositionBackground[i], null, Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < this.PositionGround.Length; i++)
            {
                spriteBatch.Draw(Ground[i], this.PositionGround[i], null, Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
            }
        }

        public void Layers(SpriteBatch spriteBatch, int layer, bool front, bool mask = false)
        {
            if (front)
            {
                for (int i_layer = layer != 0 ? layer - 1 : layer; i_layer >= 0; i_layer--) this.drawLayer(i_layer, spriteBatch, mask);
            }
            else
            {
                for (int i_layer = 4; i_layer >= layer; i_layer--) this.drawLayer(i_layer, spriteBatch, mask);
            }
        }

        private void drawLayer(int i_layer, SpriteBatch spriteBatch, bool mask)
        {
            for (int i = this.positionBoxs.Length - 1; i >= 0; i--)
            {
                if (i_layer == (int)this.positionBoxs[i].Y) this.Draw(spriteBatch, i, mask);
            }
        }

        public void Draw(SpriteBatch spriteBatch, int i, bool mask)
        {
            Color SpriteColorTexture = Color.White;
            Vector2 position = new Vector2((this.positionBoxs[i].X * (this.scale * 25)) + (this.currentPositionX), this.LinesBox[(int)this.positionBoxs[i].Y]);

            switch (SpritesColors[i])
            {
                case Render.TypeOfItems.BOX:
                    spriteBatch.Draw(this.Sprite, position, new Rectangle(new Point(25, 0), new Point(34, 32)), SpriteColorTexture, 0, new Vector2(0, 0), this.scale, SpriteEffects.None, 0f);
                    break;
                case Render.TypeOfItems.BOX_EFFECT:
                    spriteBatch.Draw(this.Sprite, position, new Rectangle(new Point(59, 0), new Point(34, 32)), SpriteColorTexture, 0, new Vector2(0, 0), this.scale, SpriteEffects.None, 0f);
                    break;
                case Render.TypeOfItems.HEART:
                    spriteBatch.Draw(this.Sprite, new Vector2(position.X + (10 * this.scale), position.Y), new Rectangle(new Point(72, 43), new Point(22, 18)), SpriteColorTexture, 0, new Vector2(0, 0), this.scale, SpriteEffects.None, 0f);
                    break;
                case Render.TypeOfItems.RAMP:
                    spriteBatch.Draw(this.Sprite, position, new Rectangle(new Point(50, 32), new Point(22, 29)), SpriteColorTexture, 0, new Vector2(0, 0), this.scale, SpriteEffects.None, 0f);
                    break;
                case Render.TypeOfItems.CONE:
                    spriteBatch.Draw(this.Sprite, position, new Rectangle(new Point(0, 0), new Point(25, 32)), SpriteColorTexture, 0, new Vector2(0, 0), this.scale, SpriteEffects.None, 0f);
                    break;
                case Render.TypeOfItems.THORN_OFF:
                    if(ChangeThornTurn) spriteBatch.Draw(this.Sprite, new Vector2(position.X, position.Y + (this.scale * 10)), new Rectangle(new Point(0, 33), new Point(25, 22)), SpriteColorTexture, 0, new Vector2(0, 0), this.scale, SpriteEffects.None, 0f);
                    else spriteBatch.Draw(this.Sprite, new Vector2(position.X, position.Y + (this.scale * 10)), new Rectangle(new Point(25, 33), new Point(25, 22)), SpriteColorTexture, 0, new Vector2(0, 0), this.scale, SpriteEffects.None, 0f);
                    break;
                case Render.TypeOfItems.THORN_ON:
                    if (ChangeThornTurn) spriteBatch.Draw(this.Sprite, new Vector2(position.X, position.Y + (this.scale * 10)), new Rectangle(new Point(25, 33), new Point(25, 22)), SpriteColorTexture, 0, new Vector2(0, 0), this.scale, SpriteEffects.None, 0f);
                    else spriteBatch.Draw(this.Sprite, new Vector2(position.X, position.Y + (this.scale * 10)), new Rectangle(new Point(0, 33), new Point(25, 22)), SpriteColorTexture, 0, new Vector2(0, 0), this.scale, SpriteEffects.None, 0f);
                    break;
                case Render.TypeOfItems.THORN:
                    spriteBatch.Draw(this.Sprite, new Vector2(position.X, position.Y + (this.scale * 10)), new Rectangle(new Point(0, 57), new Point(28, 22)), SpriteColorTexture, 0, new Vector2(0, 0), this.scale, SpriteEffects.None, 0f);
                    break;
            }

            if (SpritesColors[i] == Render.TypeOfItems.COIN && !mask)
            {
                this.sizeMutiply = 5;
                this.DrawAnimation(spriteBatch, position, scale);
            }
        }
        #endregion
    }
}
