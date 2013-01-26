using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD48
{
    public interface IsActionable
    {
        Rectangle EnterableArea { get; }
        string EnterMessage { get; }
        Action ActionToExecute { get; }
    }
}
