using UnityEngine;

namespace Marsion
{
    public class ManagersInitializer : MonoBehaviour
    {
        private void Start()
        {
            IManagerFactory factory = new ManagerFactory();

            Managers.Init(factory);
        }
    }
}