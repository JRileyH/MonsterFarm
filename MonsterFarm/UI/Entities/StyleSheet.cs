using System.Text;
using System.Collections.Generic;
using MonsterFarm.UI.DataTypes;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonsterFarm.UI.Entities
{
    /// <summary>
    /// Set of style properties for different entity states.
    /// For example, stylesheet can define that when mouse hover over a paragraph, its text turns red.
    /// </summary>
    public class StyleSheet
    {
        private Dictionary<string, StyleProperty> _defaultSheet;
        private Dictionary<string, StyleProperty> _mousedownSheet;
        private Dictionary<string, StyleProperty> _mousehoverSheet;
        private Dictionary<string, StyleProperty>[] _allStates;

        public StyleSheet(
            Dictionary<string, StyleProperty> _default = null,
            Dictionary<string, StyleProperty> _mousedown = null,
            Dictionary<string, StyleProperty> _mousehover = null
        ){
            if (_default != null) { _defaultSheet = _default; } else { _defaultSheet = _makeDefaultSheet(); }
            if (_mousedown != null) { _mousedownSheet = _mousedown; } else { _mousedownSheet = _makeBlankSheet(); }
            if (_mousehover != null) { _mousehoverSheet = _mousehover; } else { _mousehoverSheet = _makeBlankSheet(); }
            _allStates = new Dictionary<string, StyleProperty>[3];
            _allStates[(int)EntityState.Default] = _defaultSheet;
            _allStates[(int)EntityState.MouseDown] = _mousedownSheet;
            _allStates[(int)EntityState.MouseHover] = _mousehoverSheet;
        }

        private Dictionary<string, StyleProperty> _makeDefaultSheet()
        {
            return new Dictionary<string, StyleProperty> {
                { "Scale", new StyleProperty(1)},
                { "FillColor", new StyleProperty(new Color(255, 255, 255, 255))},
                { "OutlineColor", new StyleProperty(new Color(0, 0, 0, 0))},
                { "OutlineWidth", new StyleProperty(0)},
                { "ForceAlignCenter", new StyleProperty(false)},
                { "FontStyle", new StyleProperty((int)FontStyle.Regular)},
                { "SelectedHighlightColor", new StyleProperty(new Color(0, 0, 0, 0))},
                { "ShadowColor", new StyleProperty(new Color(0, 0, 0, 0))},
                { "ShadowOffset", new StyleProperty(new Vector2(0, 0))},
                { "Padding", new StyleProperty(new Vector2(30, 30))},
                { "SpaceBefore", new StyleProperty(new Vector2(0, 0))},
                { "SpaceAfter", new StyleProperty(new Vector2(0, 8))},
                { "ShadowScale", new StyleProperty(1)},
            };
        }

        private Dictionary<string, StyleProperty> _makeBlankSheet()
        {
            return new Dictionary<string, StyleProperty> {
                { "Scale", new StyleProperty()},
                { "FillColor", new StyleProperty()},
                { "OutlineColor", new StyleProperty()},
                { "OutlineWidth", new StyleProperty()},
                { "ForceAlignCenter", new StyleProperty()},
                { "FontStyle", new StyleProperty()},
                { "SelectedHighlightColor", new StyleProperty()},
                { "ShadowColor", new StyleProperty()},
                { "ShadowOffset", new StyleProperty()},
                { "Padding", new StyleProperty()},
                { "SpaceBefore", new StyleProperty()},
                { "SpaceAfter", new StyleProperty()},
                { "ShadowScale", new StyleProperty()},
            };
        }

        public Dictionary<string, StyleProperty>[] GetAllStates(){
            return _allStates;
        }

        public StyleProperty GetStyleProperty(string property, EntityState state = EntityState.Default)
        {
            if(!_allStates[(int)state].ContainsKey(property)) return _defaultSheet[property];
            if (_allStates[(int)state][property].isEmpty()) return _defaultSheet[property];
            return _allStates[(int)state][property];
        }

        /// <summary>
        /// Set a stylesheet property.
        /// </summary>
        /// <param name="property">Property identifier.</param>
        /// <param name="value">Property value.</param>
        /// <param name="state">State to set property for.</param>
        public void SetStyleProperty(string property, StyleProperty value, EntityState state = EntityState.Default)
        {
            _allStates[(int)state][property] = value;
        }

        //TODO: change this to accept the value of Style Property itself and set a new Style Property
        public void SetStyleProperties(
            EntityState state = EntityState.Default,
            object scale = null,
            object fillColor = null,
            object outlineColor = null,
            object outlineWidth = null,
            object forceAlignCenter = null,
            object fontStyle = null,
            object selectedHighlightColor = null,
            object shadowColor = null,
            object shadowOffset = null,
            object padding = null,
            object spaceBefore = null,
            object spaceAfter = null,
            object shadowScale = null
        )
        {
            int _index = (int)state;
            if (scale != null && scale is StyleProperty) {
                _allStates[_index]["Scale"] = (StyleProperty)scale;
            }
            if (fillColor != null && fillColor is StyleProperty)
            {
                _allStates[_index]["FillColor"] = (StyleProperty)fillColor;
            }
            if (outlineColor != null && outlineColor is StyleProperty)
            {
                _allStates[_index]["OutlineColor"] = (StyleProperty)outlineColor;
            }
            if (outlineWidth != null && outlineWidth is StyleProperty)
            {
                _allStates[_index]["OutlineWidth"] = (StyleProperty)outlineWidth;
            }
            if (forceAlignCenter != null && forceAlignCenter is StyleProperty)
            {
                _allStates[_index]["ForceAlignCenter"] = (StyleProperty)forceAlignCenter;
            }
            if (fontStyle != null && fontStyle is StyleProperty)
            {
                _allStates[_index]["FontStyle"] = (StyleProperty)fontStyle;
            }
            if (selectedHighlightColor != null && selectedHighlightColor is StyleProperty)
            {
                _allStates[_index]["SelectedHighlightColor"] = (StyleProperty)selectedHighlightColor;
            }
            if (shadowColor != null && shadowColor is StyleProperty)
            {
                _allStates[_index]["ShadowColor"] = (StyleProperty)shadowColor;
            }
            if (shadowOffset != null && shadowOffset is StyleProperty)
            {
                _allStates[_index]["ShadowOffset"] = (StyleProperty)shadowOffset;
            }
            if (padding != null && padding is StyleProperty)
            {
                _allStates[_index]["Padding"] = (StyleProperty)padding;
            }
            if (spaceBefore != null && spaceBefore is StyleProperty)
            {
                _allStates[_index]["SpaceBefore"] = (StyleProperty)spaceBefore;
            }
            if (spaceAfter != null && spaceAfter is StyleProperty)
            {
                _allStates[_index]["SpaceAfter"] = (StyleProperty)spaceAfter;
            }
            if (shadowScale != null && shadowScale is StyleProperty)
            {
                _allStates[_index]["ShadowScale"] = (StyleProperty)shadowScale;
            }
        }

        /// <summary>
        /// Update the entire stylesheet from a different stylesheet.
        /// </summary>
        /// <param name="other">Other StyleSheet to update from.</param>
        public void UpdateFrom(StyleSheet other)
        {
            Dictionary<string, StyleProperty>[] _otherStates = other.GetAllStates();
            foreach (KeyValuePair<string, StyleProperty> _prop in _otherStates[(int)EntityState.Default]){
                _defaultSheet[_prop.Key] = _prop.Value;
            }
            foreach (KeyValuePair<string, StyleProperty> _prop in _otherStates[(int)EntityState.MouseDown])
            {
                _mousedownSheet[_prop.Key] = _prop.Value;
            }
            foreach (KeyValuePair<string, StyleProperty> _prop in _otherStates[(int)EntityState.MouseHover])
            {
                _mousehoverSheet[_prop.Key] = _prop.Value;
            }
        }
    };
}
