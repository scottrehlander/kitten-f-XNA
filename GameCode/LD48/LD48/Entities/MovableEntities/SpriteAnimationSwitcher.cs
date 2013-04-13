using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD48
{
    public class SpriteAnimationHelper
    {
        public Dictionary<string, List<Texture2D>> Images { get; set; }
        public Dictionary<string, TimeSpan> TimeToSwitchImage { get; set; }

        private TimeSpan _timeSinceLastSwitch;
        private string _lastCollectionDrawn = "";
        int _currentImage = -1;

        public Texture2D GetNextImage(string fromCollection, GameTime timeElapsed)
        {
            _timeSinceLastSwitch += timeElapsed.ElapsedGameTime;

            // If we are starting a new collection, start from image 0
            if (_lastCollectionDrawn == string.Empty || fromCollection != _lastCollectionDrawn)
            {
                _currentImage = 0;
                _timeSinceLastSwitch = TimeSpan.FromSeconds(0);
            }

            if (_timeSinceLastSwitch > TimeToSwitchImage[fromCollection])
            {

                // If we are going to increment past the number of images, set to -1
                if (_currentImage + 1 >= Images[fromCollection].Count())
                {
                    _currentImage = -1;
                }

                _currentImage++;
                _timeSinceLastSwitch = TimeSpan.FromSeconds(0);
            }

            _lastCollectionDrawn = fromCollection;

            // Increment the image if the timespan has elapsed for this collection of images
            return Images[fromCollection][_currentImage];
        }

    }
}
