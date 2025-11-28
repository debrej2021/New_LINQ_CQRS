using DotNet_Quick_ref_all.Infrastructure;
using Microsoft.EntityFrameworkCore;
using DotNet_Quick_ref_all.DTO;
using DotNet_Quick_ref_all.Models;
namespace DotNet_Quick_ref_all.FuzzySearch
{
    public class FuzzySearchServiceDUM
    {
        private readonly AppDBContext _db;

        public FuzzySearchServiceDUM(AppDBContext db)
        {
            _db = db;
        }

        public async Task<object> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new { query, suggestion = (string?)null, results = new List<object>() };

            query = query.ToLower().Trim();

            var items = await _db.TodoItems
                .Select(t => new { t.Id, t.Title })
                .ToListAsync();

            var results = new List<(int Id, string Title, int Score, string Type)>();
            var allWords = new List<string>();

            // Pre-extract words for suggestion engine
            foreach (var i in items)
            {
                if (!string.IsNullOrWhiteSpace(i.Title))
                {
                    allWords.AddRange(
                        i.Title!.ToLower()
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                }
            }

            foreach (var item in items)
            {
                var titleLower = (item.Title ?? string.Empty).ToLower();
                int score = int.MaxValue;
                string type = string.Empty;

                // 1) prefix
                if (titleLower.StartsWith(query))
                {
                    score = 1;
                    type = "prefix";
                }
                // 2) substring
                else if (titleLower.Contains(query))
                {
                    score = 2;
                    type = "substring";
                }
                else
                {
                    var words = titleLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    int bestDist = int.MaxValue;

                    foreach (var w in words)
                    {
                        int d = DamerauLevenshtein(query, w);
                        if (d < bestDist)
                            bestDist = d;
                    }

                    if (bestDist <= 2)
                    {
                        score = 100 + bestDist;
                        type = "fuzzy-word";
                    }
                }

                if (score < int.MaxValue)
                    results.Add((item.Id, item.Title ?? "", score, type));
            }

            // Build suggestion only if no great prefix/substr match
            string? suggestion = null;

            if (!results.Any(r => r.Score <= 2))   // no prefix / substring matches
            {
                suggestion = SuggestClosestWord(query, allWords);
            }

            var output = new
            {
                query,
                suggestion,
                results = results
                    .OrderBy(r => r.Score)
                    .Select(r => new { r.Id, r.Title, MatchType = r.Type, r.Score })
                    .ToList()
            };

            return output;
        }

        public async Task<FuzzySearchResult> SearchAsyncsuggestions(string query)
        {
            query = query.ToLower();

            // Load all titles (no filtering!)
            var items = await _db.TodoItems
                .Select(t => new { t.Id, Title = t.Title.ToLower() })
                .ToListAsync();

            // Compute Levenshtein score for all
            var scored = items
                .Select(i => new
                {
                    i.Id,
                    i.Title,
                    Distance = DamerauLevenshtein(query, i.Title)
                })
                .OrderBy(x => x.Distance)
                .ToList();

            // Best suggestion = smallest distance
            var best = scored.FirstOrDefault();

            return new FuzzySearchResult
            {
                Query = query,
                Suggestion = best?.Title,
                Results = scored
        .Where(x => x.Distance <= 3)
        .Select(x => (object)new
        {
            x.Id,
            Title = x.Title,
            MatchType = "fuzzy-word",
            Score = x.Distance
        })
        .ToList()
            };
        }


        private string? SuggestClosestWord(string query, List<string> allWords)
        {
            string? bestWord = null;
            int bestDist = int.MaxValue;

            foreach (var word in allWords)
            {
                int d = DamerauLevenshtein(query, word);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestWord = word;
                }
            }

            // Suggest only if within reasonable distance
            return bestDist <= 2 ? bestWord : null;
        }

        private int DamerauLevenshtein(string a, string b)
        {
            int lenA = a.Length;
            int lenB = b.Length;
            int[,] dp = new int[lenA + 1, lenB + 1];

            for (int i = 0; i <= lenA; i++) dp[i, 0] = i;
            for (int j = 0; j <= lenB; j++) dp[0, j] = j;

            for (int i = 1; i <= lenA; i++)
            {
                for (int j = 1; j <= lenB; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;

                    dp[i, j] = Math.Min(
                        Math.Min(
                            dp[i - 1, j] + 1,       // deletion
                            dp[i, j - 1] + 1),      // insertion
                        dp[i - 1, j - 1] + cost    // substitution
                    );

                    // Transposition
                    if (i > 1 && j > 1 &&
                        a[i - 1] == b[j - 2] &&
                        a[i - 2] == b[j - 1])
                    {
                        dp[i, j] = Math.Min(dp[i, j], dp[i - 2, j - 2] + cost);
                    }
                }
            }

            return dp[lenA, lenB];
        }


    }
}
