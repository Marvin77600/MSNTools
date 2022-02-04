using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.ConsoleCommands
{
    public class CommandResetRegions : ConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                List<string> regions = PersistentContainer.Instance.RegionsReset;
                if (_params.Count > 3)
                {
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Wrong number of arguments, expected 0 to 3, found '{_params.Count}'");
                    return;
                }
                if (_params.Count == 0)
                {
                    if (_senderInfo.RemoteClientInfo == null)
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid user data. Unable to retrieve your position in game");
                        return;
                    }
                    EntityPlayer player = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
                    if (player != null)
                    {
                        double regionX, regionZ;
                        if (player.position.x < 0)
                        {
                            regionX = Math.Truncate(player.position.x / 512) - 1;
                        }
                        else
                        {
                            regionX = Math.Truncate(player.position.x / 512);
                        }
                        if (player.position.z < 0)
                        {
                            regionZ = Math.Truncate(player.position.z / 512) - 1;
                        }
                        else
                        {
                            regionZ = Math.Truncate(player.position.z / 512);
                        }
                        string region = regionX + "." + regionZ;
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"You are standing in region: {region}");
                        return;
                    }
                    else
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid user data. Unable to retrieve your position in game");
                        return;
                    }
                }
                else if (_params[0].ToLower() == "add")
                {
                    if (_params.Count == 1)
                    {
                        if (_senderInfo.RemoteClientInfo == null)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid user data. Unable to retrieve your position in game");
                            return;
                        }
                        EntityPlayer player = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
                        if (player != null)
                        {
                            double regionX, regionZ;
                            if (player.position.x < 0)
                            {
                                regionX = Math.Truncate(player.position.x / 512) - 1;
                            }
                            else
                            {
                                regionX = Math.Truncate(player.position.x / 512);
                            }
                            if (player.position.z < 0)
                            {
                                regionZ = Math.Truncate(player.position.z / 512) - 1;
                            }
                            else
                            {
                                regionZ = Math.Truncate(player.position.z / 512);
                            }
                            string region = regionX + "." + regionZ;
                            if (!regions.Contains(region))
                            {
                                regions.Add(region);
                                PersistentContainer.DataChange = true;
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{region}' has been added to the reset list.");
                                return;
                            }
                            else
                            {
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{region}' is already on the reset list");
                                return;
                            }
                        }
                        else
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid user data. Unable to retrieve your position in game");
                            return;
                        }
                    }
                    else if (_params.Count == 2)
                    {
                        if (_params[1].Contains("."))
                        {
                            string[] region = _params[1].Split('.');
                            if (int.TryParse(region[0], out int regionX))
                            {
                                if (int.TryParse(region[1], out int regionZ))
                                {
                                    if (!regions.Contains(_params[1]))
                                    {
                                        regions.Add(_params[1]);
                                        PersistentContainer.DataChange = true;
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{_params[1]}' has been added to the reset list.");
                                        return;
                                    }
                                    else
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{_params[1]}' is already on the reset list");
                                        return;
                                    }
                                }
                                else
                                {
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Invalid region format '{_params[1]}'");
                                    return;
                                }
                            }
                            else
                            {
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Invalid region format '{_params[1]}'");
                                return;
                            }
                        }
                        else
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Invalid region format '{_params[1]}'");
                            return;
                        }
                    }
                    else
                    {
                        if (float.TryParse(_params[1], out float regionX))
                        {
                            if (float.TryParse(_params[2], out float regionZ))
                            {
                                double x, z;
                                if (regionX < 0)
                                {
                                    x = Math.Truncate(regionX / 512) - 1;
                                }
                                else
                                {
                                    x = Math.Truncate(regionX / 512);
                                }
                                if (regionZ < 0)
                                {
                                    z = Math.Truncate(regionZ / 512) - 1;
                                }
                                else
                                {
                                    z = Math.Truncate(regionZ / 512);
                                }
                                string region = x + "." + z;
                                if (!regions.Contains(region))
                                {
                                    regions.Add(region);
                                    PersistentContainer.DataChange = true;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{region}' has been added to the reset list.");
                                    return;
                                }
                                else
                                {
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Region '{0}' is already on the reset list", region);
                                    return;
                                }
                            }
                            else
                            {
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Invalid region format '{_params[1]}' '{_params[2]}'");
                                return;
                            }
                        }
                        else
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Invalid region format '{_params[1]}' '{_params[2]}'");
                            return;
                        }
                    }
                }
                else if (_params[0].ToLower() == "remove" || _params[0].ToLower() == "del")
                {
                    if (_params.Count == 1)
                    {
                        if (_senderInfo.RemoteClientInfo == null)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid user data. Unable to retrieve your position in game");
                            return;
                        }
                        EntityPlayer player = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
                        if (player != null)
                        {
                            double regionX, regionZ;
                            if (player.position.x < 0)
                            {
                                regionX = Math.Truncate(player.position.x / 512) - 1;
                            }
                            else
                            {
                                regionX = Math.Truncate(player.position.x / 512);
                            }
                            if (player.position.z < 0)
                            {
                                regionZ = Math.Truncate(player.position.z / 512) - 1;
                            }
                            else
                            {
                                regionZ = Math.Truncate(player.position.z / 512);
                            }
                            string region = regionX + "." + regionZ;
                            if (regions.Contains(region))
                            {
                                regions.Remove(region);
                                PersistentContainer.DataChange = true;
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{region}' has been removed to the reset list.");
                                return;
                            }
                            else
                            {
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{region}' is not on the reset list");
                                return;
                            }
                        }
                        else
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid user data. Unable to retrieve your position in game");
                            return;
                        }
                    }
                    else if (_params.Count == 2)
                    {
                        if (_params[1].Contains("."))
                        {
                            string[] region = _params[1].Split('.');
                            if (int.TryParse(region[0], out int regionX))
                            {
                                if (int.TryParse(region[1], out int regionZ))
                                {
                                    if (regions.Contains(_params[1]))
                                    {
                                        regions.Remove(_params[1]);
                                        PersistentContainer.DataChange = true;
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{_params[1]}' has been removed to the reset list.");
                                        return;
                                    }
                                    else
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{_params[1]}' is not on the reset list");
                                        return;
                                    }
                                }
                                else
                                {
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Invalid region format '{_params[1]}'");
                                    return;
                                }
                            }
                            else
                            {
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Invalid region format '{_params[1]}'");
                                return;
                            }
                        }
                        else
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Invalid region format '{_params[1]}'");
                            return;
                        }
                    }
                    else
                    {
                        if (float.TryParse(_params[1], out float regionX))
                        {
                            if (float.TryParse(_params[2], out float regionZ))
                            {
                                double x, z;
                                if (regionX < 0)
                                {
                                    x = Math.Truncate(regionX / 512) - 1;
                                }
                                else
                                {
                                    x = Math.Truncate(regionX / 512);
                                }
                                if (regionZ < 0)
                                {
                                    z = Math.Truncate(regionZ / 512) - 1;
                                }
                                else
                                {
                                    z = Math.Truncate(regionZ / 512);
                                }
                                string region = x + "." + z;
                                if (regions.Contains(region))
                                {
                                    regions.Remove(region);
                                    PersistentContainer.DataChange = true;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{region}' has been removed to the reset list.");
                                    return;
                                }
                                else
                                {
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Region '{0}' is not on the reset list", region);
                                    return;
                                }
                            }
                            else
                            {
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Invalid region format '{_params[1]}' '{_params[2]}'");
                                return;
                            }
                        }
                        else
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Invalid region format '{_params[1]}' '{_params[2]}'");
                            return;
                        }
                    }
                }
                else if (_params[0].ToLower() == "cancel")
                {
                    if (regions.Count > 0)
                    {
                        regions.Clear();
                        PersistentContainer.DataChange = true;
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Region reset list has been cancelled and cleared");
                        return;
                    }
                    else
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("There are no regions on the reset list");
                        return;
                    }
                }
                else if (_params[0].ToLower() == "list")
                {
                    if (regions.Count > 0)
                    {
                        for (int i = 0; i < regions.Count; i++)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Region '{regions[i]}'");
                        }
                    }
                    else
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("There are no regions on the reset list");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in RegionResetConsole.Execute: {e.Message}");
            }
        }

        public override string[] GetCommands() => new string[] { "rr", "resetregions" };

        public override string GetDescription() => "...";
    }
}