﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;


public class GroupVisibilityOptions:IDisposable
{
    
    public readonly GameObject ParentContainer;
    public readonly Camera Camera;
    public readonly Matrix4x4 CameraTransform;
    public readonly Bounds GroupBounds;
    public readonly Range2 RangeZ;


    public readonly Vector2i CellInGroupCount;
    private readonly IImage2 Image;

    public GroupVisibilityOptions(TerainGeneratorBehaviour terainGenerator)
    {
        this.Camera = Camera.main;
        this.CameraTransform = this.Camera.cameraToWorldMatrix;
        this.ParentContainer = terainGenerator.gameObject;
        this.Image = terainGenerator.Image as IImage2;
        this.RangeZ = Image.RangeZ;
        this.CellInGroupCount = terainGenerator.CellInGroupCount;

        this.GroupBounds = new Bounds(new Vector3(CellInGroupCount.x / 2.0f, CellInGroupCount.y / 2.0f, RangeZ.Center), new Vector3(CellInGroupCount.x, CellInGroupCount.y, RangeZ.Size));
    }

    public bool IsChanged(GroupVisibilityOptions old)
    {
        //TODO
        //not complete check!
        return old == null ||
            this.ParentContainer != old.ParentContainer ||
            this.RangeZ!=old.RangeZ ||
            this.Camera != old.Camera ||
            this.CameraTransform != old.CameraTransform;

    }
    
    public bool IsValid
    {
        get
        {
            return  this.ParentContainer != null;
        }
    }

    public void Dispose()
    {

    }
}
