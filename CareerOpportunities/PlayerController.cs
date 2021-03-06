﻿using System;
using CareerOpportunities.weapon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace CareerOpportunities
{
    public class PlayerController : GameObject
    {

        public Game1 game;

        public bool canMoveVertical;
        public int CurrentVerticalLine;
        public float PlayerVerticalVelocity;
        public float PlayerHorizontalVelocity;
        public float PlayerHorizontalAndVelocity;

        private int BufferWidth;
        private int[] Lines;
        private int PreviousVerticalLine;
        
        public  bool isGrounded = true;
        private float JumpTime;
        private float JumpTimeCurrent = 0;

        private int runTimes;

        public enum stateAnimation
        {
            RUN,
            HIT,
            BEFORE_JUMP,
            JUMPING,
            AFTER_JUMP,
            BEFORE_FIRE,
            AFTER_FIRE,
            VERTICAL_UP,
            VERTICAL_DOWN,
            
        }
        public stateAnimation currentAnimation;


        SoundEffect HitSFX;
        SoundEffect CollectCoinSFX;
        SoundEffect ColletctHeartSFX;

        Texture2D JimSprite;
        Texture2D JosieSprite;

        public PlayerController(int BufferHeight, int BufferWidth, Game1 game)
        {
            this.game = game;
            this.JimSprite = this.game.Content.Load<Texture2D>("prototype/Jim");
            this.JosieSprite = this.game.Content.Load<Texture2D>("prototype/Josie");
            this.Scale = game.scale;

            if (this.game.MainMenu.CharacterSelected == MenuManagement.Characters.JIM) this.Sprite = JimSprite;
            else if (this.game.MainMenu.CharacterSelected == MenuManagement.Characters.JOSIE) this.Sprite = JosieSprite;

            this.CurrentVerticalLine = 3;
            this.PreviousVerticalLine = 1;
            this.PlayerVerticalVelocity = (22 * this.Scale) / 5.5f;
            this.PlayerHorizontalVelocity = 2 * this.Scale;
            this.PlayerHorizontalAndVelocity = 0.4f * this.Scale;
            this.BufferWidth = BufferWidth;

            int linePosition = (32 * this.Scale);
            Lines = new int[] {
                BufferHeight - linePosition  + (-5 * this.Scale),
                BufferHeight - (linePosition * 2) + (5 * this.Scale),
                BufferHeight - (linePosition * 3) + (15 * this.Scale),
                BufferHeight - (linePosition * 4) + (25 * this.Scale),
                BufferHeight - (linePosition * 5) + (35 * this.Scale),
                BufferHeight - (linePosition * 6) + (45 * this.Scale),
            };

            this.setJsonFile(this.game.path + "/Content/prototype/jim.json");
            this.setSprite(this.Sprite);

            canMoveVertical = true;
            this.Position = new Vector2(0, Lines[CurrentVerticalLine]);
            this.Body = new Rectangle(new Point(8 * this.Scale, 0), new Point(16 * this.Scale, 21 * this.Scale));

            this.currentAnimation = PlayerController.stateAnimation.RUN;
            this.runTimes = 0;

            this.CollisionBoss = false;

            HitSFX = game.Content.Load<SoundEffect>("Sound/sfx_exp_short_hard12");
            CollectCoinSFX = game.Content.Load<SoundEffect>("Sound/coin3");
            ColletctHeartSFX = game.Content.Load<SoundEffect>("Sound/sfx_sounds_powerup3");
        }


        public void PlayAnimation(GameTime gameTime)
        {
            if (this.isGrounded)
            {
                if (Position.Y == Lines[CurrentVerticalLine] && this.currentAnimation == PlayerController.stateAnimation.RUN)
                {
                    canMoveVertical = true;
                }
                else
                {
                    if (
                        (CurrentVerticalLine < PreviousVerticalLine && Lines[CurrentVerticalLine] < Position.Y) ||
                        (CurrentVerticalLine > PreviousVerticalLine && Lines[CurrentVerticalLine] > Position.Y))
                    {
                        Position = new Vector2(Position.X, Lines[CurrentVerticalLine]);
                        this.currentAnimation = PlayerController.stateAnimation.RUN;
                    }
                    else
                    {
                        if (Position.Y < Lines[CurrentVerticalLine])
                        {
                            Position = new Vector2(Position.X, Position.Y + PlayerVerticalVelocity);
                            this.currentAnimation = PlayerController.stateAnimation.VERTICAL_DOWN;
                        }
                        else if (Position.Y > Lines[CurrentVerticalLine])
                        {
                            Position = new Vector2(Position.X, Position.Y - PlayerVerticalVelocity);
                            this.currentAnimation = PlayerController.stateAnimation.VERTICAL_UP;
                        }
                    }
                }
            }


            if (!game.Map.isStoped)
            {
                if (this.isGrounded && this.currentAnimation == PlayerController.stateAnimation.RUN)
                {
                    if (this.lastFrame) this.runTimes += 1;

                    if (this.runTimes < 3) this.play(gameTime, "run_idle");
                    else this.play(gameTime, "run");

                    if (this.runTimes == 4) this.runTimes = 0;
                }
                else if (this.isGrounded && this.currentAnimation == PlayerController.stateAnimation.VERTICAL_UP) this.play(gameTime, "up");
                else if (this.isGrounded && this.currentAnimation == PlayerController.stateAnimation.VERTICAL_DOWN) this.play(gameTime, "down");
                else if (this.isGrounded && this.currentAnimation == PlayerController.stateAnimation.BEFORE_FIRE)
                {
                    if (this.lastFrame)
                    {
                        this.currentAnimation = PlayerController.stateAnimation.AFTER_FIRE;
                        game.Weapon.Fire(this.Position);
                    }
                    else this.play(gameTime, "start_fire");
                }
                else if (this.isGrounded && this.currentAnimation == PlayerController.stateAnimation.AFTER_FIRE)
                {
                    if (this.lastFrame) this.currentAnimation = PlayerController.stateAnimation.RUN;
                    else this.play(gameTime, "after_fire");
                }
                else if (this.currentAnimation == PlayerController.stateAnimation.AFTER_JUMP)
                {
                    if (this.lastFrame) this.currentAnimation = PlayerController.stateAnimation.RUN;
                    else this.play(gameTime, "landing");
                }
                else if (this.currentAnimation == PlayerController.stateAnimation.JUMPING)
                {
                    if (this.isGrounded)
                    {
                        this.currentAnimation = PlayerController.stateAnimation.AFTER_JUMP;
                    }
                    else this.play(gameTime, "jumping");
                }
                else
                {
                    if (this.lastFrame) this.currentAnimation = PlayerController.stateAnimation.JUMPING;
                    else this.play(gameTime, "start_jump");
                }
            } else this.play(gameTime, "hit", AnimationDirection.LOOP);
        }

        private bool CollisionBoss = false;

        private void RiggiBody()
        {
            this.Body = new Rectangle(new Point((int)this.Position.X + 22 * this.Scale, (int)this.Position.Y + 15 * this.Scale), new Point(16, 21));
        }

        public void Update(GameTime gameTime, Controller.Input input, CameraManagement camera)
        {
            float pull = 100f;
            float JumpPull = pull;
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            this.RiggiBody();

            if (this.isGrounded)
            {
                // boss level
                if (game.Boss.isBossLevel(game.Level) && game.Boss.Collision(this.Body) && !this.CollisionBoss)
                {
                    game.Hearts.remove(game.Hearts.NumberOfhearts);
                    game.Map.StopFor(60);
                    camera.TimeShake = 15;
                    this.CollisionBoss = true;
                }

                if (game.Map.Collision(this.Body, this.Position, this.CurrentVerticalLine))
                {
                    string MapItem = game.Map.CollisionItem(game.Map.CollisionPosition, false);

                    if (game.Boss.isBossLevel(game.Level)) game.Hearts.remove(game.Hearts.NumberOfhearts);
                    else game.Hearts.remove(1);

                    game.Map.StopFor(60);
                    camera.TimeShake = 15;
                    this.HitSFX.Play();
                }
                else
                {
                    string MapItem = game.Map.CollisionItem(game.Map.CollisionPosition, true);
                    switch (MapItem)
                    {
                        case "heart":
                            game.Hearts.add(1, this.Position);
                            this.ColletctHeartSFX.Play();
                            break;
                        case "coin":
                            game.Coins.add(this.Position, 1);
                            this.CollectCoinSFX.Play();
                            break;
                        case "ramp":
                            this.isGrounded = false;
                            JumpPull = pull;
                            camera.StartZoomJump();
                            game.Map.CollisionPosition = Vector2.Zero;
                            break;
                        case "hit_box":
                            game.Map.CollisionPosition = Vector2.Zero;
                            break;
                    }
                }

                if (!game.Map.isStoped && !game.Countdown.isCountdown)
                {
                    // push player back
                    float limit = (BufferWidth / 2) - (150 * this.Scale);
                    if (game.Boss != null && game.Boss.isBossLevel(game.Level)) limit = (BufferWidth / 2) - (30 * this.Scale);

                    if (this.Position.X - (pull * this.Scale * delta) > limit && !game.Map.Collision(
                    this.Body,
                    new Vector2(this.Position.X - (pull * this.Scale * delta), this.Position.Y),
                    this.CurrentVerticalLine
                    )) this.Position = new Vector2(this.Position.X - (pull * this.Scale * delta), this.Position.Y);
                    else if (this.Position.X + (pull * this.Scale * delta) < limit) this.Position = new Vector2(this.Position.X + ((pull / 2) * this.Scale * delta), this.Position.Y);

                    if (canMoveVertical)
                    {
                        PreviousVerticalLine = CurrentVerticalLine;
                        // top and down
                        this.HorizontalMove(input, game.Map);

                        // Jump
                        /*if (input.KeyPress(Controller.Input.Button.JUMP) && this.isGrounded)
                        {
                            this.isGrounded = false;
                            JumpPull = 140f;
                            camera.StartZoomJump();
                            game.Map.CollisionPosition = Vector2.Zero;
                        }*/
                    }

                    // left and right
                    this.VerticalMove(input, game.Map);

                    if (input.KeyPress(Controller.Input.Button.FIRE) && game.Level != 6 && (this.currentAnimation == PlayerController.stateAnimation.RUN || this.currentAnimation == PlayerController.stateAnimation.AFTER_FIRE)) {
                       this.currentAnimation = PlayerController.stateAnimation.BEFORE_FIRE;
                    }
                }
            }
            
            this.PlayAnimation(gameTime);
            this.Jump(gameTime, delta, pull);
        }

        public void VerticalMove(Controller.Input input, Level.Render map)
        {
            if (input.KeyDown(Controller.Input.Button.RIGHT) && !map.Collision(this.Body, new Vector2(Position.X + PlayerHorizontalVelocity, Position.Y), this.CurrentVerticalLine, false))
            {
                if ((input.KeyDown(Controller.Input.Button.DOWN) || input.KeyDown(Controller.Input.Button.UP)) && (Position.X + PlayerHorizontalAndVelocity < this.BufferWidth - (this.Scale * 32)))
                  Position = new Vector2(Position.X + PlayerHorizontalAndVelocity, Position.Y);
                else if (Position.X + PlayerHorizontalVelocity < this.BufferWidth - (this.Scale * 32))
                    Position = new Vector2(Position.X + PlayerHorizontalVelocity, Position.Y);
            }

            if (input.KeyDown(Controller.Input.Button.LEFT))
            {
                if ((input.KeyDown(Controller.Input.Button.DOWN) || input.KeyDown(Controller.Input.Button.UP)) && (Position.X - PlayerHorizontalAndVelocity > 0))
                    Position = new Vector2(Position.X - PlayerHorizontalAndVelocity, Position.Y);
                else if (Position.X - PlayerHorizontalVelocity > 0)
                    Position = new Vector2(Position.X - PlayerHorizontalVelocity, Position.Y);
            }

            game.Map.CollisionPosition = Vector2.Zero;
        }

        public void HorizontalMove(Controller.Input input, Level.Render map)
        {
            if (input.KeyDown(Controller.Input.Button.UP) && CurrentVerticalLine < 4)
            {
                if (!map.Collision(this.Body, this.Position, this.CurrentVerticalLine + 1, false))
                {
                    CurrentVerticalLine += 1;
                    canMoveVertical = false;
                }
            }
            if (input.KeyDown(Controller.Input.Button.DOWN) && CurrentVerticalLine > 0)
            {
                if (!map.Collision(this.Body, this.Position, this.CurrentVerticalLine - 1, false))
                {
                    CurrentVerticalLine -= 1;
                    canMoveVertical = false;
                }
            }
            game.Map.CollisionPosition = Vector2.Zero;
        }

        public void Jump( GameTime gameTime, float delta, float pull)
        {
            if (!this.isGrounded)
            {
                if (this.JumpTimeCurrent <= this.JumpTime && this.Position.Y > Lines[CurrentVerticalLine + 1]) this.Position = new Vector2(this.Position.X, this.Position.Y - (pull * this.Scale * delta));
                else
                {
                    this.JumpTime = 0.2f*60f;
                    if (this.JumpTimeCurrent <= this.JumpTime) this.JumpTimeCurrent++;
                    else
                    {
                        if (this.Position.Y < Lines[CurrentVerticalLine]) this.Position = new Vector2(this.Position.X, this.Position.Y + ((pull / 2 ) * this.Scale * delta));
                        else
                        {
                            this.Position = new Vector2(this.Position.X, Lines[CurrentVerticalLine]);
                            this.isGrounded = true;
                            this.JumpTimeCurrent = 0;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 sprite_position = new Vector2(this.Position.X - (2 * this.Scale), this.Position.Y - (16 * this.Scale));
            this.DrawAnimation(spriteBatch, sprite_position, this.Scale);
#if DEBUG
            this.DrawRiggidBody(spriteBatch, this.game.GraphicsDevice);
#endif
        }

    }
}
