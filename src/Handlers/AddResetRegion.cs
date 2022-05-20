using AllocsFixes.JSON;
using AllocsFixes.NetConnections.Servers.Web;
using AllocsFixes.NetConnections.Servers.Web.API;
using AllocsFixes.PersistentData;
using MSNPersistentContainer = MSNTools.PersistentData.PersistentContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools
{
    public class AddResetRegion : WebAPI
    {
        public override void HandleRequest(HttpListenerRequest _req, HttpListenerResponse _resp, WebConnection _user, int _permissionLevel)
        {
            if (_req.QueryString["region"] == null)
            {
                _resp.StatusCode = 400;
                Web.SetResponseTextContent(_resp, "No region given");
            }
            else
            {
                string str = _req.QueryString["region"];
                IConsoleCommand command = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand($"rr add {str}");
                if (command == null)
                {
                    _resp.StatusCode = 404;
                    Web.SetResponseTextContent(_resp, "Unknown command");
                }
                else
                {
                    int commandPermissionLevel = GameManager.Instance.adminTools.GetCommandPermissionLevel(command.GetCommands());
                    if (_permissionLevel > commandPermissionLevel)
                    {
                        _resp.StatusCode = 403;
                        Web.SetResponseTextContent(_resp, "You are not allowed to execute this command");
                    }
                    else
                    {
                        _resp.SendChunked = true;
                        WebCommandResult _sender = new WebCommandResult("resetregions", str, WebCommandResult.ResultType.Raw, _resp);
                        SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteAsync("rr add " + str, _sender);
                    }
                }
            }
        }

        public AddResetRegion() : base()
        {
        }
    }
}
