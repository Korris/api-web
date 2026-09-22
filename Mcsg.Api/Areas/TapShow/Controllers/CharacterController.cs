using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.TapShow.Controllers;

using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Characters (name + avatar) of a TapShow post (api/tapshow/character). Owner manages them; anyone who can see the post can list them.
/// </summary>
[ApiController]
[Route("api/tapshow/[controller]")]
public class CharacterController : ControllerBase
{
    #region -- Methods --

    public CharacterController(ITapShowCharacterService characterService)
    {
        _characterService = characterService;
    }

    /// <summary>
    /// Characters of a post ordered by Order
    /// </summary>
    [HttpGet("post/{postHashId}")]
    public async Task<IActionResult> ListByPost(string postHashId)
    {
        var request = new PostHashIdR { PostHashId = postHashId };
        request.Analyze(HttpContext);
        return Ok(await _characterService.ListByPostAsync(request));
    }

    /// <summary>
    /// Create a character. AvatarHashId (optional) must come from api/tapshow/file/upload-media.
    /// </summary>
    [HttpPost, Authorize]
    public async Task<IActionResult> Create([FromBody] CharacterCreateR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _characterService.CreateAsync(request));
    }

    [HttpPut("{id}"), Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] CharacterUpdateR request)
    {
        request.Analyze(HttpContext);
        request.Id = id;
        return Ok(await _characterService.UpdateAsync(request));
    }

    /// <summary>
    /// Soft-delete the character; segments referencing it keep their content without a speaker
    /// </summary>
    [HttpDelete("{id}"), Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var request = new CharacterUpdateR();
        request.Analyze(HttpContext);
        return Ok(await _characterService.DeleteAsync(id, request.UserId));
    }

    #endregion

    #region -- Fields --

    private readonly ITapShowCharacterService _characterService;

    #endregion
}
