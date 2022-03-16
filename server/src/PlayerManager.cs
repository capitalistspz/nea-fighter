using System;
using System.Collections.Generic;
using System.Linq;
using common.entities;
using common.utils;
using MonoGame.Extended.Collections;
using server.entities;

namespace server
{
    public class PlayerManager
    {
        private KeyedCollection<(int,ushort),ServerPlayerEntity> _players;

        public PlayerManager()
        {
            _players = new KeyedCollection<(int, ushort), ServerPlayerEntity>(p => p.Identity);
        }
        public void AddPlayer(ServerPlayerEntity player)
        {
            _players.Add(player);
        }

        public void RemovePlayer(ServerPlayerEntity player)
        {
            //var id = (long) netId << (Utils.SizeOf(localId) * 8) + localId;
            _players.Remove(player);
        }

        public ServerPlayerEntity GetPlayer(int netId, ushort localId)
        {
            //var id = (long)netId <<  (Utils.SizeOf(localId) * 8) + localId;
            return _players[(netId, localId)];
        }

        public IEnumerable<ServerPlayerEntity> GetPlayersByNetId(int netId)
        {
            return _players.Where(player => player.Identity.Item1 == netId);
        }
    }
}