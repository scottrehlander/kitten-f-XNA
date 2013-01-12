using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD48
{
    public class CatEvoLevel2 : Cat
    {
        #region Variables
        
        protected bool _isFacingRight = true;

        // Determine a new path
        Vector2 _endPathPoint = new Vector2(int.MinValue, int.MinValue);
        Vector2 _startPathPoint = new Vector2(int.MinValue, int.MinValue);

        // Manage the amount of time it takes for a path to complete
        TimeSpan _timeUntilNextMove = TimeSpan.FromMilliseconds(1.5);
        TimeSpan _timeSinceLastMove = TimeSpan.Zero;
        int _clicksToGetThere = 220;
        int _clicksOnCurrentPath = 0;

        TimeSpan _sinceHitByHero = TimeSpan.MaxValue;
        TimeSpan _timeStunnedAfterAttack = TimeSpan.FromMilliseconds(500);

        TimeSpan _sinceCollidedWithHero = TimeSpan.FromSeconds(10);
        TimeSpan _timeToStopAfterCollidingWithHero = TimeSpan.FromMilliseconds(500);

        double _moveSpeed = 2F;

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


        public CatEvoLevel2()
        {
            LoadContent();
        }


        #region XNA Methods

        public override void LoadContent()
        {
            Texture = SharedContext.Content.Load<Texture2D>("Images/catEvoLevel2");
            KittenTexture = SharedContext.Content.Load<Texture2D>("Images/innocentCat1_color");

            CollisionBox = new Rectangle(15, 10, 46, 35);

            LineOfSite = new Rectangle(-120, -120, 300, 300);

            _health = 50;

            base.LoadContent();
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {

            if (GestationState == Cat.GestationStateEnum.Kitten)
            {
                // Elapse the time as a kitten
                if (TimeAsKitten <= TimeToGrowUp)
                    TimeAsKitten += gameTime.ElapsedGameTime;
                else
                    GestationState = GestationStateEnum.Grown;
            }
            else if (GestationState == GestationStateEnum.Breeding)
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

            if (_sinceHitByHero <= _timeStunnedAfterAttack)
                _sinceHitByHero += gameTime.ElapsedGameTime;

            // Don't go crazy if we are already in the Hero's face
            if (_sinceCollidedWithHero <= _timeToStopAfterCollidingWithHero)
                _sinceCollidedWithHero += gameTime.ElapsedGameTime;
            if (_sinceCollidedWithHero < _timeToStopAfterCollidingWithHero)
                return;

            if (_sinceHitByHero < _timeStunnedAfterAttack)
                return;

            Hero hero = SharedContext.MovableEntityManager.Hero;

            // If the hero is within a specific distance, move towards him
            if ((new Rectangle((int)(WorldPosition.X + LineOfSite.X), (int)(WorldPosition.Y + LineOfSite.Y),
                LineOfSite.Width, LineOfSite.Height)
                .Intersects(new Rectangle((int)(hero.WorldPosition.X + hero.CollisionBox.X), (int)(hero.WorldPosition.Y + hero.CollisionBox.Y),
                    hero.CollisionBox.Width, hero.CollisionBox.Height))) &&
                GestationState != GestationStateEnum.Kitten)
            {
                // Determine how to get there

                // _______
                // \     |
                //   \   |
                //     \ |

                // Grab the distacnes to the target
                float distanceToTargetX = hero.WorldPosition.X - WorldPosition.X;
                float distanceToTargetY = hero.WorldPosition.Y - WorldPosition.Y;

                double totalDistanceToTravel = Math.Sqrt(Math.Pow(distanceToTargetX, 2) + Math.Pow(distanceToTargetY, 2));

                // Find the ratio of the total distance to the speed value and apply that ratio to x and y
                double ratio = _moveSpeed / totalDistanceToTravel;

                //Vector2 moveVector = new Vector2((float)(distanceToTargetX * ratio), (float)(distanceToTargetY * ratio));
                
                // Add the moveVector to our WorldPosition
                WorldPosition = new Vector2(WorldPosition.X + (float)(distanceToTargetX * ratio),
                    WorldPosition.Y + (float)(distanceToTargetY * ratio));

                // Set the direction the cat is facing
                if (distanceToTargetX > 0)
                    _isFacingRight = true;
                else
                    _isFacingRight = false;

                // Reset the roaming path
                _endPathPoint.X = int.MinValue;
            }
            else
            {
                // We don't see the hero
                

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
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float layerDepth = ((WorldPosition.Y + 27) + 99999) * 100 / 100000000;

            if (GestationState == GestationStateEnum.Kitten)
            {
                spriteBatch.Draw(Texture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 35, 25), null, Color.White, 0, Vector2.Zero,
                       !_isFacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
            }
            else
            {
                spriteBatch.Draw(Texture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 80, 55), null, Color.White, 0, Vector2.Zero,
                       !_isFacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
            }

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
                    _sinceHitByHero = TimeSpan.Zero;

                    ApplyKnockback();
                }

                _sinceCollidedWithHero = TimeSpan.Zero;
            }
            if (collider is ShortRangeBullet)
            {
                Health--;
                _sinceHitByHero = TimeSpan.Zero;

                ApplyKnockback();
            }
            if (collider is BuckShot)
            {
                Health -= 5;
                _sinceHitByHero = TimeSpan.Zero;

                ApplyKnockback();
            }
            if (collider is UpgradedBuckshot)
            {
                Health -= 500;
            }
            if (collider is CatEvoLevel2 || collider is CatEvoLevel3)
            {
                Cat newCat = SharedContext.EvolutionManager.ProduceNewCat(this, collider as Cat);
            }
        }

        private void ApplyKnockback()
        {
            // Knockback
            if (SharedContext.MovableEntityManager.Hero.IsFacingRight)
            {
                if(WorldPosition.X + 20 < 1610)
                    WorldPosition = new Vector2(WorldPosition.X + 20, WorldPosition.Y);
            }
            else
            {
                if (WorldPosition.X - 20 > -1515)
                    WorldPosition = new Vector2(WorldPosition.X - 20, WorldPosition.Y);
            }
        }

        #endregion

    }
}
