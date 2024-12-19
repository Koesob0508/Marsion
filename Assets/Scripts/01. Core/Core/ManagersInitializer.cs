using UnityEngine;

namespace Marsion
{
    [DefaultExecutionOrder(-10)]
    public class ManagersInitializer : MonoBehaviour
    {
        private void Start()
        {
            IManagersFactory factory = new DefaultManagersFactory();

            Managers.Init(factory);
        }
    }
}