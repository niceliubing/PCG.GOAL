﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace PCG.GOAL.WebService.Security
{
    public class GoalRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static readonly ConcurrentDictionary<string, AuthenticationTicket> RefreshTokens
            = new ConcurrentDictionary<string, AuthenticationTicket>();


        public void Receive(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            if (RefreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
            }
        }
        public void Create(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();
            // maybe only create a handle the first time, then re-use for same client
            // copy properties and set the desired lifetime of refresh token
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddYears(1)
            };
            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            //_refreshTokens.TryAdd(guid, context.Ticket);
            RefreshTokens.TryAdd(guid, refreshTokenTicket);

            // consider storing only the hash of the handle
            context.SetToken(guid);
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Create(context);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Receive(context);
        }

    }
}