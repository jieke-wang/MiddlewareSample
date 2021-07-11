using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MiddlewareSample.Middlewares.Helper {
    public class JsonSerializerHelper {
        private static readonly JsonSerializerOptions _serializerOptions;

        static JsonSerializerHelper () {
            _serializerOptions = new JsonSerializerOptions {
                // Encoder = JavaScriptEncoder.Create (UnicodeRanges.All)
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        public static string Serialize<T> (T value) {
            return JsonSerializer.Serialize (value, _serializerOptions);
        }
    }
}