using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonsterFarm.Game.Util
{
    public class KeyboardHandler
    {
        public enum KeyTrigger {
            Press,
            Hold,
            Release
        }

        Dictionary<KeyTrigger, Dictionary<Keys, List<Action>>> _subscriptionRegistery;
        List<Keys> _keysDown;

        public KeyboardHandler(){
            _subscriptionRegistery = new Dictionary<KeyTrigger, Dictionary<Keys, List<Action>>>();
            foreach(KeyTrigger trigger in (KeyTrigger[])Enum.GetValues(typeof(KeyTrigger))){
                _subscriptionRegistery.Add(trigger, new Dictionary<Keys, List<Action>>());
            }
            _keysDown = new List<Keys>();
        }

        public void Unsubscribe(KeyValuePair<KeyTrigger, KeyValuePair<Keys, Action>> subscription)
        {
            KeyTrigger subscriptionTrigger = subscription.Key;
            if(_subscriptionRegistery.ContainsKey(subscriptionTrigger)){
                KeyValuePair<Keys, Action> subscriptionValue = subscription.Value;
                Keys subscriptionKey = subscriptionValue.Key;
                if(_subscriptionRegistery[subscriptionTrigger].ContainsKey(subscriptionKey)){
                    Action subscriptionAction = subscriptionValue.Value;
                    if(_subscriptionRegistery[subscriptionTrigger][subscriptionKey].Contains(subscriptionAction)){
                        _subscriptionRegistery[subscriptionTrigger][subscriptionKey].Remove(subscriptionAction);
                        if(_subscriptionRegistery[subscriptionTrigger][subscriptionKey].Count == 0){
                            _subscriptionRegistery[subscriptionTrigger].Remove(subscriptionKey);
                        }
                    }
                }
            }
        }

        public KeyValuePair<KeyTrigger, KeyValuePair<Keys, Action>> Subscribe(KeyTrigger trigger, Keys key, Action action){
            Dictionary<Keys, List<Action>> subscriptions = _subscriptionRegistery[trigger];
            if(!subscriptions.ContainsKey(key)){
                subscriptions.Add(key, new List<Action>());
            }
            if(!subscriptions[key].Contains(action)){
                subscriptions[key].Add(action);
            }
            return new KeyValuePair<KeyTrigger, KeyValuePair<Keys, Action>>(trigger, new KeyValuePair<Keys, Action>(key, action));
        }

        public void Update(GameTime gameTime) {
            KeyboardState state = Keyboard.GetState();

            foreach(Keys key in state.GetPressedKeys()){
                if(!_keysDown.Contains(key)){
                    _keysDown.Add(key);
                    if (_subscriptionRegistery[KeyTrigger.Press].ContainsKey(key)){
                        foreach(Action action in _subscriptionRegistery[KeyTrigger.Press][key]){
                            action.Invoke();
                        }
                    }
                }
            }
            List<Keys> doomed = new List<Keys>();
            foreach (Keys key in _keysDown){
                if (_subscriptionRegistery[KeyTrigger.Hold].ContainsKey(key)){
                    foreach (Action action in _subscriptionRegistery[KeyTrigger.Hold][key]){
                        action.Invoke();
                    }
                }
                if(state.IsKeyUp(key)){
                    doomed.Add(key);
                    if (_subscriptionRegistery[KeyTrigger.Release].ContainsKey(key)){
                        foreach (Action action in _subscriptionRegistery[KeyTrigger.Release][key]){
                            action.Invoke();
                        }
                    }
                }
            }
            foreach(Keys key in doomed){
                _keysDown.Remove(key);
            }
            doomed = null;
        }
    }
}
