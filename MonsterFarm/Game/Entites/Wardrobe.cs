using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public Texture2D getSheet(string race, string gender) {
            Texture2D blerg = _data[race][gender]["Skin"]["base"];
            return _data[race][gender]["Skin"]["base"];
        }
    }
}
