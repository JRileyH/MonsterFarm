using System;
using System.Diagnostics;

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
        private string _id;
        private int _purity;
        private char _family;
        private char _line;
        private int _tier;
        private char[] _cousins;

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
        }

        //Getters
        public string id { get { return _id; } }
        public int purity { get { return _purity; } }
        public char family { get { return _family; } }
        public char line { get { return _line; } }
        public int tier { get { return _tier; } }
        public char[] couisins { get { return _cousins; } }

        private void _validate_id(string _id_){
            if(_id_.Length != 3
               || Array.IndexOf(_families, _id_[0]) == -1
               || Array.IndexOf(_families, _id_[1]) == -1
               || !Char.IsDigit(_id_[2])
            )
            {
                Exception ex = new InvalidMonsterNameException().AddMessage("Found: " + _id_);
                Debug.WriteLine(_id_);
                throw ex;
            }
        }

        public bool is_cousin(Monster monster){
            return Array.IndexOf(_cousins, monster.family) > -1;
        }
        public Monster breed_with(Monster mate){
            string offspring_id;
            int offspring_purity = 0;
            if (mate.is_cousin(this) && mate.line == _family)
            {
                // Breeding with the pedigree's cousin can upgrade the offsprings tier if the mate's tier is sufficiently high
                // Oh.. not like that. Grow up...
                int new_tier = _tier;
                if (mate.tier >= new_tier) new_tier++;
                offspring_id = _family.ToString() + mate.family.ToString() + new_tier.ToString();
            }
            else if (mate.family == _line)
            {
                // Breeding with a mate of the pedigree's bloodline can laterally move the offspring to the mate's tier
                offspring_id = _family.ToString() + mate.family.ToString() + mate.tier.ToString();
            }
            else if (mate.id == _id)
            {
                // Breeding two of the same species will add purity to the offspring.
                offspring_id = _id;
                offspring_purity = _purity + 1;
            }
            else
            {
                // All other breeding will result in tier 1 offspring
                offspring_id = _family.ToString() + mate.family.ToString() + "1";
            }
            Debug.WriteLine("Breed: "+_id+" + "+mate.id+" = "+offspring_id+ " +"+offspring_purity.ToString());
            return new Monster(offspring_id, offspring_purity);
        }

    }
}
