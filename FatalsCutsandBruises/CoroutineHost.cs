using UnityEngine;

namespace FatalsCutsAndBruises
{
    public class CoroutineHost : MonoBehaviour
    {
        private static CoroutineHost _instance;

        public static CoroutineHost Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CutsAndBruises_CoroutineHost");
                    _instance = go.AddComponent<CoroutineHost>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
    }
}
