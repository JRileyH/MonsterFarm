using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.UI.DataTypes;

namespace MonsterFarm.UI.Elements
{

    /// <summary>
    /// Different draw phases of the element.
    /// </summary>
    public enum DrawPhase
    {
        /// <summary>
        /// Drawing the element itself.
        /// </summary>
        Base,

        /// <summary>
        /// Drawing element outline.
        /// </summary>
        Outline,

        /// <summary>
        /// Drawing element's shadow.
        /// </summary>
        Shadow,
    }

    /// <summary>
    /// Static strings with all common style property names, to reduce string creations.
    /// </summary>
    internal static class StylePropertyIds
    {
        public static readonly string SpaceAfter = "SpaceAfter";
        public static readonly string SpaceBefore = "SpaceBefore";
        public static readonly string FillColor = "FillColor";
        public static readonly string Scale = "Scale";
        public static readonly string Padding = "Padding";
        public static readonly string ShadowColor = "ShadowColor";
        public static readonly string ShadowScale = "ShadowScale";
        public static readonly string ShadowOffset = "ShadowOffset";
        public static readonly string OutlineColor = "OutlineColor";
        public static readonly string OutlineWidth = "OutlineWidth";
    }

    /// <summary>
    /// An Anchor is a pre-defined position in parent element that we use to position a child.
    /// For eample, we can use anchors to position an element at the bottom-center point of its parent.
    /// Note: anchor affect both the position relative to parent and also the offset origin point of the element.
    /// </summary>
    public enum Anchor
    {
        /// <summary>Center of parent element.</summary>
        Center,

        /// <summary>Top-Left corner of parent element.</summary>
        TopLeft,

        /// <summary>Top-Right corner of parent element.</summary>
        TopRight,

        /// <summary>Top-Center of parent element.</summary>
        TopCenter,

        /// <summary>Bottom-Left corner of parent element.</summary>
        BottomLeft,

        /// <summary>Bottom-Right corner of parent element.</summary>
        BottomRight,

        /// <summary>Bottom-Center of parent element.</summary>
        BottomCenter,

        /// <summary>Center-Left of parent element.</summary>
        CenterLeft,

        /// <summary>Center-Right of parent element.</summary>
        CenterRight,

        /// <summary>Automatically position this element below its older sibling.</summary>
        Auto,

        /// <summary>Automatically position this element to the right side of its older sibling, and begin a new row whenever
        /// exceeding the parent container width.</summary>
        AutoInline,

        /// <summary>Automatically position this element to the right side of its older sibling, even if exceeding parent container width.</summary>
        AutoInlineNoBreak,

        /// <summary>Position of the older sibling bottom, eg align this element based on its older sibling, but center on X axis.
        /// Use this property to place elements one after another but keep them aligned to center (especially paragraphs).</summary>
        AutoCenter,
    };

    /// <summary>
    /// Possible element states and interactions with user.
    /// </summary>
    public enum ElementState
    {
        /// <summary>Default state, eg currently not interacting.</summary>
        Default = 0,

        /// <summary>Mouse is hovering over this element.</summary>
        MouseHover = 1,

        /// <summary>Mouse button is pressed down over this element.</summary>
        MouseDown = 2,
    };

    /// <summary>
    /// Basic UI element.
    /// All elements inherit from this class and share this API.
    /// </summary>
    [System.Serializable]
    public abstract class Element
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static Element()
        {
            Element.MakeSerializable(typeof(Element));
        }

        // list of child elements
        private List<Element> _children = new List<Element>();

        /// <summary>
        /// Get / set children list.
        /// </summary>
        public List<Element> Children
        {
            get
            {
                return _children;
            }
            set
            {
                ClearChildren();
                foreach (var child in value) AddChild(child);
            }
        }

        // list of sorted children
        private List<Element> _sortedChildren;

        // all child types
        internal static List<System.Type> _serializableTypes = new List<System.Type>();

        /// <summary>
        /// Make an element type serializable.
        /// </summary>
        /// <param name="type">Element type to make serializable.</param>
        public static void MakeSerializable(System.Type type)
        {
            _serializableTypes.Add(type);
        }

        // do we need to update sorted children list?
        internal bool _needToSortChildren = true;

        /// <summary>
        /// If true, will not show this element when searching.
        /// Used for internal elements.
        /// </summary>
        internal bool _hiddenInternalElement = false;

        /// <summary>
        /// A special size used value to use when you want to get the element default size.
        /// </summary>
        public static readonly Vector2 USE_DEFAULT_SIZE = new Vector2(-1, -1);

        /// <summary>The direct parent of this element.</summary>
        protected Element _parent = null;

        /// <summary>Index inside parent.</summary>
        protected int _indexInParent;

        /// <summary>
        /// Optional extra drawing priority, to bring certain objects before others.
        /// </summary>
        public int PriorityBonus = 0;

        /// <summary>Is the element currently interactable.</summary>
        protected bool _isInteractable = false;

        /// <summary>Optional identifier you can attach to elements so you can later search and retrieve by.</summary>
        public string Identifier = string.Empty;

        /// <summary>
        /// Last known scroll value, when elements are inside scrollable panels.
        /// </summary>
        protected Point _lastScrollVal = Point.Zero;

        /// <summary>
        /// If this boolean is true, events will just "go through" this element to its children or elements behind it.
        /// This bool comes to solve conditions where you have two panels without skin that hide each other but you want
        /// users to be able to click on the bottom panel through the upper panel, provided it doesn't hit any of the first
        /// panel's children.
        /// </summary>
        public bool ClickThrough = false;

        /// <summary>If in promiscuous mode, mouse button is pressed *outside* the element and then released on the element, click event will be fired.
        /// If false, in order to fire click event the mouse button must be pressed AND released over this element (but can travel outside while being
        /// held down, as long as its released inside).
        /// Note: Windows default behavior is non promiscuous mode.</summary>
        public bool PromiscuousClicksMode = false;

        /// <summary>
        /// If this set to true, this element will still react to events if its direct parent is locked.
        /// This setting is mostly for scrollbars etc, that even if parent is locked should still be scrollable.
        /// </summary>
        protected bool DoEventsIfDirectParentIsLocked = false;

        /// <summary>
        /// If true, this element will always inherit its parent state.
        /// This is useful for stuff like a paragraph that's attached to a button etc.
        /// NOTE!!! elements that inherit parent state will not trigger any events either.
        /// </summary>
        public bool InheritParentState = false;

        // optional background object for this element.
        // the background will be rendered on the full size of this element, behind it, and will not respond to events etc.
        private Element _background = null;

        // mark the first update call on this element.
        private bool _isFirstUpdate = true;

        /// <summary>
        /// Mark if this element is dirty and need to recalculate its destination rect.
        /// </summary>
        private bool _isDirty = true;

        // element current style properties
        private StyleSheet _style = new StyleSheet();

        /// <summary>
        /// Get / set raw stylesheet.
        /// </summary>
        public StyleSheet RawStyleSheet
        {
            get { return _style; }
            set { _style = value; }
        }

        /// <summary>
        /// Get overflow scrollbar value.
        /// </summary>
        protected virtual Point OverflowScrollVal { get { return Point.Zero; } }

        // optional min size.
        private Vector2? _minSize;

        // optional max size.
        private Vector2? _maxSize;

        /// <summary>
        /// If defined, will limit the minimum size of this element when calculating size.
        /// This is especially useful for elements with size that depends on their parent element size, for example
        /// if you define an element to take 20% of its parent space but can't be less than 200 pixels width.
        /// </summary>
        public Vector2? MinSize { get { return _minSize; } set { _minSize = value; MarkAsDirty(); } }

        /// <summary>
        /// If defined, will limit the maximum size of this element when calculating size.
        /// This is especially useful for elements with size that depends on their parent element size, for example
        /// if you define an element to take 20% of its parent space but can't be more than 200 pixels width.
        /// </summary>
        public Vector2? MaxSize { get { return _maxSize; } set { _maxSize = value; MarkAsDirty(); } }

        /// <summary>
        /// Every time we update destination rect and internal destination rect view the update function, we increase this counter.
        /// This is so our children will know we did an update and they need to update too.
        /// </summary>
        internal uint _destRectVersion = 0;

        /// <summary>
        /// The last known version we have of the parent dest rect version.
        /// If this number does not match our parent's _destRectVersion, we will recalculate destination rect.
        /// </summary>
        private uint _parentLastDestRectVersion = 0;

        /// <summary>Optional data you can attach to this element and retrieve later (for example when handling events).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public object AttachedData = null;

        /// <summary>
        /// If true (default), will use the actual object size for collision detection. If false, will use the size property.
        /// This is useful for paragraphs, for example, where the actual width is based on text content and can vary and be totally
        /// different than the size set in the constructor.
        /// </summary>
        public bool UseActualSizeForCollision = true;

        /// <summary>element size (in pixels). Value of 0 will take parent's full size. -1 will take defaults.</summary>
        protected Vector2 _size;

        /// <summary>Offset, in pixels, from the anchor position.</summary>
        protected Vector2 _offset;

        /// <summary>
        /// Set / get offset.
        /// </summary>
        public Vector2 Offset
        {
            get { return _offset; }
            set { SetOffset(value); }
        }

        /// <summary>Anchor to position this element based on (see Anchor enum for more info).</summary>
        protected Anchor _anchor;

        /// <summary>
        /// Set / get anchor.
        /// </summary>
        public Anchor Anchor
        {
            get { return _anchor; }
            set { SetAnchor(value); }
        }

        /// <summary>Basic default style that all elements share. Note: loaded from UI theme xml file.</summary>
        public static StyleSheet DefaultStyle = new StyleSheet();

        /// <summary>Callback to execute when mouse button is pressed over this element (called once when button is pressed).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnMouseDown = null;

        /// <summary>Callback to execute when right mouse button is pressed over this element (called once when button is pressed).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnRightMouseDown = null;

        /// <summary>Callback to execute when mouse button is released over this element (called once when button is released).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnMouseReleased = null;

        /// <summary>Callback to execute every frame while mouse button is pressed over the element.</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback WhileMouseDown = null;

        /// <summary>Callback to execute every frame while right mouse button is pressed over the element.</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback WhileRightMouseDown = null;

        /// <summary>Callback to execute every frame while mouse is hovering over the element (not called while mouse button is down).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback WhileMouseHover = null;

        /// <summary>Callback to execute every frame while mouse is hovering over the element, even if mouse is down.</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback WhileMouseHoverOrDown = null;

        /// <summary>Callback to execute when user clicks on this element (eg release mouse over it).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnClick = null;

        /// <summary>Callback to execute when user clicks on this element with right mouse button (eg release mouse over it).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnRightClick = null;

        /// <summary>Callback to execute when element value changes (relevant only for elements with value).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnValueChange = null;

        /// <summary>Callback to execute when mouse start hovering over this element (eg enters its region).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnMouseEnter = null;

        /// <summary>Callback to execute when mouse stop hovering over this element (eg leaves its region).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnMouseLeave = null;

        /// <summary>Callback to execute when mouse wheel scrolls and this element is the active element.</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnMouseWheelScroll = null;

        /// <summary>Called when element starts getting dragged (only if draggable).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnStartDrag = null;

        /// <summary>Called when element stop getting dragged (only if draggable).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnStopDrag = null;

        /// <summary>Called every frame while the element is being dragged.</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback WhileDragging = null;

        /// <summary>Callback to execute every frame before this element is rendered.</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback BeforeDraw = null;

        /// <summary>Callback to execute every frame after this element is rendered.</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback AfterDraw = null;

        /// <summary>Callback to execute every frame before this element updates.</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback BeforeUpdate = null;

        /// <summary>Callback to execute every frame after this element updates.</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback AfterUpdate = null;

        /// <summary>Callback to execute every time the visibility of this element changes (also invokes when parent becomes invisible / visible again).</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnVisiblityChange = null;

        /// <summary>Callback to execute every time this element focus / unfocus.</summary>
        [System.Xml.Serialization.XmlIgnore]
        public EventCallback OnFocusChange = null;

        /// <summary>
        /// Optional tooltip text to show if the user points on this element for long enough.
        /// </summary>
        public string ToolTipText;

        /// <summary>Is mouse currently pointing on this element.</summary>
        protected bool _isMouseOver = false;

        /// <summary>Is the element currently enabled? If false, will not be interactive and be rendered with a greyscale effect.</summary>
        public bool Enabled = true;

        /// <summary>Disable elements - will be removed in future versions!</summary>
        [System.Obsolete("'Disabled' is deprecated, please use 'Enabled' instead.")]
        public bool Disabled
        {
            get { return !Enabled; }
            set { Enabled = !value; }
        }

        /// <summary>If true, this element and its children will not respond to events (but will be drawn normally, unlike when disabled).</summary>
        public bool Locked = false;

        /// <summary>Is the element currently visible.</summary>
        private bool _visible = true;

        /// <summary>Is this element currently disabled?</summary>
        private bool _isCurrentlyDisabled = false;

        /// <summary>Current element state.</summary>
        protected ElementState _elementState = ElementState.Default;

        // is this element currently focused?
        bool _isFocused = false;

        /// <summary>Does this element or one of its children currently focused?</summary>
        [System.Xml.Serialization.XmlIgnore]
        public bool IsFocused
        {
            // get if focused
            get
            {
                return _isFocused;
            }

            // set if focused
            set
            {
                if (_isFocused != value)
                {
                    _isFocused = value;
                    DoOnFocusChange();
                }
            }
        }

        /// <summary>Currently calculated destination rect (eg the region this element is drawn on).</summary>
        internal Rectangle _destRect;

        /// <summary>Currently calculated internal destination rect (eg the region this element children are positioned in).</summary>
        protected Rectangle _destRectInternal;

        /// <summary>
        /// Get internal destination rect.
        /// </summary>
        public Rectangle InternalDestRect
        {
            get
            {
                return _destRectInternal;
            }
        }

        // is this element draggable?
        private bool _draggable = false;

        // do we need to init drag offset from current position?
        private bool _needToSetDragOffset = false;

        // current dragging offset.
        private Vector2 _dragOffset = Vector2.Zero;

        // true if this element is currently being dragged.
        private bool _isBeingDragged = false;

        /// <summary>Default size this element will have when no size is provided or when -1 is set for either width or height.</summary>
        public static Vector2 DefaultSize = Vector2.Zero;

        /// <summary>If true, users will not be able to drag this element outside its parent boundaries.</summary>
        public bool LimitDraggingToParentBoundaries = true;

        /// <summary>
        /// Create the element.
        /// </summary>
        /// <param name="size">element size, in pixels.</param>
        /// <param name="anchor">Poisition anchor.</param>
        /// <param name="offset">Offset from anchor position.</param>
        public Element(Vector2? size = null, Anchor anchor = Anchor.Auto, Vector2? offset = null)
        {
            // set as dirty (eg need to recalculate destination rect)
            MarkAsDirty();

            // store size, anchor and offset
            Vector2 defaultSize = elementDefaultSize;
            _size = size ?? defaultSize;
            _offset = offset ?? Vector2.Zero;
            _anchor = anchor;

            // set basic default style
            UpdateStyle(DefaultStyle);

            // check default size on specific axises
            if (_size.X == -1) { _size.X = defaultSize.X; }
            if (_size.Y == -1) { _size.Y = defaultSize.Y; }
        }

        /// <summary>
        /// Return the default size for this element.
        /// </summary>
        public Vector2 elementDefaultSize
        {
            get
            {
                // get current class type
                System.Type type = GetType();

                // try to get default size static property, and if not found, climb to parent class until DefaultSize is defined.
                // note: eventually it will stop at element, since we have defined default size here.
                while (true)
                {
                    // try to get DefaultSize field and if found return it
                    FieldInfo field = type.GetField("DefaultSize", BindingFlags.Public | BindingFlags.Static);
                    if (field != null)
                    {
                        return (Vector2)(field.GetValue(null));
                    }

                    // if not found climb up to parent
                    type = type.BaseType;
                }
            }
        }

        /// <summary>
        /// Get current mouse position.
        /// </summary>
        /// <param name="addVector">Optional vector to add to cursor position.</param>
        /// <returns>Mouse position.</returns>
        protected Vector2 GetMousePos(Vector2? addVector = null)
        {
            return UserInterface.Active.GetTransformedCursorPos(addVector);
        }

        /// <summary>
        /// Get input helper from active user interface.
        /// </summary>
        protected InputHelper Input
        {
            get { return UserInterface._input; }
        }

        /// <summary>
        /// Get the active user interface global scale.
        /// </summary>
        protected float GlobalScale
        {
            get { return UserInterface.Active.GlobalScale; }
        }

        /// <summary>
        /// If true, will add debug drawing to UI system to show offsets, margin, etc.
        /// </summary>
        protected bool DebugDraw
        {
            get { return UserInterface.Active.DebugDraw; }
        }

        /// <summary>
        /// Call this function when the first update occures.
        /// </summary>
        protected virtual void DoOnFirstUpdate()
        {
            // call the spawn event
            UserInterface.Active.OnElementSpawn?.Invoke(this);

            // make parent dirty
            if (_parent != null) { _parent.MarkAsDirty(); }
        }

        /// <summary>
        /// Return stylesheet property for a given state.
        /// </summary>
        /// <param name="property">Property identifier.</param>
        /// <param name="state">State to get property for (if undefined will fallback to default state).</param>
        /// <returns>Style property value for given state or default, or null if undefined.</returns>
        public StyleProperty GetStyleProperty(string property, ElementState state = ElementState.Default)
        {
            return _style.GetStyleProperty(property, state);
        }

        /// <summary>
        /// Set a stylesheet property.
        /// </summary>
        /// <param name="property">Property identifier.</param>
        /// <param name="value">Property value.</param>
        /// <param name="state">State to set property for.</param>
        /// <param name="markAsDirty">If true, will mark this element as dirty after this style change.</param>
        public void SetStyleProperty(string property, StyleProperty value, ElementState state = ElementState.Default, bool markAsDirty = true)
        {
            _style.SetStyleProperty(property, value, state);
            if (markAsDirty) { MarkAsDirty(); }
        }

        /// <summary>
        /// Return stylesheet property for current element state (or default if undefined for state).
        /// </summary>
        /// <param name="property">Property identifier.</param>
        /// <returns>Stylesheet property value for current element state, or default if not defined.</returns>
        public StyleProperty GetActiveStyle(string property)
        {
            return GetStyleProperty(property, _elementState);
        }

        /// <summary>
        /// Update the entire stylesheet from a different stylesheet.
        /// </summary>
        /// <param name="updates">Stylesheet to update from.</param>
        public void UpdateStyle(StyleSheet updates)
        {
            _style.UpdateFrom(updates);
            MarkAsDirty();
        }

        /// <summary>Get extra space after with current UI scale applied. </summary>
        protected Vector2 _scaledSpaceAfter { get { return SpaceAfter * GlobalScale; } }

        /// <summary>Get extra space before with current UI scale applied. </summary>
        protected Vector2 _scaledSpaceBefore { get { return SpaceBefore * GlobalScale; } }

        /// <summary>Get size with current UI scale applied. </summary>
        protected Vector2 _scaledSize { get { return _size * GlobalScale; } }

        /// <summary>Get offset with current UI scale applied. </summary>
        protected Vector2 _scaledOffset { get { return _offset * GlobalScale; } }

        /// <summary>Get offset with current UI scale applied. </summary>
        protected Vector2 _scaledPadding { get { return Padding * GlobalScale; } }

        /// <summary>
        /// Adds extra space outside the dest rect for collision detection.
        /// In other words, if extra margin is set to 10 and the user points with its mouse 5 pixels above this element,
        /// it would still think the user points on the element.
        /// </summary>
        public Point ExtraMargin = Point.Zero;

        /// <summary>
        /// Set / get visibility.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; DoOnVisibilityChange(); }
        }

        /// <summary>
        /// Return element priority in drawing order and event handling.
        /// </summary>
        public virtual int Priority
        {
            get { return _indexInParent + PriorityBonus; }
        }

        /// <summary>
        /// Get if this element needs to recalculate destination rect.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
        }

        /// <summary>
        /// Is the element draggable (eg can a user grab it and drag it around).
        /// </summary>
        public bool Draggable
        {
            get { return _draggable; }
            set { _needToSetDragOffset = _draggable != value; _draggable = value; MarkAsDirty(); }
        }

        /// <summary>
        /// Optional background element that will not respond to events and will always be rendered right behind this element.
        /// </summary>
        public Element Background
        {
            get { return _background; }
            set
            {
                if (value != null && value._parent != null)
                {
                    throw new Exceptions.InvalidStateException("Cannot set background element that have a parent!");
                }
                _background = value;
            }
        }

        /// <summary>
        /// Current element state (default / mouse hover / mouse down..).
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public ElementState State
        {
            get { return _elementState; }
            set { _elementState = value; }
        }

        /// <summary>
        /// Find and return first occurance of a child element with a given identifier and specific type.
        /// </summary>
        /// <typeparam name="T">element type to get.</typeparam>
        /// <param name="identifier">Identifier to find.</param>
        /// <param name="recursive">If true, will search recursively in children of children. If false, will search only in direct children.</param>
        /// <returns>First found element with given identifier and type, or null if nothing found.</returns>
        public T Find<T>(string identifier, bool recursive = false) where T : Element
        {
            // should we return any element type?
            bool anyType = typeof(T) == typeof(Element);

            // iterate children
            foreach (Element child in _children)
            {
                // skip hidden elements
                if (child._hiddenInternalElement)
                    continue;

                // check if identifier and type matches - if so, return it
                if (child.Identifier == identifier && (anyType || (child.GetType() == typeof(T))))
                {
                    return (T)child;
                }

                // if recursive, search in child
                if (recursive)
                {
                    // search in child
                    T ret = child.Find<T>(identifier, recursive);

                    // if found return it
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }

            // not found?
            return null;
        }

        /// <summary>
        /// Find and return first occurance of a child element with a given identifier.
        /// </summary>
        /// <param name="identifier">Identifier to find.</param>
        /// <param name="recursive">If true, will search recursively in children of children. If false, will search only in direct children.</param>
        /// <returns>First found element with given identifier, or null if nothing found.</returns>
        public Element Find(string identifier, bool recursive = false)
        {
            return Find<Element>(identifier, recursive);
        }

        /// <summary>
        /// Iterate over children and call 'callback' for every direct child of this element.
        /// </summary>
        /// <param name="callback">Callback function to call with every child of this element.</param>
        public void IterateChildren(EventCallback callback)
        {
            foreach (Element child in _children)
            {
                callback(child);
            }
        }

        /// <summary>
        /// element current size property.
        /// </summary>
        public Vector2 Size
        {
            get { return _size; }
            set { if (_size != value) { _size = value; MarkAsDirty(); } }
        }

        /// <summary>
        /// Extra space (in pixels) to reserve *after* this element when using Auto Anchors.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 SpaceAfter
        {
            set { SetStyleProperty(StylePropertyIds.SpaceAfter, new StyleProperty(value)); }
            get { return GetActiveStyle(StylePropertyIds.SpaceAfter).asVector; }
        }

        /// <summary>
        /// Extra space (in pixels) to reserve *before* this element when using Auto Anchors.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 SpaceBefore
        {
            set { SetStyleProperty(StylePropertyIds.SpaceBefore, new StyleProperty(value)); }
            get { return GetActiveStyle(StylePropertyIds.SpaceBefore).asVector; }
        }

        /// <summary>
        /// element fill color - this is just a sugarcoat to access the default fill color style property.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Color FillColor
        {
            set { SetStyleProperty(StylePropertyIds.FillColor, new StyleProperty(value), markAsDirty: false); }
            get { return GetActiveStyle(StylePropertyIds.FillColor).asColor; }
        }

        /// <summary>
        /// element fill color opacity - this is just a sugarcoat to access the default fill color alpha style property.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public byte Opacity
        {
            set
            {
                Color col = FillColor;
                col.A = value;
                SetStyleProperty(StylePropertyIds.FillColor, new StyleProperty(col), markAsDirty: false);
            }
            get
            {
                return FillColor.A;
            }
        }

        /// <summary>
        /// element outline color opacity - this is just a sugarcoat to access the default outline color alpha style property.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public byte OutlineOpacity
        {
            set
            {
                Color col = OutlineColor;
                col.A = value;
                SetStyleProperty(StylePropertyIds.OutlineColor, new StyleProperty(col), markAsDirty: false);
            }
            get
            {
                return OutlineColor.A;
            }
        }

        /// <summary>
        /// element padding - this is just a sugarcoat to access the default padding style property.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 Padding
        {
            set { SetStyleProperty(StylePropertyIds.Padding, new StyleProperty(value)); }
            get { return GetActiveStyle(StylePropertyIds.Padding).asVector; }
        }

        /// <summary>
        /// element shadow color - this is just a sugarcoat to access the default shadow color style property.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Color ShadowColor
        {
            set { SetStyleProperty(StylePropertyIds.ShadowColor, new StyleProperty(value), markAsDirty: false); }
            get { return GetActiveStyle(StylePropertyIds.ShadowColor).asColor; }
        }

        /// <summary>
        /// element shadow scale - this is just a sugarcoat to access the default shadow scale style property.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public float ShadowScale
        {
            set { SetStyleProperty(StylePropertyIds.ShadowScale, new StyleProperty(value), markAsDirty: false); }
            get { return GetActiveStyle(StylePropertyIds.ShadowScale).asFloat; }
        }

        /// <summary>
        /// element shadow offset - this is just a sugarcoat to access the default shadow offset style property.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 ShadowOffset
        {
            set { SetStyleProperty(StylePropertyIds.ShadowOffset, new StyleProperty(value), markAsDirty: false); }
            get { return GetActiveStyle(StylePropertyIds.ShadowOffset).asVector; }
        }

        /// <summary>
        /// element scale - this is just a sugarcoat to access the default scale style property.
        /// </summary>
        public float Scale
        {
            set { SetStyleProperty(StylePropertyIds.Scale, new StyleProperty(value)); }
            get { return GetActiveStyle(StylePropertyIds.Scale).asFloat; }
        }

        /// <summary>
        /// element outline color - this is just a sugarcoat to access the default outline color style property.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Color OutlineColor
        {
            set { SetStyleProperty(StylePropertyIds.OutlineColor, new StyleProperty(value), markAsDirty: false); }
            get { return GetActiveStyle(StylePropertyIds.OutlineColor).asColor; }
        }

        /// <summary>
        /// element outline width - this is just a sugarcoat to access the default outline color style property.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public int OutlineWidth
        {
            set { SetStyleProperty(StylePropertyIds.OutlineWidth, new StyleProperty(value), markAsDirty: false); }
            get { return GetActiveStyle(StylePropertyIds.OutlineWidth).asInt; }
        }

        /// <summary>
        /// Return if this element is currently disabled, due to self or one of the parents / grandparents being disabled.
        /// </summary>
        /// <returns>True if element is disabled.</returns>
        public bool IsDisabled()
        {
            // iterate over parents until root, starting with self.
            // if any element along the way is disabled we return true.
            Element parent = this;
            while (parent != null)
            {
                if (!parent.Enabled) { return true; }
                parent = parent._parent;
            }

            // not disabled
            return false;
        }


        /// <summary>
        /// Check if this element is a descendant of another element.
        /// This goes up all the way to root.
        /// </summary>
        /// <param name="other">element to check if this element is descendant of.</param>
        /// <returns>True if this element is descendant of the other element.</returns>
        public bool IsDeepChildOf(Element other)
        {
            // iterate over parents until root, starting with self.
            // if any element along the way is child of 'other', we return true.
            Element parent = this;
            while (parent != null)
            {
                if (parent._parent == other) { return true; }
                parent = parent._parent;
            }

            // not child of
            return false;
        }

        /// <summary>
        /// Return if this element is currently locked, due to self or one of the parents / grandparents being locked.
        /// </summary>
        /// <returns>True if element is disabled.</returns>
        public bool IsLocked()
        {
            // iterate over parents until root, starting with self.
            // if any element along the way is locked we return true.
            Element parent = this;
            while (parent != null)
            {
                if (parent.Locked)
                {
                    // special case - if should do events even when parent is locked and direct parent, skip
                    if (DoEventsIfDirectParentIsLocked)
                    {
                        if (parent == _parent)
                        {
                            parent = parent._parent;
                            continue;
                        }
                    }

                    // if parent locked return true
                    return true;
                }

                // advance to next parent
                parent = parent._parent;
            }

            // not disabled
            return false;
        }

        /// <summary>
        /// Return if this element is currently visible, eg this and all its parents and grandparents are visible.
        /// </summary>
        /// <returns>True if element is really visible.</returns>
        public bool IsVisible()
        {
            // iterate over parents until root, starting with self.
            // if any element along the way is not visible we return false.
            Element parent = this;
            while (parent != null)
            {
                if (!parent.Visible) { return false; }
                parent = parent._parent;
            }

            // visible!
            return true;
        }

        /// <summary>
        /// Set the position and anchor of this element.
        /// </summary>
        /// <param name="anchor">New anchor to set.</param>
        /// <param name="offset">Offset from new anchor position.</param>
        public void SetPosition(Anchor anchor, Vector2 offset)
        {
            SetAnchor(anchor);
            SetOffset(offset);
        }

        /// <summary>
        /// Set the anchor of this element.
        /// </summary>
        /// <param name="anchor">New anchor to set.</param>
        public void SetAnchor(Anchor anchor)
        {
            _anchor = anchor;
            MarkAsDirty();
        }

        /// <summary>
        /// Set the offset of this element.
        /// </summary>
        /// <param name="offset">New offset to set.</param>
        public void SetOffset(Vector2 offset)
        {
            if (_offset != offset || _dragOffset != offset)
            {
                _dragOffset = _offset = offset;
                MarkAsDirty();
            }
        }

        /// <summary>
        /// Return children in a sorted list by priority.
        /// </summary>
        /// <returns>List of children sorted by priority.</returns>
        protected List<Element> GetSortedChildren()
        {
            // if need to sort children, rebuild the sorted list
            if (_needToSortChildren)
            {
                // create list to sort and return
                _sortedChildren = new List<Element>(_children);

                // get children in a sorted list
                _sortedChildren.Sort((x, y) =>
                    x.Priority.CompareTo(y.Priority));

                // no longer need to sort
                _needToSortChildren = false;
            }

            // return the sorted list
            return _sortedChildren;
        }

        /// <summary>
        /// Update dest rect and internal dest rect.
        /// This is called internally whenever a change is made to the element or its parent.
        /// </summary>
        virtual public void UpdateDestinationRects()
        {
            // update dest and internal dest rects
            _destRect = CalcDestRect();
            _destRectInternal = CalcInternalRect();

            // mark as no longer dirty
            _isDirty = false;

            // increase dest rect version and update parent last known
            _destRectVersion++;
            if (_parent != null) { _parentLastDestRectVersion = _parent._destRectVersion; }
        }

        /// <summary>
        /// Update dest rect and internal dest rect, but only if needed (eg if something changed since last update).
        /// </summary>
        virtual public void UpdateDestinationRectsIfDirty()
        {
            // if dirty, update destination rectangles
            if (_parent != null && (_isDirty || (_parentLastDestRectVersion != _parent._destRectVersion)))
            {
                UpdateDestinationRects();
            }
        }

        /// <summary>
        /// Draw this element and its children.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to use for drawing.</param>
        virtual public void Draw(SpriteBatch spriteBatch)
        {
            // if not visible skip
            if (!Visible)
            {
                return;
            }

            // update if disabled
            _isCurrentlyDisabled = IsDisabled();

            // do before draw event
            OnBeforeDraw(spriteBatch);

            // draw background
            if (Background != null)
            {
                _background._parent = this;
                _background._indexInParent = 0;
                _background.Draw(spriteBatch);
                _background._parent = null;
            }

            // calc desination rects (if needed)
            UpdateDestinationRectsIfDirty();

            // draw shadow
            DrawElementShadow(spriteBatch);

            // draw element outline
            DrawElementOutline(spriteBatch);

            // draw the element itself
            UserInterface.Active.DrawUtils.StartDraw(spriteBatch, _isCurrentlyDisabled);
            DrawElement(spriteBatch, DrawPhase.Base);
            UserInterface.Active.DrawUtils.EndDraw(spriteBatch);

            // do debug drawing
            if (DebugDraw)
            {
                DrawDebugStuff(spriteBatch);
            }

            // draw all child elements
            DrawChildren(spriteBatch);

            // do after draw event
            OnAfterDraw(spriteBatch);
        }

        /// <summary>
        /// Draw debug stuff for this element.
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to use for drawing.</param>
        protected virtual void DrawDebugStuff(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            // first draw whole dest rect
            var destRectCol = new Color(0f, 1f, 0.25f, 0.05f);
            spriteBatch.Draw(Resources.WhiteTexture, _destRect, destRectCol);

            // now draw internal dest rect
            var internalCol = new Color(1f, 0.5f, 0f, 0.5f);
            spriteBatch.Draw(Resources.WhiteTexture, _destRectInternal, internalCol);

            // draw space before
            var spaceColor = new Color(0f, 0f, 0.5f, 0.5f);
            if (SpaceBefore.X > 0)
            {
                spriteBatch.Draw(Resources.WhiteTexture,
                    new Rectangle((int)(_destRect.Left - _scaledSpaceBefore.X), _destRect.Y, (int)_scaledSpaceBefore.X, _destRect.Height), spaceColor);
            }
            if (SpaceBefore.Y > 0)
            {
                spriteBatch.Draw(Resources.WhiteTexture,
                    new Rectangle(_destRect.X, (int)(_destRect.Top - _scaledSpaceBefore.Y), _destRect.Width, (int)_scaledSpaceBefore.Y), spaceColor);
            }

            // draw space after
            spaceColor = new Color(0.5f, 0f, 0.5f, 0.5f);
            if (SpaceAfter.X > 0)
            {
                spriteBatch.Draw(Resources.WhiteTexture,
                    new Rectangle(_destRect.Right, _destRect.Y, (int)_scaledSpaceAfter.X, _destRect.Height), spaceColor);
            }
            if (SpaceAfter.Y > 0)
            {
                spriteBatch.Draw(Resources.WhiteTexture,
                    new Rectangle(_destRect.X, _destRect.Bottom, _destRect.Width, (int)_scaledSpaceAfter.Y), spaceColor);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Draw all children.
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected virtual void DrawChildren(SpriteBatch spriteBatch)
        {
            // do stuff before drawing children
            BeforeDrawChildren(spriteBatch);

            // get sorted children list
            List<Element> childrenSorted = GetSortedChildren();

            // draw all children
            foreach (Element child in childrenSorted)
            {
                child.Draw(spriteBatch);
            }

            // do stuff after drawing children
            AfterDrawChildren(spriteBatch);
        }

        /// <summary>
        /// Special init after deserializing element from file.
        /// </summary>
        internal protected virtual void InitAfterDeserialize()
        {
            // fix children parent
            var temp = _children;
            _children = new List<Element>();
            foreach (var child in temp)
            {
                child._parent = null;
                AddChild(child, child.InheritParentState);
            }

            // mark as dirty
            MarkAsDirty();

            // update all children
            foreach (var child in _children)
            {
                child.InitAfterDeserialize();
            }
        }

        /// <summary>
        /// Put all elements that have identifier property in a dictionary.
        /// Note: if multiple elements share the same identifier, the deepest element in hirarchy will end up in dict.
        /// </summary>
        /// <param name="dict">Dictionary to put elements into.</param>
        public void PopulateDict(ref Dictionary<string, Element> dict)
        {
            // add self if got identifier
            if (Identifier != null && Identifier.Length > 0)
                dict[Identifier] = this;

            // iterate children
            foreach (var child in _children)
            {
                child.PopulateDict(ref dict);
            }
        }

        /// <summary>
        /// Called before drawing child elements of this element.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw elements.</param>
        protected virtual void BeforeDrawChildren(SpriteBatch spriteBatch)
        {
        }

        /// <summary>
        /// Called after drawing child elements of this element.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw elements.</param>
        protected virtual void AfterDrawChildren(SpriteBatch spriteBatch)
        {
        }

        /// <summary>
        /// Draw element shadow (if defined shadow).
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to draw on.</param>
        virtual protected void DrawElementShadow(SpriteBatch spriteBatch)
        {
            // store current 'is-dirty' flag, because it changes internally while drawing shadow
            bool isDirty = _isDirty;

            // get current shadow color and if transparent skip
            Color shadowColor = ShadowColor;
            if (shadowColor.A == 0) { return; }

            // get shadow scale
            float shadowScale = ShadowScale;

            // update position to draw shadow
            _destRect.X += (int)ShadowOffset.X;
            _destRect.Y += (int)ShadowOffset.Y;

            // store previous state and colors
            Color oldFill = FillColor;
            Color oldOutline = OutlineColor;
            float oldScale = Scale;
            int oldOutlineWidth = OutlineWidth;
            ElementState oldState = _elementState;

            // set default colors and state for shadow pass
            FillColor = shadowColor;
            OutlineColor = Color.Transparent;
            OutlineWidth = 0;
            Scale = shadowScale;
            _elementState = ElementState.Default;

            // if disabled, turn color into greyscale
            if (_isCurrentlyDisabled)
            {
                FillColor = new Color(Color.White * (((shadowColor.R + shadowColor.G + shadowColor.B) / 3f) / 255f), shadowColor.A);
            }

            // draw with shadow effect
            UserInterface.Active.DrawUtils.StartDrawSilhouette(spriteBatch);
            DrawElement(spriteBatch, DrawPhase.Shadow);
            UserInterface.Active.DrawUtils.EndDraw(spriteBatch);

            // return position and colors back to what they were
            _destRect.X -= (int)ShadowOffset.X;
            _destRect.Y -= (int)ShadowOffset.Y;
            FillColor = oldFill;
            Scale = oldScale;
            OutlineColor = oldOutline;
            OutlineWidth = oldOutlineWidth;
            _elementState = oldState;

            // restore is-dirty flag
            _isDirty = isDirty;
        }

        /// <summary>
        /// Draw element outline.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to draw on.</param>
        virtual protected void DrawElementOutline(SpriteBatch spriteBatch)
        {
            // get outline width and if 0 return
            int outlineWidth = OutlineWidth;
            if (OutlineWidth == 0) { return; }

            // get outline color
            Color outlineColor = OutlineColor;

            // if disabled, turn outline to grey
            if (_isCurrentlyDisabled)
            {
                outlineColor = new Color(Color.White * (((outlineColor.R + outlineColor.G + outlineColor.B) / 3f) / 255f), outlineColor.A);
            }

            // store previous fill color
            Color oldFill = FillColor;

            // store original destination rect
            Rectangle originalDest = _destRect;
            Rectangle originalIntDest = _destRectInternal;

            // store element previous state
            ElementState oldState = _elementState;

            // set fill color
            SetStyleProperty(StylePropertyIds.FillColor, new StyleProperty(outlineColor), oldState, markAsDirty: false);

            // draw the element outline
            UserInterface.Active.DrawUtils.StartDrawSilhouette(spriteBatch);
            _destRect.Location = originalDest.Location + new Point(-outlineWidth, 0);
            DrawElement(spriteBatch, DrawPhase.Outline);
            _destRect.Location = originalDest.Location + new Point(0, -outlineWidth);
            DrawElement(spriteBatch, DrawPhase.Outline);
            _destRect.Location = originalDest.Location + new Point(outlineWidth, 0);
            DrawElement(spriteBatch, DrawPhase.Outline);
            _destRect.Location = originalDest.Location + new Point(0, outlineWidth);
            DrawElement(spriteBatch, DrawPhase.Outline);
            UserInterface.Active.DrawUtils.EndDraw(spriteBatch);

            // turn back to previous fill color
            SetStyleProperty(StylePropertyIds.FillColor, new StyleProperty(oldFill), oldState, markAsDirty: false);

            // return to the original destination rect
            _destRect = originalDest;
            _destRectInternal = originalIntDest;
        }

        /// <summary>
        /// The internal function to draw the element itself.
        /// Implemented by inheriting element types.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw on.</param>
        /// <param name="phase">The phase we are currently drawing.</param>
        virtual protected void DrawElement(SpriteBatch spriteBatch, DrawPhase phase)
        {
        }

        /// <summary>
        /// Called every frame after drawing is done.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw on.</param>
        virtual protected void OnAfterDraw(SpriteBatch spriteBatch)
        {
            AfterDraw?.Invoke(this);
            UserInterface.Active.AfterDraw?.Invoke(this);
        }

        /// <summary>
        /// Called every frame before drawing is done.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw on.</param>
        virtual protected void OnBeforeDraw(SpriteBatch spriteBatch)
        {
            BeforeDraw?.Invoke(this);
            UserInterface.Active.BeforeDraw?.Invoke(this);
        }

        /// <summary>
        /// Get the direct parent of this element.
        /// </summary>
        public Element Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Add a child element.
        /// </summary>
        /// <param name="child">element to add as child.</param>
        /// <param name="inheritParentState">If true, this element will inherit the parent's state (set InheritParentState property).</param>
        /// <param name="index">If provided, will be the index in the children array to push the new element.</param>
        /// <returns>The newly added element.</returns>
        public Element AddChild(Element child, bool inheritParentState = false, int index = -1)
        {
            // make sure don't already have a parent
            if (child._parent != null)
            {
                if (UserInterface.Active.SilentSoftErrors) return child;
                throw new Exceptions.InvalidStateException("Child element to add already got a parent!");
            }

            // need to sort children
            _needToSortChildren = true;

            // set inherit parent mode
            child.InheritParentState = inheritParentState;

            // set child's new parent
            child._parent = this;

            // if index is -1 or out of range, set to last item in childrens list
            if (index == -1 || index >= _children.Count)
            {
                index = _children.Count;
            }

            // add child at index
            child._indexInParent = index;
            _children.Insert(index, child);

            // update any siblings which need their index updating
            for (int i = index + 1; i < _children.Count; i++)
            {
                _children[i]._indexInParent += 1;
            }

            // reset child parent dest rect version
            child._parentLastDestRectVersion = _destRectVersion - 1;

            // mark child as dirty
            child.MarkAsDirty();
            MarkAsDirty();
            return child;
        }

        /// <summary>
        /// Bring this element to be on front (inside its parent).
        /// </summary>
        public void BringToFront()
        {
            Element parent = _parent;
            parent.RemoveChild(this);
            parent.AddChild(this);
        }

        /// <summary>
        /// Remove child element.
        /// </summary>
        /// <param name="child">element to remove.</param>
        public void RemoveChild(Element child)
        {
            // make sure don't already have a parent
            if (child._parent != this)
            {
                if (UserInterface.Active.SilentSoftErrors) return;
                throw new Exceptions.InvalidStateException("Child element to remove does not belong to this element!");
            }

            // need to sort children
            _needToSortChildren = true;

            // set parent to null and remove
            child._parent = null;
            child._indexInParent = -1;
            _children.Remove(child);

            // reset index for all children
            int index = 0;
            foreach (Element itrChild in _children)
            {
                itrChild._indexInParent = index++;
            }

            // mark child and self as dirty
            child.MarkAsDirty();
            MarkAsDirty();
        }

        /// <summary>
        /// Remove all children elements.
        /// </summary>
        public void ClearChildren()
        {
            // remove all children
            foreach (Element child in _children)
            {
                child._parent = null;
                child._indexInParent = -1;
                child.MarkAsDirty();
            }
            _children.Clear();

            // mark self as dirty
            _needToSortChildren = true;
            MarkAsDirty();
        }

        /// <summary>
        /// Calculate and return the internal destination rectangle (note: this relay on the dest rect having a valid value first).
        /// </summary>
        /// <returns>Internal destination rectangle.</returns>
        virtual public Rectangle CalcInternalRect()
        {
            // calculate the internal destination rect, eg after padding
            Vector2 padding = _scaledPadding;
            _destRectInternal = GetActualDestRect();
            _destRectInternal.X += (int)padding.X;
            _destRectInternal.Y += (int)padding.Y;
            _destRectInternal.Width -= (int)padding.X * 2;
            _destRectInternal.Height -= (int)padding.Y * 2;
            return _destRectInternal;
        }

        /// <summary>
        /// Takes a size value in vector, that can be in percents or units, and convert it to absolute
        /// size in pixels. For example, if given size is 0.5f this will calculate it to be half its parent
        /// size, as it should be.
        /// </summary>
        /// <param name="size">Size to calculate.</param>
        /// <returns>Actual size in pixels.</returns>
        protected Point CalcActualSizeInPixels(Vector2 size)
        {
            // simple case: if size is not in percents, just return as-is
            if (size.X > 1f && size.Y > 1f)
                return size.ToPoint();

            // get parent internal destination rectangle
            _parent.UpdateDestinationRectsIfDirty();
            Rectangle parentDest = _parent._destRectInternal;

            // calc and return size
            return new Point(
                (size.X == 0f ? parentDest.Width : (size.X > 0f && size.X < 1f ? (int)(parentDest.Width * size.X) : (int)size.X)),
                (size.Y == 0f ? parentDest.Height : (size.Y > 0f && size.Y < 1f ? (int)(parentDest.Height * size.Y) : (int)size.Y)));
        }

        /// <summary>
        /// Calculate and return the destination rectangle, eg the space this element is rendered on.
        /// </summary>
        /// <returns>Destination rectangle.</returns>
        virtual public Rectangle CalcDestRect()
        {
            // create new rectangle
            Rectangle ret = new Rectangle();

            // no parent? stop here and return empty rect
            if (_parent == null)
            {
                return ret;
            }

            // get parent internal destination rectangle
            _parent.UpdateDestinationRectsIfDirty();
            Rectangle parentDest = _parent._destRectInternal;

            // set size:
            // 0: takes whole parent size.
            // 0.0 - 1.0: takes percent of parent size.
            // > 1.0: size in pixels.
            Vector2 size = _scaledSize;
            Point sizeInPixels = CalcActualSizeInPixels(size);

            // apply min size
            if (MinSize != null)
            {
                Point minInPixels = CalcActualSizeInPixels(MinSize.Value);
                sizeInPixels.X = System.Math.Max(minInPixels.X, sizeInPixels.X);
                sizeInPixels.Y = System.Math.Max(minInPixels.Y, sizeInPixels.Y);
            }
            // apply max size
            if (MaxSize != null)
            {
                Point maxInPixels = CalcActualSizeInPixels(MaxSize.Value);
                sizeInPixels.X = System.Math.Min(maxInPixels.X, sizeInPixels.X);
                sizeInPixels.Y = System.Math.Min(maxInPixels.Y, sizeInPixels.Y);
            }

            // set return rect size
            ret.Width = sizeInPixels.X;
            ret.Height = sizeInPixels.Y;

            // make sure its a legal size
            if (ret.Width < 1) { ret.Width = 1; }
            if (ret.Height < 1) { ret.Height = 1; }

            // first calc some helpers
            int parent_left = parentDest.X;
            int parent_top = parentDest.Y;
            int parent_right = parent_left + parentDest.Width;
            int parent_bottom = parent_top + parentDest.Height;
            int parent_center_x = parent_left + parentDest.Width / 2;
            int parent_center_y = parent_top + parentDest.Height / 2;

            // get anchor and offset
            Anchor anchor = _anchor;
            Vector2 offset = _scaledOffset;

            // if we are in dragging mode we do a little hack to use top-left anchor with the dragged offset
            // note: but only if drag offset was previously set.
            if (_draggable && !_needToSetDragOffset)
            {
                anchor = Anchor.TopLeft;
                offset = _dragOffset;
            }

            // calculate position based on anchor, parent and offset
            switch (anchor)
            {
                case Anchor.Auto:
                case Anchor.AutoInline:
                case Anchor.AutoInlineNoBreak:
                case Anchor.TopLeft:
                    ret.X = parent_left + (int)offset.X;
                    ret.Y = parent_top + (int)offset.Y;
                    break;

                case Anchor.TopRight:
                    ret.X = parent_right - ret.Width - (int)offset.X;
                    ret.Y = parent_top + (int)offset.Y;
                    break;

                case Anchor.TopCenter:
                case Anchor.AutoCenter:
                    ret.X = parent_center_x - ret.Width / 2 + (int)offset.X;
                    ret.Y = parent_top + (int)offset.Y;
                    break;

                case Anchor.BottomLeft:
                    ret.X = parent_left + (int)offset.X;
                    ret.Y = parent_bottom - ret.Height - (int)offset.Y;
                    break;

                case Anchor.BottomRight:
                    ret.X = parent_right - ret.Width - (int)offset.X;
                    ret.Y = parent_bottom - ret.Height - (int)offset.Y;
                    break;

                case Anchor.BottomCenter:
                    ret.X = parent_center_x - ret.Width / 2 + (int)offset.X;
                    ret.Y = parent_bottom - ret.Height - (int)offset.Y;
                    break;

                case Anchor.CenterLeft:
                    ret.X = parent_left + (int)offset.X;
                    ret.Y = parent_center_y - ret.Height / 2 + (int)offset.Y;
                    break;

                case Anchor.CenterRight:
                    ret.X = parent_right - ret.Width - (int)offset.X;
                    ret.Y = parent_center_y - ret.Height / 2 + (int)offset.Y;
                    break;

                case Anchor.Center:
                    ret.X = parent_center_x - ret.Width / 2 + (int)offset.X;
                    ret.Y = parent_center_y - ret.Height / 2 + (int)offset.Y;
                    break;
            }

            // special case for auto anchors
            if ((anchor == Anchor.Auto || anchor == Anchor.AutoInline || anchor == Anchor.AutoCenter || anchor == Anchor.AutoInlineNoBreak) && _parent != null)
            {
                // get previous element before this
                Element prevelement = GetPreviouselement(true);

                // if found element before this one, align based on it
                if (prevelement != null)
                {
                    // make sure sibling is up-to-date
                    prevelement.UpdateDestinationRectsIfDirty();

                    // handle inline align
                    if (anchor == Anchor.AutoInline || anchor == Anchor.AutoInlineNoBreak)
                    {
                        ret.X = prevelement._destRect.Right + (int)(offset.X + prevelement._scaledSpaceAfter.X + _scaledSpaceBefore.X);
                        ret.Y = prevelement._destRect.Y;
                    }

                    // handle inline align that ran out of width / or auto anchor not inline
                    if ((anchor == Anchor.AutoInline && ret.Right > _parent._destRectInternal.Right) ||
                        (anchor == Anchor.Auto || anchor == Anchor.AutoCenter))
                    {
                        // align x
                        if (anchor != Anchor.AutoCenter)
                        {
                            ret.X = parent_left + (int)offset.X;
                        }

                        // align y
                        ret.Y = prevelement.GetDestRectForAutoAnchors().Bottom + (int)(offset.Y +
                            prevelement._scaledSpaceAfter.Y +
                            _scaledSpaceBefore.Y);
                    }
                }
                // if this is the first element in parent, apply space-before only
                else
                {
                    ret.X += (int)_scaledSpaceBefore.X;
                    ret.Y += (int)_scaledSpaceBefore.Y;
                }
            }

            // some extra logic for draggables
            if (_draggable)
            {
                // if need to init dragged offset, set it
                // this trick is used so if an object is draggable, we first evaluate its position based on anchor etc, and we use that
                // position as starting point for the dragging
                if (_needToSetDragOffset)
                {
                    _dragOffset.X = ret.X - parent_left;
                    _dragOffset.Y = ret.Y - parent_top;
                    _needToSetDragOffset = false;
                }

                // if draggable and need to be contained inside parent, validate it
                if (LimitDraggingToParentBoundaries)
                {
                    if (ret.X < parent_left) { ret.X = parent_left; _dragOffset.X = 0; }
                    if (ret.Y < parent_top) { ret.Y = parent_top; _dragOffset.Y = 0; }
                    if (ret.Right > parent_right) { _dragOffset.X -= ret.Right - parent_right; ; ret.X -= ret.Right - parent_right; }
                    if (ret.Bottom > parent_bottom) { _dragOffset.Y -= ret.Bottom - parent_bottom; ret.Y -= ret.Bottom - parent_bottom; }
                }
            }

            // return the newly created rectangle
            _destRect = ret;
            return ret;
        }

        /// <summary>
        /// Return actual destination rectangle.
        /// This can be override and implemented by things like Paragraph, where the actual destination rect is based on
        /// text content, font and word-wrap.
        /// </summary>
        /// <returns>The actual destination rectangle.</returns>
        virtual public Rectangle GetActualDestRect()
        {
            return _destRect;
        }

        /// <summary>
        /// Return the actual dest rect for auto-anchoring purposes.
        /// This is useful for things like DropDown, that when opened they take a larger part of the screen, but we don't
        /// want it to push down other elements.
        /// </summary>
        virtual protected Rectangle GetDestRectForAutoAnchors()
        {
            return GetActualDestRect();
        }

        /// <summary>
        /// Remove this element from its parent.
        /// </summary>
        public void RemoveFromParent()
        {
            if (_parent != null)
            {
                _parent.RemoveChild(this);
            }
        }

        /// <summary>
        /// Propagate all events trigger by this element to a given other element.
        /// For example, if "OnClick" will be called on this element, it will trigger OnClick on 'other' as well.
        /// </summary>
        /// <param name="other">element to propagate events to.</param>
        public virtual void PropagateEventsTo(Element other)
        {
            OnMouseDown += (Element element) => { other.OnMouseDown?.Invoke(other); };
            OnRightMouseDown += (Element element) => { other.OnRightMouseDown?.Invoke(other); };
            OnMouseReleased += (Element element) => { other.OnMouseReleased?.Invoke(other); };
            WhileMouseDown += (Element element) => { other.WhileMouseDown?.Invoke(other); };
            WhileRightMouseDown += (Element element) => { other.WhileRightMouseDown?.Invoke(other); };
            WhileMouseHover += (Element element) => { other.WhileMouseHover?.Invoke(other); };
            WhileMouseHoverOrDown += (Element element) => { other.WhileMouseHoverOrDown?.Invoke(other); };
            OnRightClick += (Element element) => { other.OnRightClick?.Invoke(other); };
            OnClick += (Element element) => { other.OnClick?.Invoke(other); };
            OnValueChange += (Element element) => { other.OnValueChange?.Invoke(other); };
            OnMouseEnter += (Element element) => { other.OnMouseEnter?.Invoke(other); };
            OnMouseLeave += (Element element) => { other.OnMouseLeave?.Invoke(other); };
            OnMouseWheelScroll += (Element element) => { other.OnMouseWheelScroll?.Invoke(other); };
            OnStartDrag += (Element element) => { other.OnStartDrag?.Invoke(other); };
            OnStopDrag += (Element element) => { other.OnStopDrag?.Invoke(other); };
            WhileDragging += (Element element) => { other.WhileDragging?.Invoke(other); };
            BeforeDraw += (Element element) => { other.BeforeDraw?.Invoke(other); };
            AfterDraw += (Element element) => { other.AfterDraw?.Invoke(other); };
            BeforeUpdate += (Element element) => { other.BeforeUpdate?.Invoke(other); };
            AfterUpdate += (Element element) => { other.AfterUpdate?.Invoke(other); };
        }

        /// <summary>
        /// Return the relative offset, in pixels, from parent top-left corner.
        /// </summary>
        /// <remarks>
        /// This return the offset between the top left corner of this element regardless of anchor type.
        /// </remarks>
        /// <returns>Calculated offset from parent top-left corner.</returns>
        public Vector2 GetRelativeOffset()
        {
            Rectangle dest = GetActualDestRect();
            Rectangle parentDest = _parent.GetActualDestRect();
            return new Vector2(dest.X - parentDest.X, dest.Y - parentDest.Y);
        }

        /// <summary>
        /// Return the element before this one in parent container, aka the next older sibling.
        /// </summary>
        /// <returns>element before this in parent, or null if first in parent or if orphan element.</returns>
        /// <param name="skipInvisibles">If true, will skip invisible elements, eg will return the first visible older sibling.</param>
        protected Element GetPreviouselement(bool skipInvisibles = false)
        {
            // no parent? skip
            if (_parent == null) { return null; }

            // get siblings and iterate them
            List<Element> siblings = _parent.Children;
            Element prev = null;
            foreach (Element sibling in siblings)
            {
                // when getting to self, break the loop
                if (sibling == this)
                {
                    break;
                }

                // if older sibling is invisible, skip it
                if (skipInvisibles && !sibling.Visible)
                {
                    continue;
                }

                // set prev
                prev = sibling;
            }

            // return the previous element (or null if wasn't found)
            return prev;
        }

        /// <summary>
        /// Handle mouse down event.
        /// </summary>
        virtual protected void DoOnMouseDown()
        {
            OnMouseDown?.Invoke(this);
            UserInterface.Active.OnMouseDown?.Invoke(this);
        }

        /// <summary>
        /// Handle mouse up event.
        /// </summary>
        virtual protected void DoOnMouseReleased()
        {
            OnMouseReleased?.Invoke(this);
            UserInterface.Active.OnMouseReleased?.Invoke(this);
        }

        /// <summary>
        /// Handle mouse click event.
        /// </summary>
        virtual protected void DoOnClick()
        {
            OnClick?.Invoke(this);
            UserInterface.Active.OnClick?.Invoke(this);
        }

        /// <summary>
        /// Handle mouse down event, called every frame while down.
        /// </summary>
        virtual protected void DoWhileMouseDown()
        {
            WhileMouseDown?.Invoke(this);
            UserInterface.Active.WhileMouseDown?.Invoke(this);
        }

        /// <summary>
        /// Handle mouse hover event, called every frame while hover.
        /// </summary>
        virtual protected void DoWhileMouseHover()
        {
            WhileMouseHover?.Invoke(this);
            UserInterface.Active.WhileMouseHover?.Invoke(this);
        }

        /// <summary>
        /// Handle mouse hover or down event, called every frame while hover.
        /// </summary>
        virtual protected void DoWhileMouseHoverOrDown()
        {
            // invoke callback and global callback
            WhileMouseHoverOrDown?.Invoke(this);
            UserInterface.Active.WhileMouseHoverOrDown?.Invoke(this);

            // do right mouse click event
            if (Input.MouseButtonClick(MouseButton.Right))
            {
                OnRightClick?.Invoke(this);
                UserInterface.Active.OnRightClick?.Invoke(this);
            }
            // do right mouse down event
            else if (Input.MouseButtonPressed(MouseButton.Right))
            {
                OnRightMouseDown?.Invoke(this);
                UserInterface.Active.OnRightMouseDown?.Invoke(this);
            }
            // do while right mouse down even
            else if (Input.MouseButtonDown(MouseButton.Right))
            {
                WhileRightMouseDown?.Invoke(this);
                UserInterface.Active.WhileRightMouseDown?.Invoke(this);
            }
        }

        /// <summary>
        /// Handle value change event (for elements with value).
        /// </summary>
        virtual protected void DoOnValueChange()
        {
            OnValueChange?.Invoke(this);
            UserInterface.Active.OnValueChange?.Invoke(this);
        }

        /// <summary>
        /// Handle mouse enter event.
        /// </summary>
        virtual protected void DoOnMouseEnter()
        {
            OnMouseEnter?.Invoke(this);
            UserInterface.Active.OnMouseEnter?.Invoke(this);
        }

        /// <summary>
        /// Handle mouse leave event.
        /// </summary>
        virtual protected void DoOnMouseLeave()
        {
            OnMouseLeave?.Invoke(this);
            UserInterface.Active.OnMouseLeave?.Invoke(this);
        }

        /// <summary>
        /// Handle start dragging event.
        /// </summary>
        virtual protected void DoOnStartDrag()
        {
            OnStartDrag?.Invoke(this);
            UserInterface.Active.OnStartDrag?.Invoke(this);
        }

        /// <summary>
        /// Handle end dragging event.
        /// </summary>
        virtual protected void DoOnStopDrag()
        {
            OnStopDrag?.Invoke(this);
            UserInterface.Active.OnStopDrag?.Invoke(this);
        }

        /// <summary>
        /// Handle the while-dragging event.
        /// </summary>
        virtual protected void DoWhileDragging()
        {
            WhileDragging?.Invoke(this);
            UserInterface.Active.WhileDragging?.Invoke(this);
        }

        /// <summary>
        /// Handle when mouse wheel scroll and this element is the active element.
        /// </summary>
        virtual protected void DoOnMouseWheelScroll()
        {
            OnMouseWheelScroll?.Invoke(this);
            UserInterface.Active.OnMouseWheelScroll?.Invoke(this);
        }

        /// <summary>
        /// Called every frame after update.
        /// </summary>
        virtual protected void DoAfterUpdate()
        {
            AfterUpdate?.Invoke(this);
            UserInterface.Active.AfterUpdate?.Invoke(this);
        }

        /// <summary>
        /// Called every time the visibility property of this element changes.
        /// </summary>
        virtual protected void DoOnVisibilityChange()
        {
            // invoke callbacks
            OnVisiblityChange?.Invoke(this);
            UserInterface.Active.OnVisiblityChange?.Invoke(this);
        }

        /// <summary>
        /// Called every frame before update.
        /// </summary>
        virtual protected void DoBeforeUpdate()
        {
            BeforeUpdate?.Invoke(this);
            UserInterface.Active.BeforeUpdate?.Invoke(this);
        }

        /// <summary>
        /// Called every time this element is focused / unfocused.
        /// </summary>
        virtual protected void DoOnFocusChange()
        {
            OnFocusChange?.Invoke(this);
            UserInterface.Active.OnFocusChange?.Invoke(this);
        }

        /// <summary>
        /// Test if a given point is inside element's boundaries.
        /// </summary>
        /// <remarks>This function result is affected by the 'UseActualSizeForCollision' flag.</remarks>
        /// <param name="point">Point to test.</param>
        /// <returns>True if point is in element's boundaries (destination rectangle)</returns>
        virtual public bool IsInsideElement(Vector2 point)
        {
            // adjust scrolling
            point += _lastScrollVal.ToVector2();

            // get rectangle for the test
            Rectangle rect = UseActualSizeForCollision ? GetActualDestRect() : _destRect;

            // now test detection
            return (point.X >= rect.Left - ExtraMargin.X && point.X <= rect.Right + ExtraMargin.X &&
                    point.Y >= rect.Top - ExtraMargin.Y && point.Y <= rect.Bottom + ExtraMargin.Y);
        }

        /// <summary>
        /// Return true if this element is naturally interactable, like buttons, lists, etc.
        /// Elements that are not naturally interactable are things like paragraph, colored rectangle, icon, etc.
        /// </summary>
        /// <remarks>This function should be overrided and implemented by different elements, and either return constant True or False.</remarks>
        /// <returns>True if element is naturally interactable.</returns>
        virtual public bool IsNaturallyInteractable()
        {
            return false;
        }

        /// <summary>
        /// Return if the mouse is currently pressing on this element (eg over it and left mouse button is down).
        /// </summary>
        public bool IsMouseDown { get { return _elementState == ElementState.MouseDown; } }

        /// <summary>
        /// Return if the mouse is currently over this element (regardless of whether or not mouse button is down).
        /// </summary>
        public bool IsMouseOver { get { return _isMouseOver; } }

        /// <summary>
        /// Mark that this element boundaries or style changed and it need to recalculate cached destination rect and other things.
        /// </summary>
        public void MarkAsDirty()
        {
            _isDirty = true;
        }

        /// <summary>
        /// Remove the IsDirty flag.
        /// </summary>
        /// <param name="updateChildren">If true, will set a flag that will still make children update.</param>
        internal void ClearDirtyFlag(bool updateChildren = false)
        {
            _isDirty = false;
            if (updateChildren)
            {
                _destRectVersion++;
            }
        }

        /// <summary>
        /// Called every frame to update the children of this element.
        /// </summary>
        /// <param name="targetElement">The deepest child element with highest priority that we point on and can be interacted with.</param>
        /// <param name="dragTargetElement">The deepest child dragable element with highest priority that we point on and can be drag if mouse down.</param>
        /// <param name="wasEventHandled">Set to true if current event was already handled by a deeper child.</param>
        /// <param name="scrollVal">Combined scrolling value (panels with scrollbar etc) of all parents.</param>
        virtual protected void UpdateChildren(ref Element targetElement, ref Element dragTargetElement, ref bool wasEventHandled, Point scrollVal)
        {
            // update all children (note: we go in reverse order so that elements on front will receive events before entites on back.
            List<Element> childrenSorted = GetSortedChildren();
            for (int i = childrenSorted.Count - 1; i >= 0; i--)
            {
                childrenSorted[i].Update(ref targetElement, ref dragTargetElement, ref wasEventHandled, scrollVal);
            }
        }

        /// <summary>
        /// Called every frame to update element state and call events.
        /// </summary>
        /// <param name="targetElement">The deepest child element with highest priority that we point on and can be interacted with.</param>
        /// <param name="dragTargetElement">The deepest child dragable element with highest priority that we point on and can be drag if mouse down.</param>
        /// <param name="wasEventHandled">Set to true if current event was already handled by a deeper child.</param>
        /// <param name="scrollVal">Combined scrolling value (panels with scrollbar etc) of all parents.</param>
        virtual public void Update(ref Element targetElement, ref Element dragTargetElement, ref bool wasEventHandled, Point scrollVal)
        {
            // set last scroll var
            _lastScrollVal = scrollVal;

            // check if should invoke the spawn effect
            if (_isFirstUpdate)
            {
                DoOnFirstUpdate();
                _isFirstUpdate = false;
            }

            // if inherit parent state just copy it and stop
            if (InheritParentState)
            {
                _elementState = _parent._elementState;
                _isMouseOver = _parent._isMouseOver;
                IsFocused = _parent.IsFocused;
                _isCurrentlyDisabled = _parent._isCurrentlyDisabled;
                return;
            }

            // get mouse position
            Vector2 mousePos = GetMousePos();

            // add our own scroll value to the combined scroll val before updating children
            scrollVal += OverflowScrollVal;

            // get if disabled
            _isCurrentlyDisabled = IsDisabled();

            // if disabled, invisible, or locked - skip
            if (_isCurrentlyDisabled || IsLocked() || !IsVisible())
            {
                // if this very element is locked (eg not locked due to parent being locked),
                // iterate children and invoke those with DoEventsIfDirectParentIsLocked setting
                if (Locked)
                {
                    for (int i = _children.Count - 1; i >= 0; i--)
                    {
                        if (_children[i].DoEventsIfDirectParentIsLocked)
                        {
                            _children[i].Update(ref targetElement, ref dragTargetElement, ref wasEventHandled, scrollVal);
                        }
                    }
                }

                // if was previously interactable, we might need to trigger some events
                if (_isInteractable)
                {
                    // if mouse was over, trigger mouse leave event
                    if (_elementState == ElementState.MouseHover)
                    {
                        DoOnMouseLeave();
                    }
                    // if mouse was down, trigger mouse up and leave events
                    else if (_elementState == ElementState.MouseDown)
                    {
                        DoOnMouseReleased();
                        DoOnMouseLeave();
                    }
                }

                // set to default and return
                _isInteractable = false;
                _elementState = ElementState.Default;
                return;
            }

            // if click-through is true, update children and stop here
            if (ClickThrough)
            {
                UpdateChildren(ref targetElement, ref dragTargetElement, ref wasEventHandled, scrollVal);
                return;
            }

            // update dest rect if needed (dest rect is calculated before draw, but is used for mouse collision detection as well,
            // so its better to calculate it here too in case something changed).
            UpdateDestinationRectsIfDirty();

            // set if interactable
            _isInteractable = true;

            // do before update event
            DoBeforeUpdate();

            // store previous state
            ElementState prevState = _elementState;

            // store previous mouse-over state
            bool prevMouseOver = _isMouseOver;

            // STEP 1: FIRST WE CALCULATE ELEMENT STATE (EG MOUST HOVER / MOUSE DOWN / ..)

            // only if event was not already catched by another element, check for events
            if (!wasEventHandled)
            {
                // if need to calculate state locally:
                if (!InheritParentState)
                {
                    // reset the mouse-over flag
                    _isMouseOver = false;
                    _elementState = ElementState.Default;

                    // set mouse state
                    if (IsInsideElement(mousePos))
                    {
                        // set self as the current target, unless a sibling got the event first
                        if (targetElement == null || targetElement._parent != _parent)
                        {
                            targetElement = this;
                        }

                        // mouse is over element
                        _isMouseOver = true;

                        // update mouse state
                        _elementState = (IsFocused || PromiscuousClicksMode || Input.MouseButtonPressed()) &&
                            Input.MouseButtonDown() ? ElementState.MouseDown : ElementState.MouseHover;
                    }
                }

                // set if focused
                if (Input.MouseButtonPressed())
                {
                    IsFocused = _isMouseOver;
                }
            }
            // if currently other element is targeted and mouse clicked, set focused to false
            else if (Input.MouseButtonClick())
            {
                IsFocused = false;
            }

            // STEP 2: NOW WE CALL ALL CHILDREN'S UPDATE

            // update all children
            UpdateChildren(ref targetElement, ref dragTargetElement, ref wasEventHandled, scrollVal);

            // check dragging after children so that the most nested element gets priority
            if ((_draggable || IsNaturallyInteractable()) && dragTargetElement == null && _isMouseOver && Input.MouseButtonPressed(MouseButton.Left))
            {
                dragTargetElement = this;
            }

            // STEP 3: CALL EVENTS

            // if selected target is this
            if (targetElement == this)
            {
                // handled events
                wasEventHandled = true;

                // call the while-mouse-hover-or-down handler
                DoWhileMouseHoverOrDown();

                // set mouse enter / mouse leave
                if (_isMouseOver && !prevMouseOver)
                {
                    DoOnMouseEnter();
                }

                // generate events
                if (prevState != _elementState)
                {
                    // mouse down
                    if (Input.MouseButtonPressed())
                    {
                        DoOnMouseDown();
                    }
                    // mouse up
                    if (Input.MouseButtonReleased())
                    {
                        DoOnMouseReleased();
                    }
                    // mouse click
                    if (Input.MouseButtonClick())
                    {
                        DoOnClick();
                    }
                }

                // call the while-mouse-down / while-mouse-hover events
                if (_elementState == ElementState.MouseDown)
                {
                    DoWhileMouseDown();
                }
                else
                {
                    DoWhileMouseHover();
                }
            }
            // if not current target, clear element state
            else
            {
                _elementState = ElementState.Default;
            }

            // mouse leave events
            if (!_isMouseOver && prevMouseOver)
            {
                DoOnMouseLeave();
            }

            // handle mouse wheel scroll over this element
            if (targetElement == this || UserInterface.Active.ActiveElement == this)
            {
                if (Input.MouseWheelChange != 0)
                {
                    DoOnMouseWheelScroll();
                }
            }

            // STEP 4: HANDLE DRAGGING FOR DRAGABLES

            // if draggable, and after calling all the children target is self, it means we are being dragged!
            if (_draggable && (dragTargetElement == this) && IsFocused)
            {
                // check if we need to start dragging the element that was not dragged before
                if (!_isBeingDragged && Input.MousePositionDiff.Length() != 0)
                {
                    // remove self from parent and add again. this trick is to keep the dragged element always on-top
                    Element parent = _parent;
                    RemoveFromParent();
                    parent.AddChild(this);

                    // set dragging mode = true, and call the do-start-dragging event
                    _isBeingDragged = true;
                    DoOnStartDrag();
                }

                // if being dragged..
                if (_isBeingDragged)
                {
                    // update drag offset and call the dragging event
                    _dragOffset += Input.MousePositionDiff;
                    DoWhileDragging();
                }
            }
            // if not currently dragged but was dragged last frane, call the end dragging event
            else if (_isBeingDragged)
            {
                _isBeingDragged = false;
                DoOnStopDrag();
                MarkAsDirty();
            }

            // if being dragged, mark as dirty
            if (_isBeingDragged)
            {
                MarkAsDirty();
            }

            // do after-update events
            DoAfterUpdate();
        }
    }
}