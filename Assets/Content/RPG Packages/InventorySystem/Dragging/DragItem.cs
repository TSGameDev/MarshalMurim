using UnityEngine;
using UnityEngine.EventSystems;

namespace TSGameDev.UI.Inventories.Dragging
{
    /// <summary>
    /// Allows a UI element to be dragged and dropped from and to a container.
    /// 
    /// Create a subclass for the type you want to be draggable. Then place on
    /// the UI element you want to make draggable.
    /// 
    /// During dragging, the item is reparented to the parent canvas.
    /// 
    /// After the item is dropped it will be automatically return to the
    /// original UI parent. It is the job of components implementing `IDragContainer`,
    /// `IDragDestination and `IDragSource` to update the interface after a drag
    /// has occurred.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being dragged.</typeparam>
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        where T : class
    {
        #region Private Variables

        Vector3 startPosition;
        Transform originalParent;
        IDragSource<T> source;
        Canvas parentCanvas;

        #endregion

        #region Drag Handler Functions

        /// <summary>
        /// Unity function that gets called the moment a UI element is clicked on beginning the drag.
        /// </summary>
        /// <param name="eventData">
        /// Unity returns data about the event taking place causing the drag. Mouse pointer related data.
        /// </param>
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            // Caches the start pos of the UI element and its parent element.
            startPosition = transform.position;
            originalParent = transform.parent;
            // Allows for raycasts to pass through this elements canvas group so drop event can be called.
            // Sets the canvas as the parent allowing the dragged UI element to be seen above all UI elements.
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.SetParent(parentCanvas.transform, true);
        }

        /// <summary>
        /// Unity function that is called every frame similar to update when a UI element is being dragged.
        /// </summary>
        /// <param name="eventData">
        /// Mouse pointer data returned from the event by Unity.
        /// </param>
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            //Makes the Item sprite follow the mouse pointer
            transform.position = eventData.position;
        }

        /// <summary>
        /// Unity function that gets called the moment the UI element is released from the dragging operation.
        /// </summary>
        /// <param name="eventData">
        /// The mouse pointer data returned from the dragging event.
        /// </param>
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            //Returns the Item back to their original state
            transform.position = startPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.SetParent(originalParent, true);

            //Define a local container and looks for if the EndDrag was called while the mouse pointer was hovering over a IContainer element.
            IDragDestination<T> container;
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                container = parentCanvas.GetComponent<IDragDestination<T>>();
            }
            else
            {
                container = GetContainer(eventData);
            }

            //If the mouse pointer was over an IContainer, attempt to drop the Item into the container.
            if (container != null)
            {
                DropItemIntoContainer(container);
            }

            
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Awake function called on startup, sets the scripts variables.
        /// </summary>
        private void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            source = GetComponentInParent<IDragSource<T>>();
        }

        /// <summary>
        /// Function that attempts to collect the IDragDestination over the mouse pointer.
        /// </summary>
        /// <param name="eventData">
        /// The mouse pointer data returned by the drag event
        /// </param>
        /// <returns>
        /// IDragDestination which is the locate to drop an Item
        /// </returns>
        private IDragDestination<T> GetContainer(PointerEventData eventData)
        {
            if (eventData.pointerEnter)
            {
                var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();

                return container;
            }
            return null;
        }

        /// <summary>
        /// Function that Begins the process of attempting to put the dragged item into the desired slot.
        /// </summary>
        /// <param name="destination">
        /// The IDragDesitation of the mouse hovered slot
        /// </param>
        private void DropItemIntoContainer(IDragDestination<T> destination)
        {
            //checks if the destination and start pos are the same, if so returns out
            if (object.ReferenceEquals(destination, source)) return;

            //Collect the start pos and destination as containers
            var destinationContainer = destination as IDragContainer<T>;
            var sourceContainer = source as IDragContainer<T>;

            // Check if a swap is possible, if not attempt a simple swap, else go through full swap process.
            if (destinationContainer == null || sourceContainer == null || 
                destinationContainer.GetItem() == null || 
                object.ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
            {
                AttemptSimpleTransfer(destination);
                return;
            }

            AttemptSwap(destinationContainer, sourceContainer);
        }

        /// <summary>
        /// Function that performs the full process of swapping an Item into a slot no matter if its empty or full. 
        /// Accounts for returning left over stack items back to source and swapping desitnation item to source.
        /// </summary>
        /// <param name="destination">
        /// The IContainer to put the draged item into.
        /// </param>
        /// <param name="source">
        /// The IContainer the dragged item came from, the location to put the destiantion item if its there.
        /// </param>
        private void AttemptSwap(IDragContainer<T> destination, IDragContainer<T> source)
        {
            // Caches the item and number of items from both the source and destination. 
            var removedSourceNumber = source.GetNumber();
            var removedSourceItem = source.GetItem();
            var removedDestinationNumber = destination.GetNumber();
            var removedDestinationItem = destination.GetItem();

            //Remove the source and destination items
            source.RemoveItems(removedSourceNumber);
            destination.RemoveItems(removedDestinationNumber);

            //Calculate if there will be any remaining items after the swap to return to the source. 
            var sourceTakeBackNumber = CalculateTakeBack(removedSourceItem, removedSourceNumber, source, destination);
            var destinationTakeBackNumber = CalculateTakeBack(removedDestinationItem, removedDestinationNumber, destination, source);

            // Perform the returns if there are any to perform
            if (sourceTakeBackNumber > 0)
            {
                source.AddItems(removedSourceItem, sourceTakeBackNumber);
                removedSourceNumber -= sourceTakeBackNumber;
            }
            if (destinationTakeBackNumber > 0)
            {
                destination.AddItems(removedDestinationItem, destinationTakeBackNumber);
                removedDestinationNumber -= destinationTakeBackNumber;
            }

            // Abort if we can't do a successful swap
            if (source.MaxAcceptable(removedDestinationItem) < removedDestinationNumber ||
                destination.MaxAcceptable(removedSourceItem) < removedSourceNumber)
            {
                destination.AddItems(removedDestinationItem, removedDestinationNumber);
                source.AddItems(removedSourceItem, removedSourceNumber);
                return;
            }

            // Do swaps
            if (removedDestinationNumber > 0)
            {
                source.AddItems(removedDestinationItem, removedDestinationNumber);
            }
            if (removedSourceNumber > 0)
            {
                destination.AddItems(removedSourceItem, removedSourceNumber);
            }
        }

        /// <summary>
        /// Function that performs a simple transfer of the source item to the destination.
        /// Doesn't account for anything like left over stack items or destination being filled.
        /// </summary>
        /// <param name="destination">
        /// The IDestination to put the dragged item.
        /// </param>
        /// <returns>
        /// A bool, true if the transfer attempt failed, false if the transfer was successful.
        /// </returns>
        private bool AttemptSimpleTransfer(IDragDestination<T> destination)
        {
            var draggingItem = source.GetItem();
            var draggingNumber = source.GetNumber();

            var acceptable = destination.MaxAcceptable(draggingItem);
            var toTransfer = Mathf.Min(acceptable, draggingNumber);

            if (toTransfer > 0)
            {
                source.RemoveItems(toTransfer);
                destination.AddItems(draggingItem, toTransfer);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Function that calculates the left over removeSource items when transfering to destination to be returned back to the source.
        /// </summary>
        /// <param name="removedItem">
        /// An item that has been removed from its position, could be source or destination item.
        /// </param>
        /// <param name="removedNumber">
        /// The amount of the item passed in
        /// </param>
        /// <param name="removeSource">
        /// An IContainer which the passed in item came from.
        /// </param>
        /// <param name="destination">
        /// The IContainer which the passed in item will go to.
        /// </param>
        /// <returns>
        /// Int for the amount of items to be returns to the place they came from.
        /// </returns>
        private int CalculateTakeBack(T removedItem, int removedNumber, IDragContainer<T> removeSource, IDragContainer<T> destination)
        {
            var takeBackNumber = 0;
            var destinationMaxAcceptable = destination.MaxAcceptable(removedItem);

            if (destinationMaxAcceptable < removedNumber)
            {
                takeBackNumber = removedNumber - destinationMaxAcceptable;

                var sourceTakeBackAcceptable = removeSource.MaxAcceptable(removedItem);

                // Abort and reset
                if (sourceTakeBackAcceptable < takeBackNumber)
                {
                    return 0;
                }
            }
            return takeBackNumber;
        }

        #endregion
    }
}