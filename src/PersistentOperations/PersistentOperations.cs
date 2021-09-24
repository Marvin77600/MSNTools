using System.Collections.Generic;
using System.Linq;


namespace MSNTools
{
    class PersistentOperations
    {

        public static List<ClientInfo> ClientList()
        {
            List<ClientInfo> _clientList = ConnectionManager.Instance.Clients.List.ToList();
            if (_clientList != null)
            {
                return _clientList;
            }
            return null;
        }

        public static ClientInfo GetClientInfoFromSteamId(string _playerId)
        {
            ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForPlayerId(_playerId);
            if (_cInfo != null)
            {
                return _cInfo;
            }
            return null;
        }

        public static ClientInfo GetClientInfoFromEntityId(int _playerId)
        {
            ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForEntityId(_playerId);
            if (_cInfo != null)
            {
                return _cInfo;
            }
            return null;
        }

        public static List<EntityPlayer> PlayerList()
        {
            if (GameManager.Instance.World.Players.list != null && GameManager.Instance.World.Players.list.Count > 0)
            {
                return GameManager.Instance.World.Players.list;
            }
            return null;
        }

        public static EntityPlayer GetEntityPlayer(string _playerId)
        {
            PersistentPlayerData _persistentPlayerData = GetPersistentPlayerDataFromSteamId(_playerId);
            if (_persistentPlayerData != null)
            {
                if (GameManager.Instance.World.Players.dict.ContainsKey(_persistentPlayerData.EntityId))
                {
                    EntityPlayer _entityPlayer = GameManager.Instance.World.Players.dict[_persistentPlayerData.EntityId];
                    if (_entityPlayer != null)
                    {
                        return _entityPlayer;
                    }
                }
            }
            return null;
        }

        public static EntityAlive GetPlayerAlive(string _playerId)
        {
            PersistentPlayerData _persistentPlayerData = GetPersistentPlayerDataFromSteamId(_playerId);
            if (_persistentPlayerData != null)
            {
                if (GameManager.Instance.World.Players.dict.ContainsKey(_persistentPlayerData.EntityId))
                {
                    EntityAlive _entityAlive = (EntityAlive)GetEntity(_persistentPlayerData.EntityId);
                    if (_entityAlive != null)
                    {
                        return _entityAlive;
                    }
                }
            }
            return null;
        }

        public static Entity GetEntity(int _id)
        {
            Entity _entity = GameManager.Instance.World.GetEntity(_id);
            if (_entity != null)
            {
                return _entity;
            }
            return null;
        }

        public static PersistentPlayerList GetPersistentPlayerList()
        {
            PersistentPlayerList _persistentPlayerList = GameManager.Instance.persistentPlayers;
            if (_persistentPlayerList != null)
            {
                return _persistentPlayerList;
            }
            return null;
        }

        public static PersistentPlayerData GetPersistentPlayerDataFromSteamId(string _playerId)
        {
            PersistentPlayerList _persistentPlayerList = GameManager.Instance.persistentPlayers;
            if (_persistentPlayerList != null)
            {
                PersistentPlayerData _persistentPlayerData = _persistentPlayerList.GetPlayerData(_playerId);
                if (_persistentPlayerData != null)
                {
                    return _persistentPlayerData;
                }
            }
            return null;
        }

        public static PersistentPlayerData GetPersistentPlayerDataFromEntityId(int _entityId)
        {
            PersistentPlayerList _persistentPlayerList = GameManager.Instance.persistentPlayers;
            if (_persistentPlayerList != null)
            {
                PersistentPlayerData _persistentPlayerData = _persistentPlayerList.GetPlayerDataFromEntityID(_entityId);
                if (_persistentPlayerData != null)
                {
                    return _persistentPlayerData;
                }
            }
            return null;
        }

        public static PlayerDataFile GetPlayerDataFileFromSteamId(string _playerId)
        {
            PlayerDataFile _playerDatafile = new PlayerDataFile();
            _playerDatafile.Load(GameUtils.GetPlayerDataDir(), _playerId.Trim());
            if (_playerDatafile != null)
            {
                return _playerDatafile;
            }
            return null;
        }

        public static PlayerDataFile GetPlayerDataFileFromEntityId(int _entityId)
        {
            PersistentPlayerData _persistentPlayerData = GetPersistentPlayerDataFromEntityId(_entityId);
            if (_persistentPlayerData != null)
            {
                PlayerDataFile _playerDatafile = new PlayerDataFile();
                _playerDatafile.Load(GameUtils.GetPlayerDataDir(), _persistentPlayerData.PlayerId.Trim());
                if (_playerDatafile != null)
                {
                    return _playerDatafile;
                }
            }
            return null;
        }
    }
}
