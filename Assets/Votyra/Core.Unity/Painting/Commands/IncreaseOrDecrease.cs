using System;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Painting.Commands
{

    public class IncreaseOrDecrease : PaintCommand
    {
        protected override float Invoke(float value, int strength)
        {
            return value + strength;
        }
    }
}