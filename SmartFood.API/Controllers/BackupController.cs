using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartFood.Domain;

namespace SmartFood.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class BackupController : Controller
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly IConfiguration configuration;

        public BackupController(ApplicationDbContext context, IConfiguration configuration)
        {
            this.appDbContext = context;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreateDbBackup()
        {
            try
            {
                var backupDirectoryPath = configuration.GetSection("BackupSettings:BackupDirectory").Value ?? "C:\\DefaultBackupDirectory";
                var backupFileName = $"db_backup_{DateTime.UtcNow:yyyyMMddHHmmss}.bak"; 

                var backupPath = Path.Combine(backupDirectoryPath, backupFileName);
                var backupCommand = $"BACKUP DATABASE SmartFood TO DISK = '{backupPath}'";
                appDbContext.Database.ExecuteSqlRaw(backupCommand);

                return Ok($"Backup created successfully, stored at {backupPath}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Backup creation failed: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetDbBackups()
        {
            try
            {
                var backupDirectoryPath = configuration.GetSection("BackupSettings:BackupDirectory").Value ?? "C:\\DefaultBackupDirectory";
                var backupDirectory = new DirectoryInfo(backupDirectoryPath);

                var backupFiles = backupDirectory.GetFiles("db_backup_*.bak")
                    .Select(fileInfo => new
                    {
                        FileName = fileInfo.Name,
                        CreatedAt = fileInfo.CreationTime
                    })
                    .OrderByDescending(backup => backup.CreatedAt)
                    .ToList();

                return Ok(backupFiles);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve backups: {ex.Message}");
            }
        }
    }
}
