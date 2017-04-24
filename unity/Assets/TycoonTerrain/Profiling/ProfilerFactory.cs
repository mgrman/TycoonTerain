﻿using System;

namespace TycoonTerrain.Common.Profiling
{
    public static class ProfilerFactory
    {
        public static Func<string, IProfiler> Factory { get; set; }

        public static IProfiler Create(string name)
        {
            return Factory(name);
        }
    }
}