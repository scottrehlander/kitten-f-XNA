using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace LD48
{
    public static class SharedContext
    {

        public static GraphicsDevice GraphicsDevice { get; set; }

        public static SpriteBatch SpriteBatch { get; set; }

        public static ContentManager Content { get; set; }

        public static MovableEntityManager MovableEntityManager { get; set; }

        public static EvolutionManager EvolutionManager { get; set; }

        public static QuestProgressManager QuestProgressManager { get; set; }

        public static SoundEffectManager SoundEffectManager { get; set; }

    }
}
