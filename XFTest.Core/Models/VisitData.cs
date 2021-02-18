using System;
using System.Collections.Generic;

namespace XFTest.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using XFTest.Core.Helpers;

    public partial class VisitData
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public ObservableCollection<Visits> Data { get; set; }

        [JsonProperty("code")]
        public long Code { get; set; }
    }

    public partial class Visits
    {
        [JsonProperty("visitId")]
        public Guid VisitId { get; set; }

        [JsonProperty("homeBobEmployeeId")]
        public Guid HomeBobEmployeeId { get; set; }

        [JsonProperty("houseOwnerId")]
        public Guid HouseOwnerId { get; set; }

        [JsonProperty("isBlocked")]
        public bool IsBlocked { get; set; }

        [JsonProperty("startTimeUtc")]
        public DateTimeOffset StartTimeUtc { get; set; }

        [JsonProperty("endTimeUtc")]
        public DateTimeOffset EndTimeUtc { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("isReviewed")]
        public bool IsReviewed { get; set; }

        [JsonProperty("isFirstVisit")]
        public bool IsFirstVisit { get; set; }

        [JsonProperty("isManual")]
        public bool IsManual { get; set; }

        [JsonProperty("visitTimeUsed")]
        public long VisitTimeUsed { get; set; }

        [JsonProperty("rememberToday")]
        public object RememberToday { get; set; }

        [JsonProperty("houseOwnerFirstName")]
        public string HouseOwnerFirstName { get; set; }

        [JsonProperty("houseOwnerLastName")]
        public string HouseOwnerLastName { get; set; }

        //Get House Owner Full Name
        public string HouseOwnerFullName
        {
            get {
                return HouseOwnerFirstName + " " + HouseOwnerLastName;
            }
        }

        [JsonProperty("houseOwnerMobilePhone")]
        public string HouseOwnerMobilePhone { get; set; }

        [JsonProperty("houseOwnerAddress")]
        public string HouseOwnerAddress { get; set; }

        [JsonProperty("houseOwnerZip")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long HouseOwnerZip { get; set; }

        [JsonProperty("houseOwnerCity")]
        public string HouseOwnerCity { get; set; }

        //Get House Owner Full Address
        public string HouseOwnerFullAddress
        {
            get
            {
                return HouseOwnerAddress + " ," + HouseOwnerZip + " " + HouseOwnerCity;
            }
        }

        [JsonProperty("houseOwnerLatitude")]
        public double HouseOwnerLatitude { get; set; }

        [JsonProperty("houseOwnerLongitude")]
        public double HouseOwnerLongitude { get; set; }

        public double PrevHouseOwnerLatitude { get; set; }

        public double PrevHouseOwnerLongitude { get; set; }

        [JsonProperty("isSubscriber")]
        public bool IsSubscriber { get; set; }

        [JsonProperty("rememberAlways")]
        public object RememberAlways { get; set; }

        [JsonProperty("professional")]
        public string Professional { get; set; }

        [JsonProperty("visitState")]
        public string VisitState { get; set; }

        //Set Visit State Color based on VisitState Status
        public string VisitStatusColor {
            get { return (VisitState == "ToDo" ? "#4E77D6" : (VisitState == "InProgress" ? "#ffea00" : "#008000")); }
        }

        [JsonProperty("stateOrder")]
        public long StateOrder { get; set; }

        [JsonProperty("expectedTime")]
        public string ExpectedTime { get; set; }

        [JsonProperty("tasks")]
        public List<Task> Tasks { get; set; }

        public string TaskName
        { 
            get {
                return Tasks.Count > 0 ? String.Join("/", Tasks.Select(x => x.Title).ToList()) : "";
            }
        }

        public string TaskDuration {
            get { return Tasks.Sum(item => item.TimesInMinutes).ToString()+" "+"min"; }
        }

        //Set Task Time Range
        public string TaskTimeRange
        {
            get { return StartTimeUtc.ToString("HH:mm") + " / " + EndTimeUtc.ToString("HH:mm"); }
        }

        //Calculate distance between previous visit and current visit
        public string DistanceDiff
        {
            get { return (PrevHouseOwnerLatitude!=0? Math.Round(CalculateDistance.DistanceBetweenPlaces(HouseOwnerLatitude, HouseOwnerLongitude, PrevHouseOwnerLatitude, PrevHouseOwnerLongitude),1).ToString():"0")+" km"; }
        }

        [JsonProperty("houseOwnerAssets")]
        public List<object> HouseOwnerAssets { get; set; }

        [JsonProperty("visitAssets")]
        public List<object> VisitAssets { get; set; }

        [JsonProperty("visitMessages")]
        public List<object> VisitMessages { get; set; }
    }

    public partial class Task
    {
        [JsonProperty("taskId")]
        public Guid TaskId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("isTemplate")]
        public bool IsTemplate { get; set; }

        [JsonProperty("timesInMinutes")]
        public long TimesInMinutes { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("paymentTypeId")]
        public Guid PaymentTypeId { get; set; }

        [JsonProperty("createDateUtc")]
        public DateTimeOffset CreateDateUtc { get; set; }

        [JsonProperty("lastUpdateDateUtc")]
        public DateTimeOffset LastUpdateDateUtc { get; set; }

        [JsonProperty("paymentTypes")]
        public object PaymentTypes { get; set; }
    }

    public partial class VisitData
    {
        public static VisitData FromJson(string json) => JsonConvert.DeserializeObject<VisitData>(json, XFTest.Core.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this VisitData self) => JsonConvert.SerializeObject(self, XFTest.Core.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

}
