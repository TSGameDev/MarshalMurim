using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TSGameDev.Inventories;

namespace TSGameDev.UI.Inventories
{
    /// <summary>
    /// To be put on the icon representing an inventory item. Allows the slot to
    /// update the icon and number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        #region Serialized Variables

        //Reference to the background image of the item amount
        [SerializeField] GameObject textContainer = null;
        //Reference to the text element of the item amount
        [SerializeField] TextMeshProUGUI itemNumber = null;

        #endregion

        #region Public Functions

        /// <summary>
        /// Function to set the
        /// </summary>
        /// <param name="item"></param>
        public void SetItem(InventoryItem item)
        {
            SetItem(item, 0);
        }

        /// <summary>
        /// Function that sets up the Icon, Number and Number image. Assigned image componant to Item sprite, Enables number image and sets number text to the amount of items.
        /// </summary>
        /// <param name="item">
        /// The item to display aka the item in the slot this inventory slot icon represents.
        /// </param>
        /// <param name="number">
        /// The amount of items that are in the the slot this inventory slot icon represents.
        /// </param>
        public void SetItem(InventoryItem item, int number)
        {
            var iconImage = GetComponent<Image>();
            if (item == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = item.GetIcon();
            }

            if (itemNumber)
            {
                if (number <= 1)
                {
                    textContainer.SetActive(false);
                }
                else
                {
                    textContainer.SetActive(true);
                    itemNumber.text = number.ToString();
                }
            }
        }

        #endregion

    }
}