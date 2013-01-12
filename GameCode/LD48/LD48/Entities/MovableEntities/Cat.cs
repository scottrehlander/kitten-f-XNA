using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD48
{
    public class Cat : MovableEntity
    {

        public enum GestationStateEnum
        {
            Breeding,
            Kitten,
            Grown
        }

        private GestationStateEnum _gestationState = GestationStateEnum.Grown;
        public GestationStateEnum GestationState 
        {
            get { return _gestationState; }
            set { _gestationState = value; }
        }

        private TimeSpan _timeToGrowUp = TimeSpan.FromSeconds(10);
        public TimeSpan TimeToGrowUp 
        {
            get { return _timeToGrowUp; }
        }
        
        private TimeSpan _timeAsKitten = TimeSpan.FromSeconds(0);
        public TimeSpan TimeAsKitten 
        {
            get { return _timeAsKitten; }
            set { _timeAsKitten = value; }
        }

        private TimeSpan _timeToBreed = TimeSpan.FromSeconds(5);
        public TimeSpan TimeToBreed
        {
            get { return _timeToBreed; }
            set { _timeToBreed = value; }
        }

        private TimeSpan _timeBreeding = TimeSpan.FromSeconds(0);
        public TimeSpan TimeBreeding
        {
            get { return _timeBreeding; }
            set { _timeBreeding = value; }
        }

        public Cat Offspring { get; set; }

        public virtual int Health { get; set; }

        private TimeSpan _timeUntilCanMateAgain = TimeSpan.FromSeconds(25);
        public TimeSpan TimeUntilCanMateAgain
        {
            get { return _timeUntilCanMateAgain; }
            set { _timeUntilCanMateAgain = value; }
        }

        private TimeSpan _sinceMated = TimeSpan.FromSeconds(500);
        public TimeSpan SinceMated
        {
            get { return _sinceMated; }
            set { _sinceMated = value; }
        }

        public virtual Texture2D KittenTexture { get; set; }
        public Texture2D CensoredTexture { get; set; }

        
        public Cat()
        {
            CensoredTexture = SharedContext.Content.Load<Texture2D>("Images/censored");
        }

    }
}
