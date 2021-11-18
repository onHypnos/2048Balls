using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = System.Random;

namespace Core
{
    public class DesignController : MonoBehaviour
    {
        public static DesignController Current;
        [Header("Rainbow offset delta")][SerializeField] private Material _rainbowMaterial;

        [Header("Enviorment settings")] 
        [SerializeField] private MeshRenderer _envieromentGO;
        [SerializeField] private List<Material> _envieromentList;
        [Header("RoadSettings")]
        [SerializeField] private bool _needMovingRoad;
        [SerializeField] private Material _roadMaterial;
        [SerializeField] private List<Material> _arrowMaterials;
        [SerializeField] private MMFeedbacks _mm;
        
        private Vector2 tempDelta = Vector2.one * 0.01f;
        private Vector2 tempRoadDelta = Vector2.up * 0.01f;

        
        private void Awake()
        {
            Current = this;
            GameEvents.Current.OnLevelLoaded += UpdateEnvieromentMaterials;
            UpdateEnvieromentMaterials();
        }

        public Material GetRoadMaterial()
        {
            return _roadMaterial;
        }

        private void UpdateEnvieromentMaterials()
        {
            int i = UnityEngine.Random.Range(0, _envieromentList.Count);
            _envieromentGO.material = _envieromentList[i];
            _roadMaterial = _arrowMaterials[i];
        }


        private void FixedUpdate()
        {
            _rainbowMaterial.mainTextureOffset += tempDelta;
            if (_rainbowMaterial.mainTextureOffset.y >= 1)
            {
                _rainbowMaterial.mainTextureOffset = Vector2.zero;
            }

            if (_needMovingRoad)
            {
                _roadMaterial.mainTextureOffset -= tempRoadDelta;
                if (_roadMaterial.mainTextureOffset.y <= -1)
                {
                    _roadMaterial.mainTextureOffset = Vector2.zero;
                }
            }
        }
    }
}