using Sudoku.DAL;
using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sudoku.Services
{
    public class HangFireService
    {
        public void RestoreAuthorPostQuantity()
        {
            Debug.WriteLine("Start processing");
            AppDbContext db = new AppDbContext();
            var authors = db.AuthorService.ToList();
            foreach (var author in authors)
            {
                var packetPostQuantity = db.ServicePacks.Where(x => x.ID == author.ServiceId)
                                                               .Select(x => x.PostsPerDay)
                                                               .SingleOrDefault();
                author.RemainPostNumber = packetPostQuantity;
            }
            int num = db.SaveChanges();
            Debug.WriteLine(num);
            db.Dispose();
            Debug.WriteLine("Complete processing");
        }

        public void RankPlayer()
        {
            Debug.WriteLine("Start processing");
            AppDbContext db = new AppDbContext();

            var dateRank = DateTime.Now.AddMinutes(-utility.RankTime);

            var rankQuery = db.Submissions.Where(x => x.SubmitDate > dateRank && x.State == utility.StateCompleted)
                                            .OrderBy(x => x.SubmitDate)
                                            .GroupBy(x => new { x.UserId, x.ChapterId })
                                            .GroupBy(x => x.Key.UserId)
                                            .Select(x => new
                                            {
                                                UserId = x.Key,
                                                Value = x.Count(),
                                                DateRank = DateTime.Now
                                            }).OrderByDescending(x => x.Value)
                                             .ToList();
            List<Rank> ranks = new List<Rank>();
            int rank = 1;
            foreach (var rankItem in rankQuery)
            {
                Debug.WriteLine(rankItem.DateRank.ToString() + " " + rankItem.UserId + " " + rank);
                ranks.Add(new Rank
                {
                    UserId = rankItem.UserId,
                    Value = rank,
                    DateRank = rankItem.DateRank,
                    SubmitCount = rankItem.Value
                });
                rank++;
            }
            db.Ranks.AddRange(ranks);
            db.SaveChanges();
            db.Dispose();
            Debug.WriteLine("End processing");
        }
    }
}