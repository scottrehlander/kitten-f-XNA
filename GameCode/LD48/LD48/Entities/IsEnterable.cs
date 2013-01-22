using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD48
{
    public interface IsEnterable
    {
        Rectangle EnterableArea { get; }
        string EnterMessage { get; }
    }
}
