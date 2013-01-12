using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LD48
{
    public interface ICollidable
    {
        void CollisionDetected(ICollidable collider);

        Rectangle CollisionBox { get; set; }

        Vector2 WorldPosition { get; set; }

        int GetUniqueEntityId();
    }
}
