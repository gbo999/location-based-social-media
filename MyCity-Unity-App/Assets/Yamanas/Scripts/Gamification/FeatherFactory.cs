using System.Collections;
using UnityEngine;

namespace Yamanas.Scripts.MapLoader.Gamification
{
    public class FeatherFactory : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject _featherPrefab;

        [SerializeField] private Transform[] _startTransforms;

        [SerializeField] private GameObject _pointsMeter;

        [SerializeField] private float speed;

        private static FeatherFactory _instance;

        #endregion

        #region methods


        /*public void toCheck()
        {
            StartCoroutine(CreateCheck());
        }*/

        public IEnumerator CreateCheck(Transform transform)
        {
            GameObject feather = Instantiate(_featherPrefab, transform);

          float  time = 0;
            
            while (time<1)
            {
                time +=speed* Time.deltaTime;
                feather.transform.position = Vector3.Lerp(transform.position, _pointsMeter.transform.position,
                    time);
                yield return new WaitForEndOfFrame();
            }

            Destroy(feather);
        }
        
        public void CreateFive()
        {
            for (int i = 0; i < 5; i++)
            {
                StartCoroutine(CreateCheck(_startTransforms[i]));
            }
        }


        public void Create(Transform transform)
        {
            GameObject feather = Instantiate(_featherPrefab, transform);

            while (feather.transform.position != _pointsMeter.transform.position)
            {
                feather.transform.position = Vector3.Lerp(feather.transform.position, _pointsMeter.transform.position,
                    1.5f * Time.deltaTime);
            }

            Destroy(feather);
        }

        #endregion

        #region Properties

        public static FeatherFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<FeatherFactory>();
                }

                return _instance;
            }
        }

        #endregion
    }
}