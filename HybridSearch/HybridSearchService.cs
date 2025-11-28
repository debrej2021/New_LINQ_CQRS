using DotNet_Quick_ref_all.DTO;
using DotNet_Quick_ref_all.FuzzySearch;
using DotNet_Quick_ref_all.HybridSearch;
using DotNet_Quick_ref_all.Infrastructure;
using DotNet_Quick_ref_all.OpenAI;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Quick_ref_all.HybridSearch
{
    public class HybridSearchService
    {
        private readonly AppDBContext _db;
        private readonly SmartFuzzySearchService _fuzzy;
        private readonly OpenAiVectorSearchService _vector;

        public HybridSearchService(
            AppDBContext db,
            SmartFuzzySearchService fuzzy,
            OpenAiVectorSearchService vectorSearch)
        {
            _db = db;
            _fuzzy = fuzzy;
            _vector = vectorSearch;
        }

        public async Task<HybridResult> SearchAsync(string query)
        {
            query = query.Trim().ToLower();

            // 1️⃣ Keyword search (LIKE)
            var keywordResults = await _db.TodoItems
                .Where(t => t.Title.ToLower().Contains(query))
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    Score = 1.0,
                    Source = "keyword"
                })
                .ToListAsync();

            var kList = keywordResults
                .Select(k => (k.Id, k.Title, k.Score, k.Source));

            // 2️⃣ Fuzzy search
            var fuzzyObj = await _fuzzy.SearchAsync(query);
            var fList = fuzzyObj.Results.Select(r =>
                ((int)r.GetType().GetProperty("Id")!.GetValue(r)!,
                 (string)r.GetType().GetProperty("Title")!.GetValue(r)!,
                 0.8 - ((int)r.GetType().GetProperty("Score")!.GetValue(r)! * 0.01),
                 (string)r.GetType().GetProperty("MatchType")!.GetValue(r)!
                ));

            // 3️⃣ Semantic search (OpenAI vectors)
            var semantic = await _vector.SearchAsync(query, topK: 5);
            var sList = semantic.Select(r =>
                ((int)r.GetType().GetProperty("Id")!.GetValue(r)!,
                 (string)r.GetType().GetProperty("Text")!.GetValue(r)!,
                 (double)r.GetType().GetProperty("Score")!.GetValue(r)!,
                 "semantic")
                );

            // 4️⃣ Rank fusion
            var fused = HybridRanker.MergeAndRank(kList, fList, sList);

            return new HybridResult
            {
                Query = query,
                Results = fused.Select(f => (object)new
                {
                    Id = f.Id,
                    Title = f.Title,
                    FinalScore = f.Score,
                    Source = f.Source
                }).ToList()
            };
        }
    }
}
