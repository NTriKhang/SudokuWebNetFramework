using Sudoku.Controllers;
using Sudoku.Dtos.Outgoing;
using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Sudoku.Services
{
    public class RoomService
    {
        public List<ContestDto> GetContests(List<Room> rooms)
        {
            List<ContestDto> contestDtos = new List<ContestDto>();

            foreach (var room in rooms)
            {
                contestDtos.Add(new ContestDto
                {
                    Id = room.Id,
                    Point = room.Point,
                    Level = room.Level,
                    Name = room.Name,
                    NumberOfJoined = room.UsersId.Count
                });
            }
            return contestDtos;
        }
        public string GetRoomOwnId(List<Room> rooms, string roomId)
        {
            var room = rooms.Where(x => x.Id == roomId).SingleOrDefault();
            if (room == null)
                return "";
            return room.roomOwnId;
        }
        public bool IsOwnRoom(List<Room> rooms, string userId)
        {
            var room = rooms.Where(x => x.roomOwnId == userId).SingleOrDefault();
            if (room == null)
                return false;
            return true;
        }
    }
}