using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSC.Winlator.Services
{
    public class HttpServerService
    {
        private HttpListener? _listener;
        private readonly int _port;

        public HttpServerService(int port = 5000)
        {
            _port = port;
        }

        public async Task Start()
        {
            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://localhost:{_port}/");
                _listener.Start();
                
                LoggerService.LogSuccess($"HTTP Server started on port {_port}");
                
                while (_listener.IsListening)
                {
                    HttpListenerContext context = await _listener.GetContextAsync();
                    _ = HandleRequest(context);
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"HTTP Server error: {ex.Message}", ex);
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url?.AbsolutePath ?? "/";
                string method = context.Request.HttpMethod;

                LoggerService.LogInfo($"{method} {path}");

                switch (path)
                {
                    case "/mods":
                        if (method == "GET")
                            await HandleListMods(context);
                        break;
                    case "/install":
                        if (method == "POST")
                            await HandleInstallMod(context);
                        break;
                    case "/remove":
                        if (method == "POST")
                            await HandleRemoveMod(context);
                        break;
                    case "/profiles":
                        if (method == "GET")
                            await HandleListProfiles(context);
                        break;
                    case "/launch":
                        if (method == "POST")
                            await HandleLaunchGame(context);
                        break;
                    case "/health":
                        await SendJsonResponse(context, 200, new { status = "ok" });
                        break;
                    default:
                        await SendJsonResponse(context, 404, new { error = "Not found" });
                        break;
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Request handler error: {ex.Message}", ex);
                try
                {
                    await SendJsonResponse(context, 500, new { error = ex.Message });
                }
                catch { }
            }
        }

        private async Task HandleListMods(HttpListenerContext context)
        {
            try
            {
                var modDirs = Directory.GetDirectories(AppBootstrap.ModsFolder);
                var mods = new System.Collections.Generic.List<object>();

                foreach (var dir in modDirs)
                {
                    var dirInfo = new DirectoryInfo(dir);
                    mods.Add(new { name = dirInfo.Name });
                }

                await SendJsonResponse(context, 200, new { mods, count = mods.Count });
            }
            catch (Exception ex)
            {
                await SendJsonResponse(context, 500, new { error = ex.Message });
            }
        }

        private async Task HandleInstallMod(HttpListenerContext context)
        {
            try
            {
                string? modPath = context.Request.QueryString["path"];
                if (string.IsNullOrEmpty(modPath))
                {
                    await SendJsonResponse(context, 400, new { error = "Missing 'path' parameter" });
                    return;
                }

                if (!File.Exists(modPath))
                {
                    await SendJsonResponse(context, 400, new { error = "File not found" });
                    return;
                }

                bool success = await AppBootstrap.ModInstaller!.InstallMod(modPath, AppBootstrap.ModsFolder);
                
                if (success)
                {
                    await SendJsonResponse(context, 200, new { status = "installed" });
                }
                else
                {
                    await SendJsonResponse(context, 400, new { error = "Installation failed" });
                }
            }
            catch (Exception ex)
            {
                await SendJsonResponse(context, 500, new { error = ex.Message });
            }
        }

        private async Task HandleRemoveMod(HttpListenerContext context)
        {
            try
            {
                string? modName = context.Request.QueryString["name"];
                if (string.IsNullOrEmpty(modName))
                {
                    await SendJsonResponse(context, 400, new { error = "Missing 'name' parameter" });
                    return;
                }

                string modPath = Path.Combine(AppBootstrap.ModsFolder, modName);
                if (!Directory.Exists(modPath))
                {
                    await SendJsonResponse(context, 400, new { error = "Mod not found" });
                    return;
                }

                Directory.Delete(modPath, true);
                await SendJsonResponse(context, 200, new { status = "removed" });
            }
            catch (Exception ex)
            {
                await SendJsonResponse(context, 500, new { error = ex.Message });
            }
        }

        private async Task HandleListProfiles(HttpListenerContext context)
        {
            try
            {
                var profileDirs = Directory.GetDirectories(AppBootstrap.ProfilesFolder);
                var profiles = new System.Collections.Generic.List<object>();

                foreach (var dir in profileDirs)
                {
                    var dirInfo = new DirectoryInfo(dir);
                    profiles.Add(new { name = dirInfo.Name });
                }

                await SendJsonResponse(context, 200, new { profiles, count = profiles.Count });
            }
            catch (Exception ex)
            {
                await SendJsonResponse(context, 500, new { error = ex.Message });
            }
        }

        private async Task HandleLaunchGame(HttpListenerContext context)
        {
            try
            {
                await SendJsonResponse(context, 200, new { status = "launch coming soon" });
            }
            catch (Exception ex)
            {
                await SendJsonResponse(context, 500, new { error = ex.Message });
            }
        }

        private async Task SendJsonResponse(HttpListenerContext context, int statusCode, object data)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            string json = JsonSerializer.Serialize(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();

            await Task.CompletedTask;
        }

        public void Stop()
        {
            _listener?.Stop();
            _listener?.Close();
        }
    }
}
