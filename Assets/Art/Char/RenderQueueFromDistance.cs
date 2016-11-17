using UnityEngine;

/// <summary>
/// Renders this object before or after another object depending on its depth.
/// Changes the render queue depending on whether
/// this object is further away from the camera than the other object.
/// </summary>
public class RenderQueueFromDistance : MonoBehaviour {

    [Tooltip("Object to compare with. The object that's further away from the camera defines which render queue to use.")]
    public Transform otherObject;

    [Tooltip("Render queue to use when the \"other object\" is further away than this one.")]
    public int frontRenderQueue = 2021;
    [Tooltip("Render queue to use when the \"other object\" is closer than this one.")]
    public int backRenderQueue = 2019;

    [Tooltip("Puts some inertia into whether the object is rendered in front of or behind the other object.")]
    public float hysteresis = .01f;

    private new Transform transform;
    private new Renderer renderer;
    private Material material;

    private bool isBehind;

    void Awake() {
        transform = base.transform;
        renderer = GetComponent<Renderer>();
        material = renderer.material;
        ShowBehind();
    }

    void LateUpdate() {
        Camera camera = Camera.main;
        if(camera == null || otherObject == null) {
            return;
        }
        Transform cameraTransform = camera.transform;
        transform.LookAt(Camera.main.transform);

        float otherDistance = Vector3.Magnitude(otherObject.position - cameraTransform.position);
        float myDistance = Vector3.Magnitude(transform.position - cameraTransform.position);
        float distanceDiff = otherDistance - myDistance;
        if(isBehind && distanceDiff > hysteresis) {
            ShowInFront();
        }
        else if(!isBehind && distanceDiff < -hysteresis) {
            ShowBehind();
        }
    }

    private void ShowInFront() {
        this.isBehind = false;
        material.renderQueue = frontRenderQueue;
    }

    private void ShowBehind() {
        this.isBehind = true;
        material.renderQueue = backRenderQueue;
    }
}
