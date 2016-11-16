using UnityEngine;
using UnityEngine.Assertions;

namespace RGCommon {
    public static class Find {


        /// <summary>
        /// Find relative child GameObject based on name, with lots of error handling.
        /// Use slashes in case of nested children.
        /// </summary>
        /// <param name="tag">Used for error logging, and will use to get reference to gameObject. Preferably the calling MonoBehaviour</param>
        /// <param name="path">Name of child gameObject</param>
        /// <returns>GameObject</returns>
        public static GameObject ChildByName(Component tag, string path) {
            return InternalFindChildByName(tag, tag.transform, path).gameObject;
        }
        /// <summary>
        /// Find relative child GameObject based on name, with lots of error handling.
        /// Use slashes in case of nested children.
        /// </summary>
        /// <param name="tag">Used for error logging. Preferably the calling MonoBehaviour</param>
        /// <param name="go">The GameObject in which you want to search</param>
        /// <param name="path">Name of child gameObject</param>
        /// <returns>GameObject</returns>
        public static GameObject ChildByName(object tag, GameObject go, string path) {
            return InternalFindChildByName(tag, go.transform, path).gameObject;
        }




        /// <summary>
        /// Find relative child Transform based on name, with lots of error handling.
        /// Use slashes in case of nested children.
        /// </summary>
        /// <param name="tag">Used for error logging, and will use to get reference to transform. Preferably the calling MonoBehaviour</param>
        /// <param name="path">Name of child transform</param>
        /// <returns>GameObject</returns>
        public static Transform ChildTransformByName(Component tag, string path) {
            return InternalFindChildByName(tag, tag.transform, path);
        }
        /// <summary>
        /// Find relative child Transform based on name, with lots of error handling.
        /// Use slashes in case of nested children.
        /// </summary>
        /// <param name="tag">Used for error logging. Preferably the calling MonoBehaviour</param>
        /// <param name="go">The Transform in which you want to search</param>
        /// <param name="path">Name of child transform</param>
        /// <returns>GameObject</returns>
        public static Transform ChildTransformByName(object tag, Transform go, string path) {
            return InternalFindChildByName(tag, go, path);
        }




        /// <summary>
        /// Will do a simple GetComponent, but with lots of error handling in case something goes wrong
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="tag">Used for error logging. Preferably the calling MonoBehaviour</param>
        /// <returns>The component</returns>
        public static T ComponentOnGameObject<T>(Component tag) where T : Component {
            return InternalFindComponentOnGameObject<T>(tag, tag.gameObject);
        }
        /// <summary>
        /// Will do a simple GetComponent, but with lots of error handling in case something goes wrong
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="tag">The GameObject on where to get the component, also used for logging. Preferably the calling MonoBehaviour</param>
        /// <returns>The component</returns>
        public static T ComponentOnGameObject<T>(GameObject tag) where T : Component {
            return InternalFindComponentOnGameObject<T>(tag, tag);
        }
        /// <summary>
        /// Will do a simple GetComponent, but with lots of error handling in case something goes wrong
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="tag">Used for error logging. Preferably the calling MonoBehaviour</param>
        /// <param name="gameObject">The GameObject on where to get the component</param>
        /// <returns>The component</returns>
        public static T ComponentOnGameObject<T>(object tag, GameObject go) where T : Component {
            return InternalFindComponentOnGameObject<T>(tag, go);
        }

        /// <summary>
        /// Gets a component from a game object.
        /// If the game object has no component of the specified type, it will be created.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="gameObject">The game object to get the component from.</param>
        /// <returns>The already existing or newly created component</returns>
        public static T ComponentOrCreate<T>(GameObject gameObject) where T : Component {
            T component = gameObject.GetComponent<T>();
            if(component != null) {
                return component;
            }
            return gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Search for a child by name and retrieve a typed component.
        /// With lots of error handling
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="tag">Used for error logging, and to get gameObject reference. Preferably the calling MonoBehaviour</param>
        /// <param name="path">string path to child game object</param>
        /// <returns>Component</returns>
        public static T ComponentOnChild<T>(Component tag, string path) where T : Component {
            return InternalFindComponentOnChild<T>(tag, tag.gameObject, path);
        }
        /// <summary>
        /// Search for a child by name and retrieve a typed component.
        /// With lots of error handling
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="tag">Used for error logging. Preferably the calling MonoBehaviour</param>
        /// <param name="go">GameObject holding the child</param>
        /// <param name="path">string path to child game object</param>
        /// <returns>Component</returns>
        public static T ComponentOnChild<T>(object tag, GameObject go, string path) where T : Component {
            return InternalFindComponentOnChild<T>(tag, go, path);
        }



        /// <summary>
        /// Search for a component in any child. Will throw error if number of components are zero or more than one
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="tag">Used for error logging, and gameObject reference. Preferably the calling MonoBehaviour</param>
        /// <returns>Component</returns>
        public static T ComponentInChildren<T>(Component tag) where T : Component {
            return InternalFindComponentInChildren<T>(tag, tag.gameObject);
        }
        /// <summary>
        /// Search for a component in any child. Will throw error if number of components are zero or more than one
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="tag">Used for error logging. Preferably the calling MonoBehaviour</param>
        /// <param name="go">Root game object</param>
        /// <returns>Component</returns>
        public static T ComponentInChildren<T>(object tag, GameObject go) where T : Component {
            return InternalFindComponentInChildren<T>(tag, go);
        }






        // The actual implementation

        private static Transform InternalFindChildByName(object tag, Transform transform, string path) {
            string descr = "The tag [[tag]] is expecting GameObject [[gameobject]] to have a child named [[path]]";
            Transform t = transform.Find(path);
            if(t == null) {
                throw new System.Exception(FormatError(descr, tag, transform.gameObject, path, null, "GameObject does not exist: [[fullpath]]"));
            }
            return t;
        }

        private static T InternalFindComponentOnGameObject<T>(object tag, GameObject go) where T : Component {
            string descr = "The tag [[tag]] is expecting GameObject [[gameobject]] to have component [[component]]";
            T component = go.GetComponent<T>();
            if(component == null) {
                throw new System.Exception(FormatError(descr, tag, go, null, typeof(T), "component does not exist on [[gameobject]]"));
            }
            return component;
        }

        private static T InternalFindComponentOnChild<T>(object tag, GameObject go, string path) where T : Component {
            string descr = "The tag [[tag]] is expecting GameObject [[gameobject]] with child [[path]] to have component [[component]]";
            Transform t = go.transform.Find(path);
            if (t == null) {
                throw new System.Exception(FormatError(descr, tag, go, path, typeof(T), "gameObject [[fullpath]] does not exist"));
            }

            T component = t.gameObject.GetComponent<T>();
            if (component == null) {
                throw new System.Exception(FormatError(descr, tag, go, path, typeof(T), "no component [[component]] exists on [[fullpath]]"));
            }

            return component;
        }

        private static T InternalFindComponentInChildren<T>(object tag, GameObject go) {
            string descr = "The tag [[tag]] is expecting GameObject [[gameobject]] to have any child with script [[component]]";
            T[] comps = go.GetComponentsInChildren<T>();
            if (comps.Length == 0) {
                throw new System.Exception(FormatError(descr, tag, go, null, typeof(T), "component [[component]] does not exist on any children"));
            }
            if (comps.Length > 1) {
                throw new System.Exception(FormatError(descr, tag, go, null, typeof(T), "more than one component of type [[component]] exists in children"));
            }
            return comps[0];
        }





        private static string FormatError(string template, object tag, GameObject go, string path, System.Type t, string reason) {
            string error = "FIND ERROR! ";

#if UNITY_ASSERTIONS
            //error += "\n    ";
            error += template + " -->";
            error += "\n    BUT: ";
            error += reason;

            if(tag != null) {
                error = error.Replace("[tag]", FormatTag(tag));
            }
            if (go != null) {
                error = error.Replace("[gameobject]", go.name);
            }
            if (path != null) {
                error = error.Replace("[path]", "\"" + path + "\"");
            }
            if (go != null && path != null) {
                error = error.Replace("[fullpath]", go.name + "/" + path);
            }
            if (t != null) {
                error = error.Replace("[component]", t.Name);
            }
            error += "\n";
#endif
            return error;
        }

        private static string FormatTag(object theTag) {
            if(theTag is GameObject) {
                return "GameObject: '" + ((GameObject)theTag).name + "'";
            }
            if (theTag is MonoBehaviour) {
                return "MonoBehaviour: '" + (theTag.GetType().FullName) + "'";
            }
            return theTag.GetType().Name;
        }
    }
}

