global using static PalworldManager.Logging;
using System.Diagnostics;
using System.Net.Sockets;

namespace PalworldManager
{
    public class Program
    {

        private static PalworldAPI? _api;
        public static Config? Config { get; private set; }

        public static async Task Main(string[] args)
        {
            Config = new Config();
            Config.Load();
            _api = new PalworldAPI(Config.ServerIP, Config.ServerRESTPort, Config.ServerPassword);
            await ListenerThread();
        }

        private static async Task ListenerThread()
        {
            var listener = new Listener(_api!, Config!);

            while (true)
            {
                await listener.Start();
                while (listener.IsListening)
                {
                    await Task.Delay(1000);
                }
                // listener stopped, assume connection request received
                await AssureServerStarted();

                while (true)
                {
                    var players = await _api!.GetPlayers();
                    if (players != null)
                    {
                        if (players.Count == 0)
                        {
                            Info("[Server] No players online, shutting down server...");
                            await _api!.ShutdownServer();
                            while (await _api!.IsRunning())
                            {
                                Info("[Server] Waiting for server to shut down...");
                                await Task.Delay(5000);
                            }
                            Info("[Server] Server has shut down.");
                            break;
                        }
                        else
                        {
                            Info($"[Server] Players online: {players.Count}");
                        }
                    }
                    else
                    {
                        Info("[Server] Failed to get players, assuming server is down.");
                        break;
                    }
                    await Task.Delay(Config!.WaitForPlayerTimeout * 1000); // check every WaitForPlayerTimeout seconds
                }
                Info("[Server] Server is down, restarting listener...");
            }

        }

        private static async Task AssureServerStarted()
        {
            if (!await _api!.IsRunning())
            {
                Info("[Server] Server is not running, starting...");
                StartServer();
                Info("[Server] Command sent to start server, waiting for it to be ready...");
                // wait for server to be ready
                while (!await _api!.IsRunning())
                {
                    Info("[Server] Server is not ready yet, waiting 5 seconds...");
                    await Task.Delay(5000);
                }
                Info("[Server] Server is ready.");
            }
        }

        private static void StartServer()
        {
            // execute shell command to start the server
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Config!.ServerPath,
                Arguments = Config!.ServerParams,
                UseShellExecute = Environment.OSVersion.Platform == PlatformID.Win32NT,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = false
            };
            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
            }
        }
    }

    internal class Listener
    {
        private readonly PalworldAPI _api;
        private readonly Config _config;

        public bool IsListening { get; private set; } = false;

        public Listener(PalworldAPI api, Config config)
        {
            _api = api;
            _config = config;
        }

        public async Task Start()
        {
            // Start the listener thread
            await StartListening();
        }

        private async Task StartListening()
        {
            /// check if port is available, if not, log error and return
            try
            {
                var listener = new UdpClient(int.Parse(_config.ServerPort));
                IsListening = true;
                Info($"[Listener] Listening on port {_config.ServerPort}...");

                while (IsListening)
                {
                    var result = await listener.ReceiveAsync();
                    byte[] data = result.Buffer;
                    string hex = Convert.ToHexString(data);
                    Debug($"[Listener] UDP packet from {result.RemoteEndPoint} | {hex}");
                    if (data.Length > 0 && data[0] == 0x09)
                    {
                        Info($"[Listener] Received Connection Request from {result.RemoteEndPoint}");
                        IsListening = false;
                        listener.Close();
                    }
                    else
                    {
                        Debug($"[Listener] Received non-connection request packet from {result.RemoteEndPoint}");
                    }
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                Error($"[Listener] Port {_config.ServerPort} is already in use.");
                return;
            }
            catch (SocketException ex)
            {
                Error($"[Listener] Error starting listener on port {_config.ServerPort}: {ex.Message}");
                return;
            }
        }
    }
}