using UnityEngine;

namespace UserInterface
{
    using UnityEngine.UI;

    /// <summary>Represents a basic key, for use with a <see cref="UserInterface.CustomKeyboard"/>.
    /// </summary>
    public class Key : MonoBehaviour
    {
        /// <summary>
        /// The parent keyboard.
        /// </summary>
        [SerializeField][HideInInspector] protected CustomKeyboard parentKeyboard;
        /// <summary>
        /// The backing image.
        /// </summary>
        [SerializeField] protected Image backingImage;

        #if UNITY_EDITOR
        public void SetPosition(Vector3 position)
        {
            GetComponent<RectTransform>().anchoredPosition3D = position;
        }

        public void SetKeyboard(CustomKeyboard parentKeyboard)
        {
            this.parentKeyboard = parentKeyboard;
        }

        public Vector2 GetBackingImageDimensions()
        {
            Rect rect = backingImage.rectTransform.rect;

            return new Vector2(rect.width, rect.height);
        }
        #endif
    }
}