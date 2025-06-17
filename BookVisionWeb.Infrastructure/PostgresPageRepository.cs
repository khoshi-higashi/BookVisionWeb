using BookVisionWeb.Domain;
using BookVisionWeb.UseCase;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace BookVisionWeb.Infrastructure
{
    /// <summary>
    /// PostgreSQLでのページ永続化リポジトリ実装
    /// </summary>
    public class PostgresPageRepository : IPageRepository
    {
        private readonly string _connectionString;

        public PostgresPageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // 非同期でページ情報を保存
        public async Task SaveAsync(Page page)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // ページが存在すればUPDATE、なければINSERT（UPSERT）
            using var cmd = new NpgsqlCommand(
                @"INSERT INTO pages (id, image_path, ocr_text, ocr_status, text, registered_at)
                  VALUES (@id, @imagePath, @ocrText, @ocrStatus, @text, @registeredAt)
                  ON CONFLICT (id) DO UPDATE SET
                    ocr_text = EXCLUDED.ocr_text,
                    ocr_status = EXCLUDED.ocr_status,
                    text = EXCLUDED.text,
                    registered_at = EXCLUDED.registered_at;", conn);

            cmd.Parameters.AddWithValue("@id", page.Id.Value);
            cmd.Parameters.AddWithValue("@imagePath", page.ImagePath ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ocrText", (object?)page.OcrText ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ocrStatus", (int)page.OcrStatus);
            cmd.Parameters.AddWithValue("@text", (object?)page.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@registeredAt", (object?)page.RegisteredAt ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        // 非同期でページ情報をIDで取得
        public async Task<Page?> FindAsync(PageId id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                "SELECT id, image_path, ocr_text, ocr_status, text, registered_at FROM pages WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id.Value);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var pageId = new PageId(reader.GetGuid(0));
                var imagePath = reader.GetString(1);
                var page = new Page(pageId, imagePath);
                if (!reader.IsDBNull(2)) page.AttachOcr(reader.GetString(2));
                if (!reader.IsDBNull(3)) page.SetOcrStatus((OcrStatus)reader.GetInt32(3));
                if (!reader.IsDBNull(4)) page.SetText(reader.GetString(4));
                if (!reader.IsDBNull(5)) page.SetRegisteredAt(reader.GetDateTime(5));
                return page;
            }
            return null;
        }
    }
}