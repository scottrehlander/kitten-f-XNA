using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LD48
{
    public class InnocentCat : Cat
    {

        #region Variables

        protected bool _isFacingRight = true;

        // Determine a new path
        Vector2 _endPathPoint = new Vector2(int.MinValue, int.MinValue);
        Vector2 _startPathPoint = new Vector2(int.MinValue, int.MinValue);

        // Manage the amount of time it takes for a path to complete
        TimeSpan _timeUntilNextMove = TimeSpan.FromMilliseconds(2);
        TimeSpan _timeSinceLastMove = TimeSpan.Zero;
        int _clicksToGetThere = 220;
        int _clicksOnCurrentPath = 0;

        private int _health;
        public override int Health 
        {
            get
            {
                return _health;
            }
            set
            {
                if (SharedContext.MovableEntityManager.FloatingStatuses != null)
                {
                    // Create a floating status update
                    FloatingStatus fs = new FloatingStatus((value < _health ? "-" : "+") + (_health - value).ToString(), Color.White);
                    fs.WorldPosition = new Vector2(WorldPosition.X, WorldPosition.Y - 15);
                    SharedContext.MovableEntityManager.FloatingStatuses.Add(fs);
                }

                _health = value;
            }
        }

        #endregion

        public InnocentCat()
        {
            LoadContent();
        }


        #region XNA Methods

        public override void LoadContent()
        {
            Texture = SharedContext.Content.Load<Texture2D>("Images/innocentCat1_color");
            
            CollisionBox = new Rectangle(8, 5, 26, 15);

            LineOfSite = new Rectangle(-50, -70, 150, 150);

            _health = 1;

            base.LoadContent();
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (GestationState == GestationStateEnum.Breeding)
            {
                if (TimeBreeding < TimeToBreed)
                    TimeBreeding += gameTime.ElapsedGameTime;

                if (TimeBreeding > TimeToBreed)
                {
                    GestationState = GestationStateEnum.Grown;

                    if (Offspring != null)
                    {
                        SharedContext.MovableEntityManager.RequestedCatsToAdd.Add(Offspring);
                        Offspring = null;
                    }
                }

                return;
            }

            // Increase the SinceMated timer
            if (SinceMated <= TimeUntilCanMateAgain)
                SinceMated += gameTime.ElapsedGameTime;

            // If we have no current end point, make one
            if (_endPathPoint.X == int.MinValue)
            {
                CreateNextEndPoint();
                _clicksOnCurrentPath = 0;
            }

            // Increment the time since the last move and if it is greater, make the move
            _timeSinceLastMove += gameTime.ElapsedGameTime;
            if (_timeSinceLastMove > _timeUntilNextMove)
            {
                // Apply the vector to our object so they move in the right direction
                int amountToMoveX = (int)(_endPathPoint.X - _startPathPoint.X) / _clicksToGetThere;
                int amountToMoveY = (int)(_endPathPoint.Y - _startPathPoint.Y) / _clicksToGetThere;

                WorldPosition = new Vector2(WorldPosition.X + amountToMoveX, WorldPosition.Y + amountToMoveY);

                // Increment the click we are on
                _clicksOnCurrentPath++;
                _timeSinceLastMove = TimeSpan.Zero;
            }

            // We have finished out route
            if (_clicksOnCurrentPath >= _clicksToGetThere)
            {
                _endPathPoint.X = int.MinValue;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float layerDepth = ((WorldPosition.Y + 25) + 99999) * 100 / 100000000;

            spriteBatch.Draw(Texture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 40, 25), null, Color.White, 0, Vector2.Zero,
                   !_isFacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);

            if (GestationState == GestationStateEnum.Breeding)
            {
                spriteBatch.Draw(CensoredTexture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 110, 35), null, Color.White, 0, Vector2.Zero,
                   SpriteEffects.None, layerDepth + .01F);
            }

            base.Draw(gameTime, spriteBatch);
        }

        #endregion


        #region Functional Methods

        private void CreateNextEndPoint()
        {
            // Create a random number for X and Y
            Random r = new Random(UniqueId + DateTime.Now.Millisecond);
            Random r2 = new Random(UniqueId + DateTime.Now.Millisecond * 5);
            int x = r.Next(-300, 300);
            int y = r2.Next(-300, 300);

            // Set the start and end point for the path
            float endPathX = WorldPosition.X + x;
            float endPathY = WorldPosition.Y + y;

            if (endPathX > 1400) endPathX = 1390;
            if (endPathX < -1400) endPathX = -1390;
            if (endPathY > 1400) endPathY = 1390;
            if (endPathY < -1400) endPathY = -1390;

            _endPathPoint = new Vector2(endPathX, endPathY);
            _startPathPoint = new Vector2(WorldPosition.X, WorldPosition.Y);

            if (_endPathPoint.X < _startPathPoint.X)
                _isFacingRight = false;
            else
                _isFacingRight = true;
        }

        protected override void HandleCollision(ICollidable collider)
        {
            if (collider is Hero)
            {
                if ((collider as Hero).Hero_State == Hero.HeroState.Swording)
                {
                    // We are being attacked by the Hero, die!
                    Health--;
                }
            }
            if (collider is ShortRangeBullet)
            {
                Health--;
            }
            if (collider is BuckShot)
            {
                Health -= 5;
            }
            if (collider is UpgradedBuckshot)
            {
                Health -= 500;
            }
            if (collider is InnocentCat)
            {
                Cat newCat = SharedContext.EvolutionManager.ProduceNewCat(this, collider as Cat);
                //if(newCat != null)
                //    SharedContext.MovableEntityManager.Cats.Add(newCat);
            }
        }

        #endregion


    }
}
