using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Web.Api.Models.Common;
using GmWeb.Web.Common.Models;
using GmWeb.Tests.Api.Mocking;


namespace GmWeb.Tests.Api.Tests
{
    [Collection(nameof(ControllerTestCollection))]
    public class ThreadControllerTests : ControllerTestBase<UserControllerTests>
    {
        public ThreadControllerTests(TestApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task ValidateEvolvingContent()
        {
            var cache = new CarmaCache(this.ComCtx);
            var thread = await cache.Notes.Select(cache.Threads.ConvertNoteToThread).FirstOrDefaultAsync();
            var content = thread.Content;
            var json = JsonConvert.SerializeObject(content, Formatting.Indented);
            Assert.NotEmpty(json);
        }

        protected void CompareThreadToNote(ThreadDetailsDTO thread, NoteDetailsDTO note)
        {
            Assert.Equal(thread.ThreadID, note.NoteID);
            Assert.Equal(thread.Message, note.Message);
            Assert.Equal(thread.Title, note.Title);
            Assert.Equal(thread.Created, note.Created);
            Assert.Equal(thread.Modified, note.Modified);
            Assert.Equal(thread.AuthorID, note.AuthorID);
            Assert.Equal(thread.SubjectID, note.SubjectID);
            Assert.Equal(thread.IsFlagged, note.IsFlagged);
        }

        [Fact]
        public async Task ValidateNoteToThreadConversionEndpoint()
        {
            var cache = new CarmaCache(this.ComCtx);
            var deletionThreads = await cache.Threads.ToListAsync();
            await this.ComCtx.BulkDeleteAsync(deletionThreads);
            var noteDetails = this.Mapper.Map<NoteDetailsDTO>(cache.Notes).ToList();
            var responseThreads = await this.RequestDataAsync<List<ThreadDetailsDTO>>(
                Controller: "Thread", Action: "Convert-All-Notes", Method: HttpMethod.Post,
                ExpectedStatus: HttpStatusCode.OK
            );
            var responseThreadMap = responseThreads.ToDictionary(x => x.ThreadID, x => x);
            foreach (var note in noteDetails)
            {
                Assert.True(responseThreadMap.TryGetValue(note.NoteID, out ThreadDetailsDTO thread));
                this.CompareThreadToNote(thread, note);
            }
            this.ComCtx.ChangeTracker.Clear();
            var refreshedThreadMap = this.Mapper.Map<ThreadDetailsDTO>(cache.Threads).ToDictionary(x => x.ThreadID, x => x);
            foreach (var note in noteDetails)
            {
                Assert.True(refreshedThreadMap.TryGetValue(note.NoteID, out ThreadDetailsDTO thread));
                this.CompareThreadToNote(thread, note);
            }
        }

        [Fact]
        public async Task ValidateNoteToThreadConversionLogic()
        {
            var cache = new CarmaCache(this.ComCtx);
            var sourceNotes = await cache.Notes.ToListAsync();
            var newNotes = sourceNotes.Select(source =>
            {
                var newNote = new Note
                {
                    NoteID = Guid.NewGuid(),
                    Title = $"COPY:{source.Title}",
                    Message = $"COPY:{source.Message}",
                    Created = DateTimeOffset.Now,
                    Modified = null,
                    NoteAuthor = source.NoteAuthor,
                    NoteSubject = source.NoteSubject,
                    ModifiedBy = source.ModifiedBy,
                    IsFlagged = true
                };
                return newNote;
            }).ToList();
            cache.Notes.AddRange(newNotes);
            await cache.SaveAsync();
            
            var threads = cache.Threads.ConvertFromNotes(newNotes);
            cache.Threads.AddRange(threads);
            await cache.Threads.SaveAsync();

            var convertedNotes = cache.Threads.ConvertToNotes(threads);
            Assert.Equal(sourceNotes.Count, convertedNotes.Count);
            for (int i = 0; i < newNotes.Count; i++)
            {
                var newNote = newNotes[i];
                var convNote = convertedNotes[i];
                var thread = threads[i];
                Assert.Equal(newNote.Message, thread.Content);
                Assert.True(newNote.Created - thread.Created < TimeSpan.FromMilliseconds(1));
                Assert.Null(newNote.Modified);
                Assert.Null(thread.ContentModified);
                Assert.Equal(newNote.Message, convNote.Message);
            }
        }

        [Fact]
        public async Task ValidateThreadCRUD()
        {
            var insertDTO = new ThreadInsertDTO
            {
                SubjectID = this.Entities.MemberClient.UserID,
                Title = this.Faker.Company.CatchPhrase().ToUpper(),
                Message = @"Morbi mi turpis, tempus sed ultrices vel, finibus sit amet dolor. Integer sed dui id nisl pellentesque sagittis. Pellentesque convallis sit amet mauris sit amet dapibus. Nulla placerat et est tempus posuere. Integer tincidunt tortor tellus, at dapibus velit vulputate sodales. Suspendisse sit amet lectus augue. Cras eu sem mattis felis aliquam vehicula. Etiam placerat et ipsum eget porta."
            };
            var insertResult = await this.RequestDataAsync<ThreadDetailsDTO>(
                Controller: "Thread", Action: "Insert", Method: HttpMethod.Post,
                RequestData: insertDTO,
                ExpectedStatus: HttpStatusCode.OK
            );
            Assert.Equal(insertDTO.IsFlagged ?? false, insertResult.IsFlagged);
            Assert.Equal(insertDTO.SubjectID, insertResult.SubjectID);
            Assert.False(insertResult.Modified.HasValue);
            Assert.InRange(insertResult.Created, DateTimeOffset.Now.AddMinutes(-1.0), DateTimeOffset.Now);

            var searchWord = "sit";
            var replaceWord = "REDACTED";
            var updateDTO = new ThreadUpdateDTO
            {
                SubjectID = this.Entities.AdminStaffer.UserID,
                IsFlagged = true,
                Message = insertDTO.Message.Replace(searchWord, replaceWord),
                Title = insertDTO.Title.ToLower(),
                ThreadID = insertResult.ThreadID
            };
            var updateResult = await this.RequestDataAsync<ThreadDetailsDTO>(
                Controller: "Thread", Action: "Update", Method: HttpMethod.Put,
                RequestData: updateDTO,
                ExpectedStatus: HttpStatusCode.OK
            );
            Assert.Equal(updateDTO.IsFlagged, updateResult.IsFlagged);
            Assert.Equal(updateDTO.SubjectID, updateResult.SubjectID);
            Assert.True(updateResult.Modified.HasValue);
            Assert.InRange(updateResult.Modified.Value, DateTimeOffset.Now.AddMinutes(-1.0), DateTimeOffset.Now);
            Assert.True(updateResult.Modified.Value > updateResult.Created);

            var readResult = await this.RequestDataAsync<ThreadDetailsDTO>(
                Controller: "Thread", Action: $"Details/{insertResult.ThreadID}", Method: HttpMethod.Get,
                ExpectedStatus: HttpStatusCode.OK
            );

            Assert.Equal(updateDTO.SubjectID, readResult.SubjectID);
            Assert.Equal(updateDTO.IsFlagged, readResult.IsFlagged);
            Assert.Equal(readResult.Modified, updateResult.Modified);

            var searchCount = Regex.Matches(insertResult.Message, searchWord).Count;
            var replCount = Regex.Matches(readResult.Message, replaceWord).Count;
            Assert.Equal(searchCount, replCount);

            Assert.String.IsUpperCase(insertResult.Title);
            Assert.String.IsLowerCase(updateResult.Title);
            Assert.String.IsLowerCase(readResult.Title);

            var reupdateDTO = new ThreadUpdateDTO
            {
                ThreadID = insertResult.ThreadID,
                Message = updateResult.Message.Replace(replaceWord, searchWord)
            };
            var reupdateResult = await this.RequestDataAsync<ThreadDetailsDTO>(
                Controller: "Thread", Action: "Update", Method: HttpMethod.Put,
                RequestData: reupdateDTO,
                ExpectedStatus: HttpStatusCode.OK
            );
            Assert.Equal(updateDTO.IsFlagged, reupdateResult.IsFlagged);
            Assert.Equal(updateDTO.SubjectID, reupdateResult.SubjectID);
            Assert.True(reupdateResult.Modified.HasValue);
            Assert.InRange(reupdateResult.Modified.Value, DateTimeOffset.Now.AddMinutes(-1.0), DateTimeOffset.Now);
            Assert.True(reupdateResult.Modified.Value > updateResult.Modified.Value);

            await this.RequestDataAsync<ThreadDetailsDTO>(
                Controller: "Thread", Action: $"Delete/{readResult.ThreadID}", Method: HttpMethod.Delete,
                ExpectedStatus: HttpStatusCode.OK
            );

            var postDeleteReadResult = await this.RequestDataAsync<ThreadDetailsDTO>(
                Controller: "Thread", Action: $"Details/{readResult.ThreadID}", Method: HttpMethod.Get,
                ExpectedStatus: HttpStatusCode.NotFound
            );

            Assert.Null(postDeleteReadResult);
        }
    }
}
