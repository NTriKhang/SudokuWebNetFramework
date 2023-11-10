using Microsoft.AspNet.SignalR;
using Stripe.BillingPortal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.EnterpriseServices.CompensatingResourceManager;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sudoku.Hubs
{
    public class ContestHub : Hub
    {
        public async Task JoinRoom(string roomId)
        {
            await Groups.Add(Context.ConnectionId, roomId);
            int code = 200;
            await Clients.OthersInGroup(roomId).Notificate(code);
            code = 201;
            //Debug.WriteLine(Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).Notificate(code);
        }
        public async Task QuitRoom(string roomId, string roomOwnId = null)
        {
            int code = 200;
            if (roomOwnId == null)
            {
                await Clients.OthersInGroup(roomId).Notificate(code);
            }
            else
            {
                await Clients.OthersInGroup(roomId).Disperse(code);
            }
            try
            {
                await Groups.Remove(Context.ConnectionId, roomId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task CreateRoom(string roomId)
        {
            await Groups.Add(Context.ConnectionId, roomId);
            var data = new { code = 200, id = Context.ConnectionId };
            await Clients.Client(Context.ConnectionId).RoomCreateSuccessfully(data);
        }
        public async Task StartContest(string roomId, string riddle, int rows)
        {
            var data = new { code = 200, riddle = riddle, rows = rows, roomId = roomId };
            await Clients.Group(roomId).TakeRiddle(data);
        }
        public async Task EndContest(string roomId, string userName, string userImage)
        {
            await Clients.Group(roomId).NotifyEndContest(userName, userImage);
        }
    }
}