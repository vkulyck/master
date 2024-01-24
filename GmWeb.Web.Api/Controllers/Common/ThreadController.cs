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
/// The primary CRUD/view controller for threads.
/// </summary>
/// 
public class ThreadController : CarmaController, IDisposable
{
    public ThreadController(CarmaContext context, UserManager<GmIdentity> manager, IWebHostEnvironment env)
        : base(context, manager, env) { }

    /// <summary>
    /// Get the details of the specified thread.
    /// </summary>
    /// <returns></returns>
    [HttpGet("{threadID}")]
    [UseSuccessModel(typeof(ThreadDetailsDTO))]
    [UseBadRequestModel]
    public virtual async Task<IActionResult> Details([FromRoute] Guid threadID)
    {
        var ctx = this.Cache.DataContext.Threads
            .Include(x => x.Author)
            .Include(x => x.Subject)
        ;
        var thread = await ctx
            .Where(x => x.ThreadID == threadID)
            .SingleOrDefaultAsync()
            .ConfigureAwait(false)
        ;
        if (thread == null)
            return this.NotFound();
        var details = this.Mapper.Map<ThreadDetailsDTO>(thread);
        return this.Ok(details);
    }

    /// <summary>
    /// Get the list of every thread whose subject matches the provided user.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ActionName("query")]
    [UseSuccessModel(typeof(IEnumerable<ThreadDTO>))]
    [UseBadRequestModel]
    public virtual async Task<IActionResult> Query([FromQuery] PagedListRequest request, int? subjectID, int? authorID, bool flagged)
    {
        IQueryable<Thread> threads = this.Cache.Threads
            .Select(x => x) // DbSet<T> doesn't implement IAsyncEnumerable<T> because reasons. Performing a no-op select produces
                            // an IQueryable/IAsyncEnumerable result with the same data, so we'll use this hack until/unless the
                            // EFCore implementation is fixed.
        ;
        if (flagged)
            threads = threads.Where(x => x.IsFlagged == flagged);
        if (subjectID.HasValue)
            threads = threads.Where(x => x.SubjectID == subjectID);
        if (authorID.HasValue)
            threads = threads.Where(x => x.AuthorID == authorID);
        var result = await threads.ToPagedResultAsync(request).MapAsync<Thread, ThreadDTO>();
        return result;
    }

    /// <summary>
    /// Insert a new thread.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [UseSuccessModel(typeof(ThreadDetailsDTO))]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public virtual async Task<IActionResult> Insert([FromBody] ThreadInsertDTO model)
    {
        if (model == null)
            return this.MissingPostModel();

        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        var thread = this.Mapper.Map<Thread>(model);
        var author = await this.GetCarmaUser();

        var inserted = this.Cache.Threads.Insert(thread, author);
        await this.Cache.SaveAsync().ConfigureAwait(false);
        var details = this.Mapper.Map<ThreadDetailsDTO>(inserted);
        return this.Ok(details);
    }

    /// <summary>
    /// Update a thread.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [UseSuccessModel(typeof(ThreadDetailsDTO))]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public virtual async Task<IActionResult> Update([FromBody] ThreadUpdateDTO model)
    {
        if (model == null)
            return this.MissingPostModel();

        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        var thread = await this.Cache.Threads.SingleOrDefaultAsync(x => x.ThreadID == model.ThreadID);
        if (thread == null)
            return this.DataNotFound();
        this.Mapper.Map(model, thread);

        var updated = await this.Cache.Threads.UpdateSingleAsync<Thread,Guid>(thread).ConfigureAwait(false);
        await this.Cache.SaveAsync().ConfigureAwait(false);
        var details = this.Mapper.Map<ThreadDetailsDTO>(updated.Entity);
        return this.Ok(details);
    }

    /// <summary>
    /// Delete a client
    /// </summary>
    /// <param name="threadID">User account ID.</param>
    /// <returns></returns>
    [HttpDelete("{threadID}")]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public virtual async Task<IActionResult> Delete([FromRoute] Guid threadID)
    {
        await this.Cache.Threads.DeleteAsync(threadID);
        await this.Cache.SaveAsync().ConfigureAwait(false);
        return this.Success();
    }

    [HttpPost]
    [UseSuccessModel(typeof(List<ThreadDetailsDTO>))]
    public async Task<IActionResult> ConvertAllNotes()
    {
        var threads = await this.Cache.Threads.ConvertFromNotes();
        this.Cache.Threads.AddRange(threads);
        await this.Cache.SaveAsync();
        var mapped = this.Mapper.Map<ThreadDetailsDTO>(threads);
        return Ok(mapped);
    }
}
