using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mcsg.Story.Api.Queries;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class StoryPostViewParagraphH : BaseH, IRequestHandler<StoryPostViewParagraphR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public StoryPostViewParagraphH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(StoryPostViewParagraphR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new StoryPostViewParagraphV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        // Retrieve and parse the StorySubPost Body as JSON
        var jsonBody = await _context.Available<StorySubPost>(false)
            .Where(p => p.Id == request.Id)
            .Select(p => p.Body)
            .FirstOrDefaultAsync(cancellationToken);
        if (jsonBody == null)
        {
            return res;
        }

        var textData = JsonConvert.DeserializeObject<List<ChildrenObjectData>>(JObject.Parse(jsonBody).SelectToken("content")?.ToString() ?? "");
        if (textData == null || !textData.Any())
        {
            return res;
        }

        var ids = textData.Where(p => !string.IsNullOrEmpty(p.Attrs?.Id)).Select(p => Guid.Parse(p.Attrs?.Id + "")).ToList();

        var comments = _context.Available<StorySubPostComment>(false);
        var result = from comment in comments
                     where ids.Contains(comment.ParagraphId.Value)
                     join reply in comments on comment.Id equals reply.ParentId into joined
                     from reply in joined.DefaultIfEmpty()
                     group new { comment, reply } by comment.ParagraphId into g
                     select new
                     {
                         ParagraphId = g.Key,
                         TotalComments = g.Select(x => x.comment.Id).Distinct().Count() +
                                         g.Select(x => x.reply.Id).Distinct().Count()
                     };

        var commentCounts = result.ToList();

        var data = textData.Select(p =>
        {
            var firstContent = p.Content?.FirstOrDefault();
            var attr = p.Attrs;

            return new
            {
                Type = p.Type,
                Text = firstContent?.Text ?? "",
                Id = attr?.Id,
                Level = attr?.Level,
                TextAlign = attr?.TextAlign,
                Marks = firstContent?.Marks?.Select(m => m.Type).ToList() ?? [],
                CommentCount = attr?.Id != null
                    ? commentCounts.FirstOrDefault(c => c.ParagraphId == Guid.Parse(attr.Id))?.TotalComments ?? 0
                    : 0
            };
        }).ToList();

        return res.SetSuccess(data);
    }

    public class ChildrenObjectData
    {
        public string? Type { get; set; }
        public Attrs? Attrs { get; set; }
        public List<Content>? Content { get; set; }
    }

    public class Attrs
    {
        public string? Id { get; set; }
        public string? TextAlign { get; set; }
        public int? Level { get; set; }
    }

    public class Content
    {
        public string? Type { get; set; }
        public string? Text { get; set; }
        public List<Mark>? Marks { get; set; }
    }

    public class Mark
    {
        public string? Type { get; set; }
    }

    #endregion
}
