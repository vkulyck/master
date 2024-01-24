using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;
using System.Dynamic;
using Startup = GmWeb.Web.Api.Startup;
using GmWeb.Tests.Api.Mocking;
using GmWeb.Tests.Api.Extensions;
using GmWeb.Logic.Utility.Extensions.Http;

using Note = GmWeb.Logic.Data.Models.Carma.Note;
using NoteDetailsDTO = GmWeb.Web.Api.Models.Common.NoteDetailsDTO;
using NoteInsertDTO = GmWeb.Web.Api.Models.Common.NoteInsertDTO;
using NoteUpdateDTO = GmWeb.Web.Api.Models.Common.NoteUpdateDTO;

namespace GmWeb.Tests.Api.Tests
{
    [Collection(nameof(ControllerTestCollection))]
    public class NoteControllerTests : ControllerTestBase<NoteControllerTests>
    {
        public NoteControllerTests(TestApplicationFactory factory) : base(factory)
        {
        }

        protected void compareNotes(Note expected, Note actual)
        {
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Message, actual.Message);
            Assert.InRange(actual.Created, DateTimeOffset.Now.AddMinutes(-1), DateTimeOffset.Now);
            Assert.Equal(expected.AuthorID, actual.AuthorID);
            Assert.Equal(expected.SubjectID, actual.SubjectID);
        }
        [Fact]
        public async Task ValidateNoteDetails()
        {
            string badDetails = $"details/{Guid.NewGuid()}";
            await this.RequestDataAsync("Note", badDetails, HttpMethod.Get, ExpectedStatus: HttpStatusCode.NotFound);

            string okDetails = $"details/{this.Entities.AdminNote.NoteID}";
            var okNote = await this.RequestDataAsync<NoteDetailsDTO>("Note", okDetails, HttpMethod.Get, ExpectedStatus: HttpStatusCode.OK);
            var expected = this.Mapper.Map<Note>(this.Entities.AdminNote);
            var actual = this.Mapper.Map<Note>(okNote);
            compareNotes(expected, actual);
        }
        [Fact]
        public async Task ValidateNoteInsert()
        {
            var author = this.Entities.AdminStaffer;
            var requestNote = new NoteInsertDTO
            {
                SubjectID = this.Entities.MemberClient.UserID,
                Title = $"Title by {author.UserID}: {author.Email}",
                Message = $"Message by {author.UserID}: {author.Email}",
                IsFlagged = true
            };
            var expected = this.Mapper.Map<Note>(requestNote);
            expected.AuthorID = this.Entities.AdminStaffer.UserID;
            var responseNote = await this.RequestDataAsync<NoteDetailsDTO>("Note", "Insert", HttpMethod.Post, RequestData: requestNote, ExpectedStatus: HttpStatusCode.OK);
            var actual = this.Mapper.Map<Note>(responseNote);
            compareNotes(expected, actual);
        }
        [Fact]
        public async Task ValidateNoteUpdate()
        {
            var requestNote = new NoteUpdateDTO
            {
                NoteID = this.Entities.AdminNote.NoteID,
                SubjectID = this.Entities.AdminNote.SubjectID,
                Title = "ok title",
                Message = this.Entities.AdminNote.Message,
                IsFlagged = this.Entities.AdminNote.IsFlagged,
            };
            var expected = this.Mapper.Map<Note>(requestNote);
            var responseNote = await this.RequestDataAsync<NoteDetailsDTO>("Note", "Update", HttpMethod.Put, RequestData: requestNote, ExpectedStatus: HttpStatusCode.OK);
            expected.Created = responseNote.Created;
            expected.AuthorID = this.Entities.AdminStaffer.UserID;
            var actual = this.Mapper.Map<Note>(responseNote);
            compareNotes(expected, actual);
        }
        [Fact]
        public async Task ValidateDeleteNote()
        {
            string routeDetails = $"details/{this.Entities.AdminNote.NoteID}";
            string routeDelete = $"delete/{this.Entities.AdminNote.NoteID}";
            var note = await this.RequestDataAsync<NoteDetailsDTO>("Note", routeDetails, HttpMethod.Get, ExpectedStatus: HttpStatusCode.OK);
            await this.RequestDataAsync("Note", routeDelete, HttpMethod.Delete, ExpectedStatus: HttpStatusCode.OK);
            await this.RequestDataAsync("Note", "Details", HttpMethod.Get, RequestData: new { NoteID = note.NoteID }, ExpectedStatus: HttpStatusCode.NotFound);
        }
    }
}
