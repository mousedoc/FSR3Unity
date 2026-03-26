using System.Collections;
using UnityEngine;
using FidelityFX.FSR3;
using System.Collections.Generic;

namespace FidelityFX
{
    [RequireComponent(typeof(Camera))]
    public class Fsr3UpscalerImageEffectSyncer : MonoBehaviour
    {
        public static HashSet<Fsr3UpscalerImageEffectSyncer> Instances { get; private set; } = new HashSet<Fsr3UpscalerImageEffectSyncer>();

        public bool isUI;

        private Camera _renderCamera;
        private Rect _originalRect;

        private void OnEnable()
        {
            ManualInit();

            Instances.Add(this);
        }

        private void OnDisable()
        {
            ResetCameraAspectRect();
        }

        private void OnDestroy()
        {
            Instances.Remove(this);
        }

        private void OnPreCull()
        {
            if (Fsr3UpscalerImageEffect.Enable == false)
            {
                return;
            }

            var upscaleRatio = Fsr3Upscaler.GetUpscaleRatioFromQualityMode(Fsr3UpscalerImageEffect.CurrentQualityMode);
            _renderCamera.aspect = (float)_renderCamera.pixelWidth / _renderCamera.pixelHeight;

            var widthRatio = _originalRect.width / upscaleRatio;
            var heightRatio = _originalRect.height / upscaleRatio;

            if (isUI)
            {
                _renderCamera.rect = new Rect(
                    _originalRect.x * widthRatio,
                    _originalRect.y * heightRatio,
                    _originalRect.width / widthRatio,
                    _originalRect.height / heightRatio
                );
            }
            else
            {
                _renderCamera.rect = new Rect(
                    _originalRect.x * widthRatio,
                    _originalRect.y * heightRatio,
                    _originalRect.width * widthRatio,
                    _originalRect.height * heightRatio
                );
            }
        }

        public void ManualInit()
        {
            _renderCamera = GetComponent<Camera>();
            _originalRect = _renderCamera.rect;
        }

        private void ResetCameraAspectRect()
        {
            _renderCamera.rect = _originalRect;
            _renderCamera.ResetProjectionMatrix();
        }
    }
}
