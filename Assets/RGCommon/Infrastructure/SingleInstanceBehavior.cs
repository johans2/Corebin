using UnityEngine;
using UnityEngine.Assertions;

namespace RGCommon {
    /// <summary>
    /// Use as superclass to make a single instance of a MonoBehavior available.
    /// Unlike the standard Singleton pattern, trying to get the instance if
    /// no instance has been created does not create it automatically.
    /// Instead that is an error.
    /// 
    /// Typical usage is to create a script similar to this:
    /// 
    /// public class MyScript : SingleInstanceBehavior<MyScript> {
    ///     // ...
    /// }
    /// 
    /// Then add an instance of MyScript to the scene.
    /// It will then be accessible as MyScript.Instance after the Awake phase.
    /// 
    /// If your script implements Awake or OnDestroy, it has to call
    /// the corresponding method in this class.
    /// </summary>
    /// <typeparam name="T">The type of the instance to make available as Instance</typeparam>
    public class SingleInstanceBehavior<T> : MonoBehaviour where T : SingleInstanceBehavior<T> {
        private static T instance;

        public virtual void Awake() {
            Assert.IsNull(instance, "Duplicate instance of " + typeof(T) + ": " + instance + " and " + this);
            instance = (T)this;
        }

        public virtual void OnDestroy() {
            instance = null;
        }

        public static T Instance {
            get {
                Assert.IsNotNull(instance, "No instance of " + typeof(T));
                return instance;
            }
        }
    }
}
