using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MRCopyFile.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MRCopyFile
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private DateTime _lastReadConfig = DateTime.MinValue;
        private List<Job> _cached;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                var jobs = ReadConfig();
                foreach (var job in jobs)
                {
                    ExecuteJob(job);
                }

                await Task.Delay(5000, stoppingToken);
            }
        }

        private List<Job> ReadConfig()
        {
            if ((DateTime.Now - _lastReadConfig).TotalSeconds > 300)
            {
                _cached = _config.GetSection("Jobs").Get<List<Job>>();
                _lastReadConfig = DateTime.Now;
            }
            return _cached;
        }

        private void ExecuteJob(Job job)
        {
            if ((DateTime.Now - job.LastChecked).TotalSeconds >= job.Interval)
            {
                if (Directory.Exists(job.Target))
                {

                    try
                    {
                        var files = Directory.GetFiles(Path.GetDirectoryName(job.Source), Path.GetFileName(job.Source));
                        foreach (var file in files)
                        {
                            var fileName = Path.Combine(job.Target, Path.GetFileName(file));

                            _logger.LogDebug("Movendo arquivo \"{file}\" para \"{fileName}\"...", file, fileName);
                            File.Move(file, fileName);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    _logger.LogWarning("O diretório de destino não existe: \"{target}\".", job.Target);
                }

                job.LastChecked = DateTime.Now;
            }

        }
    }
}

