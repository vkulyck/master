using GmWeb.Logic.Services.Deltas;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Utility.Performance.Paging;
using GmWeb.Web.Api.Models.Common;
using GmWeb.Web.Api.Utility;
using GmWeb.Web.Api.Utility.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GmWeb.Web.Api.Controllers.Common;

/// <summary>
/// The primary CRUD/view controller for notes.
/// </summary>
/// 
public class NoteController : ThreadController
{
    public NoteController(CarmaContext context, UserManager<GmIdentity> manager, IWebHostEnvironment env)
        : base(context, manager, env)
    { }

    /// <summary>
    /// Get the details of the specified Note.
    /// </summary>
    /// <returns></returns>
    [HttpGet("{NoteID}")]
    [UseSuccessModel(typeof(NoteDetailsDTO))]
    [UseBadRequestModel]
    public override async Task<IActionResult> Details([FromRoute] Guid NoteID)
        => await base.Details(NoteID);

    /// <summary>
    /// Get the list of every Note whose subject matches the provided user.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ActionName("query")]
    [UseSuccessModel(typeof(IEnumerable<NoteDTO>))]
    [UseBadRequestModel]
    public override async Task<IActionResult> Query([FromQuery] PagedListRequest request, int? SubjectID, int? AuthorID, bool Flagged)
        => await base.Query(request, SubjectID, AuthorID, Flagged);

    /// <summary>
    /// Insert a new Note.
    /// </summary>
    /// <param name="Note"></param>
    /// <returns></returns>
    [HttpPost]
    [UseSuccessModel(typeof(NoteDetailsDTO))]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public override async Task<IActionResult> Insert([FromBody] ThreadInsertDTO Note)
        => await base.Insert(Note);
    /// <summary>
    /// Update a Note.
    /// </summary>
    /// <param name="Note"></param>
    /// <returns></returns>
    [HttpPut]
    [UseSuccessModel(typeof(NoteDetailsDTO))]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public override async Task<IActionResult> Update([FromBody] ThreadUpdateDTO Note)
        => await base.Update(Note);

    /// <summary>
    /// Delete a client
    /// </summary>
    /// <param name="NoteID">User account ID.</param>
    /// <returns></returns>
    [HttpDelete("{NoteID}")]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public override async Task<IActionResult> Delete([FromRoute] Guid NoteID)
        => await base.Delete(NoteID);
}
