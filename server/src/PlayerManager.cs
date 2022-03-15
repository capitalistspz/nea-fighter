using System.Collections.Generic;
using common.entities;
using common.utils;
using MonoGame.Extended.Collections;
using server.entities;

namespace server
{
    public class PlayerManager
    {
        private Dictionary<long, ServerPlayerEntity> _players = new ( );

        public void AddPlayer(ServerPlayerEntity player, int netId)
        {
            var id = player.LocalPlayerID + (long) (netId << (Utils.SizeOf(player.LocalPlayerID) * 8));
            _players[id] = player;
        }

        public void RemovePlayer(int netId, ushort localId)
        {
            var id = (long) netId << (Utils.SizeOf(localId) * 8) + localId;
            _players.Remove(id);
        }

        public ServerPlayerEntity GetPlayer(int netId, ushort localId)
        {
            var id = (long)netId <<  (Utils.SizeOf(localId) * 8) + localId;
            return _players[id];
        }
    }

    internal class Identifier
    {
        private ushort localId;
        private int netId;

        void t()
        {
            var t = (netId << (Utils.SizeOf(localId) * 8)) + localId;
        }
    }
}