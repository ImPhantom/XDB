using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using XDB.Common.Models;

namespace XDB.Services
{
    public class TagService
    {
        public Dictionary<string, string> FetchAllTags()
            => JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Xeno.TagsPath));

        public string FetchTagContentAsync(string tagName)
        {
            var tags = FetchAllTags();
            if (tags.TryGetValue(tagName.ToLower(), out string content))
                return content;
            else
                return null;
        }

        public async Task<bool> TryAddTagAsync(string tagName, string tagContent)
        {
            var tags = FetchAllTags();
            if (tags.TryGetValue(tagName, out string content))
                return false;
            tags.Add(tagName.ToLower(), tagContent);
            await Xeno.SaveJsonAsync(Xeno.TagsPath, JsonConvert.SerializeObject(tags));
            return true;
        }

        public async Task<bool> TryRemoveTagAsync(string tagName)
        {
            var tags = FetchAllTags();
            if(tags.TryGetValue(tagName, out string content))
            {
                tags.Remove(tagName);
                await Xeno.SaveJsonAsync(Xeno.TagsPath, JsonConvert.SerializeObject(tags));
                return true;
            } else
                return false;
        }

        public async Task<bool> TryEditTagAsync(string tagName, string newContent)
        {
            var tags = FetchAllTags();
            if(tags.TryGetValue(tagName, out string content))
            {
                tags[tagName] = newContent;
                await Xeno.SaveJsonAsync(Xeno.TagsPath, JsonConvert.SerializeObject(tags));
                return true;
            } else
                return false;
        }

        public void Initialize()
        {
            if (!File.Exists(Xeno.TagsPath))
            {
                Dictionary<string, string> tags = new Dictionary<string, string>();
                using (var file = new FileStream(Xeno.TagsPath, FileMode.Create)) { }
                File.WriteAllText(Xeno.TagsPath, JsonConvert.SerializeObject(tags));
            }
        }
    }
}
