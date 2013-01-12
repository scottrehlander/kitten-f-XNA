using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LD48
{
    public class CollisionManager
    {

        List<CollisionAssociation> _alreadyNotified = new List<CollisionAssociation>();

        int collisionCheckCount = 0;
        public void NotifyCollisions(List<ICollidable> collidables)
        {
            collisionCheckCount++;
            
            _alreadyNotified.Clear();

            foreach (ICollidable collider in collidables)
            {
                Rectangle rect1 = new Rectangle((int)(collider.WorldPosition.X + collider.CollisionBox.X),
                    (int)(collider.WorldPosition.Y + collider.CollisionBox.Y), (int)collider.CollisionBox.Width, (int)collider.CollisionBox.Height);
                foreach (ICollidable collider2 in collidables)
                {
                    if (collider.Equals(collider2)) continue;
                    if (AlreadyNotified(collider.GetUniqueEntityId(), collider2.GetUniqueEntityId())) 
                        continue;

                    Rectangle rect2 = new Rectangle((int)(collider2.WorldPosition.X + collider2.CollisionBox.X),
                        (int)(collider2.WorldPosition.Y + collider2.CollisionBox.Y), (int)collider2.CollisionBox.Width, (int)collider2.CollisionBox.Height);    
                    if (rect1.Intersects(rect2))
                    {
                        collider.CollisionDetected(collider2);
                        collider2.CollisionDetected(collider);

                        _alreadyNotified.Add(new CollisionAssociation()
                        {
                            Id1 = collider.GetUniqueEntityId(),
                            Id2 = collider2.GetUniqueEntityId()
                        });
                    }
                }
            }
        }

        private bool AlreadyNotified(int id1, int id2)
        {
            foreach (CollisionAssociation collision in _alreadyNotified)
            {
                if ((collision.Id1 == id1 && collision.Id2 == id2) ||
                    (collision.Id1 == id2 && collision.Id2 == id1))
                    return true;
            }

            return false;
        }

        struct CollisionAssociation
        {
            public int Id1;
            public int Id2;
        }

    }
}
