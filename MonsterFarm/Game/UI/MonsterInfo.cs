using System;
using Microsoft.Xna.Framework;
using MonsterFarm.Game.Entites;
using MonsterFarm.UI;
using MonsterFarm.UI.Elements;

namespace MonsterFarm.Game.UI
{
    public class MonsterInfo
    {
        private bool _uiVisible = false;
        private Panel _panel;
        private Monster _monster = null;

        public delegate void _callback(Monster monster);
        public _callback OnKill = null;
        public _callback OnClose = null;

        public MonsterInfo(UserInterface ui) {
            _panel = new Panel(new Vector2(500, 300), offset: new Vector2(0, -200));
            _panel.Visible = false;
            ui.AddElement(_panel);
        }

        //UI code
        public void Show(Monster monster) {
            if (!_uiVisible) {
                _uiVisible = true;
                _panel.Visible = true;
            }
            _populateInfo(monster);
        }
        public void Hide() {
            if (_uiVisible) {
                _uiVisible = false;
                _panel.Visible = false;
                OnClose?.Invoke(_monster);
                _monster = null;
            }
        }
        private void _populateInfo(Monster monster) {
            _monster = monster;
            _panel.ClearChildren();
            _panel.AddChild(new Image(Resources.IconTextures[monster.MonsterIcon], size: new Vector2(120, 120)));
            _panel.AddChild(new Icon(monster.FamilyIcon, anchor: Anchor.AutoInline, offset: new Vector2(10, 0)));
            _panel.AddChild(new Paragraph(monster.Name, anchor: Anchor.AutoInline, size: new Vector2(200, 50), offset: new Vector2(10, 30)));
            Button _closeButton = new Button("X", anchor: Anchor.AutoInline, size: new Vector2(40, 40), textScale: 0.7f, nobreak: true);
            _closeButton.OnClick = (Element btn) => {
                Hide();
            };
            _panel.AddChild(_closeButton);
            HorizontalLine hl = new HorizontalLine(Anchor.TopRight, offset: new Vector2(0, 50));
            hl.Size = new Vector2(300, 8);
            _panel.AddChild(hl);
            _panel.AddChild(new MulticolorParagraph("{{RED}}HP{{WHITE}}: {{YELLOW}}290{{WHITE}}/400{{BLUE}}\nM{{WHITE}}P: 17/17", Anchor.Center, offset: new Vector2(0, -30)));

            Button _killButton = new Button("Kill", anchor: Anchor.BottomRight, size: new Vector2(90, 40), nobreak: true);
            _killButton.OnClick = (Element btn) => {
                OnKill?.Invoke(_monster);
                Hide();
            };
            _panel.AddChild(_killButton);
        }
    }
}
