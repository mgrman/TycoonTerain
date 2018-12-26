using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage2i : IImage2iProvider, IEditableImage2i
    {
        private readonly Matrix2<Height1i> _editableMatrix;

        private readonly List<LockableMatrix2<Height1i>> _readonlyMatrices = new List<LockableMatrix2<Height1i>>();
        private Range2i? _invalidatedArea;
        private MatrixImage2i _image = null;

        private IImageConstraint2i _constraint;

        public EditableMatrixImage2i([InjectOptional] IImageConstraint2i constraint, IImageConfig imageConfig)
        {
            _constraint = constraint;
            _editableMatrix = new Matrix2<Height1i>(imageConfig.ImageSize.XY);
        }

        public IImage2i CreateImage()
        {
            if (_invalidatedArea == Range2i.Zero)
            {
                _image?.Dispose();
                _image = new MatrixImage2i(_image.Image, Range2i.Zero);
            }
            else if (_invalidatedArea.HasValue || _image == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.Size.ToRange2i();
                // Debug.LogFormat("Update readonlyCount:{0}", _readonlyMatrices.Count);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix2<Height1i>(_editableMatrix.Size);
                    _readonlyMatrices.Add(readonlyMatrix);
                }

                //sync
                _editableMatrix
                    .ForeachPointExlusive(i =>
                    {
                        readonlyMatrix[i] = _editableMatrix[i];
                    });

                // Debug.LogError($"_readonlyMatrices: {_readonlyMatrices.Count}");

                _image?.Dispose();
                _image = new MatrixImage2i(readonlyMatrix, _invalidatedArea.Value);
                _invalidatedArea = Range2i.Zero;
            }
            return _image;
        }

        public IEditableImageAccessor2i RequestAccess(Range2i areaRequest)
        {
            return new MatrixImageAccessor(this, areaRequest);
        }

        private void FixImage(Range2i invalidatedImageArea, Direction direction)
        {
            _invalidatedArea = _invalidatedArea?.CombineWith(invalidatedImageArea) ?? invalidatedImageArea;

            if (_constraint == null)
            {
                return;
            }

            var newInvalidatedImageArea = _constraint.FixImage(_editableMatrix, invalidatedImageArea, direction);
            _invalidatedArea = _invalidatedArea?.CombineWith(newInvalidatedImageArea) ?? newInvalidatedImageArea;
        }

        private class MatrixImageAccessor : IEditableImageAccessor2i
        {
            private readonly Height1i[,] _editableMatrix;
            private readonly EditableMatrixImage2i _editableImage;
            private Height1i.Difference _changeCounter = Height1i.Difference.Zero;

            public MatrixImageAccessor(EditableMatrixImage2i editableImage, Range2i area)
            {
                _editableMatrix = editableImage._editableMatrix.NativeMatrix;
                _editableImage = editableImage;
                Area = area.IntersectWith(editableImage._editableMatrix.Size.ToRange2i());
            }

            public Range2i Area { get; }

            public Height1i this[Vector2i pos]
            {
                get
                {
                    return _editableMatrix[pos.X, pos.Y];
                }
                set
                {
                    var existingValue = _editableMatrix[pos.X, pos.Y];
                    _changeCounter += value - existingValue;
                    _editableMatrix[pos.X, pos.Y] = value;
                }
            }

            public void Dispose()
            {
                this._editableImage.FixImage(Area, _changeCounter > Height1i.Difference.Zero ? Direction.Up : (_changeCounter < Height1i.Difference.Zero ? Direction.Down : Direction.Unknown));
            }
        }
    }
}