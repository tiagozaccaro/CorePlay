using SDL2;

namespace CorePlay.SDK.Services
{
    public class GamepadService : IGamepadService
    {
        private nint? _controller;
        private bool _running;
        private readonly List<GamepadBinding> _bindings = [];
        private readonly HashSet<int> _pressedButtons = [];
        private readonly HashSet<int> _movedAxes = [];

        public GamepadService()
        {
            InitializeSDL();
        }

        private void InitializeSDL()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER) < 0)
            {
                Console.WriteLine($"SDL could not initialize! SDL_Error: {SDL.SDL_GetError()}");
                return;
            }

            int numJoysticks = SDL.SDL_NumJoysticks();
            if (numJoysticks > 0 && SDL.SDL_IsGameController(0) == SDL.SDL_bool.SDL_TRUE)
            {
                _controller = SDL.SDL_GameControllerOpen(0);
                if (_controller != null)
                {
                    Console.WriteLine($"Controller 0: {SDL.SDL_GameControllerName(_controller.Value)} opened.");
                }
                else
                {
                    Console.WriteLine("Could not open controller.");
                }
            }
            else
            {
                Console.WriteLine("No game controller found.");
            }
        }

        public void BindAction(GamepadEventType eventType, List<int> keys, GamepadAction action)
        {
            _bindings.Add(new GamepadBinding(eventType, keys, action));
        }

        public void Start()
        {
            _running = true;
            Task.Run(() =>
            {
                while (_running)
                {
                    while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
                    {
                        switch (e.type)
                        {
                            case SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                            case SDL.SDL_EventType.SDL_CONTROLLERBUTTONUP:
                                HandleButtonEvent(e);
                                break;

                            case SDL.SDL_EventType.SDL_CONTROLLERAXISMOTION:
                                HandleAxisEvent(e);
                                break;

                            case SDL.SDL_EventType.SDL_QUIT:
                                _running = false;
                                break;
                        }
                    }

                    CheckBindings();
                }

                Cleanup();
            });
        }

        private void HandleButtonEvent(SDL.SDL_Event e)
        {
            if (e.type == SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN)
            {
                _pressedButtons.Add(e.cbutton.button);
            }
            else if (e.type == SDL.SDL_EventType.SDL_CONTROLLERBUTTONUP)
            {
                _pressedButtons.Remove(e.cbutton.button);
            }
        }

        private void HandleAxisEvent(SDL.SDL_Event e)
        {
            if (e.type == SDL.SDL_EventType.SDL_CONTROLLERAXISMOTION)
            {
                if (e.caxis.axisValue != 0)
                {
                    _movedAxes.Add(e.caxis.axis);
                }
                else
                {
                    _movedAxes.Remove(e.caxis.axis);
                }
            }
        }

        private void CheckBindings()
        {
            foreach (var binding in _bindings)
            {
                bool isTriggered = false;

                if (binding.EventType == GamepadEventType.ButtonPressed)
                {
                    isTriggered = binding.Keys.TrueForAll(key => _pressedButtons.Contains(key));
                }
                else if (binding.EventType == GamepadEventType.AxisMoved)
                {
                    isTriggered = binding.Keys.TrueForAll(axis => _movedAxes.Contains(axis));
                }

                if (isTriggered)
                {
                    binding.Action.Action?.Invoke();
                }
            }
        }

        private void Cleanup()
        {
            if (_controller != null)
            {
                SDL.SDL_GameControllerClose(_controller.Value);
            }
            SDL.SDL_Quit();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _running = false;
            Cleanup();
        }
    }
}
