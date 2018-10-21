namespace MonsterFarm.UI.DataTypes
{
    /// <summary>
    /// Meta data we attach to different textures.
    /// The values of these structs are defined in xml files that share the same name as the texture with _md sufix.
    /// It tells us things like the width of the frame (if texture is for panel), etc.
    /// </summary>
    /// 


    public class TextureData
    {
        public float FrameWidth;
        public float FrameHeight;

        public TextureData(float width = 0.2f, float height = 0.2f){
            FrameWidth = width;
            FrameHeight = height;
        }

    }
}
