using UnityEngine;
using System.Collections;

public class PlayerAvatarCustomizationManager : MonoBehaviour {

    private GameObject vanityItemInstanceReference;

	void Start () {

        Debug.Log ("Friendly Greetings from PlayerAvatarCustomizationManager.Start()");

        // assuming the scene and player avatar is scrapped between each load
        CheckForCurrentEquippedItemAndInstatiate ();
	}
	
    private void CheckForCurrentEquippedItemAndInstatiate()
    {
        InventoryCatalogue.ItemID itemID = Inventory.Instance.GetEquippedItem (ItemCategory.HeadCustomizationItems);

        Debug.Log ("Currently equipped item is:" + itemID.ToString ());

        if(itemID != InventoryCatalogue.ItemID.VanityHeadNone) {
            VanityItemPrefabDefinition vanityItemPrefabDefinition = VanityItemsList.Instance.GetVanityItemPrefabDefinition (itemID);

            vanityItemInstanceReference = (GameObject)Instantiate (vanityItemPrefabDefinition.spawnablePrefab);
            AttachableItem attachable = vanityItemInstanceReference.AddComponent<AttachableItem> ();

            // this function will find an AttachNode with the corresponding ItemCategory and child it to that gameObject
            attachable.Attach (this.gameObject, vanityItemPrefabDefinition.ItemCategory);
        }
    }
}
