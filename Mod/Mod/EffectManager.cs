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
        private const int EFFECT_THROTTLE = 300;

        public EffectManager(Game game) {
            Game = game;
            EffectList = ModService.Instance.EffectList;
            ModService.Instance.EffectManager = this;
            ModService.Instance.Game = game;
        }

        public override void LoadedGame() {
            EffectListener = new EffectListener(hostname: ModService.Instance.Hostname, port: ModService.Instance.Port);
            EffectListener.OnEffect += OnEffectRecieved;
            EffectListener.StartBackgroundListener();
        }

        public override void GameComponentTick() {
            ProcessEffectQueue();
            HandleTimedEffects();
            EffectListener.live = 10;
        }

        public override void GameComponentUpdate()
        {
            if (Verse.Find.TickManager.Paused)
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
                if (EffectList.ContainsKey(effectCommand.code)) {
                    EffectStatus result = EffectList[effectCommand.code].Execute(effectCommand);
                    EffectListener.ReportEffectStatus(effectCommand, result);
                }
                else {
                    ModService.Instance.Logger.Trace($"Effect of type '{effectCommand.type}' not found!");
                    EffectListener.ReportEffectStatus(effectCommand, EffectStatus.Failure);
                }
            }
        }

        public static void PauseEffectQueue()
        {
            if (CommandQueue.Count > 0)
            {
                EffectCommand effectCommand = CommandQueue.Dequeue();
                if (EffectList.ContainsKey(effectCommand.code))
                {
                    EffectStatus result = EffectStatus.Retry;
                    EffectListener.ReportEffectStatus(effectCommand, result);
                }
                else
                {
                    ModService.Instance.Logger.Trace($"Effect of type '{effectCommand.type}' not found!");
                    EffectListener.ReportEffectStatus(effectCommand, EffectStatus.Failure);
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
