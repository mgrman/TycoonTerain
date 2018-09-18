﻿using System.Collections.Generic;
using UniRx;
using Votyra.Core.Models;
using Votyra.Core.Painting.Commands;

namespace Votyra.Core.Painting
{
    public interface IPaintingModel
    {
        IBehaviorSubject<IReadOnlyList<IPaintCommand>> PaintCommands { get; }
        IBehaviorSubject<bool> Active { get; }
        IBehaviorSubject<IPaintCommand> SelectedPaintCommand { get; }
        IBehaviorSubject<int> Strength { get; }
        IBehaviorSubject<Vector2i> ImagePosition { get; }
    }
}