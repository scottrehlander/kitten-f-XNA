using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LD48
{
    public class EvolutionManager
    {

        public Cat ProduceNewCat(Cat cat1, Cat cat2)
        {
            if (cat1.SinceMated < cat1.TimeUntilCanMateAgain ||
                cat2.SinceMated < cat2.TimeUntilCanMateAgain)
                return null;

            Cat newCat = null;
            if (cat1 is InnocentCat || cat2 is InnocentCat)
                newCat = new CatEvoLevel2();
            else if (cat1 is CatEvoLevel2 && cat2 is CatEvoLevel2)
                newCat = new CatEvoLevel3();
            else if(cat1 is CatEvoLevel3 && cat2 is CatEvoLevel3)
                newCat = new CatEvoLevel3();
            else if (cat1 is CatEvoLevel2 && cat2 is CatEvoLevel3)
                newCat = new CatEvoLevel3();
            else if (cat1 is CatEvoLevel3 && cat2 is CatEvoLevel2)
                newCat = new CatEvoLevel3();
            
            if (newCat != null)
            {
                newCat.WorldPosition = new Vector2(cat1.WorldPosition.X, cat1.WorldPosition.Y + 15);
                newCat.GestationState = Cat.GestationStateEnum.Kitten;
                newCat.SinceMated = TimeSpan.Zero;

                cat1.SinceMated = TimeSpan.Zero;
                cat2.SinceMated = TimeSpan.Zero;
                cat1.GestationState = Cat.GestationStateEnum.Breeding;
                cat2.GestationState = Cat.GestationStateEnum.Breeding;
                cat1.TimeBreeding = TimeSpan.Zero;
                cat2.TimeBreeding = TimeSpan.Zero;

                cat1.Offspring = newCat;

                return newCat;
            }

            return null;
        }

    }
}
