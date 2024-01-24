using GmWeb.Logic.Services.Deltas;
using GmWeb.Logic.Data.Models.Carma;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Context.Carma;

#region Thread Model Configuration

public partial class CarmaContext
{
    protected override EntityTypeBuilder<Thread> OnThreadsCreating(ModelBuilder modelBuilder, IConfiguration config)
    {
        var entityBuilder = base.OnThreadsCreating(modelBuilder, config);
        entityBuilder.ConfigureDeltaModified(x => x.ContentModified);
        entityBuilder.ConfigureDeltaValue(x => x.Content);
        entityBuilder.ConfigureDeltaModified(x => x.TitleModified);
        entityBuilder.ConfigureDeltaValue(x => x.Title);
        entityBuilder.ConfigureBitValueColumn(x => x.IsFlagged);

        return entityBuilder;
    }

    protected override EntityTypeBuilder<Thread> AfterThreadsCreating(ModelBuilder modelBuilder, IConfiguration config)
    {
        var entityBuilder = base.AfterThreadsCreating(modelBuilder, config);
        entityBuilder.HasMany(x => x.Comments).WithOne(x => x.Thread).OnDelete(DeleteBehavior.Cascade);
        return entityBuilder;
    }
}

#endregion

public partial class Threads
{
    public async Task<int> DeleteAsync(Guid threadID)
    {
        int count = await this.CountAsync(x => x.ThreadID == threadID);
        this.EntitySet.RemoveAll(x => x.ThreadID == threadID);
        return count;
    }

    #region Note Conversions
    public Expression<Func<Note, Thread>> ConvertNoteToThread
        => (Note note) => new Thread
        {
            ThreadID = note.NoteID,
            AuthorID = note.AuthorID,
            Author = note.NoteAuthor,
            AgencyID = note.NoteAuthor.AgencyID,
            SubjectID = note.SubjectID,
            Subject = note.NoteSubject,
            Content = note.Message,
            ContentModified = note.Modified,
            Title = note.Title,
            TitleModified = note.Modified,
            Created = note.Created,
            IsFlagged = note.IsFlagged
        };
    public List<Thread> ConvertFromNotes(IEnumerable<Note> notes)
    {
        var threads = notes.Select(this.ConvertNoteToThread.Compile()).ToList();
        threads.ForEach(thread =>
        {
            this.DataContext.DeltaService.UpdateDeltaFields(thread);
        });
        return threads;
    }

    public async Task<List<Thread>> ConvertFromNotes()
    {
        var notes = await this.DataContext.Notes
            .Include(n => n.NoteAuthor)
            .Include(n => n.NoteSubject)
            .ToListAsync()
        ;
        return this.ConvertFromNotes(notes);
    }

    public Expression<Func<Thread, Note>> ConvertThreadToNote
        => (Thread thread) => new Note
        {
            NoteID = thread.ThreadID,
            AuthorID = thread.AuthorID,
            NoteAuthor = thread.Author,
            SubjectID = thread.SubjectID,
            NoteSubject = thread.Subject,
            Message = thread.Content,
            Modified = thread.ContentModified,
            Title = thread.Title,
            Created = thread.Created,
            IsFlagged = thread.IsFlagged
        };

    public List<Note> ConvertToNotes(IEnumerable<Thread> threads)
    {
        var notes = threads.Select(this.ConvertThreadToNote.Compile()).ToList();
        return notes;
    }
    public async Task<List<Note>> ConvertToNotes()
    {
        var threads = await this.DataContext.Threads
            .Include(t => t.Comments.OrderBy(c => c.Created).Where(c => !c.ParentCommentID.HasValue))
            .ToListAsync()
        ;
        return this.ConvertToNotes(threads);
    }
    #endregion

    public Thread Insert(Thread thread, User author)
    {
        thread.ThreadID = Guid.NewGuid();
        thread.AuthorID = author.UserID;
        thread.AgencyID = author.AgencyID;
        thread.Created = DateTimeOffset.Now;
        this.DataContext.DeltaService.UpdateDeltaFields(thread);
        return this.Insert(thread);
    }

    public override EntityEntry<Thread> Update(Thread thread)
    {
        this.DataContext.DeltaService.UpdateDeltaFields(thread);
        return base.Update(thread);
    }
}
