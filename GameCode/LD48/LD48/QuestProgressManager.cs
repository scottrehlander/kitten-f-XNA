using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD48
{
    public class QuestProgressManager
    {

        public bool QuestCompleted { get; set; }

        private int _numberOfInnocentKittensKilled = 0;
        public int NumberOfInnocentKittensKilled 
        {
            get { return _numberOfInnocentKittensKilled; }
            set
            {
                _numberOfInnocentKittensKilled = value;

                if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest1)
                {
                    if (_numberOfInnocentKittensKilled == NumberOfInnocentKittensToFinishQuest)
                    {
                        QuestCompleted = true;

                        SharedContext.SoundEffectManager.PlayQuestComplete();
                    }
                }
            }
        }
        public int NumberOfInnocentKittensToFinishQuest { get { return 1; } }// return 25; } }

        private int _numberOfMutatedKittensKilled = 0;
        public int NumberOfMutatedKittensKilled
        {
            get { return _numberOfMutatedKittensKilled; }
            set
            {
                _numberOfMutatedKittensKilled = value;

                if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest2)
                {
                    if (_numberOfMutatedKittensKilled == NumberOfMutatedKittensToFinishQuest)
                    {
                        QuestCompleted = true;

                        SharedContext.SoundEffectManager.PlayQuestComplete();
                    }
                }
            }
        }
        public int NumberOfMutatedKittensToFinishQuest { get { return 1; } }// return 25; } }

    }
}
