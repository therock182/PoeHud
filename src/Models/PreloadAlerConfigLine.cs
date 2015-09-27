﻿using System;
using SharpDX;

namespace PoeHUD.Models
{
    public class PreloadAlerConfigLine : ConfigLineBase
    {
        public Func<Color> FastColor;

        public string SoundFile { get; set; }
    }
}