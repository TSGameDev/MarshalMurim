using UnityEngine;
using UnityEngine.EventSystems;

namespace TSGameDev.UI.Inventories.ToolTips
{
    /// <summary>
    /// Abstract base class that handles the spawning of a tooltip prefab at the
    /// correct position on screen relative to a cursor.
    /// 
    /// Override the abstract functions to create a tooltip spawner for your own
    /// data.
    /// </summary>
    public abstract class TooltipSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Serialized Variables

        [Tooltip("The prefab of the tooltip to spawn.")]
        [SerializeField] GameObject tooltipPrefab = null;

        #endregion

        #region Caches

        //Instaiated gameobject of the ToolTip Prefab
        GameObject tooltip = null;

        #endregion

        #region Public Functions

        /// <summary>
        /// Called when it is time to update the information on the tooltip
        /// prefab.
        /// </summary>
        /// <param name="tooltip">
        /// The spawned tooltip prefab for updating.
        /// </param>
        public abstract void UpdateTooltip(GameObject tooltip);
        
        /// <summary>
        /// Return true when the tooltip spawner should be allowed to create a tooltip.
        /// </summary>
        public abstract bool CanCreateTooltip();

        #endregion

        #region Private Functions

        /// <summary>
        /// When the tooltip spawner is destroyed, destory tooltip gameobject.
        /// </summary>
        private void OnDestroy()
        {
            ClearTooltip();
        }

        /// <summary>
        /// When the tooltip spawner is disabled, destory tooltip.
        /// </summary>
        private void OnDisable()
        {
            ClearTooltip();
        }

        /// <summary>
        /// Unity function that is called when the mouse pointer enters a UI element with the scrip tooltip spawner.
        /// </summary>
        /// <param name="eventData">
        /// The mouse pointer data that is returned by the OnPointerEnter event
        /// </param>
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            //Cache the canvas
            var parentCanvas = GetComponentInParent<Canvas>();

            //If the tooltip is instaiated but currently can't create a tooltip, destory the tooltip gameobject
            if (tooltip && !CanCreateTooltip())
            {
                ClearTooltip();
            }

            //If the tooltip isn't instaiated but you can create a tooltip, instantiate the tooltip prefab
            if (!tooltip && CanCreateTooltip())
            {
                tooltip = Instantiate(tooltipPrefab, parentCanvas.transform);
            }

            //If the tooltip is instantiated, update and position it.
            if (tooltip)
            {
                UpdateTooltip(tooltip);
                PositionTooltip();
            }
        }

        /// <summary>
        /// Function that is called OnPointerEnter to correctly position the tooltip
        /// </summary>
        private void PositionTooltip()
        {
            // Required to ensure corners are updated by positioning elements.
            Canvas.ForceUpdateCanvases();

            //Caches the vector for each corner of the tooltip
            var tooltipCorners = new Vector3[4];
            tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipCorners);

            //Caches the vector for each corner of the inventory slot
            var slotCorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(slotCorners);

            //Determine the location of the tooltip gameobject
            bool below = transform.position.y > Screen.height / 2;
            bool right = transform.position.x < Screen.width / 2;

            //Select the tooltip corner and the slot corner that generates the furthest distance between the two gameobjects
            int slotCorner = GetCornerIndex(below, right);
            int tooltipCorner = GetCornerIndex(!below, !right);

            //Positions the tooltip next to the hovered slot/UI element anchored on the bottom left corner
            tooltip.transform.position = slotCorners[slotCorner] - tooltipCorners[tooltipCorner] + tooltip.transform.position;
        }

        /// <summary>
        /// Function used to select a corner based on the position of the tooltip position.
        /// </summary>
        /// <param name="below">
        /// Bool that says if the tooltip is below half the screen height
        /// </param>
        /// <param name="right">
        /// Bool that sayd if the tooltip is past half the screen width
        /// </param>
        /// <returns>
        /// An int to be used to select a corner in the array
        /// </returns>
        private int GetCornerIndex(bool below, bool right)
        {
            //Bottom Left
            if (below && !right) return 0;
            //Top Left
            else if (!below && !right) return 1;
            //Top Right
            else if (!below && right) return 2;
            //Bottom Right
            else return 3;

        }

        /// <summary>
        /// Unity function that is called when the mouse pointer exits a UI element with the ToolTip spawner script.
        /// </summary>
        /// <param name="eventData">
        /// The mouse pointer data returned by the OnPointerExit event.
        /// </param>
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            //Caches slot corners
            var slotCorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(slotCorners);

            //Creates an area area around the tooltip
            Rect rect = new Rect(slotCorners[0], slotCorners[2] - slotCorners[0]);
            
            //If the mouse pointer is within the tooltip area, return out of the class without destorying the tooltip
            if (rect.Contains(eventData.position)) return;

            //Else destory tooltip
            ClearTooltip();
        }

        /// <summary>
        /// Destroys the instantiated tooltip gameobject
        /// </summary>
        private void ClearTooltip()
        {
            if (tooltip)
            {
                Destroy(tooltip.gameObject);
            }
        }

        #endregion

    }
}