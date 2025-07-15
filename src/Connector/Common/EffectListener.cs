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
        private const string ResponseText = "{{\"id\":{0},\"status\":{1},\"message\":\"\",\"timeRemaining\":0,\"type\":0}}";
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
            Worker = new BackgroundWorker();
            Worker.DoWork += OnWorkerExecute;
            Worker.RunWorkerCompleted += OnWorkerFinished;
            Worker.WorkerSupportsCancellation = true;
            Worker.RunWorkerAsync();

            WorkerB = new BackgroundWorker();
            WorkerB.DoWork += OnWorkerExecuteB;
            WorkerB.WorkerSupportsCancellation = true;
            WorkerB.RunWorkerAsync();
        }

        public void ReportEffectStatus(EffectCommand message, EffectStatus status) {
            string response = string.Format(ResponseText, message.id, (int)status);
            Connector.Send(response);
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
                            ModService.Instance.Alert("Notification.Disconnected");
                            HandleState_Disconnected();
                            connected = false;
                            break;
                        case ConnectorStatus.Failure:
                            ModService.Instance.Alert("Notification.Failure");
                            HandleState_Failure();
                            connected = false;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
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
                catch (Exception ex)
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
            catch (Exception ex) {
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
            ModService.Instance.Alert($"Attempt {attempts}...");

            if (attempts > 12)
            {
                Worker.CancelAsync();
                WorkerB.CancelAsync();
                ModService.Instance.Alert($"Stopping Crowd Control");
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

        private void OnWorkerFinished(object sender, RunWorkerCompletedEventArgs e) {
            System.Threading.Thread.Sleep(7000);
            StartBackgroundListener();
        }

        public ConnectorStatus GetConnectionStatus() {
            if (Connector == null)
                return ConnectorStatus.Uninitialized;
            else
                return Connector.Status;
        }
    }
}