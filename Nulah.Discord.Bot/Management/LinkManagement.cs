using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Nulah.Discord.Bot.Management {
    public class LinkManagement {
        private readonly string _mssqlConnectionString;
        private readonly Regex _uriRegex = new Regex(@"(https?:\/\/[^\s]+)");
        private readonly HttpClient _httpClient = new HttpClient();

        public LinkManagement(string mssqlConnectionString) {
            _mssqlConnectionString = mssqlConnectionString;
        }

        public void TryParseForLinks(string content) {
            if(MessageContainsLinks(content) == true) {
                ExtractLinks(content);
            }
        }

        private bool MessageContainsLinks(string message) {
            return _uriRegex.IsMatch(message);
        }

        private List<string> ExtractLinks(string message) {
            try {
                var matches = _uriRegex.Matches(message).Select(x => new Uri(x.Value)).ToList();
                foreach(var uri in matches) {
                    var req = _httpClient.GetStringAsync(uri).Result;
                    var doc = new HtmlDocument();
                    doc.LoadHtml(req);
                    var article = doc.DocumentNode.SelectNodes("//article").FirstOrDefault();
                    var sections = article.Descendants().Where(x => x.Name == "section");
                    var tagsToKeep = new[] { "p", "figure" };
                    var usefulContent = new List<HtmlNode>();
                    foreach(var sec in sections) {
                        var content = sec.Descendants().Where(x => tagsToKeep.Contains(x.Name));
                        usefulContent.AddRange(content);
                    }
                    doc.LoadHtml($"<article>{ string.Join("", usefulContent.Select(x => x.OuterHtml.Trim()))}</article");
                }
            } catch {
                throw;
            }
            return new List<string>();
        }
    }
}
