/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Enums;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;

namespace InfinityCode.uPano.Tours
{
    [Serializable]
    public class TourItem: MonoBehaviour
    {
        public Vector2 position;

        [NonSerialized]
        private PanoRenderer _panoRenderer;

        [NonSerialized]
        private Tour _tour;

        private List<TourItem> _outLinks;

        private HotSpotManager _hotSpotManager;
        private DirectionManager _directionManager;

        public DirectionManager directionManager
        {
            get { return _directionManager; }
        }

        public HotSpotManager hotSpotManager
        {
            get { return _hotSpotManager; }
        }

        public List<TourItem> outLinks
        {
            get
            {
                if (_outLinks == null) UpdateOutLinks();
                return _outLinks;
            }
        }

        public PanoRenderer panoRenderer
        {
            get { return _panoRenderer; }
        }

        public Tour tour
        {
            get { return _tour; }
            set { _tour = value; }
        }

        public Texture texture
        {
            get
            {
                ISingleTexturePanoRenderer singleTexturePanoRenderer = panoRenderer as ISingleTexturePanoRenderer;
                if (singleTexturePanoRenderer != null) return singleTexturePanoRenderer.texture;
                return null;
            }
            set
            {
                ISingleTexturePanoRenderer singleTexturePanoRenderer = panoRenderer as ISingleTexturePanoRenderer;
                if (singleTexturePanoRenderer != null) singleTexturePanoRenderer.texture = value;
            }
        }

        public void Init()
        {
            _panoRenderer = GetComponent<PanoRenderer>();
            _hotSpotManager = GetComponent<HotSpotManager>();
            _directionManager = GetComponent<DirectionManager>();
            UpdateOutLinks();
        }

        public static TourItem Create(Tour tour, Texture texture = null)
        {
            int index = tour.items.Count;
            string name;

            do
            {
                name = "Item " + index;
                index++;
            } while (tour.transform.Find(name) != null);

            GameObject target = new GameObject(name);
            target.transform.parent = tour.transform;
            TourItem item = target.AddComponent<TourItem>();
            item._tour = tour;

            Pano pano = target.AddComponent<Pano>();
            if (tour.preset == TourPreset.googleVR)
            {
                pano.cameraType = Pano.CameraType.existing;
                pano.addPhysicsRaycaster = false;
                pano.existingCamera = Camera.main;
                pano.rotationMode = Pano.RotationMode.rotateGameObject;
            }

            if (texture != null)
            {
                if (texture is Cubemap)
                {
                    CubemapPanoRenderer r = target.AddComponent<CubemapPanoRenderer>();
                    r.cubemap = texture as Cubemap;
                    item._panoRenderer = r;
                }
                else
                {
                    SphericalPanoRenderer r = target.AddComponent<SphericalPanoRenderer>();
                    r.texture = texture;
                    item._panoRenderer = r;
                }
            }
            else item._panoRenderer = target.AddComponent<SphericalPanoRenderer>();

            item._directionManager = target.AddComponent<DirectionManager>();
            item._hotSpotManager = target.AddComponent<HotSpotManager>();

            target.SetActive(false);

            return item;
        }

        public void UpdateOutLinks()
        {
            if (_outLinks == null) _outLinks = new List<TourItem>();
            else _outLinks.Clear();

            HotSpotManager hsm = hotSpotManager;
            if (hsm != null)
            {
                for (int i = 0; i < hsm.Count; i++)
                {
                    GameObject target = hsm[i].switchToPanorama;
                    if (target != null) _outLinks.Add(target.GetComponent<TourItem>());
                }
            }

            DirectionManager dm = directionManager;
            if (dm != null)
            {
                for (int i = 0; i < dm.Count; i++)
                {
                    GameObject target = dm[i].switchToPanorama;
                    if (target != null) _outLinks.Add(target.GetComponent<TourItem>());
                }
            }
        }
    }
}