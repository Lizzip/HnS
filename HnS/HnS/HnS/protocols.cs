﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HnS
{
    public enum Protocol
    {
        Disconnected = 0,
        Connected = 1,
        PlayerMoved = 2,
        PlayerAnimationState = 3,
        KeyPressDown = 4
    }
}
