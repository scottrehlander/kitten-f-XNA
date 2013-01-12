using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace LD48
{
    public class SoundEffectManager
    {

        Dictionary<string, SoundEffectInstance> Instances { get; set; }
        Dictionary<string, SoundEffect> Effects { get; set; }

        public SoundEffectManager()
        {
            Instances = new Dictionary<string, SoundEffectInstance>();
            Effects = new Dictionary<string, SoundEffect>();
        }

        public void LoadContent()
        {
            Effects.Add("SwordAttackSound", SharedContext.Content.Load<SoundEffect>("Audio/SwordSwing"));
            Instances.Add("SwordAttackSound", Effects["SwordAttackSound"].CreateInstance());
            Instances["SwordAttackSound"].Volume = .1F;

            Effects.Add("GunAttackSound", SharedContext.Content.Load<SoundEffect>("Audio/GunShot14"));
            Instances.Add("GunAttackSound", Effects["GunAttackSound"].CreateInstance());
            Instances["GunAttackSound"].Volume = .3F;

            Effects.Add("BossFire", SharedContext.Content.Load<SoundEffect>("Audio/BossFire"));
            Instances.Add("BossFire", Effects["BossFire"].CreateInstance());
            Instances["BossFire"].Volume = .3F;

            Effects.Add("QuestComplete", SharedContext.Content.Load<SoundEffect>("Audio/QuestComplete"));
            Instances.Add("QuestComplete", Effects["QuestComplete"].CreateInstance());
            Instances["QuestComplete"].Volume = .3F;

            Effects.Add("PlayerHurt", SharedContext.Content.Load<SoundEffect>("Audio/Hit_Hurt11"));
            Instances.Add("PlayerHurt", Effects["PlayerHurt"].CreateInstance());
            Instances["PlayerHurt"].Volume = .3F;

            Effects.Add("Fart", SharedContext.Content.Load<SoundEffect>("Audio/fart"));
            Instances.Add("Fart", Effects["Fart"].CreateInstance());
            Instances["Fart"].Volume = 1F;

        }

        public void PlaySwordAttack()
        {
            Instances["SwordAttackSound"].Play();
        }

        public void PlayGunShot()
        {
            Instances["GunAttackSound"].Play();
        }

        public void PlayBossFire()
        {
            Instances["BossFire"].Play();
        }

        public void PlayQuestComplete()
        {
            Instances["QuestComplete"].Play();
        }

        public void PlayPlayerHurt()
        {
            Instances["PlayerHurt"].Play();
        }

        public void PlayFart()
        {
            Instances["Fart"].Play();
        }
    }
}
