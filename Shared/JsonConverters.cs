using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace TinderButForBartering;

public class ObservableCollectionConverter<T> : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ObservableCollection<T>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JArray array = JArray.Load(reader);
        ObservableCollection<T> collection = new ObservableCollection<T>();
        foreach (JToken token in array)
        {
            T item = token.ToObject<T>(serializer);
            collection.Add(item);
        }

        return collection;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        ObservableCollection<T> collection = (ObservableCollection<T>)value;
        JArray array = new JArray();
        foreach (T item in collection)
        {
            JToken token = JToken.FromObject(item, serializer);
            array.Add(token);
        }

        array.WriteTo(writer);
    }
}