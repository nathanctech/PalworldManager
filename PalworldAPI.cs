using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace PalworldManager
{
    internal class PalworldAPI
    {
        private readonly string _ip;
        private readonly string _port;
        private readonly string _password;

        public PalworldAPI(string ip, string port, string password)
        {
            _ip = ip;
            _port = port;
            _password = password;
        }

        public async Task<string?> SendCommand(string endpoint, HttpMethod method, object command)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri($"http://{_ip}:{_port}/");
            var byteArray = Encoding.ASCII.GetBytes($"admin:{_password}");
            endpoint = "v1/api/" + endpoint;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var content = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            try
            {
                if (method == HttpMethod.Post)
                {
                    var response = await client.PostAsync(endpoint, content);
                    return await HandleResponse(response, endpoint);
                }
                else if (method == HttpMethod.Get)
                {
                    var response = await client.GetAsync(endpoint);
                    return await HandleResponse(response, endpoint);
                }
                else
                {
                    throw new NotSupportedException($"HTTP method {method} is not supported.");
                }
            } catch (Exception ex)
            {
                // probably a connection refused, return null
                Debug("[PalworldAPI] Error sending command to " + endpoint + ": " + ex.Message);
                return null;
            }
        }

        private async Task<string?> HandleResponse(HttpResponseMessage response, string endpoint)
        {   
            if (!response.IsSuccessStatusCode)
            {
                Debug($"[PalworldAPI] Error sending command to {endpoint}: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<Player>> GetPlayers()
        {
            string? response = await SendCommand("players", HttpMethod.Get, new { });
            if (response == null)
            {
                return [];
            }
            var playersResponse = JsonSerializer.Deserialize<PlayersResponse>(response);
            if (playersResponse == null)
            {
                Error("[PalworldAPI] Failed to deserialize players response.");
                return [];
            }
            return playersResponse.players;
        }

        public async Task<bool> ShutdownServer()
        {
            string? response = await SendCommand("shutdown", HttpMethod.Post, new { waittime = 5, message = "Server is shutting down in 5 seconds." });
            if (response == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsRunning()
        {
            string? response = await SendCommand("info", HttpMethod.Get, new { });
            if (response == null)
            {
                return false;
            }
            return true;
        }

        public record PlayersResponse(
            List<Player> players
        );

        public record Player(
            string name,
            string accountName,
            string playerId,
            string userId,
            string ip,
            double ping,
            double location_x,
            double location_y,
            int level,
            int building_count
        );
    }
}