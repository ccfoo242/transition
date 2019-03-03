﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easycoustics.Transition.Common
{
    public enum AxisScale { Logarithmic, Linear, dB };
    public enum dBReference { dBV, dBm, dBSPL };
    public enum Polarity { Bipolar, Positive, Negative };
    public enum InterpolationModes { NearestNeighbor, Linear, Quadratic, Cubic };
}
