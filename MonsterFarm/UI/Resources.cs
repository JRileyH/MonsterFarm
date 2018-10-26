using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using MonsterFarm.UI.Elements;
using MonsterFarm.UI.DataTypes;
using System.Diagnostics;

namespace MonsterFarm.UI
{
    /// <summary>
    /// A class to get texture with index and constant path part.
    /// Used internally.
    /// </summary>
    public class TexturesGetter<TEnum> where TEnum : IConvertible
    {
        // textures we already loaded
        Texture2D[] _loadedTextures;

        /// <summary>
        /// Get texture for enum state.
        /// This is for textures that don't have different states, like mouse hover, down, or default.
        /// </summary>
        /// <param name="i">Texture enum identifier.</param>
        /// <returns>Loaded texture.</returns>
        public Texture2D this[TEnum i]
        {
            // get texture for a given type
            get
            {
                int indx = GetIndex(i);
                if (_loadedTextures[indx] == null)
                {
                    var path = Resources._root + _basepath + EnumToString(i) + _suffix;
                    Debug.WriteLine(path);
                    _loadedTextures[indx] = Resources._content.Load<Texture2D>(path);
                }
                return _loadedTextures[indx];
            }

            // force-override texture for a given type
            set
            {
                int indx = GetIndex(i);
                _loadedTextures[indx] = value;
            }
        }

        /// <summary>
        /// Get texture for enum state and element state.
        /// This is for textures that don't have different states, like mouse hover, down, or default.
        /// </summary>
        /// <param name="i">Texture enum identifier.</param>
        /// <param name="s">element state to get texture for.</param>
        /// <returns>Loaded texture.</returns>
        public Texture2D this[TEnum i, ElementState s]
        {
            // get texture for a given type and state
            get
            {
                int indx = GetIndex(i, s);
                if (_loadedTextures[indx] == null)
                {
                    var path = Resources._root + _basepath + EnumToString(i) + _suffix + StateEnumToString(s);
                    _loadedTextures[indx] = Resources._content.Load<Texture2D>(path);
                }
                return _loadedTextures[indx];
            }

            // force-override texture for a given type and state
            set
            {
                int indx = GetIndex(i, s);
                _loadedTextures[indx] = value;
            }
        }

        /// <summary>
        /// Get index from enum type with optional state.
        /// </summary>
        private int GetIndex(TEnum i, ElementState? s = null)
        {
            if (s != null)
                return (int)(object)i + (_typesCount * (int)s);
            return (int)(object)i;
        }

        /// <summary>
        /// Convert enum to its string for filename.
        /// </summary>
        private string EnumToString(TEnum e)
        {
            // element state enum
            if (typeof(TEnum) == typeof(ElementState))
            {
                return StateEnumToString((ElementState)(object)e);
            }

            // icon type enum
            if (typeof(TEnum) == typeof(IconType))
            {
                return e.ToString();
            }

            // all other type of enums
            return e.ToString().ToLower();
        }

        /// <summary>
        /// Convert element state enum to string.
        /// </summary>
        private string StateEnumToString(ElementState e)
        {
            switch (e)
            {
                case ElementState.MouseDown:
                    return "_down";
                case ElementState.MouseHover:
                    return "_hover";
                case ElementState.Default:
                    return string.Empty;
            }
            return null;
        }

        // base path of textures to load (index will be appended to them).
        string _basepath;

        // suffix to add to the end of texture path
        string _suffix;

        // do we use states like down / hover / default for these textures?
        bool _usesStates;

        // textures types count
        int _typesCount;

        /// <summary>
        /// Create the texture getter with base path.
        /// </summary>
        /// <param name="path">Resource path, under geonbit.ui content.</param>
        /// <param name="suffix">Suffix to add to the texture path after the enum part.</param>
        /// <param name="usesStates">If true, it means these textures may also use entit states, eg mouse hover / down / default.</param>
        public TexturesGetter(string path, string suffix = null, bool usesStates = true)
        {
            _basepath = path;
            _suffix = suffix ?? string.Empty;
            _usesStates = usesStates;
            _typesCount = Enum.GetValues(typeof(TEnum)).Length;
            _loadedTextures = new Texture2D[usesStates ? _typesCount * 3 : _typesCount];
        }
    }

    /// <summary>
    /// A static class to init and store all UI resources (textures, effects, fonts, etc.)
    /// </summary>
    public static class Resources
    {
        /// <summary>Just a plain white texture, used internally.</summary>
        public static Texture2D WhiteTexture { get { return _content.Load<Texture2D>(_root + "textures/white_texture"); } }

        /// <summary>Cursor textures.</summary>
        public static TexturesGetter<CursorType> Cursors = new TexturesGetter<CursorType>("textures/cursor_");

        /// <summary>Metadata about cursor textures.</summary>
        public static CursorTextureData[] CursorsData;

        /// <summary>All panel skin textures.</summary>
        public static TexturesGetter<PanelSkin> PanelTextures = new TexturesGetter<PanelSkin>("textures/panel_");

        /// <summary>Metadata about panel textures.</summary>
        public static TextureData[] PanelData;

        /// <summary>Button textures (accessed as [skin, state]).</summary>
        public static TexturesGetter<ButtonSkin> ButtonTextures = new TexturesGetter<ButtonSkin>("textures/button_");

        /// <summary>Metadata about button textures.</summary>
        public static TextureData[] ButtonData;

        /// <summary>CheckBox textures.</summary>
        public static TexturesGetter<ElementState> CheckBoxTextures = new TexturesGetter<ElementState>("textures/checkbox");

        /// <summary>Radio button textures.</summary>
        public static TexturesGetter<ElementState> RadioTextures = new TexturesGetter<ElementState>("textures/radio");

        /// <summary>ProgressBar texture.</summary>
        public static Texture2D ProgressBarTexture { get { return _content.Load<Texture2D>(_root + "textures/progressbar"); } }

        /// <summary>Metadata about progressbar texture.</summary>
        public static TextureData ProgressBarData;

        /// <summary>ProgressBar fill texture.</summary>
        public static Texture2D ProgressBarFillTexture { get { return _content.Load<Texture2D>(_root + "textures/progressbar_fill"); } }

        /// <summary>HorizontalLine texture.</summary>
        public static Texture2D HorizontalLineTexture { get { return _content.Load<Texture2D>(_root + "textures/horizontal_line"); } }

        /// <summary>Sliders base textures.</summary>
        public static TexturesGetter<SliderSkin> SliderTextures = new TexturesGetter<SliderSkin>("textures/slider_");

        /// <summary>Sliders mark textures (the sliding piece that shows current value).</summary>
        public static TexturesGetter<SliderSkin> SliderMarkTextures = new TexturesGetter<SliderSkin>("textures/slider_", "_mark");

        /// <summary>Metadata about slider textures.</summary>
        public static TextureData[] SliderData;

        /// <summary>All icon textures.</summary>
        public static TexturesGetter<IconType> IconTextures = new TexturesGetter<IconType>("textures/icons/");

        /// <summary>Icons inventory background texture.</summary>
        public static Texture2D IconBackgroundTexture { get { return _content.Load<Texture2D>(_root + "textures/icons/background"); } }

        /// <summary>Vertical scrollbar base texture.</summary>
        public static Texture2D VerticalScrollbarTexture { get { return _content.Load<Texture2D>(_root + "textures/scrollbar"); } }

        /// <summary>Vertical scrollbar mark texture.</summary>
        public static Texture2D VerticalScrollbarMarkTexture { get { return _content.Load<Texture2D>(_root + "textures/scrollbar_mark"); } }

        /// <summary>Metadata about scrollbar texture.</summary>
        public static TextureData VerticalScrollbarData;

        /// <summary>Arrow-down texture (used in dropdown).</summary>
        public static Texture2D ArrowDown { get { return _content.Load<Texture2D>(_root + "textures/arrow_down"); } }

        /// <summary>Arrow-up texture (used in dropdown).</summary>
        public static Texture2D ArrowUp { get { return _content.Load<Texture2D>(_root + "textures/arrow_up"); } }

        /// <summary>Default font types.</summary>
        public static SpriteFont[] Fonts;

        /// <summary>Effect for disabled entities (greyscale).</summary>
        public static Effect DisabledEffect;

        /// <summary>An effect to draw just a silhouette of the texture.</summary>
        public static Effect SilhouetteEffect;

        /// <summary>Store the content manager instance</summary>
        internal static ContentManager _content;

        /// <summary>Root for geonbit.ui content</summary>
        internal static string _root;

        /// <summary>
        /// Load all GeonBit.UI resources.
        /// </summary>
        /// <param name="content">Content manager to use.</param>
        static public void LoadContent(ContentManager content)
        {
            // set resources root path and store content manager
            _root = "UI/";
            _content = content;

            // load cursors metadata
            CursorsData = new CursorTextureData[Enum.GetValues(typeof(CursorType)).Length];
            CursorsData[(int)CursorType.Default] = new CursorTextureData(0, 0, 40);
            CursorsData[(int)CursorType.Pointer] = new CursorTextureData(-4, 0, 50);
            CursorsData[(int)CursorType.IBeam] = new CursorTextureData(0, -8, 40);

            // load panels
            PanelData = new TextureData[Enum.GetValues(typeof(PanelSkin)).Length];
            PanelData[(int)PanelSkin.Default] = new TextureData();
            PanelData[(int)PanelSkin.Fancy] = new TextureData();
            PanelData[(int)PanelSkin.Simple] = new TextureData();
            PanelData[(int)PanelSkin.Golden] = new TextureData();
            PanelData[(int)PanelSkin.ListBackground] = new TextureData();

            // load scrollbar metadata
            VerticalScrollbarData = new TextureData(0.0f, 0.3f);

            // load slider metadata
            SliderData = new TextureData[Enum.GetValues(typeof(SliderSkin)).Length];
            SliderData[(int)SliderSkin.Default] = new TextureData(0.03f, 0.0f);
            SliderData[(int)SliderSkin.Fancy] = new TextureData(0.28f, 0.0f);

            // load fonts
            Fonts = new SpriteFont[Enum.GetValues(typeof(FontStyle)).Length];
            foreach (FontStyle style in Enum.GetValues(typeof(FontStyle)))
            {
                Fonts[(int)style] = content.Load<SpriteFont>(_root + "fonts/" + style.ToString());
                Fonts[(int)style].LineSpacing += 2;
            }

            // load buttons metadata
            ButtonData = new TextureData[Enum.GetValues(typeof(ButtonSkin)).Length];
            ButtonData[(int)ButtonSkin.Default] = new TextureData(0.2f, 0.35f);
            ButtonData[(int)ButtonSkin.Alternative] = new TextureData(0.2f, 0.0f);
            ButtonData[(int)ButtonSkin.Fancy] = new TextureData(0.35f, 0.0f);


            // load progress bar metadata
            ProgressBarData = new TextureData(0.1375f, 0.0f);

            // load effects
            // These should be prebuilt as xnb files and copied by content manager instead of built
            DisabledEffect = content.Load<Effect>(_root + "effects/disabled");
            SilhouetteEffect = content.Load<Effect>(_root + "effects/silhouette");

            // Load default stylessheets up with values
            Element.DefaultStyle.SetStyleProperties(
                scale: new StyleProperty(1),
                fillColor: new StyleProperty(new Color(255, 255, 255, 255)),
                outlineColor: new StyleProperty(new Color(0, 0, 0, 0)),
                outlineWidth: new StyleProperty(0),
                forceAlignCenter: new StyleProperty(false),
                fontStyle: new StyleProperty((int)FontStyle.Regular),
                selectedHighlightColor: new StyleProperty(new Color(0, 0, 0, 0)),
                shadowColor: new StyleProperty(new Color(0, 0, 0, 0)),
                shadowOffset: new StyleProperty(new Vector2(0, 0)),
                padding: new StyleProperty(new Vector2(30, 30)),
                spaceBefore: new StyleProperty(new Vector2(0, 0)),
                spaceAfter: new StyleProperty(new Vector2(0, 8)),
                shadowScale: new StyleProperty(1)
            );
            Paragraph.DefaultStyle.SetStyleProperties(
                scale: new StyleProperty(1),
                fillColor: new StyleProperty(new Color(255, 255, 255, 255)),
                outlineColor: new StyleProperty(new Color(0, 0, 0, 255)),
                outlineWidth: new StyleProperty(2),
                forceAlignCenter: new StyleProperty(false),
                fontStyle: new StyleProperty((int)FontStyle.Regular)
            );
            Button.DefaultStyle.SetStyleProperties(
                scale: new StyleProperty(2.2f)
            );
            Button.DefaultParagraphStyle.SetStyleProperties(
                scale: new StyleProperty(1.2f)
            );
            Button.DefaultParagraphStyle.SetStyleProperties(
                state: ElementState.MouseDown,
                fillColor: new StyleProperty(new Color(170, 170, 170, 255))
            );
            CheckBox.DefaultParagraphStyle.SetStyleProperties(
                scale: new StyleProperty(1.15f)
            );
            CheckBox.DefaultParagraphStyle.SetStyleProperties(
                state: ElementState.MouseDown,
                fillColor: new StyleProperty(new Color(255, 255, 0, 255))
            );
            CheckBox.DefaultParagraphStyle.SetStyleProperties(
                state: ElementState.MouseHover,
                fillColor: new StyleProperty(new Color(255, 255, 0, 255))
            );
            ColoredRectangle.DefaultStyle.SetStyleProperties(
                fillColor: new StyleProperty(new Color(255, 255, 255, 255)),
                outlineColor: new StyleProperty(new Color(0, 0, 0, 255)),
                outlineWidth: new StyleProperty(2)
            );
            Header.DefaultStyle.SetStyleProperties(
                scale: new StyleProperty(1.2f),
                fillColor: new StyleProperty(new Color(255, 255, 0, 255))
            );
            Icon.DefaultStyle.SetStyleProperties(
                scale: new StyleProperty(1)
            );
            Icon.DefaultStyle.SetStyleProperties(
                state: ElementState.MouseHover,
                scale: new StyleProperty(1.1f)
            );
            Image.DefaultStyle.SetStyleProperties(
                scale: new StyleProperty(1),
                fillColor: new StyleProperty(new Color(255, 255, 255, 255))
            );
            Label.DefaultStyle.SetStyleProperties(
                scale: new StyleProperty(0.8f),
                fillColor: new StyleProperty(new Color(204, 204, 204, 255)),
                outlineColor: new StyleProperty(new Color(0, 0, 0, 255)),
                outlineWidth: new StyleProperty(2),
                forceAlignCenter: new StyleProperty(false),
                fontStyle: new StyleProperty((int)FontStyle.Regular)
            );
            Panel.DefaultStyle.SetStyleProperties(
                scale: new StyleProperty(2.2f),
                fillColor: new StyleProperty(new Color(255, 255, 255, 255))
            );
            ProgressBar.DefaultFillStyle.SetStyleProperties(
                fillColor: new StyleProperty(new Color(132, 204, 64, 255))
            );
            SelectList.DefaultStyle.SetStyleProperties(
                selectedHighlightColor: new StyleProperty(new Color(0, 0, 0, 100)),
                padding: new StyleProperty(new Vector2(30, 22))
            );
            SelectList.DefaultParagraphStyle.SetStyleProperties(
                scale: new StyleProperty(1.1f),
                fillColor: new StyleProperty(new Color(255, 255, 255, 255)),
                outlineColor: new StyleProperty(new Color(0, 0, 0, 255)),
                outlineWidth: new StyleProperty(2),
                forceAlignCenter: new StyleProperty(false),
                fontStyle: new StyleProperty((int)FontStyle.Regular)
            );
            SelectList.DefaultParagraphStyle.SetStyleProperties(
                state: ElementState.MouseDown,
                fillColor: new StyleProperty(new Color(255, 255, 0, 255))
            );
            SelectList.DefaultParagraphStyle.SetStyleProperties(
                state: ElementState.MouseHover,
                fillColor: new StyleProperty(new Color(255, 255, 0, 255))
            );
            TextInput.DefaultParagraphStyle.SetStyleProperties(
                scale: new StyleProperty(1.15f)
            );
            TextInput.DefaultPlaceholderStyle.SetStyleProperties(
                scale: new StyleProperty(1.15f),
                fillColor: new StyleProperty(new Color(150, 150, 150, 255))
            );
            PanelTabs.DefaultButtonStyle.SetStyleProperties(
                scale: new StyleProperty(2.2f)
            );
            PanelTabs.DefaultButtonParagraphStyle.SetStyleProperties(
                scale: new StyleProperty(1.2f)
            );
        }
    }
}
