using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Marsion
{
    public class RelayManager
    {

        // Host
        public struct RelayHostData
        {
            public string JoinCode;
            public string IPv4Address;
            public ushort Port;
            public Guid AllocationId;
            public byte[] AllocationIdBytes;
            public byte[] ConnectionData;
            public byte[] Key;
        }

        public static async Task<RelayHostData> SetupRelay(int _maxConn, string _environment)
        {
            InitializationOptions options = new InitializationOptions()
                .SetEnvironmentName(_environment);

            await UnityServices.InitializeAsync(options);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maxConn);

            RelayHostData data = new RelayHostData
            {
                IPv4Address = allocation.RelayServer.IpV4,
                Port = (ushort)allocation.RelayServer.Port,

                AllocationId = allocation.AllocationId,
                AllocationIdBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                Key = allocation.Key
            };

            data.JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return data;
        }

        // Guest
        public struct RelayJoinData
        {
            public string IPv4Address;
            public ushort Port;
            public Guid AllocationId;
            public byte[] AllocationIdBytes;
            public byte[] ConnectionData;
            public byte[] HostConnectionData;
            public byte[] Key;
        }

        public static async Task<RelayJoinData> JoinRelay(string _joinCode, string _environment)
        {
            JoinAllocation allocation = null;

            try
            {
                Debug.LogError($"Start Join by {_joinCode}");

                InitializationOptions options = new InitializationOptions()
                    .SetEnvironmentName(_environment);

                await UnityServices.InitializeAsync(options);

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                allocation = await RelayService.Instance.JoinAllocationAsync(_joinCode);

                Debug.Log("Successfully joined relay with join code: " + _joinCode);
            }
            catch (RelayServiceException e)
            {
                Debug.LogError("RelayServiceException: " + e.Message);
            }

            RelayJoinData data = new RelayJoinData
            {
                IPv4Address = allocation.RelayServer.IpV4,
                Port = (ushort)allocation.RelayServer.Port,

                AllocationId = allocation.AllocationId,
                AllocationIdBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                Key = allocation.Key
            };

            return data;
        }
    }
}