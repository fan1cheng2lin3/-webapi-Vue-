using Microsoft.EntityFrameworkCore;
using MyWordStystemWebapi.Data;
using MyWordStystemWebapi.Models;

namespace MyWordStystemWebapi.Services.Word
{
    public class CiKuService
    {
        private readonly MywordDbContext _context;
        private readonly Dictionary<string, DbSet<CiKuWord>> _viewNameToDbSetMap;

        public CiKuService(MywordDbContext context)
        {
            _context = context;
        }


        public async Task<List<CiKuWord>> GetWordsByViewNameAsync(string viewName, int pageNumber = 1, int pageSize = 50)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                Console.WriteLine("无效的视图名称");
                return new List<CiKuWord>(); // 返回空列表
            }

            var query = $"SELECT * FROM {viewName} ORDER BY Id OFFSET {(pageNumber - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            // 使用异步查询
            var words = await _context.CiKuWords.FromSqlRaw(query).ToListAsync();
            if (!words.Any())
            {
                Console.WriteLine($"视图 {viewName} 返回为空");
            }
            return words;

        }


    


    }
}
