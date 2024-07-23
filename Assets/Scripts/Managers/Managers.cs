using UnityEngine;

namespace Marsion
{
    public class Managers : MonoBehaviour
    {
        private static Managers s_instance;
        public static Managers Instance { get { Init(); return s_instance; } }

        private Logger _logger = new Logger();
        public static Logger Logger { get { return Instance._logger; } }

        private void Start()
        {
            Init();
        }

        private static void Init()
        {
            if (s_instance == null)
            {
                GameObject obj = GameObject.Find("@Managers");

                if (obj == null)
                {
                    obj = new GameObject { name = "@Managers" };
                    obj.AddComponent<Managers>();
                }

                DontDestroyOnLoad(obj);
                s_instance = obj.GetComponent<Managers>();
            }
        }

        public static void Clear()
        {

        }
    }
}