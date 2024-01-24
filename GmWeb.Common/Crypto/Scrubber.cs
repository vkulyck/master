using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace GmWeb.Common.Crypto
{
    public class Scrubber
    {

        #region Configuration
        public const string DefaultHeaders = "UserPassword;AgencyPassword;`.*Password`";
        public string HeaderConfig { get; protected set; }
        public char PasswordReplacementCharacter { get; protected set; }
        public int PasswordReplacementLength { get; protected set; }
        public string PasswordReplacement { get; protected set; }
        public Regex PasswordReplacementPattern { get; protected set; }
        #endregion

        #region Headers
        protected List<string> _PasswordFields { get; } = new List<string>();
        public IEnumerable<string> PasswordFields => this._PasswordFields;
        protected List<Regex> _PasswordPatterns { get; } = new List<Regex>();
        public IEnumerable<Regex> PasswordPatterns => this._PasswordPatterns;
        #endregion

        #region Initialization
        public Scrubber()
        {
            string headers = ConfigurationManager.AppSettings[$"GmWeb.Common.Crypto.Scrubber.PasswordHeaders"]?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(headers))
                headers = DefaultHeaders;
            this.HeaderConfig = headers;
            this.InitializeProperties();
            this.InitializeHeaders();
        }
        public Scrubber(string headers)
        {
            this.HeaderConfig = headers;
            this.InitializeProperties();
            this.InitializeHeaders();
        }

        protected virtual void InitializeProperties()
        {
            this.PasswordReplacementCharacter = '*';
            this.PasswordReplacementLength = 12;
            this.PasswordReplacement = new string(this.PasswordReplacementCharacter, this.PasswordReplacementLength);
            this.PasswordReplacementPattern = new Regex($@"^[\s{Regex.Escape(this.PasswordReplacementCharacter.ToString())}]+$");
        }
        protected virtual void InitializeHeaders()
        {
            this._PasswordFields.Clear();
            this._PasswordPatterns.Clear();
            if (string.IsNullOrWhiteSpace(this.HeaderConfig))
                return;

            string[] split = Regex.Split(this.HeaderConfig, @"\s*[,;]\s*");
            foreach (string field in split)
            {
                if (string.IsNullOrWhiteSpace(field))
                    continue;
                if (field.StartsWith("`") && field.EndsWith("`"))
                {
                    string pattern = field.Substring(1, field.Length - 2);
                    var regex = new Regex(pattern);
                    this._PasswordPatterns.Add(regex);
                }
                else
                    this._PasswordFields.Add(field);
            }
        }
        #endregion

        #region Utility
        protected IEnumerable<(DataRow, DataColumn)> GetPasswordCells(DataTable input)
        {
            foreach (DataRow row in input.Rows)
            {
                foreach (DataColumn column in input.Columns)
                {
                    if (!this.IsPasswordColumn(column))
                        continue;
                    yield return (row, column);
                }
            }
        }

        protected virtual bool IsPasswordColumn(DataColumn column)
        {
            foreach (string field in this.PasswordFields)
                if (column.ColumnName == field)
                    return true;
            foreach (var pattern in this.PasswordPatterns)
                if (pattern.IsMatch(column.ColumnName))
                    return true;
            return false;
        }
        #endregion

        #region Scrub/Hash

        public string Scrub(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return password?.Trim();
            return this.PasswordReplacement;
        }

        protected void Scrub(DataRow row, DataColumn column)
        {
            string password = row[column]?.ToString();
            string passview = this.Scrub(password);
            row[column] = passview;
        }

        public DataTable Scrub(DataTable input)
        {
            var sanitized = input.Copy();
            foreach (var (row, column) in this.GetPasswordCells(sanitized))
            {
                this.Scrub(row, column);
            }
            return sanitized;
        }

        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return password?.Trim();
            if (this.PasswordReplacementPattern.IsMatch(password))
                return null;
            string hashed = Hasher.HashPassword(password);
            return hashed;
        }

        protected void Hash(DataRow row, DataColumn column)
        {
            string password = row[column]?.ToString();
            string hashed = this.Hash(password);
            row[column] = hashed;
        }

        public DataTable Hash(DataTable input)
        {
            var hashed = input.Copy();
            foreach (var (row, column) in this.GetPasswordCells(hashed))
            {
                this.Hash(row, column);
            }
            return hashed;
        }
        #endregion
    }
}
