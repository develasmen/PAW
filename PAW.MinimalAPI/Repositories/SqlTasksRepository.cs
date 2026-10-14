using System.Data;
using Microsoft.Data.SqlClient;
using PawTask = PAW.Models.Task;

namespace PAW.MinimalAPI.Repositories;

// ADO.NET: consulta la tabla existente sin Entity Framework ni DbContext.
public sealed class SqlTasksRepository(IConfiguration configuration) : ITasksRepository
{
    private const string Columns = "Id, Name, Description, Status, DueDate, CreatedAt, LastModified, ModifiedBy";
    private const string OutputColumns = "INSERTED.Id, INSERTED.Name, INSERTED.Description, INSERTED.Status, INSERTED.DueDate, INSERTED.CreatedAt, INSERTED.LastModified, INSERTED.ModifiedBy";

    private async Task<SqlConnection> OpenAsync(CancellationToken ct)
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Tasks")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:Tasks."));
        try { await connection.OpenAsync(ct); return connection; }
        catch { await connection.DisposeAsync(); throw; }
    }

    public async Task<IReadOnlyList<PawTask>> GetAllAsync(CancellationToken ct)
    {
        await using var connection = await OpenAsync(ct);
        await using var command = new SqlCommand($"SELECT {Columns} FROM dbo.Tasks ORDER BY Id", connection);
        await using var reader = await command.ExecuteReaderAsync(ct);
        var tasks = new List<PawTask>();
        while (await reader.ReadAsync(ct)) tasks.Add(Read(reader));
        return tasks;
    }

    public async Task<PawTask?> GetByIdAsync(int id, CancellationToken ct)
    {
        await using var connection = await OpenAsync(ct);
        await using var command = new SqlCommand($"SELECT {Columns} FROM dbo.Tasks WHERE Id = @Id", connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? Read(reader) : null;
    }

    public async Task<PawTask> CreateAsync(PawTask task, CancellationToken ct)
    {
        await using var connection = await OpenAsync(ct);
        await using var command = new SqlCommand($"""
            INSERT INTO dbo.Tasks (Name, Description, Status, DueDate, CreatedAt, LastModified, ModifiedBy)
            OUTPUT {OutputColumns}
            VALUES (@Name, @Description, @Status, @DueDate, GETUTCDATE(), GETUTCDATE(), @ModifiedBy)
            """, connection);
        AddParameters(command, task);
        await using var reader = await command.ExecuteReaderAsync(ct);
        await reader.ReadAsync(ct);
        return Read(reader);
    }

    public async Task<PawTask?> UpdateAsync(int id, PawTask task, CancellationToken ct)
    {
        await using var connection = await OpenAsync(ct);
        await using var command = new SqlCommand($"""
            UPDATE dbo.Tasks
            SET Name = @Name, Description = @Description, Status = @Status,
                DueDate = @DueDate, LastModified = GETUTCDATE(), ModifiedBy = @ModifiedBy
            OUTPUT {OutputColumns}
            WHERE Id = @Id
            """, connection);
        AddParameters(command, task);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? Read(reader) : null;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        await using var connection = await OpenAsync(ct);
        await using var command = new SqlCommand("DELETE FROM dbo.Tasks WHERE Id = @Id", connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        return await command.ExecuteNonQueryAsync(ct) > 0;
    }

    private static void AddParameters(SqlCommand command, PawTask task)
    {
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 255).Value = task.Name!.Trim();
        command.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value = (object?)task.Description ?? DBNull.Value;
        command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = (object?)task.Status ?? DBNull.Value;
        command.Parameters.Add("@DueDate", SqlDbType.DateTime).Value = (object?)task.DueDate ?? DBNull.Value;
        command.Parameters.Add("@ModifiedBy", SqlDbType.NVarChar, 255).Value = (object?)task.ModifiedBy ?? DBNull.Value;
    }

    private static PawTask Read(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Name = reader.IsDBNull(1) ? null : reader.GetString(1),
        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
        Status = reader.IsDBNull(3) ? null : reader.GetString(3),
        DueDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
        CreatedAt = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
        LastModified = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
        ModifiedBy = reader.IsDBNull(7) ? null : reader.GetString(7)
    };
}