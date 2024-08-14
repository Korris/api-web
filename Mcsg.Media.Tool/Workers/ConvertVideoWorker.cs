using Newtonsoft.Json;

namespace Mcsg.Media.Tool.Workers
{
    using Common.Core.Enums;
    using Common.Core.Interfaces;
    using Common.Core.Requests;
    using Common.Domain.Entities;
    using Common.SeedWork.Extensions;
    using Interfaces;

    internal class ConvertVideoWorker : BaseWorker, IWorker
    {
        private const string TARGET = ".mp4";

        public ConvertVideoWorker(ISetting setting, IStorageClient sc) : base(setting, sc) { }

        public void Execute(Job jobInfo)
        {
            if (jobInfo.JobType != JobType.ConvertVideo)
                return;

            AddToPools(jobInfo.Id, Task.Factory.StartNew(async () =>
            {
                try
                {
                    // load resource
                    var resourceInfo = JsonConvert.DeserializeObject<BaseResource>(jobInfo.Data);
                    var url = resourceInfo.Url;
                    var orgfile = await DownloadBlobAsync(url, resourceInfo.Id);
                    var microService = resourceInfo.MicroService.ToEnum(MicroService.Social);

                    var targetFile = Path.Combine(Path.GetDirectoryName(orgfile), Path.GetFileNameWithoutExtension(url) + TARGET);
                    if (File.Exists(targetFile))
                    {
                        File.Delete(targetFile);
                    }
                    if (Path.GetFileName(orgfile) != Path.GetFileName(targetFile))
                    {
                        //Run conversion
                        // string command = "-c:v libvpx -b:v 1M -c:a libvorbis -b:a 192K";
                        //veryslow,slower,slow, medium, fast,faster,veryfast,superfast, ultrafast 
                        //string command = "-c:v libx264 -vf \"scale=trunc(iw/6)*2:trunc(ih/6)*2\" -b:v 1000k -preset faster -crf 28 -c:a aac -b:a 64k"; // optimizer
                        string command = "-c:v libx264 -vf \"scale=trunc(iw/6)*2:trunc(ih/6)*2\" -b:v 1000k -preset faster -crf 32 -c:a aac -b:a 64k"; // optimizer
                        RunFFmeg("ffmpeg", orgfile, targetFile, command);

                        //upload
                        var newUrl = url.Replace(Path.GetExtension(targetFile), TARGET);
                        await UploadBlobAsync(targetFile, newUrl);

                        //update job status
                        await DbService.UpdateJobStatus(jobInfo.Id, JobStatus.Success, string.Empty);

                        await DbService.UpdateResourceStatus(resourceInfo.Id, ResourceStatus.Done, newUrl, newUrl, microService);
                    }

                    //clean up resource
                    File.Delete(orgfile);
                    File.Delete(targetFile);

                    var pool = Pools.FirstOrDefault(x => x.Id == jobInfo.Id);
                    if (pool != null)
                    {
                        pool.IsRunning = false;
                    }

                    //Send notification when video process completed
                    var video = await DbService.LoadResource(resourceInfo.HashId, microService);

                    Console.WriteLine("ConvertVideo Job Id: {0} - Video HashId : {1} - Video Id {2}", jobInfo.Id, resourceInfo.HashId, video.Id);

                    if (video != null && !string.IsNullOrWhiteSpace(video.HashId))
                    {
                        var notiReq = new VideoNotificationR
                        {
                            Id = video.Id,
                            AuthorId = video.AuthorId,
                            AuthorName = video.AuthorName,
                            Action = NotificationAction.Completed,
                            HashId = resourceInfo.HashId,
                            PostId = video.PostId,
                            PostHashId = video.PostHashId,
                            TargetType = Common.Core.Constants.Setting.NotificationTargetType.Feed
                        };

                        await NotiService.AddVideoNotificationAsync(notiReq, _setting.Api.Realtime);
                    }
                }
                catch (Exception ex)
                {
                    await DbService.UpdateJobStatus(jobInfo.Id, JobStatus.Failed, ex.Message);
                }
            }));
        }
    }
}
