using System.Linq;
using UnityEngine;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Images
{
    public class InitialStateSetter2f
    {
        public InitialStateSetter2f(IEditableImage2f editableImage, IInitialImageConfig imageConfig, IImageSampler2i sampler, [Inject(Id = "root")]GameObject root)
        {
            FillInitialState(editableImage, imageConfig, sampler, root);
        }

        public void FillInitialState(IEditableImage2f editableImage, IInitialImageConfig imageConfig, IImageSampler2i sampler, GameObject root)
        {
            if (editableImage == null)
                return;
            if (imageConfig.InitialData is Texture2D)
            {
                FillInitialState(editableImage, imageConfig.InitialData as Texture2D, imageConfig.InitialDataScale.Z);
            }
            if (imageConfig.InitialData is GameObject)
            {
                FillInitialState(editableImage, (imageConfig.InitialData as GameObject).GetComponentsInChildren<Collider>(), imageConfig.InitialDataScale.Z, sampler, root);
            }
            if (imageConfig.InitialData is Collider)
            {
                FillInitialState(editableImage, new[] { imageConfig.InitialData as Collider }, imageConfig.InitialDataScale.Z, sampler, root);
            }
            if (imageConfig.InitialData is IMatrix2<float>)
            {
                FillInitialState(editableImage, imageConfig.InitialData as IMatrix2<float>, imageConfig.InitialDataScale.Z);
            }
            if (imageConfig.InitialData is IMatrix3<float>)
            {
                FillInitialState(editableImage, imageConfig.InitialData as IMatrix3<float>, imageConfig.InitialDataScale.Z);
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, Texture2D texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                Range2i matrixAreaToFill;
                if (imageAccessor.Area == Range2i.All)
                {
                    matrixAreaToFill = new Vector2i(texture.width, texture.height).ToRange2i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }

                var matrixSizeX = matrixAreaToFill.Size.X;
                var matrixSizeY = matrixAreaToFill.Size.Y;

                matrixAreaToFill.ForeachPointExlusive(pos =>
                {
                    imageAccessor[pos] = texture.GetPixelBilinear((float)pos.X / matrixSizeX, (float)pos.Y / matrixSizeY).grayscale * scale;
                });
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, Collider[] colliders, float scale, IImageSampler2i sampler, GameObject root)
        {
            var bounds = colliders.Select(o => o.bounds)
                .Select(o => Range3f.FromMinAndSize(o.min.ToVector3f(), o.size.ToVector3f()))
                .DefaultIfEmpty(Range3f.zero)
                .Aggregate((a, b) => a.Encapsulate(b));

            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                var area = imageAccessor.Area;
                area.ForeachPointExlusive(i =>
                {
                    var localPos = sampler.ImageToWorld(i);

                    var ray = new Ray(root.transform.TransformPoint(new Vector3(localPos.X, localPos.Y, bounds.Max.Z)), root.transform.TransformDirection(new Vector3(0, 0, -1)));

                    imageAccessor[i] = colliders
                        .Select(collider =>
                        {
                            RaycastHit hit;
                            if (collider.Raycast(ray, out hit, bounds.Size.Z))
                            {
                                return Mathf.Max(0, bounds.Max.Z - hit.distance);
                            }
                            return 0;
                        })
                        .DefaultIfEmpty(0)
                        .Max() * scale;
                });
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, IMatrix2<float> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                Range2i matrixAreaToFill;
                if (imageAccessor.Area == Range2i.All)
                {
                    matrixAreaToFill = texture.Size.ToRange2i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }
                matrixAreaToFill.ForeachPointExlusive(i =>
                {
                    imageAccessor[i] = texture[i] * scale;
                });
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, IMatrix3<float> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                Range2i matrixAreaToFill;
                if (imageAccessor.Area == Range2i.All)
                {
                    matrixAreaToFill = texture.Size.XY.ToRange2i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }
                matrixAreaToFill.ForeachPointExlusive(i =>
                {
                    float value = 0;

                    for (int iz = texture.Size.Z - 1; iz >= 0; iz--)
                    {
                        var iTexture = new Vector3i(i.X, i.Y, iz);
                        if (texture[iTexture] > 0)
                        {
                            value = iz;
                            break;
                        }
                    }
                    imageAccessor[i] = value * scale;
                });

            }
        }
    }
}