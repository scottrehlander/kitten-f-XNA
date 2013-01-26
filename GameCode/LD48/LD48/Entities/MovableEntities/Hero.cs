using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD48
{
    public class Hero : MovableEntity
    {

        #region Variables

        public enum HeroWeaponEnum
        {
            Sword = 0,
            ShotGun = 1,
            UpgradedShotGun
        }
        public HeroWeaponEnum HeroWeapon { get; set; }
              

        // Keep the state of the hero
        public enum HeroState
        {
            Idle,
            Swording,
            Walking
        }
        HeroState _heroState = HeroState.Idle;
        public HeroState Hero_State { get { return _heroState; } }
        
        // Maintain the camera in this class
        Camera _camera;
        public Camera Camera { get { return _camera; } set { _camera = value; } }

        // The speed that that the char moves at
        //int _moveSpeed = 10;
        int _moveSpeed = 4;

        // The direction we are facing
        bool _isFacingRight = true;
        public bool IsFacingRight { get { return _isFacingRight; } }

        // Sprite animation objects
        List<Texture2D> _swordAnimationTextures = new List<Texture2D>();
        TimeSpan _swordAnimationTimer = TimeSpan.Zero;
        int _swordAnimationIndex = 0;
        bool _swordingInProgress = false;

        Texture2D _shotGunTexture;
        List<Texture2D> _shotgunTextures = new List<Texture2D>();

        // Don't switch weapons too fast
        bool _weaponSwitchButtonDown = false;

        // Disallowing the spamming of bullets
        TimeSpan _sinceLastBullet = TimeSpan.Zero;
        TimeSpan _timeForNextBulletAllowe = TimeSpan.FromMilliseconds(150);
        
        // Variables for Health..  There is a framework setup here to ignore collisions with the same cat within
        //  a specified period
        int _startingHealth = 5;
        public int Health { get; set; }
        List<InvulnerableToCatTimer> _invulnerableToCatsList = new List<InvulnerableToCatTimer>();
        TimeSpan _invulnerableCatTimeout = TimeSpan.FromSeconds(1);
        TimeSpan _sinceHit = TimeSpan.FromSeconds(20);
        TimeSpan _timeStunnedAfterHit = TimeSpan.FromSeconds(.3);
        
        #endregion


        public Hero(Camera camera)
        {
            // The cam will move with the hero
            _camera = camera;

            WorldPosition = new Vector2(SharedContext.GraphicsDevice.Viewport.Width / 2, 
                SharedContext.GraphicsDevice.Viewport.Height / 2);

            Health = _startingHealth;
        }
        

        #region XNA Methods

        public override void LoadContent()
        {
            // Load the Hero Textures
            Texture = SharedContext.Content.Load<Texture2D>("Images/hero1_color");

            _swordAnimationTextures.Add(SharedContext.Content.Load<Texture2D>("Images/hero1_color"));
            _swordAnimationTextures.Add(SharedContext.Content.Load<Texture2D>("Images/heroSword1_color"));
            _swordAnimationTextures.Add(SharedContext.Content.Load<Texture2D>("Images/heroSword2_color"));

            _shotGunTexture = SharedContext.Content.Load<Texture2D>("Images/HeroFt");
            _shotgunTextures.Add(SharedContext.Content.Load<Texture2D>("Images/HeroFt"));
            _shotgunTextures.Add(SharedContext.Content.Load<Texture2D>("Images/HeroFtShooting"));

            CollisionBox = new Rectangle(10, 7, 25, 37);

            base.LoadContent();
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {


            // Remember some states as we check the keys that were pressed
            bool isSwording = false;
            bool isWalking = false;

            if (_sinceHit > _timeStunnedAfterHit)
            {

                // Check the state of the keyboard and manipulate the hero position and the 
                if (SharedContext.InputManager.MoveLeftPressed)
                {
                    if (WorldPosition.X - _moveSpeed > -1525)
                    {
                        // Left
                        WorldPosition = new Vector2(WorldPosition.X - _moveSpeed, WorldPosition.Y);
                        _camera.Position = new Vector2(_camera.Position.X - _moveSpeed, _camera.Position.Y);

                        _isFacingRight = false;

                        isWalking = true;
                    }
                }
                if (SharedContext.InputManager.MoveRightPressed)
                {
                    if (WorldPosition.X + _moveSpeed < 1650 - 35)
                    {
                        // Right
                        WorldPosition = new Vector2(WorldPosition.X + _moveSpeed, WorldPosition.Y);
                        _camera.Position = new Vector2(_camera.Position.X + _moveSpeed, _camera.Position.Y);

                        _isFacingRight = true;

                        isWalking = true;
                    }
                }
                if (SharedContext.InputManager.MoveUpPressed)
                {
                    if (WorldPosition.Y - _moveSpeed > -1500 - 10)
                    {
                        // Up
                        WorldPosition = new Vector2(WorldPosition.X, WorldPosition.Y - _moveSpeed);
                        _camera.Position = new Vector2(_camera.Position.X, _camera.Position.Y - _moveSpeed);

                        isWalking = true;
                    }
                }
                if (SharedContext.InputManager.MoveDownPressed)
                {
                    if (WorldPosition.Y + _moveSpeed < 1650 - 45)
                    {
                        // Down
                        WorldPosition = new Vector2(WorldPosition.X, WorldPosition.Y + _moveSpeed);
                        _camera.Position = new Vector2(_camera.Position.X, _camera.Position.Y + _moveSpeed);

                        isWalking = true;
                    }
                }

                // Attacking
                if (SharedContext.InputManager.AttackPressed)
                {
                    isSwording = true;
                }
            }


            // Zooming ( should we disable )?
            if (SharedContext.InputManager.ZoomOutPressed)
            {
                if(_camera.Zoom >.5F)
                    _camera.Zoom -= .05F;
            }
            if (SharedContext.InputManager.ZoomInPressed)
            {
                if(_camera.Zoom < 3)
                    _camera.Zoom += .05F;
            }

            if (SharedContext.InputManager.SwitchWeaponPressed)
            {
                if (_weaponSwitchButtonDown == false)
                {
                    // If we are on the first quest, don't allow the 2nd or third gun
                    if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.PreQuest ||
                        SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest1)
                    {
                        HeroWeapon = HeroWeaponEnum.Sword;
                    }
                    else if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest2)
                    {
                        if (HeroWeapon == HeroWeaponEnum.ShotGun)
                            HeroWeapon = HeroWeaponEnum.Sword;
                        else
                            HeroWeapon++;
                    }
                    else
                    {
                        if (HeroWeapon == HeroWeaponEnum.UpgradedShotGun)
                            HeroWeapon = HeroWeaponEnum.Sword;
                        else
                            HeroWeapon++;
                    }

                    _swordAnimationIndex = 0;
                }

                _weaponSwitchButtonDown = true;
            }
            else
            {
                _weaponSwitchButtonDown = false;
            }

            if (isSwording)
            {
                // Create a bullet for a better attack area
                if (_sinceLastBullet > _timeForNextBulletAllowe)
                {
                    // Determine the direction
                    Vector2 _bulletDirection;
                    if (_isFacingRight)
                        _bulletDirection = new Vector2(1, 0);
                    else
                        _bulletDirection = new Vector2(-1, 0);

                    // Create the bullet object
                    if (HeroWeapon == HeroWeaponEnum.Sword)
                    {
                        ShortRangeBullet srb = new ShortRangeBullet(_bulletDirection);
                        srb.WorldPosition = new Vector2(WorldPosition.X + 5, WorldPosition.Y);
                        SharedContext.MovableEntityManager.Ammo.Add(srb);
                    }
                    else if (HeroWeapon == HeroWeaponEnum.ShotGun)
                    {
                        BuckShot bs = new BuckShot(_bulletDirection);
                        bs.WorldPosition = new Vector2(WorldPosition.X + 5, WorldPosition.Y);
                        SharedContext.MovableEntityManager.Ammo.Add(bs);
                    }
                    else if (HeroWeapon == HeroWeaponEnum.UpgradedShotGun)
                    {
                        UpgradedBuckshot bs = new UpgradedBuckshot(_bulletDirection);
                        bs.WorldPosition = new Vector2(WorldPosition.X + 5, WorldPosition.Y);
                        SharedContext.MovableEntityManager.Ammo.Add(bs);
                    }          
                    
                    _sinceLastBullet = TimeSpan.Zero;
                }
                               

                // We are attacking
                ChangeHeroState(HeroState.Swording);
            }
            else if (isWalking)
            {
                // We are walking
                ChangeHeroState(HeroState.Walking);
            }
            else
            {
                // We aren't doing anything
                ChangeHeroState(HeroState.Idle);
            }
            
            // Check for invulnerable cats and remove them
            List<InvulnerableToCatTimer> _catsToRemove = new List<InvulnerableToCatTimer>();
            foreach (InvulnerableToCatTimer invulnerableCat in _invulnerableToCatsList)
            {
                invulnerableCat.TimeSinceHurtUs += gameTime.ElapsedGameTime;
                if (invulnerableCat.TimeSinceHurtUs > _invulnerableCatTimeout)
                    _catsToRemove.Add(invulnerableCat);
            }
            foreach (InvulnerableToCatTimer cat in _catsToRemove)
                _invulnerableToCatsList.Remove(cat);

            // Increment _sinceHit
            if(_sinceHit < _timeStunnedAfterHit)
                _sinceHit += gameTime.ElapsedGameTime;

            // Icrement since last bullet
            if (_sinceLastBullet < _timeForNextBulletAllowe)
                _sinceLastBullet += gameTime.ElapsedGameTime;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D _heroTexture;
            if (HeroWeapon == HeroWeaponEnum.Sword)
                _heroTexture = Texture;
            else
                _heroTexture = _shotGunTexture;

            // If we are swording, increment our animation
            if (_heroState == HeroState.Swording || _swordingInProgress)
            {
                List<Texture2D> textureList;
                if (HeroWeapon == HeroWeaponEnum.Sword)
                {
                    textureList = _swordAnimationTextures;

                    if (_swordAnimationIndex == 1)
                        SharedContext.SoundEffectManager.PlaySwordAttack();
                }
                else
                {
                    textureList = _shotgunTextures;

                    if (_swordAnimationIndex == 1)
                        SharedContext.SoundEffectManager.PlayGunShot();
                }

                _swordingInProgress = true;

                // Limit the amount of time for the animation
                if (_swordAnimationTimer > TimeSpan.FromSeconds(.1F))
                {
                    _swordAnimationIndex++;

                    // If we surpassed the index limit, reset to 0
                    if (_swordAnimationIndex >= textureList.Count)
                    {
                        _swordingInProgress = false;
                        _swordAnimationIndex = 0;
                    }

                    _swordAnimationTimer = TimeSpan.Zero;
                }

                // Set the texture to the animation texture
                try
                {
                    _heroTexture = textureList[_swordAnimationIndex];
                }
                catch { }

                _swordAnimationTimer += gameTime.ElapsedGameTime;
            }
            else
            {
                _swordAnimationIndex = 0;
                _swordAnimationTimer = TimeSpan.Zero;
            }

            // Calculate the layerdepth
            // x/100 = .Y / 100000
            float layerDepth = ((WorldPosition.Y + 50) + 99999) * 100 / 100000000;

            // Draw the texture
            spriteBatch.Draw(_heroTexture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 45, 50), null, Color.White, 0, Vector2.Zero, 
                !_isFacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);

            base.Draw(gameTime, spriteBatch);
        }

        #endregion


        #region Functional Methods

        private void ChangeHeroState(HeroState newState)
        {
            if(_heroState != newState)
                _heroState = newState;
        }

        public void RestoreHealth()
        {
            Health = _startingHealth;
        }

        public void SwitchToWeaponForQuest(LD48.MovableEntityManager.QuestEnum quest)
        {
            if (quest == MovableEntityManager.QuestEnum.Quest2)
            {
                HeroWeapon = HeroWeaponEnum.ShotGun;
            }
            if (quest == MovableEntityManager.QuestEnum.Quest3)
            {
                HeroWeapon = HeroWeaponEnum.UpgradedShotGun;
            }
        }

        #endregion


        #region Collision stuff

        protected override void  HandleCollision(ICollidable collider)
        {
            if (collider is Cat)
            {
                Cat collisionCat = (collider as Cat);

                // Make the cat invulnerable for a little white
                foreach (InvulnerableToCatTimer cat in _invulnerableToCatsList)
                    if (cat.EntityId.Equals(cat.EntityId))
                        return;
                
                if (Hero_State == HeroState.Swording)
                {
                    // If we are facing the direction of our enemy while swording, return
                    if (_isFacingRight && collisionCat.WorldPosition.X > WorldPosition.X)
                        return;
                    if (!_isFacingRight && collisionCat.WorldPosition.X < WorldPosition.X)
                        return;
                }
                
                // We aren't swording or there is a cat in the opposite direction attacking us
                if (collisionCat is InnocentCat)
                {
                    Health--;
                    SharedContext.SoundEffectManager.PlayPlayerHurt();

                    CreateFloatingStatus(1);
                }
                if (collisionCat is CatEvoLevel2)
                {
                    Health -= 2;
                    SharedContext.SoundEffectManager.PlayPlayerHurt();

                    CreateFloatingStatus(2);
                }
                if (collisionCat is CatEvoLevel3)
                {
                    Health -= 2;
                    SharedContext.SoundEffectManager.PlayPlayerHurt();

                    CreateFloatingStatus(2);
                }
                if (collisionCat is EndBossBadass)
                {
                    Health -= 2;
                    SharedContext.SoundEffectManager.PlayPlayerHurt();

                    CreateFloatingStatus(2);
                }

                _sinceHit = TimeSpan.FromSeconds(0);

                if (collisionCat.WorldPosition.X > WorldPosition.X)
                {
                    if (WorldPosition.X - 14 > -1515)
                    {
                        WorldPosition = new Vector2(WorldPosition.X - 14, WorldPosition.Y);
                        _camera.Position = new Vector2(_camera.Position.X - 14, _camera.Position.Y);
                    }
                }
                else
                {
                    if (WorldPosition.X + 14 < 1610)
                    {
                        WorldPosition = new Vector2(WorldPosition.X + 14, WorldPosition.Y);
                        _camera.Position = new Vector2(_camera.Position.X + 14, _camera.Position.Y);
                    }
                }
                
                _invulnerableToCatsList.Add(new InvulnerableToCatTimer() { EntityId = collider.GetUniqueEntityId() });

                
            }
            if (collider is BossFireball)
            {
                Health--;

                CreateFloatingStatus(1);

                SharedContext.SoundEffectManager.PlayPlayerHurt();
            }
        }

        private void CreateFloatingStatus(int amount)
        {
            FloatingStatus fs = new FloatingStatus("-" + amount.ToString(), Color.Red);
            fs.WorldPosition = new Vector2(WorldPosition.X, WorldPosition.Y - 15);
            SharedContext.MovableEntityManager.FloatingStatuses.Add(fs);
        }

        #endregion


        #region Helper Classes

        private class InvulnerableToCatTimer
        {
            public InvulnerableToCatTimer()
            {
                TimeSinceHurtUs = TimeSpan.FromSeconds(0);
            }

            public int EntityId { get; set; }
            public TimeSpan TimeSinceHurtUs { get; set; }
        }

        #endregion

    }
}
