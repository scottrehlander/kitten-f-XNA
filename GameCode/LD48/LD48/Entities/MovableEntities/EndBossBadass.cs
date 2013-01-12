using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD48
{
    public class EndBossBadass : Cat
    {
        bool _isFacingRight = false;

        TimeSpan _sinceLastFireball = TimeSpan.Zero;
        TimeSpan _timeNeededBetweenFireballs = TimeSpan.FromSeconds(3);

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
                    fs.WorldPosition = new Vector2(WorldPosition.X + 150, WorldPosition.Y + 30);
                    SharedContext.MovableEntityManager.FloatingStatuses.Add(fs);
                }

                _health = value;
            }
        }

        public override void LoadContent()
        {
            Texture = SharedContext.Content.Load<Texture2D>("Images/EvilBossBadass");

            WorldPosition = new Vector2(-400, 100);

            CollisionBox = new Rectangle(35, 5, 250, 195);

            Health = 4500;

            base.LoadContent();
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {

            if (Health == 0)
            {
                SharedContext.MovableEntityManager.CurrentQuest = MovableEntityManager.QuestEnum.YouWin;
            }

            if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest3)
            {
                if (_sinceLastFireball < _timeNeededBetweenFireballs)
                {
                    _sinceLastFireball += gameTime.ElapsedGameTime;
                }

                // Choose whether or not we should shoot a fireball
                if (_sinceLastFireball > _timeNeededBetweenFireballs)
                {
                    BossFireball bf;

                    int direction = -1;
                    float startX = WorldPosition.X;

                    // Shoot a fireball
                    if (_isFacingRight)
                    {
                        direction = 1;
                        startX = WorldPosition.X + 270;
                    }

                    // Make 3 fireballs offset by a random y
                    Random randY = new Random(DateTime.Now.Millisecond);
                    int rand = randY.Next(0, 20);

                    bf = new BossFireball(new Vector2(direction, 0));
                    bf.WorldPosition = new Vector2(startX, WorldPosition.Y + 20 + rand);
                    SharedContext.MovableEntityManager.Ammo.Add(bf);

                    bf = new BossFireball(new Vector2(direction, 0));
                    bf.WorldPosition = new Vector2(startX, WorldPosition.Y + 80 + rand);
                    SharedContext.MovableEntityManager.Ammo.Add(bf);

                    bf = new BossFireball(new Vector2(direction, 0));
                    bf.WorldPosition = new Vector2(startX, WorldPosition.Y + 140 + rand);
                    SharedContext.MovableEntityManager.Ammo.Add(bf);

                    SharedContext.SoundEffectManager.PlayBossFire();

                    // After we shoot a fireball, let's pick a random time for the next fireball to be show
                    _timeNeededBetweenFireballs = TimeSpan.FromMilliseconds(randY.Next(800, 3500));

                    _sinceLastFireball = TimeSpan.Zero;
                }
            }

            if ((SharedContext.MovableEntityManager.Hero.WorldPosition.X - WorldPosition.X + 150 < 700 &&
                SharedContext.MovableEntityManager.Hero.WorldPosition.X - WorldPosition.X + 150 > -700)
                &&
                SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest3)
            {
                if(SharedContext.MovableEntityManager.Hero.WorldPosition.X - (WorldPosition.X + 150) < 10 &&
                SharedContext.MovableEntityManager.Hero.WorldPosition.X - (WorldPosition.X + 150) > -10)
                {
                    // Dont freak out if we are really close
                }
                else
                {
                    if (SharedContext.MovableEntityManager.Hero.WorldPosition.X > WorldPosition.X + 150)
                    {
                        _isFacingRight = true;
                        WorldPosition = new Vector2(WorldPosition.X + .5F, WorldPosition.Y);
                    }
                    else
                    {
                        _isFacingRight = false;
                        WorldPosition = new Vector2(WorldPosition.X - .5F, WorldPosition.Y);
                    }
                }
            }
            else
            {
                // I need to come up with a good roaming algorithm
                Random r = new Random();
                int switchDir = r.Next(0, 250);
                if (switchDir == 5)
                    _isFacingRight = !_isFacingRight;

                if (_isFacingRight)
                    WorldPosition = new Vector2(WorldPosition.X + .5F, WorldPosition.Y);
                else
                    WorldPosition = new Vector2(WorldPosition.X - .5F, WorldPosition.Y);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float layerDepth = ((WorldPosition.Y + 215) + 99999) * 100 / 100000000;

            spriteBatch.Draw(Texture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 300, 215), null, Color.White, 0, Vector2.Zero,
                _isFacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);

            base.Draw(gameTime, spriteBatch);
        }


        protected override void HandleCollision(ICollidable collider)
        {
            if (collider is Hero)
            {
                if ((collider as Hero).Hero_State == Hero.HeroState.Swording)
                {
                    // We are being attacked by the Hero, die!
                    Health -= 0;
                }
            }
            if (collider is ShortRangeBullet)
            {
                Health -= 0;
            }
            if (collider is BuckShot)
            {
                Health -= 0;
            }
            if (collider is UpgradedBuckshot)
            {
                Health -= 50;
            }
        }

    }
}
