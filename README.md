# PalworldManager

Palworld currently does not stop simulations when no players are connected. This application will automatically shut down the server when no players are connected, and restart it when a player connects.

## Configuration

Modify the config.json file to your liking. The default configuration is as follows:
```json
{
  "ServerIP": "localhost", // server's IP address. This should usually be localhost unless support is added later for remote servers.
  "ServerPort": 8211, // server's port. This should match the port specified in the server's command line arguments.
  "ServerPassword": "adminpasswordhere", // server's password. This should match the password specified in the server's command line arguments or Palworld config file. REST support must be enabled!
  "ServerRESTPort": 8212, // server's REST port. This should match the port specified in the server's config file
  "ServerPath": "/path/to/server/executable", // path to the server executable. This should be the full path to the PalworldServer.exe file.
  "ServerParams": "-port=8211 -maxplayers=16 -NumberOfWorkerThreadsServer=4 -logformat=text", // additional parameters to pass to the server executable
  "Debugging": false, // enable or disable debugging mode
  "LogToFile": true, // enable or disable logging to a file
  "WaitForPlayerTimeout": 300 // time in seconds to wait for a player before shutting down the server
}
```

## Notes

- The Palworld client has a long connection timeout so the server should start up in time. On large servers, it may take too long and the client will time out. This is unfortunately a limitation of the Palworld client, although a client can re-connect after the server has started.
- The server will not shut down if there are any players connected, even if they are AFK. The server will only shut down when there are no players connected.
- As of this writing, the manager has only been tested on Linux. It does not support Dockerized servers due to the port binding mechanism.
- This program is not responsible for any issues that may arise from using it. Use at your own risk.