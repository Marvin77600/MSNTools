using AllocsFixes.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.ConsoleCommands
{
    public class VehiclesDataLivemap : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params[0] == "active")
                {
                    var vehiclesActive = Harmony.H_VehicleManager.vehiclesActive;
                    JSONObject jSONObject = new JSONObject();
                    for (int i = 0; i < vehiclesActive.Count; i++)
                    {
                        var vehicle = vehiclesActive[i];
                        jSONObject.Add("position", new JSONString(vehicle.position.ToCultureInvariantString()));
                        jSONObject.Add("owner", new JSONString(vehicle.GetOwner().PlatformIdentifier.ToString()));
                        jSONObject.Add("name", new JSONString(vehicle.EntityName));
                    }
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{jSONObject}");
                }
                else if (_params[0] == "unloaded")
                {
                    var vehiclesUnloaded = Harmony.H_VehicleManager.vehiclesUnloaded;
                    JSONObject jSONObject = new JSONObject();
                    for (int i = 0; i < vehiclesUnloaded.Count; i++)
                    {
                        var vehicle = vehiclesUnloaded[i];
                        jSONObject.Add("position", new JSONObject());
                        jSONObject.Add("name", new JSONString(vehicle.entityName));
                    }
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{jSONObject}");
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "vehiclesdata" };

        public override string GetDescription() => "Get vehicles position.";
    }
}