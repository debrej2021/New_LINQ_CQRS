using DotNet_Quick_ref_all.DTO;
using DotNet_Quick_ref_all.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Quick_ref_all.FuzzySearch
{
    public class SmartFuzzySearchService
    {
        private readonly AppDBContext _db;

        public SmartFuzzySearchService(AppDBContext db)
        {
            _db = db;
        }

        public async Task<SmartFuzzyResult> SearchAsync(string query)
        {
            query = query.Trim().ToLower();

            var items = await _db.TodoItems
                .Select(t => new { t.Id, Title = t.Title.ToLower() })
                .ToListAsync();

            var scored = new List<(int Id, string Title, int Score, string Type)>();

            foreach (var item in items)
            {
                int score = 1000;
                string type = "";

                // 1️⃣ PREFIX MATCH — Highest priority
                if (item.Title.StartsWith(query))
                {
                    score = 1;
                    type = "prefix";
                }
                // 2️⃣ SUBSTRING
                else if (item.Title.Contains(query))
                {
                    score = 2;
                    type = "substring";
                }
                else
                {
                    // 3️⃣ WORD-BASED FUZZY
                    var words = item.Title.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    int bestLev = words.Min(word => DamerauLevenshtein(query, word));

                    if (bestLev <= 2)
                    {
                        score = 100 + bestLev;
                        type = "word-fuzzy";
                    }

                    // 4️⃣ FULL STRING TYPO CORRECTION
                    int fullLev = DamerauLevenshtein(query, item.Title);
                    if (fullLev <= 3)
                    {
                        score = Math.Min(score, 200 + fullLev);
                        type = "full-fuzzy";
                    }
                }

                if (score < 1000)
                    scored.Add((item.Id, item.Title, score, type));
            }

            // 5️⃣ Suggestion: smallest score among fuzzy matches
            var bestSuggestion = scored
                .Where(x => x.Type != "prefix" && x.Type != "substring")
                .OrderBy(x => x.Score)
                .Select(x => x.Title)
                .FirstOrDefault();

            // 6️⃣ Final ranked output
            var results = scored
                .OrderBy(x => x.Score)
                .ThenBy(x => x.Title)
                .Select(x => (object)new
                {
                    x.Id,
                    Title = x.Title,
                    MatchType = x.Type,
                    Score = x.Score
                })
                .ToList();

            return new SmartFuzzyResult
            {
                Query = query,
                Suggestion = bestSuggestion,
                Results = results
            };
        }

        // Damerau-Levenshtein (swaps, deletes, inserts, replaces)
        private int DamerauLevenshtein(string a, string b)
        {
            int lenA = a.Length, lenB = b.Length;
            int[,] dp = new int[lenA + 1, lenB + 1];

            for (int i = 0; i <= lenA; i++) dp[i, 0] = i;
            for (int j = 0; j <= lenB; j++) dp[0, j] = j;

            for (int i = 1; i <= lenA; i++)
            {
                for (int j = 1; j <= lenB; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;

                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost);

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
