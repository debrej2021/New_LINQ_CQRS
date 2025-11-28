namespace DotNet_Quick_ref_all.HybridSearch
{
    public static class HybridRanker
    {
        public static List<(int Id, string Title, double Score, string Source)>
            MergeAndRank(
                IEnumerable<(int Id, string Title, double Score, string Source)> keyword,
                IEnumerable<(int Id, string Title, double Score, string Source)> fuzzy,
                IEnumerable<(int Id, string Title, double Score, string Source)> semantic)
        {
            var map = new Dictionary<int, (string Title, double Score, string Source)>();

            void insert(IEnumerable<(int Id, string Title, double Score, string Source)> list)
            {
                foreach (var item in list)
                {
                    if (!map.ContainsKey(item.Id))
                        map[item.Id] = (item.Title, item.Score, item.Source);
                    else
                        map[item.Id] = (
                            item.Title,
                            map[item.Id].Score + item.Score,  // cumulative scoring
                            map[item.Id].Source + "+" + item.Source
                        );
                }
            }

            insert(keyword);
            insert(fuzzy);
            insert(semantic);

            return map
                .OrderByDescending(x => x.Value.Score)
                .Select(x =>
                    (x.Key, x.Value.Title, x.Value.Score, x.Value.Source)
                )
                .ToList();
        }
    }
}
