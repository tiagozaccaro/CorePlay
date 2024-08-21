using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorePlay.SDK.Services
{
    public enum GamepadEventType
    {
        ButtonPressed,
        AxisMoved
    }

    public class GamepadAction(string name, Action action)
    {
        public string Name { get; } = name;
        public Action Action { get; } = action;
    }

    public class GamepadBinding(GamepadEventType eventType, List<int> keys, GamepadAction action)
    {
        public GamepadEventType EventType { get; } = eventType;
        public List<int> Keys { get; } = keys;
        public GamepadAction Action { get; } = action;
    }

    public interface IGamepadService : IDisposable
    {
        void BindAction(GamepadEventType eventType, List<int> keys, GamepadAction action);
        void Start();
    }
}
