﻿using System;
using Zenject;

namespace Votyra.Core
{
    [AttributeUsage(AttributeTargets.Constructor
           | AttributeTargets.Method | AttributeTargets.Parameter
           | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ConfigInjectAttribute : InjectOptionalAttribute
    {
        public ConfigInjectAttribute(string id)
        {
            Id = id;
        }
    }
}