using UnityEngine;

namespace Marsion
{
    public static class Util
    {
        /// <summary>
        /// Gets an existing component of type T from the GameObject, or adds one if it does not exist.
        /// </summary>
        /// <typeparam name="T">The type of the component to get or add.</typeparam>
        /// <param name="go">The GameObject to search or modify.</param>
        /// <returns>The component of type T.</returns>
        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();

            if (component == null)
            {
                // If the component does not exist, add it to the GameObject.
                component = go.AddComponent<T>();
            }

            return component;
        }

        /// <summary>
        /// Finds a child GameObject by name. Optionally searches recursively.
        /// </summary>
        /// <param name="go">The parent GameObject to search in.</param>
        /// <param name="name">The name of the child to find (optional).</param>
        /// <param name="recursive">Whether to search recursively through all descendants.</param>
        /// <returns>The child GameObject if found, otherwise null.</returns>
        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            if (go == null) { return null; }

            if (!recursive)
            {
                // Non-recursive search: check immediate children only.
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform child = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || child.name == name) return child.gameObject;
                }
            }
            else
            {
                // Recursive search: check all descendants.
                return FindChildRecursive(go.transform, name)?.gameObject;
            }

            // Log a warning if no matching child was found.
            Debug.LogWarning($"Child with name {name} not found in {go.name}");
            return null;
        }

        /// <summary>
        /// Finds a child of type T (e.g., a component) by name. Optionally searches recursively.
        /// </summary>
        /// <typeparam name="T">The type of component to find.</typeparam>
        /// <param name="go">The parent GameObject to search in.</param>
        /// <param name="name">The name of the child or component to find (optional).</param>
        /// <param name="recursive">Whether to search recursively through all descendants.</param>
        /// <returns>The component of type T if found, otherwise null.</returns>
        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
        {
            if (go == null) { return null; }

            if (!recursive)
            {
                // Non-recursive search: check immediate children only.
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform transform = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        // Try to get the component of type T from the child.
                        T component = transform.GetComponent<T>();
                        if (component != null) return component;
                    }
                }
            }
            else
            {
                // Recursive search: use GetComponentsInChildren to find all components of type T.
                foreach (T component in go.GetComponentsInChildren<T>())
                {
                    if (string.IsNullOrEmpty(name) || component.name == name) return component;
                }
            }

            // Log a warning if no matching component was found.
            Debug.LogWarning($"Component of type {typeof(T).Name} with name {name} not found in {go.name}");
            return null;
        }

        /// <summary>
        /// Recursively finds a child Transform by name.
        /// </summary>
        /// <param name="parent">The parent Transform to search in.</param>
        /// <param name="name">The name of the child to find.</param>
        /// <returns>The child Transform if found, otherwise null.</returns>
        private static Transform FindChildRecursive(Transform parent, string name)
        {
            if (parent == null) return null;

            // Transform is IEnumerable, so we can iterate over its children with foreach.
            foreach (Transform child in parent)
            {
                if (string.IsNullOrEmpty(name) || child.name == name)
                {
                    return child;
                }

                // Recursively search in the child's children.
                Transform found = FindChildRecursive(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            // Return null if no matching child was found.
            return null;
        }
    }
}
