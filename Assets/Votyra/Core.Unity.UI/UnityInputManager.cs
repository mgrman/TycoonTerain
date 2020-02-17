using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Votyra.Core.InputHandling;
using Votyra.Core.Models;
using Votyra.Core.Painting;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Raycasting;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity.Painting
{
    public class UnityInputManager : MonoBehaviour
    {
        [Inject]
        protected IPaintingModel _paintingModel;

        [Inject]
        protected ITerrainConfig _terrainConfig;

        [Inject]
        protected IRaycaster _raycaster;

        private bool _processing;

        [Inject(Id = "root")]
        protected GameObject _root;

        private bool _invokeWithNull;
        private int _strength;

        private string _lastActiveCommand;
        private IPaintCommand _activeCommand;

        protected void Update()
        {
            if (_processing)
            {
                return;
            }

            _processing = true;
            var cmdName = GetActiveCommand();
            var activeCommand = _activeCommand;
            if (cmdName != _lastActiveCommand)
            {
                activeCommand?.Dispose();
                activeCommand = InstantiateCommand(cmdName);
                _activeCommand = activeCommand;
                _lastActiveCommand = cmdName;
            }

            if (activeCommand == null)
            {
                _processing = false;
                return;
            }
            
            _strength = GetMultiplier() * GetDistance();
            var ray = GetRayFromPointer(Input.mousePosition, Camera.main);
            if (_terrainConfig.AsyncInput)
            {
                Task.Run(() =>
                {
                    try
                    {
                        ProcessInputs(ray, activeCommand);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                    finally
                    {
                        _processing = false;
                    }
                });
            }
            else
            {
                ProcessInputs(ray, activeCommand);
                _processing = false;
            }
        }

        private IPaintCommand InstantiateCommand(string cmdName)
        {
            if (cmdName == null)
            {
                return null;
            }

            var factory = _paintingModel.PaintCommandFactories.FirstOrDefault(o => o.Action == cmdName);
            if (factory == null)
            {
                return null;
            }

            return factory.Create();
        }

        private static string GetActiveCommand()
        {
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                return null;
            }

            var isFlat = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var isHole = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            string cmdName;
            if (isFlat)
            {
                cmdName = KnownCommands.Flatten;
            }
            else if (isHole)
            {
                cmdName = KnownCommands.MakeOrRemoveHole;
            }
            else
            {
                cmdName = KnownCommands.IncreaseOrDecrease;
            }

            return cmdName;
        }

        private void ProcessInputs(Ray3f ray, IPaintCommand cmd)
        {
            var rayHit = _raycaster.Raycast(ray);

            if (rayHit.AnyNan())
            {
                return;
            }

            var cell = rayHit.XY()
                .RoundToVector2i();

            cmd.UpdateInvocationValues(cell, _strength);
        }

        private int GetMultiplier() => Input.GetMouseButton(1) ? -1 : 1;

        private int GetDistance() => (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) ? 3 : 1;

        private Ray3f GetRayFromPointer(Vector3 screenPosition, Camera camera)
        {
            var ray = camera.ScreenPointToRay(screenPosition);

            var cameraPosition = _root.transform.InverseTransformPoint(ray.origin)
                .ToVector3f();
            var cameraDirection = _root.transform.InverseTransformDirection(ray.direction)
                .ToVector3f();

            var cameraRay = new Ray3f(cameraPosition, cameraDirection);

            return cameraRay;
        }
    }
}