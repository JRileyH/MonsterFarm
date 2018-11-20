using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonsterFarm.Game.Entites;
using MonsterFarm.UI;
using MonsterFarm.UI.Elements;

namespace MonsterFarm.Game.UI
{
    public class Menagerie
    {
        private List<Monster> _monsterList = new List<Monster>();
        private Dictionary<Guid, Element[]> _monsterRows = new Dictionary<Guid, Element[]>();
        private bool _uiVisible = false;
        private char? _filter = null;
        private Panel _panel;
        private Panel _listPanel;
        private Icon _allFilter;
        private Dictionary<char, Icon> _familyFilters;
        private Monster _selectedMonster = null;

        public delegate void _monsterCallback(Monster monster);
        public delegate void _familyCallback(char family);
        public _monsterCallback OnSelect = null;
        public _monsterCallback OnClose = null;
        public _familyCallback OnChange = null;

        public Menagerie(UserInterface ui){
            _panel = new Panel(new Vector2(500, 700), anchor: Anchor.CenterLeft, offset: new Vector2(30, 0));
            _panel.Visible = false;
            _panel.AddChild(new Header("Monster List:"));
            Button _closeButton = new Button("X", anchor: Anchor.TopRight, size: new Vector2(40, 40), textScale: 0.7f, nobreak: true);
            _closeButton.OnClick = (Element btn) => {
                Hide();
            };
            _panel.AddChild(_closeButton);

            _allFilter = new Icon(IconType.Book, anchor: Anchor.CenterLeft, offset: new Vector2(0, -260), background: true);
            _familyFilters = new Dictionary<char, Icon> {
                { 'e', new Icon(IconType.EtheranFamily, anchor: Anchor.AutoInline) },
                { 'g', new Icon(IconType.GianFamily, anchor: Anchor.AutoInline) },
                { 'c', new Icon(IconType.ChitanFamily, anchor: Anchor.AutoInline) },
                { 'b', new Icon(IconType.BeastFamily, anchor: Anchor.AutoInline) },
                { 'f', new Icon(IconType.FloranFamily, anchor: Anchor.AutoInline) },
                { 's', new Icon(IconType.SlimeFamily, anchor: Anchor.AutoInline) },
            };
            _allFilter.OnClick = (Element btn) => {
                Filter();
                _allFilter.DrawBackground = true;
            };
            _panel.AddChild(_allFilter);

            foreach (KeyValuePair<char, Icon> filter in _familyFilters) {
                _familyFilters[filter.Key].OnClick = (Element btn) => {
                    Filter(filter.Key);
                    filter.Value.DrawBackground = true;
                };
                _panel.AddChild(filter.Value);
            }

            _listPanel = new Panel(new Vector2(470, 550), anchor: Anchor.BottomCenter, skin: PanelSkin.ListBackground);
            _listPanel.PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
            _panel.AddChild(_listPanel);
            ui.AddElement(_panel);
        }

        //Getters
        public List<Monster> MonsterList(){
            return _monsterList;
        }
        public List<Monster> MonsterList(char family){
            return _monsterList.FindAll(
                delegate(Monster monster) {
                    return monster.Family == family;
                }
            );

        }

        // Adding and removing
        public void AddMonster(Monster monster){
            _monsterList.Add(monster);
            if(_uiVisible && (_filter == null || _filter == monster.Family)){
                _createMonsterRow(monster);
            }
        }
        public void RemoveMonster(Monster monster){
            _monsterList.Remove(monster);
            if (_uiVisible)
            {
                _clearMonsterRow(monster);
            }
        }

        // Selection
        public Monster SelectedMonster(){
            return _selectedMonster;
        }

        private void _select(Monster monster){
            _selectedMonster = monster;
            OnSelect?.Invoke(monster);
        }
        public void Filter(char? family = null){
            if (_uiVisible){
                _listPanel.ClearChildren();
                _monsterRows.Clear();
                OnChange?.Invoke((char)family);
                List<Monster> _ml = family == null ? _monsterList : MonsterList((char)family);
                foreach (Monster monster in _ml)
                {
                    _createMonsterRow(monster);
                }
                _filter = family;
            }
            _allFilter.DrawBackground = false;
            foreach (KeyValuePair<char, Icon> f in _familyFilters) {
                f.Value.DrawBackground = false;
            };
        }

        //UI code
        public void Show(){
            if (!_uiVisible) {
                _uiVisible = true;
                _panel.Visible = true;
                foreach (Monster monster in _monsterList)
                {
                    _createMonsterRow(monster);
                }
            }
        }
        public void Hide(){
            if (_uiVisible) {
                _listPanel.ClearChildren();
                _monsterRows.Clear();
                _uiVisible = false;
                _panel.Visible = false;
                OnClose?.Invoke(_selectedMonster);
                _selectedMonster = null;
                _filter = null;
            }
        }

        private void _clearMonsterRow(Monster monster){
            foreach (Element elem in _monsterRows[monster.Guid]){
                _listPanel.RemoveChild(elem);
            }
            _monsterRows.Remove(monster.Guid);
        }
        private void _createMonsterRow(Monster monster){
            Button monsterSelectButton = new Button("Select", anchor: Anchor.AutoInline, size: new Vector2(70, 40), textScale: 0.7f, nobreak: true);
            monsterSelectButton.OnClick = (Element btn) => {
                _select(monster);
            };
            Element[] _monsterRowElements = {
                new Icon(monster.MonsterIcon, anchor: Anchor.AutoInline, background: true),
                new Icon(monster.FamilyIcon, anchor: Anchor.AutoInline, offset: new Vector2(10, 0)),
                new Paragraph(monster.Name, anchor: Anchor.AutoInline, size: new Vector2(185, 50), offset: new Vector2(10, 30)),
                monsterSelectButton,
                new HorizontalLine(Anchor.Auto, offset: new Vector2(0, 15))
            };
            if (!_monsterRows.ContainsKey(monster.Guid)) {
                _monsterRows.Add(monster.Guid, _monsterRowElements);
                foreach (Element elem in _monsterRowElements)
                {
                    _listPanel.AddChild(elem);
                }
            }
        }
    }
}
