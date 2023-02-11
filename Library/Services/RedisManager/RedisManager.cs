﻿using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.RedisManager
{

    public enum RedisServer
    {
    }

    public class RedisManager : IRedisManager
    {
        #region Methods

        public void Set<T>(string key, int expireTime, T cacheItem, int database = 0)
        {
            IDatabase db = RedisConnection.ConnectionLocal.GetDatabase(database);
            db.StringSet(key, JsonConvert.SerializeObject(cacheItem), TimeSpan.FromMilliseconds(expireTime));
        }

        public void SetInMin<T>(string key, int expireTime, T cacheItem, int database = 0)
        {
            IDatabase db = RedisConnection.ConnectionLocal.GetDatabase(database);
            db.StringSet(key, JsonConvert.SerializeObject(cacheItem), TimeSpan.FromMinutes(expireTime));
        }

        //public void AddToken(string username, string token)
        //{
        //    _cache.SetInMin(username, _jwtOptions.Value.ExpiryMinutes, token, 0, (byte)RedisServer.Local);
        //}

        public void Set<T>(string key, TimeSpan expireTime, T cacheItem, int database = 0)
        {
            IDatabase db = RedisConnection.ConnectionLocal.GetDatabase(database);
            db.StringSet(key, JsonConvert.SerializeObject(cacheItem), expireTime);
        }

        public T Get<T>(string key, int database = 0)
        {
            IDatabase db = RedisConnection.ConnectionLocal.GetDatabase(database);
            return JsonConvert.DeserializeObject<T>(db.StringGet(key));
        }

        public T AddOrGetExisting<T>(string key, int expireTime, Func<T> valueFactory, int database = 0)
        {
            IDatabase db = RedisConnection.ConnectionLocal.GetDatabase(database);
            if (db.KeyExists(key))
            {
                return JsonConvert.DeserializeObject<T>(db.StringGet(key));
            }
            else
            {
                var val = valueFactory();
                db.StringSet(key, JsonConvert.SerializeObject(val), TimeSpan.FromMilliseconds(expireTime));
                return val;
            }

        }

        public bool IsExist(string key, int database = 0)
        {
            IDatabase db = RedisConnection.ConnectionLocal.GetDatabase(database);
            if (db.KeyExists(key))
                return true;
            else
                return false;

        }

        public void Remove(string key, int database = 0)
        {
            IDatabase db = RedisConnection.ConnectionLocal.GetDatabase(database);
            db.KeyDelete(key);
        }

        public void RemoveByPatterns(string pattern = "", int database = 0)
        {

            IDatabase db = RedisConnection.ConnectionLocal.GetDatabase(database);
            //var server = RedisConnection.ConnectionLocal.GetEndPoints().Select(x =>
            //{
            //    return RedisConnection.ConnectionLocal.GetServer(x);
            //}).FirstOrDefault();

            var keys = RedisConnection.ServerLocal.Keys(database, pattern: pattern + "*").ToArray();
            foreach (var item in keys)
            {
                db.KeyDelete(item);
            }
        }

        public IList<T> GetAllkeysData<T>(string pattern, int database = 0)
        {
            IDatabase db = RedisConnection.ConnectionLocal.GetDatabase(database);
            //var server = RedisConnection.ConnectionLocal.GetEndPoints().Select(x =>
            //{
            //    return RedisConnection.ConnectionLocal.GetServer(x);
            //}).FirstOrDefault();

            var keys = RedisConnection.ServerLocal.Keys(database, pattern: pattern + "*").ToArray();
            IList<T> data = new List<T>();

            foreach (var key in keys)
            {
                data.Add(JsonConvert.DeserializeObject<T>(db.StringGet(key)));
            }

            return data.ToList();
        }

        public IList<string> GetAllkeys(string pattern, int database = 0)
        {
            IDatabase db = RedisConnection.ConnectionLocal.GetDatabase(database);
            //var server = RedisConnection.ConnectionLocal.GetEndPoints().Select(x =>
            //{
            //    return RedisConnection.ConnectionLocal.GetServer(x);
            //}).FirstOrDefault();

            var keys = RedisConnection.ServerLocal.Keys(database, pattern: pattern + "*").ToArray();
            IList<string> data = new List<string>();

            foreach (var key in keys)
            {
                data.Add(key);
            }

            return data;
        }

        #endregion
    }
}
