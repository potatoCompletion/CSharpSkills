using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSharpSkills.Common
{
    // JsonElement에 확장 메서드를 만들어서 null check을 간단하게 하고, 보다 간결하고 쉬운 예외처리를 할 수 있다.

    #region JsonElement 확장메서드

    public static class JsonElementExtenstion
    {
        public static JsonElement? GetPropertyExtension(this JsonElement jsonElement, string propertyName)
        {
            if (jsonElement.TryGetProperty(propertyName, out JsonElement returnElement))
            {
                return returnElement;
            }

            return null;
        }
    }

    #endregion

    internal class JsonElementExtension
    {
        public void someMethod(string jsonString)
        {
            var jsonDocument = JsonDocument.Parse(jsonString);
            var jsonElement = jsonDocument.RootElement.GetPropertyExtension("Key")
                ?.GetPropertyExtension("anotherKey") ?? throw new Exception("this value isn't exist");
        }
    }
}
