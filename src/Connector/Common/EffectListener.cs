using System;
using System.ComponentModel;
using RimWorld;
using Verse;


namespace CrowdControl {
    public class EffectListener {
        public event EffectCommandHandler OnEffect;

        private BackgroundWorker Worker;
        private BackgroundWorker WorkerB;
        private TcpConnector Connector;
        private const string ResponseText = "{{\"id\":{0},\"status\":{1},\"message\":\"{2}\",\"timeRemaining\":0,\"type\":0}}";
        private string Hostname;
        private uint Port;

        private uint attempts = 0;
        public static uint live = 30;
        public static bool connected = false;


        public EffectListener(string hostname, uint port) {
            Hostname = hostname;
            Port = port;
            Connector = new TcpConnector(Hostname, Port);
        }

        public void StartBackgroundListener() {
            if (Worker != null) {
                Worker.CancelAsync();
            }
            if (WorkerB != null) {
                WorkerB.CancelAsync();
            }

            attempts = 0;
            connected = false;
            live = 30;
            Connector = new TcpConnector(Hostname, Port);

            Worker = new BackgroundWorker();
            Worker.DoWork += OnWorkerExecute;
            Worker.WorkerSupportsCancellation = true;
            Worker.RunWorkerAsync();

            WorkerB = new BackgroundWorker();
            WorkerB.DoWork += OnWorkerExecuteB;
            WorkerB.WorkerSupportsCancellation = true;
            WorkerB.RunWorkerAsync();
        }

        public void ReportEffectStatus(EffectCommand message, EffectStatus status, string statusMessage = "") {
            string response = string.Format(ResponseText, message.id, (int)status, EscapeJson(statusMessage));
            Connector.Send(response);
        }

        private static string EscapeJson(string value) {
            if (string.IsNullOrEmpty(value)) {
                return string.Empty;
            }

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }

        private void OnWorkerExecute(object sender, DoWorkEventArgs e) {
            while (Worker.CancellationPending == false && attempts <= 12)
            {
                try
                {
                    ConnectorStatus connectorStatus = Connector.Status;
                    switch (connectorStatus)
                    {
                        case ConnectorStatus.Uninitialized:
                            HandleState_Disconnected();
                            connected = false;
                            break;
                        case ConnectorStatus.Connected:
                            HandleState_Connected();
                            connected = true;
                            break;
                        case ConnectorStatus.Disconnected:
                            ModService.Instance.Logger.Trace("Disconnected, retrying connection.");
                            HandleState_Disconnected();
                            connected = false;
                            break;
                        case ConnectorStatus.Failure:
                            ModService.Instance.Logger.Trace("Connection failure, rebuilding connector.");
                            HandleState_Failure();
                            connected = false;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    //ModService.Instance.Alert(ex.ToString());
                }
            }
            if (attempts > 12) e.Cancel = true;
            
        }

        private void OnWorkerExecuteB(object sender, DoWorkEventArgs e)
        {
            while (WorkerB.CancellationPending == false && attempts<=12)
            {
                try
                {
                    //ModService.Instance.Alert($"1 {attempts} Live: {live}");

                    if (attempts == 0 && connected)
                    {
                        if (live > 0) live--;
                        else
                        {
                            EffectManager.PauseEffectQueue();
                        }
                    }
                           
                }
                catch (Exception)
                {
                    //ModService.Instance.Alert(ex.ToString());
                }
                System.Threading.Thread.Sleep(5);
            }
            if (attempts > 12) e.Cancel = true;

        }

        private EffectCommand ParseMessage(string message) {
            EffectCommand effectCommand = null;
            try {
                effectCommand = new EffectCommand(message);
                effectCommand.TryParse();
            }
            catch (Exception) {
                //ModService.Instance.Alert($"Unable to parse command: {message} - {ex}");
            }
            return effectCommand;
        }

        private void HandleState_Connected() {
            attempts = 0;
            string message = Connector.Recieve();
            EffectCommand effectCommand = ParseMessage(message);
            BroadcastEffect(effectCommand);
        }

        private void HandleState_Disconnected() {
            attempts++;
            ModService.Instance.Logger.Trace($"Connection attempt {attempts}...");

            if (attempts > 12)
            {
                Worker.CancelAsync();
                WorkerB.CancelAsync();
                ModService.Instance.Logger.Trace("Stopping Crowd Control connection attempts.");
                return;
            }
            Connector.Connect();

            System.Threading.Thread.Sleep(10000);
        }

        private void HandleState_Failure() {
            System.Threading.Thread.Sleep(5000);
            Connector = new TcpConnector(Hostname, Port);
        }

        private void BroadcastEffect(EffectCommand effectCommand) {
            if (effectCommand.IsValid)
                OnEffect.Invoke(this, effectCommand);
            else {
                //ModService.Instance.Alert($"Invalid effect command: {effectCommand}");
            }
        }

        public ConnectorStatus GetConnectionStatus() {
            if (Connector == null)
                return ConnectorStatus.Uninitialized;
            else
                return Connector.Status;
        }
    }
}