using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Identity.DTO;
using GmWeb.Logic.Utility.Primitives;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class Users
    {
        public async Task<int> DeleteAsync(User user) => await this.DeleteAsync(user.UserID);
        public async Task<int> DeleteAsync(int userID)
        {
            int count = await this.CountAsync(x => x.UserID == userID);
            this.EntitySet.RemoveAll(x => x.UserID == userID);
            return count;
        }
        public async Task<int> DeleteAsync(Guid accountId) => await this.DeleteClientAsync(accountId);
        public async Task<int> DeleteClientAsync(Guid accountId)
        {
            int count = await this.CountAsync(c => c.AccountID == accountId);
            if (count > 0)
                this.EntitySet.RemoveAll(c => c.AccountID == accountId);
            return count;
        }
        public async Task<User> GetClientAsync(Guid accountID)
        {
            var client = await this.SingleOrDefaultAsync(x => x.AccountID == accountID);
            if (client == null)
                throw new ArgumentException($"No client found with AccountID {accountID}");
            if (client.UserRole != UserRole.Client)
                throw new ArgumentException($"Invalid user role '{client.UserRole} for User {client.UserID}");
            return client;
        }
        public async Task<User> LookupClientAsync(Guid lookupID)
        {
            var client = await this.SingleOrDefaultAsync(x => x.LookupID == lookupID);
            if (client == null)
                throw new ArgumentException($"No client found with LookupID {lookupID}");
            if (client.UserRole != UserRole.Client)
                throw new ArgumentException($"Invalid user role '{client.UserRole} for User {client.UserID}");
            return client;
        }

        public IQueryable<User> GetAgencyClients(int agencyID)
        {
            var context = this.DataContext.Users
                .Include(x => x.ParentConfigs)
            ;
            var clients = context
                .Where(x => x.AgencyID == agencyID)
                .Where(x => x.UserRole == UserRole.Client)
            ;
            return clients;
        }
        public IQueryable<User> GetAgencyClients(User viewer)
            => this.GetAgencyClients(viewer, filter: null);
        public IQueryable<User> GetAgencyClients(User viewer, ViewerFilter<UserConfigStatus> filter)
        {
            var clients = this.GetAgencyClients(viewer.AgencyID);
            if (filter?.Include.HasFlag(UserConfigStatus.Starred) == true)
                clients = clients
                    .Where(x => x.ParentConfigs
                        .Where(x => x.OwnerID == viewer.UserID)
                        .Where(x => (x.Status & UserConfigStatus.Starred) == UserConfigStatus.Starred)
                        .Any()
                    )
                ;
            if(filter?.Exclude.HasFlag(UserConfigStatus.Starred) == true)
                clients = clients
                    .Where(x => !x.ParentConfigs
                        .Where(x => x.OwnerID == viewer.UserID)
                        .Where(x => (x.Status & UserConfigStatus.Starred) == UserConfigStatus.Starred)
                        .Any()
                    )
                ;
            return clients;
        }

        public async Task<List<User>> LookupUsersAsync(IEnumerable<LookupUserDTO> lookups)
        {
            var lookupSet = lookups.Where(x => x.LookupID.HasValue).Select(x => x.LookupID).ToHashSet();
            var emailSet = lookups.Where(x => !string.IsNullOrWhiteSpace(x.Email)).Select(x => x.Email).ToHashSet();
            var idSet = lookups.Where(x => x.UserID.HasValue).Select(x => x.UserID).ToHashSet();
            var query = this.EntitySet.Where(
                user
                    => lookupSet.Contains(user.LookupID)
                    || emailSet.Contains(user.Email)
                    || idSet.Contains(user.UserID)
            );
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<User> LookupUserAsync(LookupUserDTO lookup)
        {
            IQueryable<User> query = this.Cache.DataContext.Users
                .Include(x => x.RegisteredActivities)
                .ThenInclude(ca => ca.ActivityCalendar)
                .Include(x => x.ParentNotes)
                .Include(x => x.ParentConfigs)
            ;

            if (lookup.AccountID.HasValue)
                query = query.Where(x => x.AccountID == lookup.AccountID);
            else if (lookup.LookupID.HasValue)
                query = query.Where(x => x.LookupID == lookup.LookupID);
            else if (lookup.UserID.HasValue)
                query = query.Where(x => x.UserID == lookup.UserID);
            else if (!string.IsNullOrWhiteSpace(lookup.Email))
                query = query.Where(x => x.Email == lookup.Email);
            else
                return null;
            var client = await query.SingleOrDefaultAsync();
            return client;
        }

        public async Task SetStarred(User child, User owner, bool isStarred)
        {
            UserConfig config;
            if(child.ParentConfigs == null)
            {
                var childEntry = this.Entry(child);
                var configsEntry = childEntry.Collection(x => x.ParentConfigs);
                await configsEntry.LoadAsync();
                config = configsEntry.CurrentValue.SingleOrDefault(x => x.OwnerID == owner.UserID);
            }
            else
                config = child.ParentConfigs
                    .Where(x => x.OwnerID == owner.UserID)
                    .SingleOrDefault()
                ;
            if(config == null && !isStarred)
                return;
            config ??= this.Cache.UserConfigs.Create(x =>
            {
                x.ConfigOwner = owner;
                x.ConfigSubject = child;
            });
            config.IsStarred = isStarred;
        }
    }
}
