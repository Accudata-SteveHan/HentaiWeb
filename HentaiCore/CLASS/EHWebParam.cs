using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HentaiCore {
    public class Book {
        public string gid {get;set;}

        public string token {get;set;}

        public string title {get;set;}

        public string title_jpn {get;set;}

        public string category {get;set;}

        public string thumb {get;set;}

        public string posted {get;set;}

        public string filecount {get;set;}

        public string rating {get;set;}

    }

    public static class EHWebParam {
        public static string GetBookDataJson(List<KeyValuePair<int, string>> book) {
            JArray bookArr = null;
            JArray array = new JArray();

            for (int i = 0 ; i < book.Count ; i++) {
                KeyValuePair<int, string> b = book[i];

                bookArr = new JArray();
                bookArr.Add(b.Key);
                bookArr.Add(b.Value);
                array.Add(bookArr);

            }

            Json json = new Json();
            json.Add("method", "gdata");
            json.Add("namespace", 1);
            json.Add("gidlist", array);

            return json.ToString();

        }

        public static List<Book> ToBookDataObject(string resultStr) {
            List<Book> result = new List<Book>();

            Json json = new Json(resultStr);
            JToken arr = json.jsonObject["gmetadata"];
            if (arr.GetType() == typeof(JArray)) {
                foreach (JToken token in (arr as JArray)) {
                    Book book = JsonConvert.DeserializeObject<Book>(JsonConvert.SerializeObject(token));
                    result.Add(book);

                }

            }

            return result;

        }

    }

}
