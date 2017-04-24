﻿using TycoonTerrain.Images;
using UnityEngine;

namespace TycoonTerrain.Unity.Images
{
    internal class CombineImageBehaviour : MonoBehaviour, IImage2iProvider
    {
        private MonoBehaviour _oldImageA;
        public MonoBehaviour ImageA;
        
        private MonoBehaviour _oldImageB = null;
        public MonoBehaviour ImageB;

        private CombineImage.Operations _oldOperation = CombineImage.Operations.Add;
        public CombineImage.Operations Operation;


        private IImage2i _image = null;
        public IImage2i Image
        {
            get
            {
                SetImage();
                return _image;
            }
        }

        private bool _fieldsChanged = true;

        private void Start()
        {
            SetImage();
        }

        private void Update()
        {
            _fieldsChanged = false;
            if (_oldImageA != ImageA)
            {
                _oldImageA = ImageA;
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_oldImageB != ImageB)
            {
                _oldImageB = ImageB;
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_oldOperation != Operation)
            {
                _oldOperation = Operation;
                _fieldsChanged = _fieldsChanged || true;
            }
            if (_fieldsChanged)
            {
                SetImage();
            }
        }

        private void SetImage()
        {
            _image = new CombineImage((ImageA as IImage2iProvider).Image, (ImageB as IImage2iProvider).Image,Operation);
        }
    }
}