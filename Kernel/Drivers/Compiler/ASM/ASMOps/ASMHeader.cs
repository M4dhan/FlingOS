﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drivers.Compiler.ASM
{
    [ASMOpTarget(Target = OpCodes.Header)]
    public abstract class ASMHeader : ASMOp
    {
    }
}
