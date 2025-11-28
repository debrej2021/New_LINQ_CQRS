using DotNet_Quick_ref_all.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Quick_ref_all.FuzzySearch
{
    public class FuzzySearchService
    {
        private readonly AppDBContext _db;

        public FuzzySearchService(AppDBContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<object>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<object>();

            query = query.ToLower().Trim();

            // Fetch titles once from DB
            var items = await _db.TodoItems
                .Select(t => new { t.Id, t.Title })
                .ToListAsync();

            var results = new List<(int Id, string Title, int Score, string Type)>();

            foreach (var item in items)
            {
                var titleLower = (item.Title ?? string.Empty).ToLower();
                int score = int.MaxValue;
                string type = string.Empty;

                // 1️⃣ PREFIX MATCH (best)
                if (titleLower.StartsWith(query))
                {
                    score = 1;
                    type = "prefix";
                }
                // 2️⃣ SUBSTRING MATCH
                else if (titleLower.Contains(query))
                {
                    score = 2;
                    type = "substring";
                }
                else
                {
                    // 3️⃣ WORD-LEVEL FUZZY MATCH
                    // Split into words: "organize desk" -> ["organize","desk"]
                    var words = titleLower.Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    int bestDist = int.MaxValue;

                    foreach (var w in words)
                    {
                        int d = DamerauLevenshtein(query, w);
                        if (d < bestDist)
                            bestDist = d;
                    }

                    // Optional: also compare with the whole title for multi-word queries
                    if (query.Contains(' '))
                    {
                        int fullDist = DamerauLevenshtein(query, titleLower);
                        if (fullDist < bestDist)
                            bestDist = fullDist;
                    }

                    // Threshold: allow small mistakes like "deks" -> "desk"
                    if (bestDist <= 2)
                    {
                        score = 100 + bestDist; // worse than exact/prefix/substring
                        type = "fuzzy-word";
                    }
                }

                if (score < int.MaxValue)
                {
                    results.Add((item.Id, item.Title ?? string.Empty, score, type));
                }
            }

            return results
                .OrderBy(r => r.Score)
                .ThenBy(r => r.Title)
                .Select(r => new
                {
                    r.Id,
                    Title = r.Title,
                    MatchType = r.Type,
                    Score = r.Score
                })
                .ToList();
        }

        // Damerau–Levenshtein (with transposition)
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

        // Original Levenshtein (kept for reference)
        private int Levenshtein(string a, string b)
        {
            int[,] dp = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++)
                dp[i, 0] = i;

            for (int j = 0; j <= b.Length; j++)
                dp[0, j] = j;

            for (int i = 1; i < dp.GetLength(0); i++)
            {
                for (int j = 1; j < dp.GetLength(1); j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;

                    dp[i, j] = Math.Min(
                        Math.Min(
                            dp[i - 1, j] + 1,      // delete
                            dp[i, j - 1] + 1),     // insert
                        dp[i - 1, j - 1] + cost  // replace
                    );
                }
            }
            return dp[a.Length, b.Length];
        }
    }
}
