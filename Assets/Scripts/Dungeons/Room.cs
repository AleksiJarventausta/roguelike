
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueLike.Dungeons
{
    public class Room
    {
        private Rect Rect;

        public Room(Rect rect)
        {
            Rect = rect;
        }

        public Rect GetRect()
        {
            return Rect;
        }
    }
}
