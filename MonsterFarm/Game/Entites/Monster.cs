using System;
using System.Collections.Generic;
using System.Diagnostics;
using MonsterFarm.UI.Elements;

namespace MonsterFarm.Game.Entites
{
    [Serializable]
    public class InvalidMonsterNameException : System.Exception
    {
        public InvalidMonsterNameException AddMessage(string msg)
        {
            this.Data["UserMessage"] = msg;
            return this;
        }
    }

    public class Monster : Entity
    {
        private readonly char[] _families = { 'e', 'g', 'a', 'b', 'p', 's' };
        private Guid _guid = Guid.NewGuid();
        private string _id;
        private int _purity;
        private char _family;
        private char _line;
        private int _tier;
        private char[] _cousins;

        private IconType _monsterIcon;
        private IconType _familyIcon;
        private string _name;

        private Dictionary<string, string> _temp_name_map = new Dictionary<string, string>
        {
            { "es2", "Fire Snake" },
            { "ss1", "Slimey Boy" },
            { "bp2", "ManBagPig" }
        };

        private Dictionary<string, IconType> _temp_monster_icon_map = new Dictionary<string, IconType>
        {
            { "es2", IconType.RubyPink },
            { "ss1", IconType.PotionBlue },
            { "bp2", IconType.Sack }
        };

        private Dictionary<char, IconType> _temp_family_icon_map = new Dictionary<char, IconType>
        {
            { 'e', IconType.OrbRed },
            { 's', IconType.OrbBlue },
            { 'b', IconType.OrbGreen }
        };

        public Monster(string id)
        {
            _validate_id(id);
            initialize(id);

        }
        public Monster(string id, int purity)
        {
            _validate_id(id);
            initialize(id);
            _purity = purity;
        }

        private void initialize(string _id_) {
            _id = _id_;
            _purity = 0;
            _family = _id_[0];
            _line = _id_[1];
            _tier = int.Parse(_id_[2].ToString());
            int fam_pos = Array.IndexOf(_families, _family);
            int c1 = fam_pos - 1;
            if (c1 < 0) c1 = _families.Length - 1;
            int c2 = fam_pos + 1;
            if (c2 >= _families.Length) c2 = 0;
            _cousins = new char[] { _families[c1], _families[c2] };
            _name = _temp_name_map.ContainsKey(_id) ? _temp_name_map[_id_] : "Unknown";
            _monsterIcon = _temp_monster_icon_map.ContainsKey(_id_) ? _temp_monster_icon_map[_id_] : IconType.None;
            _familyIcon = _temp_family_icon_map.ContainsKey(_family) ? _temp_family_icon_map[_family] : IconType.None;
        }

        //Getters
        public Guid Guid { get { return _guid; } }
        public string Name { get { return _name; } }
        public IconType MonsterIcon { get { return _monsterIcon; } }
        public IconType FamilyIcon { get { return _familyIcon; } }
        public string Id { get { return _id; } }
        public int Purity { get { return _purity; } }
        public char Family { get { return _family; } }
        public char Line { get { return _line; } }
        public int Tier { get { return _tier; } }
        public char[] Couisins { get { return _cousins; } }

        private void _validate_id(string _id_){
            if(_id_.Length != 3
               || Array.IndexOf(_families, _id_[0]) == -1
               || Array.IndexOf(_families, _id_[1]) == -1
               || !Char.IsDigit(_id_[2])
            )
            {
                Exception ex = new InvalidMonsterNameException().AddMessage("Found: " + _id_);
                throw ex;
            }
        }

        public bool is_cousin(Monster monster){
            return Array.IndexOf(_cousins, monster.Family) > -1;
        }
        public Monster BreedWith(Monster mate){
            string offspring_id;
            int offspring_purity = 0;
            if (mate.is_cousin(this) && mate.Line == _family)
            {
                // Breeding with the pedigree's cousin can upgrade the offsprings tier if the mate's tier is sufficiently high
                // Oh.. not like that. Grow up...
                int new_tier = _tier;
                if (mate.Tier >= new_tier) new_tier++;
                offspring_id = _family.ToString() + mate.Family.ToString() + new_tier.ToString();
            }
            else if (mate.Family == _line)
            {
                // Breeding with a mate of the pedigree's bloodline can laterally move the offspring to the mate's tier
                offspring_id = _family.ToString() + mate.Family.ToString() + mate.Tier.ToString();
            }
            else if (mate.Id == _id)
            {
                // Breeding two of the same species will add purity to the offspring.
                offspring_id = _id;
                offspring_purity = _purity + 1;
            }
            else
            {
                // All other breeding will result in tier 1 offspring
                offspring_id = _family.ToString() + mate.Family.ToString() + "1";
            }
            Debug.WriteLine("Breed: "+_id+" + "+mate.Id+" = "+offspring_id+ " +"+offspring_purity.ToString());
            return new Monster(offspring_id, offspring_purity);
        }

    }
}
