namespace MonsterFarm.UI.DataTypes
{
    /// <summary>
    /// Meta data we attach to cursor textures.
    /// The values of these structs are defined in xml files that share the same name as the texture with _md sufix.
    /// </summary>
    public class CursorTextureData
    {
        public int OffsetX;
        public int OffsetY;
        public int DrawWidth;

        public CursorTextureData(int offsetX = 0, int offsetY = 0, int drawWidth = 64){
            OffsetX = offsetX;
            OffsetY = offsetY;
            DrawWidth = drawWidth;
        }
    }
}
