﻿using hackathonsqlazurebot.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Sql;
using Microsoft.WindowsAzure.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace hackathonsqlazurebot
{
    public class AzureManagement
    {
        public async Task<IEnumerable<Subscription>> ListAzureSubscriptionsAsync(string token)
        {
            var credentials = new TokenCloudCredentials(token);
            SubscriptionClient subscriptionClient = null;
            using (subscriptionClient = new SubscriptionClient(credentials))
            {
                var subscriptionsResult = subscriptionClient.Subscriptions.List();
                var subscriptions = subscriptionsResult.Subscriptions.OrderBy(x => x.SubscriptionName).Select(sub => 
                new Subscription { SubscriptionId = sub.SubscriptionId, DisplayName = sub.SubscriptionName }).ToList();
                return subscriptions;
            }
        }

        public async Task<List<SQLServer>> GetSQLServers(string subscription, string token)
        {
            var credentials = new TokenCloudCredentials(subscription, token);

            using (SqlManagementClient sqlManagementClient = 
                new SqlManagementClient(credentials))
            {
                var serversResult = await sqlManagementClient.Servers.ListAsync();
                List<SQLServer> serverNames = serversResult.OrderBy(s => s.FullyQualifiedDomainName).Select(s => new SQLServer()
                {
                     DisplayName = s.FullyQualifiedDomainName
                }).ToList();
                return await Task.FromResult(serverNames);
            }
        }

        public async Task<List<string>> GetSQLServerInfo(string token, string subscriptionid, string servername)
        {
            var credentials = new TokenCloudCredentials(subscriptionid, token);
            using (SqlManagementClient sqlManagementClient =
                new SqlManagementClient(credentials))
            {
                var servers = await sqlManagementClient.Servers.ListAsync();
                var server = (from s in servers where s.Name == servername select s).FirstOrDefault();
                var serverInfo = new List<string>();
                serverInfo.Add("Name: " + server.FullyQualifiedDomainName);
                serverInfo.Add("Location:" + server.Location);
                serverInfo.Add("Version: " + server.Version);
                return await Task.FromResult(serverInfo);
            }
        }

    }
}