using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Outgoing
{
    public class CDiscuss
    {
        public string DiscussId { set; get; }
        public string TextContent { set; get; }
        public DateTime Date { set; get; }
        public string UserName { set; get; }
        public string UserImage_Path { set; get; }
        public string UserId { set; get; }
    }
    public class DiscussionDto
    {
        public string ChapterId { get; set; }
        public string CurrentUserId { set; get; }
        public List<CDiscuss> Discuss { set; get; }

    }
}