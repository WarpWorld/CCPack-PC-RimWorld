using System.Collections.Generic;
using Verse;
using System.Linq;

namespace CrowdControl {
    public class EffectManager : GameComponent {

        private static Queue<EffectCommand> CommandQueue = new Queue<EffectCommand>();
        private static List<TimedEffect> TimedEffects = new List<TimedEffect>();

        private static Dictionary<string, Effect> EffectList;
        private static EffectListener EffectListener;
        private static Game Game = null;

        private int _counter = 0;
        private int _reconnectCounter = 0;
        private const int EFFECT_THROTTLE = 300;
        private const int RECONNECT_THROTTLE = 600;

        public EffectManager(Game game) {
            Game = game;
            EffectList = ModService.Instance.EffectList;
            ModService.Instance.EffectManager = this;
            ModService.Instance.Game = game;
        }

        public override void StartedNewGame() {
            EnsureListenerStarted();
        }

        public override void LoadedGame() {
            EnsureListenerStarted();
        }

        public override void FinalizeInit() {
            EnsureListenerStarted();
        }

        public void EnsureConnection() {
            EnsureListenerStarted();
        }

        private void EnsureListenerStarted() {
            if (EffectListener == null) {
                EffectListener = new EffectListener(hostname: ModService.Instance.Hostname, port: ModService.Instance.Port);
                EffectListener.OnEffect += OnEffectRecieved;
            }

            if (EffectListener.GetConnectionStatus() != ConnectorStatus.Connected) {
                EffectListener.StartBackgroundListener();
            }
        }

        public override void GameComponentTick() {
            if (EffectListener == null) {
                EnsureListenerStarted();
            }
            else if (EffectListener.connected == false) {
                _reconnectCounter++;
                if (_reconnectCounter >= RECONNECT_THROTTLE) {
                    _reconnectCounter = 0;
                    EnsureListenerStarted();
                }
            }
            else {
                _reconnectCounter = 0;
            }

            if (EffectListener != null && EffectListener.connected) {
                ProcessEffectQueue();
                HandleTimedEffects();
            }

            if (EffectListener != null) {
                EffectListener.live = 10;
            }
        }

        public override void GameComponentUpdate()
        {
            if (Verse.Find.TickManager.Paused && EffectListener.connected) 
            {
                PauseEffectQueue();
                EffectListener.live = 10;
            }
        }

        public string GetConnectionStatusCode() {
            string statusCode = "Network.Disconnected";
            if (EffectListener != null) {
                ConnectorStatus connectorStatus = EffectListener.GetConnectionStatus();
                if (connectorStatus == ConnectorStatus.Uninitialized)
                    statusCode = "Network.Uninitialized";
                if (connectorStatus == ConnectorStatus.Connected)
                    statusCode = "Network.Connected";
                if (connectorStatus == ConnectorStatus.Disconnected)
                    statusCode = "Network.Disconnected";
                if (connectorStatus == ConnectorStatus.Failure)
                    statusCode = "Settings.Status.Failed";
            }
            return statusCode;
        }

        public void AddTimedEffect(TimedEffect effect) {
            TimedEffects.Add(effect);
        }

        public void RemoveTimedEffect(TimedEffect effect) {
            TimedEffects.Remove(effect);
        }

        private void OnEffectRecieved(object sender, EffectCommand effectCommand) {
            CommandQueue.Enqueue(effectCommand);
        }

        private void ProcessEffectQueue() {
            if (CommandQueue.Count > 0) {
                EffectCommand effectCommand = CommandQueue.Dequeue();


                var code = effectCommand.code;
                if(code.Contains("_")){
                    code = code.Split('_')[0];
                }


                if (EffectList.ContainsKey(code)) {
                    ModService.Instance.LastEffectStatusMessage = string.Empty;
                    EffectStatus result = EffectList[code].Execute(effectCommand);
                    EffectListener.ReportEffectStatus(effectCommand, result, ModService.Instance.LastEffectStatusMessage);
                }
                else {
                    ModService.Instance.Alert($"Effect '{effectCommand.code}' not found!");
                    EffectListener.ReportEffectStatus(effectCommand, EffectStatus.Failure, $"Effect '{effectCommand.code}' not found.");
                }
            }
        }

        public static void PauseEffectQueue()
        {
            if (CommandQueue.Count > 0)
            {
                EffectCommand effectCommand = CommandQueue.Dequeue();

                var code = effectCommand.code;
                if (code.Contains("_"))
                {
                    code = code.Split('_')[0];
                }

                if (EffectList.ContainsKey(code))
                {
                    EffectStatus result = EffectStatus.Retry;
                    EffectListener.ReportEffectStatus(effectCommand, result, string.Empty);
                }
                else
                {
                    ModService.Instance.Alert($"Effect '{effectCommand.code}' not found!");
                    EffectListener.ReportEffectStatus(effectCommand, EffectStatus.Failure, $"Effect '{effectCommand.code}' not found.");
                }
            }
        }

        private void HandleTimedEffects() {
            if (_counter >= EFFECT_THROTTLE) {
                TimedEffects.ForEach(effect => effect.Tick());
                TimedEffects.RemoveAll(effect => effect.IsActive == false);
                _counter = 0;
            }
            _counter++;
        }
    }
}
