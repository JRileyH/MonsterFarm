using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterFarm.Game.Entites
{
    public class Wardrobe
    {
        string _root;

        Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Texture2D>>>> _data = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Texture2D>>>>();
        //TODO: enumerate these
        string[] _races = { "Dwarf", "Elf", "Human", "Rodentian", "Tongo", "Umborian" };
        string[] _genders = { "Male", "Female" };
        string[] _articles = { "Hat", "Hair", "Shirt", "Pants", "Shoes", "Skin" };

        public Wardrobe(string root)
        {
            _root = root;
        }

        public Wardrobe LoadContent(ContentManager content, GraphicsDevice graphicsDevice) {
            foreach (string race in _races)
            {
                _data[race] = new Dictionary<string, Dictionary<string, Dictionary<string, Texture2D>>>();
                foreach (string gender in _genders)
                {
                    _data[race][gender] = new Dictionary<string, Dictionary<string, Texture2D>>();
                    foreach (string article in _articles)
                    {
                        _data[race][gender][article] = new Dictionary<string, Texture2D>();
                        string directory = _root + race + "/" + gender + "/" + article + "/";
                        if (Directory.Exists(@"Content/" + directory))
                        {
                            string[] files = Directory.GetFiles(@"Content/" + directory, "*.xnb", SearchOption.AllDirectories);
                            foreach (string file in files)
                            {
                                string[] pathArr = file.Split('/');
                                string sheet = pathArr[pathArr.Length - 1].Split('.')[0];
                                _data[race][gender][article][sheet] = content.Load<Texture2D>(directory+sheet);
                            }
                        }
                    }
                }
            }
            return this;
        }

        private void applyLayer(ref Texture2D sheet, Texture2D layer, Color color) {
            Color highlight = new Color(Math.Min(color.R + 20, 255), Math.Min(color.G + 20, 255), Math.Min(color.B + 20, 255));
            Color shadow = new Color(Math.Max(color.R - 20, 0), Math.Max(color.G - 20, 0), Math.Max(color.B - 20, 0));
            Color[] sheetData = new Color[sheet.Width * sheet.Height];
            Color[] layerData = new Color[layer.Width * layer.Height];
            if (sheetData.Length != layerData.Length) throw new Exception("The fuck you tryin ta do?");
            sheet.GetData(sheetData);
            layer.GetData(layerData);
            for (int i = 0; i < layerData.Length; i++) {
                Color c = layerData[i];
                if (c.A > 0)
                {
                    if (c.R == c.G && c.G == c.B && c.B == 0)
                    {
                        c = new Color(shadow, c.A);
                    }
                    else if (c.R == c.G && c.G == c.B && c.B == 127)
                    {
                        c = new Color(color, c.A);
                    }
                    else if (c.R == c.G && c.G == c.B && c.B == 255)
                    {
                        c = new Color(highlight, c.A);
                    }
                    sheetData[i] = c;
                }
            }
            sheet.SetData(sheetData);
        }

        public Texture2D GetSheet(string race, string gender) {
            Texture2D skin = _data[race][gender]["Skin"]["base"];
            applyLayer(ref skin, skin, new Color(211, 175, 142, 255));
            applyLayer(ref skin, _data[race][gender]["Shoes"]["boots"], new Color(66, 60, 53, 255));
            applyLayer(ref skin, _data[race][gender]["Pants"]["trousers"], new Color(147, 130, 107, 255));
            applyLayer(ref skin, _data[race][gender]["Shirt"]["tunic"], new Color(221, 86, 59, 255));
            applyLayer(ref skin, _data[race][gender]["Hair"]["balding"], new Color(188, 130, 58, 255));
            return skin;
        }
    }
}
