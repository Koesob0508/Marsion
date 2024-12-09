using UnityEngine;

namespace Marsion
{
    [DefaultExecutionOrder(-10)]
    public class ManagersInitializer : MonoBehaviour
    {
        private void Start()
        {
            IManagerFactory factory = new DefaultManagerFactory();

            Managers.Init(factory);
        }
    }
}