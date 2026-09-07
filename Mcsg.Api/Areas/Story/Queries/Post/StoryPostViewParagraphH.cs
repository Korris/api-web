using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mcsg.Api.Areas.Story.Queries;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Story.Requests;
using Mcsg.Api.Areas.Story.Validators;
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
        var subPost = await _context.Available<StorySubPost>(false)
            .Where(p => p.Id == request.Id)
            .Select(p => new { p.Body, p.IsPremium, p.UserId })
            .FirstOrDefaultAsync(cancellationToken);

        if (subPost == null || subPost.Body == null)
        {
            return res;
        }

        var userId = request.UserId;

        if (subPost.IsPremium)
        {
            if (userId == null)
            {
                return res;
            }

            var isPremium = await _context.UserAvailable.Where(p => p.Id == userId).Select(p => p.IsPremium).FirstOrDefaultAsync();
            if (subPost.UserId != userId && isPremium != true && !request.IsAdministrator)
            {
                return res;
            }
        }

        var textData = JsonConvert.DeserializeObject<List<ChildrenObjectData>>(JObject.Parse(subPost.Body).SelectToken("content")?.ToString() ?? "");
        if (textData == null || textData.Count == 0)
        {
            return res;
        }

        var ids = GetIds(textData);

        var comments = _context.Available<StorySubPostComment>(false);
        var result = from comment in comments
                     where ids.Contains(comment.ParagraphId ?? Guid.Empty)
                     join reply in comments on comment.Id equals reply.ParentId into joined
                     from reply in joined.DefaultIfEmpty()
                     group new { comment, reply } by comment.ParagraphId into g
                     select new CommentCount
                     {
                         ParagraphId = g.Key,
                         TotalComments = g.Select(x => x.comment.Id).Distinct().Count() +
                                         g.Select(x => x.reply.Id).Distinct().Count()
                     };

        var commentCounts = result.ToList();

        var data = ConvertToParapraphData(textData, commentCounts);

        return res.SetSuccess(data);
    }

    private List<ParapraphData> ConvertToParapraphData(List<ChildrenObjectData> contents, List<CommentCount> commentCounts)
    {
        if (contents == null)
        {
            return [];
        }

        return contents.Select(content => new ParapraphData
        {
            Type = content.Type,
            Id = content.Attrs?.Id,
            Level = content.Attrs?.Level,
            TextAlign = content.Attrs?.TextAlign,
            Text = content.Text,
            Marks = content.Marks ?? [],
            Content = content.Content != null ? ConvertToParapraphData(content.Content, commentCounts) : [],
            CommentCount = !string.IsNullOrEmpty(content.Attrs?.Id) && Guid.TryParse(content.Attrs.Id, out Guid id)
                ? commentCounts.FirstOrDefault(c => c.ParagraphId == id)?.TotalComments ?? 0
                : 0
        }).ToList();
    }

    private List<Guid> GetIds(List<ChildrenObjectData> contents)
    {
        var ids = new List<Guid>();
        CollectIds(contents, ids);
        return ids;
    }

    private void CollectIds(List<ChildrenObjectData> contents, List<Guid> ids)
    {
        if (contents == null)
        {
            return;
        }

        foreach (var content in contents)
        {
            if (content?.Attrs?.Id != null && Guid.TryParse(content.Attrs.Id, out Guid id))
            {
                ids.Add(id);
            }

            if (content?.Content != null)
            {
                CollectIds(content.Content, ids);
            }
        }
    }

    #endregion

    #region -- Classes --

    public class CommentCount
    {
        public Guid? ParagraphId { get; set; }
        public int TotalComments { get; set; }
    }

    public class ParapraphData
    {
        public string? Type { get; set; }
        public string? Text { get; set; }
        public string? Id { get; set; }
        public int? Level { get; set; }
        public string? TextAlign { get; set; }
        public List<ParapraphData> Content { get; set; } = [];
        public List<Mark> Marks { get; set; } = [];
        public int CommentCount { get; set; }
    }

    public class ChildrenObjectData
    {
        public string? Type { get; set; }
        public Attrs? Attrs { get; set; }
        public List<ChildrenObjectData>? Content { get; set; } = [];
        public List<Mark>? Marks { get; set; } = [];
        public string? Text { get; set; }
    }

    public class Attrs
    {
        public string? Id { get; set; }
        public string? TextAlign { get; set; }
        public int? Level { get; set; }
    }

    public class Mark
    {
        public string? Type { get; set; }
        public MarkAttrs? Attrs { get; set; }
    }

    public class MarkAttrs
    {
        public string? href { get; set; }
        public string? target { get; set; }
    }

    #endregion
}
