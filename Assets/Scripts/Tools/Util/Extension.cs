using System;
using UnityEngine;

namespace Marsion
{
    /// <summary>
    /// Extension methods for GameObject to simplify Util class
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Gets an existing component of type T from the GameObject, or adds one if it does not exist.
        /// </summary>
        /// <typeparam name="T">The type of the component to get or add.</typeparam>
        /// <param name="go">The GameObject to search or modify.</param>
        /// <returns>The component of type T.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the GameObject is null.</exception>
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go == null)
                throw new ArgumentNullException(nameof(go), "GameObject cannot be null");

            // Use the Util class to get or add the component.
            return Util.GetOrAddComponent<T>(go);
        }

        /// <summary>
        /// Finds a child GameObject by name. Optionally searches recursively.
        /// </summary>
        /// <param name="go">The parent GameObject to search in.</param>
        /// <param name="name">The name of the child to find.</param>
        /// <param name="recursive">Whether to search recursively through all descendants.</param>
        /// <returns>The child GameObject if found, otherwise null.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the GameObject is null.</exception>
        public static GameObject FindChild(this GameObject go, string name, bool recursive = false)
        {
            if (go == null)
                throw new ArgumentNullException(nameof(go), "GameObject cannot be null.");

            // Use the Util class to find the child.
            return Util.FindChild(go, name, recursive);
        }

        /// <summary>
        /// Finds a component of type T in the GameObject's children, optionally by name.
        /// Always searches recursively.
        /// </summary>
        /// <typeparam name="T">The type of component to find.</typeparam>
        /// <param name="go">The parent GameObject to search in.</param>
        /// <param name="name">The name of the child or component to find (optional).</param>
        /// <returns>The component of type T if found, otherwise null.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the GameObject is null.</exception>
        public static T FindComponentInChildren<T>(this GameObject go, string name = null) where T : Component
        {
            if (go == null)
                throw new ArgumentNullException(nameof(go), "GameObject cannot be null.");

            // Use the Util class to find the component in children recursively.
            return Util.FindChild<T>(go, name, recursive: true);
        }
    }
}
