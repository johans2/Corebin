using UnityEngine;
using System.Collections;

public class AttachNode : MonoBehaviour {
    [SerializeField]
    private ItemCategory itemCategory;
    public ItemCategory ItemCategory { get { return this.itemCategory; } }
}
