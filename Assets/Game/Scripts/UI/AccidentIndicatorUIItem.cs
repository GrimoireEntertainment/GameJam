using Game.Accidents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class AccidentIndicatorUIItem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Icon image for the accident.")]
        private Image _iconImage;

        [SerializeField, Tooltip("Text displaying the accident title.")]
        private TMP_Text _titleText;

        public void Setup(AccidentDefinition accident)
        {
            if (accident == null)
            {
                return;
            }

            if (_iconImage != null)
            {
                _iconImage.sprite = accident.Icon;
                _iconImage.enabled = accident.Icon != null;
            }

            if (_titleText != null)
            {
                _titleText.text = accident.DisplayName;
            }
        }
    }
}
