using System.Collections.Generic;
using System;

namespace SPTools
{
    public class PlaceBlocks : ConsoleCmdAbstract
    {
        public override string GetHelp() => "Permer de placer un bloc à une coordonnée précise.";

        public override string GetDescription() => "Permer de placer un bloc à une coordonnée précise.\nUtilisation :\n    Exemple : placeblock 1000 50 1000 concreteBlock";

        public override string[] GetCommands() => new string[] { "placeblock", "pb" };

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                WorldBase world = GameManager.Instance.World;
                if (_params.Count == 4)
                {
                    int x;
                    int y;
                    int z;
                    int.TryParse(_params[0], out x);
                    int.TryParse(_params[1], out y);
                    int.TryParse(_params[2], out z);
                    Vector3i pos = new Vector3i(x, y, z);
                    BlockValue blockName = Block.GetBlockValue(_params[3], false);
                    world.SetBlockRPC(0, pos, blockName);
                    return;
                }
                else
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output(GetDescription());
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }
    }
}